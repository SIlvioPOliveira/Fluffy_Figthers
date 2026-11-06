using UnityEngine;

public enum PetRarity { Common, Rare, Epic, Legendary, Secret }
public enum RegionId { Water, Fire, Lava, Ice }

[CreateAssetMenu(menuName = "Fluffy Fighters/Pet")]
public sealed class PetData : ScriptableObject
{
    [SerializeField] private string petId;
    [SerializeField] private string petName;
    [SerializeField] private PetRarity rarity;
    [SerializeField] private RegionId region;
    [SerializeField, Min(1)] private int baseDamage = 1;
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject prefab;

    public string PetId => petId;
    public string PetName => petName;
    public PetRarity Rarity => rarity;
    public RegionId Region => region;
    public int BaseDamage => baseDamage;
    public Sprite Icon => icon;
    public GameObject Prefab => prefab;
}
