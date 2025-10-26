using System;
using System.Collections;
using UnityEngine;

public class WebSpawner : MonoBehaviour
{
    public GameObject web;
    public bool isWebCreated = false;
    public WebController webController;
    private Renderer renderer;
    private Collider col;

    [Header("References")]
    public MainSpiderHungary mainSpiderHungary;   // örümceğe referans
    public Animator spiderAnimator;               // örümceğin Animator’ı atanacak

    [Header("Durations")]
    public float spawnWebDuration = 2f;
    public float getAFlyDuration = 1f;
    private float holdCounter = 0f;

    private bool inTrigger = false;
    private bool isFlyGather = false;

    public AudioClip sfxWeb;
    private bool isTwerk;

    public float webDestroyCooldown = 10f;
    private float webDestroyTimer;

    private void Awake()
    {
        webController = web.GetComponent<WebController>();
        renderer = web.GetComponent<Renderer>();
        col = web.GetComponent<Collider>();
        destroyWeb();
    }

    private void Update()
    {
        // 🕸️ Ağ oluşturma
        if (inTrigger && !isWebCreated)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                spiderAnimator.SetBool("isTwerking", true);

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
                    spiderAnimator.SetBool("isTwerking", false);
                    isTwerk = false;
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                holdCounter = 0f;
                destroyWeb();
                spiderAnimator.SetBool("isTwerking", false);
            }
        }

        // 🪰 Ağdaki sineği alma
        else if (isWebCreated && inTrigger && webController.IsAnyFlyCatched() && !mainSpiderHungary.isThereFlyOnBack)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlaySpiderFlyGather(() =>
                {
                    mainSpiderHungary.TakeAFlyToBackSpawnPosition();
                    webController.GetAFly();
                });
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                holdCounter = 0f;
            }
        }

        // 🕸️ Ağın yok olma süresi
        if (isWebCreated && webDestroyCooldown <= webDestroyTimer)
        {
            destroyWeb();
        }
        if (isWebCreated)
        {
            webDestroyTimer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Spider"))
        {
            inTrigger = true;
            Debug.Log("Örümcek alana girdi");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Spider"))
        {
            inTrigger = false;
            holdCounter = 0f;
            Debug.Log("Örümcek alandan çıktı");
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
        webDestroyTimer = 0f;
    }

    // 🎯 Artık örümceğin Animator’ını kontrol eden versiyon
    public void PlaySpiderFlyGather(Action onCompleted)
    {
        if (isFlyGather) return;

        isFlyGather = true;
        spiderAnimator.SetBool("isGatherFly", true);
        StartCoroutine(FlyGatherRoutine(onCompleted));
    }

    private IEnumerator FlyGatherRoutine(Action onCompleted)
    {
        const int BASE_LAYER = 0;
        const string STATE = "AğdanSinekAlmaAnim"; // Animator’daki state adıyla aynı olmalı

        yield return new WaitUntil(() => spiderAnimator.GetCurrentAnimatorStateInfo(BASE_LAYER).IsName(STATE));

        yield return new WaitUntil(() =>
        {
            var s = spiderAnimator.GetCurrentAnimatorStateInfo(BASE_LAYER);
            return s.IsName(STATE) && s.normalizedTime >= 0.95f && !spiderAnimator.IsInTransition(BASE_LAYER);
        });

        spiderAnimator.SetBool("isGatherFly", false);
        isFlyGather = false;

        onCompleted?.Invoke();
    }
}
