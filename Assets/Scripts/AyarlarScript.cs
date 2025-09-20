using UnityEngine;
using UnityEngine.UI;

public class AyarlarScript : MonoBehaviour
{
    public Slider mainMusicSlider;
    public AudioManager audioManager;

    public void setMainMusicVolume()
    {
        audioManager.SetMainMusicVolume(mainMusicSlider.value);
    }
    
}
