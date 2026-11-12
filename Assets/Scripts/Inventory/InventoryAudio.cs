using UnityEngine;

// One soft bubble sound, varied slightly for different inventory actions.
[RequireComponent(typeof(AudioSource))]
public sealed class InventoryAudio : MonoBehaviour
{
    [SerializeField] private AudioClip bubble;
    [SerializeField, Range(0f, 1f)] private float effectsVolume = .48f;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    public void Pop(float pitch = 1f, float volume = 1f)
    {
        if (source == null || bubble == null || !isActiveAndEnabled) return;
        source.pitch = Mathf.Clamp(pitch, .5f, 2f);
        source.PlayOneShot(bubble, volume * effectsVolume * GameAudioSettings.MasterVolume * GameAudioSettings.EffectsVolume);
    }
}
