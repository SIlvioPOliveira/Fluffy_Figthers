using UnityEngine;

[CreateAssetMenu(menuName = "Fluffy Fighters/Region")]
public sealed class RegionData : ScriptableObject
{
    [SerializeField] private RegionId region;
    [SerializeField] private string displayName;
    [SerializeField, Min(1)] private int rollCost = 100;
    [SerializeField, Min(0)] private int nextRegionCost;

    public RegionId Region => region;
    public string DisplayName => displayName;
    public int RollCost => rollCost;
    public int NextRegionCost => nextRegionCost;
}
