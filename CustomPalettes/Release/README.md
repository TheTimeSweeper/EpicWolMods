# CustomPalettes
An API for adding custom palettes for custom robes (or other unique robe effects (like neve's)).

### How to use:

Visit [this page](https://github.com/WoL-Modding-Extravaganza/WoLWiki/wiki/Custom-Content:-Robes) for a tutorial and examples on how to do use this with your custom robes.  
(as well as how to make a custom robe mod)

You'll want to pass in a 2x56 texture that looks something like this  
![palette strip](https://raw.githubusercontent.com/TheTimeSweeper/EpicWolMods/master/_EpicUnityProject/Assets/Bundo/Strips/WalterBoos1.png)  
![palette strip](https://github.com/TheTimeSweeper/EpicWolMods/blob/master/CustomPalettes/Release/readme/WalterBoos1Big.png?raw=true)

By using `CustomPalettes.Palettes.AddPalette` and its overloads:
```csharp
CustomPalettes.Palettes.AddPalette(string fullPath)
CustomPalettes.Palettes.AddPalette(params string[] pathDirectories)
CustomPalettes.Palettes.AddPalette(Texture2D palleteTexture)
```
Examples:
```csharp
string fullPathToSpriteFile;
int myPalette = CustomPalettes.Palettes.AddPalette(fullPathToSpriteFile)
//use myPalette in custom robe
```
```csharp
string assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location;
int myPalette = CustomPalettes.Palettes.AddPalette(assemblyLocation, "Assets", "MySprite.png")
//use myPalette in custom robe
```
```csharp
Texture2D textureLoadedFromAssetBundleOrSomething;
int myPalette = CustomPalettes.Palettes.AddPalette(textureLoadedFromAssetBundleOrSomething)
//use myPalette in custom robe
```

### To make a custom robe palette yourself without code:
1. Visit http://tailoroflegend.epizy.com/
2. Follow instructions to create a custom .robe file
3. Download and install [Tournament Edition](https://wizard-of-legend.thunderstore.io/package/Team_Mythic/TournamentEdition/)
4. Add your .robe file to the Custom Robes folder
___
Any questions or feedback or mind exploding issues, ping/message `thetimesweeper` on Discord
### This mod does not add anything by itself. It is a tool for other mods to use.
Some mods that use custom palettes:
- [Tournament Edition](https://wizard-of-legend.thunderstore.io/package/Team_Mythic/TournamentEdition/) by Team_Mythic
- [Clothes](https://wizard-of-legend.thunderstore.io/package/TheTimesweeper/Clothes/) by TheTimesweeper
___
## Credits
- `only_going_up_fr0m_here` - original palette code and image loading code
- `theholygrind` - modified player sprite sheet
- `thetimesweeper` - migrating and improving palette code
- `kvadratisk` - help improving palette code
___

### Installation (manual):
- Make sure all dependencies are installed
- Download and extract the .zip
- in your `BepInEx/plugins` folder create a new folder called `TheTimesweeper-CustomPalettes`
- drag the contents of this mod's `plugins` folder into this new folder.

### Changelog:

`0.1.0`
 - c: