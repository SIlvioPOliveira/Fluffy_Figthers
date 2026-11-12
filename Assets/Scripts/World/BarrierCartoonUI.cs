using System;
using UnityEngine;
using UnityEngine.UIElements;

// Visual only: purchase, persistence and collision remain with the barrier.
public sealed class BarrierCartoonUI : IDisposable
{
    private readonly ZoneBarrierTest barrier;
    private readonly GameObject documentObject;
    private readonly PanelSettings settings;
    private readonly Label balance, feedback;
    private static readonly Color Blue = new Color(.04f,.46f,.61f);
    private static readonly Color Gold = new Color(1f,.78f,.30f);
    private static readonly Color Ink = new Color(.12f,.23f,.29f);
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
    private static Label Text(string value, int size, Color color, bool bold = false)
    {
        var l = new Label(value); l.style.fontSize = size; l.style.color = color; l.style.whiteSpace = WhiteSpace.Normal;
        l.style.unityFontStyleAndWeight = bold ? FontStyle.Bold : FontStyle.Normal;
        l.style.unityTextAlign = TextAnchor.MiddleCenter; l.pickingMode = PickingMode.Ignore; return l;
    }
    private static Button MakeButton(string text, Color color, Action action)
    {
        var b = new Button(action) { text = text }; b.style.backgroundColor = color; b.style.color = Color.white;
        b.style.fontSize = 22; b.style.unityFontStyleAndWeight = FontStyle.Bold;
        b.style.height = 55; b.style.flexGrow = 1; b.style.flexBasis = 0; b.style.marginLeft = 5; b.style.marginRight = 5;
        b.style.borderTopLeftRadius = 12; b.style.borderTopRightRadius = 12; b.style.borderBottomLeftRadius = 12; b.style.borderBottomRightRadius = 12;
        Border(b, Color.Lerp(color, Color.black, .3f), 2); b.style.borderBottomWidth = 5;
        b.RegisterCallback<PointerEnterEvent>(_ => b.style.backgroundColor = Color.Lerp(color, Color.white, .18f));
        b.RegisterCallback<PointerLeaveEvent>(_ => b.style.backgroundColor = color); return b;
    }
    public BarrierCartoonUI(ZoneBarrierTest barrier, ThemeStyleSheet theme, Font font)
    {
        this.barrier = barrier; IsVisible = true;
        settings = ScriptableObject.CreateInstance<PanelSettings>(); settings.themeStyleSheet = theme;
        settings.scaleMode = PanelScaleMode.ScaleWithScreenSize; settings.referenceResolution = new Vector2Int(1600,900);
        documentObject = new GameObject("BarreiraCartoonUI"); documentObject.transform.SetParent(barrier.transform, false);
        var doc = documentObject.AddComponent<UIDocument>(); doc.panelSettings = settings;
        var root = doc.rootVisualElement; if (font != null) root.style.unityFontDefinition = FontDefinition.FromFont(font);
        root.style.flexGrow = 1; root.style.alignItems = Align.Center; root.style.justifyContent = Justify.Center;
        root.style.backgroundColor = new Color(0,0,0,.68f);
        var frame = Panel(new Color(.06f,.53f,.63f), 24); Border(frame, new Color(.02f,.30f,.39f), 5);
        frame.style.width = Length.Percent(58); frame.style.maxWidth = 700;
        frame.style.paddingLeft = 14; frame.style.paddingRight = 14; frame.style.paddingTop = 14; frame.style.paddingBottom = 14; root.Add(frame);
        var header = Panel(Blue, 14); Border(header, Gold, 2); header.style.paddingTop = 8; header.style.paddingBottom = 8; frame.Add(header);
        header.Add(Text("DESBLOQUEAR REGIÃO", 25, Color.white, true));
        header.Add(Text(barrier.Destination.ToUpperInvariant(), 32, Gold, true));
        var body = Panel(new Color(.98f,.95f,.86f), 16); Border(body, new Color(.88f,.78f,.56f), 2);
        body.style.marginTop = 8; body.style.paddingLeft = 20; body.style.paddingRight = 20;
        body.style.paddingTop = 12; body.style.paddingBottom = 12; frame.Add(body);
        body.Add(Text("Abrir esta passagem permanentemente?", 22, Ink, true));
        var cost = new VisualElement(); cost.style.flexDirection = FlexDirection.Row; cost.style.justifyContent = Justify.Center;
        cost.style.alignItems = Align.Center; cost.style.marginTop = 12; body.Add(cost);
        cost.Add(new InventoryIcon(InventoryIcon.Shape.Coin, new Color(.93f,.66f,.17f), 36));
        var price = Text("  Custo: " + barrier.UnlockCost.ToString("N0"), 24, Ink, true); cost.Add(price);
        var balanceRow = new VisualElement(); balanceRow.style.flexDirection = FlexDirection.Row; balanceRow.style.justifyContent = Justify.Center; balanceRow.style.alignItems = Align.Center; balanceRow.style.marginTop = 5; body.Add(balanceRow);
        balance = Text("", 19, Ink); balanceRow.Add(balance);
        balanceRow.Add(new InventoryIcon(InventoryIcon.Shape.Coin, Color.white, 27));
        feedback = Text("", 18, new Color(.70f,.16f,.18f), true); feedback.style.minHeight = 28; feedback.style.marginTop = 4; body.Add(feedback);
        var buttons = new VisualElement(); buttons.style.flexDirection = FlexDirection.Row; buttons.style.marginTop = 5; body.Add(buttons);
        buttons.Add(MakeButton("CONFIRMAR", new Color(.22f,.65f,.27f), barrier.ConfirmFromUI));
        buttons.Add(MakeButton("CANCELAR  [ESC]", Blue, barrier.CancelFromUI));
        Tick();
    }
    public void Tick()
    {
        balance.text = "Saldo: " + barrier.Balance.ToString("N0");
        feedback.text = barrier.Feedback;
    }
    public void Dispose()
    {
        IsVisible = false;
        if (documentObject != null) { documentObject.SetActive(false); UnityEngine.Object.Destroy(documentObject); }
        if (settings != null) UnityEngine.Object.Destroy(settings);
    }
}
