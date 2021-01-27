# Guu Framework - Features
All features from Guu are listed here with some bullet points with their functions as well as description of each main feature (or section). Some features do have small examples to explain them, but this is not a documentation file, it just tells you what features exist. This list contains the features available on the lastest version of the framework, when the documentation is done there will be a page to check when said feature was added as well as an improved version of the feature list.

**PLEASE NOTE** that some features require the mods to be loaded by Guu itself, otherwise they won't work. Those features are identified, however, no feature can be guaranteed to work for mods loaded by a different loader.

Documentation is currently being developed, this file will be dropped in favor to it once that is done.

For context the **Guu Framework** is referenced to as **Guu**, while the **Eden Framework** is referenced to as **Eden**

### General
These are just general features and each point just lists some important general information related to what the framework does and how it works.

- Uses Doorstop (previously known as Unity Doorstop) to load the assembly into the game without patching any of the game's files. 
  - A log file called *Injection.log* is found within Guu's folder that provides a log of the injection process
- The log file from Unity is copied from its normal place to the Guu's folder, and it's named *Unity.log*
- Disables the Sentry SDK from the game to prevent messages from modded games going to Monomi Park.
- Reads custom command line arguments to allow custom behaviours (modders can tap into this).
  - **`--debug`** Runs the game in a sort of Debug Mode created by Guu
  - `--guuDebug` - Activates Guu Debug Mode which presents further information (not required in most cases)
  - ***--guuLauncher*** - Added by the Launcher to check if the launcher was used to launch the game, prevents the game from launching if not set. Debug Mode ignores this.
  - ***--guuSilent*** - Prevents the game from reopening the launcher when the game closes. Debug Mode ignores this.
  - ***--trace*** - Prints a stack trace for every logged message
- The folder strucuture from Guu is as follows:
  - ***Guu/Bindings*** - Contains the keybinds and button binds used by mods **(do not edit things unless you know what you are doing)**
  - ***Guu/Configs*** - Currently is not being used, but will contain config files for mods
  - ***Guu/Framework*** - This is the folder where the framework files reside **(do not mess with it, independent of if you know what you are doing or not)**
    - ***Guu/Framework/Libraries*** - Modders can reference the libraries/assemblies in this folder when making mods, otherwise leave it alone.
    - ***Guu/Framework/Tools:*** - Modders can find useful tools here, look at the documentation to learn what each does, some of this tools are used by Guu when playing, so leave it alone if you are not using them when making your mods.
  - ***Guu/Libraries*** - The Addon Libraries to be loaded by Guu, you should only put files here when mods tell you to, otherwise leave it alone.
  - ***Guu/Mods*** - The folder to place mods in, all mods need to have their own folder and a valid modinfo.yaml to be loaded. Unless stated otherwise Guu Mods go in here.
  - ***Guu/Reports*** - Crash reports will be created inside this folder, and only the last 10 will be available, older ones will be deleted automatically.
  - ***Guu/Saves*** - Currently is not being used, but will contain the current save files (copies of the ones in the appdata), and will contain Guu specific files.
  
### Addon Libraries
Any library (.dll file) present inside *Guu/Libraries* will be loaded into the game as an addon library, this is useful to add new modules to Guu. This can also be used for the same purpose as the *Core Mods* from *Minecraft Forge*.

- Allows modders to create new modules for Guu.
- If the module contains a class that inherits from IAddonLoad, a call to that class will be done when loading the addon library.

### Crash Handler
The crash handler taps into the system to catch all and any crash that might occur inside the game, no exception/crash will be uncaught unless it occurs outside the normal runtime of the game, that is a limitation that can't be worked around at the moment.

- Can capture any crash caused by the game, a mod or unity itself.
- Provides a crash report with important info (found in *Guu/Reports*)
- Will display a UI when the game crashes to show the crash report generated
  - Allows to easily open the Log files to check what happened
  - You can also copy the report text diretly to your clipboard
  - There is a button to open the report folder
  
### Enum Injector
> **This feature is not available when using another mod loader to load your mods**

This uses the Enum Fixer availabe in Eden, to add enum values during runtime. And will facilitate the injection of enums into the game without conflicting with other enum patchers from other loaders. 

- Injects using EnumFixer provided by Eden
- Enums can only be injected into the game during the Initialization step of the mod, meaning that it is only usable inside the `ModMain.Init()` method
- Allows for enums to be injected dynamically
