using System;
using System.Collections.Generic;

public readonly struct PetStackKey : IEquatable<PetStackKey>
{
    public string PetId { get; }
    public int FusionLevel { get; }
    public PetStackKey(string petId, int fusionLevel) { PetId = petId; FusionLevel = fusionLevel; }
    public bool Equals(PetStackKey other) => PetId == other.PetId && FusionLevel == other.FusionLevel;
    public override bool Equals(object obj) => obj is PetStackKey other && Equals(other);
    public override int GetHashCode() => ((PetId != null ? StringComparer.Ordinal.GetHashCode(PetId) : 0) * 397) ^ FusionLevel;
}

public readonly struct InventoryEntry
{
    public PetStackKey Key { get; }
    public int Total { get; }
    public int Equipped { get; }
    public int Available => Total - Equipped;
    public InventoryEntry(PetStackKey key, int total, int equipped) { Key = key; Total = total; Equipped = equipped; }
}

public sealed class PlayerInventory
{
    public const int MaxEquipment = 3;
    public const int MaxFusionLevel = 3;
    private readonly GameCatalog catalog;
    private readonly Dictionary<PetStackKey, int> quantities = new Dictionary<PetStackKey, int>();
    private readonly PetStackKey?[] slots = new PetStackKey?[MaxEquipment];
    public event Action Changed;

    public PlayerInventory(GameCatalog catalog)
    {
        this.catalog = catalog != null ? catalog : throw new ArgumentNullException(nameof(catalog));
    }

    public PetStackKey? GetSlot(int slot) => slot >= 0 && slot < slots.Length ? slots[slot] : null;

    public InventoryEntry GetEntry(PetStackKey key)
    {
        quantities.TryGetValue(key, out int total);
        int equipped = 0;
        foreach (var slot in slots) if (slot.HasValue && slot.Value.Equals(key)) equipped++;
        return new InventoryEntry(key, total, equipped);
    }

    public List<InventoryEntry> GetEntries()
    {
        var entries = new List<InventoryEntry>();
        foreach (var pair in quantities) entries.Add(GetEntry(pair.Key));
        return entries;
    }

    public bool TryAdd(string petId, int level = 1, int amount = 1)
    {
        if (!TryAddSilently(petId, level, amount)) return false;
        NotifyChanged();
        return true;
    }

    internal bool CanAdd(string petId, int level, int amount)
    {
        if (amount <= 0 || !IsValid(petId, level)) return false;
        var key = new PetStackKey(petId, level);
        quantities.TryGetValue(key, out int total);
        return amount <= int.MaxValue - total;
    }

    internal bool TryAddSilently(string petId, int level, int amount)
    {
        if (!CanAdd(petId, level, amount)) return false;
        var key = new PetStackKey(petId, level);
        quantities.TryGetValue(key, out int total);
        quantities[key] = total + amount;
        return true;
    }

    internal void NotifyChanged() => Changed?.Invoke();

    public bool TryEquip(string petId, int level, int slot)
    {
        if (slot < 0 || slot >= slots.Length || slots[slot].HasValue || !IsValid(petId, level)) return false;
        var key = new PetStackKey(petId, level);
        if (GetEntry(key).Available <= 0) return false;
        slots[slot] = key;
        Changed?.Invoke();
        return true;
    }

    public bool TryUnequip(int slot)
    {
        if (slot < 0 || slot >= slots.Length || !slots[slot].HasValue) return false;
        slots[slot] = null;
        Changed?.Invoke();
        return true;
    }

    public bool TryFuse(string petId, int level)
    {
        if (level < 1 || level >= MaxFusionLevel || !IsValid(petId, level)) return false;
        var source = new PetStackKey(petId, level);
        var target = new PetStackKey(petId, level + 1);
        var entry = GetEntry(source);
        quantities.TryGetValue(target, out int targetTotal);
        if (entry.Available < 3 || targetTotal == int.MaxValue) return false;
        int remaining = entry.Total - 3;
        if (remaining == 0) quantities.Remove(source); else quantities[source] = remaining;
        quantities[target] = targetTotal + 1;
        Changed?.Invoke();
        return true;
    }

    public long TeamDamage
    {
        get
        {
            long sum = 0;
            foreach (var slot in slots)
                if (slot.HasValue && catalog.TryGetPet(slot.Value.PetId, out var pet)) sum += PetDamage.Calculate(pet.BaseDamage, slot.Value.FusionLevel);
            return sum;
        }
    }

    private bool IsValid(string id, int level) => level >= 1 && level <= MaxFusionLevel && catalog.TryGetPet(id, out _);
}

public static class PetDamage
{
    // Exact integer arithmetic implements positive midpoint rounding away from zero.
    public static long Calculate(int baseDamage, int level)
    {
        if (baseDamage <= 0) throw new ArgumentOutOfRangeException(nameof(baseDamage));
        switch (level)
        {
            case 1: return baseDamage;
            case 2: return ((long)baseDamage * 3 + 1) / 2;
            case 3: return ((long)baseDamage * 9 + 2) / 4;
            default: throw new ArgumentOutOfRangeException(nameof(level));
        }
    }
}
