#ifndef UTILS
#define UTILS

#define INT_MAX 2147483647
#define FLT_MAX 3.402823466e+38F
#define FLT3_MAX float3(FLT_MAX, FLT_MAX, FLT_MAX)

int resolution;
float2 terrain_dims;

int3 adaptSlice(in int3 pos)
{
    int3 p = pos;
    if (p.x >= (resolution))
    {
        p.x -= (resolution);
        p.z += 1;
        if (p.z % terrain_dims.x == 0)
            p.z = INT_MAX;
    }
    else if (p.x < 0)
    {
        p.x += (resolution - 1);
        if (p.z % terrain_dims.x == 0)
            p.z = INT_MAX;
        else
            p.z -= 1;
    }
    
    if (p.y >= (resolution))
    {
        p.y -= (resolution);
        p.z += terrain_dims.x;
        if (p.z >= terrain_dims.x * terrain_dims.y)
            p.z = INT_MAX;
    }
    else if (p.y < 0)
    {
        p.y += (resolution - 1);
        p.z -= terrain_dims.x;
        if (p.z < 0)
            p.z = INT_MAX;
    }
    
    return p;
}

float3 adaptSlice(in float3 pos)
{
    float3 p = pos;
    if (p.x >= (resolution))
    {
        p.x -= (resolution);
        p.z += 1;
        if (p.z % terrain_dims.x == 0)
            p.z = FLT_MAX;
    }
    else if (p.x < 0)
    {
        p.x += (resolution - 1);
        if (p.z % terrain_dims.x == 0)
            p.z = FLT_MAX;
        else
            p.z -= 1;
    }
    
    if (p.y >= (resolution))
    {
        p.y -= (resolution);
        p.z += terrain_dims.x;
        if (p.z >= terrain_dims.x * terrain_dims.y)
            p.z = FLT_MAX;
    }
    else if (p.y < 0)
    {
        p.y += (resolution - 1);
        p.z -= terrain_dims.x;
        if (p.z < 0)
            p.z = FLT_MAX;
    }
    
    return p;
}

uint getSliceFromIdChunks(float2 chunks)
{
    int slice = chunks.x + chunks.y * terrain_dims.x;
    return slice;
}

uint getSliceFromChunks(float2 chunks)
{
    // y-chunks start at negative terrain_dims
    int slice = chunks.x + (chunks.y + terrain_dims.y) * terrain_dims.x;
    return slice;
}

float2 getChunksFromSlice(uint slice)
{
    int buffer = slice;
    int x = buffer % (terrain_dims.x);
    int y = buffer - x;
    y /= terrain_dims.x;
    return float2(x, y);
}

float2 getAbsolutePosition(float3 coord)
{
    float2 chunks = getChunksFromSlice(coord.z);
    return float2(coord.x + chunks.x * resolution, coord.y + chunks.y * resolution);
}

float2 getAbsolutePosition(int3 coord)
{
    float2 chunks = getChunksFromSlice(coord.z);
    return float2(coord.x + chunks.x * resolution, coord.y + chunks.y * resolution);
}

float getTotalDistance(float3 coord1, float3 coord2)
{
    return length(getAbsolutePosition(coord1) - getAbsolutePosition(coord2));
}

#endif