using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.TerrainTools;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    SerializedProperty noiseShader;
    SerializedProperty terrainMaterial;
    SerializedProperty noiseResolution;
    SerializedProperty noiseScale;
    SerializedProperty noiseOffset;
    SerializedProperty mapScale;
    SerializedProperty chunks;

    protected virtual void OnEnable()
    {
        terrainMaterial = serializedObject.FindProperty("_terrainMaterial");
        noiseShader = serializedObject.FindProperty("_noiseShader");
        noiseResolution = serializedObject.FindProperty("_resolution");
        noiseScale = serializedObject.FindProperty("_noiseScale");
        noiseOffset = serializedObject.FindProperty("_offset");
        mapScale = serializedObject.FindProperty("_mapScale");
        chunks = serializedObject.FindProperty("_chunks");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TerrainGenerator myTarget = (TerrainGenerator)target;
        EditorGUILayout.LabelField("Generation Properties");
        EditorGUILayout.PropertyField(terrainMaterial, new GUIContent("Terrain Material"));
        EditorGUILayout.PropertyField(noiseShader, new GUIContent("Noise Shader"));
        EditorGUILayout.PropertyField(noiseResolution, new GUIContent("Noise Resolution"));
        EditorGUILayout.PropertyField(noiseScale, new GUIContent("Noise Scale"));
        EditorGUILayout.PropertyField(noiseOffset, new GUIContent("Noise Offset"));

        EditorGUILayout.LabelField("Map Properties");
        EditorGUILayout.PropertyField(mapScale, new GUIContent("Total Map Scale"));
        EditorGUILayout.PropertyField(chunks, new GUIContent("Number of Chunks"));

        if (GUILayout.Button("Generate Preview"))
        {
            myTarget.MakeMapAndCollider();
        }

        if (GUILayout.Button("Destroy Preview"))
        {
            myTarget.DestroyAllChildren();
        }

        serializedObject.ApplyModifiedProperties();
    }

}
