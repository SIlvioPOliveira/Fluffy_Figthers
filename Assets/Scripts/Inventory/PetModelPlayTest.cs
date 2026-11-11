#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

// Isolated Editor play test: does not alter the player's wallet or inventory.
public sealed class PetModelPlayTest : MonoBehaviour
{
    private const BindingFlags PrivateInstance = BindingFlags.NonPublic | BindingFlags.Instance;
    private readonly List<float> samples = new List<float>();
    private readonly List<Renderer> hidden = new List<Renderer>();
    private PetEquipmentTestDriver driver;
    private CombatTargetTest combat;
    private EnemyHealth oldTarget;
    private EnemyHealth enemy;
    private PetEquipmentView actualView;
    private bool actualViewEnabled;
    private GameObject dummyPlayer;
    private GameObject testView;
    private GameObject testEnemy;
    private float start;
    private bool previousBackground;
    public bool Done { get; private set; }
    public string Result { get; private set; }

    public static string Begin()
    {
        if (!EditorApplication.isPlaying) return "O teste exige Play Mode.";
        if (FindFirstObjectByType<PetModelPlayTest>() != null) return "Teste já existe.";
        var owner = new GameObject("PetModelPerformanceTest");
        var test = owner.AddComponent<PetModelPlayTest>();
        try { test.Setup(); return "Teste iniciado."; }
        catch (Exception e) { Destroy(owner); return "Falha: " + e; }
    }

    private void Setup()
    {
        driver = FindFirstObjectByType<PetEquipmentTestDriver>();
        if (driver == null || driver.Player == null) throw new Exception("Cena sem jogador.");
        combat = driver.GetComponent<CombatTargetTest>();
        if (combat == null) throw new Exception("Cena sem combate.");
        previousBackground = Application.runInBackground;
        Application.runInBackground = true;
        oldTarget = combat.Target;
        actualView = driver.GetComponent<PetEquipmentView>();
        actualViewEnabled = actualView.enabled;
        actualView.enabled = false;
        var originalViews = (GameObject[])typeof(PetEquipmentView).GetField("views", PrivateInstance).GetValue(actualView);
        foreach (var view in originalViews)
            if (view != null) foreach (var renderer in view.GetComponentsInChildren<Renderer>())
                if (renderer.enabled) { renderer.enabled = false; hidden.Add(renderer); }

        dummyPlayer = new GameObject("PetBenchmarkPlayer");
        dummyPlayer.transform.position = driver.Player.position;
        dummyPlayer.transform.rotation = driver.Player.rotation;
        testEnemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        testEnemy.name = "PetBenchmarkTarget";
        testEnemy.SetActive(false);
        testEnemy.transform.position = driver.Player.position + driver.Player.forward * 4f;
        enemy = testEnemy.AddComponent<EnemyHealth>();
        typeof(EnemyHealth).GetField("maxHealth", PrivateInstance).SetValue(enemy, 10000000);
        testEnemy.SetActive(true);
        if (!combat.TrySelect(enemy)) throw new Exception("Não foi possível selecionar o alvo de teste.");

        testView = new GameObject("PetBenchmarkTeam");
        var viewTest = testView.AddComponent<PetEquipmentView>();
        typeof(PetEquipmentView).GetField("combat", PrivateInstance).SetValue(viewTest, combat);
        var inventory = new PlayerInventory(driver.Catalog);
        string[] ids = { "water_dragao_marinho", "lava_dragao", "ice_dragao" };
        for (int i = 0; i < ids.Length; i++)
            if (!inventory.TryAdd(ids[i]) || !inventory.TryEquip(ids[i], 1, i)) throw new Exception("Não foi possível equipar " + ids[i]);
        viewTest.Initialize(inventory, driver.Catalog, dummyPlayer.transform);
        start = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        if (driver == null || dummyPlayer == null || Done) return;
        float elapsed = Time.realtimeSinceStartup - start;
        dummyPlayer.transform.position = driver.Player.position + new Vector3(Mathf.Sin(elapsed) * 1.5f, 0, 0);
        if (elapsed >= 2f && elapsed <= 12f && Time.unscaledDeltaTime > 0) samples.Add(Time.unscaledDeltaTime * 1000f);
        if (elapsed < 12f) return;
        samples.Sort();
        float mean = samples.Count > 0 ? samples.Average() : 0;
        float p95 = samples.Count > 0 ? samples[Mathf.Min(samples.Count - 1, Mathf.FloorToInt(samples.Count * .95f))] : 0;
        Result = "samples=" + samples.Count + "; mean_ms=" + mean.ToString("F2") + "; p95_ms=" + p95.ToString("F2") + "; fps_mean=" + (mean > 0 ? (1000f / mean).ToString("F1") : "n/a") + "; target_damage=" + (10000000 - enemy.Health) + "; total_allocated_MiB=" + (Profiler.GetTotalAllocatedMemoryLong() / 1048576d).ToString("F1");
        Done = true;
        Cleanup();
    }

    private void Cleanup()
    {
        if (combat != null)
        {
            combat.ClearTarget();
            if (oldTarget != null && oldTarget.IsAlive) combat.TrySelect(oldTarget);
        }
        foreach (var renderer in hidden) if (renderer != null) renderer.enabled = true;
        hidden.Clear();
        if (actualView != null) actualView.enabled = actualViewEnabled;
        if (testView != null) Destroy(testView);
        if (testEnemy != null) Destroy(testEnemy);
        if (dummyPlayer != null) Destroy(dummyPlayer);
        testView = null; testEnemy = null; dummyPlayer = null;
        Application.runInBackground = previousBackground;
    }

    private void OnDestroy() { if (!Done) Cleanup(); }
}
#endif
