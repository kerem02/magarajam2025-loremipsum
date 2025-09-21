using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSpiderHungary : MonoBehaviour
{
    
    public float maxHunger = 100f;
    public float hunger = 100f;

    public float hungerRate = 2f;
    public float feedAmount = 10f;
    public GameObject babySpider;

    public GameObject flyPrefab;
    public GameObject backSpawnPosition;
    public GameObject spawnedFlyOnBack;
    public bool isThereFlyOnBack = false;
    
    private float tick;
    private bool isOnFeedZone = false;
    
    public SpiderSurfaceWalker spiderSurfaceWalker;
    public float walkDebuffAmount = 1;


    void Awake()
    {
        spiderSurfaceWalker = GetComponent<SpiderSurfaceWalker>();
    }
    void Update()
    {

        if (isThereFlyOnBack && Input.GetKeyDown(KeyCode.E))
        {
            if (isOnFeedZone)
            {
                babySpider.GetComponent<BabySpiderScript>().FeedBaby(feedAmount);
                DestroyFlyOnBack();
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

    public void TakeAFlyToBackSpawnPosition()
    {
        spiderSurfaceWalker.moveSpeed -= walkDebuffAmount;
        if (isThereFlyOnBack) { 
            Debug.Log("There is already a fly on back"); 
            return; 
        }

        var inst = Instantiate(flyPrefab);

        inst.transform.SetParent(backSpawnPosition.transform, false);

        inst.transform.localPosition = Vector3.zero;
        inst.transform.localRotation = Quaternion.Euler(180f,0f,0f);
        inst.transform.localScale    = new Vector3(0.4f, 0.4f, 0.4f);


        inst.GetComponentInChildren<Animator>(true)?.SetBool("isDeath", true);
        spawnedFlyOnBack = inst;
        isThereFlyOnBack = true;
    }


    public void DestroyFlyOnBack()
    {
        if (isThereFlyOnBack)
        {
            Destroy(spawnedFlyOnBack);
            spiderSurfaceWalker.moveSpeed -= walkDebuffAmount;
            isThereFlyOnBack = false;
        }
        else
        {
            Debug.Log("There is no fly on back");
        }
    }

    public void FeedSpider(float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0f, maxHunger);
        DestroyFlyOnBack();
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
