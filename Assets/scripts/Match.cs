using UnityEngine;
using System;

class Match : MonoBehaviour {

    public event Action<Player,int> ScoreUpdated;

    public void Score(Player scorer) {
        int pIndex = scorer == players[0] ? 0 : 1;
        ++scores[pIndex];
        if (ScoreUpdated != null) {
            ScoreUpdated(scorer, scores[pIndex]);
        }
        goalCelebration.Play();
    }

    public Player GetPlayer(Player.PlayerIndex p) {
        return players[0].Index == p ? players[0] : players[1];
    }

    public bool HoldFire {
        get { return players[0].HoldFire; }
        set {
            players[0].HoldFire = players[1].HoldFire = value;
        }
    }

    //////////////////////////////////////////////////

    int[] scores = new int[2];
    Player[] players;

    [SerializeField] ParticleSystem goalCelebration;

    //////////////////////////////////////////////////

    void Awake() {
        players = FindObjectsOfType<Player>();
    }
}
