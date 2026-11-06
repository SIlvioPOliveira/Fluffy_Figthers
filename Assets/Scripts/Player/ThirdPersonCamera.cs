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
    private float yaw;

    private void Start() => yaw = transform.eulerAngles.y;

    private void LateUpdate()
    {
        if (target == null) return;
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * sensitivity;
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * sensitivity, minPitch, maxPitch);
        }
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.SetPositionAndRotation(target.position + Vector3.up * targetHeight - rotation * Vector3.forward * distance, rotation);
    }
}
