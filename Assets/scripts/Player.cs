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

class Player : MonoBehaviour {

    public string Name {
        get { return p.ToString(); }
    }
    
    public enum PlayerIndex {
        P1,
        P2,
    }

    public PlayerIndex Index {
        get { return p; }
        set { p = value; }
    }

    public void ApplyYoke(PlayerYoke newYoke) {
        var currentAngles = transform.eulerAngles;

        // wrap back into range so 0 is flat
        while(currentAngles.z > 180) {
            currentAngles.z -= 360;
        }

        while(currentAngles.z < -180) {
            currentAngles.z += 360;
        }

        // players are on different sides, so different actions depending on flip

        currentAngles.z +=
            newYoke.Aim * rotateSpeed * Time.deltaTime * transform.localScale.x;
        if (transform.localScale.x > 0f) {
            currentAngles.z = Mathf.Clamp(currentAngles.z, minAngle, maxAngle);
        } else {
            currentAngles.z = Mathf.Clamp(currentAngles.z, -maxAngle, -minAngle);
        }
        transform.eulerAngles = currentAngles;

        ammo += Time.unscaledDeltaTime * rechargeRate * activeMatch.ShootingBoost;
        ammo = Mathf.Min(MaxAmmo, ammo);
        cooldown -= Time.deltaTime;

        bool canFire = HoldFire == false && ammo >= 1f && cooldown <= 0f;

        statusLight.color = canFire ? Color.green : Color.red;

        if (canFire && newYoke.Fire) {
            var newBall = (Ball)Instantiate(ballPrefab, activeMatch.transform);
            newBall.owner = p;
            newBall.transform.position = emittPoint.position;
            var pColor = p == PlayerIndex.P1 ? p1BallCol : p2BallCol;
            newBall.Color = pColor;

            activeMatch.ApplyMods(newBall, Index);
            
            float flipper = transform.localScale.x;
            float force = Random.Range(-fireForceRandomDelta, fireForceRandomDelta)
                + newBall.AdditionalPower
                + powerByWeight.Evaluate(newBall.phys.mass);
            newBall.phys.AddForce(transform.right * force * flipper);
            cooldown = cooldownDuration;
            ammo -= 1f;
            shootPfx.startColor = pColor;
            shootPfx.Play();

            sfx.pitch = 1f + Random.Range(-shootPitchShift, shootPitchShift);
            sfx.PlayOneShot(shootClip);
        }
    }

    // match starts with no firing
    public bool HoldFire = true;

    public float Ammo {
        get { return ammo; }
    }

    public float MaxAmmo {
        get { return maxAmmo * activeMatch.ShootingBoost; }
    }

    //////////////////////////////////////////////////

    [SerializeField] Ball ballPrefab;
    [SerializeField] Transform emittPoint;
    [SerializeField] AnimationCurve powerByWeight;
    [SerializeField] float fireForceRandomDelta = 50f;
    [SerializeField] float rotateSpeed;
    [SerializeField] float maxAmmo = 4f;
    [SerializeField] float rechargeRate = 2f;
    [SerializeField] float cooldownDuration = 0.2f;
    [SerializeField] SpriteRenderer statusLight;
    [SerializeField] float minAngle = -20f;
    [SerializeField] float maxAngle = 30f;
    [SerializeField] Color p1BallCol = Color.blue;
    [SerializeField] Color p2BallCol = Color.green;
    [SerializeField] ParticleSystem shootPfx;
    [SerializeField] AudioSource sfx;
    [SerializeField] AudioClip shootClip;
    [SerializeField] float shootPitchShift = 0.2f;

    [SerializeField] PlayerIndex p;

    float cooldown = 0f;
    float ammo = 1f;
    Match activeMatch;

    //////////////////////////////////////////////////

    void Awake() {
        activeMatch = FindObjectOfType<Match>();
    }
}
