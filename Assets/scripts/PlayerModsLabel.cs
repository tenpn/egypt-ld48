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
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
class PlayerModsLabel : MonoBehaviour {

    //////////////////////////////////////////////////

    Text label;
    // slightly convoluted so we work if switched
    [SerializeField] Player target;

    //////////////////////////////////////////////////

    void Awake() {
        label = GetComponent<Text>();
        
        var activeMatch = FindObjectOfType<Match>();
        activeMatch.ModsChanged += OnModsChanged;
        label.text = "";
    }

    void OnModsChanged(Player.PlayerIndex i, IEnumerable<BallMod> mods) {
        if (i == target.Index) {
            label.text = mods.SummariseMods(false);
        }
    }
}
