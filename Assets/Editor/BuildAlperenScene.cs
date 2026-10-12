using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BuildAlperenScene
{
    [MenuItem("Tools/Build Alperen Scene")]
    public static void Build()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientLight = new Color(0.72f, 0.76f, 0.8f);

        Material grass = MakeMaterial("Grass", new Color(0.24f, 0.43f, 0.31f));
        Material path = MakeMaterial("Path", new Color(0.68f, 0.69f, 0.62f));
        Material playerMaterial = MakeMaterial("Player", new Color(0.18f, 0.43f, 0.77f));
        Material cargoMaterial = MakeMaterial("Cargo", new Color(0.92f, 0.61f, 0.2f));
        Material goalMaterial = MakeMaterial("Goal", new Color(0.19f, 0.78f, 0.48f));
        Material hazardMaterial = MakeMaterial("Hazard", new Color(0.83f, 0.23f, 0.2f));

        CreateCube("Ground", new Vector3(0f, -0.3f, 0f), new Vector3(24f, 0.6f, 16f), grass);
        CreateCube("Delivery route", new Vector3(0f, 0.015f, 0f), new Vector3(20f, 0.03f, 3.5f), path);
        CreateCube("Start pad", new Vector3(-8f, 0.06f, 0f), new Vector3(2.4f, 0.08f, 4.5f), playerMaterial);
        CreateCube("Goal pad", new Vector3(8f, 0.06f, 0f), new Vector3(2.4f, 0.08f, 5f), goalMaterial);

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Courier";
        player.transform.position = new Vector3(-8f, 1f, 0f);
        player.GetComponent<MeshRenderer>().sharedMaterial = playerMaterial;
        Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
        CharacterController characterController = player.AddComponent<CharacterController>();
        characterController.height = 2f;
        characterController.radius = 0.45f;
        player.AddComponent<PlayerCarry>();
        player.AddComponent<SimpleCourierController>();

        GameObject carryPoint = new GameObject("Carry point");
        carryPoint.transform.SetParent(player.transform, false);
        carryPoint.transform.localPosition = new Vector3(0f, 0.45f, 0.7f);
        SerializedObject carryData = new SerializedObject(player.GetComponent<PlayerCarry>());
        carryData.FindProperty("carryPoint").objectReferenceValue = carryPoint.transform;
        carryData.ApplyModifiedPropertiesWithoutUndo();

        GameObject cargo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cargo.name = "Cargo";
        cargo.transform.position = new Vector3(-5.5f, 0.5f, 0f);
        cargo.transform.localScale = Vector3.one * 0.7f;
        cargo.GetComponent<MeshRenderer>().sharedMaterial = cargoMaterial;
        cargo.AddComponent<Rigidbody>();
        SphereCollider pickupRange = cargo.AddComponent<SphereCollider>();
        pickupRange.isTrigger = true;
        pickupRange.radius = 2.4f;
        cargo.AddComponent<Cargo>();

        GameObject goal = new GameObject("Delivery zone");
        goal.transform.position = new Vector3(8f, 1f, 0f);
        BoxCollider goalTrigger = goal.AddComponent<BoxCollider>();
        goalTrigger.isTrigger = true;
        goalTrigger.size = new Vector3(2.4f, 2.2f, 5f);
        goal.AddComponent<DeliveryZone>();

        GameObject hazard = new GameObject("Hazard trigger");
        hazard.transform.position = new Vector3(0.5f, 1f, 0f);
        BoxCollider hazardTrigger = hazard.AddComponent<BoxCollider>();
        hazardTrigger.isTrigger = true;
        hazardTrigger.size = new Vector3(1.6f, 2f, 4.2f);
        hazard.AddComponent<Hazard>();
        CreateCube("Hazard marker", new Vector3(0.5f, 0.08f, 0f), new Vector3(1.5f, 0.1f, 4.1f), hazardMaterial);

        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 18f, -20f);
        cameraObject.transform.rotation = Quaternion.Euler(42f, 0f, 0f);
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 11f;
        camera.clearFlags = CameraClearFlags.Skybox;

        GameObject lightObject = new GameObject("Directional Light", typeof(Light));
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        Light light = lightObject.GetComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.3f;

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/alperen scene.unity");
        List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList();
        if (buildScenes.All(item => item.path != "Assets/Scenes/alperen scene.unity"))
        {
            buildScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/alperen scene.unity", true));
            EditorBuildSettings.scenes = buildScenes.ToArray();
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Created Assets/Scenes/alperen scene.unity");
    }

    private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.position = position;
        cube.transform.localScale = scale;
        cube.GetComponent<MeshRenderer>().sharedMaterial = material;
        if (name == "Hazard marker") Object.DestroyImmediate(cube.GetComponent<BoxCollider>());
        return cube;
    }

    private static Material MakeMaterial(string name, Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        Material material = new Material(shader) { name = name, color = color };
        return material;
    }
}