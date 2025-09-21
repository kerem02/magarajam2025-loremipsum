using UnityEngine;

public class BabySpiderScript : MonoBehaviour
{
    public float maxHunger = 100f;
    public float hunger = 100f;

    public float hungerRate = 2f;

    public float jumpingThreshold = 40f;

    private float tick;

    public Animator[] animators;

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
    }

    public void FeedBaby(float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0f, maxHunger);
        Debug.Log("Hunger: " + hunger);;
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
}