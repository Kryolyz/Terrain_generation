using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _noiseShader;

    [SerializeField]
    private Vector2 _offset;
    [SerializeField]
    private int _resolution = 128;
    [SerializeField]
    private float _mapScale = 5.0f;
    [SerializeField]
    private Vector2 _noiseScale = new Vector2(1.0f, 1.0f);
    [SerializeField]
    private Vector2Int _chunks = new Vector2Int(6, 10);

    private RenderTexture _terrainTex;
    [SerializeField]
    private Material _terrainMaterial;
    private RenderTexture _cellTypes; // For treating pixels differently based on their type

    public Action OnMapFinishedLoading;
    public Action OnTerrainChanged;

    public void MakeMapAndCollider()
    {
        if (transform.childCount > 0)
        {
            DestroyAllChildren();
        }

        var t = DateTime.Now;

        if (_terrainTex)
            _terrainTex.Release();
        _terrainTex = new RenderTexture(_resolution, _resolution, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat);
        _terrainTex.dimension = TextureDimension.Tex2DArray;
        _terrainTex.volumeDepth = _chunks.x * _chunks.y;
        _terrainTex.enableRandomWrite = true;
        _terrainTex.filterMode = FilterMode.Point;
        _terrainTex.Create();

        if (_cellTypes)
            _cellTypes.Release();
        _cellTypes = new RenderTexture(_resolution, _resolution, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat);
        _cellTypes.dimension = TextureDimension.Tex2DArray;
        _cellTypes.volumeDepth = _chunks.x * _chunks.y;
        _cellTypes.enableRandomWrite = true;
        _cellTypes.filterMode = FilterMode.Point;
        _cellTypes.Create();

        Debug.Assert(_terrainMaterial, "Terrain Material not found");

        Mesh quad = CreateQuad(_mapScale, _mapScale);
        MeshRenderer mr = new MeshRenderer();

        int createMapKernelId = _noiseShader.FindKernel("CreateMap");
        _noiseShader.SetTexture(createMapKernelId, "Pixels", _terrainTex);
        _noiseShader.SetTexture(createMapKernelId, "CellTypes", _cellTypes);
        _noiseShader.SetInt("resolution", _resolution);
        _noiseShader.SetVector("scale", _noiseScale);
        _noiseShader.SetVector("offset", _offset);
        _noiseShader.SetVector("terrain_dims", new Vector2(_chunks.x, _chunks.y));

        // Generate whole texture
        _noiseShader.Dispatch(createMapKernelId, _resolution * _chunks.x / 8, _resolution * _chunks.y / 8, 1);

        // Create Chunk GameObjects
        for (int x = 0; x < _chunks.x; ++x)
        {
            for (int y = -_chunks.y; y < 0; ++y)
            {
                GameObject child = new GameObject();
                Vector2 current_chunk = new Vector2(x, y);
                child.layer = 7; //terrain layer
                child.name = "Chunk" + x.ToString() + y.ToString() + "-" + (x + (y + _chunks.y) * _chunks.x).ToString();
                child.transform.parent = this.transform;

                MeshFilter mf = child.AddComponent(typeof(MeshFilter)) as MeshFilter;
                mf.mesh = quad;
                mr = child.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

#if UNITY_EDITOR
                var tempmat = new Material(_terrainMaterial);
                mr.sharedMaterial = tempmat;
                mr.sharedMaterial.SetInt("_Slice", ToChunkIndex(current_chunk.x, current_chunk.y + _chunks.y));
#else
                mr.sharedMaterial = ComplexLighting;
                mr.materials[0].SetInt("_Slice", ToChunkIndex(current_chunk.x, current_chunk.y + chunks.y));
#endif

                child.transform.localPosition = new Vector3((x - (_chunks.x - 1.0f) / 2) * _mapScale, y * _mapScale, 0.0f);
                mr.sharedMaterial.SetTexture("_MainTex", _terrainTex);
                mr.sharedMaterial.SetFloat("_UVScale", 1 / _mapScale);
                mr.sharedMaterial.SetInt("_TerrainWidth", _chunks.x);
            }
        }

        var dt = System.DateTime.Now - t;
        Debug.Log($"Generated Map in: {dt.Milliseconds} ms");
        OnMapFinishedLoading?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        MakeMapAndCollider();
    }

    private void Update()
    {
        int createMapKernelId = _noiseShader.FindKernel("CreateMap");
        _offset += new Vector2(Time.deltaTime * 1, 0);
        _noiseShader.SetVector("offset", _offset);
        _noiseShader.Dispatch(createMapKernelId, _resolution * _chunks.x / 8, _resolution * _chunks.y / 8, 1);
    }

    public int ToIndex(int x, int y)
    {
        return x + y * _resolution;
    }

    public int ToIndex(Vector2Int xy)
    {
        return xy.x + xy.y * _resolution;
    }

    public Vector2Int FromIndex(int i)
    {
        int x = i % _resolution;
        int y = (i - x) / _resolution;
        return new Vector2Int(x, y);
    }

    public int ToChunkIndex(int x, int y)
    {
        return x + y * _chunks.x;
    }

    public int ToChunkIndex(float x, float y)
    {
        return (int)(x + y * _chunks.x);
    }

    public int ToChunkIndex(Vector2 c)
    {
        return (int)(c.x + c.y * _chunks.x);
    }

    Mesh CreateQuad(float width, float height)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(0, height, 0),
            new Vector3(width, height, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        mesh.uv = uv;

        return mesh;
    }
    public void DestroyAllChildren()
    {
        int count = 0;
        do
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
            count += 1;
            if (count > 10) break;
        } while (transform.GetComponent<Transform>());
    }
}
