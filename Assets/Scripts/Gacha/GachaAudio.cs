using UnityEngine;

// Local 2D effects for one machine. Clips and volumes can be tuned in the Inspector.
[RequireComponent(typeof(AudioSource))]
public sealed class GachaAudio : MonoBehaviour
{
    [SerializeField] private AudioClip start;
    [SerializeField] private AudioClip tick;
    [SerializeField] private AudioClip error;
    [SerializeField] private AudioClip common;
    [SerializeField] private AudioClip rare;
    [SerializeField] private AudioClip epic;
    [SerializeField] private AudioClip legendary;
    [SerializeField] private AudioClip secret;
    [SerializeField, Range(0f, 1f)] private float effectsVolume = .7f;
    [SerializeField, Range(0f, 1f)] private float tickVolume = .32f;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    private void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (source == null || clip == null) return;
        source.pitch = pitch;
        source.PlayOneShot(clip, volume * effectsVolume * GameAudioSettings.MasterVolume * GameAudioSettings.EffectsVolume);
    }

    public void StartRoll() => Play(start);
    public void Tick() => Play(tick, tickVolume);
    public void Error() => Play(error);
    public void Stop() { if (source != null) source.Stop(); }

    public void Reveal(PetRarity rarity)
    {
        switch (rarity)
        {
            case PetRarity.Rare: Play(rare); break;
            case PetRarity.Epic: Play(epic); break;
            case PetRarity.Legendary: Play(legendary); break;
            case PetRarity.Secret: Play(secret != null ? secret : legendary, 1f, 1.12f); break;
            default: Play(common); break;
        }
    }
}
