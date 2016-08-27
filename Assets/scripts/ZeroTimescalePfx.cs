using UnityEngine;

// http://answers.unity3d.com/questions/445843/how-to-emit-particle-or-un-pause-particle-when-tim.html
public class ZeroTimescalePfx : MonoBehaviour
{
    ParticleSystem pfx;

    void Awake() {
        pfx = GetComponent<ParticleSystem>();
    }
    
    void Update() {
        if (Time.timeScale < 0.01f) {
            pfx.Simulate(Time.unscaledDeltaTime, true, false);
        }
    }
}
