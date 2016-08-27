using UnityEngine;
using System.Collections.Generic;

class Ball : MonoBehaviour {
    public Rigidbody2D phys = null;
    public Player owner = null;

    public Color Color {
        set { GetComponent<SpriteRenderer>().color = value; }
    }

    public void ApplyMods(IList<BallMod> newMods) {
        mods.AddRange(newMods);

        float massMul = 1f;
        foreach(var mod in newMods) {
            massMul *= mod.MassMul;
        }
        phys.mass *= massMul;
    }

    public int Points {
        get {
            float mul = 1f;
            foreach(var mod in mods) {
                mul *= mod.PointsMul;
            }
            return Mathf.FloorToInt(mul);
        }
    }

    public float AdditionalPower {
        get {
            float power = 0f;
            foreach(var mod in mods) {
                power += mod.PowerAdd;
            }
            return power;
        }
    }

    //////////////////////////////////////////////////
    
    readonly List<BallMod> mods = new List<BallMod>();
}
