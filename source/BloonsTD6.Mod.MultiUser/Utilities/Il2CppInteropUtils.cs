using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Il2CppInterop.Runtime;

namespace BloonsTD6.Mod.MultiUser.Utilities;

// Copied from my fork of Il2CppAssemblyUnhollower

public class Il2CppInteropUtils
{
    private const string GenericDeclaringTypeName = "MethodInfoStoreGeneric_";
    private const string GenericFieldName = "Pointer";

    private static FieldInfo GetFieldInfoFromMethod(MethodBase method, string prefix, FieldType type = FieldType.None)
    {
        var body = method.GetMethodBody();
        if (body == null) throw new ArgumentException("Target method may not be abstract");
        var methodModule = method.DeclaringType.Assembly.Modules.Single();

        // get using reflection Il2CppInterop.Common.MiniIlParser.Decode from Il2CppInterop.Common.MiniIlParser but its an internal class
        IEnumerable<(OpCode, long)>? ilBody = (IEnumerable<(OpCode, long)>) AccessTools.Method(typeof(Il2CppInterop.Common.Il2CppInteropUtils).Assembly.GetType("Il2CppInterop.Common.MiniIlParser"), "Decode").Invoke(null,
            [body.GetILAsByteArray()]);

        foreach ((var opCode, long opArg) in ilBody)
        {
            if (opCode != OpCodes.Ldsfld) continue;
            var fieldInfo = methodModule.ResolveField((int)opArg);
            if (fieldInfo?.FieldType != typeof(IntPtr))
                continue;

            switch (type)
            {
                case FieldType.None:
                    if (fieldInfo.Name.StartsWith(prefix))
                        return fieldInfo;

                    break;

                case FieldType.GenericMethod:

                    if (fieldInfo.Name.Equals(GenericFieldName) &&
                        fieldInfo.DeclaringType.Name.StartsWith(GenericDeclaringTypeName))
                    {
                        var genericType = fieldInfo.DeclaringType.GetGenericTypeDefinition().MakeGenericType(method.GetGenericArguments());
                        return genericType.GetField(GenericFieldName, BindingFlags.NonPublic | BindingFlags.Static);
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        return null;
    }

    public static FieldInfo GetIl2CppMethodInfoPointerFieldForGeneratedMethod(MethodBase method)
    {
        const string prefix = "NativeMethodInfoPtr_";
        if (method.IsGenericMethod)
            return GetFieldInfoFromMethod(method, prefix, FieldType.GenericMethod);

        return GetFieldInfoFromMethod(method, prefix);
    }

    private enum FieldType
    {
        None,
        GenericMethod
    }
}