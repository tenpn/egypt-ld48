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
using System;
using System.Collections.Generic;

[Serializable]
public class BallMod {
    public float PointsMul = 1f;
    public float PowerAdd = 0f;
    public float MassMul = 1f;
}

public static class BallModExtensions {
    public static string SummariseMods(this IEnumerable<BallMod> mods, bool isForBall) {
        var res = "";
        if (mods.Points() != 1f) {
            res += mods.Points() + "pts";
        }
        if (mods.MassMul() != 1f) {
            if (res != "") {
                res += "\n";
            }
            res += mods.MassMul() + "kg";
        }
        float power = mods.AdditionalPower();
        if (isForBall == false && power != 0f) {
            if (res != "") {
                res += "\n";
            }
            var prefix = power < 0 ? "" : "+";
            res += prefix + mods.AdditionalPower() + "mph";
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
