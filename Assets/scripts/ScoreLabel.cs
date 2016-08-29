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
using UnityEngine.UI;

class ScoreLabel : MonoBehaviour {

    //////////////////////////////////////////////////

    Animator anims;
    Match activeMatch;
    Player targetPlayer;
    [SerializeField] Player.PlayerIndex initialPlayer;

    [SerializeField] Text nameLabel;
    [SerializeField] Text scoreLabel;

    //////////////////////////////////////////////////

    void Start() {
        anims = GetComponent<Animator>();
        activeMatch = FindObjectOfType<Match>();
        
        activeMatch.ScoreUpdated += OnNewScore;
        activeMatch.PlayerUpdated += OnNewPlayer;

        targetPlayer = activeMatch.GetPlayer(initialPlayer);
        
        scoreLabel.text = "0";
        OnNewPlayer();
    }

    void OnNewPlayer() {
        nameLabel.text = targetPlayer.Name;
        scoreLabel.text = activeMatch.GetScoreOfPlayer(targetPlayer.Index).ToString();
    }

    void OnNewScore(Player p, float newScore) {
        if (p == targetPlayer) {
            scoreLabel.text = newScore.ToString();
            anims.SetTrigger("on-score-inc");
        }
    }
}
