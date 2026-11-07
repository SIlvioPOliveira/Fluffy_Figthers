using UnityEngine;

// A dedicated test-scene driver, not the new-game inventory or the final UI.
public sealed class PetEquipmentTestDriver : MonoBehaviour
{
    [SerializeField] private GameCatalog catalog;
    [SerializeField] private PetEquipmentView equipmentView;
    [SerializeField] private Transform player;
    [SerializeField] private ThirdPersonCamera cameraController;
    [SerializeField] private bool seedTestPets = true;
    [SerializeField] private string quickEquipPetId = "water_peixinho";
    [SerializeField] private bool persistProgress;
    [SerializeField, Min(1f)] private float autosaveInterval = 30f;
    private LocalSaveStore saveStore;
    private ZoneBarrierTest barrier;
    private bool savedFireUnlocked;
    private bool saveBlocked;
    private bool barrierRegistered;
    private string lastSavedJson;
    private string saveMessage;
    private float nextSave;
    public PlayerInventory Inventory { get; private set; }
    public CurrencyWallet Wallet { get; private set; }
    public GameCatalog Catalog => catalog;
    public Transform Player => player;
    public ThirdPersonCamera CameraController => cameraController;
    public bool InventoryOpen { get; set; }
    public bool GachaOpen { get; set; }
    public bool BarrierOpen { get; set; }

    private void Awake()
    {
        if (catalog == null || equipmentView == null || player == null)
        {
            Debug.LogError("Pet equipment test scene is missing references.", this);
            enabled = false;
            return;
        }
        if (persistProgress)
        {
            saveStore = new LocalSaveStore(System.IO.Path.Combine(Application.persistentDataPath, "fluffy_fighters_save.json"), catalog);
            SaveLoadStatus status = saveStore.Load(out PlayerSaveData data);
            if ((status == SaveLoadStatus.Loaded || status == SaveLoadStatus.Recovered) && PlayerSaveCodec.TryRestore(data, catalog, out CurrencyWallet restoredWallet, out PlayerInventory restoredInventory))
            {
                Wallet = restoredWallet;
                Inventory = restoredInventory;
                savedFireUnlocked = data.fireUnlocked;
                lastSavedJson = JsonUtility.ToJson(data);
                if (status == SaveLoadStatus.Recovered) Debug.LogWarning("Progresso recuperado do backup.", this);
            }
            else if (status != SaveLoadStatus.Missing)
            {
                saveBlocked = true;
                saveMessage = "Salvamento inválido: gravação bloqueada para proteger seus arquivos.";
                Debug.LogError(saveMessage, this);
            }
        }
        if (Inventory == null) Inventory = new PlayerInventory(catalog);
        if (Wallet == null) Wallet = new CurrencyWallet();
        equipmentView.Initialize(Inventory, catalog, player);
        if (seedTestPets && !persistProgress)
        {
            Inventory.TryAdd("water_peixinho", 1, 8);
            Inventory.TryAdd("water_tartaruga", 1, 3);
        }
        if (persistProgress)
        {
            Inventory.Changed += SaveProgress;
            Wallet.BalanceChanged += OnBalanceChanged;
            nextSave = Time.unscaledTime + Mathf.Max(1f, autosaveInterval);
        }
    }

    public void RegisterBarrier(ZoneBarrierTest value)
    {
        if (!persistProgress || value == null) return;
        barrier = value;
        barrierRegistered = true;
        barrier.RestoreUnlocked(savedFireUnlocked);
        SaveProgress();
    }

    private void OnBalanceChanged(int ignored) => SaveProgress();

    private void SaveProgress()
    {
        if (!persistProgress || saveBlocked || !barrierRegistered || Inventory == null || Wallet == null) return;
        if (barrier != null) savedFireUnlocked = barrier.IsUnlocked;
        PlayerSaveData data = PlayerSaveCodec.Capture(Wallet, Inventory, savedFireUnlocked);
        string json = JsonUtility.ToJson(data);
        if (json == lastSavedJson) return;
        if (saveStore.Save(data, out string error)) { lastSavedJson = json; saveMessage = null; }
        else { saveMessage = "Não foi possível salvar o progresso."; Debug.LogError(saveMessage + " " + error, this); }
    }

    private void OnApplicationQuit() => SaveProgress();

    private void OnDestroy()
    {
        SaveProgress();
        if (Inventory != null) Inventory.Changed -= SaveProgress;
        if (Wallet != null) Wallet.BalanceChanged -= OnBalanceChanged;
    }

    private void Update()
    {
        if (persistProgress && Time.unscaledTime >= nextSave)
        {
            SaveProgress();
            nextSave = Time.unscaledTime + Mathf.Max(1f, autosaveInterval);
        }
        if (InventoryOpen || GachaOpen || BarrierOpen) return;
        for (int i = 0; i < PlayerInventory.MaxEquipment; i++)
        {
            if (!Input.GetKeyDown(KeyCode.Alpha1 + i)) continue;
            if (Inventory.GetSlot(i).HasValue) Inventory.TryUnequip(i);
            else if (!Inventory.TryEquip(quickEquipPetId, 1, i))
            {
                // In empty-start scenes, the first Gacha reward can be any rarity.
                foreach (InventoryEntry entry in Inventory.GetEntries())
                    if (entry.Available > 0 && Inventory.TryEquip(entry.Key.PetId, entry.Key.FusionLevel, i)) break;
            }
        }
    }

    private void OnGUI()
    {
        if (!InventoryOpen && !GachaOpen && !BarrierOpen) GUI.Label(new Rect(15, 15, 900, 30), "Teste: I abre inventário; 1–3 alternam Pets; WASD movimenta.");
        if (!string.IsNullOrEmpty(saveMessage)) GUI.Box(new Rect(15, 105, 650, 35), saveMessage);
    }
}
