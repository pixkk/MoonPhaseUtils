
<img width="2480" height="799" alt="logoofmod" src="https://github.com/user-attachments/assets/e2bf33b8-2993-4631-ab6e-575d62168ac8" />

# MoonPhaseUtils
A BepInEx mod that enhances the moon system by providing visual information about the current moon phase and its attributes directly in the game's HUD. Changes the boring system of constant moon phase.  
It works as server-side and client-side mod.

Mod page on the Github: https://github.com/pixkk/MoonPhaseUtils

Mod can work in two modes:

- **Server-side**: server owner can see HUD with current moon information, but currently it have a **BUG** when another players don't see this information. In server-side case, only OWNER must have this mod. Another players can play without it.
- **Client-side**: in SINGLEPLAYER you can see HUD.

## Screenshots
![202508~3](https://github.com/user-attachments/assets/dfa0dba6-e2e5-4ab9-99bb-d1116eda28e3)
![202508~1](https://github.com/user-attachments/assets/3ad2bbc3-86d5-49a2-9bd8-770f73a63740)

## Compatibility
Tested on all versions of R.E.P.O. Currenty 100% working on **0.2.1**.

## Features

### 🌙 **In-Game Moon Display**
- **Real-time Moon Information**: Displays the current moon's name and attributes in the bottom-right corner of the game screen
- **Moon Icon**: Shows the corresponding moon icon above the text information
- **Dynamic Positioning**: The moon icon automatically adjusts its position based on the text size
- **Clean UI Integration**: Seamlessly integrates with the existing game HUD without interfering with gameplay

### 🔧 **Moon Level Calculation Override**
- **Custom Moon Progression**: Modifies the moon level calculation to follow a 4-phase cycle pattern
- **Formula**: `moonLevel = ((levelsCompleted - 1) % 4) + 1`
- **Predictable Phases**: Ensures moon phases cycle through levels 1-4 in a predictable pattern
<img width="303" height="624" alt="image" src="https://github.com/user-attachments/assets/d2e91c24-8631-4d43-8a01-7cc97a370cec" />


### 💾 **Enhanced Save File Display**
> [!WARNING]
> Be careful with mods that modify the save menu - there may be incompatibility with them.
- **Moon Information in Saves**: Shows moon icons and information in the save file selection screen

## Installation

1. Make sure you have [BepInEx](https://github.com/BepInEx/BepInEx) installed
2. Download the latest release from the releases page
3. Extract the mod files to your `BepInEx/plugins` folder
4. Launch the game

## Configuration

The mod works out of the box with no configuration required. The moon display will automatically appear when you're in a gameplay level (not in lobby, menu, tutorial, shop etc.).

## Technical Details
### Patched Methods
- `LevelGenerator.GenerateDone` - Triggers moon display setup
- `RunManager.CalculateMoonLevel` - Overrides moon level calculation
- `MenuPageSaves.SaveFileSelected` - Updates save file display

## Development

### Building from Source
```bash
dotnet build MoonPhaseUtils.csproj
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Changelog

### Version 1.0.0
- Initial release
- Moon information display in HUD
- Custom moon level calculation

## Support

If you encounter any issues or have suggestions for improvements, please open an issue on the GitHub repository.
