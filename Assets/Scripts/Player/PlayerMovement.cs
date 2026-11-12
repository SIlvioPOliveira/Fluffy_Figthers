using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float gravity = -20f;
    [SerializeField, Min(0.1f)] private float acceleration = 32f;
    [SerializeField, Min(0.1f)] private float deceleration = 40f;
    private CharacterController controller;
    private float verticalVelocity;
    private Vector3 planarVelocity;
    private Vector3 lastPosition;
    private bool hasLastPosition;
    public bool ControlsEnabled { get; set; } = true;

    private void Awake() => controller = GetComponent<CharacterController>();

    private void Update()
    {
        Vector2 input = ControlsEnabled ? Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f) : Vector2.zero;
        MovePlayer(input, Time.deltaTime);
    }

    private void MovePlayer(Vector2 input, float deltaTime)
    {
        // A respawn or region teleport must not carry momentum into the new position.
        if (hasLastPosition && (transform.position - lastPosition).sqrMagnitude > 9f)
            planarVelocity = Vector3.zero;
        Transform basis = cameraTransform != null ? cameraTransform : transform;
        Vector3 forward = Vector3.ProjectOnPlane(basis.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(basis.right, Vector3.up).normalized;
        Vector3 direction = forward * input.y + right * input.x;
        Vector3 desiredVelocity = direction * speed;
        if (!ControlsEnabled) planarVelocity = Vector3.zero;
        else planarVelocity = Vector3.MoveTowards(planarVelocity, desiredVelocity,
            (direction.sqrMagnitude > 0.001f ? acceleration : deceleration) * deltaTime);
        if (planarVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(planarVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * deltaTime);
        }
        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        verticalVelocity += gravity * deltaTime;
        controller.Move((planarVelocity + Vector3.up * verticalVelocity) * deltaTime);
        lastPosition = transform.position;
        hasLastPosition = true;
    }
}
