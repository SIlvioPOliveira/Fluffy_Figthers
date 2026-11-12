using System;
using UnityEngine;
using UnityEngine.UIElements;

// Shared visual presentation for the main and pause menus; actions stay in their controllers.
public sealed class MenuCartoonView : IDisposable
{
    private readonly GameObject documentObject;
    private readonly PanelSettings settings;
    private readonly Label feedback;
    private static readonly Color Blue = new Color(.04f,.46f,.61f);
    private static readonly Color Gold = new Color(1f,.78f,.30f);
    private static readonly Color Ink = new Color(.12f,.23f,.29f);

    private static VisualElement Panel(Color color, float radius)
    {
        var element = new VisualElement(); element.style.backgroundColor = color;
        element.style.borderTopLeftRadius = radius; element.style.borderTopRightRadius = radius;
        element.style.borderBottomLeftRadius = radius; element.style.borderBottomRightRadius = radius;
        return element;
    }
    private static void Border(VisualElement element, Color color, float width)
    {
        element.style.borderTopWidth = width; element.style.borderBottomWidth = width;
        element.style.borderLeftWidth = width; element.style.borderRightWidth = width;
        element.style.borderTopColor = color; element.style.borderBottomColor = color;
        element.style.borderLeftColor = color; element.style.borderRightColor = color;
    }
    private static Label Text(string value, int size, Color color)
    {
        var label = new Label(value); label.style.fontSize = size; label.style.color = color;
        label.style.unityFontStyleAndWeight = FontStyle.Bold; label.style.unityTextAlign = TextAnchor.MiddleCenter;
        label.style.whiteSpace = WhiteSpace.Normal; label.pickingMode = PickingMode.Ignore; return label;
    }
    private static Button MakeButton(string label, Color color, Action action)
    {
        var button = new Button(action) { text = label }; button.style.backgroundColor = color;
        button.style.color = Color.white; button.style.fontSize = 23; button.style.unityFontStyleAndWeight = FontStyle.Bold;
        button.style.height = 58; button.style.flexShrink = 0; button.style.marginTop = 8;
        button.style.borderTopLeftRadius = 15; button.style.borderTopRightRadius = 15;
        button.style.borderBottomLeftRadius = 15; button.style.borderBottomRightRadius = 15;
        Border(button, Color.Lerp(color, Color.black, .32f), 2); button.style.borderBottomWidth = 5;
        button.RegisterCallback<PointerEnterEvent>(_ => button.style.backgroundColor = Color.Lerp(color, Color.white, .18f));
        button.RegisterCallback<PointerLeaveEvent>(_ => button.style.backgroundColor = color);
        return button;
    }

    public MenuCartoonView(Transform owner, ThemeStyleSheet theme, Font font, Texture2D logo, bool paused,
        Action primary, Action secondary, Action exit = null)
    {
        settings = ScriptableObject.CreateInstance<PanelSettings>(); settings.themeStyleSheet = theme;
        settings.scaleMode = PanelScaleMode.ScaleWithScreenSize; settings.referenceResolution = new Vector2Int(1600,900);
        documentObject = new GameObject(paused ? "MenuPausaCartoon" : "MenuInicialCartoon");
        documentObject.transform.SetParent(owner, false);
        var document = documentObject.AddComponent<UIDocument>(); document.panelSettings = settings;
        var root = document.rootVisualElement; if (font != null) root.style.unityFontDefinition = FontDefinition.FromFont(font);
        root.style.flexGrow = 1; root.style.alignItems = Align.Center; root.style.justifyContent = Justify.Center;
        root.style.backgroundColor = paused ? new Color(.02f,.09f,.13f,.78f) : Color.black;
        if (!paused)
        {
            var image = new Image { image = logo, scaleMode = ScaleMode.ScaleToFit, pickingMode = PickingMode.Ignore };
            image.style.width = Length.Percent(75); image.style.maxWidth = 1050;
            image.style.height = 380; image.style.maxHeight = Length.Percent(48); image.style.flexShrink = 1;
            image.style.marginBottom = 16; root.Add(image);
        }
        var frame = Panel(new Color(.06f,.53f,.63f), 24); Border(frame, new Color(.02f,.30f,.39f), 5);
        frame.style.width = Length.Percent(paused ? 48 : 45); frame.style.maxWidth = paused ? 620 : 650;
        frame.style.maxHeight = Length.Percent(94); frame.style.paddingLeft = 18; frame.style.paddingRight = 18;
        frame.style.paddingTop = 14; frame.style.paddingBottom = 14; root.Add(frame);
        var inside = Panel(new Color(.98f,.95f,.86f), 18); Border(inside, Gold, 2);
        inside.style.alignItems = Align.Center; inside.style.paddingLeft = 20; inside.style.paddingRight = 20;
        inside.style.paddingTop = 10; inside.style.paddingBottom = 14; frame.Add(inside);
        if (paused)
        {
            var heading = Text("JOGO PAUSADO", 26, Ink);
            heading.style.marginBottom = 6; inside.Add(heading);
        }
        var buttons = new VisualElement(); buttons.style.width = Length.Percent(75); buttons.style.maxWidth = 470; inside.Add(buttons);
        buttons.Add(MakeButton(paused ? "CONTINUAR  [ESC]" : "JOGAR", new Color(.22f,.65f,.27f), primary));
        buttons.Add(MakeButton(paused ? "SALVAR E VOLTAR AO MENU" : "SAIR", Blue, secondary));
        if (paused && exit != null) buttons.Add(MakeButton("SALVAR E SAIR", new Color(.64f,.29f,.32f), exit));
        feedback = Text("", 16, new Color(.65f,.18f,.18f)); feedback.style.marginTop = 6; inside.Add(feedback);
    }

    public void SetMessage(string message) => feedback.text = message ?? "";
    public void Dispose()
    {
        if (documentObject != null) { documentObject.SetActive(false); UnityEngine.Object.Destroy(documentObject); }
        if (settings != null) UnityEngine.Object.Destroy(settings);
    }
}
