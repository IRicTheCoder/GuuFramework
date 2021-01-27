# Guu Framework - Features
All features from Guu are listed here with some bullet points with their functions as well as description of each main feature (or section). Some features do have small examples to explain them, but this is not a documentation file, it just tells you what features exist. This list contains the features available on the lastest version of the framework, when the documentation is done there will be a page to check when said feature was added as well as an improved version of the feature list.

**PLEASE NOTE** that some features require the mods to be loaded by Guu itself, otherwise they won't work. Those features are identified, however, no feature can be guaranteed to work for mods loaded by a different loader.

Documentation is currently being developed, this file will be dropped in favor to it once that is done.

**For context** the **Guu Framework** is referenced to as **Guu**, while the **Eden Framework** is referenced to as **Eden**

### General
These are just general features and each point just lists some important general information related to what the framework does and how it works.

- Uses Doorstop (previously known as Unity Doorstop) to load the assembly into the game without patching any of the game's files. 
  - A log file called *`Injection.log`* is found within Guu's folder that provides a log of the injection process
- The log file from Unity is copied from its normal place to the Guu's folder, and it's named *`Unity.log`*
- Disables the Sentry SDK from the game to prevent messages from modded games going to Monomi Park.
- Reads custom command line arguments to allow custom behaviours, modders can also read those arguments if they so wish to.
  - `--debug` - Runs the game in a sort of Debug Mode created by Guu
  - `--guuDebug` - Activates Guu Debug Mode which presents further information (not required in most cases)
  - `--guuLauncher` - Added by the Launcher to check if the launcher was used to launch the game, prevents the game from launching if not set. Debug Mode ignores this.
  - `--guuSilent` - Prevents the game from reopening the launcher when the game closes. Debug Mode ignores this.
  - `--trace` - Prints a stack trace for every logged message
- The folder strucuture from Guu is as follows:
  - *`Guu/Bindings`* - Contains the keybinds and button binds used by mods **(do not edit things unless you know what you are doing)**
  - *`Guu/Configs`* - Currently is not being used, but will contain config files for mods
  - *`Guu/Framework`* - This is the folder where the framework files reside **(do not mess with it, independent of if you know what you are doing or not)**
    - *`Guu/Framework/Libraries`* - Modders can reference the libraries/assemblies in this folder when making mods, otherwise leave it alone.
    - *`Guu/Framework/Tools`* - Modders can find useful tools here, look at the documentation to learn what each does, some of this tools are used by Guu when playing, so leave it alone if you are not using them when making your mods.
  - *`Guu/Libraries`* - The Addon Libraries to be loaded by Guu, you should only put files here when mods tell you to, otherwise leave it alone.
  - *`Guu/Mods`* - The folder to place mods in, all mods need to have their own folder and a valid modinfo.yaml to be loaded. Unless stated otherwise Guu Mods go in here.
  - *`Guu/Reports`* - Crash reports will be created inside this folder, and only the last 10 will be available, older ones will be deleted automatically.
  - *`Guu/Saves`* - Currently is not being used, but will contain the current save files (copies of the ones in the appdata), and will contain Guu specific files.
  
### Addon Libraries
Any library (*`.dll`* file) present inside *Guu/Libraries* will be loaded into the game as an addon library, this is useful to add new modules to Guu. This can also be used for the same purpose as the *Core Mods* from *Minecraft Forge*.

- Allows modders to create new modules for Guu.
- If the module contains a class that inherits from `IAddonLoad`, a call to that class will be done when loading the addon library.

### Crash Handler
The crash handler taps into the system to catch all and any crash that might occur inside the game, no exception/crash will be uncaught unless it occurs outside the normal runtime of the game, that is a limitation that can't be worked around at the moment.

- Can capture any crash caused by the game, a mod or unity itself.
- Provides a crash report with important info (found in *`Guu/Reports`*).
- Will display a UI when the game crashes to show the crash report generated.
  - Allows to easily open the Log files to check what happened.
  - You can also copy the report text diretly to your clipboard.
  - There is a button to open the report folder.
  
### `Enum` Injector
> **This feature is not available when using another mod loader to load your mods**

This uses the `EnumFixer` available in Eden, to add enum values during runtime. And will facilitate the injection of any enum into the game without conflicting with other enum patchers from other loaders. 

- Injects using `EnumFixer` provided by Eden.
- Enums can only be injected into the game during the Initialization step of the mod, meaning that it is only usable inside the `ModMain.Init()` method.
- Allows for any enum to be injected dynamically.
- Can use `EnumInject` annotation to mark a class for enum injection.
  - Will inject and set the field value to the inject one for any field of any enum type.
  - Fields must be `static` and `readonly`
  - Fields need to have the default value of `0`
- If the enum name already exists, it will populate the field with the current value instead of making a new one.

### Log System
Guu comes with a custom logging system in place to facilitate logging things into both files and the console itself. It is easy to use and mimics partially Unity's log system.

- When a mod is loaded, a logger is provided and can be accessed using `ModMain.Logger`.
  - This logger allows modders to log into the console and the log file and customizes the message with the mod ID so log entries are easier to identify.
  - If you load your mod from another loader, you have to create an instance of `ModLogger` yourself.
- Custom log files can be provided to any `ModLogger` to allow that logger to also log to said file.

### Mod Loader
Because you need a way to load mods if you want to use only Guu, a Mod loading system is also provided so mods can be loaded into the game. To make sure loading is done right, some conventions are required to be uphold to make sure everything works as expected, so if said conventions are not uphold, the mod will not load.

- Every mod needs to be inside its own folder
- Only mods that contain a valid modinfo.yaml file inside their folders will be loaded
- Mods require a class to inherit from `ModMain` in order to have an entry point. 
  - If none is found the Loader will crash
  - If more than one exists, a random one will be selected
- When loading, only the main assembly will be loaded into the game
- To load additional modules the `ModMain` class should have `ModModule` annotations identifying other modules to load
  - A mod can provide a dependency ID and version to only load if a dependency is loaded or exists
  - Multiple modules can be loaded, one for each `ModModule` annotation present
  - Modules require a class to inherit from `ModuleMain` in other to have an entry point.
- Can check if mods or assemblies are loaded
  - `ModLoader.IsModLoaded` can be used to check, by ID, for those loaded by Guu. Or use the ID with special prefixes to check other loaders.
    - `srml:ID` can be used to check if there SRML has a loaded mod with mod id ID.
    - `assem:ID` will call `ModLoader.IsAssemblyLoaded` using ID.
  - `ModLoader.IsAssemblyLoaded` can be used to check, by simplified name, if an assembly is loaded.
- For Modders use, using `ModLoader.GetAllUnknownAssemblies` a file will be dumped into the Game's root folder with a list of all assemblies the loader couldn't identify.
- Any field with `IsLoaded` annotation will be populated with the loading state of said mod by the `IsLoadedInjector`
  - The ID provided to `IsLoaded` follows the same rules used fofr `ModLoader.IsModLoaded`
  - Fields must be `static` and `readonly`
  - Fields have to be of type `bool`

### Improved Market
The Plort Market is one of the 
