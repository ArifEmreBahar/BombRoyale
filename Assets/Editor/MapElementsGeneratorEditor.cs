
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapElementsGenerator))]
public class MapElementsGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapElementsGenerator mapGen = (MapElementsGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMapElements();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMapElements();
        }
    }
}