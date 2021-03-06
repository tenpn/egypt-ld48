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

[RequireComponent(typeof(Player))]
class PlayerInput : MonoBehaviour {

    //////////////////////////////////////////////////

    Player p;

    //////////////////////////////////////////////////

    void Awake() {
        p = GetComponent<Player>();
    }

    void Update() {
        var axis = p.Index == Player.PlayerIndex.P1 ? "P1Aim" : "P2Aim";
        var button = p.Index == Player.PlayerIndex.P1 ? "P1Fire" : "P2Fire";

        // hack: space also fires for p1
        bool isFiring = Input.GetButtonDown(button)
            || (p.Index == Player.PlayerIndex.P1 && Input.GetKeyDown(KeyCode.Space));

        var newYoke = new PlayerYoke {
            Aim = Input.GetAxis(axis),
            Fire = isFiring,
        };

        p.ApplyYoke(newYoke);
    }
}
