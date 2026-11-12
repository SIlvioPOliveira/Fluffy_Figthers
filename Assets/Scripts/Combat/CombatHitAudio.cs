using UnityEngine;

// A single shared source keeps simultaneous pet attacks from becoming too loud.
[RequireComponent(typeof(AudioSource))]
public sealed class CombatHitAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] hits;
    [SerializeField, Range(0f, 1f)] private float effectsVolume = .35f;
    [SerializeField, Min(0f)] private float minimumInterval = .07f;
    [SerializeField, Range(.5f, 2f)] private float minimumPitch = .92f;
    [SerializeField, Range(.5f, 2f)] private float maximumPitch = 1.08f;
    private readonly System.Random random = new System.Random();
    private AudioSource source;
    private int lastClip = -1;
    private float lastPitch = float.NaN;
    private float nextHitTime;
    public static CombatHitAudio Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    private void OnDestroy() { if (Instance == this) Instance = null; }

    public void PlayHit()
    {
        if (!isActiveAndEnabled || source == null || hits == null || hits.Length == 0 || Time.unscaledTime < nextHitTime) return;
        int index = random.Next(hits.Length);
        if (hits.Length > 1 && index == lastClip) index = (index + 1 + random.Next(hits.Length - 1)) % hits.Length;
        if (hits[index] == null) return;
        lastClip = index;
        nextHitTime = Time.unscaledTime + minimumInterval;
        float low = Mathf.Min(minimumPitch, maximumPitch), high = Mathf.Max(minimumPitch, maximumPitch);
        float middle = (low + high) * .5f;
        float gap = (high - low) * .1f;
        bool useLow = float.IsNaN(lastPitch) ? random.Next(2) == 0 : lastPitch >= middle;
        float bandLow = useLow ? low : middle + gap;
        float bandHigh = useLow ? middle - gap : high;
        source.pitch = Mathf.Lerp(bandLow, bandHigh, (float)random.NextDouble());
        lastPitch = source.pitch;
        source.PlayOneShot(hits[index], effectsVolume * GameAudioSettings.MasterVolume * GameAudioSettings.EffectsVolume);
    }
}
