using UnityEngine;

public sealed class PetEquipmentView : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField, Min(0.1f)] private float followDistance = 2f;
    [SerializeField, Min(0.1f)] private float spacing = 1.2f;
    [SerializeField, Min(0.1f)] private float followSpeed = 6f;
    [SerializeField, Min(0.1f)] private float teleportDistance = 15f;
    [SerializeField] private Color placeholderColor = new Color(1f, 0.55f, 0.08f);
    [SerializeField] private CombatTargetTest combat;
    [SerializeField, Min(0.1f)] private float attackDistance = 1.5f;
    [SerializeField, Min(0f)] private float attackRecoil = 0.18f;
    [SerializeField, Min(0f)] private float attackLunge = 0.45f;
    [SerializeField, Min(0f)] private float attackHop = 0.18f;

    private const float WindupDuration = 0.12f;
    private const float ImpactTime = 0.22f;
    private const float AttackDuration = 0.4f;

    private readonly GameObject[] views = new GameObject[PlayerInventory.MaxEquipment];
    private readonly PetStackKey?[] displayed = new PetStackKey?[PlayerInventory.MaxEquipment];
    private readonly float[] nextAttack = new float[PlayerInventory.MaxEquipment];
    private readonly Transform[] models = new Transform[PlayerInventory.MaxEquipment];
    private readonly float[] modelRadii = new float[PlayerInventory.MaxEquipment];
    private readonly Vector3[] modelOrigins = new Vector3[PlayerInventory.MaxEquipment];
    private readonly EnemyHealth[] attackTargets = new EnemyHealth[PlayerInventory.MaxEquipment];
    private readonly float[] attackStarted = new float[PlayerInventory.MaxEquipment];
    private readonly bool[] impactApplied = new bool[PlayerInventory.MaxEquipment];
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
            nextAttack[i] = 0f;
            models[i] = null;
            attackTargets[i] = null;
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
            nextAttack[i] = 0f;
            models[i] = null;
            attackTargets[i] = null;
            if (!key.HasValue || !catalog.TryGetPet(key.Value.PetId, out PetData pet)) continue;
            var view = new GameObject("Pet " + pet.PetName + " (slot " + (i + 1) + ")");
            GameObject model;
            if (pet.Prefab != null) model = Instantiate(pet.Prefab, view.transform, false);
            else
            {
                model = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                model.transform.SetParent(view.transform, false);
                model.transform.localScale = Vector3.one * 0.6f;
                var block = new MaterialPropertyBlock();
                block.SetColor("_Color", placeholderColor);
                block.SetColor("_BaseColor", placeholderColor);
                model.GetComponent<Renderer>().SetPropertyBlock(block);
            }
            foreach (Collider collider in view.GetComponentsInChildren<Collider>()) collider.enabled = false;
            models[i] = model.transform;
            modelOrigins[i] = model.transform.localPosition;
            modelRadii[i] = VisualRadius(view.transform);
            view.transform.position = player.position - player.forward * followDistance;
            Vector3 playerFacing = Vector3.ProjectOnPlane(player.forward, Vector3.up);
            if (playerFacing.sqrMagnitude > 0.001f) view.transform.rotation = Quaternion.LookRotation(playerFacing);
            views[i] = view;
            displayed[i] = key;
        }
    }

    private static bool NullableEquals(PetStackKey? a, PetStackKey? b)
        => a.HasValue == b.HasValue && (!a.HasValue || a.Value.Equals(b.Value));

    private static float VisualRadius(Transform root)
    {
        float radius = 0f;
        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>())
        {
            Bounds bounds = renderer.bounds;
            Vector3 offset = bounds.center - root.position;
            float centerDistance = new Vector2(offset.x, offset.z).magnitude;
            float extent = new Vector2(bounds.extents.x, bounds.extents.z).magnitude;
            radius = Mathf.Max(radius, centerDistance + extent);
        }
        return radius;
    }

    private float AttackDistanceFor(EnemyHealth target, int slot)
    {
        Collider hitbox = target.GetComponent<Collider>();
        if (hitbox == null) return attackDistance;
        Vector3 extents = hitbox.bounds.extents;
        float targetRadius = Mathf.Max(extents.x, extents.z);
        // Include the pet's size and forward lunge so its model stays outside.
        return Mathf.Max(attackDistance, targetRadius + modelRadii[slot] + attackLunge + 0.25f);
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 0f) return;
        if (player == null) return;
        EnemyHealth target = combat != null ? combat.Target : null;
        if (target != null && !target.IsAlive) target = null;
        int count = 0;
        for (int i = 0; i < views.Length; i++) if (views[i] != null) count++;
        int index = 0;
        for (int i = 0; i < views.Length; i++)
        {
            if (views[i] == null) continue;
            float lateral = (index++ - (count - 1) * 0.5f) * spacing;
            Vector3 goal = player.position - player.forward * followDistance + player.right * lateral;
            if (target != null)
            {
                Vector3 towardPlayer = Vector3.ProjectOnPlane(player.position - target.transform.position, Vector3.up).normalized;
                if (towardPlayer.sqrMagnitude < 0.01f) towardPlayer = -player.forward;
                float angle = (index - 1 - (count - 1) * 0.5f) * 45f;
                goal = target.transform.position + Quaternion.Euler(0, angle, 0) * towardPlayer * AttackDistanceFor(target, i);
            }
            else nextAttack[i] = 0f;
            goal.y = player.position.y - 0.7f;
            Transform pet = views[i].transform;
            Vector3 previousPosition = pet.position;
            if (target == null && Vector3.Distance(pet.position, goal) > teleportDistance) pet.position = goal;
            else pet.position = Vector3.MoveTowards(pet.position, goal, followSpeed * Time.deltaTime);
            if (target != null)
            {
                Vector3 facing = Vector3.ProjectOnPlane(target.transform.position - pet.position, Vector3.up);
                if (facing.sqrMagnitude > 0.001f) pet.rotation = Quaternion.LookRotation(facing);
            }
            else
            {
                Vector3 movement = Vector3.ProjectOnPlane(pet.position - previousPosition, Vector3.up);
                if (movement.sqrMagnitude > 0.00000001f) pet.rotation = Quaternion.LookRotation(movement);
            }
            UpdateAttack(i, target, goal);
        }
    }

    private void UpdateAttack(int slot, EnemyHealth target, Vector3 goal)
    {
        Transform model = models[slot];
        if (model == null) return;
        Transform pet = views[slot].transform;
        bool withinReach = target != null && target.IsAlive &&
            Vector3.ProjectOnPlane(target.transform.position - pet.position, Vector3.up).magnitude <= AttackDistanceFor(target, slot) + 0.15f;

        if (attackTargets[slot] != null && (attackTargets[slot] != target || !withinReach))
            attackTargets[slot] = null;

        if (attackTargets[slot] == null && withinReach && Vector3.Distance(pet.position, goal) <= 0.05f && Time.time >= nextAttack[slot])
        {
            attackTargets[slot] = target;
            attackStarted[slot] = Time.time;
            impactApplied[slot] = false;
            // Preserve this cooldown even if the player switches targets during the windup.
            nextAttack[slot] = Time.time + 1f;
        }

        if (attackTargets[slot] == null)
        {
            model.localPosition = modelOrigins[slot];
            return;
        }

        float elapsed = Time.time - attackStarted[slot];
        if (!impactApplied[slot] && elapsed >= ImpactTime)
        {
            impactApplied[slot] = true;
            if (withinReach && attackTargets[slot] == target && displayed[slot].HasValue && catalog.TryGetPet(displayed[slot].Value.PetId, out PetData data))
                target.TakeDamage(PetDamage.Calculate(data.BaseDamage, displayed[slot].Value.FusionLevel), data.Rarity);
        }

        if (elapsed >= AttackDuration)
        {
            attackTargets[slot] = null;
            model.localPosition = modelOrigins[slot];
            return;
        }
        float forward = elapsed < WindupDuration
            ? Mathf.Lerp(0f, -attackRecoil, elapsed / WindupDuration)
            : elapsed < ImpactTime
                ? Mathf.Lerp(-attackRecoil, attackLunge, (elapsed - WindupDuration) / (ImpactTime - WindupDuration))
                : Mathf.Lerp(attackLunge, 0f, (elapsed - ImpactTime) / (AttackDuration - ImpactTime));
        float hop = Mathf.Sin(Mathf.Clamp01(elapsed / AttackDuration) * Mathf.PI) * attackHop;
        model.localPosition = modelOrigins[slot] + Vector3.forward * forward + Vector3.up * hop;
    }
}
