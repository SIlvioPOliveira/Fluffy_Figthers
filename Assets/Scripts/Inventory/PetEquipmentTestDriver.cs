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
    [SerializeField, Tooltip("Somente no Editor: usa um salvamento separado para testar o começo do tutorial.")]
    private bool editorTutorialTestSave;
    [SerializeField, Min(1f)] private float autosaveInterval = 30f;
    private LocalSaveStore saveStore;
    private ZoneBarrierTest barrier;
    private bool savedFireUnlocked;
    private bool saveBlocked;
    private bool barrierRegistered;
    private string lastSavedJson;
    private string saveMessage;
    private float nextSave;
    public int TutorialStage { get; private set; }
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
            // A separate test file lets tutorial playtests start fresh without touching real progress.
            bool tutorialTest = System.Array.IndexOf(System.Environment.GetCommandLineArgs(), "--tutorial-teste") >= 0;
            string saveName = tutorialTest ? "fluffy_fighters_tutorial_test.json" : "fluffy_fighters_save.json";
#if UNITY_EDITOR
            if (editorTutorialTestSave) saveName = "fluffy_fighters_editor_tutorial_test.json";
#endif
            saveStore = new LocalSaveStore(System.IO.Path.Combine(Application.persistentDataPath, saveName), catalog);
            SaveLoadStatus status = saveStore.Load(out PlayerSaveData data);
            if ((status == SaveLoadStatus.Loaded || status == SaveLoadStatus.Recovered) && PlayerSaveCodec.TryRestore(data, catalog, out CurrencyWallet restoredWallet, out PlayerInventory restoredInventory))
            {
                Wallet = restoredWallet;
                Inventory = restoredInventory;
                savedFireUnlocked = data.fireUnlocked;
                TutorialStage = data.version == 1 ? 0 : data.tutorialStage;
                ReconcileTutorial();
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
            Inventory.Changed += OnInventoryChanged;
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

    private void OnInventoryChanged()
    {
        ReconcileTutorial();
        SaveProgress();
    }

    private void ReconcileTutorial()
    {
        if (Inventory == null) return;
        if (TutorialStage < 2 && Inventory.GetEntries().Count > 0) TutorialStage = 2;
        for (int i = 0; i < PlayerInventory.MaxEquipment; i++)
            if (Inventory.GetSlot(i).HasValue) { TutorialStage = Mathf.Max(TutorialStage, 4); break; }
    }

    public void TutorialMachineOpened()
    {
        if (TutorialStage != 0) return;
        TutorialStage = 1;
        SaveProgress();
    }

    public void TutorialInventoryOpened()
    {
        if (TutorialStage != 2) return;
        TutorialStage = 3;
        SaveProgress();
    }

    private void SaveProgress()
    {
        if (!persistProgress || saveBlocked || !barrierRegistered || Inventory == null || Wallet == null) return;
        if (barrier != null) savedFireUnlocked = barrier.IsUnlocked;
        PlayerSaveData data = PlayerSaveCodec.Capture(Wallet, Inventory, savedFireUnlocked, TutorialStage);
        string json = JsonUtility.ToJson(data);
        if (json == lastSavedJson) return;
        if (saveStore.Save(data, out string error)) { lastSavedJson = json; saveMessage = null; }
        else { saveMessage = "Não foi possível salvar o progresso."; Debug.LogError(saveMessage + " " + error, this); }
    }

    private void OnApplicationQuit() => SaveProgress();

    private void OnDestroy()
    {
        SaveProgress();
        if (Inventory != null) Inventory.Changed -= OnInventoryChanged;
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
        if (GetComponent<PlayerHudPrototype>() == null && !InventoryOpen && !GachaOpen && !BarrierOpen) GUI.Label(new Rect(15, 15, 900, 30), "Teste: I abre inventário; 1–3 alternam Pets; WASD movimenta.");
        if (!string.IsNullOrEmpty(saveMessage)) GUI.Box(new Rect(15, 105, 650, 35), saveMessage);
    }
}
