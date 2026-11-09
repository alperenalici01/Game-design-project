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

    [Header("Çıkmaz Sokak (Ödül) Ayarları")]
    [Tooltip("Çıkmaz sokakların sonuna konulacak özel eşya (Örn: Büyük Elmas/Sandık)")]
    public GameObject specialItemPrefab;

    [Header("Hazard (Tehlike) Ayarları")]
    public GameObject hazardPrefab;
    public int hazardCount = 5;

    [Header("Düşman Ayarları")]
    public GameObject enemyPrefab;
    public int enemyCount = 5;
    [Tooltip("Başlangıç noktasına bu kadar hücre yakınlıkta düşman/hazard çıkmaz")]
    public int safeCells = 3;

    [Header("Goal (Hedef) Ayarları")]
    public GameObject goalPrefab;

    private int[,] mazeGrid;
    private List<Vector3> emptyPaths = new List<Vector3>();

    void Start()
    {
        GenerateMazeData();
        BuildMaze();

        SpawnGoal();
        SpawnItemsInDeadEnds();

        SpawnHazards();
        SpawnEnemies();
    }

    void GenerateMazeData()
    {
        mazeGrid = new int[depth, width];

        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
                mazeGrid[z, x] = 1;

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

                Vector2Int wall = current + (chosen - current) / 2;
                mazeGrid[wall.y, wall.x] = 0;
                mazeGrid[chosen.y, chosen.x] = 0;

                stack.Push(chosen);
            }
        }

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
                Vector3 position = new Vector3(x * wallSize, 0, z * wallSize);

                if (mazeGrid[z, x] == 1)
                {
                    GameObject newWall = Instantiate(wallPrefab, position, Quaternion.identity);
                    newWall.transform.parent = this.transform;
                }
                else if (mazeGrid[z, x] == 0)
                {
                    emptyPaths.Add(position);
                }
            }
        }
    }

    void SpawnItemsInDeadEnds()
    {
        if (specialItemPrefab == null) return;

        for (int z = 1; z < depth - 1; z++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if (mazeGrid[z, x] == 0)
                {
                    int wallCount = 0;

                    if (mazeGrid[z + 1, x] == 1) wallCount++; // Üst
                    if (mazeGrid[z - 1, x] == 1) wallCount++; // Alt
                    if (mazeGrid[z, x + 1] == 1) wallCount++; // Sağ
                    if (mazeGrid[z, x - 1] == 1) wallCount++; // Sol

                    if (wallCount == 3 && !(x == 1 && z == 1))
                    {
                        Vector3 spawnPos = new Vector3(x * wallSize, 0, z * wallSize);

                        GameObject item = Instantiate(specialItemPrefab, spawnPos + Vector3.up * 0.5f, Quaternion.identity);
                        item.transform.parent = this.transform;

                        emptyPaths.Remove(spawnPos);
                    }
                }
            }
        }
    }

    void SpawnHazards()
    {
        SpawnRandom(hazardPrefab, hazardCount);
    }

    void SpawnEnemies()
    {
        SpawnRandom(enemyPrefab, enemyCount);
    }

    void SpawnRandom(GameObject prefab, int count)
    {
        if (prefab == null) return;

        Vector3 start = new Vector3(1 * wallSize, 0, 1 * wallSize);
        float safeDistance = safeCells * wallSize;

        List<Vector3> candidates = emptyPaths.FindAll(p => Vector3.Distance(p, start) > safeDistance);
        int spawnLimit = Mathf.Min(count, candidates.Count);

        for (int i = 0; i < spawnLimit; i++)
        {
            int randomIndex = Random.Range(0, candidates.Count);
            Vector3 spawnPos = candidates[randomIndex];

            spawnPos.y += 0.5f;

            GameObject spawnedObj = Instantiate(prefab, spawnPos, Quaternion.identity);
            spawnedObj.transform.parent = this.transform; 

            emptyPaths.Remove(candidates[randomIndex]);
            candidates.RemoveAt(randomIndex);
        }
    }

    void SpawnGoal()
    {
        Vector3 exitPos = new Vector3((width - 2) * wallSize, 0, (depth - 1) * wallSize);
        emptyPaths.Remove(exitPos);

        GameObject goal;
        if (goalPrefab != null)
        {
            goal = Instantiate(goalPrefab, exitPos + Vector3.up * 0.5f, Quaternion.identity);
        }
        else
        {
            goal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goal.transform.position = exitPos + Vector3.up * 0.5f;
            goal.transform.localScale = Vector3.one * wallSize * 0.8f;
            goal.GetComponent<Renderer>().material.color = Color.green;
        }
        goal.name = "Goal";
        goal.transform.parent = this.transform;

        Collider col = goal.GetComponent<Collider>();
        if (col == null) col = goal.AddComponent<BoxCollider>();
        col.isTrigger = true;

        if (goal.GetComponent<Goal>() == null) goal.AddComponent<Goal>();
    }
}