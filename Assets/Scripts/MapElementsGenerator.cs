using UnityEngine;
using System.Collections;

public class MapElementsGenerator : MonoBehaviour
{
    [Range(0, 100)]
    public int mapWidth;
    [Range(0, 100)]
    public int mapHeight;
    public float noiseScale;
    [Range(1, 10)]
    public int regionAmount;
    public Vector2 offset;

    public enum DrawMode { PerlinMap, PerlinColourMap, VoronoiMap, VoronoiColourMap, VPCombineColorMap };
    public DrawMode drawMode;

    public bool autoUpdate;

    public TerrainType[] regions;

    public void GenerateMapElements()
    {
        offset = new Vector2(Random.Range(-10000, 10000), Random.Range(-10000, 10000));
        CombineMapData combineVPMap = GenerateCombineVPMapData();

        MapGenerator mapGenerator = FindObjectOfType<MapGenerator>();
        if (mapGenerator != null)
        {
            mapGenerator.mapWidth = mapWidth;
            mapGenerator.mapHeight = mapHeight;
            mapGenerator.terrainType = regions;
            mapGenerator.nameMapCombine = combineVPMap.nameMapCombine;
            mapGenerator.nameMapVoronoi = combineVPMap.nameMapVoronoi;
        }       

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.PerlinMap)
        {
            MapData pelvinMap = GeneratePelvinMapData();
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(pelvinMap.heightMap));
        }
        else if (drawMode == DrawMode.PerlinColourMap)
        {
            MapData pelvinMap = GeneratePelvinMapData();
            display.DrawTexture(TextureGenerator.TextureFromColourMap(pelvinMap.colourMap, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.VoronoiMap)
        {
            MapData voronoiMap = GenerateVoronoiMapData();
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(voronoiMap.heightMap));
        }
        else if (drawMode == DrawMode.VoronoiColourMap)
        {
            MapData voronoiMap = GenerateVoronoiMapData();
            display.DrawTexture(TextureGenerator.TextureFromColourMap(voronoiMap.colourMap, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.VPCombineColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(combineVPMap.colourMapCombine, mapWidth, mapHeight));
        }

        Debug.Log("Map Elements Generated!");

    }

    MapData GeneratePelvinMapData()
    {
        float[,] noiseMap = Noise.GeneratePerlinNoiseMap(mapWidth, mapHeight, noiseScale, offset);
        Color[] colourMap = new Color[mapWidth * mapHeight];
        string[] nameMap = new string[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        nameMap[y * mapWidth + x] = regions[i].name;
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colourMap, nameMap);
    }

    MapData GenerateVoronoiMapData()
    {
        float[,] noiseMap = Noise.GenerateVoronoiNoiseMap(mapWidth, mapHeight, regionAmount);
        Color[] colourMap = new Color[mapWidth * mapHeight];
        string[] nameMap = new string[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        nameMap[y * mapWidth + x] = regions[i].name;
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colourMap, nameMap);
    }

    CombineMapData GenerateCombineVPMapData()
    {
        float[,] noiseMapP = Noise.GeneratePerlinNoiseMap(mapWidth, mapHeight, noiseScale, offset);
        float[,] noiseMapV = Noise.GenerateVoronoiNoiseMap(mapWidth, mapHeight, regionAmount);
        Color[] colourMapCombine = new Color[mapWidth * mapHeight];
        Color[] colourMapPelvin = new Color[mapWidth * mapHeight];
        Color[] colourMapVoronoi = new Color[mapWidth * mapHeight];
        string[] nameMapCombine = new string[mapWidth * mapHeight];
        string[] nameMapPelvin = new string[mapWidth * mapHeight];
        string[] nameMapVoronoi = new string[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeightP = noiseMapP[x, y];
                float currentHeightV = noiseMapV[x, y];


                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeightV <= regions[i].height)
                    {
                        for (int j = 0; j < regions[i].terrainElements.Length; j++)
                        {
                            if (currentHeightP <= regions[i].terrainElements[j].height)
                            {
                                colourMapCombine[y * mapWidth + x] = regions[i].terrainElements[j].colour;
                                nameMapCombine[y * mapWidth + x] = regions[i].terrainElements[j].name;
                                break;
                            }
                        }
                        break;
                    }  
                }

                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeightV <= regions[i].height)
                    {
                        colourMapVoronoi[y * mapWidth + x] = regions[i].colour;
                        nameMapVoronoi[y * mapWidth + x] = regions[i].name;
                        break;
                    }
                }


                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeightP <= regions[i].height)
                    {
                        colourMapPelvin[y * mapWidth + x] = regions[i].colour;
                        nameMapPelvin[y * mapWidth + x] = regions[i].name;
                        break;
                    }
                }
            }
        }
                return new CombineMapData(noiseMapP,noiseMapV,colourMapCombine,colourMapPelvin,colourMapVoronoi,nameMapCombine,nameMapPelvin,nameMapVoronoi);
    }

    void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (regionAmount < 1)
        {
            mapHeight = 1;
        }

    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
    public Transform[] terrain;
    public TerrainElements[] terrainElements;
}

[System.Serializable]
public struct TerrainElements
{
    public string name;
    public float height;
    public Color colour;
    public Transform[] element;
}


public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;
    public readonly string[] nameMap;

    public MapData(float[,] heightMap, Color[] colourMap, string[] nameMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
        this.nameMap = nameMap;
    }
}


public struct CombineMapData
{
    public readonly float[,] heightMapPelvin;
    public readonly float[,] heightMapVoronoi;
    public readonly Color[] colourMapCombine;
    public readonly Color[] colourMapPelvin;
    public readonly Color[] colourMapVoronoi;
    public readonly string[] nameMapCombine;
    public readonly string[] nameMapPelvin;
    public readonly string[] nameMapVoronoi;

    public CombineMapData(float[,] heightMapPelvin, float[,] heightMapVoronoi, Color[] colourMapCombine, Color[] colourMapPelvin, Color[] colourMapVoronoi, string[] nameMapCombine, string[] nameMapPelvin, string[] nameMapVoronoi)
    {
        this.heightMapPelvin = heightMapPelvin;
        this.heightMapVoronoi = heightMapVoronoi;
        this.colourMapCombine = colourMapCombine;
        this.colourMapPelvin = colourMapPelvin;
        this.colourMapVoronoi = colourMapVoronoi;
        this.nameMapCombine = nameMapCombine;
        this.nameMapPelvin = nameMapPelvin;
        this.nameMapVoronoi = nameMapVoronoi;
    }
}
