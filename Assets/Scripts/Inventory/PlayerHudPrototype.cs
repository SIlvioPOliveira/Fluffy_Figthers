using UnityEngine;

// Provisional HUD for the Water test scene; final art and tutorial remain separate.
[RequireComponent(typeof(PetEquipmentTestDriver), typeof(InventoryTestUI))]
public sealed class PlayerHudPrototype : MonoBehaviour
{
    private PetEquipmentTestDriver driver;
    private InventoryTestUI inventoryUI;
    private PlayerHealthTest health;
    private GachaMachineTest machine;
    private BossControllerTest waterBoss;
    private ZoneBarrierTest gate;
    private GUIStyle panelStyle;
    private GUIStyle textStyle;
    private GUIStyle buttonStyle;
    private int lastTutorialStage;
    private float completedUntil;

    private void Start()
    {
        driver = GetComponent<PetEquipmentTestDriver>();
        inventoryUI = GetComponent<InventoryTestUI>();
        health = driver.Player != null ? driver.Player.GetComponent<PlayerHealthTest>() : null;
        foreach (var candidate in FindObjectsByType<GachaMachineTest>(FindObjectsSortMode.None))
            if (candidate.Region == RegionId.Water) { machine = candidate; break; }
        foreach (var boss in FindObjectsByType<BossControllerTest>(FindObjectsSortMode.None))
            if (boss.IsWaterBoss) { waterBoss = boss; break; }
        foreach (var candidate in FindObjectsByType<ZoneBarrierTest>(FindObjectsSortMode.None))
            if (candidate.SourceRegion == RegionId.Water) { gate = candidate; break; }
        lastTutorialStage = driver.TutorialStage;
    }

    private void Update()
    {
        if (driver != null && lastTutorialStage < 8 && driver.TutorialStage == 8) completedUntil = Time.unscaledTime + 5f;
        if (driver != null) lastTutorialStage = driver.TutorialStage;
    }

    private static Rect InventoryRect => new Rect(Screen.width - 165, Screen.height - 60, 150, 45);

    public static bool IsPointerOverInventoryButton()
    {
        var hud = FindFirstObjectByType<PlayerHudPrototype>();
        if (hud == null || !hud.isActiveAndEnabled || hud.driver == null || hud.driver.GachaOpen || hud.driver.BarrierOpen) return false;
        Vector3 p = Input.mousePosition;
        return InventoryRect.Contains(new Vector2(p.x, Screen.height - p.y));
    }

    private void OnGUI()
    {
        if (driver == null || driver.Wallet == null || driver.Inventory == null) return;
        if (panelStyle == null)
        {
            panelStyle = new GUIStyle(GUI.skin.box) { fontSize = 15, alignment = TextAnchor.UpperCenter };
            textStyle = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 14 };
        }
        GUI.Box(new Rect(15, 15, 180, 48), "MOEDAS\n" + driver.Wallet.Balance, panelStyle);
        string objective = driver.TutorialStage == 0 ? "Vá até a máquina de Gacha e pressione E" :
            driver.TutorialStage == 1 ? "Realize seu primeiro Roll" :
            driver.TutorialStage == 2 ? "Abra o inventário [I]" :
            driver.TutorialStage == 3 ? "Equipe seu primeiro Pet" :
            driver.TutorialStage == 4 ? "Clique em um inimigo e derrote-o" :
            driver.TutorialStage == 5 ? "Consiga moedas e obtenha outro Pet" :
            driver.TutorialStage == 6 ? "Derrote o Guardião das Marés" :
            driver.TutorialStage == 7 ? "Desbloqueie a região Fogo" + (gate != null ? " (" + gate.UnlockCost + " moedas)" : "") : null;
        if (objective != null)
        {
            float objectiveWidth = Mathf.Min(420, Screen.width - 30);
            GUI.Box(new Rect((Screen.width - objectiveWidth) / 2, 70, objectiveWidth, 35), objective, panelStyle);
            if (driver.TutorialStage == 0 && machine != null && Camera.main != null && !driver.GachaOpen && !driver.InventoryOpen && !driver.BarrierOpen)
            {
                Vector3 point = Camera.main.WorldToScreenPoint(machine.transform.position + Vector3.up * 3);
                if (point.z > 0 && point.x >= 0 && point.x <= Screen.width && point.y >= 0 && point.y <= Screen.height)
                    GUI.Box(new Rect(Mathf.Clamp(point.x - 55, 0, Screen.width - 110), Mathf.Clamp(Screen.height - point.y - 30, 110, Screen.height - 55), 110, 28), "GACHA ↓", panelStyle);
            }
            Transform destination = driver.TutorialStage == 6 && waterBoss != null ? waterBoss.transform : driver.TutorialStage == 7 && gate != null ? gate.transform : null;
            if (destination != null && Camera.main != null && !driver.GachaOpen && !driver.InventoryOpen && !driver.BarrierOpen)
            {
                Vector3 point = Camera.main.WorldToScreenPoint(destination.position + Vector3.up * 3);
                if (point.z > 0 && point.x >= 0 && point.x <= Screen.width && point.y >= 0 && point.y <= Screen.height)
                    GUI.Box(new Rect(Mathf.Clamp(point.x - 70, 0, Screen.width - 140), Mathf.Clamp(Screen.height - point.y - 30, 110, Screen.height - 55), 140, 28), driver.TutorialStage == 6 ? "BOSS ↓" : "BARREIRA ↓", panelStyle);
            }
        }
        else if (Time.unscaledTime < completedUntil)
            GUI.Box(new Rect(Screen.width * 0.5f - 170, 70, 340, 35), "Tutorial concluído!", panelStyle);
        if (health != null)
        {
            GUI.Box(new Rect(Screen.width - 175, 15, 160, 48), "VIDA  " + health.Health + " / " + health.MaxHealth, panelStyle);
            Color previous = GUI.color;
            for (int i = 0; i < health.MaxHealth; i++)
            {
                GUI.color = i < health.Health ? new Color(1f, .3f, .35f) : new Color(.35f, .35f, .35f);
                GUI.DrawTexture(new Rect(Screen.width - 160 + i * 25, 43, 19, 12), Texture2D.whiteTexture);
            }
            GUI.color = previous;
        }
        GUI.Box(new Rect(15, Screen.height - 125, 250, 110), "EQUIPE  •  DPS: " + driver.Inventory.TeamDamage, panelStyle);
        for (int i = 0; i < PlayerInventory.MaxEquipment; i++)
        {
            PetStackKey? slot = driver.Inventory.GetSlot(i);
            string label = "Vazio";
            if (slot.HasValue && driver.Catalog.TryGetPet(slot.Value.PetId, out var pet))
                label = pet.PetName + "  Nv." + slot.Value.FusionLevel;
            GUI.Label(new Rect(28, Screen.height - 96 + i * 23, 225, 22), (i + 1) + ". " + label, textStyle);
        }
        if (!driver.PauseOpen && !driver.VictoryOpen && !driver.GachaOpen && !driver.BarrierOpen && GUI.Button(InventoryRect, driver.InventoryOpen ? "FECHAR [I]" : "INVENTÁRIO [I]", buttonStyle))
            inventoryUI.ToggleFromHud();
    }
}
