using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventHandler : MonoBehaviour
{
    public Post megParty, hackScam;
    public PostFeedManager pfm;
    public UIManager uim;
    public GameObject panel, gradesWarning;
    public TMP_Text highlight;
    public bool partyInvite, scammed; // TODO: use this later!
    public List<string> importantUsers;
    public Button nextDay;
    public GameObject backToFeed, choiceHolder, choice1, choice2, choice3;

    public void Start()
    {
        // set bools to false
        partyInvite = false;
        scammed = false;

        panel = GameObject.Find("Highlight Panel");
        // gradesWarning = GameObject.Find("Time Warning");
        highlight = GameObject.Find("Highlight Text").GetComponent<TMP_Text>();
        pfm = GameObject.Find("Post Feed Manager").GetComponent<PostFeedManager>();
        uim = GameObject.Find("UI Manager").GetComponent<UIManager>();

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
    }

    public void GoToNextDay()
    {
        uim.HideAllPhoneScreens();
        PickAndShowHighlight();
    }

    // the return is for debugging and to break the function so it doesn't overwrite - the string returned should ideally not be used
    // and more occurs in the function rather than just making that string.
    public string PickAndShowHighlight()
    {
        panel.SetActive(true);
        
        // TODO: fix null pointer here - manually assigned for now :(

        if(gradesWarning.activeSelf == true)
        {
            highlight.text = "You spent too much time on social media today! Hopefully it doesn't affect your grades...";

            choiceHolder.SetActive(false);
            backToFeed.SetActive(true);

            return highlight.text;
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
            if(megParty.rLiked || pfm.reputations.TryGetValue("meg.farber", out int rep) && rep >= 4)
            {
                partyInvite = true;
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
                backToFeed.SetActive(false);

                // set up buttons and listeners
                choice1.GetComponentInChildren<TMP_Text>().text = "For sure!";
                choice1.GetComponent<Button>().onClick.RemoveAllListeners();
                choice1.GetComponent<Button>().onClick.AddListener(() => {
                    highlight.text = "Meg is posting about the party on her account. Would you like to post about the party online?";

                    choice1.GetComponentInChildren<TMP_Text>().text = "Post a private story";
                    choice1.GetComponent<Button>().onClick.RemoveAllListeners();
                    choice1.GetComponent<Button>().onClick.AddListener(() => {
                        if(!pfm.reputations.TryAdd("meg.farber", 8))
                        {
                            pfm.reputations["meg.farber"] += 8;
                        }
                        GoToNextDay();
                    });

                    choice2.GetComponentInChildren<TMP_Text>().text = "Post a public story";
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

                    choice3.GetComponentInChildren<TMP_Text>().text = "Don't post about it";
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
                    backToFeed.SetActive(true);
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
                    backToFeed.SetActive(true);
                    if(!pfm.reputations.TryAdd("meg.farber", -8))
                    {
                        pfm.reputations["meg.farber"] += -8;
                    }
                    // block you
                });
                
                return highlight.text;
            }
        }
        if(!scammed)
        {
            if(hackScam.rShared == true)
            {
                scammed = true;
                highlight.text = "That post from Carlos seemed a little strange... I wonder if he really got that phone, or if someone hacked his account? Hopefully nothing bad happens.";
                
                choiceHolder.SetActive(false);
                backToFeed.SetActive(true);

                return highlight.text;
            }
        }
        // else
        foreach(string user in importantUsers)
        {
            if(pfm.reputations.TryGetValue(user, out int rep) && rep >= 3)
            {
                highlight.text = "You and " + user + " have been getting along well!";

                choiceHolder.SetActive(false);
                backToFeed.SetActive(true);

                return highlight.text;
            }
            else if(rep < 0)
            {
                highlight.text = "Looks like you really upset " + user + ". Did you say something rude?";

                choiceHolder.SetActive(false);
                backToFeed.SetActive(true);

                return highlight.text;
            }
        }
        // emergency failsafe
        highlight.text = "Looks like you haven't done much yet.";

        choiceHolder.SetActive(false);
        backToFeed.SetActive(true);
        
        return highlight.text;
    }
}
