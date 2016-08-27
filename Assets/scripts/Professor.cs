using UnityEngine;
using System.Collections;
using UnityEngine.UI;

class Professor : MonoBehaviour {

    //////////////////////////////////////////////////

    Match activeMatch;
    [SerializeField] Text speechBox;
    [SerializeField] GameObject continueHelper;

    //////////////////////////////////////////////////

    void Start() {
        activeMatch = FindObjectOfType<Match>();
        StartCoroutine(MatchTutorial());
    }

    IEnumerator MatchTutorial() {
        activeMatch.HoldFire = true;

        continueHelper.SetActive(true);

        string[] sequence = {
            "Oh??\n\nOh!\n",
            "Welcome to my dig site!\n\nI am Professor Dare, and I have been investigating Ancient Egyptian sports!\n",
            "Would you like to play my latest discovery?\n\n",
            "You would??\n\nSplendid!\n",
            "We know very little about this game, but it involves two players trying to put balls into a goal.\n",
            "Player 1 aims with W/S and shoots with left shift.\n\nPlayer 2 aims with the up/down cursor keys and shoots with M.\n",
            "I'm... er... not really sure how you win.\n\nBut I'll try to find out while you play!\n",
            "Play a few rounds and let me know how you get on!\n",
        };

        foreach(var text in sequence) {
            speechBox.text = text;

            while(Input.GetButtonDown("P1Fire") == false
                  && Input.GetButtonDown("P2Fire") == false) {
                yield return null;
            }
            yield return null;
        }

        continueHelper.SetActive(false);
        activeMatch.HoldFire = false;
        gameObject.SetActive(false);
    }
}
