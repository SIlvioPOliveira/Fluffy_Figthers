using UnityEngine;
using UnityEngine.UIElements;

// Small vector decorations stay sharp at different interface scales.
public sealed class InventoryIcon : VisualElement
{
    public enum Shape { Paw, Sparkle, Sword, Coin, Heart }
    private readonly Shape shape;
    private readonly Color tint;

    public InventoryIcon(Shape shape, Color tint, float size)
    {
        this.shape = shape; this.tint = tint;
        style.width = size; style.height = size; style.flexShrink = 0;
        pickingMode = PickingMode.Ignore;
        generateVisualContent += Draw;
    }

    private static void Circle(Painter2D p, float x, float y, float radius)
    {
        p.BeginPath(); p.Arc(new Vector2(x, y), radius, Angle.Degrees(0), Angle.Degrees(360)); p.ClosePath(); p.Fill();
    }

    private void Draw(MeshGenerationContext context)
    {
        float size = Mathf.Min(contentRect.width, contentRect.height);
        if (size <= 0) return;
        var p = context.painter2D; p.fillColor = tint;
        if (shape == Shape.Paw)
        {
            Circle(p, size * .18f, size * .38f, size * .115f);
            Circle(p, size * .38f, size * .19f, size * .12f);
            Circle(p, size * .62f, size * .19f, size * .12f);
            Circle(p, size * .82f, size * .38f, size * .115f);
            p.BeginPath(); p.MoveTo(new Vector2(size * .18f, size * .75f));
            p.BezierCurveTo(new Vector2(size * .18f, size * .57f), new Vector2(size * .33f, size * .42f), new Vector2(size * .5f, size * .43f));
            p.BezierCurveTo(new Vector2(size * .67f, size * .42f), new Vector2(size * .82f, size * .57f), new Vector2(size * .82f, size * .75f));
            p.BezierCurveTo(new Vector2(size * .82f, size * .94f), new Vector2(size * .64f, size * .88f), new Vector2(size * .5f, size * .84f));
            p.BezierCurveTo(new Vector2(size * .36f, size * .88f), new Vector2(size * .18f, size * .94f), new Vector2(size * .18f, size * .75f));
            p.ClosePath(); p.Fill();
        }
        else if (shape == Shape.Sparkle)
        {
            p.BeginPath(); p.MoveTo(new Vector2(size * .5f, 0)); p.LineTo(new Vector2(size * .62f, size * .37f));
            p.LineTo(new Vector2(size, size * .5f)); p.LineTo(new Vector2(size * .62f, size * .63f));
            p.LineTo(new Vector2(size * .5f, size)); p.LineTo(new Vector2(size * .38f, size * .63f));
            p.LineTo(new Vector2(0, size * .5f)); p.LineTo(new Vector2(size * .38f, size * .37f)); p.ClosePath(); p.Fill();
        }
        else if (shape == Shape.Coin)
        {
            Circle(p, size * .5f, size * .5f, size * .46f);
            p.fillColor = new Color(1f, .90f, .47f); Circle(p, size * .5f, size * .5f, size * .34f);
            p.fillColor = new Color(.90f, .61f, .14f); Circle(p, size * .5f, size * .5f, size * .21f);
        }
        else if (shape == Shape.Heart)
        {
            p.BeginPath(); p.MoveTo(new Vector2(size * .5f, size * .90f));
            p.BezierCurveTo(new Vector2(size * .38f, size * .79f), new Vector2(size * .05f, size * .57f), new Vector2(size * .05f, size * .32f));
            p.BezierCurveTo(new Vector2(size * .05f, size * .05f), new Vector2(size * .37f, size * .01f), new Vector2(size * .5f, size * .24f));
            p.BezierCurveTo(new Vector2(size * .63f, size * .01f), new Vector2(size * .95f, size * .05f), new Vector2(size * .95f, size * .32f));
            p.BezierCurveTo(new Vector2(size * .95f, size * .57f), new Vector2(size * .62f, size * .79f), new Vector2(size * .5f, size * .90f));
            p.ClosePath(); p.Fill();
        }
        else
        {
            // Contrasting blade, guard and grip keep the sword recognizable at HUD size.
            p.strokeColor = new Color(.20f, .26f, .31f); p.lineWidth = Mathf.Max(1f, size * .055f);
            p.lineJoin = LineJoin.Round;
            p.fillColor = new Color(.86f, .93f, .96f);
            p.BeginPath(); p.MoveTo(new Vector2(size * .33f, size * .55f));
            p.LineTo(new Vector2(size * .76f, size * .10f)); p.LineTo(new Vector2(size * .92f, size * .08f));
            p.LineTo(new Vector2(size * .90f, size * .24f)); p.LineTo(new Vector2(size * .45f, size * .67f));
            p.ClosePath(); p.Fill(); p.Stroke();
            p.strokeColor = new Color(.59f, .72f, .78f); p.lineWidth = Mathf.Max(.7f, size * .035f);
            p.BeginPath(); p.MoveTo(new Vector2(size * .40f, size * .60f)); p.LineTo(new Vector2(size * .85f, size * .15f)); p.Stroke();
            p.strokeColor = new Color(.20f, .26f, .31f); p.lineWidth = Mathf.Max(1f, size * .055f);
            p.fillColor = new Color(.48f, .27f, .14f);
            p.BeginPath(); p.MoveTo(new Vector2(size * .27f, size * .65f)); p.LineTo(new Vector2(size * .35f, size * .73f));
            p.LineTo(new Vector2(size * .18f, size * .90f)); p.LineTo(new Vector2(size * .10f, size * .82f)); p.ClosePath(); p.Fill(); p.Stroke();
            p.fillColor = new Color(1f, .75f, .25f);
            p.BeginPath(); p.MoveTo(new Vector2(size * .20f, size * .42f)); p.LineTo(new Vector2(size * .25f, size * .37f));
            p.LineTo(new Vector2(size * .63f, size * .75f)); p.LineTo(new Vector2(size * .58f, size * .80f)); p.ClosePath(); p.Fill(); p.Stroke();
            Circle(p, size * .12f, size * .88f, size * .065f);
        }
    }
}
