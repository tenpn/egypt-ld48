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
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.Analytics;

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
    [SerializeField] Screenshake shaker;
    [SerializeField] Image speechBG;
    [SerializeField] Sprite ludumBG;
    [SerializeField] Sprite dareBG;
    [SerializeField] GameObject[] playerScoreLabels;
    [SerializeField] GameObject endgameRoot;
    [SerializeField] Text endgameScoreLabel;
    [SerializeField] Text endgameTimeLabel;
    [SerializeField] AudioClip bigShake;
    [SerializeField] AudioClip smallShake;
    [SerializeField] Animator arena;
    [SerializeField] GameObject entireUI;

    //////////////////////////////////////////////////

    void Start() {
        activeMatch = FindObjectOfType<Match>();
        activeMatch.ScoreUpdated += OnScore;
        StartCoroutine(MatchTutorial());
        endgameRoot.SetActive(false);
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
            "You are standing in a recently-uncovered arena for the sport of \"Dare\"",
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
        activeMatch.ReportScoresToAnalytics("tutorial-end");
    }

    void OnScore(Player p, float newScore) {
        if (currentState == State.WaitForFirstScore) {
            speechBox.text = string.Format("Congratulations, {0}! That's it!\n\nYou both seem to be getting the hang of it. Carry on while I return to the dig.\n",
                                           p.Name);
            root.SetActive(true);
            currentState = State.WaitForRules;
            HideInSeconds(5);
            activeMatch.ReportScoresToAnalytics("first-score");
            
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
        float goalTarget = activeMatch.TotalGoalsScored + (float)goalDelta;
        while(activeMatch.TotalGoalsScored < goalTarget) {
            yield return null;
        }
    }

    IEnumerator RulesScript() {
        activeMatch.ReportScoresToAnalytics("script-start");
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

        yield return StartCoroutine(WaitThenHide(6));

        yield return new WaitForSecondsRealtime(7);

        var heavyBalls = ApplyMod(
            new MatchMod {
                Type = MatchModType.Ball,
                Ball = new BallMod { MassMul = 2.5f },
            },
            "A beautiful tablet!\n\nAnd it says the balls should be heavier!\n",
            ForDuration(40),
            "Not again! A peer review has rejected my heavier-balls hypothesis.\n");
        // will overlap with following rules:
        StartCoroutine(heavyBalls);
                                
        yield return new WaitForSecondsRealtime(20);
        
        var flipGravity = ApplyMod(
            new MatchMod {
                Type = MatchModType.Gravity,
                Strength = -0.5f,
            },
            "Here's a tablet that suggests the balls are supposed to go... up?\n",
            ForDuration(5),
            "\n...I guess not!\n");
        yield return StartCoroutine(flipGravity);

        yield return new WaitForSecondsRealtime(20);

        StartCoroutine(EndlessRules());
        activeMatch.ReportScoresToAnalytics("script-end");
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
            Weight = 0.3f,
            SelectionCount = 3,
            SelectionSeparate = 6,
        },
        new Rule {
            CreateMod = () => new MatchMod {
                Type = MatchModType.Gravity,
                Strength = -0.5f,
            },
            IsAlwaysEnded = true,
            Intro = "A meta-analysis has found that balls indeed should go up!!\n",
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
        new Rule {
            CreateMod = () => new MatchMod {
                Type = MatchModType.AmmoBoost,
                Strength = 3f,
            },
            Intro = "Scrolls from Nubia imply each player had more shots\n",
            Outro = "...but they may have been talking about ultimate frisbee?\n",
            // don't let it stick around because I'm worried about fire rates
            IsAlwaysEnded = true,
            Weight = 0.35f,
        }
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

    int stepsUntilAlts = 3;
    float altChance = 0.5f;
    bool isAltSpeechDone = false;
    
    IEnumerator EndlessRules() {

        activeMatch.ReportScoresToAnalytics("endless-start");
        float endTime = Time.unscaledTime + 60 * 3f;

        while(endTime > Time.unscaledTime) {
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

                --stepsUntilAlts;
                var mod = chosenRule.CreateMod();
                bool isAlt = stepsUntilAlts <= 0
                    && Random.value < altChance
                    && mod.Type == MatchModType.Ball;
                mod.Type = mod.Type == MatchModType.Ball && isAlt ? MatchModType.AltBall
                    : mod.Type;

                if (isAltSpeechDone == false && isAlt) {
                    isAltSpeechDone = true;
                    var altIntro = "It says here that the Ancient Egyptians played with alternating balls of different qualities...\n\nLet's try that!\n";
                    yield return StartCoroutine(ShowTextFor(altIntro, 5f));
                }

                var endChoice = Random.value;
                EndCondition endCond
                    = endChoice < 0.4 && chosenRule.IsAlwaysEnded == false ? null
                    : ForDuration(Random.Range(7f, 14f));
                
                var modder = ApplyMod(mod,
                                      chosenRule.Intro,
                                      endCond,
                                      chosenRule.Outro);
                yield return StartCoroutine(modder);
            }
        }

        StartCoroutine(EndGame());
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
        speechBG.sprite = ludumBG;
        speechBox.text = text;
        
        yield return new WaitForSecondsRealtime(5);

        if (speechBox.text == text) {
            // we still have control
            root.SetActive(false);
        }
    }

    IEnumerator PlayScript(IList<string> script, float initDelay) {
        root.SetActive(true);
        speechBG.sprite = ludumBG;
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

    IEnumerator PlayTimedScript(IList<string> script,
                                float delay) {
        root.SetActive(true);
        speechBG.sprite = ludumBG;
        
        foreach(var line in script) {
            speechBox.text = line;
            yield return new WaitForSecondsRealtime(delay);
        }
    }

    struct ShakeLine {
        public string Text;
        public float Duration;
        public float ShakeDuration;
        public float ShakeForce;
    }

    IEnumerator PlayShakeScript(IList<ShakeLine> script) {
        root.SetActive(true);
        speechBG.sprite = dareBG;

        foreach(var line in script) {
            speechBox.text = line.Text;
            if (line.ShakeForce > 0f) {
                shaker.StartShake(line.ShakeDuration, line.ShakeForce);
                var clip = line.ShakeForce < 0.8f ? smallShake : bigShake;
                sfx.PlayOneShot(clip);
            }
            yield return new WaitForSecondsRealtime(line.Duration);
        }
    }


    IEnumerator EndGame() {

        var dareIntro = new []{
            new ShakeLine {
                Text = "\n...\n",
                Duration = 2,
                ShakeDuration = 0.5f,
                ShakeForce = 0.25f,
            },
            new ShakeLine {
                Text = "\n... ...\n",
                Duration = 2,
                ShakeDuration = 1f,
                ShakeForce = 0.6f,
            },
            new ShakeLine {
                Text = "\n... ... ...\n",
                Duration = 3,
                ShakeDuration = 2,
                ShakeForce = 1.2f,
            },
        };

        yield return StartCoroutine(PlayShakeScript(dareIntro));
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(3);

        yield return StartCoroutine(PlayTimedScript(new []{
                    "Um.\n",
                    "\nWas that one of you?\n",
                    "\n\n...because it wasn't me.",
                }
                , 3f));
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(1);

        activeMatch.HoldFire = true;

        activeMatch.ReportScoresToAnalytics("endgame-intro");
        foreach(var playerScore in playerScoreLabels) {
            playerScore.SetActive(false);
        }
        yield return StartCoroutine(PlayShakeScript(new [] {
                    new ShakeLine {
                        Text = "\nWHO\n",
                        Duration = 3,
                        ShakeDuration = 0.5f,
                        ShakeForce = 0.25f,
                    },
                    new ShakeLine {
                        Text = "\nAWAKENS\n",
                        Duration = 3,
                        ShakeDuration = 0.5f,
                        ShakeForce = 0.25f,
                    },
                    new ShakeLine {
                        Text = "\nDA\n",
                        Duration = 4,
                        ShakeDuration = 1f,
                        ShakeForce = 0.25f,
                    },
                }));
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(2);

        yield return StartCoroutine(PlayTimedScript(new []{
                    "\n(you answer it?)\n",
                }
                , 1f));

        yield return StartCoroutine(PlayShakeScript(new [] {
                    new ShakeLine {
                        Text = "\nSILENCE!\n",
                        Duration = 3,
                        ShakeDuration = 1,
                        ShakeForce = 0.8f,
                    },
                }));
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(3);

        yield return StartCoroutine(PlayTimedScript(new []{
                    "\n\n\nThis is bad.",
                    "Have you ever heard that name before?\n",
                    "\nIt's Da, the Ancient Egyptian God of Play!\n",
                    "...Dare must be more than a sport, it must be some sort of ritual...\n",
                    "And we've accidentally completed it!\n",
                    "\nWe've awoken a God! And It doesn't sound happy!\n",
                }
                , 4f));
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(1);

        yield return StartCoroutine(PlayTimedScript(new []{"Just let me think...\n"} , 2f));
        
        yield return StartCoroutine(PlayShakeScript(new [] {
                    new ShakeLine {
                        Text = "\nWHERE\n",
                        Duration = 3,
                        ShakeDuration = 0.5f,
                        ShakeForce = 0.25f,
                    },
                    new ShakeLine {
                        Text = "\nIS\n",
                        Duration = 3,
                        ShakeDuration = 0.25f,
                        ShakeForce = 0.25f,
                    },
                    new ShakeLine {
                        Text = "\nMY\n",
                        Duration = 3,
                        ShakeDuration = 0.25f,
                        ShakeForce = 0.25f,
                    },
                    new ShakeLine {
                        Text = "\nTEMPLE\n",
                        Duration = 4,
                        ShakeDuration = 0.5f,
                        ShakeForce = 0.6f,
                    },
                }));
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(1);
        
        yield return StartCoroutine(PlayTimedScript(new []{
                    "Ah, well, Your... Sportiness?\n",
                    "A long time has passed.\n\nYour children are gone.",
                    "We are here to study the history of this Ancient Civilisation\n",
                }
                , 4f));
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(5);

        yield return StartCoroutine(PlayTimedScript(new []{"...Hello?\n"} , 2f));
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(5);

        yield return StartCoroutine(PlayShakeScript(new [] {
                    new ShakeLine {
                        Text = "\ngone?\n",
                        Duration = 4,
                    }
                }));
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(1);

        yield return StartCoroutine(PlayTimedScript(new []{
                    "Y-yes, oh Mighty Jock\n",
                    "But we honour them through research!\n"
                },
                4f));
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(2);
        
        yield return StartCoroutine(PlayShakeScript(new [] {
                    new ShakeLine {
                        Text = "\nSHOW\n",
                        Duration = 2,
                        ShakeDuration = 0.25f,
                        ShakeForce = 0.5f,
                    },
                    new ShakeLine {
                        Text = "\nME\n",
                        Duration = 2,
                        ShakeDuration = 0.25f,
                        ShakeForce = 0.25f,
                    },
                    new ShakeLine {
                        Text = "",
                        Duration = 2
                    },
                    new ShakeLine {
                        Text = "\nSHOW\n",
                        Duration = 2,
                        ShakeDuration = 0.25f,
                        ShakeForce = 0.25f,
                    },
                    new ShakeLine {
                        Text = "\nME\n",
                        Duration = 2,
                        ShakeDuration = 0.25f,
                        ShakeForce = 0.25f,
                    },
                    new ShakeLine {
                        Text = "\nOR\n",
                        Duration = 2,
                        ShakeDuration = 0.25f,
                        ShakeForce = 0.1f,
                    },
                    new ShakeLine {
                        Text = "\nBE\n",
                        Duration = 2,
                        ShakeDuration = 0.25f,
                        ShakeForce = 0.1f,
                    },
                    new ShakeLine {
                        Text = "\nDESTROYED\n",
                        Duration = 3,
                        ShakeDuration = 1.5f,
                        ShakeForce = 2f,
                    },
                }));
        root.SetActive(false);
        
        yield return new WaitForSecondsRealtime(2);

        float targetDelta = Mathf.Ceil(30 * activeMatch.AverageBallPoints);
        
        yield return StartCoroutine(PlayTimedScript(new []{
                    "This is not how I imagined my Thursday turning out.\n",
                    "Ok, just like we've been practicing, folks.\n",
                    "\nLet's show Da how we play this game downtown.\n",
                    "Work together to score a combined " + targetDelta + " points in 60 seconds to placate the Deity!\n",
                }
                , 4f));
        root.SetActive(false);
        
        yield return new WaitForSecondsRealtime(0.5f);

        activeMatch.ReportScoresToAnalytics("endgame-start");
        activeMatch.HoldFire = false;
        yield return StartCoroutine(PlayShakeScript(new [] {
                    new ShakeLine {
                        Text = "\nBEGIN\n",
                        Duration = 2,
                        ShakeDuration = 0.5f,
                        ShakeForce = 0.5f,
                    }
                }));
        root.SetActive(false);

        endgameRoot.SetActive(true);

        float targetTime = Time.unscaledTime + 60f;
        float baseScore = activeMatch.TotalGoalsScored;
        float targetScore = baseScore + targetDelta;

        while(targetTime >= Time.unscaledTime) {

            float newScore = activeMatch.TotalGoalsScored;

            endgameTimeLabel.text
                = Mathf.FloorToInt(targetTime - Time.unscaledTime).ToString();
            endgameScoreLabel.text = Mathf.Max(0, targetScore - newScore).ToString();

            if (newScore >= targetScore) {
                Analytics.CustomEvent("endgame-complete", new Dictionary<string,object> {
                        {"result", "win"},
                        {"time-left", targetTime - Time.unscaledTime}
                    });
                
                activeMatch.HoldFire = true;
                yield return StartCoroutine(PlayShakeScript(new [] {
                            new ShakeLine {
                                Text = "\nENOUGH\n",
                                Duration = 4,
                                ShakeDuration = 1,
                                ShakeForce = 0.8f,
                            },
                            new ShakeLine {
                                Text = "",
                                Duration = 1,
                            },
                            new ShakeLine {
                                Text = "\nI\n",
                                Duration = 2,
                                ShakeDuration = 0.25f,
                                ShakeForce = 0.1f,
                            },
                            new ShakeLine {
                                Text = "\nAM\n",
                                Duration = 2,
                                ShakeDuration = 0.25f,
                                ShakeForce = 0.1f,
                            },
                            new ShakeLine {
                                Text = "\nPLACATED\n",
                                Duration = 4,
                                ShakeDuration = 0.25f,
                                ShakeForce = 0.3f,
                            },
                        }));
                root.SetActive(false);
                yield return new WaitForSecondsRealtime(1);
                yield return StartCoroutine(PlayShakeScript(new [] {
                            new ShakeLine {
                                Text = "\nI\n",
                                Duration = 2,
                                ShakeDuration = 0.25f,
                                ShakeForce = 0.1f,
                            },
                            new ShakeLine {
                                Text = "\nWILL\n",
                                Duration = 2,
                                ShakeDuration = 0.25f,
                                ShakeForce = 0.1f,
                            },
                            new ShakeLine {
                                Text = "\n\n",
                                Duration = 2,
                            },
                            new ShakeLine {
                                Text = "\nsleep\n",
                                Duration = 4,
                            },
                        }));
                root.SetActive(false);
                
                yield return new WaitForSecondsRealtime(3);

                yield return StartCoroutine(PlayTimedScript(new []{
                            "\nYou did it!\n",
                            "\n\nOh, the papers I'm going to publish...\n",
                            "Thank you so much.\n\nYou saved us all!",
                            "Well done!",
                        }
                        , 4f));
                root.SetActive(false);

                yield return new WaitForSecondsRealtime(3);

                root.SetActive(true);
                speechBox.text = "Egyptian Dare\nAndrew Fray @tenpn\nThank you for playing!";
                yield break;

            }

            yield return null;
        }

        Analytics.CustomEvent("endgame-complete", new Dictionary<string,object> {
                {"result", "loss"},
                {"goals-left", targetScore - activeMatch.TotalGoalsScored },
            });

        // death!
        activeMatch.HoldFire = true;
        yield return StartCoroutine(PlayShakeScript(new [] {
                    new ShakeLine {
                        Text = "\nENOUGH\n",
                        Duration = 4,
                        ShakeDuration = 1,
                        ShakeForce = 0.8f,
                    },
                    new ShakeLine {
                        Text = "",
                        Duration = 1,
                    },
                    new ShakeLine {
                        Text = "\nYOU\n",
                        Duration = 2,
                        ShakeDuration = 0.25f,
                        ShakeForce = 0.1f,
                    },
                    new ShakeLine {
                        Text = "\nARE\n",
                        Duration = 2,
                        ShakeDuration = 0.25f,
                        ShakeForce = 0.1f,
                    },
                    new ShakeLine {
                        Text = "\nWEAK\n",
                        Duration = 4,
                        ShakeDuration = 1f,
                        ShakeForce = 1.25f,
                    },
                }));
        root.SetActive(false);
                
        yield return new WaitForSecondsRealtime(3);
        entireUI.SetActive(false);

        yield return StartCoroutine(PlayTimedScript(new []{
                    "\nOh no...\n",
                    "\n\nIf only there was some way to replay the game!\n",
                    "I think this is the end!\n",
                    "\n\nOh, the papers I could have published...\n",
                    "\nYou did your best, but goodbye!\n"
                },
                4f));
        root.SetActive(false);

        yield return new WaitForSecondsRealtime(1);
        shaker.StartShake(0.5f, 0.5f);
        sfx.PlayOneShot(smallShake);
        yield return new WaitForSecondsRealtime(0.75f);
        shaker.StartShake(1f, 1f);
        sfx.PlayOneShot(smallShake);
        yield return new WaitForSecondsRealtime(1);
        shaker.StartShake(1f, 2f);
        arena.SetTrigger("on-world-end");
        sfx.PlayOneShot(bigShake);
        yield return new WaitForSecondsRealtime(1);
        shaker.StartShake(3f, 2f);
        sfx.PlayOneShot(bigShake);
        yield return new WaitForSecondsRealtime(1);
        sfx.PlayOneShot(bigShake);
        yield return new WaitForSecondsRealtime(1.5f);
        shaker.StartShake(1f, 0.5f);
        yield return new WaitForSecondsRealtime(1.5f);
        shaker.StartShake(1f, 0.5f);

        yield return new WaitForSecondsRealtime(1);
        root.SetActive(true);
        speechBox.text = "Egyptian Dare\nAndrew Fray @tenpn\nThank you for playing!";
    }
}
