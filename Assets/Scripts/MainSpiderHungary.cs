using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSpiderHungary : MonoBehaviour
{
    public int gainedFood = 0;
    
    public float maxHunger = 100f;
    public float hunger = 100f;

    public float hungerRate = 2f;
    public float feedAmount = 30f;
    public GameObject babySpider;

    public GameObject flyPrefab;
    public GameObject backSpawnPosition;
    public GameObject spawnedFlyOnBack;
    public bool isThereFlyOnBack = false;
    
    private float tick;

    private bool isOnFeedZone = false;

    public AudioClip sfxEat;

    private bool isEating = false;
    const int BASE_LAYER = 0;   
    
    public SpiderSurfaceWalker spiderSurfaceWalker;
    public float walkDebuffAmount = 1;

    public GameObject stealthImage;

    public Animator animator;
    public GameObject eatingAnimFlyHead;
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
                
            }
            else
            {
                DestroyFlyOnBack();
                StartEat();
                //FeedSpider(feedAmount);
            }
        }
        
        
        
        
        tick += Time.deltaTime;

        while (tick >= 1f)
        {
            hunger -= hungerRate;
            tick -= 1f;
        }

        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
        UpdateStealthImage();
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
            spiderSurfaceWalker.moveSpeed += walkDebuffAmount;
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


    public void UpdateStealthImage()
    {
        stealthImage.SetActive(StealthState.SpiderHidden);
    }
    
    
    void StartEat()
    {
        eatingAnimFlyHead.SetActive(true);
        isEating = true;               
        animator.SetBool("isEating", true);   
        StartCoroutine(EatRoutine());    
    }

    IEnumerator EatRoutine()
    {
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(BASE_LAYER).IsName("ÖrümcekYemeAnim"));

        yield return new WaitUntil(() =>
        {
            var s = animator.GetCurrentAnimatorStateInfo(BASE_LAYER);
            return s.IsName("ÖrümcekYemeAnim") && s.normalizedTime >= 1f && !animator.IsInTransition(BASE_LAYER);
        });

        DoEatLogic();  
        
        eatingAnimFlyHead.SetActive(false);
        animator.SetBool("isEating", false);
        isEating = false;
    }

    void DoEatLogic()
    {
        FeedSpider(feedAmount);
    }
}
