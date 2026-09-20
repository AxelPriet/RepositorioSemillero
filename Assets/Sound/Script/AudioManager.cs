using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Librería")]
    [SerializeField] private SoundLibrary library;

    [Header("AudioMixer")]
    [SerializeField] private AudioMixer mainMixer;

    [Header("Pool de AudioSources")]
    [SerializeField] private int sfxPoolSize = 10;
    [SerializeField] private int musicPoolSize = 2;
    [SerializeField] private int ambientPoolSize = 2;

    private AudioSource musicSource;
    private AudioSource ambientSource;
    private AudioSource[] sfxPool;
    private int sfxPoolIndex = 0;

    // Parámetros del mixer 
    private const string MASTER_PARAM = "MasterVolume";
    private const string MUSIC_PARAM = "MusicVolume";
    private const string SFX_PARAM = "SFXVolume";
    private const string UI_PARAM = "UIVolume";
    private const string AMBIENT_PARAM = "AmbientVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSources()
    {
        // Music
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // Ambient
        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.loop = true;
        ambientSource.playOnAwake = false;

        // SFX 
        sfxPool = new AudioSource[sfxPoolSize];
        for (int i = 0; i < sfxPoolSize; i++)
        {
            sfxPool[i] = gameObject.AddComponent<AudioSource>();
            sfxPool[i].playOnAwake = false;
        }
    }

    // MÉTODOS PÚBLICOS

    // Reproduce música de fondo 
    public void PlayMusic(string id, float fadeDuration = 1f)
    {
        SoundSO sound = library.Get(id);
        if (sound == null || sound.Clip == null) return;

        StartCoroutine(CrossfadeMusic(sound, fadeDuration));
    }

    public void StopMusic(float fadeDuration = 1f)
    {
        StartCoroutine(FadeOut(musicSource, fadeDuration));
    }

    // Reproduce un efecto de sonido
    public void PlaySFX(string id, Vector3 position = default)
    {
        SoundSO sound = library.Get(id);
        if (sound == null || sound.Clip == null) return;

        AudioSource source = GetFreeSFXSource();
        ConfigureSource(source, sound);

        if (sound.Is3D && position != default)
        {
            source.transform.position = position;
            source.spatialBlend = 1f;
        }
        else
        {
            source.spatialBlend = 0f;
        }

        source.Play();
    }

    // Reproduce un sonido de UI
    public void PlayUI(string id)
    {
        SoundSO sound = library.Get(id);
        if (sound == null || sound.Clip == null) return;

        AudioSource source = GetFreeSFXSource();
        ConfigureSource(source, sound);
        source.spatialBlend = 0f;
        source.Play();
    }

    // Reproduce ambiente 
    public void PlayAmbient(string id, float fadeDuration = 1f)
    {
        SoundSO sound = library.Get(id);
        if (sound == null || sound.Clip == null) return;

        StartCoroutine(CrossfadeAmbient(sound, fadeDuration));
    }

    public void StopAmbient(float fadeDuration = 1f)
    {
        StartCoroutine(FadeOut(ambientSource, fadeDuration));
    }

    // CONTROL DE VOLUMEN 

    public void SetMasterVolume(float volume) => SetMixerVolume(MASTER_PARAM, volume);
    public void SetMusicVolume(float volume) => SetMixerVolume(MUSIC_PARAM, volume);
    public void SetSFXVolume(float volume) => SetMixerVolume(SFX_PARAM, volume);
    public void SetUIVolume(float volume) => SetMixerVolume(UI_PARAM, volume);
    public void SetAmbientVolume(float volume) => SetMixerVolume(AMBIENT_PARAM, volume);

    private void SetMixerVolume(string parameter, float volume)
    {
        if (mainMixer == null) return;

        // Convertir de 0-1 a decibelios (-80 a 0)
        float db = volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
        mainMixer.SetFloat(parameter, db);
    }

    // HELPERS PRIVADOS

    private AudioSource GetFreeSFXSource()
    {
        foreach (var source in sfxPool)
        {
            if (!source.isPlaying) return source;
        }

        AudioSource next = sfxPool[sfxPoolIndex];
        sfxPoolIndex = (sfxPoolIndex + 1) % sfxPool.Length;
        return next;
    }

    private void ConfigureSource(AudioSource source, SoundSO sound)
    {
        source.clip = sound.Clip;
        source.volume = sound.Volume;
        source.pitch = sound.GetRandomPitch();
        source.loop = sound.Loop;
        source.outputAudioMixerGroup = sound.MixerGroup;
        source.playOnAwake = sound.PlayOnAwake;

        if (sound.Is3D)
        {
            source.minDistance = sound.MinDistance;
            source.maxDistance = sound.MaxDistance;
        }
    }

    private IEnumerator CrossfadeMusic(SoundSO sound, float duration)
    {
        if (musicSource.isPlaying)
            yield return FadeOut(musicSource, duration);

        ConfigureSource(musicSource, sound);
        musicSource.Play();

        yield return FadeIn(musicSource, sound.Volume, duration);
    }

    private IEnumerator CrossfadeAmbient(SoundSO sound, float duration)
    {
        if (ambientSource.isPlaying)
            yield return FadeOut(ambientSource, duration);

        ConfigureSource(ambientSource, sound);
        ambientSource.Play();

        yield return FadeIn(ambientSource, sound.Volume, duration);
    }

    private IEnumerator FadeIn(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = 0f;
        source.volume = startVolume;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, t / duration);
            yield return null;
        }
        source.volume = targetVolume;
    }

    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }
        source.volume = 0f;
        source.Stop();
    }
}