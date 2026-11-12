using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class BossImpactAudio : MonoBehaviour
{
    [SerializeField] private AudioClip impact;
    [SerializeField, Range(0f, 1f)] private float effectsVolume = .55f;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    public void PlayImpact()
    {
        if (source == null || impact == null || !isActiveAndEnabled) return;
        source.PlayOneShot(impact, effectsVolume * GameAudioSettings.MasterVolume * GameAudioSettings.EffectsVolume);
    }
}
