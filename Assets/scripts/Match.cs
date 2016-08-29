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
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.Analytics;

using Random = UnityEngine.Random;

class Match : MonoBehaviour {

    public event Action<Player,float> ScoreUpdated;
    public event Action PlayerUpdated;

    public void Score(Ball ball) {
        var scorer = ball.owner;
        int pIndex = scorer == Player.PlayerIndex.P1 ? 0 : 1;
        scores[pIndex] += ball.Points;
        if (ScoreUpdated != null) {
            ScoreUpdated(GetPlayer(ball.owner), scores[pIndex]);
        }

        var clip = ball.Points >= 2 ? bigGoalSfx : goalSfx;
        sfx.pitch = 1 + Random.Range(-pitchShift, pitchShift);
        sfx.PlayOneShot(clip);

        goalCelebration.startColor = ball.Color;
        goalCelebration.Play();
        freeze += 0.15f;
    }

    public Player GetPlayer(Player.PlayerIndex p) {
        return players[0].Index == p ? players[0] : players[1];
    }

    public float GetScoreOfPlayer(Player.PlayerIndex p) {
        var scoreIndex = p == Player.PlayerIndex.P1 ? 0 : 1;
        return scores[scoreIndex];
    }

    public float TotalGoalsScored {
        get { return scores[0] + scores[1]; }
    }

    public bool HoldFire {
        get { return players[0].HoldFire; }
        set {
            players[0].HoldFire = players[1].HoldFire = value;
        }
    }

    public IList<MatchMod> Mods {
        get { return activeMods; }
    }

    public event Action<Player.PlayerIndex, IEnumerable<BallMod>> ModsChanged;

    public void AddMod(MatchMod mod) {
        Assert.IsNotNull(mod);
        Assert.IsFalse(activeMods.Contains(mod), "mod " + mod + " already active");
        activeMods.Add(mod);

        var analData = new Dictionary<string,object> {
            { "type", mod.Type.ToString() }
        };
        
        switch(mod.Type) {
        case MatchModType.MirrorSides:
            SwitchSides();
            break;

        case MatchModType.Gravity:
            Physics2D.gravity = new Vector2(Physics2D.gravity.x,
                                            Physics2D.gravity.y * mod.Strength);
            analData["strength"] = mod.Strength;
            break;

        case MatchModType.AmmoBoost:
            ShootingBoost *= mod.Strength;
            analData["strength"] = mod.Strength;
            break;
            
        case MatchModType.Ball:
            Assert.IsNotNull(mod.Ball);
            activeBallMods.Add(mod.Ball);
            analData["ball-pts"] = mod.Ball.PointsMul;
            analData["ball-pow"] = mod.Ball.PowerAdd;
            analData["ball-mass"] = mod.Ball.MassMul;
            break;
            
        case MatchModType.AltBall:
            Assert.IsNotNull(mod.Ball);
            altBallMods.Add(mod.Ball);
            isAltBallModsEnabled = true;
            analData["ball-pts"] = mod.Ball.PointsMul;
            analData["ball-pow"] = mod.Ball.PowerAdd;
            analData["ball-mass"] = mod.Ball.MassMul;
            break;
        }

        if (ModsChanged != null) {
            ModsChanged(Player.PlayerIndex.P1, GetBallModsForPlayer(Player.PlayerIndex.P1));
            ModsChanged(Player.PlayerIndex.P2, GetBallModsForPlayer(Player.PlayerIndex.P2));
        }
        
        Analytics.CustomEvent("new-mod", analData);
    }

    public void RemoveMod(MatchMod modToRemove) {
        Assert.IsNotNull(modToRemove);
        Assert.IsTrue(activeMods.Contains(modToRemove),
                      "mod " + modToRemove + " is not active");
        activeMods.Remove(modToRemove);
        switch(modToRemove.Type) {
            
        case MatchModType.MirrorSides:
            SwitchSides();
            break;
            
        case MatchModType.Gravity:
            Physics2D.gravity = new Vector2(Physics2D.gravity.x,
                                            Physics2D.gravity.y / modToRemove.Strength);
            break;

        case MatchModType.AmmoBoost:
            ShootingBoost /= modToRemove.Strength;
            break;
            
        case MatchModType.Ball:
            Assert.IsNotNull(modToRemove.Ball);
            activeBallMods.Remove(modToRemove.Ball);
            break;
            
        case MatchModType.AltBall:
            Assert.IsNotNull(modToRemove.Ball);
            altBallMods.Remove(modToRemove.Ball);
            break;
        }

        if (ModsChanged != null) {
            ModsChanged(Player.PlayerIndex.P1, GetBallModsForPlayer(Player.PlayerIndex.P1));
            ModsChanged(Player.PlayerIndex.P2, GetBallModsForPlayer(Player.PlayerIndex.P2));
        }
    }

    public void ApplyMods(Ball ball, Player.PlayerIndex sourcePlayer) {

        var sourceMods = GetBallModsForPlayer(sourcePlayer);
        
        ball.ApplyMods(sourceMods);

        labels.AttachLabel(ball.SummariseMods(), ball.transform);

        activeBalls.Add(ball.gameObject);
        TrimOldBalls();

        int index = sourcePlayer == Player.PlayerIndex.P1 ? 0 : 1;
        ++shotCounts[index];

        if (ModsChanged != null) {
            // get mods again, because we've cycled shot
            ModsChanged(sourcePlayer, GetBallModsForPlayer(sourcePlayer));
        }
    }

    public float MinBallPoints {
        get {
            float minP = float.MaxValue;
            foreach(var mainMod in activeBallMods) {
                minP = Mathf.Min(mainMod.PointsMul, minP);
            }
            foreach(var altMod in altBallMods) {
                minP = Mathf.Min(altMod.PointsMul, minP);
            }
            return minP;
        }
    }

    public void ReportScoresToAnalytics(string eventID) {
        Analytics.CustomEvent("stage", new Dictionary<string, object> {
                { "id", eventID },
                { "time", Time.timeSinceLevelLoad },
                { "score-p1", GetScoreOfPlayer(Player.PlayerIndex.P1) },
                { "score-p2", GetScoreOfPlayer(Player.PlayerIndex.P2) },
                { "auto-p2", GetPlayer(Player.PlayerIndex.P2)
                        .GetComponent<PlayerAutopilot>().IsAutopiloted }
            });
    }

    public bool RequestPause = false;

    public float ShootingBoost = 1f;

    //////////////////////////////////////////////////

    float[] scores = new float[2];
    int[] shotCounts = new int[2];
    Player[] players;
    float freeze = 0f;

    FloatingLabels labels;

    List<GameObject> activeBalls = new List<GameObject>();
    bool isInActiveCullMode = false;

    IList<MatchMod> activeMods = new List<MatchMod>();
    IList<BallMod> activeBallMods = new List<BallMod>();
    IList<BallMod> altBallMods = new List<BallMod>();
    bool isAltBallModsEnabled = false;

    [SerializeField] ParticleSystem goalCelebration;
    [SerializeField] AudioClip goalSfx;
    [SerializeField] AudioClip bigGoalSfx;
    [SerializeField] AudioSource sfx;
    [SerializeField] float pitchShift = 0.1f;
    [SerializeField] int ballCullTriggerCount = 30;
    [SerializeField] int ballTargetCount = 15;
    [SerializeField] int ballMaxReduction = 2;
    
    //////////////////////////////////////////////////

    void Awake() {
        players = FindObjectsOfType<Player>();
        labels = FindObjectOfType<FloatingLabels>();
    }

    void Update() {
        freeze -= Time.unscaledDeltaTime;
        freeze = Mathf.Max(0f, freeze);
        Time.timeScale = freeze == 0f && RequestPause == false ? 1f : 0f;
    }

    IList<BallMod> GetBallModsForPlayer(Player.PlayerIndex p) {
        int index = p == Player.PlayerIndex.P1 ? 0 : 1;
        int shotCount = shotCounts[index];
        
        var sourceMods = shotCount % 2 == 0 || isAltBallModsEnabled == false
            ? activeBallMods
            : altBallMods;
        return sourceMods;
    }

    void TrimOldBalls() {

        // clear dead balls first
        for(int ballIndex = 0; ballIndex < activeBalls.Count; ++ballIndex) {
            var ball = activeBalls[ballIndex];
            if (ball == null) {
                activeBalls.RemoveAt(ballIndex);
                --ballIndex;
            } 
        }

        if (activeBalls.Count <= ballTargetCount) {
            isInActiveCullMode = false;
            return;
        }
        
        isInActiveCullMode = isInActiveCullMode || activeBalls.Count >= ballCullTriggerCount;

        if (isInActiveCullMode == false) {
            return;
        }

        // can only clear a few balls at a time, to try and lower the level gradually
        for(int reduceIndex = 0; reduceIndex < ballMaxReduction; ++reduceIndex) {
            if (activeBalls.Count > ballTargetCount) {
                var ball = activeBalls[0];
                Destroy(ball);
                activeBalls.RemoveAt(0);
            }
        }
    }

    void SwitchSides() {
        var p2 = players[1].Index;
        players[1].Index = players[0].Index;
        players[0].Index = p2;
        if (PlayerUpdated != null) {
            PlayerUpdated();
        }
    }
}
