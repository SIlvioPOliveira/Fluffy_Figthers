using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// One-time authoring tool for the provisional continuous world.
public static class WorldPrototypeBuilder
{
    private static void SetObject(Object target, string name, Object value)
    {
        var so = new SerializedObject(target);
        so.FindProperty(name).objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetNumber(Object target, string name, int value)
    {
        var so = new SerializedObject(target);
        so.FindProperty(name).intValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetFloat(Object target, string name, float value)
    {
        var so = new SerializedObject(target);
        so.FindProperty(name).floatValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Material MaterialCopy(string name, string source, Color color)
    {
        string path = "Assets/Art/TestMaterials/" + name + ".mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(path) != null) throw new System.Exception("Material já existe: " + path);
        var material = new Material(AssetDatabase.LoadAssetAtPath<Material>(source));
        material.name = name;
        material.color = color;
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static GameObject Copy(GameObject original, Transform parent, float offset, string name)
    {
        var clone = Object.Instantiate(original, parent);
        clone.name = name;
        clone.transform.position = original.transform.position + Vector3.forward * offset;
        return clone;
    }

    public static string Build()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != "Assets/Scenes/RegiaoAguaPrototipo.unity" || scene.isDirty) throw new System.Exception("Abra a cena Água sem alterações pendentes.");
        if (GameObject.Find("07_MundoExpandido") != null) throw new System.Exception("Mundo expandido já existe.");
        var waterGacha = GameObject.Find("02_Gacha");
        var waterEnemies = GameObject.Find("03_Inimigos");
        var waterArena = GameObject.Find("04_ArenaBoss");
        var waterExit = GameObject.Find("05_SaidaParaFogo");
        var waterScenery = GameObject.Find("06_Cenario");
        var driver = Object.FindFirstObjectByType<PetEquipmentTestDriver>();
        var player = Object.FindFirstObjectByType<PlayerHealthTest>();
        if (waterGacha == null || waterEnemies == null || waterArena == null || waterExit == null || waterScenery == null || driver == null || player == null) throw new System.Exception("Estrutura da Água incompleta.");

        string[] names = { "Fogo", "Lava", "Gelo" };
        string[] poolNames = { "Fire", "Lava", "Ice" };
        Color[] colors = { new Color(.95f,.55f,.3f), new Color(.52f,.34f,.3f), new Color(.72f,.87f,1f) };
        int[] weakHp = { 250, 900, 2600 }, weakReward = { 80, 250, 750 };
        int[] strongHp = { 550, 1800, 5500 }, strongReward = { 150, 450, 1300 };
        int[] bossHp = { 1000, 3600, 21000 }, bossReward = { 600, 1600, 4500 };
        string[] bossNames = { "Guardião das Chamas", "Colosso Vulcânico", "Guardião Glacial" };
        var world = new GameObject("07_MundoExpandido");
        var entranceRoot = new GameObject("Pontos de entrada");
        entranceRoot.transform.SetParent(world.transform);
        var entries = new Transform[4];
        entries[0] = GameObject.Find("Entrada da região").transform;
        for (int i = 0; i < 3; i++)
        {
            float offset = (i + 1) * 70f;
            var root = new GameObject("Região " + names[i]);
            root.transform.SetParent(world.transform);
            var entry = new GameObject("Entrada " + names[i]);
            entry.transform.SetParent(entranceRoot.transform);
            entry.transform.position = new Vector3(0, 1.05f, offset - 24f);
            entries[i + 1] = entry.transform;
            var gacha = Copy(waterGacha, root.transform, offset, "Gacha " + names[i]);
            var machine = gacha.GetComponentInChildren<GachaMachineTest>();
            machine.gameObject.name = "Máquina " + names[i];
            SetObject(machine, "pool", AssetDatabase.LoadAssetAtPath<GachaPool>("Assets/Data/Gacha/" + poolNames[i] + "Gacha.asset"));
            var enemies = Copy(waterEnemies, root.transform, offset, "Inimigos " + names[i]);
            var healths = enemies.GetComponentsInChildren<EnemyHealth>(true);
            foreach (var health in healths)
            {
                bool strong = health.name.StartsWith("Forte");
                SetNumber(health, "maxHealth", strong ? strongHp[i] : weakHp[i]);
                SetNumber(health, "reward", strong ? strongReward[i] : weakReward[i]);
            }
            var arena = Copy(waterArena, root.transform, offset, "Arena " + names[i]);
            var boss = arena.GetComponentInChildren<BossControllerTest>();
            boss.gameObject.name = bossNames[i];
            SetNumber(boss, "pattern", i + 1);
            SetFloat(boss, "cooldownSeconds", i == 2 ? 1f : 2f);
            if (i == 2) { SetFloat(boss, "warningSeconds", 1f); SetNumber(boss, "doubleEvery", 1); SetFloat(boss, "ultimateCooldown", 5f); }
            var bossHealth = boss.GetComponent<EnemyHealth>();
            SetNumber(bossHealth, "maxHealth", bossHp[i]);
            SetNumber(bossHealth, "reward", bossReward[i]);
            var scenery = Copy(waterScenery, root.transform, offset, "Cenário " + names[i]);
            var backWall = scenery.transform.Find("Limite 3");
            if (backWall != null) Object.DestroyImmediate(backWall.gameObject);
            var groundMat = MaterialCopy("Region" + poolNames[i] + "Ground", "Assets/Art/TestMaterials/TestGround.mat", colors[i]);
            var accentMat = MaterialCopy("Region" + poolNames[i] + "Accent", "Assets/Art/TestMaterials/RegionWaterWall.mat", colors[i] * .7f);
            foreach (var renderer in scenery.GetComponentsInChildren<Renderer>(true))
                renderer.sharedMaterial = renderer.name == "Ground" ? groundMat : accentMat;
            if (i < 2)
            {
                var exit = Copy(waterExit, root.transform, offset, "Saída " + names[i]);
                var gate = exit.GetComponentInChildren<ZoneBarrierTest>();
                gate.gameObject.name = "Barreira para " + names[i + 1];
                SetObject(gate, "region", AssetDatabase.LoadAssetAtPath<RegionData>("Assets/Data/Regions/" + poolNames[i] + ".asset"));
            }
            else
            {
                var endWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                endWall.name = "Limite final Gelo";
                endWall.transform.SetParent(root.transform);
                endWall.transform.position = new Vector3(0, 1.25f, offset + 30f);
                endWall.transform.localScale = new Vector3(8, 2.5f, 1);
                endWall.GetComponent<Renderer>().sharedMaterial = accentMat;
            }
        }
        // Join the side walls across the gaps so a player cannot walk around a paid gate.
        for (int i = 0; i < 3; i++)
        {
            for (int side = -1; side <= 1; side += 2)
            {
                var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = "Limite de ligação " + (i + 1) + (side < 0 ? " esquerdo" : " direito");
                wall.transform.SetParent(world.transform);
                wall.transform.position = new Vector3(side * 27.5f, 1.25f, 35f + i * 70f);
                wall.transform.localScale = new Vector3(1f, 2.5f, 11f);
                wall.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/TestMaterials/RegionWaterWall.mat");
            }
        }
        var regionManager = world.AddComponent<WorldRegionProgress>();
        SetObject(regionManager, "driver", driver);
        SetObject(regionManager, "health", player);
        var serialized = new SerializedObject(regionManager);
        var array = serialized.FindProperty("regionEntrances");
        array.arraySize = entries.Length;
        for (int i = 0; i < entries.Length; i++) array.GetArrayElementAtIndex(i).objectReferenceValue = entries[i];
        serialized.ApplyModifiedPropertiesWithoutUndo();
        driver.gameObject.AddComponent<FinalVictoryNotice>();
        EditorSceneManager.MarkSceneDirty(scene);
        AssetDatabase.SaveAssets();
        if (!EditorSceneManager.SaveScene(scene)) throw new System.Exception("Não foi possível salvar a cena.");
        return "Três regiões provisórias adicionadas à cena da Água.";
    }
}
