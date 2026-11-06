using System;
using System.Collections.Generic;
using UnityEngine;

// Provisional test interface. The real game will supply its own inventory and UI.
[RequireComponent(typeof(PetEquipmentTestDriver))]
public sealed class InventoryTestUI : MonoBehaviour
{
    private PetEquipmentTestDriver driver;
    private PlayerMovement movement;
    private bool open;
    private bool confirmFusion;
    private PetStackKey? selected;
    private Vector2 scroll;
    private string feedback = "";

    private void Start()
    {
        driver = GetComponent<PetEquipmentTestDriver>();
        movement = driver.Player != null ? driver.Player.GetComponent<PlayerMovement>() : null;
    }

    private void Update()
    {
        if (driver.GachaOpen) return;
        if (Input.GetKeyDown(KeyCode.I) || (open && Input.GetKeyDown(KeyCode.Escape))) SetOpen(!open);
    }

    private void OnDisable() { if (driver != null) SetOpen(false); }

    private void SetOpen(bool value)
    {
        open = value;
        confirmFusion = false;
        driver.InventoryOpen = value;
        if (movement != null) movement.ControlsEnabled = !value;
        if (driver.CameraController != null) driver.CameraController.ControlsEnabled = !value;
    }

    private void OnGUI()
    {
        if (!open || driver == null || driver.Inventory == null) return;
        float width = Mathf.Min(900, Screen.width - 30);
        float height = Mathf.Min(620, Screen.height - 30);
        GUILayout.BeginArea(new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height), GUI.skin.box);
        GUILayout.BeginHorizontal();
        GUILayout.Label("INVENTÁRIO — TESTE", GUILayout.Height(25));
        if (GUILayout.Button("Fechar (I / Esc)", GUILayout.Width(140))) SetOpen(false);
        GUILayout.EndHorizontal();
        if (!open) { GUILayout.EndArea(); return; }
        GUILayout.Label("Equipe — DPS total: " + driver.Inventory.TeamDamage);
        GUILayout.BeginHorizontal();
        for (int i = 0; i < PlayerInventory.MaxEquipment; i++)
        {
            var slot = driver.Inventory.GetSlot(i);
            string text = "Vazio";
            if (slot.HasValue && driver.Catalog.TryGetPet(slot.Value.PetId, out PetData pet))
                text = pet.PetName + " Lv." + slot.Value.FusionLevel + " | DPS " + PetDamage.Calculate(pet.BaseDamage, slot.Value.FusionLevel);
            GUILayout.Label((i + 1) + ": " + text, GUI.skin.box, GUILayout.ExpandWidth(true));
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(width * 0.55f));
        GUILayout.Label("Pets (selecione para ver detalhes)");
        scroll = GUILayout.BeginScrollView(scroll);
        List<InventoryEntry> entries = driver.Inventory.GetEntries();
        entries.Sort(CompareEntries);
        foreach (InventoryEntry entry in entries)
        {
            if (!driver.Catalog.TryGetPet(entry.Key.PetId, out PetData pet)) continue;
            string label = pet.PetName + " | " + pet.Rarity + " | Lv." + entry.Key.FusionLevel + " | DPS " + PetDamage.Calculate(pet.BaseDamage, entry.Key.FusionLevel) + " | x" + entry.Total + " (equipados " + entry.Equipped + ")";
            if (GUILayout.Button(label, GUILayout.MinHeight(36))) { selected = entry.Key; confirmFusion = false; feedback = ""; }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandHeight(true));
        DrawDetails();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private int CompareEntries(InventoryEntry a, InventoryEntry b)
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
        if (!selected.HasValue || !driver.Catalog.TryGetPet(selected.Value.PetId, out PetData pet)) { GUILayout.Label("Selecione um Pet."); return; }
        PetStackKey key = selected.Value;
        InventoryEntry entry = driver.Inventory.GetEntry(key);
        if (entry.Total == 0) { selected = null; GUILayout.Label("Selecione um Pet."); return; }
        GUILayout.Label(pet.PetName + " — " + pet.Rarity + " / " + pet.Region);
        GUILayout.Label("Nível " + key.FusionLevel + " | Dano base " + pet.BaseDamage + " | Dano final " + PetDamage.Calculate(pet.BaseDamage, key.FusionLevel));
        GUILayout.Label("Total: " + entry.Total + " | Equipados: " + entry.Equipped + " | Disponíveis: " + entry.Available);
        GUI.enabled = entry.Available > 0;
        if (GUILayout.Button("Equipar"))
        {
            bool equipped = false;
            for (int i = 0; i < PlayerInventory.MaxEquipment; i++) if (driver.Inventory.TryEquip(key.PetId, key.FusionLevel, i)) { equipped = true; break; }
            feedback = equipped ? "Pet equipado." : "Equipe cheia.";
        }
        GUI.enabled = entry.Equipped > 0;
        if (GUILayout.Button("Desequipar"))
            for (int i = 0; i < PlayerInventory.MaxEquipment; i++) { var slot = driver.Inventory.GetSlot(i); if (slot.HasValue && slot.Value.Equals(key)) { driver.Inventory.TryUnequip(i); feedback = "Pet desequipado."; break; } }
        GUI.enabled = entry.Available >= 3 && key.FusionLevel < PlayerInventory.MaxFusionLevel;
        if (GUILayout.Button("Fundir 3 cópias")) confirmFusion = true;
        GUI.enabled = true;
        if (confirmFusion)
        {
            GUILayout.Label("Consumir 3 cópias para criar nível " + (key.FusionLevel + 1) + " (dano " + PetDamage.Calculate(pet.BaseDamage, key.FusionLevel + 1) + ")?");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirmar")) { bool ok = driver.Inventory.TryFuse(key.PetId, key.FusionLevel); feedback = ok ? "Fusão concluída!" : "Fusão indisponível."; confirmFusion = false; if (ok) selected = new PetStackKey(key.PetId, key.FusionLevel + 1); }
            if (GUILayout.Button("Cancelar")) confirmFusion = false;
            GUILayout.EndHorizontal();
        }
        GUILayout.Label(feedback);
    }
}
