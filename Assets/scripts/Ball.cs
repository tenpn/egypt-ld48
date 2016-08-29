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
using System.Collections.Generic;

class Ball : MonoBehaviour {
    public Rigidbody2D phys = null;
    public Player.PlayerIndex owner;

    public Color Color {
        set { GetComponent<SpriteRenderer>().color = value; }
        get { return GetComponent<SpriteRenderer>().color; }
    }

    public void ApplyMods(IList<BallMod> newMods) {
        mods.AddRange(newMods);

        phys.mass = initialMass * MassMul;
    }

    public float MassMul {
        get { return mods.MassMul(); }
    }

    public float Points {
        get { return mods.Points(); }
    }

    public float AdditionalPower {
        get { return mods.AdditionalPower(); }
    }

    public string SummariseMods() {
        return mods.SummariseMods(true);
    }

    //////////////////////////////////////////////////
    
    readonly List<BallMod> mods = new List<BallMod>();
    float initialMass;

    //////////////////////////////////////////////////

    void Awake() {
        initialMass = phys.mass;
    }
}
