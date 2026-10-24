using UnityEngine;
using System.Collections.Generic;

public class ProceduralMazeGenerator : MonoBehaviour
{
    [Header("Labirent Ayarları")]
    public GameObject wallPrefab;
    public float wallSize = 2f;

    [Tooltip("Genişlik ve Derinlik mutlaka TEK SAYI olmalıdır (Örn: 31, 51, 101)")]
    public int width = 25;
    public int depth = 31;

    [Header("Hazard (Tehlike) Ayarları")]
    public GameObject hazardPrefab; // Top prefabını buraya sürükle
    public int hazardCount = 5;     // Kaç top üretilecek?

    private int[,] mazeGrid;
    private List<Vector3> emptyPaths = new List<Vector3>(); // Boş yolların listesi

    void Start()
    {
        GenerateMazeData();
        BuildMaze();
        SpawnHazards(); // Labirent fiziksel olarak oluştuktan sonra topları yarat
    }

    void GenerateMazeData()
    {
        mazeGrid = new int[depth, width];

        // Tüm alanı duvar ile doldur
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
                mazeGrid[z, x] = 1;

        // DFS Algoritması
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int startPos = new Vector2Int(1, 1);
        mazeGrid[startPos.y, startPos.x] = 0;
        stack.Push(startPos);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                stack.Push(current);

                Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];

                // Matematiksel hata düzeltildi (chosen - current)
                Vector2Int wall = current + (chosen - current) / 2;
                mazeGrid[wall.y, wall.x] = 0;
                mazeGrid[chosen.y, chosen.x] = 0;

                stack.Push(chosen);
            }
        }

        // Giriş ve Çıkış Noktalarını Belirle
        mazeGrid[0, 1] = 0;
        mazeGrid[depth - 1, width - 2] = 0;
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = { new Vector2Int(0, 2), new Vector2Int(0, -2), new Vector2Int(2, 0), new Vector2Int(-2, 0) };

        foreach (var dir in directions)
        {
            Vector2Int n = cell + dir;
            if (n.x > 0 && n.x < width - 1 && n.y > 0 && n.y < depth - 1)
            {
                if (mazeGrid[n.y, n.x] == 1)
                    neighbors.Add(n);
            }
        }
        return neighbors;
    }

    void BuildMaze()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                // Pozisyon hesaplama formülü düzeltildi (x * wallSize)
                Vector3 position = new Vector3(x * wallSize, 0, z * wallSize);

                if (mazeGrid[z, x] == 1)
                {
                    // Burası duvar, duvar yarat
                    GameObject newWall = Instantiate(wallPrefab, position, Quaternion.identity);
                    newWall.transform.parent = this.transform;
                }
                else if (mazeGrid[z, x] == 0)
                {
                    // YENİ: Burası boş yol, pozisyonunu listeye kaydet
                    emptyPaths.Add(position);
                }
            }
        }
    }

    void SpawnHazards()
    {
        int spawnLimit = Mathf.Min(hazardCount, emptyPaths.Count);

        for (int i = 0; i < spawnLimit; i++)
        {
            int randomIndex = Random.Range(0, emptyPaths.Count);
            Vector3 spawnPos = emptyPaths[randomIndex];

            spawnPos.y += 0.5f;

            Instantiate(hazardPrefab, spawnPos, Quaternion.identity);

            emptyPaths.RemoveAt(randomIndex);
        }
    }
}