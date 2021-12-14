# Terrain_generation
A compute-shader based terrain+caves generation system for Unity2D

## Usage
Drop the main file and editor+shaders folder into the assets folder of your project. Then add the GenerateMap object to an empty GameObject. The editor interface has, in addition to the generation settings, two buttons to generate or destroy the map in editor.
Note that the map is re-generated when running the game, meaning the pre-generated one can be considered preview and manually editing it won't affect the in-game terrain.

The terrain is generated through a compute shader. It has collision polygons and background sprites as visible in the exemplary images.