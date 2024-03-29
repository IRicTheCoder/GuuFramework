# Guu Framework
The Guu Framework is a Library and Mod Loader for [Slime Rancher](https://store.steampowered.com/app/433340/Slime_Rancher/) to allow players to create mods. Different from the current mod loaders for the game, Guu does not require any patching done to the game to be able to inject itself into the game.

Using a sort-of loader called [UnityDoorstop](https://github.com/NeighTools/UnityDoorstop) and by having it load Guu's entry point the game recognizes it without having to be patched, making it easier to use and remove.

Guu brings the ability to not only write code and add it to the game easily through a easy to use yet powerful API, but it also allows modders to patch the game's code to add their own as well as use Untiy directly when making content for each mod.

**PLEASE NOTE** that Guu can be used as a Library (to make your mods) even if you are loading your mod using another Loader. But some features might not be available if you do so. You do not need to worry about assemblies being available when your mod loads with a different loader as Guu loads before Unity itself.

## Guu's Dev Kit (For Unity)
Along side the framework itself, you will also find a .unitypackage file that will install the Dev Kit directly into Unity. This Dev Kit will allow the creation of content from normal Assets to Prefabs directly in the Editor.

<!-- It comes packed with Fake Components that mimic the Game's original ones and will be changed to the those original ones when they are loaded into the game. Modders can also use the API System within the Editor by creating custom files that are easily edited, and when loaded into the game will automatically be registered in their respective places. -->

## External Libraries
Besides itself, Guu also brings some Third Party libraries into the game that can be used by Modders. Those libraries are required for the framework to function and cannot be removed. And they are:

### Harmony 2.0
Harmony is used to make runtime patches to the code. When loading this library, it will try to favor versions that are already loaded into the game (for compatibility with other
loaders). Modders can use this to make their patches but it's highly discourage as there is a better system in place.

### Eden Framework
Eden is a framework developed by me ([RicTheCoder](https://ricthecoder.com/)) mainly for game development, however it does serve a lot of purposes and contains an extensive and powerful library of code. Examples of this would be EdenHarmony an improved version of the Harmony's original system and the Event Handling system that allows better handling of C# events. A lot of useful tools are provided by this and modders can access it to use said tools.

Only the required files are shipped with Guu, as Eden works in modules and not all is required if not used. If you wish to use a module from Eden not available with Guu, you can add it as an addon library to Guu (the 'how to' coming soon).

**TO PATCH THE GAME** EdenHarmony is the go to system, it is easier to use, makes patch classes easier to read and has new functionality that the original Harmony does not provide.

_**PLEASE NOTE** that Eden is also in constant development, so sometimes Guu will wait for an update to release or fix a feature dependent of Eden_

<!--## Feature List
### Guu Framework
All features and their respective description can be found [here](https://github.com/RicardoTheCoder/GuuFramework/blob/main/.features/guu.md), as for documentation on each thing it is still being developed. All changelogs can be found [here](https://github.com/RicardoTheCoder/GuuFramework/tree/main/.changelogs)

### Eden Framework
The important feature list for modding can be found on [Guu's Feature List](https://github.com/RicardoTheCoder/GuuFramework/blob/main/.features/[[0.1p-21312](../.changelogs/0.1p-21312.md)].md), while the complete feature list and documentation can be found on Eden's Github page (currently unavailable).

### Harmony
All features and documentation for harmony can be found [here](https://harmony.pardeike.net/articles/intro.html).-->
