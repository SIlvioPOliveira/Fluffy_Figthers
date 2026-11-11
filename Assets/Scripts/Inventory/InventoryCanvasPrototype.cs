using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Isolated inventory prototype. The existing inventory screen remains untouched.
[RequireComponent(typeof(PetEquipmentTestDriver))]
public sealed class InventoryCanvasPrototype : MonoBehaviour
{
    [SerializeField] private ThemeStyleSheet theme;
    private PetEquipmentTestDriver driver;
    private PlayerMovement movement;
    private PetInventoryPreview preview;
    private PetStackKey? selected;
    private UIDocument document;
    private PanelSettings panelSettings;
    private VisualElement root, team, grid, details;
    private readonly List<Tuple<Image, PetData, bool>> pictures = new List<Tuple<Image, PetData, bool>>();
    private Image selectedPicture;
    private bool open, confirmFusion;
    private string feedback = "";
    private static readonly Color Cream = new Color(.98f, .95f, .86f);
    private static readonly Color Ink = new Color(.12f, .23f, .29f);
    private static readonly Color Blue = new Color(.04f, .46f, .61f);
    private static readonly Color Gold = new Color(1f, .78f, .30f);

    private static void Outline(VisualElement element, Color color, float width)
    {
        element.style.borderTopColor = color; element.style.borderBottomColor = color;
        element.style.borderLeftColor = color; element.style.borderRightColor = color;
        element.style.borderTopWidth = width; element.style.borderBottomWidth = width;
        element.style.borderLeftWidth = width; element.style.borderRightWidth = width;
    }

    private static void Hover(VisualElement element, Color normal, Color hovered)
    {
        element.RegisterCallback<PointerEnterEvent>(_ => { if (element.enabledInHierarchy) element.style.backgroundColor = hovered; });
        element.RegisterCallback<PointerLeaveEvent>(_ => element.style.backgroundColor = normal);
    }

    private void Start()
    {
        driver = GetComponent<PetEquipmentTestDriver>();
        movement = driver.Player != null ? driver.Player.GetComponent<PlayerMovement>() : null;
        driver.Inventory.Changed += InventoryChanged;
        panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        panelSettings.themeStyleSheet = theme;
        panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        panelSettings.referenceResolution = new Vector2Int(1600, 900);
        var docObject = new GameObject("InventarioVisualTeste");
        docObject.transform.SetParent(transform, false);
        document = docObject.AddComponent<UIDocument>();
        document.panelSettings = panelSettings;
        root = document.rootVisualElement;
        root.style.display = DisplayStyle.None;
        Build();
    }

    private void Update()
    {
        if (open && preview != null)
        {
            preview.Tick(Time.unscaledDeltaTime);
            foreach (var item in pictures)
                item.Item1.image = item.Item3 ? preview.SelectedImage : preview.Thumbnail(item.Item2);
            if (selectedPicture != null) selectedPicture.image = preview.SelectedImage;
        }
        if (driver.GachaOpen || driver.BarrierOpen || driver.PauseOpen || driver.VictoryOpen) return;
        if (Input.GetKeyDown(KeyCode.I) || (open && Input.GetKeyDown(KeyCode.Escape))) SetOpen(!open);
    }

    private void OnDestroy()
    {
        if (driver != null && driver.Inventory != null) driver.Inventory.Changed -= InventoryChanged;
        if (open) SetOpen(false);
        if (panelSettings != null) Destroy(panelSettings);
    }

    private void SetOpen(bool value)
    {
        open = value;
        driver.InventoryOpen = value;
        if (movement != null) movement.ControlsEnabled = !value;
        if (driver.CameraController != null) driver.CameraController.ControlsEnabled = !value;
        if (value)
        {
            preview = new PetInventoryPreview();
            driver.TutorialInventoryOpened();
            root.style.display = DisplayStyle.Flex;
            Refresh();
        }
        else
        {
            if (preview != null) preview.Dispose();
            preview = null; selected = null; confirmFusion = false; feedback = "";
            root.style.display = DisplayStyle.None;
        }
    }

    private static VisualElement Panel(Color color, float radius = 18)
    {
        var e = new VisualElement(); e.style.backgroundColor = color;
        e.style.borderTopLeftRadius = radius; e.style.borderTopRightRadius = radius;
        e.style.borderBottomLeftRadius = radius; e.style.borderBottomRightRadius = radius;
        return e;
    }

    private static Label Text(string text, int size, Color color, bool bold = false)
    {
        var label = new Label(text); label.style.fontSize = size; label.style.color = color;
        label.style.unityFontStyleAndWeight = bold ? FontStyle.Bold : FontStyle.Normal;
        label.style.whiteSpace = WhiteSpace.Normal; return label;
    }

    private static Button ActionButton(string label, Color color, Action action)
    {
        var button = new Button(action) { text = label };
        button.style.backgroundColor = color; button.style.color = Color.white;
        button.style.fontSize = 19; button.style.unityFontStyleAndWeight = FontStyle.Bold;
        button.style.minHeight = 46; button.style.flexGrow = 1; button.style.marginLeft = 4; button.style.marginRight = 4;
        button.style.borderTopLeftRadius = 12; button.style.borderTopRightRadius = 12;
        button.style.borderBottomLeftRadius = 12; button.style.borderBottomRightRadius = 12;
        Outline(button, new Color(color.r * .65f, color.g * .65f, color.b * .65f), 2);
        button.style.borderBottomWidth = 5;
        Hover(button, color, Color.Lerp(color, Color.white, .18f));
        return button;
    }

    private void Build()
    {
        root.style.flexGrow = 1; root.style.backgroundColor = new Color(0, 0, 0, .68f);
        root.style.alignItems = Align.Center; root.style.justifyContent = Justify.Center;
        var frame = Panel(new Color(.06f, .53f, .63f), 24);
        Outline(frame, new Color(.02f, .30f, .39f), 5);
        frame.style.width = Length.Percent(94); frame.style.height = Length.Percent(92);
        frame.style.maxWidth = 1500; frame.style.paddingLeft = 12; frame.style.paddingRight = 12;
        frame.style.paddingTop = 12; frame.style.paddingBottom = 12; root.Add(frame);
        var header = Panel(Blue); Outline(header, Gold, 2); header.style.height = 64; header.style.flexDirection = FlexDirection.Row;
        header.style.justifyContent = Justify.SpaceBetween; header.style.alignItems = Align.Center;
        header.style.paddingLeft = 22; header.style.paddingRight = 14; frame.Add(header);
        var title = Text("✦  INVENTÁRIO", 31, Color.white, true); header.Add(title);
        var close = ActionButton("Fechar  ×", new Color(.04f, .33f, .46f), () => SetOpen(false));
        close.style.flexGrow = 0; close.style.width = 145; header.Add(close);
        team = Panel(Cream); Outline(team, new Color(.88f, .78f, .56f), 2); team.style.height = 115; team.style.marginTop = 9;
        team.style.flexDirection = FlexDirection.Row; team.style.alignItems = Align.Center;
        team.style.paddingLeft = 14; team.style.paddingRight = 14; frame.Add(team);
        var body = new VisualElement(); body.style.flexDirection = FlexDirection.Row; body.style.flexGrow = 1;
        body.style.minHeight = 0; body.style.marginTop = 9; frame.Add(body);
        var left = Panel(Cream); Outline(left, new Color(.88f, .78f, .56f), 2); left.style.flexGrow = 3; left.style.flexBasis = 0; left.style.paddingLeft = 12;
        left.style.paddingRight = 12; left.style.paddingTop = 10; left.style.paddingBottom = 10; body.Add(left);
        var collectionTitle = Panel(new Color(.06f, .49f, .61f), 12); collectionTitle.style.paddingLeft = 12; collectionTitle.style.paddingTop = 3; collectionTitle.style.paddingBottom = 3;
        collectionTitle.Add(Text("MEUS PETS", 22, Color.white, true)); left.Add(collectionTitle);
        var scroll = new ScrollView(ScrollViewMode.Vertical); scroll.style.flexGrow = 1; left.Add(scroll);
        grid = scroll.contentContainer; grid.style.flexDirection = FlexDirection.Row; grid.style.flexWrap = Wrap.Wrap;
        details = Panel(Cream); Outline(details, new Color(.88f, .78f, .56f), 2); details.style.flexGrow = 2; details.style.flexBasis = 0;
        details.style.marginLeft = 10; details.style.paddingLeft = 16; details.style.paddingRight = 16;
        details.style.paddingTop = 12; details.style.paddingBottom = 12; body.Add(details);
    }

    private static Color Rarity(PetRarity rarity)
    {
        switch (rarity) { case PetRarity.Rare: return new Color(.30f,.64f,.92f); case PetRarity.Epic: return new Color(.66f,.39f,.80f); case PetRarity.Legendary: return new Color(.91f,.64f,.22f); case PetRarity.Secret: return new Color(.86f,.36f,.53f); default: return new Color(.53f,.65f,.68f); }
    }
    private static string RarityName(PetRarity rarity)
    {
        switch (rarity) { case PetRarity.Rare: return "Raro"; case PetRarity.Epic: return "Épico"; case PetRarity.Legendary: return "Lendário"; case PetRarity.Secret: return "Secreto"; default: return "Comum"; }
    }

    private static string RegionName(RegionId region)
    {
        switch (region) { case RegionId.Water: return "Água"; case RegionId.Fire: return "Fogo"; case RegionId.Lava: return "Lava"; case RegionId.Ice: return "Gelo"; default: return region.ToString(); }
    }

    private void InventoryChanged() { if (open) Refresh(); }
    private void Refresh()
    {
        team.Clear(); grid.Clear(); details.Clear(); pictures.Clear(); selectedPicture = null;
        var heading = Text("EQUIPE\nDPS " + driver.Inventory.TeamDamage, 21, Ink, true); heading.style.width = 170; team.Add(heading);
        for (int i = 0; i < PlayerInventory.MaxEquipment; i++)
        {
            var slot = driver.Inventory.GetSlot(i); string name = "Vazio";
            PetData pet = null;
            if (slot.HasValue && driver.Catalog.TryGetPet(slot.Value.PetId, out pet))
                name = pet.PetName + "\nNv. " + slot.Value.FusionLevel + "  |  DPS " + PetDamage.Calculate(pet.BaseDamage, slot.Value.FusionLevel);
            Color slotColor = pet != null ? new Color(.80f,.92f,.91f) : new Color(.86f,.90f,.85f);
            var box = Panel(slotColor, 12); Outline(box, new Color(.59f,.76f,.73f), 2); box.style.flexGrow = 1; box.style.height = 82; box.style.marginRight = 8;
            box.style.flexBasis = 0; box.style.flexDirection = FlexDirection.Row;
            box.style.alignItems = Align.Center; box.style.paddingLeft = 10;
            if (pet != null)
            {
                PetStackKey key = slot.Value;
                bool chosen = selected.HasValue && selected.Value.Equals(key);
                if (chosen) Outline(box, Gold, 4);
                Hover(box, slotColor, Color.Lerp(slotColor, Color.white, .35f));
                var thumbnail = new Image(); thumbnail.scaleMode = ScaleMode.ScaleToFit; thumbnail.style.width = 65; thumbnail.style.height = 70; thumbnail.pickingMode = PickingMode.Ignore; box.Add(thumbnail);
                pictures.Add(Tuple.Create(thumbnail, pet, chosen));
                var caption = Text(name, 16, Ink, true); caption.pickingMode = PickingMode.Ignore; box.Add(caption);
                PetData equippedPet = pet;
                box.RegisterCallback<ClickEvent>(_ => SelectPet(key, equippedPet, false));
            }
            else box.Add(Text(name, 18, Ink, true));
            team.Add(box);
        }
        var entries = driver.Inventory.GetEntries(); entries.Sort(Compare);
        if (entries.Count == 0) grid.Add(Text("Sua coleção está vazia. Visite uma máquina de Gacha para obter seu primeiro Pet!", 20, Ink));
        foreach (var entry in entries)
        {
            if (!driver.Catalog.TryGetPet(entry.Key.PetId, out PetData pet)) continue;
            bool chosen = selected.HasValue && selected.Value.Equals(entry.Key);
            Color rarity = Rarity(pet.Rarity);
            var card = Panel(rarity, 14); Outline(card, Color.Lerp(rarity, Ink, .30f), 3); card.style.width = Length.Percent(23); card.style.height = 205;
            card.style.marginLeft = Length.Percent(1); card.style.marginRight = Length.Percent(1); card.style.marginTop = 8;
            card.style.paddingLeft = 5; card.style.paddingRight = 5; card.style.paddingTop = 4; card.style.paddingBottom = 4;
            if (chosen) Outline(card, Gold, 4);
            Hover(card, rarity, Color.Lerp(rarity, Color.white, .18f));
            var image = new Image(); image.scaleMode = ScaleMode.ScaleToFit; image.style.height = 108; image.pickingMode = PickingMode.Ignore; card.Add(image);
            pictures.Add(Tuple.Create(image, pet, chosen));
            var stars = Text(new string('★', entry.Key.FusionLevel), 17, new Color(1f, .9f, .25f), true);
            stars.name = "EstrelasNivel"; stars.tooltip = "Nível " + entry.Key.FusionLevel;
            stars.style.position = Position.Absolute; stars.style.top = 4; stars.style.right = 5;
            stars.pickingMode = PickingMode.Ignore; card.Add(stars);
            var name = Text(pet.PetName, 16, Color.white, true); name.pickingMode = PickingMode.Ignore; card.Add(name);
            var info = Text(RarityName(pet.Rarity) + " | Nv. " + entry.Key.FusionLevel + "\nDPS " + PetDamage.Calculate(pet.BaseDamage, entry.Key.FusionLevel) + " | x" + entry.Total + " (eq. " + entry.Equipped + ")", 13, Color.white); info.pickingMode = PickingMode.Ignore; card.Add(info);
            PetStackKey key = entry.Key;
            card.RegisterCallback<ClickEvent>(_ => SelectPet(key, pet, true));
            grid.Add(card);
        }
        DrawDetails();
    }

    private void SelectPet(PetStackKey key, PetData pet, bool toggle)
    {
        bool deselect = toggle && selected.HasValue && selected.Value.Equals(key);
        selected = deselect ? (PetStackKey?)null : key;
        preview.Select(deselect ? null : pet);
        confirmFusion = false;
        feedback = "";
        Refresh();
    }

    private int Compare(InventoryEntry a, InventoryEntry b)
    {
        if (!driver.Catalog.TryGetPet(a.Key.PetId, out PetData pa)) return 1;
        if (!driver.Catalog.TryGetPet(b.Key.PetId, out PetData pb)) return -1;
        int c = PetDamage.Calculate(pb.BaseDamage, b.Key.FusionLevel).CompareTo(PetDamage.Calculate(pa.BaseDamage, a.Key.FusionLevel));
        if (c == 0) c = pb.Rarity.CompareTo(pa.Rarity);
        if (c == 0) c = string.Compare(pa.PetName, pb.PetName, StringComparison.CurrentCulture);
        return c != 0 ? c : a.Key.FusionLevel.CompareTo(b.Key.FusionLevel);
    }

    private void DrawDetails()
    {
        if (!selected.HasValue || !driver.Catalog.TryGetPet(selected.Value.PetId, out PetData pet)) { details.Add(Text("Selecione um Pet para ver os detalhes.", 23, Ink, true)); return; }
        PetStackKey key = selected.Value; InventoryEntry entry = driver.Inventory.GetEntry(key);
        if (entry.Total == 0) { selected = null; preview.Select(null); details.Add(Text("Selecione um Pet.", 23, Ink)); return; }
        var petTitle = Panel(Blue, 12); petTitle.style.paddingLeft = 12; petTitle.style.paddingTop = 3; petTitle.style.paddingBottom = 3;
        petTitle.Add(Text(pet.PetName, 25, Color.white, true)); details.Add(petTitle);
        var rarityLabel = Text(RarityName(pet.Rarity) + "  •  " + RegionName(pet.Region), 19, Rarity(pet.Rarity), true); rarityLabel.style.marginTop = 5; details.Add(rarityLabel);
        var stage = Panel(new Color(.80f, .93f, .93f), 24); stage.style.flexGrow = 1; stage.style.minHeight = 100; stage.style.marginTop = 5; stage.style.marginBottom = 7;
        Outline(stage, new Color(.65f, .83f, .82f), 2); details.Add(stage);
        selectedPicture = new Image(); selectedPicture.scaleMode = ScaleMode.ScaleToFit; selectedPicture.style.flexGrow = 1; stage.Add(selectedPicture);
        var stats = Panel(new Color(.94f, .88f, .74f), 10); stats.style.paddingLeft = 8; stats.style.paddingTop = 5; stats.style.paddingBottom = 5;
        stats.Add(Text("Nível " + key.FusionLevel + "   |   Dano base " + pet.BaseDamage + "   |   DPS " + PetDamage.Calculate(pet.BaseDamage, key.FusionLevel), 17, Ink, true));
        stats.Add(Text("Total " + entry.Total + "   •   Equipados " + entry.Equipped + "   •   Disponíveis " + entry.Available, 16, Ink)); details.Add(stats);
        var actions = new VisualElement(); actions.style.flexDirection = FlexDirection.Row; actions.style.marginTop = 10; details.Add(actions);
        var equip = ActionButton("Equipar", new Color(.22f,.65f,.27f), () => { bool ok = false; for (int i = 0; i < PlayerInventory.MaxEquipment; i++) if (driver.Inventory.TryEquip(key.PetId, key.FusionLevel, i)) { ok = true; break; } feedback = ok ? "Pet equipado!" : "Equipe cheia."; Refresh(); }); equip.SetEnabled(entry.Available > 0); actions.Add(equip);
        var unequip = ActionButton("Desequipar", Blue, () => { for (int i = 0; i < PlayerInventory.MaxEquipment; i++) { var slot = driver.Inventory.GetSlot(i); if (slot.HasValue && slot.Value.Equals(key)) { driver.Inventory.TryUnequip(i); feedback = "Pet desequipado."; break; } } Refresh(); }); unequip.SetEnabled(entry.Equipped > 0); actions.Add(unequip);
        var fuse = ActionButton("Fundir", new Color(.58f,.32f,.70f), () => { confirmFusion = true; Refresh(); }); fuse.SetEnabled(entry.Available >= 3 && key.FusionLevel < PlayerInventory.MaxFusionLevel); actions.Add(fuse);
        if (confirmFusion)
        {
            details.Add(Text("Usar 3 cópias para criar nível " + (key.FusionLevel + 1) + "?", 17, Ink, true));
            var confirm = new VisualElement(); confirm.style.flexDirection = FlexDirection.Row; details.Add(confirm);
            confirm.Add(ActionButton("Confirmar", new Color(.22f,.65f,.27f), () => { bool ok = driver.Inventory.TryFuse(key.PetId, key.FusionLevel); confirmFusion = false; feedback = ok ? "Fusão concluída!" : "Fusão indisponível."; if (ok) { selected = new PetStackKey(key.PetId, key.FusionLevel + 1); preview.Select(pet); } Refresh(); }));
            confirm.Add(ActionButton("Cancelar", Blue, () => { confirmFusion = false; Refresh(); }));
        }
        details.Add(Text(feedback, 16, Ink));
    }
}
