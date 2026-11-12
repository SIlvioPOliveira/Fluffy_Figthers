using UnityEngine;

public sealed class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 5f;
    [SerializeField] private float targetHeight = 1.2f;
    [SerializeField] private float sensitivity = 3f;
    [SerializeField] private float pitch = 20f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 70f;
    [SerializeField, Min(0.01f)] private float collisionRadius = 0.35f;
    [SerializeField, Min(0f)] private float collisionPadding = 0.05f;
    [SerializeField] private LayerMask collisionLayers = Physics.DefaultRaycastLayers;
    private readonly RaycastHit[] collisionHits = new RaycastHit[32];
    private Camera controlledCamera;
    private float yaw;
    public bool ControlsEnabled { get; set; } = true;

    private void Start()
    {
        yaw = transform.eulerAngles.y;
        controlledCamera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null) return;
        if (ControlsEnabled && Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * sensitivity;
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * sensitivity, minPitch, maxPitch);
        }
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + Vector3.up * targetHeight;
        Vector3 direction = -(rotation * Vector3.forward);
        float radius = collisionRadius;
        if (controlledCamera != null && !controlledCamera.orthographic)
        {
            float near = controlledCamera.nearClipPlane;
            float halfHeight = near * Mathf.Tan(controlledCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float halfWidth = halfHeight * controlledCamera.aspect;
            radius = Mathf.Max(radius, Mathf.Sqrt(near * near + halfWidth * halfWidth + halfHeight * halfHeight));
        }
        float safeDistance = Mathf.Max(0f, distance);
        int count = Physics.SphereCastNonAlloc(pivot, radius, direction, collisionHits, safeDistance, collisionLayers, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < count; i++)
        {
            RaycastHit hit = collisionHits[i];
            if (hit.collider == null || hit.transform == target || hit.transform.IsChildOf(target)) continue;
            safeDistance = Mathf.Min(safeDistance, Mathf.Max(0f, hit.distance - collisionPadding));
        }
        transform.SetPositionAndRotation(pivot + direction * safeDistance, rotation);
    }
}
