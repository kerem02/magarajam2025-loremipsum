using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class EndCinematic : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName = "Menu";

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}