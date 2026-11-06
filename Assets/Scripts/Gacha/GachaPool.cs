using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class GachaEntry
{
    [SerializeField] private PetData pet;
    [SerializeField, Min(0)] private int weight;
    public PetData Pet => pet;
    // 10,000 points equals 100%; one point equals 0.01%.
    public int Weight => weight;
}

[CreateAssetMenu(menuName = "Fluffy Fighters/Gacha Pool")]
public sealed class GachaPool : ScriptableObject
{
    public const int TotalWeight = 10000;
    [SerializeField] private RegionData region;
    [SerializeField] private GachaEntry[] entries = Array.Empty<GachaEntry>();
    public RegionData Region => region;
    public IReadOnlyList<GachaEntry> Entries => entries;

    public bool IsValid(GameCatalog catalog)
    {
        if (catalog == null || region == null || region.RollCost <= 0 || entries == null || entries.Length != 5) return false;
        long sum = 0;
        var rarities = new HashSet<PetRarity>();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (GachaEntry entry in entries)
        {
            if (entry == null || entry.Pet == null || entry.Weight < 0 || entry.Pet.Region != region.Region ||
                !catalog.TryGetPet(entry.Pet.PetId, out PetData registered) || registered != entry.Pet ||
                !ids.Add(entry.Pet.PetId) || !rarities.Add(entry.Pet.Rarity)) return false;
            sum += entry.Weight;
        }
        return sum == TotalWeight;
    }

    public PetData Select(int ticket)
    {
        if (ticket < 0 || ticket >= TotalWeight || entries == null) return null;
        long cumulative = 0;
        foreach (GachaEntry entry in entries)
        {
            if (entry == null || entry.Pet == null || entry.Weight < 0) return null;
            cumulative += entry.Weight;
            if (ticket < cumulative) return entry.Pet;
        }
        return null;
    }
}
