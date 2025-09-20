using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySpawner : MonoBehaviour {
    
    [Header("Config")]
    public FlyConfig config;
    public Camera cam;

    [Header("Targets")]
    public List<Transform> targets = new(); // dalların uçlarına koyduğun noktalar

    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new(); // kamera dışındaki spawn noktaları

    [Header("Prefab")]
    public FlyAgent flyPrefab;

    int alive;

    void Start(){ if(!cam) cam=Camera.main; StartCoroutine(Loop()); }
    IEnumerator Loop(){
        while (true){
            yield return new WaitForSeconds(config.spawnInterval);

            if (alive >= config.maxAlive) continue;
            if (spawnPoints.Count == 0 || targets.Count == 0) continue;

            // rastgele spawn noktası
            var sp = spawnPoints[Random.Range(0, spawnPoints.Count)];
            // rastgele hedef
            var ap = targets[Random.Range(0, targets.Count)];

            // sineği üret
            var fly = Instantiate(flyPrefab, sp.position, sp.rotation);
            fly.Init(this, config, ap, cam, sp.position);
            alive++;
        }
    }
    public void OnFlyDespawn(FlyAgent f){ alive=Mathf.Max(0, alive-1); }
}