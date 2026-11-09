using System;
using System.Collections.Generic;

[Serializable]
public sealed class PlayerSaveData
{
    public int version;
    public int coins;
    public List<PetStackSave> pets;
    public PetSlotSave[] slots;
    public bool fireUnlocked;
    public bool lavaUnlocked;
    public bool iceUnlocked;
    public bool finalBossDefeated;
    public int tutorialStage;
    public bool tutorialEnemyDefeated;
    public int tutorialRollCount;
    public bool tutorialWaterBossDefeated;
}

[Serializable]
public sealed class PetStackSave
{
    public string petId;
    public int level;
    public int quantity;
}

[Serializable]
public sealed class PetSlotSave
{
    public string petId;
    public int level;
}

public static class PlayerSaveCodec
{
    public const int CurrentVersion = 5;

    public static PlayerSaveData Capture(CurrencyWallet wallet, PlayerInventory inventory, bool fireUnlocked, int tutorialStage = 0, bool tutorialEnemyDefeated = false, int tutorialRollCount = 0, bool tutorialWaterBossDefeated = false, bool lavaUnlocked = false, bool iceUnlocked = false, bool finalBossDefeated = false)
    {
        var data = new PlayerSaveData { version = CurrentVersion, coins = wallet.Balance, fireUnlocked = fireUnlocked, lavaUnlocked = lavaUnlocked, iceUnlocked = iceUnlocked, finalBossDefeated = finalBossDefeated, tutorialStage = tutorialStage, tutorialEnemyDefeated = tutorialEnemyDefeated, tutorialRollCount = tutorialRollCount, tutorialWaterBossDefeated = tutorialWaterBossDefeated, pets = new List<PetStackSave>(), slots = new PetSlotSave[PlayerInventory.MaxEquipment] };
        foreach (InventoryEntry entry in inventory.GetEntries())
            data.pets.Add(new PetStackSave { petId = entry.Key.PetId, level = entry.Key.FusionLevel, quantity = entry.Total });
        for (int i = 0; i < data.slots.Length; i++)
        {
            PetStackKey? slot = inventory.GetSlot(i);
            data.slots[i] = slot.HasValue ? new PetSlotSave { petId = slot.Value.PetId, level = slot.Value.FusionLevel } : new PetSlotSave { petId = "", level = 0 };
        }
        return data;
    }

    // Restore into a new inventory so invalid input can never partially modify the live game.
    public static bool TryRestore(PlayerSaveData data, GameCatalog catalog, out CurrencyWallet wallet, out PlayerInventory inventory)
    {
        wallet = null;
        inventory = null;
        if (data == null || catalog == null || data.version < 1 || data.version > CurrentVersion || data.coins < 0 || (data.version >= 2 && (data.tutorialStage < 0 || data.tutorialStage > (data.version == 2 ? 4 : data.version == 3 ? 6 : 8))) || (data.version >= 3 && (data.tutorialRollCount < 0 || data.tutorialRollCount > 2)) || data.pets == null || data.slots == null || data.slots.Length != PlayerInventory.MaxEquipment) return false;
        if (data.version >= 5 && ((data.lavaUnlocked && !data.fireUnlocked) || (data.iceUnlocked && !data.lavaUnlocked))) return false;
        var restored = new PlayerInventory(catalog);
        var keys = new HashSet<PetStackKey>();
        foreach (PetStackSave stack in data.pets)
        {
            if (stack == null || stack.quantity <= 0 || !keys.Add(new PetStackKey(stack.petId, stack.level)) || !restored.TryAdd(stack.petId, stack.level, stack.quantity)) return false;
        }
        for (int i = 0; i < data.slots.Length; i++)
        {
            PetSlotSave slot = data.slots[i];
            if (slot == null) return false;
            if (string.IsNullOrEmpty(slot.petId)) { if (slot.level != 0) return false; continue; }
            if (!restored.TryEquip(slot.petId, slot.level, i)) return false;
        }
        wallet = new CurrencyWallet(data.coins);
        inventory = restored;
        return true;
    }
}
