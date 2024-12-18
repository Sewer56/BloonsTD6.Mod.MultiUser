# Multi User for Bloons TD 6

## Introduction

Multi User is a mod for Bloons TD 6 that allows you to run multiple copies of Bloons TD6 at once.

## Download

You can find the download for the mod in the [Releases Section on GitHub](https://github.com/Sewer56/BloonsTD6.Mod.MultiUser//releases/latest).  
Simply extract the DLL to your `Mods` directory.  

## Warning

⚠️ RISK OF DATA LOSS.

Before using this mod, consider creating a backup of your save data contained in:
- `Steam/userdata/<user-id>/960090`  

## Usage

In order to use an alternative profile:  
- Create a game shortcut.  
- Add desired flags (see below).  
- Launch game.  

![Example Usage](./docs/images/UsageExample.png)

In this example, adding `--profile Sewer56 --save Sewer56` will make the game use an alternative account associated with the name `Sewer56`.  When you boot into the game you'll be prompted to log-in. When you use the same shortcut in the future, you will be automatically logged into that account with its own separate save file.  

### List of Available Flags

`--profile ProfileName`: Specifies an alternative account to use.  
`--save SaveName`: Specifies an alternative save file to use.  

It is recommended to specify both `--profile` and `--save`; unless your intention is to share the same save file between multiple accounts (co-op with self).  

Note: *This mod will likely get you hacker pooled and (by design) cannot be used to circumvent the hacker pool.*  

## Features List

✅ indicates a feature is implemented.  
⚠️ indicates a feature is not yet complete.  
❌ indicates a feature is not yet implemented.   

- ✅ Multiple Instances of Game At Once.  
- ✅ Test Co-Op Mods without Another PC.  
- ✅ Run Multiple In-Game Profiles Concurrently.  

## Building

All source code is contained inside the `source` folder.  

### First Time Setup

- Install [.NET 6.0 SDK or newer](https://dotnet.microsoft.com/en-us/download).  

Compiled game mod should appear in the `Mods` directory in your game folder.

### Building from Command Line
- Open a terminal (cmd/powershell/bash) and navigate to the project folder.
- Run `dotnet build -c Release`.

### Building from Visual Studio
- Install Visual Studio 2022 (or Newer).
- Open `BloonsTD6.Mod.MultiUser.sln`.
