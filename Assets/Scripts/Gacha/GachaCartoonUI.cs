using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Presentation only: the machine continues to own the roll, reward and reel timing.
public sealed class GachaCartoonUI : IDisposable
{
    private readonly GachaMachineTest machine;
    private readonly GameObject documentObject;
    private readonly PanelSettings settings;
    private readonly List<Tuple<Image, PetData>> poolImages = new List<Tuple<Image, PetData>>();
    private readonly List<Tuple<VisualElement, Image, PetData>> reelCards = new List<Tuple<VisualElement, Image, PetData>>();
    private readonly Label balance, cost, feedback, resultName, resultRarity, status;
    private readonly VisualElement viewport, reelTrack, resultPanel, idleHint;
    private readonly Image resultImage;
    private readonly RarityRays resultRays;
    private readonly Button roll, continueButton;
    private PetData lastReward;
    private const float CardWidth = 152f;
    private static readonly Color Cream = new Color(.98f, .95f, .86f);
    private static readonly Color Ink = new Color(.12f, .23f, .29f);
    private static readonly Color Blue = new Color(.04f, .46f, .61f);
    private static readonly Color Gold = new Color(1f, .78f, .30f);
    public static bool IsVisible { get; private set; }

    // Drawn behind the pet in the result panel; it never affects the roll itself.
    private sealed class RarityRays : VisualElement
    {
        public Color RayColor;

        public RarityRays()
        {
            pickingMode = PickingMode.Ignore;
            generateVisualContent += Draw;
        }

        private void Draw(MeshGenerationContext context)
        {
            float width = contentRect.width, height = contentRect.height;
            if (width <= 0 || height <= 0) return;
            var center = new Vector2(width * .5f, height * .5f);
            float radius = Mathf.Sqrt(width * width + height * height);
            float rotation = Time.unscaledTime * .32f;
            var painter = context.painter2D;
            Color color = RayColor;
            color.a = .35f + .06f * Mathf.Sin(Time.unscaledTime * 2f);
            painter.fillColor = color;
            const int rayCount = 12;
            float step = Mathf.PI * 2f / rayCount;
            for (int i = 0; i < rayCount; i++)
            {
                float angle = rotation + i * step;
                float edge = angle + step * .42f;
                painter.BeginPath();
                painter.MoveTo(center);
                painter.LineTo(center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
                painter.LineTo(center + new Vector2(Mathf.Cos(edge), Mathf.Sin(edge)) * radius);
                painter.ClosePath();
                painter.Fill();
            }
        }
    }

    private static VisualElement Panel(Color color, float radius = 14)
    {
        var e = new VisualElement(); e.style.backgroundColor = color;
        e.style.borderTopLeftRadius = radius; e.style.borderTopRightRadius = radius;
        e.style.borderBottomLeftRadius = radius; e.style.borderBottomRightRadius = radius; return e;
    }
    private static void Border(VisualElement e, Color color, float width)
    {
        e.style.borderTopColor = color; e.style.borderBottomColor = color; e.style.borderLeftColor = color; e.style.borderRightColor = color;
        e.style.borderTopWidth = width; e.style.borderBottomWidth = width; e.style.borderLeftWidth = width; e.style.borderRightWidth = width;
    }
    private static Label Text(string text, int size, Color color, bool bold = false)
    {
        var l = new Label(text); l.style.fontSize = size; l.style.color = color;
        l.style.unityFontStyleAndWeight = bold ? FontStyle.Bold : FontStyle.Normal;
        l.style.whiteSpace = WhiteSpace.Normal; l.pickingMode = PickingMode.Ignore; return l;
    }
    private static Button MakeButton(string label, Color color, Action action)
    {
        var b = new Button(action) { text = label }; b.style.backgroundColor = color; b.style.color = Color.white;
        b.style.fontSize = 21; b.style.unityFontStyleAndWeight = FontStyle.Bold; b.style.minHeight = 48;
        b.style.borderTopLeftRadius = 12; b.style.borderTopRightRadius = 12; b.style.borderBottomLeftRadius = 12; b.style.borderBottomRightRadius = 12;
        Border(b, Color.Lerp(color, Color.black, .3f), 2); b.style.borderBottomWidth = 5;
        b.RegisterCallback<PointerEnterEvent>(_ => { if (b.enabledInHierarchy) b.style.backgroundColor = Color.Lerp(color, Color.white, .18f); });
        b.RegisterCallback<PointerLeaveEvent>(_ => b.style.backgroundColor = color); return b;
    }
    private static Color Rarity(PetRarity r)
    {
        switch (r) { case PetRarity.Rare: return new Color(.30f,.64f,.92f); case PetRarity.Epic: return new Color(.66f,.39f,.80f); case PetRarity.Legendary: return new Color(.91f,.64f,.22f); case PetRarity.Secret: return new Color(.86f,.36f,.53f); default: return new Color(.53f,.65f,.68f); }
    }
    private static string RarityName(PetRarity r)
    {
        switch (r) { case PetRarity.Rare: return "Raro"; case PetRarity.Epic: return "Épico"; case PetRarity.Legendary: return "Lendário"; case PetRarity.Secret: return "Secreto"; default: return "Comum"; }
    }

    public GachaCartoonUI(GachaMachineTest machine, ThemeStyleSheet theme, Font font)
    {
        this.machine = machine; IsVisible = true;
        settings = ScriptableObject.CreateInstance<PanelSettings>(); settings.themeStyleSheet = theme;
        settings.scaleMode = PanelScaleMode.ScaleWithScreenSize; settings.referenceResolution = new Vector2Int(1600, 900);
        documentObject = new GameObject("GachaCartoonUI"); documentObject.transform.SetParent(machine.transform, false);
        var doc = documentObject.AddComponent<UIDocument>(); doc.panelSettings = settings;
        var root = doc.rootVisualElement; if (font != null) root.style.unityFontDefinition = FontDefinition.FromFont(font);
        root.style.flexGrow = 1; root.style.backgroundColor = new Color(0,0,0,.68f);
        root.style.alignItems = Align.Center; root.style.justifyContent = Justify.Center;
        var frame = Panel(new Color(.06f,.53f,.63f), 24); Border(frame, new Color(.02f,.30f,.39f), 5);
        frame.style.width = Length.Percent(92); frame.style.height = Length.Percent(92); frame.style.maxWidth = 1320;
        frame.style.paddingLeft = 12; frame.style.paddingRight = 12; frame.style.paddingTop = 12; frame.style.paddingBottom = 12; root.Add(frame);
        var header = Panel(Blue); Border(header, Gold, 2); header.style.height = 65; header.style.flexDirection = FlexDirection.Row;
        header.style.alignItems = Align.Center; header.style.justifyContent = Justify.SpaceBetween; header.style.paddingLeft = 18; header.style.paddingRight = 10; frame.Add(header);
        var title = new VisualElement(); title.style.flexDirection = FlexDirection.Row; title.style.alignItems = Align.Center;
        title.Add(new InventoryIcon(InventoryIcon.Shape.Sparkle, Gold, 26));
        var titleLabel = Text("GACHA  •  " + machine.Pool.Region.DisplayName, 29, Color.white, true); titleLabel.style.marginLeft = 10; title.Add(titleLabel); header.Add(title);
        var close = MakeButton("Fechar  ×", new Color(.04f,.33f,.46f), machine.CloseFromUI); close.style.width = 145; header.Add(close);
        var body = Panel(Cream, 18); Border(body, new Color(.88f,.78f,.56f), 2); body.style.flexGrow = 1; body.style.minHeight = 0;
        body.style.marginTop = 9; body.style.paddingLeft = 14; body.style.paddingRight = 14; body.style.paddingTop = 8; body.style.paddingBottom = 8; frame.Add(body);
        var currencyRow = new VisualElement(); currencyRow.style.flexDirection = FlexDirection.Row; currencyRow.style.alignItems = Align.Center; body.Add(currencyRow);
        balance = Text("", 20, Ink, true); currencyRow.Add(balance);
        currencyRow.Add(new InventoryIcon(InventoryIcon.Shape.Coin, Color.white, 29));
        var heading = Text("PETS DISPONÍVEIS", 20, Ink, true); body.Add(heading);
        var poolRow = new VisualElement(); poolRow.style.height = 140; poolRow.style.flexDirection = FlexDirection.Row; body.Add(poolRow);
        foreach (GachaEntry entry in machine.Pool.Entries)
        {
            var card = Panel(Rarity(entry.Pet.Rarity)); Border(card, Color.Lerp(Rarity(entry.Pet.Rarity), Ink, .3f), 2);
            card.style.flexGrow = 1; card.style.flexBasis = 0; card.style.marginLeft = 3; card.style.marginRight = 3;
            card.style.alignItems = Align.Center; card.style.paddingTop = 1; poolRow.Add(card);
            var img = new Image(); img.scaleMode = ScaleMode.ScaleToFit; img.style.height = 76; img.style.width = Length.Percent(100); card.Add(img); poolImages.Add(Tuple.Create(img, entry.Pet));
            var name = Text(entry.Pet.PetName, 15, Color.white, true); name.style.unityTextAlign = TextAnchor.MiddleCenter; card.Add(name);
            var chance = Text(RarityName(entry.Pet.Rarity) + "  •  " + (entry.Weight / 100f).ToString("0.##") + "%", 14, Color.white, true); chance.style.unityTextAlign = TextAnchor.MiddleCenter; card.Add(chance);
        }
        status = Text("Pronto para sortear!", 18, Ink, true); status.style.marginTop = 6; status.style.unityTextAlign = TextAnchor.MiddleCenter; body.Add(status);
        viewport = Panel(new Color(.11f,.34f,.39f), 12); Border(viewport, new Color(.08f,.25f,.29f), 3);
        viewport.style.height = 150; viewport.style.overflow = Overflow.Hidden; viewport.style.flexShrink = 0; body.Add(viewport);
        reelTrack = new VisualElement(); reelTrack.style.position = Position.Absolute; reelTrack.style.left = 0; reelTrack.style.right = 0; reelTrack.style.top = 0; reelTrack.style.bottom = 0; viewport.Add(reelTrack);
        idleHint = new VisualElement(); idleHint.style.flexGrow = 1; idleHint.style.alignItems = Align.Center; idleHint.style.justifyContent = Justify.Center;
        idleHint.pickingMode = PickingMode.Ignore;
        idleHint.Add(new InventoryIcon(InventoryIcon.Shape.Paw, Gold, 55));
        var goodLuck = Text("BOA SORTE!", 25, Color.white, true); goodLuck.style.unityTextAlign = TextAnchor.MiddleCenter; idleHint.Add(goodLuck); viewport.Add(idleHint);
        var marker = Text("▼", 25, Gold, true); var symbolFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (symbolFont != null) marker.style.unityFontDefinition = FontDefinition.FromFont(symbolFont);
        marker.style.position = Position.Absolute; marker.style.left = Length.Percent(50); marker.style.marginLeft = -10; marker.style.top = -4; viewport.Add(marker);
        resultPanel = Panel(new Color(.80f,.93f,.93f), 15); Border(resultPanel, new Color(.65f,.83f,.82f), 2);
        resultPanel.style.flexGrow = 1; resultPanel.style.minHeight = 90; resultPanel.style.marginTop = 8; resultPanel.style.flexDirection = FlexDirection.Row; resultPanel.style.alignItems = Align.Center; body.Add(resultPanel);
        var resultVisual = new VisualElement(); resultVisual.style.width = 175; resultVisual.style.height = Length.Percent(100);
        resultVisual.style.flexShrink = 0; resultVisual.style.overflow = Overflow.Hidden; resultPanel.Add(resultVisual);
        resultRays = new RarityRays(); resultRays.style.position = Position.Absolute;
        resultRays.style.left = 0; resultRays.style.right = 0; resultRays.style.top = 0; resultRays.style.bottom = 0; resultVisual.Add(resultRays);
        resultImage = new Image(); resultImage.scaleMode = ScaleMode.ScaleToFit; resultImage.style.width = Length.Percent(100); resultImage.style.height = Length.Percent(100);
        resultImage.pickingMode = PickingMode.Ignore; resultVisual.Add(resultImage);
        var resultText = new VisualElement(); resultText.style.flexGrow = 1; resultPanel.Add(resultText);
        resultText.Add(Text("VOCÊ RECEBEU", 20, Ink, true)); resultName = Text("", 27, Ink, true); resultText.Add(resultName);
        resultRarity = Text("", 20, Ink, true); resultText.Add(resultRarity);
        feedback = Text("", 16, Ink); feedback.style.unityTextAlign = TextAnchor.MiddleCenter; body.Add(feedback);
        roll = MakeButton("", new Color(.22f,.65f,.27f), machine.RollFromUI);
        roll.style.flexDirection = FlexDirection.Row; roll.style.alignItems = Align.Center; roll.style.justifyContent = Justify.Center;
        cost = Text(machine.Pool.Region.RollCost.ToString("N0"), 21, Color.white, true); roll.Add(cost);
        roll.Add(new InventoryIcon(InventoryIcon.Shape.Coin, Color.white, 32));
        body.Add(roll);
        continueButton = MakeButton("CONTINUAR", Blue, machine.ContinueFromUI); body.Add(continueButton);
        Tick();
    }

    private void RebuildReel()
    {
        reelTrack.Clear(); reelCards.Clear();
        foreach (PetData pet in machine.Reel)
        {
            if (pet == null) continue;
            var card = Panel(Rarity(pet.Rarity), 11); Border(card, Color.Lerp(Rarity(pet.Rarity), Ink, .3f), 3);
            card.style.position = Position.Absolute; card.style.top = 12; card.style.width = CardWidth - 8; card.style.height = 128;
            card.style.alignItems = Align.Center; reelTrack.Add(card);
            var image = new Image(); image.scaleMode = ScaleMode.ScaleToFit; image.style.width = Length.Percent(100); image.style.height = 80; card.Add(image);
            var label = Text(pet.PetName + "\n" + RarityName(pet.Rarity), 14, Color.white, true); label.style.unityTextAlign = TextAnchor.MiddleCenter; card.Add(label);
            reelCards.Add(Tuple.Create(card, image, pet));
        }
    }

    public void Tick()
    {
        if (machine.Preview == null) return;
        foreach (var pair in poolImages) pair.Item1.image = machine.Preview.Thumbnail(pair.Item2);
        balance.text = "SALDO: " + machine.Balance.ToString("N0");
        cost.text = machine.Pool.Region.RollCost.ToString("N0");
        var reward = machine.Reward;
        if (lastReward != reward) { lastReward = reward; if (reward != null) RebuildReel(); else { reelTrack.Clear(); reelCards.Clear(); } }
        foreach (var card in reelCards) card.Item2.image = machine.Preview.Thumbnail(card.Item3);
        float center = viewport.resolvedStyle.width * .5f;
        for (int i = 0; i < reelCards.Count; i++) reelCards[i].Item1.style.left = center + (i - machine.ReelPosition) * CardWidth - CardWidth * .5f + 4;
        bool finished = reward != null && !machine.IsSpinning;
        idleHint.style.display = reward == null ? DisplayStyle.Flex : DisplayStyle.None;
        viewport.style.flexGrow = reward == null ? 1 : 0;
        resultPanel.style.display = finished ? DisplayStyle.Flex : DisplayStyle.None;
        roll.style.display = reward == null ? DisplayStyle.Flex : DisplayStyle.None;
        continueButton.style.display = finished ? DisplayStyle.Flex : DisplayStyle.None;
        status.text = reward == null ? "Pronto para sortear!" : machine.IsSpinning ? "Sorteando..." : "Resultado do sorteio";
        feedback.text = finished ? "Pet recebido! Abra o inventário com I após fechar." : machine.Feedback;
        if (finished)
        {
            Color rarityColor = Rarity(reward.Rarity);
            resultImage.image = machine.Preview.SelectedImage; resultName.text = reward.PetName;
            resultRarity.text = RarityName(reward.Rarity); resultRarity.style.color = rarityColor;
            resultRays.RayColor = rarityColor; resultRays.MarkDirtyRepaint();
        }
    }

    public void Dispose()
    {
        IsVisible = false;
        if (documentObject != null) { documentObject.SetActive(false); UnityEngine.Object.Destroy(documentObject); }
        if (settings != null) UnityEngine.Object.Destroy(settings);
    }
}
