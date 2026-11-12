using UnityEngine;

// Central volume values are ready for future settings UI and do not affect game saves.
public static class GameAudioSettings
{
    private const string MasterKey = "Fluffy.Audio.Master";
    private const string EffectsKey = "Fluffy.Audio.Effects";
    private const string MusicKey = "Fluffy.Audio.Music";
    public static float MasterVolume { get => Mathf.Clamp01(PlayerPrefs.GetFloat(MasterKey, 1f)); set => Set(MasterKey, value); }
    public static float EffectsVolume { get => Mathf.Clamp01(PlayerPrefs.GetFloat(EffectsKey, 1f)); set => Set(EffectsKey, value); }
    public static float MusicVolume { get => Mathf.Clamp01(PlayerPrefs.GetFloat(MusicKey, 1f)); set => Set(MusicKey, value); }
    private static void Set(string key, float value) { PlayerPrefs.SetFloat(key, Mathf.Clamp01(value)); PlayerPrefs.Save(); }
}
