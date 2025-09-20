using System;
using System.Collections.Generic;
using UnityEngine;

public class WebController : MonoBehaviour
{
    public int maxFlyCount;
    
    Queue<GameObject> flies = new Queue<GameObject>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fly") && !IsFull())
        {
            flies.Enqueue(other.gameObject);
        }
    }

    public bool IsFull()
    {
        return flies.Count == maxFlyCount;
    }

    public bool isAnyFlyCatched()
    {
        return flies.Count > 0;
    }

    public void destroyAllFlies()
    {
        while (flies.Count > 0)
        {
            GameObject fly = flies.Dequeue();
            Destroy(fly);
        }
    }

    public void getAFly()
    {
        Destroy(flies.Dequeue());
    }
}
