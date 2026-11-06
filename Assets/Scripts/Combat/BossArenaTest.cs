using UnityEngine;

// The sphere trigger marks the playable area; only the assigned player can end combat.
[RequireComponent(typeof(SphereCollider))]
public sealed class BossArenaTest : MonoBehaviour
{
    [SerializeField] private PlayerHealthTest player;
    [SerializeField] private BossControllerTest boss;

    public bool ContainsPlayer()
    {
        if (player == null) return false;
        SphereCollider sphere = GetComponent<SphereCollider>();
        Vector3 center = transform.TransformPoint(sphere.center);
        Vector3 offset = Vector3.ProjectOnPlane(player.transform.position - center, Vector3.up);
        float radius = sphere.radius * Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.z));
        return offset.sqrMagnitude <= radius * radius;
    }

    public Vector3 ClampInside(Vector3 position)
    {
        SphereCollider sphere = GetComponent<SphereCollider>();
        Vector3 center = transform.TransformPoint(sphere.center);
        float radius = sphere.radius * Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.z));
        Vector3 offset = Vector3.ProjectOnPlane(position - center, Vector3.up);
        if (offset.magnitude > radius) offset = offset.normalized * radius;
        return new Vector3(center.x + offset.x, position.y, center.z + offset.z);
    }

    private void OnTriggerExit(Collider other)
    {
        if (player != null && boss != null && other.GetComponentInParent<PlayerHealthTest>() == player && !ContainsPlayer())
            boss.ResetEncounter();
    }
}
