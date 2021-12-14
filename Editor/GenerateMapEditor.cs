using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateMap))]
public class GenerateMapEditor : Editor
{
    public override void OnInspectorGUI()
    {

        GenerateMap myTarget = (GenerateMap)target;
        EditorGUILayout.LabelField("Terrain Gen");
        // myTarget.scale = EditorGUILayout.Vector2Field("Coordinate Scale", myTarget.scale);
        // myTarget.UsePixelCollider = EditorGUILayout.Toggle("Use better collision", myTarget.UsePixelCollider);
        // myTarget.ColliderChillout = EditorGUILayout.Slider("Collision Chillout", myTarget.ColliderChillout, 0.0f, 1.0f);
        // myTarget.UseMesh = EditorGUILayout.Toggle("Use Mesh based graphics", myTarget.UseMesh);
       //myTarget.seed = EditorGUILayout.FloatField("Noise Seed", myTarget.seed);
        //myTarget.NoiseScale = EditorGUILayout.FloatField("Noise Scale", myTarget.NoiseScale);
        //myTarget.smoothness = EditorGUILayout.FloatField("Smoothness", myTarget.smoothness);
        myTarget.offset = EditorGUILayout.Vector2Field("Positional Offset", myTarget.offset);
        myTarget.size = EditorGUILayout.Vector2IntField("Texture Dimensions", myTarget.size);
        //myTarget.height = EditorGUILayout.FloatField("Height", myTarget.height);
        myTarget.TextureScale = EditorGUILayout.FloatField("Pixel per unit", myTarget.TextureScale);
        myTarget.ColliderAlpha = EditorGUILayout.Slider("Collider Alpha Cutoff", myTarget.ColliderAlpha, 0.0f, 1.0f);

        EditorGUILayout.LabelField("Shader");
        myTarget.scaleNoise = EditorGUILayout.Vector2Field("Scale Noise", myTarget.scaleNoise);
        myTarget.offsetNoise = EditorGUILayout.Vector2Field("Offset Noise", myTarget.offsetNoise);
        myTarget.computeShader = EditorGUILayout.ObjectField("Compute Shader", myTarget.computeShader, typeof(ComputeShader), true) as ComputeShader;
        myTarget.BackgroundShader = EditorGUILayout.ObjectField("Background Shader", myTarget.BackgroundShader, typeof(ComputeShader), true) as ComputeShader;
        myTarget.img = EditorGUILayout.ObjectField("Render texture", myTarget.img, typeof(RenderTexture), true) as RenderTexture;

        
        EditorGUILayout.LabelField("Terrain Visuals");
        myTarget.dirtMat = EditorGUILayout.ObjectField("Dirt Material", myTarget.dirtMat, typeof(Material), true) as Material;
        myTarget.dirtBackground = EditorGUILayout.ObjectField("Dirt Background", myTarget.dirtBackground, typeof(Material), true) as Material;
 /*
        EditorGUILayout.LabelField("Cave Gen");
        //myTarget.modifier = EditorGUILayout.FloatField("Modifier", myTarget.modifier);
        myTarget.modifier.x = EditorGUILayout.Slider("Modifier X", myTarget.modifier.x, 0.001f, 0.1f);
        myTarget.modifier.y = EditorGUILayout.Slider("Modifier Y", myTarget.modifier.y, 0.001f, 0.1f);
        myTarget.density = EditorGUILayout.Slider("Density", myTarget.density, 0.0f, 1.0f);
        myTarget.grad = EditorGUILayout.Slider("Gradient Strength", myTarget.grad, 0.0f, 1.0f);

        myTarget.modifierlh.x = EditorGUILayout.Slider("Modifier large caves X", myTarget.modifierlh.x, 0.0005f, 0.05f);
        myTarget.modifierlh.y = EditorGUILayout.Slider("Modifier large caves Y", myTarget.modifierlh.y, 0.0005f, 0.05f);
        myTarget.densitylh = EditorGUILayout.Slider("Density large caves", myTarget.densitylh, 0.0f, 1.0f);
        myTarget.gradlh = EditorGUILayout.Slider("Gradient Strength", myTarget.gradlh, 0.0f, 2.0f);
        myTarget.bias = EditorGUILayout.Slider("Bias", myTarget.bias, -1.0f, 1.0f);
        //myTarget.density = EditorGUILayout.FloatField("Density", myTarget.density);*/

        EditorGUILayout.LabelField("Map Properties");
        myTarget.chunks = EditorGUILayout.Vector2IntField("Chunks", myTarget.chunks);

        if (GUILayout.Button("Generate Map"))
        {
            myTarget.MakeMapAndCollider();
            EditorUtility.SetDirty(myTarget);
        }

        if (GUILayout.Button("Destroy Map"))
        {
            myTarget.DestroyAllChildren();
            // EditorUtility.SetDirty(myTarget);
        }
    }

}
