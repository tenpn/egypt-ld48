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
    [SerializeField] AudioSource sfx;

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
            "Welcome to my dig site!\n\nI am Professor Ludum, and I have been investigating Ancient Egyptian sports!\n",
            "Would you like to play my latest discovery?\n\n",
            "You would??\n\nSplendid!\n",
            "We know very little about this game, but it involves two players trying to put balls into a goal.\n",
            "Player 1 aims with W/S and shoots with left shift.\n\nPlayer 2 aims with the up/down cursor keys and shoots with M.\n",
            "(It's best with two players, but you can tap T to play against the computer)\n",
            "I'm... er... not really sure how you win.\n\nBut I'll try to find out while you play!\n",
            "Play a few rounds and let me know how you get on!\n",
        };

        yield return StartCoroutine(PlayScript(sequence, 1f));

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

    IEnumerator WaitForGoals(int goalDelta) {
        int goalTarget = activeMatch.TotalGoalsScored + goalDelta;
        while(activeMatch.TotalGoalsScored < goalTarget) {
            yield return null;
        }
    }

    IEnumerator RulesScript() {
        activeMatch.RequestPause = true;
        activeMatch.HoldFire = true;

        var rulesIntro = new string[]{
            "Hang on a second...\n\n",
            "...what's this??\n\n",
            "I appear to have discovered a hieroglyphic tablet that pertains to our little sport!\n\nIt's incomplete, but let's see if I can make it out...\n",
            "...yes...\n",
            "\n...yes...\n",
            "\n\n...yes!",
            "The tablet says we've been playing it a little bit wrong.\n",
            "It seems the goals should be worth many more points!\n",
        };

        yield return StartCoroutine(PlayScript(rulesIntro, 1f));

        var newMod = new BallMod {PointsMul = 10f};
        activeMatch.ActiveBallMods.Add(newMod);

        activeMatch.RequestPause = false;
        activeMatch.HoldFire = false;

        speechBox.text = "Try playing some more. I'll be back if I find anything else!\n";

        yield return StartCoroutine(WaitThenHide(5));

        yield return StartCoroutine(WaitForGoals(100));

        activeMatch.RequestPause = true;
        activeMatch.HoldFire = true;
        var apology = new string[] {
            "Ha...\n",
            "Um!\n",
            "\nWhoops!\n",
            "Turns out, I was reading this wrong.\nIt's not an exact science, you know!",
        };
        yield return StartCoroutine(PlayScript(apology, 1f));

        activeMatch.ActiveBallMods.Remove(newMod);
        activeMatch.RequestPause = false;
        activeMatch.HoldFire = false;

        speechBox.text = "Back to the dig!\n\nI'll figure this out, or my name isn't Professor Leslie Hamilton She-Ra Ludum The Third!";

        yield return StartCoroutine(WaitThenHide(5));
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(7);

        activeMatch.ActiveBallMods.Add(new BallMod {
                MassMul = 2.5f,
                PowerAdd = 800f,
            });

        var massModScript = new string[] {
            "A beautiful tablet!\n\nAnd it says the balls should be heavier!",
        };
        yield return StartCoroutine(PlayTimedScript(massModScript, 5f));
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(20);
        
        root.SetActive(true);
        speechBox.text =
            "Here's a tablet that suggests the balls are supposed to go... up?\n";
        yield return new WaitForSecondsRealtime(4);
        var flipGravity = new MatchMod {
            Type = MatchModType.Gravity,
            Strength = -0.5f,
        };
        activeMatch.AddMod(flipGravity);
        yield return new WaitForSecondsRealtime(5);
        speechBox.text = "\n...I guess not!\n";
        yield return new WaitForSecondsRealtime(2);
        activeMatch.RemoveMod(flipGravity);
        yield return new WaitForSecondsRealtime(2);
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(20);
        root.SetActive(true);
        speechBox.text = "Thank you for playing!\nThis is a work-in-progress LD48 game.\nSend me feedback!";
    }

    IEnumerator PlayScript(IList<string> script, float initDelay) {
        root.SetActive(true);
        continueHelper.SetActive(true);
        
        foreach(var text in script) {
            speechBox.text = text;

            if (initDelay > 0) {
                yield return new WaitForSecondsRealtime(initDelay);
                initDelay = 0f;
            }

            while(Input.GetButtonDown("P1Fire") == false
                  && Input.GetButtonDown("P2Fire") == false) {
                yield return null;
            }
            sfx.Play();
            yield return null;
        }

        continueHelper.SetActive(false);
    }

    IEnumerator PlayTimedScript(IList<string> script, float delay) {
        root.SetActive(true);
        
        foreach(var line in script) {
            speechBox.text = line;
            yield return new WaitForSecondsRealtime(delay);
        }
    }
}
