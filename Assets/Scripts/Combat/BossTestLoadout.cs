using UnityEngine;

// Only used in standalone boss test scenes to make the encounter practical to test.
public sealed class BossTestLoadout : MonoBehaviour
{
    [SerializeField] private PetEquipmentTestDriver driver;
    [SerializeField] private PetData testPet;

    private void Start()
    {
        if (driver == null || driver.Inventory == null || testPet == null)
        {
            Debug.LogError("Boss test loadout is missing references.", this);
            return;
        }
        if (!driver.Inventory.TryAdd(testPet.PetId, 1, 3)) return;
        for (int i = 0; i < PlayerInventory.MaxEquipment; i++) driver.Inventory.TryEquip(testPet.PetId, 1, i);
    }
}
