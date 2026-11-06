using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fluffy Fighters/Game Catalog")]
public sealed class GameCatalog : ScriptableObject
{
    [SerializeField] private PetData[] pets = Array.Empty<PetData>();
    [SerializeField] private RegionData[] regions = Array.Empty<RegionData>();

    public IReadOnlyList<PetData> Pets => pets;
    public IReadOnlyList<RegionData> Regions => regions;

    public bool TryGetPet(string id, out PetData pet)
    {
        pet = null;
        if (string.IsNullOrWhiteSpace(id) || pets == null) return false;
        foreach (PetData candidate in pets)
            if (candidate != null && candidate.PetId == id) { pet = candidate; return true; }
        return false;
    }

    public List<string> ValidateData()
    {
        var errors = new List<string>();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var regionIds = new HashSet<RegionId>();
        if (regions == null || regions.Length == 0) errors.Add("No regions configured.");
        else foreach (RegionData region in regions)
        {
            if (region == null) { errors.Add("Missing region reference."); continue; }
            if (!Enum.IsDefined(typeof(RegionId), region.Region) || !regionIds.Add(region.Region)) errors.Add("Invalid or duplicate region: " + region.name);
            if (string.IsNullOrWhiteSpace(region.DisplayName) || region.RollCost <= 0 || region.NextRegionCost < 0) errors.Add("Invalid region data: " + region.name);
        }
        if (pets == null || pets.Length == 0) errors.Add("No pets configured.");
        else foreach (PetData pet in pets)
        {
            if (pet == null) { errors.Add("Missing pet reference."); continue; }
            if (string.IsNullOrWhiteSpace(pet.PetId) || pet.PetId != pet.PetId.Trim() || !ids.Add(pet.PetId)) errors.Add("Invalid or duplicate pet ID: " + pet.name);
            if (string.IsNullOrWhiteSpace(pet.PetName) || pet.BaseDamage <= 0 || !Enum.IsDefined(typeof(PetRarity), pet.Rarity) || !regionIds.Contains(pet.Region)) errors.Add("Invalid pet data: " + pet.name);
        }
        return errors;
    }

    [ContextMenu("Validate Catalog")]
    private void ValidateInEditor()
    {
        var errors = ValidateData();
        if (errors.Count == 0) Debug.Log("Catalog valid.", this);
        else foreach (string error in errors) Debug.LogError(error, this);
    }
}
