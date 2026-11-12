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
#if UNITY_EDITOR
    [SerializeField, Min(1), Tooltip("Somente no Editor: moedas adicionadas ao pressionar M durante o teste.")]
    private int editorMoneyAmount = 1000;
#endif
    private LocalSaveStore saveStore;
    private ZoneBarrierTest barrier;
    private ZoneBarrierTest lavaBarrier;
    private ZoneBarrierTest iceBarrier;
    private bool savedFireUnlocked;
    private bool savedLavaUnlocked;
    private bool savedIceUnlocked;
    public bool FinalBossDefeated { get; private set; }
    public event System.Action FinalBossFirstDefeated;
    public int HighestUnlockedRegion => savedIceUnlocked ? 3 : savedLavaUnlocked ? 2 : savedFireUnlocked ? 1 : 0;
    private bool saveBlocked;
    private bool barrierRegistered;
    private string lastSavedJson;
    private string saveMessage;
    private float nextSave;
    public int TutorialStage { get; private set; }
    private bool tutorialEnemyDefeated;
    private int tutorialRollCount;
    private bool tutorialWaterBossDefeated;
    public PlayerInventory Inventory { get; private set; }
    public CurrencyWallet Wallet { get; private set; }
    public GameCatalog Catalog => catalog;
    public Transform Player => player;
    public ThirdPersonCamera CameraController => cameraController;
    public bool InventoryOpen { get; set; }
    public bool GachaOpen { get; set; }
    public bool BarrierOpen { get; set; }
    public bool PauseOpen { get; set; }
    public bool VictoryOpen { get; set; }
    public bool PersistenceEnabled => persistProgress;

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
                savedLavaUnlocked = data.version >= 5 && data.lavaUnlocked;
                savedIceUnlocked = data.version >= 5 && data.iceUnlocked;
                FinalBossDefeated = data.version >= 5 && data.finalBossDefeated;
                TutorialStage = data.version == 1 ? 0 : data.tutorialStage;
                tutorialEnemyDefeated = data.version >= 3 && data.tutorialEnemyDefeated;
                tutorialRollCount = data.version >= 3 ? data.tutorialRollCount : 0;
                tutorialWaterBossDefeated = data.version >= 4 && data.tutorialWaterBossDefeated;
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
        if (value.SourceRegion == RegionId.Water) { barrier = value; barrierRegistered = true; value.RestoreUnlocked(savedFireUnlocked); }
        else if (value.SourceRegion == RegionId.Fire) { lavaBarrier = value; value.RestoreUnlocked(savedLavaUnlocked); }
        else if (value.SourceRegion == RegionId.Lava) { iceBarrier = value; value.RestoreUnlocked(savedIceUnlocked); }
        ReconcileTutorial();
        SaveProgress();
    }

    private void OnBalanceChanged(int ignored)
    {
        if (barrier != null) savedFireUnlocked = barrier.IsUnlocked;
        if (lavaBarrier != null) savedLavaUnlocked = lavaBarrier.IsUnlocked;
        if (iceBarrier != null) savedIceUnlocked = iceBarrier.IsUnlocked;
        ReconcileTutorial();
        SaveProgress();
    }

    private void OnInventoryChanged()
    {
        ReconcileTutorial();
        SaveProgress();
    }

    private void ReconcileTutorial()
    {
        if (Inventory == null) return;
        int minimumPets = 0;
        foreach (InventoryEntry entry in Inventory.GetEntries())
        {
            // A fused pet represents at least three earlier pets; only 0, 1 or 2 matters here.
            minimumPets = Mathf.Min(2, minimumPets + (entry.Key.FusionLevel > 1 ? 2 : Mathf.Min(2, entry.Total)));
        }
        tutorialRollCount = Mathf.Max(tutorialRollCount, minimumPets);
        if (TutorialStage < 2 && tutorialRollCount > 0) TutorialStage = 2;
        for (int i = 0; i < PlayerInventory.MaxEquipment; i++)
            if (Inventory.GetSlot(i).HasValue) { TutorialStage = Mathf.Max(TutorialStage, 4); break; }
        if (TutorialStage == 4 && tutorialEnemyDefeated) TutorialStage = 5;
        if (TutorialStage == 5 && tutorialRollCount >= 2) TutorialStage = 6;
        if (TutorialStage == 6 && tutorialWaterBossDefeated) TutorialStage = 7;
        if (TutorialStage == 7 && savedFireUnlocked) TutorialStage = 8;
    }

    public void TutorialWaterBossDefeated()
    {
        tutorialWaterBossDefeated = true;
        ReconcileTutorial();
        SaveProgress();
    }

    public void RecordFinalBossDefeat()
    {
        if (FinalBossDefeated) return;
        FinalBossDefeated = true;
        SaveProgress();
        FinalBossFirstDefeated?.Invoke();
    }

    public void TutorialEnemyRewardGranted()
    {
        tutorialEnemyDefeated = true;
        ReconcileTutorial();
        SaveProgress();
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

    private void SaveProgress() => TrySaveProgress();

    public bool TrySaveProgress()
    {
        if (!persistProgress) return true;
        if (saveBlocked || !barrierRegistered || Inventory == null || Wallet == null || saveStore == null) return false;
        if (barrier != null) savedFireUnlocked = barrier.IsUnlocked;
        if (lavaBarrier != null) savedLavaUnlocked = lavaBarrier.IsUnlocked;
        if (iceBarrier != null) savedIceUnlocked = iceBarrier.IsUnlocked;
        PlayerSaveData data = PlayerSaveCodec.Capture(Wallet, Inventory, savedFireUnlocked, TutorialStage, tutorialEnemyDefeated, tutorialRollCount, tutorialWaterBossDefeated, savedLavaUnlocked, savedIceUnlocked, FinalBossDefeated);
        string json = JsonUtility.ToJson(data);
        if (json == lastSavedJson) return true;
        if (saveStore.Save(data, out string error)) { lastSavedJson = json; saveMessage = null; return true; }
        saveMessage = "Não foi possível salvar o progresso."; Debug.LogError(saveMessage + " " + error, this); return false;
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
#if UNITY_EDITOR
        // The regular wallet event refreshes the HUD and saves when persistence is enabled.
        if (!PawSceneTransition.IsActive && Wallet != null && !PauseOpen && !VictoryOpen && Input.GetKeyDown(KeyCode.M))
            GrantEditorMoney();
#endif
        if (PawSceneTransition.IsActive || InventoryOpen || GachaOpen || BarrierOpen || PauseOpen || VictoryOpen) return;
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

#if UNITY_EDITOR
    private void GrantEditorMoney() => Wallet.TryAdd(Mathf.Max(1, editorMoneyAmount));
#endif

    private void OnGUI()
    {
        if (PawSceneTransition.IsActive) return;
        if (GetComponent<PlayerHudPrototype>() == null && !InventoryOpen && !GachaOpen && !BarrierOpen) GUI.Label(new Rect(15, 15, 900, 30), "Teste: I abre inventário; 1–3 alternam Pets; WASD movimenta.");
        if (!string.IsNullOrEmpty(saveMessage)) GUI.Box(new Rect(15, 105, 650, 35), saveMessage);
    }
}
