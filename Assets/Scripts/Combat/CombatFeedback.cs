using System.Collections.Generic;
using UnityEngine;

// Generic, temporary feedback independent of pet and enemy models.
[RequireComponent(typeof(EnemyHealth))]
public sealed class CombatFeedback : MonoBehaviour
{
    [SerializeField, Min(0.05f)] private float numberDuration = 0.7f;
    [SerializeField, Min(0.01f)] private float flashDuration = 0.12f;
    [SerializeField] private Renderer flashRenderer;

    private struct FloatingNumber
    {
        public int Damage;
        public Vector3 Origin;
        public Color Color;
        public float Started;
        public float Horizontal;
    }

    private readonly List<FloatingNumber> numbers = new List<FloatingNumber>();
    private EnemyHealth health;
    private MaterialPropertyBlock originalBlock;
    private MaterialPropertyBlock flashBlock;
    private float flashUntil;
    private bool flashing;
    private int sequence;
    private GUIStyle numberStyle;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        if (flashRenderer == null) flashRenderer = GetComponent<Renderer>();
        originalBlock = new MaterialPropertyBlock();
        flashBlock = new MaterialPropertyBlock();
    }

    private void OnEnable() => health.Damaged += OnDamaged;
    private void OnDisable()
    {
        health.Damaged -= OnDamaged;
        RestoreFlash();
        numbers.Clear();
    }

    private void OnDamaged(int damage, PetRarity? rarity)
    {
        numbers.Add(new FloatingNumber
        {
            Damage = damage,
            Origin = transform.position + Vector3.up * 1.35f,
            Color = RarityColor(rarity),
            Started = Time.time,
            Horizontal = ((sequence++ % 5) - 2) * 14f
        });
        if (flashRenderer == null || !health.IsAlive) return;
        if (!flashing) flashRenderer.GetPropertyBlock(originalBlock);
        flashRenderer.GetPropertyBlock(flashBlock);
        flashBlock.SetColor("_Color", Color.white);
        flashBlock.SetColor("_BaseColor", Color.white);
        flashRenderer.SetPropertyBlock(flashBlock);
        flashing = true;
        flashUntil = Time.time + flashDuration;
    }

    private void Update()
    {
        if (flashing && (Time.time >= flashUntil || !health.IsAlive)) RestoreFlash();
        for (int i = numbers.Count - 1; i >= 0; i--)
            if (Time.time - numbers[i].Started >= numberDuration) numbers.RemoveAt(i);
    }

    private void RestoreFlash()
    {
        if (flashing && flashRenderer != null) flashRenderer.SetPropertyBlock(originalBlock);
        flashing = false;
    }

    private void OnGUI()
    {
        Camera camera = Camera.main;
        if (camera == null || numbers.Count == 0) return;
        if (numberStyle == null)
            numberStyle = new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        Color previous = GUI.color;
        foreach (FloatingNumber number in numbers)
        {
            float progress = Mathf.Clamp01((Time.time - number.Started) / numberDuration);
            Vector3 screen = camera.WorldToScreenPoint(number.Origin + Vector3.up * progress * 0.8f);
            if (screen.z <= 0) continue;
            var rect = new Rect(screen.x - 50 + number.Horizontal, Screen.height - screen.y - 30, 100, 40);
            Color shadow = new Color(0f, 0f, 0f, 1f - progress);
            GUI.color = shadow;
            GUI.Label(new Rect(rect.x + 2, rect.y + 2, rect.width, rect.height), number.Damage.ToString(), numberStyle);
            Color color = number.Color;
            color.a = 1f - progress;
            GUI.color = color;
            GUI.Label(rect, number.Damage.ToString(), numberStyle);
        }
        GUI.color = previous;
    }

    private static Color RarityColor(PetRarity? rarity)
    {
        switch (rarity)
        {
            case PetRarity.Rare: return new Color(0.4f, 0.7f, 1f);
            case PetRarity.Epic: return new Color(0.8f, 0.5f, 1f);
            case PetRarity.Legendary: return new Color(1f, 0.8f, 0.25f);
            case PetRarity.Secret: return new Color(1f, 0.5f, 0.7f);
            default: return Color.white;
        }
    }
}
