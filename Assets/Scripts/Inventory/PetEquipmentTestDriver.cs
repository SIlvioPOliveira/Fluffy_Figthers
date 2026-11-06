using UnityEngine;

// A dedicated test-scene driver, not the new-game inventory or the final UI.
public sealed class PetEquipmentTestDriver : MonoBehaviour
{
    [SerializeField] private GameCatalog catalog;
    [SerializeField] private PetEquipmentView equipmentView;
    [SerializeField] private Transform player;
    [SerializeField] private ThirdPersonCamera cameraController;
    [SerializeField] private bool seedTestPets = true;
    public PlayerInventory Inventory { get; private set; }
    public CurrencyWallet Wallet { get; private set; }
    public GameCatalog Catalog => catalog;
    public Transform Player => player;
    public ThirdPersonCamera CameraController => cameraController;
    public bool InventoryOpen { get; set; }
    public bool GachaOpen { get; set; }

    private void Awake()
    {
        if (catalog == null || equipmentView == null || player == null)
        {
            Debug.LogError("Pet equipment test scene is missing references.", this);
            enabled = false;
            return;
        }
        Inventory = new PlayerInventory(catalog);
        Wallet = new CurrencyWallet();
        equipmentView.Initialize(Inventory, catalog, player);
        if (seedTestPets)
        {
            Inventory.TryAdd("water_peixinho", 1, 8);
            Inventory.TryAdd("water_tartaruga", 1, 3);
        }
    }

    private void Update()
    {
        if (InventoryOpen || GachaOpen) return;
        for (int i = 0; i < PlayerInventory.MaxEquipment; i++)
        {
            if (!Input.GetKeyDown(KeyCode.Alpha1 + i)) continue;
            if (Inventory.GetSlot(i).HasValue) Inventory.TryUnequip(i);
            else Inventory.TryEquip("water_peixinho", 1, i);
        }
    }

    private void OnGUI()
    {
        if (!InventoryOpen && !GachaOpen) GUI.Label(new Rect(15, 15, 900, 30), "Teste: I abre inventário; 1–3 alternam Peixinhos; WASD movimenta.");
    }
}
