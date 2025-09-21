using UnityEngine;


public class CanvasController : MonoBehaviour
{
    public GameObject tutorial;
    public bool tutorialActive = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (tutorialActive)
            {
                tutorial.SetActive(!tutorialActive);
                Time.timeScale = 1;
                tutorialActive = false;
            }
            else
            {
                tutorial.SetActive(!tutorialActive);
                Time.timeScale = 0;
                tutorialActive = true;
            }
        }
    }

    public void ShowTutorial()
    {
        tutorial.gameObject.SetActive(true);
    }
    public void HideTutorial()
    {
        tutorial.gameObject.SetActive(false);
    }
}
