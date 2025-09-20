using System;
using System.Collections.Generic;
using UnityEngine;

public class WebController : MonoBehaviour
{
    public int maxFlyCount;
    
    public Queue<GameObject> flies = new Queue<GameObject>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fly") && !IsFull())
        {
            Debug.Log("Fly catched");
            flies.Enqueue(other.gameObject);
        }
    }

    public bool IsFull()
    {
        return flies.Count >= maxFlyCount;
    }

    public bool IsAnyFlyCatched()
    {
        return flies.Count > 0;
    }

    public void DestroyAllFlies()
    {
        while (flies.Count > 0)
        {
            GameObject fly = flies.Dequeue();
            Destroy(fly);
        }
    }

    public void GetAFly()
    {
        Destroy(flies.Dequeue());
    }
}
