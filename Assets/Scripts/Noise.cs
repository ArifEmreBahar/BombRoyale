using UnityEngine;

public static class Noise 
{
    public static float[,] GeneratePerlinNoiseMap(int mapWidth, int mapHeight, float scale, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapWidth / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float sampleX = (x-halfWidth) / scale + offset.x;
                float sampleY = (y-halfHeight) / scale + offset.y;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }

    public static float[,] GenerateVoronoiNoiseMap(int mapWidth, int mapHeight, int regionAmount)
    {
        Vector2Int[] centroids = new Vector2Int[regionAmount];
        float[] regions = new float[regionAmount];

        for (int i = 0; i < regionAmount; i++)
        {
            centroids[i] = new Vector2Int(Random.Range(0, mapWidth), Random.Range(0, mapHeight));
            regions[i] = Random.Range(0f, 1f);
        }

        float[,] noiseMap = new float[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector2Int pixelPos = new Vector2Int(x, y);
                float smallestDst = float.MaxValue;
                int index = 0;
                for (int i = 0; i < centroids.Length; i++)
                {
                    if (Vector2.Distance(pixelPos, centroids[i]) < smallestDst)
                    {
                        smallestDst = Vector2.Distance(pixelPos, centroids[i]);
                        index = i;
                    }
                }
                noiseMap[x, y] = regions[index];
            }
        }
        return noiseMap;
    }
}
