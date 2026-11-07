using System.Collections.Generic;
using UnityEngine;

public enum BossPattern { Water, Fire, Lava, Ice }

[RequireComponent(typeof(EnemyHealth))]
public sealed class BossControllerTest : MonoBehaviour, IDamageGate
{
    [SerializeField] private PlayerHealthTest player;
    [SerializeField] private CombatTargetTest targetManager;
    [SerializeField] private BossArenaTest arena;
    [SerializeField] private Material warningMaterial;
    [SerializeField] private BossPattern pattern;
    [SerializeField, Min(0.1f)] private float warningSeconds = 1.5f;
    [SerializeField, Min(0.1f)] private float cooldownSeconds = 3f;
    [SerializeField, Min(0.1f)] private float attackRadius = 2.5f;
    [SerializeField, Min(1)] private int doubleEvery = 3;
    [SerializeField, Min(0f)] private float predictionSeconds = 1.2f;
    [SerializeField, Min(1)] private int ultimateAreas = 4;
    [SerializeField, Min(0.1f)] private float ultimateSpawnInterval = 0.55f;
    [SerializeField, Min(1f)] private float ultimateCooldown = 20f;

    private sealed class Warning
    {
        public GameObject Visual;
        public Vector3 Position;
        public float ImpactAt;
    }

    private readonly List<Warning> warnings = new List<Warning>();
    private EnemyHealth health;
    private float nextAction;
    private float nextUltimate;
    private float nextUltimateArea;
    private int attacks;
    private int areasSpawned;
    private bool ultimateActive;
    public bool InCombat { get; private set; }
    public bool IsWaterBoss => pattern == BossPattern.Water;
    public bool CanReceiveDamage => player != null && arena != null && arena.ContainsPlayer();

    private void Awake() => health = GetComponent<EnemyHealth>();
    private void OnEnable()
    {
        health.Damaged += OnDamaged;
        if (player != null) player.Died += ResetEncounter;
    }
    private void OnDisable()
    {
        health.Damaged -= OnDamaged;
        if (player != null) player.Died -= ResetEncounter;
        ClearWarnings();
    }

    private void OnDamaged(int amount, PetRarity? rarity)
    {
        if (!health.IsAlive) { InCombat = false; ClearWarnings(); return; }
        if (InCombat || player == null || arena == null || !arena.ContainsPlayer()) return;
        InCombat = true;
        attacks = 0;
        ultimateActive = false;
        nextAction = Time.time;
        nextUltimate = Time.time + ultimateCooldown;
    }

    private void Update()
    {
        if (!InCombat) return;
        if (!health.IsAlive || player == null || arena == null) { InCombat = false; ClearWarnings(); return; }
        if (!arena.ContainsPlayer()) { ResetEncounter(); return; }

        // A simultaneous pair is one attack, so overlapping circles cannot remove two hearts at once.
        bool hit = false;
        for (int i = warnings.Count - 1; i >= 0; i--)
        {
            if (Time.time < warnings[i].ImpactAt) continue;
            Vector3 delta = Vector3.ProjectOnPlane(player.transform.position - warnings[i].Position, Vector3.up);
            hit |= delta.sqrMagnitude <= attackRadius * attackRadius;
            if (warnings[i].Visual != null) Destroy(warnings[i].Visual);
            warnings.RemoveAt(i);
        }
        if (hit) player.TakeDamage(1);
        if (!InCombat) return;

        if (ultimateActive)
        {
            if (areasSpawned < ultimateAreas && Time.time >= nextUltimateArea)
            {
                SpawnWarning(player.transform.position);
                areasSpawned++;
                nextUltimateArea = Time.time + ultimateSpawnInterval;
            }
            if (areasSpawned >= ultimateAreas && warnings.Count == 0)
            {
                ultimateActive = false;
                nextUltimate = Time.time + ultimateCooldown;
                nextAction = Time.time + cooldownSeconds;
            }
            return;
        }

        if (warnings.Count > 0 || Time.time < nextAction) return;
        if (pattern == BossPattern.Ice && Time.time >= nextUltimate)
        {
            ultimateActive = true;
            areasSpawned = 0;
            nextUltimateArea = Time.time;
            return;
        }

        attacks++;
        SpawnWarning(player.transform.position);
        if ((pattern == BossPattern.Lava || pattern == BossPattern.Ice) && attacks % Mathf.Max(1, doubleEvery) == 0)
        {
            Vector3 velocity = Vector3.ProjectOnPlane(player.GetComponent<CharacterController>().velocity, Vector3.up);
            SpawnWarning(arena.ClampInside(player.transform.position + velocity * predictionSeconds));
        }
        nextAction = Time.time + warningSeconds + cooldownSeconds;
    }

    private void SpawnWarning(Vector3 position)
    {
        position.y = 0.03f;
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        visual.name = "Área de ataque do Boss";
        visual.transform.position = position;
        visual.transform.localScale = new Vector3(attackRadius * 2f, 0.01f, attackRadius * 2f);
        visual.GetComponent<Collider>().enabled = false;
        if (warningMaterial != null) visual.GetComponent<Renderer>().sharedMaterial = warningMaterial;
        warnings.Add(new Warning { Visual = visual, Position = position, ImpactAt = Time.time + warningSeconds });
    }

    public void ResetEncounter()
    {
        if (!InCombat) return;
        InCombat = false;
        ultimateActive = false;
        ClearWarnings();
        if (health.IsAlive) health.ResetHealth();
        if (targetManager != null && targetManager.Target == health) targetManager.ClearTarget();
    }

    private void ClearWarnings()
    {
        foreach (Warning warning in warnings) if (warning.Visual != null) Destroy(warning.Visual);
        warnings.Clear();
    }

    private string BossName => pattern == BossPattern.Fire ? "Guardião das Chamas" : pattern == BossPattern.Lava ? "Colosso Vulcânico" : pattern == BossPattern.Ice ? "Guardião Glacial" : "Guardião das Marés";

    private void OnGUI()
    {
        if (!InCombat || !health.IsAlive) return;
        float width = Mathf.Min(420f, Screen.width - 40f);
        var rect = new Rect((Screen.width - width) / 2f, 15f, width, 34f);
        Color old = GUI.color;
        GUI.color = new Color(.08f, .08f, .1f, .95f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = new Color(.85f, .22f, .16f);
        GUI.DrawTexture(new Rect(rect.x + 2, rect.y + 2, (rect.width - 4) * Mathf.Clamp01((float)health.Health / health.MaxHealth), rect.height - 4), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(rect.x + 10, rect.y + 6, rect.width - 20, 24), BossName + "   " + health.Health + " / " + health.MaxHealth);
        GUI.color = old;
        if (ultimateActive) GUI.Box(new Rect((Screen.width - 220f) / 2f, 52f, 220f, 30f), "ATAQUE ESPECIAL!");
    }
}
