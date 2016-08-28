using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
class PlayerModsLabel : MonoBehaviour {

    //////////////////////////////////////////////////

    Text label;

    //////////////////////////////////////////////////

    void Awake() {
        label = GetComponent<Text>();
        
        var activeMatch = FindObjectOfType<Match>();
        activeMatch.ModsChanged += OnModsChanged;
        label.text = "";
    }

    void OnModsChanged(IEnumerable<BallMod> mods) {
        label.text = mods.SummariseMods(false);
    }
}
