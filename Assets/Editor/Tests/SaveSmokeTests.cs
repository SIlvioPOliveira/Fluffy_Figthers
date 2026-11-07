using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Run with Unity -batchmode -executeMethod SaveSmokeTests.Run. Uses only project Temp.
public static class SaveSmokeTests
{
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    public static void Run()
    {
        var catalog = AssetDatabase.LoadAssetAtPath<GameCatalog>("Assets/Data/GameCatalog.asset");
        Require(catalog != null, "Catálogo ausente.");
        string dir = Path.Combine(Directory.GetCurrentDirectory(), "Temp", "SaveSmokeTests", Guid.NewGuid().ToString("N"));
        string path = Path.Combine(dir, "save.json");
        var store = new LocalSaveStore(path, catalog);
        Require(store.Load(out _) == SaveLoadStatus.Missing, "Novo arquivo não identificado.");

        var wallet = new CurrencyWallet(250);
        var inventory = new PlayerInventory(catalog);
        Require(inventory.TryAdd("water_peixinho", 1, 5), "Adicionar falhou.");
        Require(inventory.TryEquip("water_peixinho", 1, 0) && inventory.TryEquip("water_peixinho", 1, 2), "Equipar duplicatas falhou.");
        Require(inventory.TryFuse("water_peixinho", 1), "Fusão falhou.");
        var first = PlayerSaveCodec.Capture(wallet, inventory, true, 3);
        Require(store.Save(first, out string error), "Primeira gravação falhou: " + error);
        Require(store.Load(out var loaded) == SaveLoadStatus.Loaded, "Leitura falhou.");
        Require(PlayerSaveCodec.TryRestore(loaded, catalog, out var restoredWallet, out var restored), "Restauração falhou.");
        Require(restoredWallet.Balance == 250 && loaded.fireUnlocked && loaded.tutorialStage == 3 && restored.GetEntry(new PetStackKey("water_peixinho", 1)).Total == 2 && restored.GetEntry(new PetStackKey("water_peixinho", 1)).Equipped == 2 && restored.GetEntry(new PetStackKey("water_peixinho", 2)).Total == 1 && !restored.GetSlot(1).HasValue, "Conteúdo restaurado incorreto.");
        var legacy = PlayerSaveCodec.Capture(wallet, inventory, true);
        legacy.version = 1;
        Require(PlayerSaveCodec.TryRestore(legacy, catalog, out _, out _), "Salvamento anterior não foi aceito.");
        legacy.version = PlayerSaveCodec.CurrentVersion;
        legacy.tutorialStage = 5;
        Require(!PlayerSaveCodec.TryRestore(legacy, catalog, out _, out _), "Etapa inválida foi aceita.");

        // Exercise the tutorial without opening or modifying the player's actual save.
        var testObject = new GameObject("TutorialSmokeTest");
        try
        {
            var tutorial = testObject.AddComponent<PetEquipmentTestDriver>();
            var testInventory = new PlayerInventory(catalog);
            typeof(PetEquipmentTestDriver).GetProperty("Inventory").GetSetMethod(true).Invoke(tutorial, new object[] { testInventory });
            var changed = typeof(PetEquipmentTestDriver).GetMethod("OnInventoryChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Require(tutorial.TutorialStage == 0, "Tutorial não começou na máquina.");
            tutorial.TutorialMachineOpened();
            Require(tutorial.TutorialStage == 1, "Abrir máquina não avançou.");
            Require(testInventory.TryAdd("water_peixinho"), "Pet do tutorial não foi adicionado.");
            changed.Invoke(tutorial, null);
            Require(tutorial.TutorialStage == 2, "Primeiro Pet não avançou.");
            tutorial.TutorialInventoryOpened();
            Require(tutorial.TutorialStage == 3, "Abrir inventário não avançou.");
            Require(testInventory.TryEquip("water_peixinho", 1, 0), "Pet do tutorial não foi equipado.");
            changed.Invoke(tutorial, null);
            Require(tutorial.TutorialStage == 4, "Equipar não avançou.");
        }
        finally { UnityEngine.Object.DestroyImmediate(testObject); }

        wallet.TryAdd(20);
        Require(store.Save(PlayerSaveCodec.Capture(wallet, inventory, true), out error), "Segunda gravação falhou: " + error);
        Require(File.Exists(store.BackupPath), "Backup anterior ausente.");
        File.WriteAllText(path, "{corrompido");
        Require(store.Load(out var backup) == SaveLoadStatus.Recovered && backup.coins == 250, "Recuperação do backup falhou.");
        Require(store.Save(backup, out error), "Regravação após recuperação falhou: " + error);
        Require(store.Load(out var repaired) == SaveLoadStatus.Loaded && repaired.coins == 250, "Arquivo recuperado incorreto.");
        Require(File.ReadAllText(store.BackupPath).Contains("water_peixinho"), "Backup válido foi sobrescrito por dados corrompidos.");

        var invalid = PlayerSaveCodec.Capture(wallet, inventory, false);
        invalid.slots[1] = new PetSlotSave { petId = "water_peixinho", level = 1 };
        Require(!PlayerSaveCodec.TryRestore(invalid, catalog, out _, out _), "Equipamento maior que a quantidade foi aceito.");
        invalid = PlayerSaveCodec.Capture(wallet, inventory, false);
        invalid.pets.Add(new PetStackSave { petId = "water_peixinho", level = 1, quantity = 1 });
        Require(!PlayerSaveCodec.TryRestore(invalid, catalog, out _, out _) && !store.Save(invalid, out _), "Entrada duplicada foi aceita.");
        invalid = PlayerSaveCodec.Capture(wallet, inventory, false);
        invalid.pets[0].petId = "id_inexistente";
        Require(!PlayerSaveCodec.TryRestore(invalid, catalog, out _, out _), "Pet desconhecido foi aceito.");
        File.WriteAllText(path, "{inválido");
        File.WriteAllText(store.BackupPath, "{inválido");
        Require(store.Load(out _) == SaveLoadStatus.Invalid, "Arquivos inválidos foram aceitos.");

        var scene = EditorSceneManager.OpenScene("Assets/Scenes/RegiaoAguaPrototipo.unity");
        var driver = UnityEngine.Object.FindFirstObjectByType<PetEquipmentTestDriver>();
        var gate = UnityEngine.Object.FindFirstObjectByType<ZoneBarrierTest>();
        Require(scene.IsValid() && driver != null && gate != null && new SerializedObject(driver).FindProperty("persistProgress").boolValue, "Cena da Água sem salvamento ou barreira.");
        Debug.Log("SAVE_SMOKE_TESTS_PASSED");
    }
}
