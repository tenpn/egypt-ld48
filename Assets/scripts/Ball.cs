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

        phys.mass = initialMass * MassMul;
    }

    public float MassMul {
        get {
            float massMul = 1f;
            foreach(var mod in mods) {
                massMul *= mod.MassMul;
            }
            return massMul;
        }
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

    public string SummariseMods() {
        var res = "";
        if (Points != 1f) {
            res += "x" + Points + "pts";
        }
        if (MassMul != 1f) {
            if (res != "") {
                res += "\n";
            }
            res += "x" + MassMul + "kg";
        }
        return res;
    }

    //////////////////////////////////////////////////
    
    readonly List<BallMod> mods = new List<BallMod>();
    float initialMass;

    //////////////////////////////////////////////////

    void Awake() {
        initialMass = phys.mass;
    }
}
