using UnityEngine;
using System;
using System.Collections.Generic;

class Match : MonoBehaviour {

    public event Action<Player,int> ScoreUpdated;

    public void Score(Ball ball) {
        var scorer = ball.owner;
        int pIndex = scorer == players[0] ? 0 : 1;
        scores[pIndex] += ball.Points;
        if (ScoreUpdated != null) {
            ScoreUpdated(scorer, scores[pIndex]);
        }
        goalCelebration.Play();
        freeze += 0.15f;
    }

    public Player GetPlayer(Player.PlayerIndex p) {
        return players[0].Index == p ? players[0] : players[1];
    }

    public int TotalGoalsScored {
        get { return scores[0] + scores[1]; }
    }

    public bool HoldFire {
        get { return players[0].HoldFire; }
        set {
            players[0].HoldFire = players[1].HoldFire = value;
        }
    }

    public IList<BallMod> ActiveBallMods = new List<BallMod>();

    public void ApplyMods(Ball ball) {
        ball.ApplyMods(ActiveBallMods);
    }

    public bool RequestPause = false;

    //////////////////////////////////////////////////

    int[] scores = new int[2];
    Player[] players;
    float freeze = 0f;

    [SerializeField] ParticleSystem goalCelebration;

    //////////////////////////////////////////////////

    void Awake() {
        players = FindObjectsOfType<Player>();
    }

    void Update() {
        freeze -= Time.unscaledDeltaTime;
        freeze = Mathf.Max(0f, freeze);
        Time.timeScale = freeze == 0f && RequestPause == false ? 1f : 0f;
    }
}
