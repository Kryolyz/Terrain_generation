using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralNoiseProject;

public class GenerateMap : MonoBehaviour
{

    //terrain gen
    public Vector2 offset = new Vector2(0.0f, 0.0f);
    public Vector2Int size = new Vector2Int(300, 300);
    public float TextureScale = 100.0f;

    //Terrain gen
    public Vector2 scale = new Vector2(0.01f, 0.01f);

    public Material dirtMat;
    public Material dirtBackground;

    //map gen
    public Vector2Int chunks = new Vector2Int(6, 4);

    //noise function
    AccidentalNoise.ModuleBase TerrainNoise;

    //collider
    public float ColliderAlpha = 0.5f;
    public float ColliderChillout = 2.0f;

    //Shader stuff
    public ComputeShader computeShader;
    public ComputeShader BackgroundShader;
    public Vector2 scaleNoise = new Vector2(1.0f,1.0f);
    public Vector2 offsetNoise = new Vector2(0.0f, 0.0f);  
    public RenderTexture img;
    public RenderTexture imgBack;
    Vector2 current_chunk;

    // Start is called before the first frame update
    void Start()
    {
        MakeMapAndCollider();
    }

    public void MakeMapAndCollider()
    {
        //MapNoise noise = GetComponent<MapNoise>();
        //TerrainNoise = noise.Terrain();
        DestroyAllChildren();
        Texture2D map = CreateTexture2D(size.x, size.y, new Color(0,0,0,0));

        for (int x = 0; x < chunks.x; x++)
        {
            for (int y = 0; y < chunks.y; y++)
            {
                map = CreateTexture2D(size.x, size.y, new Color(0,0,0,0));
                current_chunk = new Vector2(x,y);
                //Debug.Log(OffsetNoise);
                img = MakeTerrainTexture();
                RenderTexture.active = img;
                map.ReadPixels(new Rect(0, 0, img.width, img.height), 0, 0, false);
                map.Apply();
                
                
                //Texture2D map = TerrainGenerator(x,y);
                GameObject child = new GameObject();
                child.layer = 7; //terrain layer
                child.name = "Chunk" + x.ToString() + y.ToString();
                child.transform.parent = this.transform;
                Vector2 displ = new Vector2(chunks.x,chunks.y);
                SpriteRenderer renderer = child.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                renderer.sprite = Sprite.Create(map, new Rect(0.0f, 0.0f, map.width, map.height), new Vector2(0.5f, 0.5f), TextureScale);

                child.AddComponent(typeof(PolygonCollider2D));

                DestroyImmediate(child.GetComponent<SpriteRenderer>());
                child.AddComponent(typeof(MeshFilter));
                child.AddComponent(typeof(MeshRenderer));
                child.GetComponent<MeshRenderer>().material = dirtMat;
                child.GetComponent<MeshFilter>().mesh = child.GetComponent<PolygonCollider2D>().CreateMesh(false, false);
                child.transform.localPosition = new Vector3((x - (displ.x - 1.0f) / 2) * size.x / TextureScale, (y + 1.0f / 2) * size.y / TextureScale, 0.0f);//new Vector3(0,0,0);
                // child.GetComponent<PolygonCollider2D>().offset = new Vector2((x - (displ.x - 1.0f) / 2) * size.x / TextureScale, (y + 1.0f / 2) * size.y / TextureScale);

                // Mesh m = child.GetComponent<PolygonCollider2D>().CreateMesh(false, false);

                //remove empty polygons
                PolygonCollider2D ce = child.GetComponent<PolygonCollider2D>();
                if (ce.shapeCount == 1 && ce.GetTotalPointCount() == 5 && ce.bounds.extents.x < size.x/TextureScale/2 && ce.bounds.extents.y < size.y/TextureScale/2)
                {
                    DestroyImmediate(child);
                    continue;
                }
                
                // Sprite s = Sprite.Create(map, new Rect(0.0f, 0.0f, map.width, map.height), new Vector2(0.5f, 0.5f), TextureScale, 0);
                // s.OverrideGeometry(ConvertArrayV3V2(m.vertices), ConvertArrayIntUshort(m.triangles));
                // child.GetComponent<SpriteRenderer>().sprite = s;
                
                GameObject background = new GameObject();
                // background.layer = 7; //terrain layer
                background.name = "BackgroundChunk" + x.ToString() + y.ToString();
                background.transform.parent = child.transform;
                renderer = background.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                imgBack = MakeBackgroundTexture();
                RenderTexture.active = imgBack;
                map = CreateTexture2D(size.x, size.y, new Color(0,0,0,0));
                map.ReadPixels(new Rect(0, 0, imgBack.width, imgBack.height), 0, 0, false);
                map.Apply();
                renderer.sprite = Sprite.Create(map, new Rect(0.0f, 0.0f, map.width, map.height), new Vector2(0.5f, 0.5f), TextureScale);
                background.transform.localPosition = new Vector3(0.0f,0.0f,0.1f);
                renderer.sortingOrder = 5;
                renderer.material = dirtBackground;
            }
        }


        //PolygonCollider2D collider = GetComponent(typeof(PolygonCollider2D)) as PolygonCollider2D;
        //DestroyImmediate(collider);
        //gameObject.AddComponent(typeof(PolygonCollider2D));
        //SpriteRenderer renderer = AddChild
        //GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
    }

    public Texture2D TerrainGenerator(int ChunkX, int ChunkY)
    {
        Texture2D map = CreateTexture2D(size.x, size.y, new Color(0,0,0,0));
        Color[] cols = map.GetPixels(0);

        Color dirt = new Color(0.609f/2, 0.476f/2, 0.367f/2, 1); //dirt brown
        Color clear = new Color(0, 0, 0, 0); //transparent

        /*
        float perlinHeight;
        float caveValue;
        float cavelhvalue;
        */

        double val = 0;

        for (int x = 0; x < size.x; x++)
        {

            //perlinHeight = Mathf.PerlinNoise(Offset.x + x / smoothness, seed) * height + Offset.y;
            //if (ChunkY

            for (int y = 0; y < size.y; y++)
            {

                val = TerrainNoise.Get((x + ChunkX * size.x + offset.x)*scale.x, ((chunks.y*size.y) - (y + ChunkY * size.y + offset.y))*scale.y);
                //dirt[3] = (float)val;
                //cols[x + y * size.x] = dirt;

                if (val > 0.5)
                {
                    cols[x + y * size.x] = dirt;
                }
                else
                {
                    cols[x + y * size.x] = clear;
                }

                /*if (y < perlinHeight || ChunkY < chunks.y-1)
               {
                   //ChunkX*size.x
                   //ChunkX*size.x + size.x
                   //caveValue = Mathf.PerlinNoise(((x + ChunkX * size.x) * modifier.x) + seed, ((y + ChunkY * size.y) * modifier.y) + seed) + grad * (size.y * chunks.y - (y + ChunkY * size.y)) / (size.y * chunks.y);



                  if (caveValue < density)
                   {
                       //cavelhvalue = Mathf.PerlinNoise(((x + ChunkX*size.x) * modifierlh.x) + seed, ((y + ChunkY*size.y) * modifierlh.y) + seed) + gradlh * (size.y*chunks.y - (y+ChunkY*size.y)) / (size.y*chunks.y) + bias;
                       if (cavelhvalue < densitylh)
                       {
                           cols[x + y * size.x] = dirt;
                       }
                       else
                       {
                           cols[x + y * size.x] = clear;
                       }
                   }
                   else
                   {
                       cols[x + y * size.x] = clear;
                   }
               }
               else
               {
                   cols[x + y * size.x] = clear;
               }*/
                
            }
        }

        map.SetPixels(cols);
        map.Apply(false);
        return map;
    }

    public static Texture2D CreateTexture2D(int width, int height, Color color)
    {
        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, true);
        tex.SetPixel(0, 0, color);
        tex.Resize(width, height);
        tex.Apply(false);
        return tex;
    }

    public void DestroyAllChildren()
    {
        int count = 0;
        do
        {
            foreach (Transform child in transform)
            {
                //Debug.Log(count);
                DestroyImmediate(child.gameObject);
            }
            count += 1;
            if (count > 10) break;
        } while (transform.GetComponent<Transform>());
        img = null;
    }

    public void ChilloutCollision(GameObject child, float LenThreshold)
    {
        PolygonCollider2D collider = child.GetComponent<PolygonCollider2D>();

        Vector2 buffer1;
        Vector2 buffer2;
        //Vector2 buffer3;

        Vector2 Diff1;
        //Vector2 Diff2;
        
        float mag1;
        //float mag2;
        //float angle1;
        //float angle2;

        if (LenThreshold >= 1.0f) LenThreshold = 1/TextureScale + 1/(3*TextureScale);
        //int Paths = collider.pathCount;

        for (int n = 0; n < collider.pathCount; n++)
        {
            Vector2[] colp = collider.GetPath(n);
            Vector2[] newColp = new Vector2[colp.Length];

            buffer1 = colp[0];
            newColp[0] = colp[0];
            newColp[1] = colp[1];

            int lenCounter = 2;

            for (int i = 2; i < colp.Length; i++)
            {
                // buffer3 = colp[i-2];
                buffer2 = colp[i-1];
                buffer1 = colp[i];

                Diff1 = buffer2 - buffer1;
                // Diff2 = buffer3 - buffer1;

                mag1 = Diff1.magnitude;
                // mag2 = Diff2.magnitude;

                //angle1 = Vector2.Angle(Diff1, Diff2);
                //Debug.Log(Mathf.Abs((Mathf.Abs(angle1) - 90.0f)));
                //Debug.Log(mag1);
                if (mag1 > LenThreshold)
                {
                    newColp[lenCounter] = colp[i];
                    lenCounter += 1;
                    
                }
                // else if (mag2 > LenThreshold)
                // {
                //     newColp[lenCounter] = colp[i];
                //     lenCounter += 1;
                // }
            }

            Vector2[] p = new Vector2[lenCounter];
            for (int i = 0; i < lenCounter; i++)
            {
                p[i] = newColp[i];
            }
            child.GetComponent<PolygonCollider2D>().SetPath(n,p);

        }

        


    }

    public RenderTexture MakeTerrainTexture()
    {
        RenderTexture img = new RenderTexture(size.x, size.y, 4);
        img.enableRandomWrite = true;
        img.Create();

        computeShader.SetTexture(0, "Result", img);
        computeShader.SetFloat("resolution", img.width);
        computeShader.SetVector("scale", scaleNoise);
        computeShader.SetVector("offset", offsetNoise - new Vector2(0, chunks.y));
        computeShader.SetVector("current_chunk", current_chunk);
        computeShader.Dispatch(0, img.width, img.height, 1);
        return img;
    }

    public RenderTexture MakeBackgroundTexture()
    {
        RenderTexture img = new RenderTexture(size.x, size.y, 4);
        img.enableRandomWrite = true;
        img.Create();

        BackgroundShader.SetTexture(0, "Result", img);
        BackgroundShader.SetFloat("resolution", img.width);
        BackgroundShader.SetVector("scale", scaleNoise);
        BackgroundShader.SetVector("offset", offsetNoise - new Vector2(0, chunks.y));
        BackgroundShader.SetVector("current_chunk", current_chunk);
        BackgroundShader.Dispatch(0, img.width, img.height, 1);
        return img;
    }

    Vector2[] ConvertArrayV3V2(Vector3[] v3)
    {
        Vector2 [] v2 = new Vector2[v3.Length];
        for(int i = 0; i <  v3.Length; i++)
        {
            Vector3 tempV3 = v3[i];
            v2[i] = new Vector2(tempV3.x, tempV3.y);
        }
        return v2;
    }
    
    ushort[] ConvertArrayIntUshort(int[] v3)
    {
        ushort [] v2 = new ushort[v3.Length];
        for(int i = 0; i <  v3.Length; i++)
        {
            v2[i] = (ushort)v3[i];
        }
        return v2;
    }

    // private Mesh SpriteToMesh(Sprite sprite)
    // {
    //     Mesh mesh = new Mesh();
    //     mesh.SetVertices(Array.ConvertAll(sprite.vertices, i => (Vector3)i).ToList());
    //     mesh.SetUVs(0,sprite.uv.ToList());
    //     mesh.SetTriangles(Array.ConvertAll(sprite.triangles, i => (int)i),0);

    //     return mesh;
    // }


}