using UnityEngine;

// A dedicated test-scene driver, not the new-game inventory or the final UI.
public sealed class PetEquipmentTestDriver : MonoBehaviour
{
    [SerializeField] private GameCatalog catalog;
    [SerializeField] private PetEquipmentView equipmentView;
    [SerializeField] private Transform player;
    [SerializeField] private ThirdPersonCamera cameraController;
    [SerializeField] private bool seedTestPets = true;
    [SerializeField] private string quickEquipPetId = "water_peixinho";
    public PlayerInventory Inventory { get; private set; }
    public CurrencyWallet Wallet { get; private set; }
    public GameCatalog Catalog => catalog;
    public Transform Player => player;
    public ThirdPersonCamera CameraController => cameraController;
    public bool InventoryOpen { get; set; }
    public bool GachaOpen { get; set; }
    public bool BarrierOpen { get; set; }

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
        if (InventoryOpen || GachaOpen || BarrierOpen) return;
        for (int i = 0; i < PlayerInventory.MaxEquipment; i++)
        {
            if (!Input.GetKeyDown(KeyCode.Alpha1 + i)) continue;
            if (Inventory.GetSlot(i).HasValue) Inventory.TryUnequip(i);
            else if (!Inventory.TryEquip(quickEquipPetId, 1, i))
            {
                // In empty-start scenes, the first Gacha reward can be any rarity.
                foreach (InventoryEntry entry in Inventory.GetEntries())
                    if (entry.Available > 0 && Inventory.TryEquip(entry.Key.PetId, entry.Key.FusionLevel, i)) break;
            }
        }
    }

    private void OnGUI()
    {
        if (!InventoryOpen && !GachaOpen && !BarrierOpen) GUI.Label(new Rect(15, 15, 900, 30), "Teste: I abre inventário; 1–3 alternam Pets; WASD movimenta.");
    }
}
