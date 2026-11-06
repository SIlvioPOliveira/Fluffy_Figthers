using UnityEngine;

// Test-scene interaction and temporary UI, not the final machine or presentation.
public sealed class GachaMachineTest : MonoBehaviour
{
    [SerializeField] private PetEquipmentTestDriver driver;
    [SerializeField] private GachaPool pool;
    [SerializeField, Min(0.1f)] private float interactionDistance = 3f;
    private GachaSystem system;
    private bool open;
    private PetData result;
    private string message = "";

    private void Start()
    {
        if (driver == null || pool == null || driver.Inventory == null || driver.Wallet == null)
        {
            Debug.LogError("Gacha test scene is missing initialized references.", this);
            enabled = false;
            return;
        }
        system = new GachaSystem(driver.Catalog, driver.Wallet, driver.Inventory);
    }

    private bool InRange => driver != null && driver.Player != null &&
        Vector3.Distance(driver.Player.position, transform.position) <= interactionDistance;

    private void Update()
    {
        if (open && Input.GetKeyDown(KeyCode.Escape)) SetOpen(false);
        else if (!open && !driver.InventoryOpen && InRange && Input.GetKeyDown(KeyCode.E)) SetOpen(true);
    }

    private void OnDisable() { if (driver != null && open) SetOpen(false); }

    private void SetOpen(bool value)
    {
        open = value;
        driver.GachaOpen = value;
        var movement = driver.Player.GetComponent<PlayerMovement>();
        if (movement != null) movement.ControlsEnabled = !value;
        if (driver.CameraController != null) driver.CameraController.ControlsEnabled = !value;
        if (!value) { result = null; message = ""; }
    }

    private void OnGUI()
    {
        if (driver == null || driver.Inventory == null || pool == null || system == null) return;
        if (!open)
        {
            GUI.Label(new Rect(15, 45, 600, 25), "Moedas: " + driver.Wallet.Balance + (InRange && !driver.InventoryOpen ? "     [E] Abrir Gacha de Água" : ""));
            return;
        }
        float width = Mathf.Min(580, Screen.width - 30);
        float height = Mathf.Min(480, Screen.height - 30);
        GUILayout.BeginArea(new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height), GUI.skin.box);
        GUILayout.BeginHorizontal();
        GUILayout.Label("GACHA — " + pool.Region.DisplayName);
        if (GUILayout.Button("Fechar (Esc)", GUILayout.Width(130))) SetOpen(false);
        GUILayout.EndHorizontal();
        if (!open) { GUILayout.EndArea(); return; }
        GUILayout.Label("Saldo: " + driver.Wallet.Balance + " moedas | Custo: " + pool.Region.RollCost);
        foreach (GachaEntry entry in pool.Entries)
            GUILayout.Label(entry.Pet.PetName + " — " + entry.Pet.Rarity + " — " + (entry.Weight / 100f).ToString("0.##") + "%");
        if (result == null)
        {
            if (GUILayout.Button("Roll ×1", GUILayout.Height(45)))
            {
                var status = system.TryRoll(pool, Random.Range(0, GachaPool.TotalWeight), out result);
                message = status == GachaResult.Success ? "Pet recebido! Abra o inventário com I após fechar." :
                    status == GachaResult.InsufficientFunds ? "Moedas insuficientes." : "Não foi possível realizar o Roll: " + status;
            }
        }
        else
        {
            GUILayout.Label("Você recebeu: " + result.PetName + " (" + result.Rarity + ")");
            if (GUILayout.Button("Continuar", GUILayout.Height(40))) { result = null; message = ""; }
        }
        GUILayout.Label(message);
        GUILayout.EndArea();
    }
}
