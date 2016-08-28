using System;
using System.Collections.Generic;

[Serializable]
public class BallMod {
    public float PointsMul = 1f;
    public float PowerAdd = 0f;
    public float MassMul = 1f;
}

public static class BallModExtensions {
    public static string SummariseMods(this IEnumerable<BallMod> mods) {
        var res = "";
        if (mods.Points() != 1f) {
            res += "x" + mods.Points() + "pts";
        }
        if (mods.MassMul() != 1f) {
            if (res != "") {
                res += "\n";
            }
            res += "x" + mods.MassMul() + "kg";
        }
        return res;
    }

    public static float MassMul(this IEnumerable<BallMod> mods) {
        float massMul = 1f;
        foreach(var mod in mods) {
            massMul *= mod.MassMul;
        }
        return massMul;
    }

    public static float Points(this IEnumerable<BallMod> mods) {
        float mul = 1f;
        foreach(var mod in mods) {
            mul *= mod.PointsMul;
        }
        return mul;
    }

    public static float AdditionalPower(this IEnumerable<BallMod> mods) {
        float power = 0f;
        foreach(var mod in mods) {
            power += mod.PowerAdd;
        }
        return power;
    }
}
