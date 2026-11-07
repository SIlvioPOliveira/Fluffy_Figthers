using System;
using UnityEngine;

// Temporary player health and respawn for the isolated boss test scene.
[RequireComponent(typeof(CharacterController))]
public sealed class PlayerHealthTest : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 3;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private CombatTargetTest targetManager;
    public int Health { get; private set; }
    public int MaxHealth => maxHealth;
    public event Action Died;

    private void Awake() => Health = maxHealth;

    public bool TakeDamage(int amount)
    {
        if (amount <= 0 || Health <= 0) return false;
        Health = Mathf.Max(0, Health - amount);
        if (Health > 0) return true;
        Died?.Invoke();
        if (targetManager != null) targetManager.ClearTarget();
        CharacterController controller = GetComponent<CharacterController>();
        controller.enabled = false;
        if (respawnPoint != null) transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
        controller.enabled = true;
        Health = maxHealth;
        return true;
    }

    private void OnGUI()
    {
        if (GetComponentInParent<PlayerHudPrototype>() == null && FindFirstObjectByType<PlayerHudPrototype>() == null)
            GUI.Box(new Rect(Screen.width - 175, 15, 160, 30), "Vida: " + Health + " / " + maxHealth);
    }
}
