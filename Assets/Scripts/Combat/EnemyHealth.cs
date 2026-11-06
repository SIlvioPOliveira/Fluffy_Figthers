using UnityEngine;

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

    public bool TakeDamage(long damage, PetRarity? sourceRarity = null)
    {
        if (!IsAlive || damage <= 0) return false;
        int previousHealth = Health;
        Health = (int)System.Math.Max(0L, Health - damage);
        if (Health == 0)
        {
            IsAlive = false;
            SetSelected(false);
            foreach (Renderer visual in visuals) visual.enabled = false;
            foreach (Collider hitbox in hitboxes) hitbox.enabled = false;
            if (driver != null && driver.Wallet != null && !driver.Wallet.TryAdd(reward)) Debug.LogWarning("Não foi possível adicionar a recompensa.", this);
            respawnAt = Time.time + respawnDelay;
        }
        // Feedback is optional and cannot cancel an already-applied hit or defeat.
        try { Damaged?.Invoke(previousHealth - Health, sourceRarity); }
        catch (System.Exception exception) { Debug.LogException(exception, this); }
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

    private void OnGUI()
    {
        if (!IsAlive || !IsSelected || Camera.main == null) return;
        Vector3 point = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
        if (point.z <= 0) return;
        float x = point.x - 65, y = Screen.height - point.y - 30;
        GUI.Box(new Rect(x, y, 130, 25), "Inimigo  " + Health + " / " + maxHealth);
    }
}
