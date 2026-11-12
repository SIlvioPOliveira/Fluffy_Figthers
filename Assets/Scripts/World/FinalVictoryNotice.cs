using UnityEngine;

[RequireComponent(typeof(PetEquipmentTestDriver))]
public sealed class FinalVictoryNotice : MonoBehaviour
{
    private PetEquipmentTestDriver driver;
    private bool pending;
    private bool oldMovement;
    private bool oldCamera;
    private VictoryCartoonUI cartoonUI;

    private void Start()
    {
        driver = GetComponent<PetEquipmentTestDriver>();
        driver.FinalBossFirstDefeated += OnVictory;
    }

    private void OnVictory() => pending = true;

#if UNITY_EDITOR
    [ContextMenu("Teste: mostrar tela de vitória")]
    private void PreviewVictoryInEditor()
    {
        if (Application.isPlaying && driver != null && !driver.VictoryOpen) pending = true;
    }
#endif

    private void Update()
    {
        if (cartoonUI != null) cartoonUI.Tick();
        if (!pending || driver.InventoryOpen || driver.GachaOpen || driver.BarrierOpen || driver.PauseOpen) return;
        pending = false;
        driver.VictoryOpen = true;
        var movement = driver.Player.GetComponent<PlayerMovement>();
        oldMovement = movement != null && movement.ControlsEnabled;
        oldCamera = driver.CameraController != null && driver.CameraController.ControlsEnabled;
        if (movement != null) movement.ControlsEnabled = false;
        if (driver.CameraController != null) driver.CameraController.ControlsEnabled = false;
        var inventory = driver.GetComponent<InventoryCanvasPrototype>();
        if (inventory != null && inventory.isActiveAndEnabled && inventory.Theme != null)
            cartoonUI = new VictoryCartoonUI(transform, inventory.Theme, inventory.DisplayFont, Close);
    }

    private void Close()
    {
        if (driver == null || !driver.VictoryOpen) return;
        driver.VictoryOpen = false;
        if (cartoonUI != null) cartoonUI.Dispose();
        cartoonUI = null;
        var movement = driver.Player.GetComponent<PlayerMovement>();
        if (movement != null) movement.ControlsEnabled = oldMovement;
        if (driver.CameraController != null) driver.CameraController.ControlsEnabled = oldCamera;
    }

    private void OnDisable() => Close();

    private void OnDestroy()
    {
        Close();
        if (driver != null) driver.FinalBossFirstDefeated -= OnVictory;
    }

    private void OnGUI()
    {
        if (driver == null || !driver.VictoryOpen) return;
        if (cartoonUI != null) return;
        int oldDepth = GUI.depth;
        GUI.depth = -1000;
        float width = Mathf.Min(520f, Screen.width - 30f);
        GUILayout.BeginArea(new Rect((Screen.width - width) / 2, (Screen.height - 180) / 2, width, 180), GUI.skin.box);
        GUILayout.Label("PARABÉNS! VOCÊ CONQUISTOU TODAS AS REGIÕES DE FLUFFY FIGHTERS!");
        GUILayout.Label("Você pode continuar explorando e fortalecendo sua equipe.");
        if (GUILayout.Button("Continuar", GUILayout.Height(42))) Close();
        GUILayout.EndArea();
        GUI.depth = oldDepth;
    }
}
