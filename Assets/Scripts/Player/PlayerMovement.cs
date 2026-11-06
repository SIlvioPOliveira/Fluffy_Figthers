using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float gravity = -20f;
    private CharacterController controller;
    private float verticalVelocity;
    public bool ControlsEnabled { get; set; } = true;

    private void Awake() => controller = GetComponent<CharacterController>();

    private void Update()
    {
        Vector2 input = ControlsEnabled ? Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f) : Vector2.zero;
        Transform basis = cameraTransform != null ? cameraTransform : transform;
        Vector3 forward = Vector3.ProjectOnPlane(basis.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(basis.right, Vector3.up).normalized;
        Vector3 direction = forward * input.y + right * input.x;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }
        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move((direction * speed + Vector3.up * verticalVelocity) * Time.deltaTime);
    }
}
