using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySpawner : MonoBehaviour {
    public FlyConfig config;
    public Camera cam;
    public Transform branchTarget;
    public List<Transform> spawnPoints = new();
    public FlyAgent flyPrefab;
    int alive;

    void Start(){ if(!cam) cam=Camera.main; StartCoroutine(Loop()); }
    IEnumerator Loop(){
        while(true){
            yield return new WaitForSeconds(config.spawnInterval);
            if (alive>=config.maxAlive || spawnPoints.Count==0 || !branchTarget) continue;
            var sp = spawnPoints[Random.Range(0, spawnPoints.Count)];
            var fly = Instantiate(flyPrefab, sp.position, sp.rotation);
            fly.Init(this, config, branchTarget, cam, sp.position);
            alive++;
        }
    }
    public void OnFlyDespawn(FlyAgent f){ alive=Mathf.Max(0, alive-1); }
}