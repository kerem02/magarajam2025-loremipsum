using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music")]
    public AudioSource musicSource;   // müzik için ayrı
    [Header("SFX")]
    public AudioSource sfxSource;     // tüm efektler için tek kaynak

    void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip clip, float volume = 1f){
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f){
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }
}