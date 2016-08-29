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
        initPosition = transform.localPosition;
    }

    IEnumerator Shake(float duration, float scale) {

        float timer = 0f;

        while(timer < duration) {
            float normTime = timer/duration;
            
            float amp = maxAmplitude * amplitudeOverTime.Evaluate(normTime) * scale;
            var shakeOffset = Random.insideUnitCircle * amp;
            transform.localPosition = initPosition + (Vector3)shakeOffset;

            timer += Time.unscaledDeltaTime;

            yield return null;
        }

        transform.localPosition = initPosition;
    }
}
