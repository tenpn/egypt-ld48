using UnityEngine;

class HitSfx : MonoBehaviour {
    //////////////////////////////////////////////////

    float ignoreSfxCountdown = 0.5f;
    [SerializeField] AudioClip smallHit;
    [SerializeField] AudioClip ballHit;
    [SerializeField] AnimationCurve volBySqrVal;
    [SerializeField] AnimationCurve ballVolBySqrVal;
    [SerializeField] AudioSource sfx;
    [SerializeField] float pitchShift = 0.1f;

    //////////////////////////////////////////////////

    void Update() {
        ignoreSfxCountdown -= Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (ignoreSfxCountdown > 0) {
            return;
        }

        bool isBallOnBall = other.gameObject.CompareTag("ball");

        var ducker = isBallOnBall ? ballVolBySqrVal : volBySqrVal;
        float targetVol = ducker.Evaluate(other.relativeVelocity.sqrMagnitude);
        
        if (targetVol > 0f) {
            sfx.pitch = 1f + Random.Range(-pitchShift, pitchShift);
            var clip = isBallOnBall ? ballHit : smallHit;
            sfx.PlayOneShot(clip, targetVol);
        }
    }
}
