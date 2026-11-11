using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// One off-screen camera renders the selected model and cached frontal thumbnails.
// Preview objects live far outside the playable map and are excluded from collisions.
public sealed class PetInventoryPreview : IDisposable
{
    private const int PreviewLayer = 31;
    private const int ImageSize = 256;
    private const float TurnDegreesPerSecond = 45f;
    private static readonly Vector3 Origin = new Vector3(10000f, 10000f, 10000f);
    private readonly Dictionary<string, Texture2D> thumbnails = new Dictionary<string, Texture2D>();
    private readonly HashSet<string> queued = new HashSet<string>();
    private readonly Queue<PetData> pending = new Queue<PetData>();
    private GameObject cameraObject;
    private GameObject keyLight;
    private GameObject fillLight;
    private Camera camera;
    private RenderTexture target;
    private GameObject selectedModel;
    private PetData selectedPet;
    private float angle;

    public Texture SelectedImage => selectedModel != null ? target : null;

    public PetInventoryPreview()
    {
        cameraObject = new GameObject("InventoryPreviewCamera");
        camera = cameraObject.AddComponent<Camera>();
        camera.enabled = false;
        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        camera.cullingMask = 1 << PreviewLayer;
        camera.nearClipPlane = .01f;
        camera.farClipPlane = 100f;
        camera.allowHDR = false;
        camera.allowMSAA = false;
        target = new RenderTexture(ImageSize, ImageSize, 24, RenderTextureFormat.ARGB32);
        target.name = "InventoryPetPreview";
        target.Create();
        camera.targetTexture = target;
        keyLight = CreateLight("InventoryPreviewKey", Quaternion.Euler(35f, 145f, 0f), 1.15f);
        fillLight = CreateLight("InventoryPreviewFill", Quaternion.Euler(15f, -45f, 0f), .65f);
    }

    private static GameObject CreateLight(string name, Quaternion rotation, float intensity)
    {
        var go = new GameObject(name);
        go.transform.rotation = rotation;
        var light = go.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = intensity;
        light.cullingMask = 1 << PreviewLayer;
        light.shadows = LightShadows.None;
        return go;
    }

    public Texture Thumbnail(PetData pet)
    {
        if (pet == null) return null;
        if (thumbnails.TryGetValue(pet.PetId, out var image)) return image;
        if (queued.Add(pet.PetId)) pending.Enqueue(pet);
        return pet.Icon != null ? pet.Icon.texture : null;
    }

    public void Select(PetData pet)
    {
        if (selectedPet == pet) return;
        if (selectedModel != null) { selectedModel.SetActive(false); UnityEngine.Object.Destroy(selectedModel); }
        selectedModel = null;
        selectedPet = pet;
        angle = 0f;
        if (pet != null) selectedModel = CreateModel(pet);
    }

    public void Tick(float deltaTime)
    {
        // Limit readbacks to one model per frame rather than rendering all 20 at once.
        if (pending.Count > 0)
        {
            PetData pet = pending.Dequeue();
            if (pet != null && !thumbnails.ContainsKey(pet.PetId)) CaptureThumbnail(pet);
        }
        if (selectedModel == null) return;
        angle = (angle + TurnDegreesPerSecond * Mathf.Max(0f, deltaTime)) % 360f;
        selectedModel.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        Frame(selectedModel);
        camera.Render();
    }

    private GameObject CreateModel(PetData pet)
    {
        GameObject model = pet.Prefab != null ? UnityEngine.Object.Instantiate(pet.Prefab) : GameObject.CreatePrimitive(PrimitiveType.Sphere);
        model.name = "InventoryPreview_" + pet.PetId;
        model.transform.position = Origin;
        model.transform.rotation = Quaternion.identity;
        foreach (Transform node in model.GetComponentsInChildren<Transform>(true)) node.gameObject.layer = PreviewLayer;
        foreach (Collider collider in model.GetComponentsInChildren<Collider>(true)) collider.enabled = false;
        // Some imported models contain their own lights; they must not illuminate previews or the world.
        foreach (Light light in model.GetComponentsInChildren<Light>(true)) light.enabled = false;
        foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>(true))
        {
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }
        return model;
    }

    private void Frame(GameObject model)
    {
        var renderers = model.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) return;
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
        float diameter = Mathf.Max(bounds.size.y, Mathf.Sqrt(bounds.size.x * bounds.size.x + bounds.size.z * bounds.size.z));
        camera.orthographicSize = Mathf.Max(.1f, diameter * .64f);
        camera.transform.position = bounds.center + Vector3.forward * 4f;
        camera.transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
    }

    private void CaptureThumbnail(PetData pet)
    {
        if (selectedModel != null) selectedModel.SetActive(false);
        GameObject clone = CreateModel(pet);
        try
        {
            Frame(clone);
            camera.Render();
            var previous = RenderTexture.active;
            try
            {
                RenderTexture.active = target;
                var image = new Texture2D(ImageSize, ImageSize, TextureFormat.RGBA32, false);
                image.name = "InventoryThumbnail_" + pet.PetId;
                image.ReadPixels(new Rect(0, 0, ImageSize, ImageSize), 0, 0);
                image.Apply(false, false);
                thumbnails.Add(pet.PetId, image);
            }
            finally { RenderTexture.active = previous; }
        }
        finally
        {
            clone.SetActive(false);
            UnityEngine.Object.Destroy(clone);
            if (selectedModel != null) selectedModel.SetActive(true);
        }
    }

    public void Dispose()
    {
        if (selectedModel != null) UnityEngine.Object.Destroy(selectedModel);
        foreach (var image in thumbnails.Values) if (image != null) UnityEngine.Object.Destroy(image);
        thumbnails.Clear();
        queued.Clear();
        pending.Clear();
        if (camera != null) camera.targetTexture = null;
        if (target != null) { target.Release(); UnityEngine.Object.Destroy(target); }
        if (keyLight != null) UnityEngine.Object.Destroy(keyLight);
        if (fillLight != null) UnityEngine.Object.Destroy(fillLight);
        if (cameraObject != null) UnityEngine.Object.Destroy(cameraObject);
    }
}
