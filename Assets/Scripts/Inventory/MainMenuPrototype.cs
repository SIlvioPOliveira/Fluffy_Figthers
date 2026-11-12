using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// Temporary front end; the final visual identity will replace this presentation.
public sealed class MainMenuPrototype : MonoBehaviour
{
    [SerializeField] private ThemeStyleSheet theme;
    [SerializeField] private Font displayFont;
    [SerializeField] private Texture2D logo;
    private MenuCartoonView cartoon;
    private string message;
    private GUIStyle titleStyle;
    private GUIStyle subtitleStyle;
    private GUIStyle buttonStyle;

    private void Start()
    {
        if (theme != null && logo != null)
            cartoon = new MenuCartoonView(transform, theme, displayFont, logo, false, () => Play(), Quit);
    }
    private void Update() { if (cartoon != null) cartoon.SetMessage(message); }
    private void OnDestroy() { if (cartoon != null) cartoon.Dispose(); }

    public bool Play()
    {
        if (!Application.CanStreamedLevelBeLoaded("RegiaoAguaPrototipo"))
        {
            message = "Cena da região Água indisponível.";
            return false;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene("RegiaoAguaPrototipo");
        return true;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnGUI()
    {
        if (cartoon != null) return;
        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 42, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            subtitleStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 19, fontStyle = FontStyle.Bold };
        }
        float width = Mathf.Min(520f, Screen.width - 40f);
        float height = 300f;
        Rect panel = new Rect((Screen.width - width) * .5f, (Screen.height - height) * .5f, width, height);
        GUI.Box(panel, GUIContent.none);
        GUI.Label(new Rect(panel.x + 10, panel.y + 16, width - 20, 85), "FLUFFY FIGHTERS", titleStyle);
        GUI.Label(new Rect(panel.x + 10, panel.y + 96, width - 20, 34), "Uma aventura com seus pets", subtitleStyle);
        float buttonWidth = Mathf.Min(280f, width - 40f);
        float x = panel.center.x - buttonWidth * .5f;
        if (GUI.Button(new Rect(x, panel.y + 145, buttonWidth, 48), "JOGAR", buttonStyle)) Play();
        if (GUI.Button(new Rect(x, panel.y + 205, buttonWidth, 48), "SAIR", buttonStyle)) Quit();
        GUI.Label(new Rect(panel.x + 10, panel.y + 257, width - 20, 35), message ?? "", subtitleStyle);
    }
}
