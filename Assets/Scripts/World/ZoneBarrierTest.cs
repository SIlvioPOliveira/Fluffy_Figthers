using UnityEngine;

// Temporary Water-to-Fire gate. Persistence and the Fire region are separate milestones.
public sealed class ZoneBarrierTest : MonoBehaviour
{
    [SerializeField] private PetEquipmentTestDriver driver;
    [SerializeField] private RegionData region;
    [SerializeField, Min(0.1f)] private float interactionDistance = 4f;
    private Collider barrierCollider;
    private Renderer barrierRenderer;
    private PlayerHealthTest playerHealth;
    private bool modalOpen;
    private bool unlocked;
    private string message = "";
    public bool IsUnlocked => unlocked;
    public int UnlockCost => region != null ? region.NextRegionCost : 0;

    private void Awake()
    {
        barrierCollider = GetComponent<Collider>();
        barrierRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        if (driver == null || region == null || driver.Player == null || barrierCollider == null || barrierRenderer == null)
        {
            Debug.LogError("Barreira de teste sem referências necessárias.", this);
            enabled = false;
            return;
        }
        playerHealth = driver.Player.GetComponent<PlayerHealthTest>();
        if (playerHealth != null) playerHealth.Died += CloseOnDeath;
        driver.RegisterBarrier(this);
    }

    public void RestoreUnlocked(bool value)
    {
        unlocked = value;
        if (barrierCollider != null) barrierCollider.enabled = !value;
        if (barrierRenderer != null) barrierRenderer.enabled = !value;
    }

    private void OnDestroy()
    {
        if (playerHealth != null) playerHealth.Died -= CloseOnDeath;
        if (modalOpen && driver != null) SetOpen(false);
    }

    private bool InRange => driver != null && driver.Player != null &&
        Vector3.Distance(driver.Player.position, transform.position) <= interactionDistance;

    private void Update()
    {
        if (driver.PauseOpen) return;
        if (modalOpen && Input.GetKeyDown(KeyCode.Escape)) SetOpen(false);
        else if (!modalOpen && !unlocked && !driver.InventoryOpen && !driver.GachaOpen && InRange && Input.GetKeyDown(KeyCode.E)) SetOpen(true);
    }

    private void CloseOnDeath() { if (modalOpen) SetOpen(false); }

    private void SetOpen(bool value)
    {
        modalOpen = value;
        driver.BarrierOpen = value;
        PlayerMovement movement = driver.Player.GetComponent<PlayerMovement>();
        if (movement != null) movement.ControlsEnabled = !value;
        if (driver.CameraController != null) driver.CameraController.ControlsEnabled = !value;
        if (!value) message = "";
    }

    private bool TryUnlock()
    {
        if (!modalOpen || unlocked || region.NextRegionCost <= 0) return false;
        if (!driver.Wallet.TrySpendSilently(region.NextRegionCost))
        {
            message = "Moedas insuficientes.";
            return false;
        }
        unlocked = true;
        barrierCollider.enabled = false;
        barrierRenderer.enabled = false;
        SetOpen(false);
        // Notify observers only after the payment and gate state are both updated.
        try { driver.Wallet.NotifyChanged(); }
        catch (System.Exception exception) { Debug.LogException(exception, this); }
        return true;
    }

    private void OnGUI()
    {
        if (driver == null || region == null) return;
        if (!modalOpen)
        {
            if (InRange && !driver.InventoryOpen && !driver.GachaOpen)
                GUI.Label(new Rect(15, 75, 550, 25), unlocked ? "Passagem para Fogo aberta (região em construção)." : "[E] Desbloquear caminho para Fogo");
            return;
        }
        float width = Mathf.Min(440f, Screen.width - 30f);
        GUILayout.BeginArea(new Rect((Screen.width - width) / 2f, (Screen.height - 210f) / 2f, width, 210f), GUI.skin.box);
        GUILayout.Label("DESBLOQUEAR REGIÃO FOGO");
        GUILayout.Label("Custo: " + region.NextRegionCost + " moedas | Saldo: " + driver.Wallet.Balance);
        GUILayout.Label("A próxima região ainda está em construção.");
        if (GUILayout.Button("Confirmar", GUILayout.Height(35))) TryUnlock();
        if (GUILayout.Button("Cancelar / Esc", GUILayout.Height(35))) SetOpen(false);
        GUILayout.Label(message);
        GUILayout.EndArea();
    }
}
