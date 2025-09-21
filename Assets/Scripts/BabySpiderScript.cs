using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BabySpiderScript : MonoBehaviour
{
    public float maxHunger = 100f;
    public float hunger = 100f;

    public float hungerRate = 2f;

    public float jumpingThreshold = 40f;

    private float tick;

    public Animator[] animators;

    public AudioClip sfxBabyFeed;

    public Image hungaryBar;

    public GameObject kaybettinText;
    public bool isLost = false;
    
    public int feedWinAmount = 15;
    public int feedWinCount = 0;

    void Update()
    {
        tick += Time.deltaTime;

        while (tick >= 1f)
        {
            hunger -= hungerRate;
            tick -= 1f;
        }
        if(hunger <= jumpingThreshold)
        {
            MakeJumpingTrue();
        }
        else
        {
            
            MakeJumpingFalse();
        }
        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
        UpdateHungaryBar(hunger);
        
        if(hunger <= 0)
        {
            LoseGame();
        }

        if (isLost && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(1);
        }

        if (feedWinCount >= feedWinAmount)
        {
            WinGame();
        }
    }

    public void FeedBaby(float amount)
    {
        feedWinCount++;
        hunger = Mathf.Clamp(hunger + amount, 0f, maxHunger);
        Debug.Log("Hunger: " + hunger);;
        AudioManager.instance.PlaySFX(sfxBabyFeed, 0.9f);
    }



    public void MakeJumpingTrue() {
        foreach(Animator animator in animators){
            animator.SetBool("isJumping", true);
        }
    }
    
    public void MakeJumpingFalse() {
        foreach(Animator animator in animators){
            animator.SetBool("isJumping", false);
        }
    }

    public void UpdateHungaryBar(float value)
    {
        hungaryBar.fillAmount = value/maxHunger;
    }

    public void LoseGame()
    {
        kaybettinText.SetActive(true);
        Time.timeScale = 0;
        isLost = true;
    }
    
    public void WinGame()
    {
        SceneManager.LoadScene(2);
    }
}