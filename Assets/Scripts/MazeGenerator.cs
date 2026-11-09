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

    [Header("Düşman Ayarları")]
    public GameObject enemyPrefab;  // Düşman prefabını buraya sürükle
    public int enemyCount = 5;      // Kaç düşman üretilecek?
    [Tooltip("Başlangıç noktasına bu kadar hücre yakınlıkta düşman/hazard çıkmaz")]
    public int safeCells = 3;

    [Header("Goal (Hedef) Ayarları")]
    public GameObject goalPrefab;   // Boş bırakılırsa otomatik bir hedef küpü yaratılır

    private int[,] mazeGrid;
    private List<Vector3> emptyPaths = new List<Vector3>(); // Boş yolların listesi

    void Start()
    {
        GenerateMazeData();
        BuildMaze();
        SpawnGoal();    // Önce goal, böylece onun hücresi spawn listesinden çıkar
        SpawnHazards(); // Labirent fiziksel olarak oluştuktan sonra topları yarat
        SpawnEnemies();
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
        SpawnRandom(hazardPrefab, hazardCount);
    }

    void SpawnEnemies()
    {
        SpawnRandom(enemyPrefab, enemyCount);
    }

    // Rastgele boş yollara prefab yerleştirir; başlangıca yakın hücreleri atlar
    void SpawnRandom(GameObject prefab, int count)
    {
        if (prefab == null) return;

        Vector3 start = new Vector3(1 * wallSize, 0, 0);
        float safeDistance = safeCells * wallSize;

        List<Vector3> candidates = emptyPaths.FindAll(p => Vector3.Distance(p, start) > safeDistance);
        int spawnLimit = Mathf.Min(count, candidates.Count);

        for (int i = 0; i < spawnLimit; i++)
        {
            int randomIndex = Random.Range(0, candidates.Count);
            Vector3 spawnPos = candidates[randomIndex];

            spawnPos.y += 0.5f;

            Instantiate(prefab, spawnPos, Quaternion.identity);

            emptyPaths.Remove(candidates[randomIndex]);
            candidates.RemoveAt(randomIndex);
        }
    }

    // Çıkış hücresine (sağ alt köşe) goal koyar
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
            // Prefab yoksa yeşil, görünür bir küp
            goal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goal.transform.position = exitPos + Vector3.up * 0.5f;
            goal.transform.localScale = Vector3.one * wallSize * 0.8f;
            goal.GetComponent<Renderer>().material.color = Color.green;
        }
        goal.name = "Goal";
        goal.transform.parent = this.transform;

        // Çarpışınca sahne geçişi için trigger + Goal scripti
        Collider col = goal.GetComponent<Collider>();
        if (col == null) col = goal.AddComponent<BoxCollider>();
        col.isTrigger = true;
        if (goal.GetComponent<Goal>() == null) goal.AddComponent<Goal>();
    }
}