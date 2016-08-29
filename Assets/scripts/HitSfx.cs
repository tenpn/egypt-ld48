/*
  Egyptian Dare, August 2016 Ludum Dare entry  
  Copyright (C) 2016 Andrew Fray

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using UnityEngine;

class HitSfx : MonoBehaviour {
    //////////////////////////////////////////////////

    float ignoreSfxCountdown = 0.5f;
    [SerializeField] AudioClip smallHit;
    [SerializeField] AudioClip bigHit;
    [SerializeField] AudioClip ballHit;
    [SerializeField] AnimationCurve volBySqrVal;
    [SerializeField] AnimationCurve ballVolBySqrVal;
    [SerializeField] AudioSource sfx;
    [SerializeField] float pitchShift = 0.1f;
    [SerializeField] float bigHitMassThreshold = 2f;

    bool isBig = false;

    //////////////////////////////////////////////////

    void Start() {
        isBig = GetComponent<Rigidbody2D>().mass >= bigHitMassThreshold;
    }

    void Update() {
        ignoreSfxCountdown -= Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (ignoreSfxCountdown > 0) {
            return;
        }

        bool isBallOnBall = other.gameObject.CompareTag("ball");
        bool isBigHit = isBallOnBall && isBig;

        var ducker = isBallOnBall ? ballVolBySqrVal : volBySqrVal;
        float targetVol = ducker.Evaluate(other.relativeVelocity.sqrMagnitude);
        
        if (targetVol > 0f) {
            sfx.pitch = 1f + Random.Range(-pitchShift, pitchShift);
            var clip = isBigHit ? bigHit
                : isBallOnBall ? ballHit
                : smallHit;
            sfx.PlayOneShot(clip, targetVol);
        }
    }
}
