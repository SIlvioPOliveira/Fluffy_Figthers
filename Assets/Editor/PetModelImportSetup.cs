using System;
using System.IO;
using UnityEditor;
using UnityEngine;

// Creates wrappers for the provided meshes and connects them to the existing catalog.
public static class PetModelImportSetup
{
    private static readonly string[] Ids = {
        "water_peixinho", "water_tartaruga", "water_polvinho", "water_golfinho", "water_dragao_marinho",
        "fire_gatinho", "fire_raposa", "fire_fenix", "fire_leao", "fire_dragao",
        "lava_pedrinha", "lava_caranguejo", "lava_salamandra", "lava_urso", "lava_dragao",
        "ice_pinguinzinho", "ice_raposa", "ice_lobo", "ice_urso", "ice_dragao"
    };
    private const string Models = "Assets/Art/Pets/ModelosProntos";
    private const string Prefabs = "Assets/Art/Pets/Prefabs";

    public static string Configure()
    {
        if (AssetDatabase.IsValidFolder(Prefabs)) throw new Exception("A pasta de prefabs já existe; não será sobrescrita.");
        foreach (string id in Ids)
        {
            var pet = AssetDatabase.LoadAssetAtPath<PetData>("Assets/Data/Pets/" + id + ".asset");
            string[] models = AssetDatabase.FindAssets("t:Model", new[] { Models + "/" + id });
            if (pet == null || pet.Prefab != null || models.Length != 1)
                throw new Exception("Verifique dados, prefab existente ou FBX para " + id);
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(models[0]));
            var renderers = model != null ? model.GetComponentsInChildren<Renderer>(true) : Array.Empty<Renderer>();
            if (renderers.Length == 0 || renderers[0].sharedMaterial == null || renderers[0].sharedMaterial.mainTexture == null)
                throw new Exception("Modelo ou textura ausente: " + id);
        }
        AssetDatabase.CreateFolder("Assets/Art/Pets", "Prefabs");
        foreach (string id in Ids)
        {
            string[] models = AssetDatabase.FindAssets("t:Model", new[] { Models + "/" + id });
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(models[0]));
            var wrapper = new GameObject("Pet_" + id);
            try
            {
                var visual = (GameObject)PrefabUtility.InstantiatePrefab(model);
                visual.transform.SetParent(wrapper.transform, false);
                visual.transform.localPosition = new Vector3(0f, -.35f, 0f);
                visual.transform.localScale = Vector3.one * .8f;
                string path = Prefabs + "/" + id + ".prefab";
                var prefab = PrefabUtility.SaveAsPrefabAsset(wrapper, path);
                if (prefab == null) throw new Exception("Falha ao criar prefab " + id);
                var pet = AssetDatabase.LoadAssetAtPath<PetData>("Assets/Data/Pets/" + id + ".asset");
                var serialized = new SerializedObject(pet);
                serialized.FindProperty("prefab").objectReferenceValue = prefab;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(pet);
            }
            finally { UnityEngine.Object.DestroyImmediate(wrapper); }
        }
        AssetDatabase.SaveAssets();
        return "20 prefabs criados e associados ao catálogo.";
    }
}
