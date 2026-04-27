using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource thrusterSource;
    [SerializeField] private int sfxPoolSize = 5;

    [Header("Clips")]
    [SerializeField] private AudioClip defaultMusic;
    [SerializeField] private AudioClip thrusterClip;
    [SerializeField] private AudioClip landingSuccessClip;
    [SerializeField] private AudioClip crashClip;
    [SerializeField] private AudioClip coinClip;
    [SerializeField] private AudioClip uiClickClip;

    [Header("Default Volumes")]
    [Range(0f, 1f)][SerializeField] private float defaultMusicVolume = 0.8f;
    [Range(0f, 1f)][SerializeField] private float defaultSfxVolume = 0.8f;

    private readonly List<AudioSource> sfxSources = new List<AudioSource>();
    private int nextSfxSourceIndex;
    private float musicVolume;
    private float sfxVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePersistentSources();
        CreateSfxPool();
        LoadVolumes();
        ApplyVolumes();

        if (defaultMusic != null)
        {
            PlayMusic(defaultMusic);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSources.Count == 0)
        {
            return;
        }

        AudioSource source = GetAvailableSfxSource();
        source.clip = clip;
        source.loop = false;
        source.volume = sfxVolume;
        source.Play();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
        {
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource?.Stop();
    }

    public void SetThrusterActive(bool isActive, float intensity = 1f)
    {
        if (thrusterSource == null || thrusterClip == null)
        {
            return;
        }

        thrusterSource.volume = Mathf.Clamp01(intensity) * sfxVolume;
        if (isActive)
        {
            if (!thrusterSource.isPlaying)
            {
                thrusterSource.Play();
            }
        }
        else if (thrusterSource.isPlaying)
        {
            thrusterSource.Stop();
        }
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.Save();
        ApplyVolumes();
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
        PlayerPrefs.Save();
        ApplyVolumes();
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSfxVolume()
    {
        return sfxVolume;
    }

    public void PlayLandingSuccessSfx()
    {
        PlaySFX(landingSuccessClip);
    }

    public void PlayCrashSfx()
    {
        PlaySFX(crashClip);
    }

    public void PlayCoinSfx()
    {
        PlaySFX(coinClip);
    }

    public void PlayUIClickSfx()
    {
        PlaySFX(uiClickClip);
    }

    private void InitializePersistentSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }

        if (thrusterSource == null)
        {
            thrusterSource = gameObject.AddComponent<AudioSource>();
            thrusterSource.playOnAwake = false;
            thrusterSource.loop = true;
            thrusterSource.clip = thrusterClip;
        }
        else
        {
            thrusterSource.clip = thrusterClip;
            thrusterSource.loop = true;
            thrusterSource.playOnAwake = false;
        }
    }

    private void CreateSfxPool()
    {
        sfxSources.Clear();
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            sfxSources.Add(source);
        }
    }

    private AudioSource GetAvailableSfxSource()
    {
        for (int i = 0; i < sfxSources.Count; i++)
        {
            int index = (nextSfxSourceIndex + i) % sfxSources.Count;
            if (!sfxSources[index].isPlaying)
            {
                nextSfxSourceIndex = (index + 1) % sfxSources.Count;
                return sfxSources[index];
            }
        }

        AudioSource fallback = sfxSources[nextSfxSourceIndex];
        nextSfxSourceIndex = (nextSfxSourceIndex + 1) % sfxSources.Count;
        return fallback;
    }

    private void LoadVolumes()
    {
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, defaultMusicVolume);
        sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, defaultSfxVolume);
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }

        if (thrusterSource != null)
        {
            thrusterSource.volume = sfxVolume;
        }

        foreach (AudioSource source in sfxSources)
        {
            source.volume = sfxVolume;
        }
    }
}
