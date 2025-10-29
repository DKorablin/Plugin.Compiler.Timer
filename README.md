# Plugin.Compiler.Timer
[![Auto build](https://github.com/DKorablin/Plugin.Compiler.Timer/actions/workflows/release.yml/badge.svg)](https://github.com/DKorablin/Plugin.Compiler.Timer/releases/latest)

A GUI wrapper for the Timers Plugin and the .NET Compiler.

## What it is  
Plugin.Compiler.Timer provides a simple Windows-GUI front-end around a timer-plugin and the .NET compiler workflow. It allows you to launch and manage timed plugin executions within a .NET build / plugin environment.

## Why you might use it  
- If you are working with a plugin architecture where timed execution matters (e.g., scheduled tasks, periodic plugin runs)  
- If you want a visual interface rather than only command-line/time-based invocation  
- If you are leveraging the .NET compiler infrastructure and want integration with timer-based plugin activation  
- To simplify and unify the setup of timed plugin launches in a .NET context  

## Key features  
- GUI front-end for configuring and launching timer-powered plugin executions  
- Leverages the .NET compiler (so you can compile and run code/plugins)  
- Easily configurable timer options for plugin tasks  

## Requirements
- .NET Framework (or .NET Core/.NET 5+ depending on how the project is configured)
- Windows (GUI application)
- Visual Studio or equivalent for building from source
- Plugin architecture compatible with the “Timers Plugin” (this repo acts as a wrapper)

## Installation and Usage
To use the Plugin Compiler Timer Plugin, follow these steps:
1. Download the latest release from the [Releases](https://github.com/DKorablin/Plugin.Compiler/releases)
2. Extract the downloaded ZIP file to a desired location.
3. Use the provided [Flatbed.Dialog (Lite)](https://dkorablin.github.io/Flatbed-Dialog-Lite) executable or download one of the supported host applications:
	- [Flatbed.Dialog](https://dkorablin.github.io/Flatbed-Dialog)
	- [Flatbed.MDI](https://dkorablin.github.io/Flatbed-MDI)
	- [Flatbed.MDI (WPF)](https://dkorablin.github.io/Flatbed-MDI-Avalon)
4. Download the [Plugin.Compiler](https://github.com/DKorablin/Plugin.Compiler) that is responsible for compiling .NET code and execute it.
5. Download the [Plugin.Timers](https://github.com/DKorablin/Plugin.Timers) that is responsible for various timer functionalities and execute it.