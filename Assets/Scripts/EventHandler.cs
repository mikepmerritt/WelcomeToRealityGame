using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventHandler : MonoBehaviour
{
    public Post megParty, hackScam, allisonPlay;
    public PostFeedManager pfm;
    public UIManager uim;
    public GameObject panel, gradesWarning;
    public TMP_Text highlight, title;
    public bool partyInvite, scammed, playInvite; // TODO: use this later!
    public List<string> importantUsers;
    public Button nextDay;
    public GameObject loadHighlight, backToFeed, choiceHolder, choice1, choice2, choice3;
    public int grade, collegeRep;

    public void Start()
    {
        // set initial rep values
        grade = 100;
        collegeRep = 0;

        // set bools to false
        partyInvite = false;
        scammed = false;

        panel = GameObject.Find("Highlight Panel");
        title = GameObject.Find("Popup Title").GetComponent<TMP_Text>();
        gradesWarning = GameObject.Find("Time Warning");
        highlight = GameObject.Find("Highlight Text").GetComponent<TMP_Text>();
        pfm = GameObject.Find("Post Feed Manager").GetComponent<PostFeedManager>();
        uim = GameObject.Find("UI Manager").GetComponent<UIManager>();

        loadHighlight = GameObject.Find("To Summary");
        loadHighlight.GetComponent<Button>().onClick.AddListener(() => 
        {
            ChangeEventToFeedback();
        });
        
        backToFeed = GameObject.Find("Close Highlight");
        backToFeed.GetComponent<Button>().onClick.AddListener(() => 
        {
            panel.SetActive(false);
            pfm.RefreshFeedForNewDay();
        });

        nextDay = GameObject.Find("Next Day").GetComponent<Button>();
        nextDay.onClick.AddListener(() => 
        {
            GoToNextDay();
        });

        choiceHolder = GameObject.Find("Choice Holder");
        choice1 = GameObject.Find("Choice 1");
        choice2 = GameObject.Find("Choice 2");
        choice3 = GameObject.Find("Choice 3");

        panel.SetActive(false);
    }

    public void GoToNextDay()
    {
        uim.HideAllPhoneScreens();
        uim.UpdateTime();
        PickAndShowEvent();
    }

    public void AdjustGradesBasedOnTime()
    {
        if(uim.timeExceeded)
        {
            grade += 3 * (pfm.dailyTime + 2); // 3*(time+2) so -3 time is -3 grade, -4 time is -6 grade, etc.
        }
    }

    public void ChangeEventToFeedback()
    {
        title.text = "Daily Feedback";

        bool feedbackSet = false;

        choiceHolder.SetActive(false);
        loadHighlight.SetActive(false);
        backToFeed.SetActive(true);

        // randomize important user list so we don't get the same hint always
        for (int index = 0; index < importantUsers.Count; index++) {
            string initial = importantUsers[index];
            int swapIndex = Random.Range(index, importantUsers.Count);
            importantUsers[index] = importantUsers[swapIndex];
            importantUsers[swapIndex] = initial;
        }

        if(uim.timeExceeded)
        {
            highlight.text = "You spent too much time on social media today! Hopefully it doesn't affect your grades...";
            AdjustGradesBasedOnTime();
            feedbackSet = true;
            uim.timeExceeded = false;
        }
        if(!feedbackSet)
        {
            foreach(string user in importantUsers) 
            {
                if(pfm.reputations.TryGetValue(user, out int rep) && rep <= -4)
                {
                    highlight.text = "Due to your frequent hate toward " + user + ", they have blocked you. Hopefully you had nothing else to say to them.";

                    // user blocks you
                    pfm.blockedUsers.Add(user);

                    feedbackSet = true;
                }
            }
        }
        if(!feedbackSet)
        {
            foreach(string user in importantUsers) 
            {
                if(pfm.reputations.TryGetValue(user, out int rep) && rep < 0)
                {
                    highlight.text = "Looks like you really upset " + user + ". Did you say something rude?";

                    feedbackSet = true;
                }
            }
        }
        if(!feedbackSet)
        {
            foreach(string user in importantUsers)
            {
                if(pfm.reputations.TryGetValue(user, out int rep) && rep >= 3)
                {
                    highlight.text = "You and " + user + " have been getting along well!";

                    feedbackSet = true;
                }  
            }
        }
        if(!feedbackSet)
        {
            // emergency failsafe
            highlight.text = "Looks like you haven't done much yet.";
        }

        highlight.text += "\n\nCurrent grade: " + grade;
        
        if(collegeRep > 0)
        {
            highlight.text += "\nCollege reputation: Good";
        }
        else if(collegeRep < 0)
        {
            highlight.text += "\nCollege reputation: Poor";
        }
        else
        {
            highlight.text += "\nCollege reputation: Standard";
        }
    }

    // the return is for debugging and to break the function so it doesn't overwrite - the string returned should ideally not be used
    // and more occurs in the function rather than just making that string.
    public void PickAndShowEvent()
    {
        panel.SetActive(true);

        title.text = "Daily Event";
        highlight.text = "";
        backToFeed.SetActive(false);
        
        // TODO: fix null pointer here - manually assigned for now :(
        if(!scammed)
        {
            if(hackScam.rShared == true)
            {
                scammed = true;
                highlight.text = "That post from Carlos seemed a little strange... did he really get that phone, or did someone hack his account? \n\nAfter you shared it, your account got hacked and it took a whole day of working with support to get back in. You should be more careful with suspicious posts in the future.";
                
                choiceHolder.SetActive(true);
                choice1.SetActive(true);
                choice2.SetActive(false);
                choice3.SetActive(false);
                loadHighlight.SetActive(false);

                choice1.GetComponentInChildren<TMP_Text>().text = "Hopefully I didn't miss anything...";
                choice1.GetComponent<Button>().onClick.RemoveAllListeners();
                choice1.GetComponent<Button>().onClick.AddListener(() => {
                    // skip a day
                    panel.SetActive(false);
                    pfm.RefreshFeedForNewDay();
                    pfm.RefreshFeedForNewDay();
                });
            }
        }
        if(!partyInvite)
        {
            foreach(CommentChain c in megParty.rComments)
            {
                if(c.initial.Equals(megParty.postableComments[2].initial)) // the one about an invite
                {
                    partyInvite = true;
                }
            }
            if(partyInvite) 
            {
                // set text
                highlight.text = "Before you close the app for today, a DM from Megan Farber comes in. \n\n[@meg.farber]: Hey hey! Saw you liked my party post and I thought I’d extend the invite for tonight if you’d be interested <3";
                
                // set up choice buttons and disable back button
                choiceHolder.SetActive(true);
                choice1.SetActive(true);
                choice2.SetActive(true);
                choice3.SetActive(true);
                loadHighlight.SetActive(false);

                // set up buttons and listeners
                choice1.GetComponentInChildren<TMP_Text>().text = "For sure!";
                choice1.GetComponent<Button>().onClick.RemoveAllListeners();
                choice1.GetComponent<Button>().onClick.AddListener(() => {
                    highlight.text = "Meg is posting about the party on her account. Would you like to post about the party online?";

                    choice1.GetComponentInChildren<TMP_Text>().text = "Make a post about it";
                    choice1.GetComponent<Button>().onClick.RemoveAllListeners();
                    choice1.GetComponent<Button>().onClick.AddListener(() => {
                        if(!pfm.reputations.TryAdd("meg.farber", 8))
                        {
                            pfm.reputations["meg.farber"] += 8;
                        }
                        GoToNextDay();
                    });

                    choice2.GetComponentInChildren<TMP_Text>().text = "Make a story about it";
                    choice2.GetComponent<Button>().onClick.RemoveAllListeners();
                    choice2.GetComponent<Button>().onClick.AddListener(() => {
                        if(!pfm.reputations.TryAdd("meg.farber", 8))
                        {
                            pfm.reputations["meg.farber"] += 4;
                        }
                        if(!pfm.reputations.TryAdd("kayla_brownie", 4))
                        {
                            pfm.reputations["kayla_brownie"] += 4;
                        }
                        if(!pfm.reputations.TryAdd("carlosgonzales28", 4))
                        {
                            pfm.reputations["carlosgonzales28"] += 4;
                        }
                        GoToNextDay();
                    });

                    choice3.GetComponentInChildren<TMP_Text>().text = "Don't post anything about it";
                    choice3.GetComponent<Button>().onClick.RemoveAllListeners();
                    choice3.GetComponent<Button>().onClick.AddListener(() => {
                        if(!pfm.reputations.TryAdd("meg.farber", -1))
                        {
                            pfm.reputations["meg.farber"] += -1;
                        }
                        GoToNextDay();
                    });
                });

                choice2.GetComponentInChildren<TMP_Text>().text = "I’m all set, thanks anyway tho!";
                choice2.GetComponent<Button>().onClick.RemoveAllListeners();
                choice2.GetComponent<Button>().onClick.AddListener(() => {
                    highlight.text = "The conversation continues... \n\n[@meg.farber]: Oh, uh, okay. Sorry, thought you wanted in based off the comment!\n\n[@you]: Naw, I gotchu, just not feeling it rn. But hope you have fun!\n\n[@meg.farber]: Suit yourself! Thanks <3";
                    choiceHolder.SetActive(false);
                    loadHighlight.SetActive(true);
                    if(!pfm.reputations.TryAdd("meg.farber", -3))
                    {
                        pfm.reputations["meg.farber"] += -3;
                    }
                });

                choice3.GetComponentInChildren<TMP_Text>().text = "Ew, no. I like to avoid that bullshit.";
                choice3.GetComponent<Button>().onClick.RemoveAllListeners();
                choice3.GetComponent<Button>().onClick.AddListener(() => {
                    highlight.text = "The conversation continues... \n\n[@meg.farber]: Wtf? Then why did you say you wanted to come??\n\n[@you]: Idk, just bored ig, and you’re easy to fuck with.\n\nAfter that, Meg blocked you.";
                    choiceHolder.SetActive(false);
                    loadHighlight.SetActive(true);
                    if(!pfm.reputations.TryAdd("meg.farber", -8))
                    {
                        pfm.reputations["meg.farber"] += -8;
                    }
                    // meg blocks you
                    pfm.blockedUsers.Add("meg.farber");
                });
            }
        }
        if(!playInvite)
        {
            // school play - allison event
            foreach(CommentChain c in allisonPlay.rComments)
            {
                if(c.initial.Equals(allisonPlay.postableComments[0].initial)) // the one about an invite
                {
                    playInvite = true;
                }
            }
            if(playInvite) 
            {
                // set text
                highlight.text = "The school play is opening tonight. You told Allison you were interested.";
                
                // set up choice buttons and disable back button
                choiceHolder.SetActive(true);
                choice1.SetActive(true);
                choice2.SetActive(true);
                choice3.SetActive(true);
                loadHighlight.SetActive(false);

                // set up buttons and listeners
                choice1.GetComponentInChildren<TMP_Text>().text = "Buy tickets and attend";
                choice1.GetComponent<Button>().onClick.RemoveAllListeners();
                choice1.GetComponent<Button>().onClick.AddListener(() => {
                    highlight.text = "You decide to buy tickets and enjoy the show! It’s pretty good despite the lack of budget… The peers involved are appreciative of your support.";
                    choiceHolder.SetActive(false);
                    loadHighlight.SetActive(true);
                    if(!pfm.reputations.TryAdd("all.is.on_line", 5))
                    {
                        pfm.reputations["all.is.on_line"] += 5;
                    }
                    if(!pfm.reputations.TryAdd("azure.does.art", 5))
                    {
                        pfm.reputations["azure.does.art"] += 5;
                    }
                });

                choice2.GetComponentInChildren<TMP_Text>().text = "Stay home and do homework";
                choice2.GetComponent<Button>().onClick.RemoveAllListeners();
                choice2.GetComponent<Button>().onClick.AddListener(() => {
                    highlight.text = "You decide that your crushing load of homework needs to be done before going to anything like a school play. Although you miss out on a fun show, your grades are certainly going to take a turn uphill for this one.";
                    choiceHolder.SetActive(false);
                    loadHighlight.SetActive(true);
                    if(!pfm.reputations.TryAdd("all.is.on_line", -5))
                    {
                        pfm.reputations["all.is.on_line"] += -5;
                    }
                    // TODO: grades reward
                });

                choice3.GetComponentInChildren<TMP_Text>().text = "Stay home and play video games";
                choice3.GetComponent<Button>().onClick.RemoveAllListeners();
                choice3.GetComponent<Button>().onClick.AddListener(() => {
                    highlight.text = "School plays are kind of stupid, and you need some time to recharge. The recharging goes great, but maybe lasts a bit too long. You play video games for hours upon end before accidentally falling asleep without doing your homework. Shoot!";
                    choiceHolder.SetActive(false);
                    loadHighlight.SetActive(true);
                    if(!pfm.reputations.TryAdd("all.is.on_line", -5))
                    {
                        pfm.reputations["all.is.on_line"] += -5;
                    }
                    // TODO: grades penalty
                });
            }
        }
        if(highlight.text == "")
        {
            // no suitable event
            ChangeEventToFeedback();
        }
    }
}
