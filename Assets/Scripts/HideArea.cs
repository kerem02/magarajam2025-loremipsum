using UnityEngine;

public class HideArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Spider gizlendi");
        if (other.CompareTag("Spider"))
        {
            StealthState.SpiderHidden = true;
            Debug.Log("Spider gizlendi");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Spider"))
        {
            StealthState.SpiderHidden = false;
            Debug.Log("Spider ortaya çıktı");
        }
    }
}

