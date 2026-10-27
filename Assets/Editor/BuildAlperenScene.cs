using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BuildAlperenScene
{
    private const string ScenePath = "Assets/Scenes/alperen scene.unity";
    private const string MaterialsPath = "Assets/Scenes/AlperenLevelMaterials.asset";
    private const string CharacterPrefabPath = "Assets/ProjectPrefab/Tunahan/Mario.prefab";
    private const string WallPrefabPath = "Assets/Maze/Prefabs/Wall_2M.prefab";
    private const string TurtlePrefabPath = "Assets/ProjectPrefab/Ilteris/PF_TurtlePatrol.prefab";

    [MenuItem("Tools/Build Alperen Scene")]
    public static void Build()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer < 0) throw new System.InvalidOperationException("The Ground layer is missing.");
        GameObject characterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CharacterPrefabPath);
        GameObject wallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(WallPrefabPath);
        GameObject turtlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TurtlePrefabPath);
        if (characterPrefab == null || wallPrefab == null || turtlePrefab == null)
            throw new System.InvalidOperationException("An Ilteris character, turtle, or wall prefab could not be loaded.");

        Material[] materials = CreateMaterials();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientLight = new Color(0.72f, 0.78f, 0.84f);

        Transform level = new GameObject("Level").transform;
        Transform platforms = CreateGroup("Platforms", level);
        Transform coins = CreateGroup("Coins", level);
        Transform enemies = CreateGroup("Enemies", level);
        Transform hazards = CreateGroup("Hazards", level);
        Transform decoration = CreateGroup("Decoration", level);

        CreatePlatform("Start Island", new Vector3(0f, -0.5f, 3f), new Vector3(16f, 1f, 22f), materials[0], materials[1], platforms, groundLayer, wallPrefab);
        CreatePlatform("Step 01", new Vector3(0f, 0.1f, 17f), new Vector3(9f, 0.8f, 5f), materials[0], materials[1], platforms, groundLayer, wallPrefab);
        CreatePlatform("Step 02", new Vector3(-2f, 0.75f, 23f), new Vector3(8f, 1.3f, 5f), materials[0], materials[1], platforms, groundLayer, wallPrefab);
        CreatePlatform("Bridge", new Vector3(1f, 0.15f, 29.5f), new Vector3(9f, 0.7f, 6f), materials[0], materials[1], platforms, groundLayer, wallPrefab);
        CreatePlatform("Spikes Platform", new Vector3(0f, 0.15f, 36f), new Vector3(13f, 0.7f, 7f), materials[0], materials[1], platforms, groundLayer, wallPrefab);
        CreatePlatform("Turtle Island", new Vector3(0f, 0.15f, 44f), new Vector3(15f, 0.7f, 8f), materials[0], materials[1], platforms, groundLayer, wallPrefab);
        CreatePlatform("Step 03", new Vector3(2f, 0.55f, 52f), new Vector3(8f, 1.1f, 4f), materials[0], materials[1], platforms, groundLayer, wallPrefab);
        CreatePlatform("Step 04", new Vector3(-1.5f, 1.15f, 58f), new Vector3(8f, 1.3f, 4f), materials[0], materials[1], platforms, groundLayer, wallPrefab);
        CreatePlatform("Finish Island", new Vector3(0f, 0.55f, 66f), new Vector3(16f, 1.1f, 13f), materials[0], materials[1], platforms, groundLayer, wallPrefab);

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0f, 1.15f, 0f);
        player.transform.SetParent(level, true);
        player.GetComponent<MeshRenderer>().enabled = false;
        CapsuleCollider playerCollider = player.GetComponent<CapsuleCollider>();
        playerCollider.height = 1.8f;
        playerCollider.radius = 0.42f;
        Rigidbody playerBody = player.AddComponent<Rigidbody>();
        playerBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        playerBody.interpolation = RigidbodyInterpolation.Interpolate;
        playerBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        player.AddComponent<PlayerMove>();
        PlayerJump jump = player.AddComponent<PlayerJump>();
        SerializedObject jumpData = new SerializedObject(jump);
        jumpData.FindProperty("jumpForce").floatValue = 6.3f;
        jumpData.FindProperty("groundLayers").intValue = 1 << groundLayer;
        jumpData.ApplyModifiedPropertiesWithoutUndo();

        GameObject character = PrefabUtility.InstantiatePrefab(characterPrefab) as GameObject;
        character.name = "Ilteris Character";
        character.transform.SetParent(player.transform, false);
        character.transform.localPosition = Vector3.zero;
        character.transform.localRotation = Quaternion.identity;
        character.transform.localScale = Vector3.one;

        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = player.transform.position + new Vector3(0f, 7f, -11f);
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.fieldOfView = 58f;
        camera.nearClipPlane = 0.1f;
        CameraFollow cameraFollow = cameraObject.AddComponent<CameraFollow>();
        SerializedObject cameraData = new SerializedObject(cameraFollow);
        cameraData.FindProperty("target").objectReferenceValue = player.transform;
        cameraData.FindProperty("offset").vector3Value = new Vector3(0f, 7f, -11f);
        cameraData.ApplyModifiedPropertiesWithoutUndo();

        CreateCoin(new Vector3(0f, 1.5f, 6f), coins, materials[4]);
        CreateCoin(new Vector3(-2f, 2.3f, 15.5f), coins, materials[4]);
        CreateCoin(new Vector3(0f, 2.1f, 18f), coins, materials[4]);
        CreateCoin(new Vector3(-2f, 2.6f, 23f), coins, materials[4]);
        CreateCoin(new Vector3(1f, 1.7f, 29f), coins, materials[4]);
        CreateCoin(new Vector3(0f, 2f, 40.5f), coins, materials[4]);
        CreateCoin(new Vector3(2f, 2.6f, 52f), coins, materials[4]);
        CreateCoin(new Vector3(-1.5f, 3.3f, 58f), coins, materials[4]);
        CreateCoin(new Vector3(0f, 2.8f, 63f), coins, materials[4]);
        CreateCoin(new Vector3(0f, 2.8f, 66f), coins, materials[4]);

        GameObject enemy = PrefabUtility.InstantiatePrefab(turtlePrefab) as GameObject;
        enemy.name = "Turtle_01";
        enemy.transform.SetParent(enemies, false);
        enemy.transform.localPosition = new Vector3(0f, 0.51f, 43f);
        enemy.transform.localRotation = Quaternion.identity;
        TurtlePatrol patrol = enemy.GetComponentInChildren<TurtlePatrol>(true);
        if (patrol == null) throw new System.InvalidOperationException("The Ilteris turtle prefab has no patrol script.");
        patrol.gameObject.AddComponent<StompableEnemy>();

        CreateSpikeTrap(hazards);
        CreateExit(decoration, materials[7]);
        CreateCube("Fall Reset", new Vector3(0f, -6f, 35f), new Vector3(60f, 1f, 120f), materials[6], hazards, true, -1, true).AddComponent<KillZone>();

        CreateIlterisDecor("Assets/ProjectPrefab/Ilteris/Bridge_Small.fbx", "Ilteris Bridge", new Vector3(1f, 0.5f, 29.5f), new Vector3(8f, 0.8f, 5f), decoration);
        CreateIlterisDecor("Assets/ProjectPrefab/Ilteris/Tree_Fruit.fbx", "Ilteris Tree", new Vector3(-6f, 0.1f, 5f), new Vector3(3f, 4f, 3f), decoration);
        CreateIlterisDecor("Assets/ProjectPrefab/Ilteris/Tree.fbx", "Ilteris Tree", new Vector3(6f, 0.1f, 8f), new Vector3(3f, 4f, 3f), decoration);
        CreateIlterisDecor("Assets/ProjectPrefab/Ilteris/rocks.fbx", "Ilteris Rocks", new Vector3(-5.4f, 0.55f, 43f), new Vector3(2f, 1.2f, 2f), decoration);
        CreateIlterisDecor("Assets/ProjectPrefab/Ilteris/Grass_3.fbx", "Ilteris Grass", new Vector3(5.5f, 0.55f, 45f), new Vector3(1.8f, 1.2f, 1.8f), decoration);
        CreateIlterisDecor("Assets/ProjectPrefab/Ilteris/flowers.fbx", "Ilteris Flowers", new Vector3(5f, 0.6f, 66f), new Vector3(1.2f, 1.2f, 1.2f), decoration);
        CreateCloud(new Vector3(-9f, 7f, 12f), decoration, materials[8]);
        CreateCloud(new Vector3(9f, 8f, 27f), decoration, materials[8]);
        CreateCloud(new Vector3(-10f, 7f, 49f), decoration, materials[8]);
        CreateCube("Start Banner", new Vector3(0f, 3.2f, 8f), new Vector3(0.35f, 3.2f, 0.35f), materials[3], decoration, false);
        CreateCube("Start Banner Top", new Vector3(0f, 4.7f, 8f), new Vector3(4f, 0.4f, 0.4f), materials[3], decoration, false);

        GameObject managerObject = new GameObject("Level Manager");
        LevelManager manager = managerObject.AddComponent<LevelManager>();
        SerializedObject managerData = new SerializedObject(manager);
        managerData.FindProperty("player").objectReferenceValue = playerBody;
        managerData.FindProperty("coinsParent").objectReferenceValue = coins;
        managerData.FindProperty("enemiesParent").objectReferenceValue = enemies;
        managerData.ApplyModifiedPropertiesWithoutUndo();

        GameObject sunlight = new GameObject("Directional Light", typeof(Light));
        sunlight.transform.rotation = Quaternion.Euler(48f, -28f, 0f);
        Light sun = sunlight.GetComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.25f;
        sun.color = new Color(1f, 0.93f, 0.78f);

        ValidateContents(player, manager, coins, enemies);
        EditorSceneManager.SaveScene(scene, ScenePath);
        UpdateBuildSettings();
        AssetDatabase.SaveAssets();
        Debug.Log("Created playable 3D platform level at " + ScenePath);
    }

    [MenuItem("Tools/Validate Alperen Scene")]
    public static void ValidateSavedScene()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject player = GameObject.Find("Player");
        LevelManager manager = Object.FindFirstObjectByType<LevelManager>();
        Transform coins = GameObject.Find("Coins")?.transform;
        Transform enemies = GameObject.Find("Enemies")?.transform;
        if (!scene.IsValid() || player == null || manager == null || coins == null || enemies == null)
            throw new System.InvalidOperationException("The saved platform level is missing required objects.");
        ValidateContents(player, manager, coins, enemies);
        Debug.Log("Validated saved platform level: player, movement, jump, coins, enemies, hazards and exit are present.");
    }

    private static void ValidateContents(GameObject player, LevelManager manager, Transform coins, Transform enemies)
    {
        if (player.GetComponent<Rigidbody>() == null || player.GetComponent<PlayerMove>() == null || player.GetComponent<PlayerJump>() == null)
            throw new System.InvalidOperationException("Player physics or movement components are missing.");
        if (player.transform.Find("Ilteris Character") == null)
            throw new System.InvalidOperationException("The Ilteris character visual is missing.");
        if (Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Count(item => item.name == "Wall Rail") < 30)
            throw new System.InvalidOperationException("The expanded platforms are missing the Ilteris wall rails.");
        if (coins.GetComponentsInChildren<CoinPickup>(true).Length < 8)
            throw new System.InvalidOperationException("The level needs at least eight working coins.");
        if (enemies.GetComponentsInChildren<StompableEnemy>(true).Length == 0 || enemies.GetComponentsInChildren<TurtlePatrol>(true).Length == 0)
            throw new System.InvalidOperationException("The level has no stompable enemy.");
        GameObject fallReset = GameObject.Find("Fall Reset");
        BoxCollider fallTrigger = fallReset != null ? fallReset.GetComponent<BoxCollider>() : null;
        if (fallTrigger == null || !fallTrigger.isTrigger || fallReset.GetComponent<KillZone>() == null)
            throw new System.InvalidOperationException("The fall reset needs a trigger collider and KillZone.");
        if (Object.FindFirstObjectByType<KillZone>() == null || Object.FindFirstObjectByType<LevelExit>() == null)
            throw new System.InvalidOperationException("The fall reset or level exit is missing.");

        SerializedObject data = new SerializedObject(manager);
        if (data.FindProperty("player").objectReferenceValue == null ||
            data.FindProperty("coinsParent").objectReferenceValue == null ||
            data.FindProperty("enemiesParent").objectReferenceValue == null)
            throw new System.InvalidOperationException("Level Manager references are incomplete.");
    }

    private static void UpdateBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();
        scenes.RemoveAll(scene => scene.path == ScenePath);
        scenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static Material[] CreateMaterials()
    {
        if (AssetDatabase.LoadMainAssetAtPath(MaterialsPath) != null) AssetDatabase.DeleteAsset(MaterialsPath);
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        Color[] colors =
        {
            new Color(0.32f, 0.66f, 0.35f),
            new Color(0.42f, 0.25f, 0.18f),
            new Color(0.16f, 0.48f, 0.9f),
            new Color(0.94f, 0.3f, 0.2f),
            new Color(1f, 0.72f, 0.12f),
            new Color(0.28f, 0.72f, 0.32f),
            new Color(0.9f, 0.19f, 0.2f),
            new Color(0.17f, 0.78f, 0.84f),
            new Color(0.9f, 0.96f, 1f)
        };
        string[] names = { "Platform Grass", "Platform Earth", "Player Blue", "Accent Red", "Coin Gold", "Enemy Green", "Hazard Red", "Exit Cyan", "Cloud White" };
        Material[] materials = new Material[colors.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = new Material(shader) { name = names[i], color = colors[i] };
            if (i == 0) AssetDatabase.CreateAsset(materials[i], MaterialsPath);
            else AssetDatabase.AddObjectToAsset(materials[i], MaterialsPath);
        }
        AssetDatabase.SaveAssets();
        return materials;
    }

    private static Transform CreateGroup(string name, Transform parent)
    {
        Transform group = new GameObject(name).transform;
        group.SetParent(parent, false);
        return group;
    }

    private static void CreatePlatform(string name, Vector3 center, Vector3 size, Material grass, Material earth, Transform parent, int groundLayer, GameObject wallPrefab)
    {
        CreateCube(name + " Earth", center, size, earth, parent, true, groundLayer);
        Vector3 capSize = new Vector3(size.x, 0.16f, size.z);
        Vector3 capCenter = new Vector3(center.x, center.y + size.y * 0.5f + 0.08f, center.z);
        CreateCube(name + " Grass", capCenter, capSize, grass, parent, true, groundLayer);
        float wallBase = capCenter.y + capSize.y * 0.5f;
        float wallStart = center.z - size.z * 0.5f;
        CreateWallRail(wallPrefab, center.x - size.x * 0.5f - 0.15f, wallBase, wallStart, size.z, parent, groundLayer);
        CreateWallRail(wallPrefab, center.x + size.x * 0.5f + 0.15f, wallBase, wallStart, size.z, parent, groundLayer);
    }

    private static void CreateWallRail(GameObject wallPrefab, float x, float y, float startZ, float length, Transform parent, int groundLayer)
    {
        for (float offset = 0f; offset < length; offset += 2f)
        {
            float segmentLength = Mathf.Min(2f, length - offset);
            GameObject wall = PrefabUtility.InstantiatePrefab(wallPrefab) as GameObject;
            wall.name = "Wall Rail";
            wall.transform.SetParent(parent, false);
            wall.transform.position = new Vector3(x, y, startZ + offset);
            wall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            wall.transform.localScale = new Vector3(segmentLength * 0.5f, 0.3f, 1f);
            SetLayerRecursively(wall.transform, groundLayer);
        }
    }

    private static void SetLayerRecursively(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root) SetLayerRecursively(child, layer);
    }

    private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material, Transform parent = null, bool collider = true, int layer = -1, bool trigger = false)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.position = position;
        cube.transform.localScale = scale;
        if (parent != null) cube.transform.SetParent(parent, true);
        if (material != null) cube.GetComponent<MeshRenderer>().sharedMaterial = material;
        if (layer >= 0) cube.layer = layer;
        if (!collider) Object.DestroyImmediate(cube.GetComponent<BoxCollider>());
        else if (trigger) cube.GetComponent<BoxCollider>().isTrigger = true;
        return cube;
    }

    private static GameObject CreateSphere(string name, Transform parent, Vector3 position, Vector3 scale, Material material, bool trigger)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = name;
        sphere.transform.SetParent(parent, false);
        sphere.transform.localPosition = position;
        sphere.transform.localScale = scale;
        sphere.GetComponent<MeshRenderer>().sharedMaterial = material;
        if (trigger) sphere.GetComponent<SphereCollider>().isTrigger = true;
        else Object.DestroyImmediate(sphere.GetComponent<SphereCollider>());
        return sphere;
    }

    private static void CreateCoin(Vector3 position, Transform parent, Material material)
    {
        GameObject coin = CreateSphere("Coin", parent, position, new Vector3(0.65f, 0.85f, 0.25f), material, true);
        coin.AddComponent<CoinPickup>();
        coin.AddComponent<HoverBob>();
    }

    private static void CreateSpikeTrap(Transform parent)
    {
        GameObject trap = new GameObject("Spikes_01");
        trap.transform.SetParent(parent, false);
        trap.transform.position = new Vector3(0f, 0.42f, 36f);
        BoxCollider trigger = trap.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(7f, 0.42f, 2.2f);
        trap.AddComponent<KillZone>();
        CreateIlterisDecor("Assets/ProjectPrefab/Ilteris/Hazard_SpikeTrap.fbx", "Ilteris Spike Trap", new Vector3(0f, 0.5f, 36f), new Vector3(7f, 0.8f, 2.2f), trap.transform);
    }

    private static void CreateExit(Transform parent, Material material)
    {
        CreateCube("Exit Left", new Vector3(-1.45f, 2.4f, 69f), new Vector3(0.35f, 3.2f, 0.45f), material, parent, false);
        CreateCube("Exit Right", new Vector3(1.45f, 2.4f, 69f), new Vector3(0.35f, 3.2f, 0.45f), material, parent, false);
        CreateCube("Exit Top", new Vector3(0f, 4f, 69f), new Vector3(3.2f, 0.4f, 0.45f), material, parent, false);
        GameObject exit = new GameObject("Level Exit");
        exit.transform.SetParent(parent, false);
        exit.transform.position = new Vector3(0f, 2f, 69f);
        BoxCollider trigger = exit.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(2.5f, 3.6f, 1f);
        exit.AddComponent<LevelExit>();
    }

    private static void CreateCloud(Vector3 position, Transform parent, Material material)
    {
        CreateSphere("Cloud", parent, position, new Vector3(3f, 1f, 1.4f), material, false);
        GameObject puffA = CreateSphere("Cloud Puff", parent, position + new Vector3(-1f, 0.35f, 0f), Vector3.one * 1.25f, material, false);
        GameObject puffB = CreateSphere("Cloud Puff", parent, position + new Vector3(0.7f, 0.35f, 0f), Vector3.one * 1.4f, material, false);
    }

    private static GameObject CreateIlterisDecor(string assetPath, string objectName, Vector3 position, Vector3 targetSize, Transform parent)
    {
        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (source == null) throw new System.InvalidOperationException("Could not load Ilteris model: " + assetPath);
        GameObject model = PrefabUtility.InstantiatePrefab(source) as GameObject;
        if (model == null) throw new System.InvalidOperationException("Could not instantiate Ilteris model: " + assetPath);

        model.name = objectName;
        model.transform.SetParent(parent, false);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;

        Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
            if (bounds.size.x > 0.001f && bounds.size.y > 0.001f && bounds.size.z > 0.001f)
            {
                model.transform.localScale = new Vector3(
                    targetSize.x / bounds.size.x,
                    targetSize.y / bounds.size.y,
                    targetSize.z / bounds.size.z);
            }
        }

        model.transform.localPosition = position;
        foreach (Collider collider in model.GetComponentsInChildren<Collider>(true)) collider.enabled = false;
        return model;
    }
}