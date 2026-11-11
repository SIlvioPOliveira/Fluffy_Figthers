using UnityEngine;
using UnityEngine.UIElements;

// Decorative vector artwork sits behind the actual rotating pet preview.
public sealed class InventoryPedestal : VisualElement
{
    public InventoryPedestal()
    {
        pickingMode = PickingMode.Ignore;
        style.position = Position.Absolute;
        style.left = 0; style.right = 0; style.top = 0; style.bottom = 0;
        generateVisualContent += Draw;
    }

    private static void Oval(Painter2D p, float cx, float cy, float rx, float ry, Color color)
    {
        const float k = .55228475f;
        p.fillColor = color;
        p.BeginPath(); p.MoveTo(new Vector2(cx + rx, cy));
        p.BezierCurveTo(new Vector2(cx + rx, cy + ry * k), new Vector2(cx + rx * k, cy + ry), new Vector2(cx, cy + ry));
        p.BezierCurveTo(new Vector2(cx - rx * k, cy + ry), new Vector2(cx - rx, cy + ry * k), new Vector2(cx - rx, cy));
        p.BezierCurveTo(new Vector2(cx - rx, cy - ry * k), new Vector2(cx - rx * k, cy - ry), new Vector2(cx, cy - ry));
        p.BezierCurveTo(new Vector2(cx + rx * k, cy - ry), new Vector2(cx + rx, cy - ry * k), new Vector2(cx + rx, cy));
        p.ClosePath(); p.Fill();
    }

    private void Draw(MeshGenerationContext context)
    {
        float w = contentRect.width, h = contentRect.height;
        if (w <= 0 || h <= 0) return;
        var p = context.painter2D;
        float cx = w * .5f, cy = h * .86f;
        float rx = Mathf.Min(w * .32f, 130f), ry = Mathf.Min(h * .07f, 18f);
        Oval(p, cx, cy + ry * .65f, rx * 1.08f, ry * 1.25f, new Color(.20f, .47f, .50f, .16f));
        Oval(p, cx, cy + ry * .32f, rx, ry, new Color(.20f, .52f, .59f, .85f));
        Oval(p, cx, cy, rx, ry, new Color(.34f, .75f, .78f, .95f));
        Oval(p, cx, cy - ry * .05f, rx * .83f, ry * .70f, new Color(.73f, .96f, .94f, .95f));
        Oval(p, cx, cy - ry * .05f, rx * .62f, ry * .47f, new Color(.34f, .78f, .80f, .72f));
    }
}
