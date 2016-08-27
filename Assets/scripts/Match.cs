using UnityEngine;
using System;

class Match : MonoBehaviour {

    public event Action<Player,int> ScoreUpdated;

    public void Score(Player scorer) {
        int pIndex = scorer == players[0] ? 0 : 1;
        ++scores[pIndex];
        Debug.Log("player " + scorer.Name + " scored!");
        if (ScoreUpdated != null) {
            ScoreUpdated(scorer, scores[pIndex]);
        }
    }

    public Player GetPlayer(Player.PlayerIndex p) {
        return players[0].Index == p ? players[0] : players[1];
    }

    //////////////////////////////////////////////////

    int[] scores = new int[2];
    Player[] players;

    //////////////////////////////////////////////////

    void Awake() {
        players = FindObjectsOfType<Player>();
    }
}
