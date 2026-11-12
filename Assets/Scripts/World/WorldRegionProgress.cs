using UnityEngine;

// Provisional region entry points for the continuous test world.
public sealed class WorldRegionProgress : MonoBehaviour
{
    [SerializeField] private PetEquipmentTestDriver driver;
    [SerializeField] private PlayerHealthTest health;
    [SerializeField] private Transform[] regionEntrances;
    private int activeRegion = -1;

    private void Start()
    {
        if (driver == null || health == null || regionEntrances == null || regionEntrances.Length != 4) { enabled = false; return; }
        int start = Mathf.Clamp(driver.HighestUnlockedRegion, 0, 3);
        Transform spawn = regionEntrances[start];
        if (spawn == null) { enabled = false; return; }
        var controller = health.GetComponent<CharacterController>();
        controller.enabled = false;
        health.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
        controller.enabled = true;
        SetRegion(start);
    }

    private void Update()
    {
        float z = health.transform.position.z;
        int index = z >= 170f ? 3 : z >= 100f ? 2 : z >= 30f ? 1 : 0;
        if (index != activeRegion) SetRegion(index);
    }

    private void SetRegion(int index)
    {
        if (regionEntrances[index] == null) return;
        activeRegion = index;
        health.SetRespawnPoint(regionEntrances[index]);
    }
}
