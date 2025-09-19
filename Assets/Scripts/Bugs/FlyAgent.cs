
using UnityEngine;
public enum FlyState { ToBranch, Flee, Captured }

[RequireComponent(typeof(Collider))]
public class FlyAgent : MonoBehaviour
{
    FlySpawner spawner;
    FlyConfig cfg;
    Transform branch;
    Camera cam;
    Vector3 spawnPos;
    public FlyState state { get; private set; } = FlyState.ToBranch;
    float stateTimer; 
    Vector3 fleeDir;
    bool despawnRequested;
    Transform body;
    float noiseT;

    public void Init(FlySpawner sp, FlyConfig config, Transform branchTarget, Camera cameraRef, Vector3 spawnPosition)
    {
        spawner = sp; cfg = config; branch = branchTarget; cam = cameraRef; spawnPos = spawnPosition;
        body = transform;
        gameObject.layer = LayerMask.NameToLayer("Bug");
        var col = GetComponent<Collider>(); col.isTrigger = true;
    }

    void Update()
    {
        switch (state)
        {
            case FlyState.ToBranch: TickToBranch(); break;
            case FlyState.Flee: TickFlee(); break;
            case FlyState.Captured: TickCaptured(); break;
        }
    }

    void TickToBranch(){
        if (!branch){ Despawn(); return; }

        float speedScale;
        Vector3 dirNoisy = ComputeNoisyDir(branch.position, cfg.toBranchSpeed, out speedScale);

        
        transform.forward = Vector3.Slerp(transform.forward, dirNoisy, Time.deltaTime * cfg.pathCurvatureLerp);

        
        transform.position += transform.forward * (cfg.toBranchSpeed * speedScale) * Time.deltaTime;

        
        Vector3 to = branch.position - transform.position;
        if (to.sqrMagnitude < 0.16f){
            if (StealthState.SpiderHidden){
                
            } else {
                EnterFlee();
            }
        }
    }

    
    void EnterFlee(){ state=FlyState.Flee; stateTimer=0f; fleeDir=(spawnPos-transform.position).normalized; }
    void TickFlee(){
        stateTimer += Time.deltaTime;

        
        float dummy;
        Vector3 fleeNoisy = (fleeDir + new Vector3(
            (Mathf.PerlinNoise(noiseT, 1.7f)*2f-1f)*0.2f,
            0f,
            (Mathf.PerlinNoise(2.3f, noiseT)*2f-1f)*0.2f
        )).normalized;

        transform.forward = Vector3.Slerp(transform.forward, fleeNoisy, Time.deltaTime * (cfg.pathCurvatureLerp*0.7f));
        transform.position += transform.forward * cfg.fleeSpeed * Time.deltaTime;

        if (stateTimer>=cfg.minFleeTime && IsOutsideCamera(cam, cfg.offscreenMargin)) Despawn();
    }

    
    void TickCaptured(){
        stateTimer += Time.deltaTime;
        if (body){
            float a=cfg.captureWiggleAmp, f=cfg.captureWiggleFreq;
            body.localScale = Vector3.one * (1f + Mathf.Sin(stateTimer*f)*a);
        }
    }
    
    void MoveAndFace(Vector3 dir, float speed){
        transform.position += dir * speed * Time.deltaTime;
        transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * cfg.turnLerp);
    }
    
    bool IsOutsideCamera(Camera c, float m){
        if(!c) return true;
        var vp = c.WorldToViewportPoint(transform.position);
        return (vp.z<0f) || (vp.x<-m || vp.x>1f+m || vp.y<-m || vp.y>1f+m);
    }
    
    void Despawn(){
        if (despawnRequested) return; despawnRequested=true;
        spawner.OnFlyDespawn(this); Destroy(gameObject);
    }
    
    void OnTriggerEnter(Collider other){
        if (state==FlyState.Captured) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Web")) EnterCaptured();
    }
    public void EnterCaptured(){ state=FlyState.Captured; stateTimer=0f; }
    
    Vector3 ComputeNoisyDir(Vector3 target, float baseSpeed, out float speedScale){
        Vector3 to = (target - transform.position);
        float dist = to.magnitude;
        if (dist < 0.0001f){ speedScale=0f; return transform.forward; }

        
        Vector3 fwd = to / dist;

        
        Vector3 right = Vector3.Cross(Vector3.up, fwd).normalized;

        
        noiseT += Time.deltaTime * cfg.noiseFreq;
        float n = Mathf.PerlinNoise(noiseT, 0.123f) * 2f - 1f; 
        float n2 = Mathf.PerlinNoise(0.917f, noiseT) * 2f - 1f; 

        
        Vector3 noisy = fwd + right * (n * cfg.noiseAmp * 0.3f) + fwd * (n2 * cfg.noiseAmp * 0.1f);

        
        float slow = 1f;
        if (dist < cfg.arrivalSlowRadius){
            slow = Mathf.Clamp01(dist / cfg.arrivalSlowRadius);
        }
        speedScale = slow;

        return noisy.normalized;
    }
}
