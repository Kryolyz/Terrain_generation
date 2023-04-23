# Terrain_generation
A compute-shader based terrain + caves generation system for Unity2D

## Usage
The folder "Terrain_Generator" contains the project, it should be easy to export and import all necessary components into you own project if you want.

## Description
The script generates a given number of chunks for a terrain defined by a noise function compute shader. It generates grass on top, then dirt, then stone and then a glowing crystal-like material. The lower layers gradually flow into each other. For performance testing reasons the script currently executes the generation compute shader in the update function because it neatly shows hows powerful this approach is. Regenerating the entire map with several million pixels in the map-texture works above 60fps.

The generator also assign a material ID to the pixels in a second 3D texture (called "cellTypes" in the main script), which could theoretically be used to treat the pixels differently. I am going to use this for a project of mine but you may ignore or delete that part.

It should also be possible to assign a polygon collision component to each chunk and have it use the alpha shape of the texture to automatically generate a collision shape, though it is likely the resolution of the collision shape will be insufficent. 
