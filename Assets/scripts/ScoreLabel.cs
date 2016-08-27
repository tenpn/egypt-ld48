using UnityEngine;
using UnityEngine.UI;

class ScoreLabel : MonoBehaviour {

    //////////////////////////////////////////////////

    Animator anims;
    Match activeMatch;
    [SerializeField] Player.PlayerIndex targetPlayer;

    [SerializeField] Text nameLabel;
    [SerializeField] Text scoreLabel;

    //////////////////////////////////////////////////

    void Start() {
        anims = GetComponent<Animator>();
        activeMatch = FindObjectOfType<Match>();
        activeMatch.ScoreUpdated += OnNewScore;
        nameLabel.text = activeMatch.GetPlayer(targetPlayer).Name;
        scoreLabel.text = "0";
    }

    void OnNewScore(Player p, int newScore) {
        if (p.Index == targetPlayer) {
            scoreLabel.text = newScore.ToString();
            anims.SetTrigger("on-score-inc");
        }
    }
}
