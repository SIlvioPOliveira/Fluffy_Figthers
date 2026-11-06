using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public sealed class BossControllerTest : MonoBehaviour, IDamageGate
{
    [SerializeField] private PlayerHealthTest player;
    [SerializeField] private CombatTargetTest targetManager;
    [SerializeField] private BossArenaTest arena;
    [SerializeField] private Material warningMaterial;
    [SerializeField, Min(0.1f)] private float warningSeconds = 1.5f;
    [SerializeField, Min(0.1f)] private float cooldownSeconds = 3f;
    [SerializeField, Min(0.1f)] private float attackRadius = 2.5f;
    private EnemyHealth health;
    private GameObject warning;
    private Vector3 impactPosition;
    private float nextAction;
    private bool warningActive;
    public bool InCombat { get; private set; }
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
        ClearWarning();
    }

    private void OnDamaged(int amount, PetRarity? rarity)
    {
        if (!health.IsAlive) { InCombat = false; ClearWarning(); return; }
        if (InCombat || player == null || arena == null || !arena.ContainsPlayer()) return;
        InCombat = true;
        nextAction = Time.time;
    }

    private void Update()
    {
        if (!InCombat) return;
        if (!health.IsAlive || player == null || arena == null) { InCombat = false; ClearWarning(); return; }
        // Position check complements trigger events, including teleports and fast movement.
        if (!arena.ContainsPlayer()) { ResetEncounter(); return; }
        if (Time.time < nextAction) return;
        if (!warningActive) BeginWarning();
        else ResolveAttack();
    }

    private void BeginWarning()
    {
        impactPosition = player.transform.position;
        impactPosition.y = 0.03f;
        warning = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        warning.name = "Área de ataque do Boss";
        warning.transform.position = impactPosition;
        warning.transform.localScale = new Vector3(attackRadius * 2f, 0.01f, attackRadius * 2f);
        warning.GetComponent<Collider>().enabled = false;
        if (warningMaterial != null) warning.GetComponent<Renderer>().sharedMaterial = warningMaterial;
        warningActive = true;
        nextAction = Time.time + warningSeconds;
    }

    private void ResolveAttack()
    {
        Vector3 delta = Vector3.ProjectOnPlane(player.transform.position - impactPosition, Vector3.up);
        ClearWarning();
        // Set cooldown before damage: death can reset the encounter synchronously.
        nextAction = Time.time + cooldownSeconds;
        if (delta.sqrMagnitude <= attackRadius * attackRadius) player.TakeDamage(1);
    }

    public void ResetEncounter()
    {
        if (!InCombat) return;
        InCombat = false;
        ClearWarning();
        if (health.IsAlive) health.ResetHealth();
        if (targetManager != null && targetManager.Target == health) targetManager.ClearTarget();
    }

    private void ClearWarning()
    {
        if (warning != null) Destroy(warning);
        warning = null;
        warningActive = false;
    }

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
        GUI.Label(new Rect(rect.x + 10, rect.y + 6, rect.width - 20, 24), "Guardião das Marés   " + health.Health + " / " + health.MaxHealth);
        GUI.color = old;
    }
}
