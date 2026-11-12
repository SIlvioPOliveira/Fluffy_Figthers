using UnityEngine;

// Temporary world-space health display for the test scene.
[RequireComponent(typeof(EnemyHealth))]
public sealed class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private string displayName = "Inimigo";
    [SerializeField] private float heightOffset = 1.7f;
    [SerializeField, Min(0.1f)] private float fillSpeed = 2f;
    private EnemyHealth health;
    private float displayedFraction = 1f;
    private GUIStyle titleStyle;
    private GUIStyle valueStyle;

    private void Awake() => health = GetComponent<EnemyHealth>();

    private void Update()
    {
        if (!health.IsAlive) { displayedFraction = 0f; return; }
        float actual = health.MaxHealth > 0 ? Mathf.Clamp01((float)health.Health / health.MaxHealth) : 0f;
        // Respawn refills immediately; damage drains smoothly toward the real value.
        if (actual > displayedFraction) displayedFraction = actual;
        else displayedFraction = Mathf.MoveTowards(displayedFraction, actual, fillSpeed * Time.deltaTime);
    }

    private void OnGUI()
    {
        if (!health.IsAlive || !health.IsSelected || Camera.main == null || InventoryCanvasPrototype.IsVisible || GachaCartoonUI.IsVisible || BarrierCartoonUI.IsVisible || VictoryCartoonUI.IsVisible) return;
        Vector3 point = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * heightOffset);
        if (point.z <= 0f) return;
        if (titleStyle == null)
            titleStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 14 };
        if (valueStyle == null)
            valueStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 12 };

        const float width = 150f;
        float x = point.x - width * 0.5f;
        float y = Screen.height - point.y - 48f;
        Color previous = GUI.color;
        GUI.color = Color.white;
        GUI.Label(new Rect(x, y, width, 22f), displayName, titleStyle);
        var outer = new Rect(x, y + 23f, width, 20f);
        GUI.color = new Color(0.06f, 0.07f, 0.09f, 0.95f);
        GUI.DrawTexture(outer, Texture2D.whiteTexture);
        var inner = new Rect(outer.x + 2f, outer.y + 2f, (outer.width - 4f) * Mathf.Clamp01(displayedFraction), outer.height - 4f);
        GUI.color = Color.Lerp(new Color(0.9f, 0.2f, 0.15f), new Color(0.25f, 0.85f, 0.35f), displayedFraction);
        GUI.DrawTexture(inner, Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(outer, health.Health + " / " + health.MaxHealth, valueStyle);
        GUI.color = previous;
    }
}
