using UnityEngine;

public interface IDamageGate
{
    bool CanReceiveDamage { get; }
}

public sealed class EnemyHealth : MonoBehaviour
{
    [SerializeField] private PetEquipmentTestDriver driver;
    [SerializeField, Min(1)] private int maxHealth = 50;
    [SerializeField, Min(1)] private int reward = 20;
    [SerializeField, Min(0.1f)] private float respawnDelay = 5f;
    [SerializeField] private GameObject selectionMarker;
    private Renderer[] visuals;
    private Collider[] hitboxes;
    private float respawnAt;
    public int Health { get; private set; }
    public int MaxHealth => maxHealth;
    public bool IsAlive { get; private set; }
    public bool IsSelected { get; private set; }
    public event System.Action<int, PetRarity?> Damaged;
    public event System.Action<int> RewardGranted;

    private void Awake()
    {
        visuals = GetComponentsInChildren<Renderer>();
        hitboxes = GetComponentsInChildren<Collider>();
        Health = maxHealth;
        IsAlive = true;
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected && IsAlive;
        if (selectionMarker != null) selectionMarker.SetActive(IsSelected);
    }

    public void ResetHealth()
    {
        if (IsAlive) Health = maxHealth;
    }

    public bool TakeDamage(long damage, PetRarity? sourceRarity = null)
    {
        IDamageGate gate = GetComponent<IDamageGate>();
        if (!IsAlive || damage <= 0 || (gate != null && !gate.CanReceiveDamage)) return false;
        int previousHealth = Health;
        bool rewardGranted = false;
        Health = (int)System.Math.Max(0L, Health - damage);
        if (Health == 0)
        {
            IsAlive = false;
            SetSelected(false);
            foreach (Renderer visual in visuals) visual.enabled = false;
            foreach (Collider hitbox in hitboxes) hitbox.enabled = false;
            if (driver != null && driver.Wallet != null)
            {
                rewardGranted = driver.Wallet.TryAdd(reward);
                if (!rewardGranted) Debug.LogWarning("Não foi possível adicionar a recompensa.", this);
            }
            BossControllerTest boss = GetComponent<BossControllerTest>();
            if (driver != null && boss != null && boss.IsWaterBoss) driver.TutorialWaterBossDefeated();
            respawnAt = Time.time + respawnDelay;
        }
        // Feedback is optional and cannot cancel an already-applied hit or defeat.
        try { Damaged?.Invoke(previousHealth - Health, sourceRarity); }
        catch (System.Exception exception) { Debug.LogException(exception, this); }
        if (rewardGranted)
        {
            driver.TutorialEnemyRewardGranted();
            try { RewardGranted?.Invoke(reward); }
            catch (System.Exception exception) { Debug.LogException(exception, this); }
        }
        return true;
    }

    private void Update()
    {
        if (IsAlive || Time.time < respawnAt) return;
        Health = maxHealth;
        IsAlive = true;
        foreach (Renderer visual in visuals) visual.enabled = true;
        foreach (Collider hitbox in hitboxes) hitbox.enabled = true;
        SetSelected(false);
    }

}
