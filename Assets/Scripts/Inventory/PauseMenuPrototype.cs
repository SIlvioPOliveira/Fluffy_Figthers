using UnityEngine;
using UnityEngine.SceneManagement;

// Temporary pause menu for the Water prototype.
[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(PetEquipmentTestDriver))]
public sealed class PauseMenuPrototype : MonoBehaviour
{
    private PetEquipmentTestDriver driver;
    private PlayerMovement movement;
    private float previousTimeScale;
    private bool previousMovement;
    private bool previousCamera;
    private bool previousCursorVisible;
    private CursorLockMode previousCursorLock;
    private string message;

    private void Start()
    {
        driver = GetComponent<PetEquipmentTestDriver>();
        movement = driver.Player != null ? driver.Player.GetComponent<PlayerMovement>() : null;
    }

    private void Update()
    {
        if (driver == null || !Input.GetKeyDown(KeyCode.Escape)) return;
        if (driver.PauseOpen) Resume();
        else TryPause();
    }

    public bool TryPause()
    {
        if (driver == null || driver.PauseOpen || driver.InventoryOpen || driver.GachaOpen || driver.BarrierOpen || driver.VictoryOpen) return false;
        previousTimeScale = Time.timeScale;
        previousMovement = movement != null && movement.ControlsEnabled;
        previousCamera = driver.CameraController != null && driver.CameraController.ControlsEnabled;
        previousCursorVisible = Cursor.visible;
        previousCursorLock = Cursor.lockState;
        driver.PauseOpen = true;
        Time.timeScale = 0f;
        if (movement != null) movement.ControlsEnabled = false;
        if (driver.CameraController != null) driver.CameraController.ControlsEnabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        message = "";
        return true;
    }

    public void Resume()
    {
        if (driver == null || !driver.PauseOpen) return;
        driver.PauseOpen = false;
        Time.timeScale = previousTimeScale;
        if (movement != null) movement.ControlsEnabled = previousMovement;
        if (driver.CameraController != null) driver.CameraController.ControlsEnabled = previousCamera;
        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;
    }

    private void OnDisable() => Resume();

    private void ExitGame()
    {
        if (!driver.TrySaveProgress())
        {
            message = "Não foi possível salvar. Continue no jogo e tente novamente.";
            return;
        }
        Resume();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public bool ReturnToMainMenu()
    {
        if (driver == null || !driver.PauseOpen) return false;
        if (!Application.CanStreamedLevelBeLoaded("MenuInicial"))
        {
            message = "Menu inicial indisponível.";
            return false;
        }
        if (!driver.TrySaveProgress())
        {
            message = "Não foi possível salvar. Continue no jogo e tente novamente.";
            return false;
        }
        Resume();
        SceneManager.LoadScene("MenuInicial");
        return true;
    }

    private void OnGUI()
    {
        if (driver == null || !driver.PauseOpen) return;
        int oldDepth = GUI.depth;
        Color oldColor = GUI.color;
        GUI.depth = -1000;
        GUI.color = new Color(0, 0, 0, .65f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = oldColor;
        float width = Mathf.Min(400, Screen.width - 30);
        GUILayout.BeginArea(new Rect((Screen.width - width) / 2, (Screen.height - 260) / 2, width, 260), GUI.skin.box);
        GUILayout.Label("JOGO PAUSADO");
        if (GUILayout.Button("Continuar (Esc)", GUILayout.Height(42))) Resume();
        if (GUILayout.Button("Salvar e voltar ao menu inicial", GUILayout.Height(42))) ReturnToMainMenu();
        if (GUILayout.Button("Salvar e sair", GUILayout.Height(42))) ExitGame();
        GUILayout.Label(message);
        GUILayout.EndArea();
        GUI.depth = oldDepth;
    }
}
