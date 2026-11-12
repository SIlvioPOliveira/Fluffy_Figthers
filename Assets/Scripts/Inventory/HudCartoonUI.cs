using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Runtime HUD; data and game rules remain owned by the existing driver.
public sealed class HudCartoonUI : IDisposable
{
    private readonly PetEquipmentTestDriver driver;
    private readonly PlayerHealthTest health;
    private readonly GameObject documentObject;
    private readonly PanelSettings settings;
    private readonly VisualElement root, hearts, team;
    private readonly Label coins, objective, teamDps;
    private readonly Button inventoryButton;
    private readonly List<Tuple<Image, PetData>> portraits = new List<Tuple<Image, PetData>>();
    private PetInventoryPreview preview;
    private int lastHealth = -1, lastMaxHealth = -1;
    private bool visible, teamDirty = true;
    private static readonly Color Ink = new Color(.12f,.23f,.29f);
    private static readonly Color Blue = new Color(.04f,.46f,.61f);
    private static readonly Color Gold = new Color(1f,.78f,.30f);

    private static VisualElement Panel(Color color, float radius = 15)
    {
        var p = new VisualElement { pickingMode = PickingMode.Ignore }; p.style.backgroundColor = color;
        p.style.borderTopLeftRadius = radius; p.style.borderTopRightRadius = radius;
        p.style.borderBottomLeftRadius = radius; p.style.borderBottomRightRadius = radius;
        p.style.borderTopWidth = 2; p.style.borderBottomWidth = 3; p.style.borderLeftWidth = 2; p.style.borderRightWidth = 2;
        p.style.borderTopColor = Gold; p.style.borderBottomColor = Gold; p.style.borderLeftColor = Gold; p.style.borderRightColor = Gold;
        return p;
    }
    private static Label Text(string value, int size, Color color, bool bold = false)
    {
        var l = new Label(value) { pickingMode = PickingMode.Ignore }; l.style.fontSize = size; l.style.color = color;
        l.style.unityFontStyleAndWeight = bold ? FontStyle.Bold : FontStyle.Normal; return l;
    }

    public HudCartoonUI(PetEquipmentTestDriver driver, PlayerHealthTest health, InventoryCanvasPrototype inventory, ThemeStyleSheet theme, Font font)
    {
        this.driver = driver; this.health = health;
        settings = ScriptableObject.CreateInstance<PanelSettings>(); settings.themeStyleSheet = theme;
        settings.scaleMode = PanelScaleMode.ScaleWithScreenSize; settings.referenceResolution = new Vector2Int(1600,900); settings.sortingOrder = -10;
        documentObject = new GameObject("HUDCartoon"); documentObject.transform.SetParent(driver.transform, false);
        var doc = documentObject.AddComponent<UIDocument>(); doc.panelSettings = settings; root = doc.rootVisualElement;
        root.pickingMode = PickingMode.Ignore; root.style.flexGrow = 1;
        if (font != null) root.style.unityFontDefinition = FontDefinition.FromFont(font);

        var wallet = Panel(Blue); wallet.style.position = Position.Absolute; wallet.style.top = 20; wallet.style.left = 22;
        wallet.style.width = 240; wallet.style.height = 58; wallet.style.flexDirection = FlexDirection.Row; wallet.style.alignItems = Align.Center; wallet.style.paddingLeft = 10; root.Add(wallet);
        wallet.Add(new InventoryIcon(InventoryIcon.Shape.Coin, new Color(.93f,.66f,.17f), 36));
        coins = Text("", 23, Color.white, true); coins.style.marginLeft = 9; wallet.Add(coins);

        var life = Panel(Blue); life.style.position = Position.Absolute; life.style.top = 20; life.style.right = 22;
        life.style.height = 58; life.style.minWidth = 190; life.style.flexDirection = FlexDirection.Row; life.style.alignItems = Align.Center;
        life.style.paddingLeft = 12; life.style.paddingRight = 12; root.Add(life);
        life.Add(Text("VIDA", 19, Color.white, true));
        hearts = new VisualElement { pickingMode = PickingMode.Ignore }; hearts.style.flexDirection = FlexDirection.Row; hearts.style.marginLeft = 8; life.Add(hearts);

        var task = Panel(new Color(.98f,.95f,.86f,.97f)); task.style.position = Position.Absolute; task.style.top = 82;
        task.style.left = Length.Percent(50); task.style.translate = new Translate(Length.Percent(-50), 0);
        task.style.minWidth = 370; task.style.maxWidth = 560; task.style.paddingLeft = 15; task.style.paddingRight = 15;
        task.style.paddingTop = 5; task.style.paddingBottom = 5; root.Add(task);
        objective = Text("", 18, Ink, true); objective.style.unityTextAlign = TextAnchor.MiddleCenter; objective.style.whiteSpace = WhiteSpace.Normal; task.Add(objective);

        var teamPanel = Panel(Blue); teamPanel.style.position = Position.Absolute; teamPanel.style.left = 22; teamPanel.style.bottom = 22;
        teamPanel.style.width = 380; teamPanel.style.paddingLeft = 10; teamPanel.style.paddingRight = 10; teamPanel.style.paddingTop = 5; teamPanel.style.paddingBottom = 8; root.Add(teamPanel);
        teamDps = Text("", 18, Color.white, true); teamPanel.Add(teamDps);
        team = new VisualElement { pickingMode = PickingMode.Ignore }; team.style.flexDirection = FlexDirection.Row; teamPanel.Add(team);

        inventoryButton = new Button(inventory.ToggleFromHud) { text = "INVENTÁRIO  [I]" };
        inventoryButton.style.position = Position.Absolute; inventoryButton.style.right = 22; inventoryButton.style.bottom = 22;
        inventoryButton.style.width = 220; inventoryButton.style.height = 60; inventoryButton.style.backgroundColor = new Color(.19f,.63f,.27f);
        inventoryButton.style.color = Color.white; inventoryButton.style.fontSize = 20; inventoryButton.style.unityFontStyleAndWeight = FontStyle.Bold;
        inventoryButton.style.borderTopLeftRadius = 14; inventoryButton.style.borderTopRightRadius = 14;
        inventoryButton.style.borderBottomLeftRadius = 14; inventoryButton.style.borderBottomRightRadius = 14;
        inventoryButton.style.borderTopWidth = 2; inventoryButton.style.borderLeftWidth = 2; inventoryButton.style.borderRightWidth = 2; inventoryButton.style.borderBottomWidth = 5;
        inventoryButton.style.borderTopColor = new Color(.11f,.38f,.16f); inventoryButton.style.borderBottomColor = new Color(.11f,.38f,.16f);
        inventoryButton.style.borderLeftColor = new Color(.11f,.38f,.16f); inventoryButton.style.borderRightColor = new Color(.11f,.38f,.16f);
        root.Add(inventoryButton);
        driver.Inventory.Changed += OnInventoryChanged;
    }

    private void OnInventoryChanged() => teamDirty = true;

    public void Tick(string objectiveText)
    {
        bool show = !driver.InventoryOpen && !driver.GachaOpen && !driver.BarrierOpen && !driver.PauseOpen && !driver.VictoryOpen;
        if (visible != show)
        {
            visible = show; root.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            if (!show && preview != null) { preview.Dispose(); preview = null; }
            if (show) { preview = new PetInventoryPreview(); teamDirty = true; }
        }
        if (!visible) return;
        coins.text = driver.Wallet.Balance + " moedas";
        objective.text = objectiveText ?? ""; objective.parent.style.display = string.IsNullOrEmpty(objectiveText) ? DisplayStyle.None : DisplayStyle.Flex;
        int current = health != null ? health.Health : 0, maximum = health != null ? health.MaxHealth : 0;
        if (current != lastHealth || maximum != lastMaxHealth)
        {
            lastHealth = current; lastMaxHealth = maximum; hearts.Clear();
            for (int i = 0; i < maximum; i++) { var heart = new InventoryIcon(InventoryIcon.Shape.Heart, i < current ? new Color(1f,.35f,.40f) : new Color(.43f,.51f,.51f), 27); heart.style.marginRight = 2; hearts.Add(heart); }
        }
        if (teamDirty) RebuildTeam();
        if (preview != null)
        {
            preview.Tick(Time.unscaledDeltaTime);
            foreach (var portrait in portraits) portrait.Item1.image = preview.Thumbnail(portrait.Item2);
        }
    }

    private void RebuildTeam()
    {
        teamDirty = false; team.Clear(); portraits.Clear(); teamDps.text = "EQUIPE    •    DPS " + driver.Inventory.TeamDamage;
        for (int i = 0; i < PlayerInventory.MaxEquipment; i++)
        {
            var slot = driver.Inventory.GetSlot(i);
            var card = Panel(new Color(.84f,.93f,.89f), 9); card.style.flexGrow = 1; card.style.flexBasis = 0; card.style.height = 82;
            card.style.marginRight = i < PlayerInventory.MaxEquipment - 1 ? 5 : 0; card.style.alignItems = Align.Center; team.Add(card);
            if (slot.HasValue && driver.Catalog.TryGetPet(slot.Value.PetId, out PetData pet))
            {
                var picture = new Image { pickingMode = PickingMode.Ignore }; picture.scaleMode = ScaleMode.ScaleToFit;
                picture.style.width = 55; picture.style.height = 50; card.Add(picture); portraits.Add(Tuple.Create(picture, pet));
                var label = Text(pet.PetName + " • " + slot.Value.FusionLevel + "★", 12, Ink, true);
                var symbols = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (symbols != null) label.style.unityFontDefinition = FontDefinition.FromFont(symbols);
                label.style.unityTextAlign = TextAnchor.MiddleCenter; label.style.whiteSpace = WhiteSpace.Normal; card.Add(label);
            }
            else
            {
                var icon = new InventoryIcon(InventoryIcon.Shape.Paw, new Color(.53f,.70f,.67f), 30); icon.style.marginTop = 10; card.Add(icon);
                card.Add(Text("Vazio", 14, Ink, true));
            }
        }
    }

    public bool IsPointerOverInventoryButton()
    {
        if (!visible || Screen.width <= 0 || Screen.height <= 0) return false;
        Vector3 p = Input.mousePosition;
        Vector2 point = new Vector2(p.x * root.worldBound.width / Screen.width, (Screen.height - p.y) * root.worldBound.height / Screen.height);
        return inventoryButton.worldBound.Contains(point);
    }

    public void Dispose()
    {
        driver.Inventory.Changed -= OnInventoryChanged;
        if (preview != null) preview.Dispose();
        if (documentObject != null) { documentObject.SetActive(false); UnityEngine.Object.Destroy(documentObject); }
        if (settings != null) UnityEngine.Object.Destroy(settings);
    }
}
