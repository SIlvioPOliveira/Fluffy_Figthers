using UnityEngine;

// A dedicated test-scene driver, not the new-game inventory or the final UI.
public sealed class PetEquipmentTestDriver : MonoBehaviour
{
    [SerializeField] private GameCatalog catalog;
    [SerializeField] private PetEquipmentView equipmentView;
    [SerializeField] private Transform player;
    public PlayerInventory Inventory { get; private set; }

    private void Start()
    {
        if (catalog == null || equipmentView == null || player == null)
        {
            Debug.LogError("Pet equipment test scene is missing references.", this);
            enabled = false;
            return;
        }
        Inventory = new PlayerInventory(catalog);
        equipmentView.Initialize(Inventory, catalog, player);
        Inventory.TryAdd("water_peixinho", 1, 3);
    }

    private void Update()
    {
        for (int i = 0; i < PlayerInventory.MaxEquipment; i++)
        {
            if (!Input.GetKeyDown(KeyCode.Alpha1 + i)) continue;
            if (Inventory.GetSlot(i).HasValue) Inventory.TryUnequip(i);
            else Inventory.TryEquip("water_peixinho", 1, i);
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(15, 15, 800, 30), "Teste provisório: teclas 1, 2 e 3 equipam/desequipam Pets. WASD movimenta.");
    }
}
