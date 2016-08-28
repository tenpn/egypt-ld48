using UnityEngine;

class HitSfx : MonoBehaviour {
    //////////////////////////////////////////////////

    float ignoreSfxCountdown = 0.5f;
    [SerializeField] AudioClip smallHit;
    [SerializeField] AnimationCurve volBySqrVal;
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
        float targetVol = volBySqrVal.Evaluate(other.relativeVelocity.sqrMagnitude);
        if (targetVol > 0f) {
            sfx.pitch = 1f + Random.Range(-pitchShift, pitchShift);
            sfx.PlayOneShot(smallHit, targetVol);
        }
    }
}
