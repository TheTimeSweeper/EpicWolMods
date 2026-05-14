# Clothes
Adds some robes with custom effects and colors.
![cool robes](https://raw.githubusercontent.com/TheTimeSweeper/EpicWolMods/master/Clothes/Release/readme/Clothes.png)

Pandemonium Cloak inspired by Shovel Knight:  
![HEE HEE HEE](https://raw.githubusercontent.com/TheTimeSweeper/EpicWolMods/master/Clothes/Release/readme/pandemonium.gif)  
Randomizes color and stats every X seconds

Any questions or feedback or mind exploding issues, ping/message `thetimesweeper` on Discord.  
I would especially like feedback on Impatience and Pandemonium, feeling-wise and balance-wise.

### Plans
- ~~Pandemonium cloak from shovel knight~~
- ~~Custom color compat with TED~~
- Config custom cloak
___

### Installation (manual):
- Make sure all dependencies are installed
- Download and extract the .zip
- in your `BepInEx/plugins` folder create a new folder called `TheTimesweeper-Clothes`
- drag the contents of this mod's `plugins` folder into this new folder.

### Changelog:

`0.6.3`
- added missing dependency on LegendAPI
- fixed mod not working when the Custom Palettes mod wasn't installed
  - *This mod is intended to still function without it, but has support with it as well*
- lowered gold gain stat in pandemonium robe
  - *it turned out to be a bit too much as it's a permanent gain from what should be a temporary stat*

`0.6.2`
- migrated to new Custom Palettes api
- migrated to new LegendAPI version. make sure you update that too!
- fixed Impatience improperly speeding up basics' final hits with combo gloves
- slight buff to Impatience's end time reduction. felt it wasn't enough to justify the harsh health and damage penalties
- added darkened palettes for Pandemonium when Shadow is equipped with it.

`0.6.1`
- removed some accidentally left in debug code

`0.6.0`
 - added Pandemonium Cloak which randomizes your stats and colors
   -  thanks to wife for the idea and Shovel Knight for the inspiration
 - added transparency to Aqua to feel more watery

`0.5.0`
 - c: