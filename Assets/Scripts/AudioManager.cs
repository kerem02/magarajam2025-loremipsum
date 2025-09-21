using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music")]
    public AudioSource musicSource;   // müzik
    [Header("SFX")]
    public AudioSource sfxSource;     // efektler

    float musicVol = 1f;
    float sfxVol = 1f;

    void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Kaydı yükle
            musicVol = PlayerPrefs.GetFloat("musicVol", 1f);
            sfxVol   = PlayerPrefs.GetFloat("sfxVol", 1f);
            ApplyVolumes();
        } else {
            Destroy(gameObject);
        }
    }

    void ApplyVolumes(){
        if (musicSource) musicSource.volume = musicVol;
        if (sfxSource)   sfxSource.volume   = sfxVol;
    }

    public void PlayMusic(AudioClip clip, float volume = -1f){
        if (clip == null || musicSource == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = (volume >= 0f ? volume : musicVol);
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = -1f){
        if (clip == null || sfxSource == null) return;
        float v = (volume >= 0f ? volume : sfxVol);
        sfxSource.PlayOneShot(clip, v);
    }

    // === Yeni: Dışarıdan ses ayarı ===
    public void SetMusicVolume(float v){
        musicVol = Mathf.Clamp01(v);
        if (musicSource) musicSource.volume = musicVol;
        PlayerPrefs.SetFloat("musicVol", musicVol);
    }

    public float GetMusicVolume() => musicVol;

    public void SetSFXVolume(float v){
        sfxVol = Mathf.Clamp01(v);
        if (sfxSource) sfxSource.volume = sfxVol;
        PlayerPrefs.SetFloat("sfxVol", sfxVol);
    }

    public float GetSFXVolume() => sfxVol;
}