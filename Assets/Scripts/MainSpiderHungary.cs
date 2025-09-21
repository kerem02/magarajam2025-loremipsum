using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSpiderHungary : MonoBehaviour
{
    public int gainedFood = 0;
    
    public float maxHunger = 100f;
    public float hunger = 100f;

    public float hungerRate = 2f;
    public float feedAmount = 10f;
    public GameObject babySpider;

    private float tick;

    private bool isOnFeedZone = false;

    public AudioClip sfxEat;
    
    void Update()
    {

        if (gainedFood > 0 && Input.GetKeyDown(KeyCode.E))
        {
            if (isOnFeedZone)
            {
                babySpider.GetComponent<BabySpiderScript>().FeedBaby(feedAmount);
                gainedFood--;
            }
            else
            {
                FeedSpider(feedAmount);
            }
        }
        
        
        
        
        tick += Time.deltaTime;

        while (tick >= 1f)
        {
            hunger -= hungerRate;
            tick -= 1f;
        }

        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
    }

    public void FeedSpider(float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0f, maxHunger);
        gainedFood--;
        AudioManager.instance.PlaySFX(sfxEat, 0.9f);
        Debug.Log("Hunger: " + hunger);;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BabySpider")) isOnFeedZone = true;
    }
    
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("BabySpider")) isOnFeedZone = false;
    }
}
