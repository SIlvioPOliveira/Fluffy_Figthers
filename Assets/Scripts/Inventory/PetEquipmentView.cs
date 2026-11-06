using UnityEngine;

public sealed class PetEquipmentView : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField, Min(0.1f)] private float followDistance = 2f;
    [SerializeField, Min(0.1f)] private float spacing = 1.2f;
    [SerializeField, Min(0.1f)] private float followSpeed = 6f;
    [SerializeField, Min(0.1f)] private float teleportDistance = 15f;

    private readonly GameObject[] views = new GameObject[PlayerInventory.MaxEquipment];
    private readonly PetStackKey?[] displayed = new PetStackKey?[PlayerInventory.MaxEquipment];
    private PlayerInventory inventory;
    private GameCatalog catalog;

    public void Initialize(PlayerInventory source, GameCatalog petCatalog, Transform playerTransform)
    {
        if (source == null) throw new System.ArgumentNullException(nameof(source));
        if (petCatalog == null) throw new System.ArgumentNullException(nameof(petCatalog));
        if (playerTransform == null) throw new System.ArgumentNullException(nameof(playerTransform));
        if (inventory != null) inventory.Changed -= Sync;
        ClearViews();
        inventory = source;
        catalog = petCatalog;
        player = playerTransform;
        inventory.Changed += Sync;
        Sync();
    }

    private void OnDestroy()
    {
        if (inventory != null) inventory.Changed -= Sync;
        ClearViews();
    }

    private void ClearViews()
    {
        for (int i = 0; i < views.Length; i++)
        {
            if (views[i] != null) Destroy(views[i]);
            views[i] = null;
            displayed[i] = null;
        }
    }

    private void Sync()
    {
        for (int i = 0; i < views.Length; i++)
        {
            var key = inventory.GetSlot(i);
            if (NullableEquals(key, displayed[i]) && (key == null || views[i] != null)) continue;
            if (views[i] != null) Destroy(views[i]);
            views[i] = null;
            displayed[i] = null;
            if (!key.HasValue || !catalog.TryGetPet(key.Value.PetId, out PetData pet)) continue;
            GameObject view;
            if (pet.Prefab != null) view = Instantiate(pet.Prefab);
            else
            {
                view = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                view.transform.localScale = Vector3.one * 0.6f;
            }
            foreach (Collider collider in view.GetComponentsInChildren<Collider>()) collider.enabled = false;
            view.name = "Pet " + pet.PetName + " (slot " + (i + 1) + ")";
            view.transform.position = player.position - player.forward * followDistance;
            views[i] = view;
            displayed[i] = key;
        }
    }

    private static bool NullableEquals(PetStackKey? a, PetStackKey? b)
        => a.HasValue == b.HasValue && (!a.HasValue || a.Value.Equals(b.Value));

    private void LateUpdate()
    {
        if (player == null) return;
        int count = 0;
        for (int i = 0; i < views.Length; i++) if (views[i] != null) count++;
        int index = 0;
        for (int i = 0; i < views.Length; i++)
        {
            if (views[i] == null) continue;
            float lateral = (index++ - (count - 1) * 0.5f) * spacing;
            Vector3 goal = player.position - player.forward * followDistance + player.right * lateral;
            goal.y = player.position.y - 0.7f;
            Transform pet = views[i].transform;
            if (Vector3.Distance(pet.position, goal) > teleportDistance) pet.position = goal;
            else pet.position = Vector3.MoveTowards(pet.position, goal, followSpeed * Time.deltaTime);
        }
    }
}
