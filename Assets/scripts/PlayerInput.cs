using UnityEngine;

[RequireComponent(typeof(Player))]
class PlayerInput : MonoBehaviour {

    //////////////////////////////////////////////////

    Player p;

    //////////////////////////////////////////////////

    void Awake() {
        p = GetComponent<Player>();
    }

    void Update() {
        var axis = p.Index == Player.PlayerIndex.P1 ? "P1Aim" : "P2Aim";
        var button = p.Index == Player.PlayerIndex.P1 ? "P1Fire" : "P2Fire";

        var newYoke = new PlayerYoke {
            Aim = Input.GetAxis(axis),
            Fire = Input.GetButtonDown(button),
        };

        p.ApplyYoke(newYoke);
    }
}
