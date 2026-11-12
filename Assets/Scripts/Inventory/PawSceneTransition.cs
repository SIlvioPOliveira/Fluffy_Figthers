using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// Persistent overlay shared by scene changes and the shorter respawn transition.
public sealed class PawSceneTransition : MonoBehaviour
{
    private const int MaskWidth = 960;
    private const int MaskHeight = 540;
    private const float CloseSeconds = 1.05f;
    private const float LoadingSeconds = 4.0f;
    private const float PawStepSeconds = 0.28f;
    private const float OpenSeconds = 1.15f;
    private const float RespawnCloseSeconds = 0.45f;
    private const float RespawnOpenSeconds = 0.5f;
    private static bool running;
    public static bool IsActive => running;

    private RenderTexture mask;
    private Material maskMaterial;
    private Texture2D pawIcon;
    private PanelSettings settings;
    private VisualElement loading;
    private Image[] paws;
    private float previousTimeScale;

    public static bool Begin(string sceneName, ThemeStyleSheet theme, Font font)
    {
        if (running || !Application.CanStreamedLevelBeLoaded(sceneName)) return false;
        StartTransition(sceneName, null, theme, font);
        return true;
    }

    public static bool BeginRespawn(System.Action respawn, ThemeStyleSheet theme, Font font)
    {
        if (running || respawn == null || theme == null) return false;
        StartTransition(null, respawn, theme, font);
        return true;
    }

    private static void StartTransition(string sceneName, System.Action atBlack, ThemeStyleSheet theme, Font font)
    {
        running = true;
        var owner = new GameObject("TransicaoPatinha");
        DontDestroyOnLoad(owner);
        var transition = owner.AddComponent<PawSceneTransition>();
        transition.StartCoroutine(transition.Run(sceneName, atBlack, theme, font));
    }

    private IEnumerator Run(string sceneName, System.Action atBlack, ThemeStyleSheet theme, Font font)
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        bool overlayReady = false;
        try { CreateOverlay(theme, font); overlayReady = true; }
        catch (System.Exception exception) { Debug.LogException(exception); }
        if (!overlayReady)
        {
            Time.timeScale = previousTimeScale;
            if (atBlack != null) atBlack();
            else SceneManager.LoadScene(sceneName);
            Destroy(gameObject);
            yield break;
        }

        bool respawn = atBlack != null;
        float closeDuration = respawn ? RespawnCloseSeconds : CloseSeconds;
        float openDuration = respawn ? RespawnOpenSeconds : OpenSeconds;
        float duration = 0f;
        while (duration < closeDuration)
        {
            duration += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(duration / closeDuration));
            DrawMask(OpeningAt(1f - t));
            yield return null;
        }
        DrawMask(0f);
        // Show a completely black frame before allowing the scene to activate.
        yield return null;

        if (respawn)
        {
            try { atBlack(); }
            catch (System.Exception exception) { Debug.LogException(exception); }
            // Let the camera catch up to the new position before revealing it.
            yield return null;
        }
        else
        {
            loading.style.display = DisplayStyle.Flex;
            AsyncOperation operation = null;
            try { operation = SceneManager.LoadSceneAsync(sceneName); }
            catch (System.Exception exception) { Debug.LogException(exception); }
            float elapsed = 0f;
            while (operation != null && (!operation.isDone || elapsed < LoadingSeconds))
            {
                elapsed += Time.unscaledDeltaTime;
                AnimatePaws(elapsed);
                yield return null;
            }
            if (operation == null) Debug.LogError("Não foi possível carregar a cena " + sceneName);
            loading.style.display = DisplayStyle.None;
        }

        duration = 0f;
        while (duration < openDuration)
        {
            duration += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(duration / openDuration));
            DrawMask(OpeningAt(t));
            yield return null;
        }
        Time.timeScale = previousTimeScale;
        Destroy(gameObject);
    }

    private void CreateOverlay(ThemeStyleSheet theme, Font font)
    {
        settings = ScriptableObject.CreateInstance<PanelSettings>();
        settings.themeStyleSheet = theme;
        settings.sortingOrder = 1000;
        settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        settings.referenceResolution = new Vector2Int(1600, 900);
        var document = gameObject.AddComponent<UIDocument>();
        document.panelSettings = settings;
        var root = document.rootVisualElement;
        root.style.flexGrow = 1;
        if (font != null) root.style.unityFontDefinition = FontDefinition.FromFont(font);

        Shader shader = Resources.Load<Shader>("PawTransitionMask");
        if (shader == null || !shader.isSupported) throw new System.InvalidOperationException("Shader da transição indisponível.");
        maskMaterial = new Material(shader);
        mask = new RenderTexture(MaskWidth, MaskHeight, 0, RenderTextureFormat.ARGB32);
        mask.useMipMap = false;
        mask.Create();
        mask.wrapMode = TextureWrapMode.Clamp;
        mask.filterMode = FilterMode.Bilinear;
        DrawMask(MaxOpening());
        var image = new Image { image = mask, scaleMode = ScaleMode.StretchToFill };
        image.style.position = Position.Absolute;
        image.style.left = 0; image.style.right = 0; image.style.top = 0; image.style.bottom = 0;
        root.Add(image);

        loading = new VisualElement { pickingMode = PickingMode.Ignore };
        loading.style.position = Position.Absolute;
        loading.style.left = 0; loading.style.right = 0; loading.style.top = 0; loading.style.bottom = 0;
        loading.style.alignItems = Align.Center;
        loading.style.justifyContent = Justify.Center;
        loading.style.display = DisplayStyle.None;
        root.Add(loading);
        var orbit = new VisualElement { pickingMode = PickingMode.Ignore };
        orbit.style.width = 430;
        orbit.style.height = 200;
        orbit.style.alignItems = Align.Center;
        orbit.style.justifyContent = Justify.Center;
        loading.Add(orbit);
        var label = new Label("CARREGANDO...") { pickingMode = PickingMode.Ignore };
        label.style.color = new Color(1f, .94f, .78f);
        label.style.fontSize = 30;
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        label.style.unityTextAlign = TextAnchor.MiddleCenter;
        orbit.Add(label);
        pawIcon = MakePawIcon();
        paws = new Image[4];
        for (int i = 0; i < paws.Length; i++)
        {
            paws[i] = new Image { image = pawIcon, scaleMode = ScaleMode.ScaleToFit, pickingMode = PickingMode.Ignore };
            paws[i].style.position = Position.Absolute;
            paws[i].style.width = 34; paws[i].style.height = 34;
            orbit.Add(paws[i]);
        }
        AnimatePaws(0f);
    }

    private static float MaxOpening() => 4.5f * Mathf.Max(1f, .75f * Screen.width / Mathf.Max(1, Screen.height));

    // Distribute the visible part of the iris across more of the animation.
    private static float OpeningAt(float progress) => Mathf.Exp(Mathf.Log(1f + MaxOpening()) * progress) - 1f;

    private void DrawMask(float opening)
    {
        maskMaterial.SetFloat("_Opening", opening);
        maskMaterial.SetFloat("_Aspect", (float)Screen.width / Mathf.Max(1, Screen.height));
        Graphics.Blit(Texture2D.whiteTexture, mask, maskMaterial);
    }

    private static float PawCoverage(float x, float y, float edge)
    {
        float body = Ellipse(x, y, 0f, -.12f, .34f, .27f, edge);
        float toe1 = Ellipse(x, y, -.30f, .19f, .105f, .13f, edge);
        float toe2 = Ellipse(x, y, -.11f, .31f, .105f, .13f, edge);
        float toe3 = Ellipse(x, y, .11f, .31f, .105f, .13f, edge);
        float toe4 = Ellipse(x, y, .30f, .19f, .105f, .13f, edge);
        return Mathf.Max(body, Mathf.Max(Mathf.Max(toe1, toe2), Mathf.Max(toe3, toe4)));
    }

    private static float Ellipse(float x, float y, float cx, float cy, float rx, float ry, float edge)
    {
        float dx = (x - cx) / rx;
        float dy = (y - cy) / ry;
        float distance = Mathf.Sqrt(dx * dx + dy * dy);
        float blend = Mathf.InverseLerp(1f - edge * 3f, 1f + edge * 3f, distance);
        return 1f - Mathf.SmoothStep(0f, 1f, blend);
    }

    private static Texture2D MakePawIcon()
    {
        const int size = 64;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        var pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float coverage = PawCoverage((x + .5f) / size - .5f, (y + .5f) / size - .5f, .015f);
            pixels[y * size + x] = new Color32(255, 205, 89, (byte)(coverage * 255f));
        }
        texture.SetPixels32(pixels);
        texture.Apply(false, false);
        return texture;
    }

    private void AnimatePaws(float elapsed)
    {
        float progress = elapsed / PawStepSeconds;
        int step = Mathf.FloorToInt(progress) + paws.Length - 1;
        float fraction = progress - Mathf.Floor(progress);
        for (int i = 0; i < paws.Length; i++)
        {
            float angle = (step - i) * (2f * Mathf.PI / 12f) - Mathf.PI * .5f;
            paws[i].style.left = 215f + Mathf.Cos(angle) * 165f - 17f;
            paws[i].style.top = 100f + Mathf.Sin(angle) * 70f - 17f;
            paws[i].style.rotate = new Rotate(new Angle(angle * Mathf.Rad2Deg + 90f));
            paws[i].style.opacity = Mathf.Clamp01(1f - (i + fraction) / paws.Length);
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = previousTimeScale;
        running = false;
        if (mask != null) { mask.Release(); Destroy(mask); }
        if (maskMaterial != null) Destroy(maskMaterial);
        if (pawIcon != null) Destroy(pawIcon);
        if (settings != null) Destroy(settings);
    }
}
