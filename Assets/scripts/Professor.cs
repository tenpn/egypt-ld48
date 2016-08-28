using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

using Random = UnityEngine.Random;

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

    void OnScore(Player p, float newScore) {
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
                StartCoroutine(EndlessRules());
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
        float goalTarget = activeMatch.TotalGoalsScored + (float)goalDelta;
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

        var pointsMod = new MatchMod {
            Type = MatchModType.Ball,
            Ball = new BallMod {PointsMul = 10f},
        };
        activeMatch.AddMod(pointsMod);

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

        activeMatch.RemoveMod(pointsMod);
        activeMatch.RequestPause = false;
        activeMatch.HoldFire = false;

        speechBox.text = "Back to the dig!\n\nI'll figure this out, or my name isn't Professor Leslie Hamilton Thundercats Ludum The Third!";

        yield return StartCoroutine(WaitThenHide(5));

        yield return new WaitForSecondsRealtime(7);

        var heavyBalls = ApplyMod(
            new MatchMod {
                Type = MatchModType.Ball,
                Ball = new BallMod { MassMul = 2.5f },
            },
            "A beautiful tablet!\n\nAnd it says the balls should be heavier!\n",
            ForDuration(45),
            "Not again! A peer review has rejected my heavier-balls hypothesis.\n");
        StartCoroutine(heavyBalls);
                                
        yield return new WaitForSecondsRealtime(30);
        
        var flipGravity = ApplyMod(
            new MatchMod {
                Type = MatchModType.Gravity,
                Strength = -0.5f,
            },
            "Here's a tablet that suggests the balls are supposed to go... up?\n",
            ForDuration(5),
            "\n...I guess not!\n");
        yield return StartCoroutine(flipGravity);

        yield return new WaitForSecondsRealtime(10);
        root.SetActive(true);
        speechBox.text = "Thank you for playing so far!\nThis is a work-in-progress LD48 game.\nSend me feedback!";

        yield return StartCoroutine(WaitThenHide(5));

        StartCoroutine(EndlessRules());
    }

    class Rule {
        public bool IsAlwaysEnded = false;
        public string Intro;
        public string Outro;
        public float Weight = 1f;

        // how many rounds until we can select again
        public int SelectionCount = 0;
        public bool IsSelectable {
            get { return SelectionCount <= 0; }
        }
        // minimum how many rounds between selections
        public int SelectionSeparate = 2;
        
        public Func<MatchMod> CreateMod;

        public override string ToString() {
            return Weight + " " + Intro;
        }
    }

    Rule[] rules = new [] {
        new Rule {
            CreateMod = () => new MatchMod { Type = MatchModType.MirrorSides },
            Intro = "Ah it turns out, you should have started on the other sides!\n",
            Outro = "The evidence is conflicting! Let's swap sides again.\n",
            Weight = 0.2f,
            SelectionCount = 3,
            SelectionSeparate = 6,
        },
        new Rule {
            CreateMod = () => new MatchMod {
                Type = MatchModType.Gravity,
                Strength = -0.5f,
            },
            IsAlwaysEnded = true,
            Intro = "A metareview has found that balls indeed should go up!!\n",
            Outro = "Hmm that's as useless as last time...\n",
            Weight = 0.2f,
            SelectionCount = 4,
            SelectionSeparate = 10,
        },
        new Rule {
            CreateMod = () => new MatchMod {
                Type = MatchModType.Ball,
                Ball = new BallMod { PowerAdd = 200f },
            },
            Weight = 0.5f,
            Intro = "Slave graffiti shows balls moving much faster.\n\nLet's speed it up!",
            Outro = "Turns out I sneezed and assumed it was graffiti.\n\nSorry.",
        },
        new Rule {
            CreateMod = () => new MatchMod {
                Type = MatchModType.Ball,
                Ball = new BallMod { PowerAdd = -100f },
            },
            Intro = "I found a tablet that suggests fast throwing was illegal!\n",
            Outro = "I dropped that tablet...\n\nJust forget I mentioned it.\n",
            SelectionCount = 2,
            Weight = 0.3f,
        },
        new Rule {
            CreateMod = () => new MatchMod {
                Type = MatchModType.Ball,
                Ball = new BallMod { PointsMul = 3f },
            },
            Weight = 0.6f,
            Intro = "A new source that agrees goals should be worth more!\n",
            Outro = "That goal score source was... discredited.\n",
        },
        new Rule {
            CreateMod = () => new MatchMod {
                Type = MatchModType.Ball,
                Ball = new BallMod { PointsMul = 0.5f },
            },
            Intro = "Cross-discipline scholars think our goals are worth too much.\n",
            Outro = "Oh turns out those scholars were talking metaphorically.\n",
            SelectionCount = 2,
            Weight = 0.4f,
        },
        new Rule {
            CreateMod = () => new MatchMod {
                Type = MatchModType.Ball,
                Ball = new BallMod { MassMul = 1.5f },
            },
            Intro = "My research assistant found references to heavier balls!\n",
            Outro = "My assistant... was talking about something else.\n",
            Weight = 0.3f,
        },
        new Rule {
            CreateMod = () => new MatchMod {
                Type = MatchModType.Ball,
                Ball = new BallMod { MassMul = 0.7f },
            },
            Intro = "I don't think the Egyptians would have had the tech to make balls this heavy.\n\nLet's try going lighter.",
            Outro = "But I suppose if they had alien help, they could have made those heavier balls...\n",
            SelectionCount = 2,
            Weight = 0.3f,
        },
    };

    Rule SelectRule() {
        float totalWeight = 0f;
        foreach(var rule in rules) {
            if (rule.IsSelectable) {
                totalWeight += rule.Weight;
            }
        }

        var choice = Random.value * totalWeight;
        Debug.Log("choice: " + choice + "/" + totalWeight);
        Rule chosenRule = null;
        foreach(var rule in rules) {
            if (rule.IsSelectable) {
                choice -= rule.Weight;
                if (choice <= 0f) {
                    chosenRule = rule;
                    break;
                }
            }
        }

        foreach(var rule in rules) {
            --rule.SelectionCount;
        }

        if (chosenRule != null) {
            chosenRule.SelectionCount = chosenRule.SelectionSeparate;
        }
        return chosenRule;
    }

    IEnumerator EndlessRules() {

        while(true) {
            float interval = Random.Range(5f, 8f);
            Debug.Log("delay " + interval);
            yield return new WaitForSecondsRealtime(interval);

            float totalWeight = 0f;
            foreach(var rule in rules) {
                if (rule.IsSelectable) {
                    totalWeight += rule.Weight;
                }
            }

            Rule chosenRule = SelectRule();

            if (chosenRule != null) {
                chosenRule.SelectionCount = chosenRule.SelectionSeparate;

                var endChoice = Random.value;
                EndCondition endCond
                    = endChoice < 0.4 && chosenRule.IsAlwaysEnded == false ? null
                    : ForDuration(Random.Range(7f, 14f));
                Debug.Log("end choice:" + endCond);
                
                var modder = ApplyMod(chosenRule.CreateMod(),
                                      chosenRule.Intro,
                                      endCond,
                                      chosenRule.Outro);
                yield return StartCoroutine(modder);
            }
        }
    }

    class EndCondition {
        public int GoalsScored = -1;
        public float Duration = -1f;
        
        float TargetTime = float.MaxValue;
        float TargetGoals = float.MaxValue;

        public void Start(Match activeMatch) {
            if (GoalsScored > 0) {
                TargetGoals = activeMatch.TotalGoalsScored + GoalsScored;
            } else {
                TargetTime = Time.time + Duration;
            }
        }

        public bool IsInProgress(Match activeMatch) {
            return activeMatch.TotalGoalsScored < TargetGoals
                && Time.time < TargetTime;
        }
    }

    EndCondition ForDuration(float duration) {
        return new EndCondition { Duration = duration };
    }

    EndCondition ForGoals(int goals) {
        return new EndCondition { GoalsScored = goals };
    }

    IEnumerator ApplyMod(MatchMod mod,
                         string intro = null,
                         EndCondition ender = null,
                         string outro = null) {
        
        // intro text has to be read
        activeMatch.AddMod(mod);
        yield return StartCoroutine(ShowTextFor(intro, 5f));

        if (ender == null) {
            yield break;
        }

        ender.Start(activeMatch);
        while(ender.IsInProgress(activeMatch)) {
            yield return null;
        }

        activeMatch.RemoveMod(mod);

        StartCoroutine(ShowTextFor(outro, 5f));
    }

    // if text is null or empty then ignored
    IEnumerator ShowTextFor(string text, float duration) {

        if (string.IsNullOrEmpty(text)) {
            yield break;
        }

        root.SetActive(true);
        speechBox.text = text;
        
        yield return new WaitForSecondsRealtime(5);

        if (speechBox.text == text) {
            // we still have control
            root.SetActive(false);
        }
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
