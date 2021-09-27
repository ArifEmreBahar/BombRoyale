using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform[] obstaclePrefab;

    [HideInInspector] public int mapWidth;
    [HideInInspector] public int mapHeight;
    [HideInInspector] public string[] nameMapCombine;
    [HideInInspector] public string[] nameMapVoronoi;
    [HideInInspector] public TerrainType[] terrainType;

    public float tileSize;

    [Range(0, 1)]
    public float outlinePercent;
    [Range(0, 1)]
    public float obstaclePercent;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    public int seed = 10;

    public bool autoUpdate;

    Coord mapCentre;

    public void GenerateMap()
    {
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
        mapCentre = new Coord((int)mapWidth / 2, (int)mapHeight / 2);

        string holderName = "Generated Map";
        string colliderName = "Map Collider";

        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        if (transform.Find(colliderName))
        {
            DestroyImmediate(transform.Find(colliderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        Transform mapCollider= new GameObject(colliderName).transform;
        mapCollider.parent = transform;
        mapCollider.Rotate(90, 0, 0);
        BoxCollider mapBoxCollider = mapCollider.gameObject.AddComponent<BoxCollider>();
        mapBoxCollider.size = new Vector3(mapWidth * tileSize, mapHeight * tileSize);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(FindTileType(new Vector3(x, 0, y)), tilePosition + Vector3.up * -.5f, Quaternion.identity) as Transform;
                newTile.parent = mapHolder;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
            }
        }

        for (int x = -1; x < mapWidth+1; x++)
        {
            if (x == -1 || x == mapWidth)
            {
                for (int y = -1; y < mapHeight+1; y++)
                {
                    Vector3 tilePosition = CoordToPosition(x, y);
                    Transform newTile = Instantiate(obstaclePrefab[0], tilePosition + Vector3.up * .5f, Quaternion.Euler(Vector3.right * 90)) as Transform;
                    newTile.parent = mapHolder;
                    newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                }
            }
            else
            {  
                for (int y = -1; y < mapHeight+1; y+=mapHeight+1)
                {              
                    Vector3 tilePosition = CoordToPosition(x, y);
                    Transform newTile = Instantiate(obstaclePrefab[0], tilePosition + Vector3.up * .5f, Quaternion.Euler(Vector3.right * 90)) as Transform;
                    newTile.parent = mapHolder;
                    newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                }
            }    
        }

        bool[,] obstacleMap = new bool[(int)mapWidth, (int)mapHeight];

        int obstacleCount = (int)(mapWidth * mapHeight * obstaclePercent);
        int currentObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(FindObstacleType(new Vector3(randomCoord.x,0,randomCoord.y)), obstaclePosition + Vector3.up * .5f, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        Debug.Log("Map Generated!");
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) {
		bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (mapCentre);
		mapFlags [mapCentre.x, mapCentre.y] = true;

		int accessibleTileCount = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue();

			for (int x = -1; x <= 1; x ++) {
				for (int y = -1; y <= 1; y ++) {
					int neighbourX = tile.x + x;
					int neighbourY = tile.y + y;
					if (x == 0 || y == 0) {
						if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) {
							if (!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX,neighbourY]) {
								mapFlags[neighbourX,neighbourY] = true;
								queue.Enqueue(new Coord(neighbourX,neighbourY));
								accessibleTileCount ++;
							}
						}
					}
				}
			}
		}

		int targetAccessibleTileCount = (int)(mapWidth * mapHeight - currentObstacleCount);
		return targetAccessibleTileCount == accessibleTileCount;
	}

	Vector3 CoordToPosition(int x, int y) {     
		return new Vector3 (-mapWidth / 2 + 0.5f + x, 0, -mapHeight / 2 + 0.5f + y) * tileSize;
	}

	public Coord GetRandomCoord() {
		Coord randomCoord = shuffledTileCoords.Dequeue ();
		shuffledTileCoords.Enqueue (randomCoord);
		return randomCoord;
	}


    Transform FindTileType(Vector3 tylePosition)
    {
        string name = nameMapVoronoi[(int)tylePosition.x * mapWidth + (int)tylePosition.z];

        for (int i = 0; i < terrainType.Length; i++)
        {
            if (terrainType[i].name == name)
            {
                if (terrainType[i].terrain.Length != 0)
                {
                    if (terrainType[i].terrain.Length == 1)
                    {
                        return terrainType[i].terrain[0];
                    }
                    else
                    {
                        return terrainType[i].terrain[Random.Range(0, terrainType[i].terrain.Length)];
                    }
                }
                else
                {
                    Debug.LogWarning("This name of object missing! Add terrain object. ;" + name);
                    return null;
                }
            }
        }
        Debug.LogWarning("There is no Terrain with this name! ;" + name);
        return null;
    }


    Transform FindObstacleType(Vector3 obstaclePosition)
    {
        string name = nameMapCombine[(int)obstaclePosition.x * mapWidth + (int)obstaclePosition.z];

        for (int i = 0; i < terrainType.Length; i++)
        {
            for (int j = 0; j < terrainType[i].terrainElements.Length; j++)
            {
                if (terrainType[i].terrainElements[j].name == name)
                {
                    if (terrainType[i].terrainElements.Length != 0)
                    {
                        if (terrainType[i].terrainElements.Length == 1)
                        {
                            return terrainType[i].terrainElements[j].element[0];
                        }
                        else
                        {
                            return terrainType[i].terrainElements[j].element[Random.Range(0, terrainType[i].terrainElements[j].element.Length)];
                        }
                    }
                    else
                    {
                        Debug.LogWarning("This name of object missing! Add obstacle object. ;" + name);
                        return null;
                    }
                }
            }
        }
        Debug.LogWarning("There is no Obstacle with this name! ; " +name);
        return null;
    }

	public struct Coord {
		public int x;
		public int y;

		public Coord(int _x, int _y) {
			x = _x;
			y = _y;
		}

		public static bool operator ==(Coord c1, Coord c2) {
			return c1.x == c2.x && c1.y == c2.y;
		}

		public static bool operator !=(Coord c1, Coord c2) {
			return !(c1 == c2);
		}

	}
}
