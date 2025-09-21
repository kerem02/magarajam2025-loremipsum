
using UnityEngine;
[CreateAssetMenu(fileName = "FlyConfig", menuName = "Config/Fly")]
public class FlyConfig : ScriptableObject
{
    [Header("Movement")]
    public float toBranchSpeed = 2.2f;
    public float fleeSpeed = 4f;
    public float turnLerp = 10f;
    [Header("Spawn")]
    public float spawnInterval = 1.5f;
    public int maxAlive = 8;
    [Header("Despawn")]
    public float offscreenMargin = 0.08f;
    public float minFleeTime = 1.0f;
    [Header("Capture")]
    public float captureWiggleAmp = 0.08f;
    public float captureWiggleFreq = 12f;
    [Header("Noisy Flight")]
    public float noiseAmp = 0.8f;
    public float noiseFreq = 0.7f;
    public float arrivalSlowRadius = 2.0f;
    public float pathCurvatureLerp = 6f;
    [Header("Decision")]
    public float approachRadius = 1.0f;
    [Header("Web Check")]
    public LayerMask webMask;          // Web layer'ını seç (sadece Web)
    public float webCheckRadius = 0.6f; // hedefte ağ var mı diye bakacağımız yarıçap
}
