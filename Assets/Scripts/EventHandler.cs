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
    public Button closePanel, nextDay;

    public void Start()
    {
        // set bools to false
        partyInvite = false;
        scammed = false;

        panel = GameObject.Find("Highlight Panel");
        gradesWarning = GameObject.Find("Time Warning");
        highlight = GameObject.Find("Highlight Text").GetComponent<TMP_Text>();
        pfm = GameObject.Find("Post Feed Manager").GetComponent<PostFeedManager>();
        uim = GameObject.Find("UI Manager").GetComponent<UIManager>();

        closePanel = GameObject.Find("Close Highlight").GetComponent<Button>();
        closePanel.onClick.AddListener(() => 
        {
            panel.SetActive(false);
            pfm.RefreshFeedForNewDay();
        });

        nextDay = GameObject.Find("Next Day").GetComponent<Button>();
        nextDay.onClick.AddListener(() => 
        {
            uim.HideAllPhoneScreens();
            PickAndShowHighlight();
        });
    }

    public string PickAndShowHighlight()
    {
        panel.SetActive(true);
        if(gradesWarning.activeSelf == true)
        {
            highlight.text = "You spent too much time on social media today! Hopefully it doesn't affect your grades...";
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
            if(pfm.reputations.TryGetValue("meg.farber", out int rep) && rep >= 4)
            {
                partyInvite = true;
            }
            if(partyInvite) 
            {
                highlight.text = "Looks like you've been getting along well with Meg - maybe she'll invite you to her next party?";
                return highlight.text;
            }
        }
        if(!scammed)
        {
            if(hackScam.rShared == true)
            {
                scammed = true;
                highlight.text = "That post from Carlos seemed a little strange... I wonder if he really got that phone, or if someone hacked his account? Hopefully nothing bad happens.";
                return highlight.text;
            }
        }
        // else
        foreach(string user in importantUsers)
        {
            if(pfm.reputations.TryGetValue(user, out int rep) && rep >= 3)
            {
                highlight.text = "You and " + user + " have been getting along well!";
                return highlight.text;
            }
            else if(rep < 0)
            {
                highlight.text = "Looks like you really upset " + user + ". Did you say something rude?";
                return highlight.text;
            }
        }
        // emergency failsafe
        highlight.text = "Looks like you haven't done much yet.";
        return highlight.text;
    }
}
