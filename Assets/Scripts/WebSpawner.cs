using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebSpawner : MonoBehaviour
{
    public GameObject web;
    public bool isWebCreated = false;
    public WebController webController;
    private Renderer renderer;
    private Collider col;
    
    public MainSpiderHungary mainSpiderHungary;

    public float spawnWebDuration = 2f;
    public float getAFlyDuration = 1f;
    private float holdCounter = 0f;

    private bool inTrigger = false;

    public Animator animator;

    public AudioClip sfxWeb;
    private bool isTwerk;

    private void Awake()
    {
        webController = web.GetComponent<WebController>();
        renderer = web.GetComponent<Renderer>();
        col = web.GetComponent<Collider>();
        destroyWeb();
    }

    private void Update()
    {
        if (inTrigger && !isWebCreated)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                
                animator.SetBool("isTwerking", true);
                
                
                web.SetActive(true);
                holdCounter += Time.deltaTime;

                Color c = renderer.material.color;
                c.a = Mathf.Clamp01(holdCounter / (spawnWebDuration * 1.7f));
                renderer.material.color = c;

                if (holdCounter >= spawnWebDuration)
                {
                    col.enabled = true;
                    isWebCreated = true;
                    holdCounter = 0f;
                    Color ca = renderer.material.color;
                    ca.a = 1f;
                    renderer.material.color = ca;
                    animator.SetBool("isTwerking", false);
                    isTwerk = false;
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                holdCounter = 0f;
                destroyWeb();
                animator.SetBool("isTwerking", false);
            }
        } else if (isWebCreated && inTrigger && webController.IsAnyFlyCatched() && !mainSpiderHungary.isThereFlyOnBack)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                holdCounter += Time.deltaTime;

                if (holdCounter >= getAFlyDuration)
                {
                    //Burda bir adet fly sırtına attırıcaz
                    mainSpiderHungary.TakeAFlyToBackSpawnPosition();
                    webController.GetAFly();
                    holdCounter = 0f;
                }
            }   
            if (Input.GetKeyUp(KeyCode.Space))
            {
                holdCounter = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Spider"))
        {
            Debug.Log("alana girdi");
            inTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Spider"))
        {
            inTrigger = false;
            holdCounter = 0f;
            Debug.Log("alandan çıktı");
            
        }
    }

    public void destroyWeb()
    {
        Color color = renderer.material.color;
        color.a = 0f;
        renderer.material.color = color;
        web.SetActive(false);
        isWebCreated = false;
        col.enabled = false;
        webController.DestroyAllFlies();
    }
    
}