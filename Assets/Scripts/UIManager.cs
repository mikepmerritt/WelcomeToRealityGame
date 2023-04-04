using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject posts, comments, leavingComments, profile;
    public GameObject commentContent, userCommentContent;
    public LayoutElement commentLayout, userCommentLayout;
    public PostFeedManager pfm;
    public GameObject commentPrefab, userCommentPrefab, replyPrefab;
    public TMP_Text commentExceptionText, profileTitle;
    public Button leaveCommentButton, profileReturn;
    public TMP_Text timeTracker, timeWarning;
    public Sprite kaylaProfile, markusProfile, megProfile, scamProfile; // TODO: these should be removed when the profile interface is finished
    public List<string> profileExceptions = new List<string>(); // TODO: this too
    public GameObject profileOverlay, profileBackup; // TODO: this too
    public GameObject reputation;
    public Image[] hearts;
    public Sprite fullHeart, halfHeart, emptyHeart;

    void Start()
    {
        posts = GameObject.Find("Post UI");
        comments = GameObject.Find("Comments UI");
        leavingComments = GameObject.Find("Leave Comment UI");
        profile = GameObject.Find("Profile UI");
        timeTracker = GameObject.Find("Time Tracker").GetComponent<TMP_Text>();

        commentContent = comments.GetComponentInChildren<LayoutElement>().gameObject;
        commentLayout = comments.GetComponentInChildren<LayoutElement>();
        commentExceptionText = GameObject.Find("Comments Exception Text").GetComponent<TMP_Text>();
        leaveCommentButton = GameObject.Find("Leave Comment").GetComponent<Button>();

        userCommentContent = leavingComments.GetComponentInChildren<LayoutElement>().gameObject;
        userCommentLayout = leavingComments.GetComponentInChildren<LayoutElement>();

        profileTitle = GameObject.Find("Profile Title").GetComponent<TMP_Text>();
        reputation = GameObject.Find("Reputation");
        hearts = reputation.gameObject.GetComponentsInChildren<Image>();
        profileReturn = GameObject.Find("Profile Return").GetComponent<Button>();

        pfm = GameObject.Find("Post Feed Manager").GetComponent<PostFeedManager>();

        // TODO: these should be removed when the profile interface is finished
        profileOverlay = GameObject.Find("Profile Interface");
        profileBackup = GameObject.Find("Profile Backup");

        GoToPosts();
    }

    public void GoToPosts()
    {
        // activate one screen, deactivate the rest
        posts.SetActive(true);
        comments.SetActive(false);
        leavingComments.SetActive(false);
        profile.SetActive(false);
        
        // clean up screens for next opening
        ClearComments();
        ClearUserCommentButtons();
        // update the comment counter when returning to the post in case it changed (added a commment?)
        if(pfm.commentNum != null) // ignore on first time load
        {
            pfm.commentNum.text = "" + pfm.curPost.rComments.Count;
        }
    }

    public void GoToComments()
    {
        // activate one screen, deactivate the rest
        comments.SetActive(true);
        posts.SetActive(false);
        leavingComments.SetActive(false);
        profile.SetActive(false);

        // clean up screens for next opening
        ClearUserCommentButtons();
        leaveCommentButton.interactable = true; // enable button for leaving comments
        commentExceptionText.gameObject.SetActive(false); // remove text for no comments

        // enable the exception text box if needed (for example, if there are no comments)
        if(pfm.curPost.rComments.Count == 0)
        {
            // set explanatory text for no comments
            if(pfm.curPost.rPostableComments.Count > 0)
            {
                commentExceptionText.text = "Be the first to leave a comment!";
            }
            else
            {
                commentExceptionText.text = "Comments are disabled for this post.";
                leaveCommentButton.interactable = false;
            }
            // make text visible
            commentExceptionText.gameObject.SetActive(true);
        }

        // allocate space for comment box
        commentLayout.preferredHeight = 0;
        foreach(CommentChain c in pfm.curPost.rComments)
        {
            commentLayout.preferredHeight += c.CalculateSpaceNeeded();
        }

        // add comments to comments box
        foreach(CommentChain c in pfm.curPost.rComments)
        {
            GameObject o = Instantiate(commentPrefab, commentContent.transform);
            TMP_Text[] commentBoxes = o.GetComponentsInChildren<TMP_Text>();
            // TODO: reimplement system to create comment chain in box

            // create singleton comment using initial
            foreach(TMP_Text t in commentBoxes)
            {
                if(t.gameObject.name == "Commenter")
                {
                    t.text = "@" + c.initial.commenter;
                    // add button behavior for clicking on the username which brings the player to the user's profile
                    t.gameObject.GetComponent<Button>().onClick.AddListener(() => 
                    {
                        GoToProfileFromComment(c.initial.commenter);
                    });
                }
                else if(t.gameObject.name == "Comment Text")
                {
                    t.text = c.initial.commentText;
                }
            }

            foreach(Reply r in c.postedReplies)
            {
                GameObject ro = Instantiate(replyPrefab, commentContent.transform);
                TMP_Text[] replyBoxes = ro.GetComponentsInChildren<TMP_Text>();
                // TODO: reimplement system to create comment chain in box

                // create singleton comment using initial
                foreach(TMP_Text t in replyBoxes)
                {
                    if(t.gameObject.name == "Commenter")
                    {
                        t.text = "@" + r.commenter;
                        // add button behavior for clicking on the username which brings the player to the user's profile
                        t.gameObject.GetComponent<Button>().onClick.AddListener(() => 
                        {
                            GoToProfileFromComment(r.commenter);
                        });
                    }
                    else if(t.gameObject.name == "Comment Text")
                    {
                        t.text = r.commentText;
                    }
                }
            }
        }        
    }

    public void GoToLeavingComments()
    {
        // activate one screen, deactivate the rest
        leavingComments.SetActive(true);
        posts.SetActive(false);
        comments.SetActive(false);
        profile.SetActive(false);
        
        // clean up screens for next opening
        ClearComments();

        // generate user comment buttons
        int userCommentsContentSize = 0;
        foreach(CommentChain c in pfm.curPost.rPostableComments)
        {
            GameObject o = Instantiate(userCommentPrefab, userCommentContent.transform);
            o.GetComponentInChildren<TMP_Text>().text = c.initial.commentText;
            o.GetComponent<Button>().onClick.AddListener(() => 
            {
                c.postDate = pfm.currentDay;
                pfm.AddCommentToPost(c); 

                // add rep changes on comment
                foreach(Modifier m in c.initial.reputationChanges)
                {
                    if(!pfm.reputations.TryAdd(m.userToChange, m.amount))
                    {
                        pfm.reputations[m.userToChange] += m.amount;
                    }
                }

                // remove comment from postable
                pfm.curPost.rPostableComments.Remove(c);

                // save the post so it carries across days
                pfm.curPost.rCommentedToday = true;

                // remove button
                Destroy(o);
                userCommentLayout.preferredHeight -= 100;
            });
            userCommentsContentSize += 100; // increase box size by 100 to fit a new button
        }

        userCommentLayout.preferredHeight = userCommentsContentSize;
    }

    public void GoToProfile()
    {
        // activate one screen, deactivate the rest
        profile.SetActive(true);
        posts.SetActive(false);
        comments.SetActive(false);
        leavingComments.SetActive(false);

        // clean up screens for next opening
        ClearComments();
        ClearUserCommentButtons();
        DestroyProfileListeners();

        // set up profile
        profileTitle.text = "@" + pfm.curPost.username;
        int repNum = 0;
        pfm.reputations.TryGetValue(pfm.curPost.username, out repNum);
        // reputation.text = "Reputation: " + repNum;

        

        // set return screen since ambiguous
        // in this case, the user came from a post, so return them to the current post
        profileReturn.onClick.AddListener(() =>
        {
            GoToPosts();
            DestroyProfileListeners();
        });
    }

    public void GoToProfileFromFeed(string username)
    {
        // activate one screen, deactivate the rest
        profile.SetActive(true);
        posts.SetActive(false);
        comments.SetActive(false);
        leavingComments.SetActive(false);

        // clean up screens for next opening
        ClearComments();
        ClearUserCommentButtons();
        DestroyProfileListeners();

        // TODO: this should be removed when the profile interface is finished
        // determine if we should use the image overlay or the backup one
        if (profileExceptions.Contains(username))
        {
            profileOverlay.SetActive(true);
            profileBackup.SetActive(false);

            if(username == "meg.farber")
            {
                profileOverlay.GetComponent<Image>().sprite = megProfile;
            }
            else if(username == "marku.s.mith")
            {
                profileOverlay.GetComponent<Image>().sprite = markusProfile;
            }
            else if(username == "kayla_brownie")
            {
                profileOverlay.GetComponent<Image>().sprite = kaylaProfile;
            }
            else if(username == "user8390118")
            {
                profileOverlay.GetComponent<Image>().sprite = scamProfile;
            }
        }
        else
        {
            profileOverlay.SetActive(false);
            profileBackup.SetActive(true);
        }

        // set up profile
        profileTitle.text = "@" + username;
        int repNum = 0;
        pfm.reputations.TryGetValue(username, out repNum);
        // reputation.text = "Reputation: " + repNum;

        // heart meter - count number of hearts
        int numHearts = (repNum + 4) / 4;
        bool half = false;
        if(numHearts > 5) 
        {
            numHearts = 5;
        }
        else if(numHearts < 0) 
        {
            numHearts = 0;
        }
        else
        {
            half = repNum % 4 >= 2;
        }
        // draw in hearts based on calculations
        foreach(Image i in hearts)
        {
            if(numHearts > 0)
            {
                i.sprite = fullHeart;
                numHearts--;
            }
            else if(half)
            {
                i.sprite = halfHeart;
                half = false;
            }
            else
            {
                i.sprite = emptyHeart;
            }
        }

        // set return screen since ambiguous
        // in this case, the user came from a post, so return them to the current post
        profileReturn.onClick.AddListener(() =>
        {
            GoToPosts();
            DestroyProfileListeners();
        });
    }

    public void GoToProfileFromComment(string commenterName)
    {
        // activate one screen, deactivate the rest
        profile.SetActive(true);
        posts.SetActive(false);
        comments.SetActive(false);
        leavingComments.SetActive(false);

        // clean up screens for next opening
        ClearComments();
        ClearUserCommentButtons();

        // TODO: this should be removed when the profile interface is finished
        // determine if we should use the image overlay or the backup one
        if (profileExceptions.Contains(commenterName))
        {
            profileOverlay.SetActive(true);
            profileBackup.SetActive(false);

            if(commenterName == "meg.farber")
            {
                profileOverlay.GetComponent<Image>().sprite = megProfile;
            }
            else if(commenterName == "marku.s.mith")
            {
                profileOverlay.GetComponent<Image>().sprite = markusProfile;
            }
            else if(commenterName == "kayla_brownie")
            {
                profileOverlay.GetComponent<Image>().sprite = kaylaProfile;
            }
            else if(commenterName == "user8390118")
            {
                profileOverlay.GetComponent<Image>().sprite = scamProfile;
            }
        }
        else
        {
            profileOverlay.SetActive(false);
            profileBackup.SetActive(true);
        }

        // set up profile
        profileTitle.text = "@" + commenterName;
        int repNum = 0;
        pfm.reputations.TryGetValue(commenterName, out repNum);
        // reputation.text = "Reputation: " + repNum;

        // heart meter - count number of hearts
        int numHearts = (repNum + 4) / 4;
        bool half = false;
        if(numHearts > 5) 
        {
            numHearts = 5;
        }
        else if(numHearts < 0) 
        {
            numHearts = 0;
        }
        else
        {
            half = repNum % 4 >= 2;
        }
        // draw in hearts based on calculations
        foreach(Image i in hearts)
        {
            if(numHearts > 0)
            {
                i.sprite = fullHeart;
                numHearts--;
            }
            else if(half)
            {
                i.sprite = halfHeart;
                half = false;
            }
            else
            {
                i.sprite = emptyHeart;
            }
        }

        // set return screen since ambiguous
        // in this case, the user came from a commenter, so return them to the current post's comment section
        profileReturn.onClick.AddListener(() =>
        {
            GoToComments();
            DestroyProfileListeners();
        });
    }

    public void ClearComments()
    {
        // clear all comments so they don't stick around for other posts
        foreach(Transform childTransform in commentContent.transform)
        {
            GameObject.Destroy(childTransform.gameObject);
        }
    }

    public void ClearUserCommentButtons()
    {
        // clear all comments so they don't stick around for other posts
        foreach(Transform childTransform in userCommentContent.transform)
        {
            GameObject.Destroy(childTransform.gameObject);
        }
    }

    public void DestroyProfileListeners()
    {
        profileReturn.onClick.RemoveAllListeners();
    }

    public void UpdateTime()
    {
        // since these are called by the Post Feed Manager, start may not have finished yet
        // as such, if they don't exist yet, fix that first
        if(timeTracker == null)
        {
            timeTracker = GameObject.Find("Time Tracker").GetComponent<TMP_Text>();
        }
        if(pfm == null)
        {
            pfm = GameObject.Find("Post Feed Manager").GetComponent<PostFeedManager>();
        }
        if(timeWarning == null)
        {
            timeWarning = GameObject.Find("Time Warning").GetComponent<TMP_Text>();
        }

        // update time
        timeTracker.text = "Time Remaining:\n" + pfm.dailyTime;
        
        // update warning
        if(pfm.dailyTime <= -5)
        {
            // time penalty
            timeWarning.gameObject.SetActive(true);
            timeWarning.text = "You spent too much time on social media rather than studying!";
        }
        else if(pfm.dailyTime <= 0)
        {
            // low on time warning
            timeWarning.gameObject.SetActive(true);
            timeWarning.text = "Spending more time will hurt your grades!";
        }
        else
        {
            // hide warning (used for day transitions)
            timeWarning.gameObject.SetActive(false);
        }
    }
}
