using UnityEngine;
using System.Collections;

class Screenshake : MonoBehaviour {

    public void StartShake(float duration, float scale) {
        StopAllCoroutines();
        StartCoroutine(Shake(duration, scale));
    }

    //////////////////////////////////////////////////

    [SerializeField] float maxAmplitude;
    [SerializeField] AnimationCurve amplitudeOverTime;

    Vector3 initPosition;

    //////////////////////////////////////////////////

    void Awake() {
        initPosition = transform.position;
    }

    IEnumerator Shake(float duration, float scale) {

        float timer = 0f;

        while(timer < duration) {
            float normTime = timer/duration;
            
            float amp = maxAmplitude * amplitudeOverTime.Evaluate(normTime) * scale;
            var shakeOffset = Random.insideUnitCircle * amp;
            transform.position = initPosition + (Vector3)shakeOffset;

            timer += Time.unscaledDeltaTime;

            yield return null;
        }

        transform.position = initPosition;
    }
}
