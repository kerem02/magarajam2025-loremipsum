using UnityEngine;

public class BabySpiderScript : MonoBehaviour
{
    public float maxHunger = 100f;
    public float hunger = 100f;

    public float hungerRate = 2f;

    private float tick;

    void Update()
    {
        tick += Time.deltaTime;

        while (tick >= 1f)
        {
            hunger -= hungerRate;
            tick -= 1f;
        }

        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
    }

    public void FeedBaby(float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0f, maxHunger);
        Debug.Log("Hunger: " + hunger);;
    }
}