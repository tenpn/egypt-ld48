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

class PlayerAmmoSlider : MonoBehaviour {

    //////////////////////////////////////////////////

    Slider slide;
    [SerializeField] Player target;
    [SerializeField] Image fill;

    //////////////////////////////////////////////////

    void Awake() {
        slide = GetComponent<Slider>();
    }

    void Update() {
        slide.value = target.Ammo / target.MaxAmmo;
        fill.color = target.Ammo >= 1f ? Color.green : Color.red;
    }
}
