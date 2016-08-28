using UnityEngine;
using System.Collections.Generic;

class Ball : MonoBehaviour {
    public Rigidbody2D phys = null;
    public Player.PlayerIndex owner;

    public Color Color {
        set { GetComponent<SpriteRenderer>().color = value; }
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
