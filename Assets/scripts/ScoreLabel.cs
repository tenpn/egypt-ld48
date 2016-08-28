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

    void OnNewScore(Player p, int newScore) {
        if (p == targetPlayer) {
            scoreLabel.text = newScore.ToString();
            anims.SetTrigger("on-score-inc");
        }
    }
}
