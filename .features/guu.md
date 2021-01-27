# Guu Framework - Features
All features from Guu are listed here with some bullet points with their functions as well as description of each main feature (or section). Some features do have small examples to explain them, but this is not a documentation file, it just tells you what features exist. This list contains the features available on the lastest version of the framework, when the documentation is done there will be a page to check when said feature was added as well as an improved version of the feature list.

**PLEASE NOTE** that some features require the mods to be loaded by Guu itself, otherwise they won't work. Those features are identified, however, no feature can be guaranteed to work for mods loaded by a different loader.

Documentation is currently being developed, this file will be dropped in favor to it once that is done.

**For context** the **Guu Framework** is referenced to as **Guu**, while the **Eden Framework** is referenced to as **Eden**

## General
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
  
## Addon Libraries
Any library (*`.dll`* file) present inside *`Guu/Libraries`* will be loaded into the game as an addon library, this is useful to add new modules to Guu. This can also be used for the same purpose as the *Core Mods* from *Minecraft Forge*.

- Allows modders to create new modules for Guu.
- If the module contains a class that inherits from `IAddonLoad`, a call to that class will be done when loading the addon library.

## Crash Handler
The crash handler taps into the system to catch all and any crash that might occur inside the game, no exception/crash will be uncaught unless it occurs outside the normal runtime of the game, that is a limitation that can't be worked around at the moment.

- Can capture any crash caused by the game, a mod or unity itself.
- Provides a crash report with important info (found in *`Guu/Reports`*).
- Will display a UI when the game crashes to show the crash report generated.
  - Allows to easily open the Log files to check what happened.
  - You can also copy the report text diretly to your clipboard.
  - There is a button to open the report folder.
  
## `Enum` Injector
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

## Log System
Guu comes with a custom logging system in place to facilitate logging things into both files and the console itself. It is easy to use and mimics partially Unity's log system.

- When a mod is loaded, a logger is provided and can be accessed using `ModMain.Logger`.
  - This logger allows modders to log into the console and the log file and customizes the message with the mod ID so log entries are easier to identify.
  - If you load your mod from another loader, you have to create an instance of `ModLogger` yourself.
- Custom log files can be provided to any `ModLogger` to allow that logger to also log to said file.

## Mod Loader
> **This feature is not available when using another mod loader to load your mods**

Because you need a way to load mods if you want to use only Guu, a Mod loading system is also provided so mods can be loaded into the game. To make sure loading is done right, some conventions are required to be uphold to make sure everything works as expected, so if said conventions are not uphold, the mod will not load.

- Every mod needs to be inside its own folder.
- Only mods that contain a valid modinfo.yaml file inside their folders will be loaded.
- Mods require a class to inherit from `ModMain` in order to have an entry point. 
  - If none is found the Loader will crash.
  - If more than one exists, a random one will be selected.
- When loading, only the main assembly will be loaded into the game.
- To load additional modules the `ModMain` class should have `ModModule` annotations identifying other modules to load.
  - A mod can provide a dependency ID and version to only load if a dependency is loaded or exists.
  - Multiple modules can be loaded, one for each `ModModule` annotation present.
  - Modules require a class to inherit from `ModuleMain` in other to have an entry point.
- Can check if mods or assemblies are loaded.
  - `ModLoader.IsModLoaded(ID)` can be used to check, by ID, for those loaded by Guu. Or use the ID with special prefixes to check other loaders.
    - `srml:ID` can be used to check if there SRML has a loaded mod with mod id ID.
    - `assem:ID` will call `ModLoader.IsAssemblyLoaded(name)` using ID as name.
  - `ModLoader.IsAssemblyLoaded(name)` can be used to check, by simplified name, if an assembly is loaded.
- For Modders use, using `ModLoader.GetAllUnknownAssemblies()` a file will be dumped into the Game's root folder with a list of all assemblies the loader couldn't identify.
- Any field with `IsLoaded` annotation will be populated with the loading state of said mod by the `IsLoadedInjector`.
  - The ID provided to `IsLoaded` follows the same rules used fofr `ModLoader.IsModLoaded(ID)`.
  - Fields must be `static` and `readonly`.
  - Fields have to be of type `bool`.

## Asset Packs
To facilitate the load of assets into the game, Guu provides Asset Packs which are similar to Unity's *Asset Bundles*. These packs use the *Asset Bundle* under the hood to save assets from the *Unity Editor* and load them into the game. However they offer better methods to acquire assets from them, also they can load API Files automatically and register them. A tool to save them from within the editor is provided by Guu's Dev Kit.

- Assets Packs are loaded with `AssetLoader`.
  - Assets can be loaded from any given path.
  - Assets can be loaded from the mod's *`Assets`* folder directly.
- Asset Packs use the *Asset Bundle* system under the hood, but are easier to use.
  - Any asset can be obtained by using `AssetPack.Get<T>(name)`.
  - Can get all assets of a type using `AssetPack.GetAll<T>()`.
  - All assets should be used and/or cached during mod loading whenever possible to prevent memory leaks.
- A creation tool is provided by Guu's Dev Kit to create asset packs easily within *Unity Editor*.

## Guu Services
Guu provides services, those are features that are only loaded and used if a mod asks for them, so they will be inactive until some mod initializes them. There are two services that are always running, they are considered services because they are only truly useful game wise if a mod uses them, but Guu also taps into them to do some of it's work.

- System Windows, this service allows mods to register windows that are part of the system and not the game.
  - There windows use the *IMGUI* or *Legacy GUI* system provided by Unity draw them.
  - They serve as windows that do interface with the game directly and are used as special menus for mods (Ex: Console, Cheat Menu...).
  
## SRML Bridge
**This only gets loaded if SRML is present** and is used as a Bridge to better integrate them together, this is required to prevent both loader from overlapping each other and causing unnecessary problems.

- Removes the console provided by SRML in favor of Guu's Console.
- Registers all SRML Commands into Guu's Console.
  - Some commands will not be registered cause they are equal to ones already present.
  - If a command finds a conflic in terms of IDs, the prefix *`srml.`* will be added to the command's ID.
- Redirects all Command Catchers from SRML to Guu's Console. They run after the Catchers from Guu's Console.
- All logs done by SRML are redirected to Guu's Log System using the ID *`SRML`*.
- Allows Guu to check if a SRML mod is loaded.
- Allows Guu to check if an assembly belongs to a SRML mod.

## Guu Console
The almighty Guu console that also works as a Cheat Menu and some other things. Guu's Console resides in the Dev Tools Window that contains a bunch of system related things, from the console to the cheat menu. The console is overpowered with the ability to execute some powerful debug commands to check objects and their values without having to log them into the console by code.

- Allows new commands to be registered.
- Allows custom UI buttons to be registered.
- Can dump information into files when using the `dump` command.
  - Allows new dump actions to be registered.
  - Dump actions collect information and dump that information into a file.
  - Dump files are created inside the *`Dumps`* folder that resides inside the game's root folder.
- Can register command catchers.
  - Command Catchers will be able to catch console input before any command is executed.
  - A command catcher can prevent a command's execution and/or check the current execution state.
  - Methods registered as Command Catchers can use the `EventPriority` annotation from Eden to change their execution priority.
  - Registering the same method more than once won't make it run multiple times, as the catchers are invoked as unique.
- All items, except command catchers, can have the replace flag set to `true` to replace values if conflicts arise.

## Language Controller
Controls the entire language and translation system within the game, and allows modders to add their own translations or load them directly from language files. It also fixes some issues with the language in game as well as provide new features not available on the original game.

- Can register new languages to the game.
- Can read translations from files within the mod's *`Lang`* folder, or have them provided.
- It is possible to register listeners that are called when the language changes.
- Adds support for `RTL` (Right to Left) languages.
- Fixes the auto resizing of texts when the translated version is too big.
- Can register alternative ids for languages instead of using the default ones provided by `Culture`.
- Language files are .yaml for compatibility with most team localization tools (Ex: Crowdin).

## SR Objects
A system that can get any asset or object from the game, these objects can be retrieved in two different ways, from the resources and/or from the world/scene.

- Aids in getting an object from the game.
- Contains some already obtained values that are common.
- When using it please cache the value or use it only at load to prevent memory leaks.

## Improved Market
The Plort Market is one of the most problematic things when registering new content, if the limit is reached, it will either crash the game or stop setting the values right. This fixes the problem by replacing the Market with a custom version.

- Fixes the market display when the limit of plorts is exceeded
- Plorts can now be hidden completly instead of just greyed out
  - After being shown they will still be greyed out unless both states are unlocked by the same thing
- Plort entries can check if a progress is achieved as well as the number of said progress

# Eden Framework - Relevant Features for Modding
Eden provides a lot of features overall, focused on game development, but also useful for software development in general. Guu uses Eden to allow it to go a bit further than commonly possible and provide the most advanced and useful tools possible to make mods for the game. The following list contains only the features that are relevant for using when making mods.

## General
Just some general features that can greatly improve development in any sense.

- Allows the easy creation of Singletons.

## `Event` Handling
Improves the event handling system provided by C#, making this system far more powerful and useful then just the normal registration and invocation features. This does not override the default system and that can still be used to bypass the handler.

- Upgrades the current `event` system provided by C#.
- To use the upgraded system use `event.Handle(args)` or `event.Handle<R>(args)` instead of the normal `event.Invoke(args)`.
  - Can force events to only trigger unique methods, so repetitions won't be called, by setting the `unique` parameter to `true`.
  - Can provide context to each method called by the event by setting the `provider` parament to a `IContextProvider` instance.
  - Each method in the call can get the value returned by the last method called using `EventHandler.LastResult<T>()`
- Can use `event.HandleSpecific<T>(args)` or `event.HandleSpecific(args)` to only call methods that belong to the type provided.
- Sorts method calls by priority when the event is triggered.
  - To set the method's priority add a `[EventPriority]` annotation to the method called by the event.
- The context can be obtained using `EventHandler.GetContext<T>()`.
- You can use `event.Invoke(args)` to bypass these features.

## `Enum` Fixer
Grants the ability to add custom enum values and names during runtime, this does not actually change the structure of the object/enum, it just redirects the calls to its methods into the holder that contains the extra data.

- Allows custom values and names to be added to enums.
- If a name is already present will reuse the value already present.
- All values start at `1000000` (1 million) to avoid problems.

## Eden Harmony
Eden Harmony is an enhanced version of the Harmony system, it still depends on the Harmony library to work and simply improves things on top of it. It also adds a different (and maybe better) way to design patch classes. All these features require the use o `EdenHarmony` instead of `Harmony`, the first inherits from the latter so only `EdenHarmony` needs to be called even if patches from the original systema are present.

- Patch class names need to always be *`TypeName_PatchX`*.
  - *`TypeName`* is the name of the type to patch.
  - *`X`* is an extra to help when making multiple patch classes that use the same *`TypeName`*. It can be omitted.
  - *`X`* can't have a `_` or it won't be considered valid.
- To use all the following features the patch class requires a `[EdenHarmony.Wrapper]` annotation to be present.
  - This is only valid if you use `EdenHarmony` when patching.
  - A type can be passed to identify the type to patch.
  - When passing a type to the Wrapper the name of the class still needs to be *`TypeName_PatchX`*.
  - If no type is provided to the Wrapper, you need to register a method to `EdenHarmony.TypeResolver`.
    - The method should return the correct type to be patched (the *`TypeName`* is provided).
    - To ensure registration happens, you can use the static constructor to register the method into the event.
  - If a method needs to be resolved, register a method to `EdenHarmony.MethodResolver`.
    - The method should return the valid method info based on the method name provided.
    - The static constructor can also be used to register this method.
  - Alternatively, methods can have the `[EdenHarmony.DefineOriginal]` annotation that provides the list of arguments to help clarify the method to patch.
- Better readability for patch methods (that use the `EdenHarmony.Wrapper` system).
  - Methods will use `@Name` for values instead of the less readable `__Name`.
    - `__instance` is mapped to `@this`.
    - `__result` is mapped to `@return`.
    - `__state` is mapped to `@state`.
    - `__originalMethod` is mapped to `@origin`.
    - These are not mapped if a parameter with a `@` in their name matches one of those. (Ex: method has parameter `@return`, then `__result` remains the same).
  - For private fields they are also `_Name` instead of `___Name`.
  - Index based argument names `__n` remain the same.
- Better identification for patch methods (that use the `EdenHarmony.Wrapper` system).
  - Runs through all methods inside the patch classes to find methods to patch.
  - All methods follow the format *`MethodName_X`*.
    - For getters/setters use *'get_PropertyName'* or *'set_PropertyName'* respectively instead of *`MethodName`*.
    - *`X`* is a suffix that identifies the type of patch.
  - If method ends with *`_Prefix`* registers method as prefix.
  - If method ends with *`_Postfix`* registers method as postfix.
  - If method ends with *`_Transpiler`* registers method as transpiler.
    - Transpilers follow the original logic.
  - If method ends with *`_Catch`* registers method as finalizer.
    - '__exception' is now mapped to '@throw'.
  - If method doesn't have a suffix (and exists in the type being patched) registers method as a resverse patch of type original.
    - These follow the original logic.
- Patches can still be made by the normal harmony conventions.
- It is possible to execute base methods inside patches methods using `MethodBase.InvokeAsBase(args)`.
  - Depending on the patch, this might not be achievable, but it will not throw an error, just a warning.
- Patches can be registered to be executed later than on the spot by using `EdenHarmony.LatePatchAll(args)`.
  - They can be later executed with `EdenHarmony.ExecuteLatePatches()`.
- Enums can now be fully patched.
  - Make a new enum and add the `[EdenHarmony.EnumWrapper]` annotation
  - EnumWrapper receives a type to identify the enum type being patched
  - If the original enum contains the `[Flags]` annotation, this new enum requires it too
  - Set the second argument in EnumWrapper to `true` to take the numeric values in the new enum into account
    - For Flag enums this is ignored
  - To access the patched values just call them normally and do `Enum.As<T>()`
  - **Patching Flag enums is still very experimental**
