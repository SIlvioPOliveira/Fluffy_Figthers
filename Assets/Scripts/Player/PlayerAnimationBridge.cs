using UnityEngine;

// The gameplay controller turns toward its travel direction, so walking always uses
// the forward clip. Camera-relative strafing is a separate movement design decision.
[RequireComponent(typeof(PlayerMovement), typeof(CharacterController))]
public sealed class PlayerAnimationBridge : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField, Min(0f)] private float movementThreshold = .05f;
    [SerializeField, Min(0f)] private float transitionSeconds = .12f;
    private CharacterController controller;
    private PlayerMovement movement;
    private bool? walking;
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Forward = Animator.StringToHash("Frente");

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        movement = GetComponent<PlayerMovement>();
        if (animator != null) animator.applyRootMotion = false;
    }

    private void LateUpdate()
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;
        Vector3 velocity = controller.velocity;
        velocity.y = 0;
        bool moving = movement.ControlsEnabled && velocity.sqrMagnitude > movementThreshold * movementThreshold;
        if (walking.HasValue && walking.Value == moving) return;
        walking = moving;
        animator.CrossFadeInFixedTime(moving ? Forward : Idle, transitionSeconds);
    }
}
