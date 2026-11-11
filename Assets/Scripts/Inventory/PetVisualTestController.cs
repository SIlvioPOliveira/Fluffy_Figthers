using UnityEngine;

// Attached only to the dedicated visual-test scene; the inventory there is not persisted.
[RequireComponent(typeof(PetEquipmentTestDriver))]
public sealed class PetVisualTestController : MonoBehaviour
{
    private PetEquipmentTestDriver driver;
    private int selected;
    private string message;
    private static Rect Panel => new Rect(Screen.width - 330, 75, 315, 190);

    private void Start()
    {
        driver = GetComponent<PetEquipmentTestDriver>();
        if (driver.PersistenceEnabled)
        {
            Debug.LogError("O seletor de testes precisa de uma cena sem salvamento.", this);
            enabled = false;
            return;
        }
        foreach (var pet in driver.Catalog.Pets) driver.Inventory.TryAdd(pet.PetId, 1, 3);
    }

    public static bool PointerOverPanel()
    {
        var test = FindFirstObjectByType<PetVisualTestController>();
        if (test == null || !test.isActiveAndEnabled) return false;
        Vector3 p = Input.mousePosition;
        return Panel.Contains(new Vector2(p.x, Screen.height - p.y));
    }

    private void Equip(int slot)
    {
        var pet = driver.Catalog.Pets[selected];
        driver.Inventory.TryUnequip(slot);
        message = driver.Inventory.TryEquip(pet.PetId, 1, slot) ? pet.PetName + " equipado no espaço " + (slot + 1) : "Não foi possível equipar.";
    }

    private void OnGUI()
    {
        if (driver == null || driver.Catalog == null || driver.Catalog.Pets.Count == 0 || driver.PauseOpen || driver.InventoryOpen) return;
        GUILayout.BeginArea(Panel, GUI.skin.box);
        GUILayout.Label("TESTE DE MODELOS — NÃO ALTERA SEU SAVE");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<", GUILayout.Width(45))) selected = (selected - 1 + driver.Catalog.Pets.Count) % driver.Catalog.Pets.Count;
        var pet = driver.Catalog.Pets[selected];
        GUILayout.Label(pet.PetName + "  (" + (selected + 1) + "/" + driver.Catalog.Pets.Count + ")", GUILayout.ExpandWidth(true));
        if (GUILayout.Button(">", GUILayout.Width(45))) selected = (selected + 1) % driver.Catalog.Pets.Count;
        GUILayout.EndHorizontal();
        GUILayout.Label("Equipar no espaço:");
        GUILayout.BeginHorizontal();
        for (int i = 0; i < PlayerInventory.MaxEquipment; i++) if (GUILayout.Button((i + 1).ToString())) Equip(i);
        GUILayout.EndHorizontal();
        GUILayout.Label(message ?? "Use WASD e selecione um inimigo para testar combate.");
        GUILayout.EndArea();
    }
}
