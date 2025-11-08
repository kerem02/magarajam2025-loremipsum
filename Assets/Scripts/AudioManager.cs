using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music")]
    public AudioSource musicSource;   // müzik
    [Header("SFX")]
    public AudioSource sfxSource;     // efektler
    
    [Header("Mixer")]
    public AudioMixer mixer;  
    public string musicParam = "MusicVol";
    public string sfxParam   = "SFXVol";
    
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
        mixer?.SetFloat(musicParam, LinearToDb(musicVol));
        mixer?.SetFloat(sfxParam,   LinearToDb(sfxVol));
    }
    
    float LinearToDb(float v) => (v <= 0.0001f) ? -80f : Mathf.Log10(v) * 20f;
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

    
    public void SetMusicVolume(float v){
        musicVol = Mathf.Clamp01(v);
        if (musicSource) musicSource.volume = musicVol;
        mixer?.SetFloat(musicParam, LinearToDb(musicVol));
        PlayerPrefs.SetFloat("musicVol", musicVol);
    }

    public float GetMusicVolume() => musicVol;

    public void SetSFXVolume(float v){
        sfxVol = Mathf.Clamp01(v);
        if (sfxSource) sfxSource.volume = sfxVol;
        mixer?.SetFloat(sfxParam, LinearToDb(sfxVol));
        PlayerPrefs.SetFloat("sfxVol", sfxVol);
    }

    public float GetSFXVolume() => sfxVol;
}