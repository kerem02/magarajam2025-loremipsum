
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
    private float life;
    WebController web;
    public AudioClip sfxCaptured;
    public AudioSource buzzSource;
    public AudioClip buzzClip;

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
        
        life += Time.deltaTime;
        if (life > 20f && state != FlyState.Captured) Despawn();
    }
    
    void Start(){
        if (buzzSource && buzzClip){
            buzzSource.clip = buzzClip;
            buzzSource.loop = true;
            buzzSource.Play();
        }
    }
    
    bool IsWebAtTarget(Vector3 targetPos){
        // Hedef çevresinde "Web" layer collider var mı?
        var hits = Physics.OverlapSphere(targetPos, cfg.webCheckRadius, cfg.webMask, QueryTriggerInteraction.Collide);
        return hits != null && hits.Length > 0;
    }
    
    
    WebController FindWebAtTarget(Vector3 pos){
        // Web layer'ındaki collider'ları tara; içlerinden WebController olanı döndür
        var hits = Physics.OverlapSphere(pos, cfg.webCheckRadius, cfg.webMask, QueryTriggerInteraction.Collide);
        if (hits == null) return null;
        foreach (var h in hits){
            var wc = h.GetComponent<WebController>() ?? h.GetComponentInParent<WebController>();
            if (wc != null) return wc;
        }
        return null;
    }



    void TickToBranch(){
        if (!branch){ Despawn(); return; }

        float speedScale;
        Vector3 dirNoisy = ComputeNoisyDir(branch.position, cfg.toBranchSpeed, out speedScale);

        
        transform.forward = Vector3.Slerp(transform.forward, dirNoisy, Time.deltaTime * cfg.pathCurvatureLerp);

        
        transform.position += transform.forward * (cfg.toBranchSpeed * speedScale) * Time.deltaTime;

        
        Vector3 to = branch.position - transform.position;
        if (to.sqrMagnitude < cfg.approachRadius * cfg.approachRadius){
            // 🔎 hedefte aktif bir ağ var mı? varsa cache'le
            if (web == null) web = FindWebAtTarget(branch.position);

            bool canUseWeb = StealthState.SpiderHidden && web != null && web.IsFull() == false;
            if (!canUseWeb){
                EnterFlee(); // gizli değilse ya da ağ yoksa ya da ağ doluysa → kaç
            }
            // else: devam et; ağa değerse Captured olacak
        }
    }

    
    void EnterFlee(){ state=FlyState.Flee; stateTimer=0f; fleeDir=(spawnPos-transform.position).normalized; }
    void TickFlee(){
        stateTimer += Time.deltaTime;
        
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
            //body.localScale = Vector3.one * (1f + Mathf.Sin(stateTimer*f)*a);
        }
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
        if (state == FlyState.Captured) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Web")) return;

        // web referansı yoksa, çarptığın objeden çek
        if (!web){
            web = other.GetComponent<WebController>() ?? other.GetComponentInParent<WebController>();
        }

        // ağ doluysa yakalama yok → kaç
        if (web != null && web.IsFull()){
            EnterFlee();
            return;
        }

        EnterCaptured();
    }

    public void EnterCaptured()
    {
        state=FlyState.Captured; stateTimer=0f;
        AudioManager.instance.PlaySFX(sfxCaptured, 0.5f);
        buzzSource.Stop();
    }
    
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
