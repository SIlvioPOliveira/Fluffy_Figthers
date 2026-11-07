using System;
using System.IO;
using System.Text;
using UnityEngine;

public enum SaveLoadStatus { Missing, Loaded, Recovered, Invalid }

// One local slot. A failed or invalid load is never silently overwritten.
public sealed class LocalSaveStore
{
    private readonly string path;
    private readonly GameCatalog catalog;
    private bool recoveredFromBackup;
    public string BackupPath => path + ".bak";

    public LocalSaveStore(string path, GameCatalog catalog)
    {
        this.path = path ?? throw new ArgumentNullException(nameof(path));
        this.catalog = catalog != null ? catalog : throw new ArgumentNullException(nameof(catalog));
    }

    public SaveLoadStatus Load(out PlayerSaveData data)
    {
        data = null;
        recoveredFromBackup = false;
        bool primaryExists = File.Exists(path);
        bool backupExists = File.Exists(BackupPath);
        if (!primaryExists && !backupExists) return SaveLoadStatus.Missing;
        if (primaryExists && TryRead(path, out data)) return SaveLoadStatus.Loaded;
        if (backupExists && TryRead(BackupPath, out data)) { recoveredFromBackup = true; return SaveLoadStatus.Recovered; }
        data = null;
        return SaveLoadStatus.Invalid;
    }

    private bool TryRead(string file, out PlayerSaveData data)
    {
        data = null;
        try
        {
            PlayerSaveData candidate = JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(file, Encoding.UTF8));
            if (!PlayerSaveCodec.TryRestore(candidate, catalog, out _, out _)) return false;
            data = candidate;
            return true;
        }
        catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException)
        {
            return false;
        }
    }

    public bool Save(PlayerSaveData data, out string error)
    {
        error = null;
        if (!PlayerSaveCodec.TryRestore(data, catalog, out _, out _)) { error = "Dados de progresso inválidos."; return false; }
        string temp = path + ".tmp";
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data, true));
            using (var stream = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush(true);
            }
            if (File.Exists(path)) File.Replace(temp, path, recoveredFromBackup ? null : BackupPath);
            else File.Move(temp, path);
            recoveredFromBackup = false;
            return true;
        }
        catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException || e is NotSupportedException)
        {
            error = e.Message;
            return false;
        }
    }
}
