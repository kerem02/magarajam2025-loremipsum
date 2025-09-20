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

    public float spawnWebDuration = 2f;
    public float getAFlyDuration = 2f;
    private float holdCounter = 0f;

    private bool inTrigger = false;

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
                web.SetActive(true);
                holdCounter += Time.deltaTime;

                Color c = renderer.material.color;
                c.a = Mathf.Clamp01(holdCounter / spawnWebDuration);
                renderer.material.color = c;

                if (holdCounter >= spawnWebDuration)
                {
                    col.enabled = true;
                    isWebCreated = true;
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                holdCounter = 0f;
                destroyWeb();
            }
        } else if (isWebCreated && inTrigger && webController.isAnyFlyCatched())
        {
            if (Input.GetKey(KeyCode.Space))
            {
                holdCounter += Time.deltaTime;

                if (holdCounter >= getAFlyDuration)
                {
                    //Burda bir adet fly sırtına attırıcaz
                    webController.getAFly();
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
        inTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        inTrigger = false;
        holdCounter = 0f;
    }

    public void destroyWeb()
    {
        Color color = renderer.material.color;
        color.a = 0f;
        renderer.material.color = color;
        web.SetActive(false);
        isWebCreated = false;
        col.enabled = false;
        webController.destroyAllFlies();
    }
}