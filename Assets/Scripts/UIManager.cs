using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject posts, comments, leavingComments, profile, timeUI, fakePhone;
    public GameObject commentContent, userCommentContent;
    public LayoutElement commentLayout, userCommentLayout;
    public PostFeedManager pfm;
    public GameObject commentPrefab, userCommentPrefab, replyPrefab;
    public TMP_Text commentExceptionText, profileTitle;
    public Button leaveCommentButton, profileReturn;
    public TMP_Text timeTracker;
    public Sprite alBanner, anBanner, azBanner, caBanner, kaBanner, maBanner, meBanner, noBanner;
    public Sprite alPosts, anPosts, azPosts, caPosts, caPostsScam, maPosts, mePosts, noPosts;
    public string alBio, anBio, azBio, caBio, kaBio, maBio, meBio, noBio;
    public List<string> profileExceptions = new List<string>();
    public GameObject profileBanner, profileBio, profilePosts;
    public GameObject reputation;
    public Image[] hearts;
    public Sprite fullHeart, halfHeart, emptyHeart;
    public bool timeExceeded, coroutineStarted;
    public Image clock;
    public Sprite clock0, clock1, clock2, clock3, clock4, clock5, clock6, clockOver;

    void Start()
    {
        posts = GameObject.Find("Post UI");
        comments = GameObject.Find("Comments UI");
        leavingComments = GameObject.Find("Leave Comment UI");
        profile = GameObject.Find("Profile UI");
        timeTracker = GameObject.Find("Time Tracker").GetComponent<TMP_Text>();
        timeUI = GameObject.Find("Time UI");
        fakePhone = GameObject.Find("Fake Phone");
        fakePhone.SetActive(false);

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

        clock = GameObject.Find("Clock").GetComponent<Image>();

        pfm = GameObject.Find("Post Feed Manager").GetComponent<PostFeedManager>();

        profileBanner = GameObject.Find("Profile Banner");
        profileBio = GameObject.Find("Profile Bio");
        profilePosts = GameObject.Find("Profile Posts");

        coroutineStarted = false;

        GoToPosts();
    }

    public void GoToPosts()
    {
        // activate one screen, deactivate the rest
        fakePhone.SetActive(false);
        posts.SetActive(true);
        comments.SetActive(false);
        leavingComments.SetActive(false);
        profile.SetActive(false);
        timeUI.SetActive(true);
        
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
        timeUI.SetActive(true);

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

        // add comments to comments box
        foreach(CommentChain c in pfm.curPost.rComments)
        {
            GameObject o = Instantiate(commentPrefab, commentContent.transform);
            TMP_Text[] commentBoxes = o.GetComponentsInChildren<TMP_Text>();

            float size = 25; // initial size for title is 25, rest will come later

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
                    size += t.preferredHeight; // allocate space for comment text
                }
            }

            o.GetComponent<RectTransform>().sizeDelta = new Vector2(300, size);

            foreach(Reply r in c.postedReplies)
            {
                GameObject ro = Instantiate(replyPrefab, commentContent.transform);
                TMP_Text[] replyBoxes = ro.GetComponentsInChildren<TMP_Text>();

                float rsize = 25; // initial size for title is 25, rest will come later

                // create reply comment
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
                        rsize += t.preferredHeight; // allocate space for reply text
                    }
                }

                ro.GetComponent<RectTransform>().sizeDelta = new Vector2(300, rsize);
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
        timeUI.SetActive(true);
        
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
                c.initial.postDate = pfm.currentDay;
                pfm.AddCommentToPost(c); 

                // add rep changes on comment
                foreach(Modifier m in c.initial.reputationChanges)
                {
                    if(!pfm.reputations.TryAdd(m.userToChange, m.amount))
                    {
                        pfm.reputations[m.userToChange] += m.amount;
                    }
                }

                // time costs
                pfm.dailyTime -= 3;
                UpdateTime();

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
        timeUI.SetActive(false);
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
        timeUI.SetActive(false);
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
            if(username == "meg.farber")
            {
                profileBanner.GetComponent<Image>().sprite = meBanner;
                profileBio.GetComponent<TMP_Text>().text = meBio;
                profilePosts.GetComponent<Image>().sprite = mePosts;
            }
            else if(username == "marku.s.mith")
            {
                profileBanner.GetComponent<Image>().sprite = maBanner;
                profileBio.GetComponent<TMP_Text>().text = maBio;
                profilePosts.GetComponent<Image>().sprite = maPosts;
            }
            else if(username == "kayla_brownie")
            {
                profileBanner.GetComponent<Image>().sprite = kaBanner;
                profileBio.GetComponent<TMP_Text>().text = kaBio;
                profilePosts.GetComponent<Image>().sprite = noPosts;
            }
            else if(username == "carlosgonzales28")
            {
                profileBanner.GetComponent<Image>().sprite = caBanner;
                profileBio.GetComponent<TMP_Text>().text = caBio;
                if(pfm.currentDay == 2 || pfm.currentDay == 3) // TODO: check dates
                {
                    profilePosts.GetComponent<Image>().sprite = caPostsScam;
                }
                else
                {
                    profilePosts.GetComponent<Image>().sprite = caPosts;
                }
            }
            else if(username == "all.is.on_line")
            {
                profileBanner.GetComponent<Image>().sprite = alBanner;
                profileBio.GetComponent<TMP_Text>().text = alBio;
                profilePosts.GetComponent<Image>().sprite = alPosts;
            }
            else if(username == "annapurna")
            {
                profileBanner.GetComponent<Image>().sprite = anBanner;
                profileBio.GetComponent<TMP_Text>().text = anBio;
                profilePosts.GetComponent<Image>().sprite = anPosts;
            }
            else if(username == "azure.does.art")
            {
                profileBanner.GetComponent<Image>().sprite = azBanner;
                profileBio.GetComponent<TMP_Text>().text = azBio;
                profilePosts.GetComponent<Image>().sprite = azPosts;
            }
        }
        else
        {
            profileBanner.GetComponent<Image>().sprite = noBanner;
            profileBio.GetComponent<TMP_Text>().text = noBio;
            profilePosts.GetComponent<Image>().sprite = noPosts;
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
        timeUI.SetActive(false);
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
            if(commenterName == "meg.farber")
            {
                profileBanner.GetComponent<Image>().sprite = meBanner;
                profileBio.GetComponent<TMP_Text>().text = meBio;
                profilePosts.GetComponent<Image>().sprite = mePosts;
            }
            else if(commenterName == "marku.s.mith")
            {
                profileBanner.GetComponent<Image>().sprite = maBanner;
                profileBio.GetComponent<TMP_Text>().text = maBio;
                profilePosts.GetComponent<Image>().sprite = maPosts;
            }
            else if(commenterName == "kayla_brownie")
            {
                profileBanner.GetComponent<Image>().sprite = kaBanner;
                profileBio.GetComponent<TMP_Text>().text = kaBio;
                profilePosts.GetComponent<Image>().sprite = noPosts;
            }
            else if(commenterName == "carlosgonzales28")
            {
                profileBanner.GetComponent<Image>().sprite = caBanner;
                profileBio.GetComponent<TMP_Text>().text = caBio;
                if(pfm.currentDay == 2 || pfm.currentDay == 3) // TODO: check dates
                {
                    profilePosts.GetComponent<Image>().sprite = caPostsScam;
                }
                else
                {
                    profilePosts.GetComponent<Image>().sprite = caPosts;
                }
            }
            else if(commenterName == "all.is.on_line")
            {
                profileBanner.GetComponent<Image>().sprite = alBanner;
                profileBio.GetComponent<TMP_Text>().text = alBio;
                profilePosts.GetComponent<Image>().sprite = alPosts;
            }
            else if(commenterName == "annapurna")
            {
                profileBanner.GetComponent<Image>().sprite = anBanner;
                profileBio.GetComponent<TMP_Text>().text = anBio;
                profilePosts.GetComponent<Image>().sprite = anPosts;
            }
            else if(commenterName == "azure.does.art")
            {
                profileBanner.GetComponent<Image>().sprite = azBanner;
                profileBio.GetComponent<TMP_Text>().text = azBio;
                profilePosts.GetComponent<Image>().sprite = azPosts;
            }
        }
        else
        {
            profileBanner.GetComponent<Image>().sprite = noBanner;
            profileBio.GetComponent<TMP_Text>().text = noBio;
            profilePosts.GetComponent<Image>().sprite = noPosts;
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

        // update time
        timeTracker.text = "" + pfm.dailyTime;

        // update time penalty
        if(pfm.dailyTime <= -3)
        {
            // time penalty
            timeExceeded = true;
        }
        else
        {
            timeExceeded = false;
        }

        // update clock
        if(clock == null)
        {
            clock = GameObject.Find("Clock").GetComponent<Image>();
        }

        float fractime = ((float) pfm.dailyTime) / pfm.timePerDay;
        if(timeExceeded && !coroutineStarted)
        {
            coroutineStarted = true;
            StartCoroutine(AlternateClockWarningColor(true));
        }
        else if(coroutineStarted)
        {
            // do nothing - it's already flashing
            // check for reset
            if(fractime > 0f)
            {
                clock.sprite = clock6;
                coroutineStarted = false;
                StopAllCoroutines();
            }
        }
        else
        {
            coroutineStarted = false;
            StopAllCoroutines();

            if(fractime > 5f/6)
            {
                clock.sprite = clock6;
            }
            else if(fractime > 4f/6)
            {
                clock.sprite = clock5;
            }
            else if(fractime > 3f/6)
            {
                clock.sprite = clock4;
            }
            else if(fractime > 2f/6)
            {
                clock.sprite = clock3;
            }
            else if(fractime > 1f/6)
            {
                clock.sprite = clock2;
            }
            else if(fractime > 0f)
            {
                clock.sprite = clock1;
            }
            else
            {
                clock.sprite = clock0;
            }
        }
    }

    public IEnumerator AlternateClockWarningColor(bool warningColorNext)
    {
        if(warningColorNext)
        {
            clock.sprite = clockOver;
        }
        else
        {
            clock.sprite = clock0;
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(AlternateClockWarningColor(!warningColorNext));
    }

    public void HideAllPhoneScreens()
    {
        fakePhone.SetActive(true);
        timeUI.SetActive(true);
        profile.SetActive(false);
        posts.SetActive(false);
        comments.SetActive(false);
        leavingComments.SetActive(false);
    }
}
