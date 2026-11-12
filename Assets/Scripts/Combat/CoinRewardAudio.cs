using UnityEngine;

// The shared source keeps the reward audible even when the defeated enemy disappears.
[RequireComponent(typeof(AudioSource))]
public sealed class CoinRewardAudio : MonoBehaviour
{
    [SerializeField] private AudioClip reward;
    [SerializeField, Range(0f, 1f)] private float effectsVolume = .7f;
    private AudioSource source;
    public static CoinRewardAudio Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    private void OnDestroy() { if (Instance == this) Instance = null; }

    public void PlayReward()
    {
        if (!isActiveAndEnabled || source == null || reward == null) return;
        source.PlayOneShot(reward, effectsVolume * GameAudioSettings.MasterVolume * GameAudioSettings.EffectsVolume);
    }
}
