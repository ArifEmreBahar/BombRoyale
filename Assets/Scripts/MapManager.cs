using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    MapGenerator mapGenerator;
    MapElementsGenerator mapElementGenerator;


    private void Awake()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        mapElementGenerator = FindObjectOfType<MapElementsGenerator>();
    }

    private void Start()
    {
        mapElementGenerator.GenerateMapElements();
        mapGenerator.GenerateMap();
    }
}
