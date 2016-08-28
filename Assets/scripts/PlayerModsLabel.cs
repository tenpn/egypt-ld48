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
