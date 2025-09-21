using UnityEngine;
using UnityEngine.UI;

public class AyarlarScript : MonoBehaviour
{
    [Header("Sliders")]
    public Slider mainMusicSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (AudioManager.instance != null){
            if (mainMusicSlider){
                mainMusicSlider.value = AudioManager.instance.GetMusicVolume();
                mainMusicSlider.onValueChanged.AddListener(OnMusicChanged);
            }
            if (sfxSlider){
                sfxSlider.value = AudioManager.instance.GetSFXVolume();
                sfxSlider.onValueChanged.AddListener(OnSFXChanged);
            }
        }
    }

    void OnDestroy(){
        if (mainMusicSlider) mainMusicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        if (sfxSlider)       sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
    }

    public void OnMusicChanged(float v){
        AudioManager.instance?.SetMusicVolume(v);
    }

    public void OnSFXChanged(float v){
        AudioManager.instance?.SetSFXVolume(v);
    }
}