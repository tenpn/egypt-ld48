using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

class Professor : MonoBehaviour {

    //////////////////////////////////////////////////

    enum State {
        Tutorial,
        WaitForFirstScore,
        WaitForRules,
        Rules,
    }

    State currentState;
    Match activeMatch;
    [SerializeField] Text speechBox;
    [SerializeField] GameObject continueHelper;
    [SerializeField] GameObject root;

    //////////////////////////////////////////////////

    void Start() {
        activeMatch = FindObjectOfType<Match>();
        activeMatch.ScoreUpdated += OnScore;
        StartCoroutine(MatchTutorial());
    }

    IEnumerator MatchTutorial() {
        currentState = State.Tutorial;
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

        yield return StartCoroutine(PlayScript(sequence));

        continueHelper.SetActive(false);
        activeMatch.HoldFire = false;
        root.SetActive(false);
        currentState = State.WaitForFirstScore;
    }

    void OnScore(Player p, int newScore) {
        if (currentState == State.WaitForFirstScore) {
            speechBox.text = string.Format("Congratulations, {0}! That's it!\n\nYou both seem to be getting the hang of it. Carry on while I return to the dig.\n",
                                           p.Name);
            root.SetActive(true);
            currentState = State.WaitForRules;
            HideInSeconds(5);
            
        } else if (currentState == State.WaitForRules) {
            if (activeMatch.TotalGoalsScored > 5) {
                currentState = State.Rules;
                StopAllCoroutines();
                StartCoroutine(RulesScript());
            }
        }
    }

    void HideInSeconds(float delay) {
        StartCoroutine(WaitThenHide(delay));
    }

    IEnumerator WaitThenHide(float delay) {
        var waiter = new WaitForSecondsRealtime(delay);
        yield return waiter;
        root.SetActive(false);
    }

    IEnumerator RulesScript() {
        activeMatch.RequestPause = true;
        activeMatch.HoldFire = true;
        root.SetActive(true);

        var rulesIntro = new string[]{
            "Hang on a second...\n\n",
            "...what's this??\n\n",
            "I appear to have discovered a hieroglyphic tablet that pertains to our little sport!\n\nIt's incomplete, but let's see if I can make it out...\n",
            "...yes...\n",
            "\n...yes...\n",
            "\n\n...yes!",
            "Umm, it turns out we've been playing it a little bit wrong.\n",
            "It says here that goals should be worth double points!\n",
        };

        yield return StartCoroutine(PlayScript(rulesIntro));

        activeMatch.ActiveBallMods.Add(new BallMod {PointsMul = 2f});

        activeMatch.RequestPause = false;
        activeMatch.HoldFire = false;

        speechBox.text = "Try playing some more. I'll be back if I find anything else!\n";
        HideInSeconds(5);
    }

    IEnumerator PlayScript(IList<string> script) {
        foreach(var text in script) {
            speechBox.text = text;

            while(Input.GetButtonDown("P1Fire") == false
                  && Input.GetButtonDown("P2Fire") == false) {
                yield return null;
            }
            yield return null;
        }        
    }
}
