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

// drives the yoke with random rantings
[RequireComponent(typeof(Player))]
class PlayerAutopilot : MonoBehaviour {

    public bool IsAutopiloted {
        get { return isInControl; }
    }

    //////////////////////////////////////////////////

    Player p;

    [SerializeField] float fireInterval = 2f;
    [SerializeField] float twistInterval = 2f;

    float fireCountdown = 0f;
    float twistCountdown = 0f;
    float lastAim = 0f;
    PlayerInput input = null;
    bool isInControl = false;

    //////////////////////////////////////////////////

    void Awake() {
        p = GetComponent<Player>();
        input = GetComponent<PlayerInput>();
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.T)) {
            isInControl = !isInControl;
        }

        input.enabled = isInControl == false || p.Index == Player.PlayerIndex.P1;

        if (input.enabled) {
            // player is controlling!
            return;
        }

        fireCountdown -= Time.deltaTime;
        twistCountdown -= Time.deltaTime;

        if (twistCountdown < 0f) {
            float dice = Random.value;
            lastAim = dice < 0.4f ? -1f
                : dice > 0.6f ? 1f
                : 0f;
            twistCountdown = Random.value * twistInterval;
        }

        var newYoke = new PlayerYoke {
            Aim = lastAim,
            Fire = fireCountdown <= 0f,
        };
        p.ApplyYoke(newYoke);

        if (newYoke.Fire) {
            fireCountdown = Random.value * fireInterval;
        }
    }
}
