using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        MapGenerator map = target as MapGenerator;

        if (DrawDefaultInspector())
        {
            if (map.autoUpdate)
            {
                map.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            map.GenerateMap();
        }
    }

}