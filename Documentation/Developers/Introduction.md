## Welcome
This document is a developer introduction to **FancyTrainsimTools** to get you started if you like to help me developing this application 
further, you should start reading this document carefully. It introduces the way the application is set up, the tools and technologies 
that are used and some elemntary guidelines on programming style.

before all: please contatc me if you like to help and discuss what you are going to do, before starting. You can reach me by mail: 
_trainsimulator_ at _hollandhiking.nl_

## Tools

* Visual Studio 2019 Community Edition
* Jetbrains Resharper (payware, you can do without)
* Ghostdoc for source documentation (payware, you can do without)
* DB Browser for SQLite [link](https://sqlitebrowser.org/)
* Microsoft Word for user documentation

## Technologies

* C# 8.0
* .Net Core 3.1
* WPF
* Linq
* xUnit
* SQLite databases
* Lua version 5.03 (not the newer ones!)

Nuget packages:

* Microsoft.Extensions.DependencyInjection
* Microsoft.Extensions.Configuration.Binder
* Microsoft.Extensions.Configuration.Json
* System.ComponentModel.Composition
* Microsoft.Windows.Compatibility (not sure if this is needed)
* System.Composition.
* Dapper for database access
* System.Data.SQLite.Core

I do **NOT** use entity framework or a specific MVVM framework

You will need a number of additional programs. These are used for the main program:

* [7Zip](http://www.7-zip.org/download.html) Decompress (you will really need this one)
* [Notepad++](https://notepad-plus-plus.org/)	Text editor (preferred optional)
* [XMLNotepad](http://www.lovettsoftware.com/downloads/xmlnotepad/readme.htm)	XML editor, optional
* [Luadec](http://files.luaforge.net/releases/luadec/luadec) Lua decompiler, optional
* [RWTextEdit](https://www.ivimey.org/content/rwtextedit) Edit compressed Trainsimulator files, required
* [LUA binaries](http://luabinaries.sourceforge.net/) Lua compiler. Note: you need version 5.03
* [SQLite DBBrowser](https://sqlitebrowser.org/) Create SQLite tables and view them in the database directly

I assume you do have DTG trainSimulator.

## Required knowledge

To be helpful, you should be aware of a reasonable subset of the knowledge base below:

* C# in general
* WPF
* SOLID principles
* SQL and database design 
* MVVM
* Unit testing

## Projects

The whole application consists of anumber of projects:

* Assets.Library contains the data access functions for assets and routes. 
* Documentation consists of all documentation. This project does not need compilation.
* FancyTrainsimTools.Desktop contains the user interface.
* FancytrainsimToolsDesktop.Library may contain a seprate UI library (not used at the moment)
* FancytrainsimTools.Test is for the xUnit tests for the UI
* Logging.Library is a small set of functionality to support logging. 
* Mvvm.Library is a kind of Mvvm framework. In the long run it probably will be replaced by Caliburn.Mirco, but for now I am happy with a litthe bit of Mvvm support.
* Styles.Library contains all WPF support stuff. This includes UserControls, Converters, and styles (dictionaries) for various UI elements.

The three latter libraries ar meant to be used in multiple projects. So please take great care if you need to change them.

## Code guide lines

* DataAccess should stay hidden for the UI.
* For the UI I use MVVM, View First and the included library. This is not very fancy, but I am still learning the ropes and I need code I can understand.
* Application wide settings are stored in the UI, appsettings.json In the static class Settings.cs these are mapped for easy access.
* I still need to create a new edit screen for the settings.
* Settings should not be accessed from withing the code in the data access. use the veiemodels to to pass settings to the data access
* Dependency Injection is done using the Microsoft.Extensions.DependecyInjection functions.
* Models should not contain any other login apart from conversions and validation.
* NotiFyOfpropertyChanged is implemented in the ViewModels, by inheritance for BindableBase of ValidatingBindableBase.
* The Views use a ViewModelLocator to bind to the ViewModel
* Actions are invoked using Commands in WPF, ICommand and relayCommand as implementation.
* In WPF parts, almost all elements refer explicity to a style. I try to avoid ard coded sizing in windows.
* For large windows, use scrollbars and a derived class from WindowSizing, you find them in the styles library.
* I created my own logging framework. It works standard, using messaging to send logging to who wants to hear. It also returs a string directly, e.g. for a popup.
* Coding style: see the code that is done as an example. Style guide will come later.





