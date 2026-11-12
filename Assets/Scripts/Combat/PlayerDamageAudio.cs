using UnityEngine;

// One random sound per accepted hit, including a fatal hit.
[RequireComponent(typeof(AudioSource))]
public sealed class PlayerDamageAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;
    [SerializeField, Range(0f, 1f)] private float effectsVolume = .65f;
    private readonly System.Random random = new System.Random();
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    public void PlayDamage()
    {
        if (!isActiveAndEnabled || source == null || sounds == null || sounds.Length == 0) return;
        var clip = sounds[random.Next(sounds.Length)];
        if (clip != null) source.PlayOneShot(clip, effectsVolume * GameAudioSettings.MasterVolume * GameAudioSettings.EffectsVolume);
    }
}
