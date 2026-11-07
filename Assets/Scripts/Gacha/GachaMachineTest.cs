using UnityEngine;

// Test-scene interaction and temporary UI, not the final machine or presentation.
public sealed class GachaMachineTest : MonoBehaviour
{
    [SerializeField] private PetEquipmentTestDriver driver;
    [SerializeField] private GachaPool pool;
    [SerializeField, Min(0.1f)] private float interactionDistance = 3f;
    [SerializeField, Min(0.1f)] private float spinDuration = 4.5f;
    private const int ReelLength = 40;
    private const int ReelStart = 4;
    private const int WinningIndex = 34;
    private GachaSystem system;
    private bool open;
    private PetData result;
    private readonly PetData[] reel = new PetData[ReelLength];
    private float spinStarted;
    private bool spinning;
    private string message = "";

    private void Start()
    {
        if (driver == null || pool == null || driver.Inventory == null || driver.Wallet == null)
        {
            Debug.LogError("Gacha test scene is missing initialized references.", this);
            enabled = false;
            return;
        }
        system = new GachaSystem(driver.Catalog, driver.Wallet, driver.Inventory);
    }

    private bool InRange => driver != null && driver.Player != null &&
        Vector3.Distance(driver.Player.position, transform.position) <= interactionDistance;

    private void Update()
    {
        if (driver.PauseOpen) return;
        if (spinning && Time.time - spinStarted >= Mathf.Max(0.1f, spinDuration)) spinning = false;
        if (open && Input.GetKeyDown(KeyCode.Escape)) SetOpen(false);
        else if (!open && !driver.InventoryOpen && !driver.BarrierOpen && InRange && Input.GetKeyDown(KeyCode.E)) SetOpen(true);
    }

    private void OnDisable() { if (driver != null && open) SetOpen(false); }

    private void SetOpen(bool value)
    {
        open = value;
        driver.GachaOpen = value;
        if (value) driver.TutorialMachineOpened();
        var movement = driver.Player.GetComponent<PlayerMovement>();
        if (movement != null) movement.ControlsEnabled = !value;
        if (driver.CameraController != null) driver.CameraController.ControlsEnabled = !value;
        // A closed window does not cancel an already-delivered reward or unlock a second roll.
        if (!value && result == null) message = "";
    }

    private GachaResult BeginRoll(int ticket)
    {
        if (result != null || spinning) return GachaResult.Busy;
        GachaResult status = system.TryRoll(pool, ticket, out PetData reward);
        if (status != GachaResult.Success) return status;
        result = reward;
        for (int i = 0; i < reel.Length; i++) reel[i] = pool.Entries[Random.Range(0, pool.Entries.Count)].Pet;
        reel[WinningIndex] = reward;
        // The cards on either side are visual only; neither influences the selected reward.
        reel[WinningIndex - 1] = OtherPet(reward);
        reel[WinningIndex + 1] = OtherPet(reward);
        spinStarted = Time.time;
        spinning = true;
        return status;
    }

    private PetData OtherPet(PetData reward)
    {
        int start = Random.Range(0, pool.Entries.Count);
        for (int i = 0; i < pool.Entries.Count; i++)
        {
            PetData candidate = pool.Entries[(start + i) % pool.Entries.Count].Pet;
            if (candidate != reward) return candidate;
        }
        return reward;
    }

    private static Color RarityColor(PetRarity rarity)
    {
        switch (rarity)
        {
            case PetRarity.Rare: return new Color(0.25f, 0.55f, 1f);
            case PetRarity.Epic: return new Color(0.7f, 0.35f, 0.95f);
            case PetRarity.Legendary: return new Color(1f, 0.7f, 0.15f);
            case PetRarity.Secret: return new Color(1f, 0.35f, 0.55f);
            default: return new Color(0.75f, 0.8f, 0.83f);
        }
    }

    private void DrawReel()
    {
        Rect area = GUILayoutUtility.GetRect(10, 90, GUILayout.ExpandWidth(true));
        GUI.Box(area, GUIContent.none);
        float progress = Mathf.Clamp01((Time.time - spinStarted) / Mathf.Max(0.1f, spinDuration));
        float ease = 1f - Mathf.Pow(1f - progress, 3f);
        const float cardWidth = 120f;
        float position = Mathf.Lerp(ReelStart, WinningIndex, ease);
        GUI.BeginGroup(area);
        for (int i = 0; i < reel.Length; i++)
        {
            float x = area.width * 0.5f + (i - position) * cardWidth - cardWidth * 0.5f;
            if (x > area.width || x + cardWidth < 0 || reel[i] == null) continue;
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = RarityColor(reel[i].Rarity);
            GUI.Box(new Rect(x + 3, 10, cardWidth - 6, 70), reel[i].PetName + "\n" + reel[i].Rarity);
            GUI.backgroundColor = previous;
        }
        GUI.Label(new Rect(area.width * 0.5f - 5, 0, 20, 20), "▼");
        GUI.EndGroup();
    }

    private void OnGUI()
    {
        if (driver == null || driver.Inventory == null || pool == null || system == null) return;
        if (!open)
        {
            if (driver.GetComponent<PlayerHudPrototype>() == null)
                GUI.Label(new Rect(15, 45, 600, 25), "Moedas: " + driver.Wallet.Balance + (InRange && !driver.InventoryOpen ? "     [E] Abrir Gacha de Água" : ""));
            else if (InRange && !driver.InventoryOpen && !driver.BarrierOpen)
                GUI.Label(new Rect(15, 75, 350, 25), "[E] Abrir Gacha de Água");
            return;
        }
        float width = Mathf.Min(580, Screen.width - 30);
        float height = Mathf.Min(550, Screen.height - 30);
        GUILayout.BeginArea(new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height), GUI.skin.box);
        GUILayout.BeginHorizontal();
        GUILayout.Label("GACHA — " + pool.Region.DisplayName);
        if (GUILayout.Button("Fechar (Esc)", GUILayout.Width(130))) SetOpen(false);
        GUILayout.EndHorizontal();
        if (!open) { GUILayout.EndArea(); return; }
        GUILayout.Label("Saldo: " + driver.Wallet.Balance + " moedas | Custo: " + pool.Region.RollCost);
        foreach (GachaEntry entry in pool.Entries)
            GUILayout.Label(entry.Pet.PetName + " — " + entry.Pet.Rarity + " — " + (entry.Weight / 100f).ToString("0.##") + "%");
        if (result == null)
        {
            if (GUILayout.Button("Roll ×1", GUILayout.Height(45)))
            {
                var status = BeginRoll(Random.Range(0, GachaPool.TotalWeight));
                message = status == GachaResult.Success ? "Sorteando..." :
                    status == GachaResult.InsufficientFunds ? "Moedas insuficientes." : "Não foi possível realizar o Roll: " + status;
            }
        }
        else if (spinning)
        {
            DrawReel();
            GUILayout.Label("Sorteando...");
        }
        else
        {
            DrawReel();
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = RarityColor(result.Rarity);
            GUILayout.Box("VOCÊ RECEBEU\n" + result.PetName + "\n" + result.Rarity, GUILayout.ExpandWidth(true), GUILayout.Height(75));
            GUI.backgroundColor = previous;
            message = "Pet recebido! Abra o inventário com I após fechar.";
            if (GUILayout.Button("Continuar", GUILayout.Height(40))) { result = null; message = ""; }
        }
        GUILayout.Label(message);
        GUILayout.EndArea();
    }
}
