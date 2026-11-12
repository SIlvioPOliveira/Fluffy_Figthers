using System;
using UnityEngine;
using UnityEngine.UIElements;

// Presentation for the first victory; progression and persistence stay in the driver.
public sealed class VictoryCartoonUI : IDisposable
{
    private readonly GameObject documentObject;
    private readonly PanelSettings settings;
    private readonly VisualElement sparkles;
    public static bool IsVisible { get; private set; }

    private static VisualElement Panel(Color color, float radius)
    {
        var e = new VisualElement(); e.style.backgroundColor = color;
        e.style.borderTopLeftRadius = radius; e.style.borderTopRightRadius = radius;
        e.style.borderBottomLeftRadius = radius; e.style.borderBottomRightRadius = radius; return e;
    }
    private static void Border(VisualElement e, Color color, float width)
    {
        e.style.borderTopWidth = width; e.style.borderBottomWidth = width; e.style.borderLeftWidth = width; e.style.borderRightWidth = width;
        e.style.borderTopColor = color; e.style.borderBottomColor = color; e.style.borderLeftColor = color; e.style.borderRightColor = color;
    }
    private static Label Text(string value, int size, Color color)
    {
        var label = new Label(value); label.style.fontSize = size; label.style.color = color;
        label.style.unityFontStyleAndWeight = FontStyle.Bold; label.style.unityTextAlign = TextAnchor.MiddleCenter;
        label.style.whiteSpace = WhiteSpace.Normal; label.pickingMode = PickingMode.Ignore; return label;
    }
    public VictoryCartoonUI(Transform owner, ThemeStyleSheet theme, Font font, Action continueAction)
    {
        IsVisible = true;
        settings = ScriptableObject.CreateInstance<PanelSettings>(); settings.themeStyleSheet = theme;
        settings.scaleMode = PanelScaleMode.ScaleWithScreenSize; settings.referenceResolution = new Vector2Int(1600, 900);
        documentObject = new GameObject("VitoriaCartoonUI"); documentObject.transform.SetParent(owner, false);
        var document = documentObject.AddComponent<UIDocument>(); document.panelSettings = settings;
        var root = document.rootVisualElement; if (font != null) root.style.unityFontDefinition = FontDefinition.FromFont(font);
        root.style.flexGrow = 1; root.style.alignItems = Align.Center; root.style.justifyContent = Justify.Center;
        root.style.backgroundColor = new Color(.02f,.09f,.13f,.78f);
        var frame = Panel(new Color(.06f,.53f,.63f), 24); Border(frame, new Color(.02f,.30f,.39f), 5);
        frame.style.width = Length.Percent(65); frame.style.maxWidth = 860;
        frame.style.paddingLeft = 16; frame.style.paddingRight = 16; frame.style.paddingTop = 16; frame.style.paddingBottom = 16; root.Add(frame);
        var inside = Panel(new Color(.98f,.95f,.86f), 18); Border(inside, new Color(1f,.78f,.30f), 3);
        inside.style.alignItems = Align.Center; inside.style.paddingLeft = 24; inside.style.paddingRight = 24;
        inside.style.paddingTop = 22; inside.style.paddingBottom = 22; frame.Add(inside);
        sparkles = new VisualElement { pickingMode = PickingMode.Ignore }; sparkles.style.flexDirection = FlexDirection.Row;
        sparkles.style.alignItems = Align.Center; sparkles.style.justifyContent = Justify.Center; inside.Add(sparkles);
        var left = new InventoryIcon(InventoryIcon.Shape.Sparkle, new Color(1f,.73f,.19f), 35); left.style.marginRight = 20; sparkles.Add(left);
        sparkles.Add(new InventoryIcon(InventoryIcon.Shape.Paw, new Color(.05f,.52f,.61f), 65));
        var right = new InventoryIcon(InventoryIcon.Shape.Sparkle, new Color(1f,.73f,.19f), 35); right.style.marginLeft = 20; sparkles.Add(right);
        var heading = Text("PARABÉNS!", 40, new Color(.04f,.46f,.61f)); heading.style.marginTop = 10; inside.Add(heading);
        var message = Text("Você conquistou todas as regiões de Fluffy Fighters!", 25, new Color(.12f,.23f,.29f)); message.style.marginTop = 8; inside.Add(message);
        var subtitle = Text("Continue explorando e fortalecendo sua equipe.", 19, new Color(.25f,.39f,.42f)); subtitle.style.marginTop = 8; inside.Add(subtitle);
        var button = new Button(continueAction) { text = "CONTINUAR JOGANDO" };
        button.style.width = Length.Percent(76); button.style.maxWidth = 440; button.style.height = 60; button.style.marginTop = 20;
        button.style.backgroundColor = new Color(.22f,.65f,.27f); button.style.color = Color.white;
        button.style.fontSize = 22; button.style.unityFontStyleAndWeight = FontStyle.Bold;
        button.style.borderTopLeftRadius = 14; button.style.borderTopRightRadius = 14;
        button.style.borderBottomLeftRadius = 14; button.style.borderBottomRightRadius = 14;
        Border(button, new Color(.12f,.42f,.16f), 2); button.style.borderBottomWidth = 5;
        button.RegisterCallback<PointerEnterEvent>(_ => button.style.backgroundColor = new Color(.35f,.75f,.38f));
        button.RegisterCallback<PointerLeaveEvent>(_ => button.style.backgroundColor = new Color(.22f,.65f,.27f)); inside.Add(button);
    }
    public void Tick()
    {
        float glow = .75f + .25f * Mathf.Sin(Time.unscaledTime * 2.2f);
        sparkles.style.opacity = glow;
    }
    public void Dispose()
    {
        IsVisible = false;
        if (documentObject != null) { documentObject.SetActive(false); UnityEngine.Object.Destroy(documentObject); }
        if (settings != null) UnityEngine.Object.Destroy(settings);
    }
}
