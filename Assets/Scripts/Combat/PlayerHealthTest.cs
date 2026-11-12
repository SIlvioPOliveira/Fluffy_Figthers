using System;
using UnityEngine;

// Temporary player health and respawn for the isolated boss test scene.
[RequireComponent(typeof(CharacterController))]
public sealed class PlayerHealthTest : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 3;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private CombatTargetTest targetManager;
    [SerializeField] private ThirdPersonCamera damageCamera;
    public int Health { get; private set; }
    public int MaxHealth => maxHealth;
    public event Action Died;
    private PlayerDamageAudio damageAudio;

    private void Awake()
    {
        Health = maxHealth;
        damageAudio = GetComponent<PlayerDamageAudio>();
    }
    public void SetRespawnPoint(Transform point) { if (point != null) respawnPoint = point; }

    public bool TakeDamage(int amount)
    {
        if (amount <= 0 || Health <= 0) return false;
        Health = Mathf.Max(0, Health - amount);
        if (damageAudio != null) damageAudio.PlayDamage();
        if (damageCamera != null) damageCamera.ShakeOnDamage();
        if (Health > 0) return true;
        Died?.Invoke();
        if (targetManager != null) targetManager.ClearTarget();
        var pause = FindFirstObjectByType<PauseMenuPrototype>();
        if (pause == null || !PawSceneTransition.BeginRespawn(Respawn, pause.TransitionTheme, pause.TransitionFont))
            Respawn();
        return true;
    }

    private void Respawn()
    {
        if (this == null) return;
        CharacterController controller = GetComponent<CharacterController>();
        controller.enabled = false;
        if (respawnPoint != null) transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
        controller.enabled = true;
        Health = maxHealth;
    }

    private void OnGUI()
    {
        if (PawSceneTransition.IsActive) return;
        if (GetComponentInParent<PlayerHudPrototype>() == null && FindFirstObjectByType<PlayerHudPrototype>() == null)
            GUI.Box(new Rect(Screen.width - 175, 15, 160, 30), "Vida: " + Health + " / " + maxHealth);
    }
}
