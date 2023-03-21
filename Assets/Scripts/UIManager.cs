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
    public GameObject commentPrefab, userCommentPrefab;
    public TMP_Text commentExceptionText, profileTitle, reputation;
    public Button leaveCommentButton, profileReturn;

    void Start()
    {
        posts = GameObject.Find("Post UI");
        comments = GameObject.Find("Comments UI");
        leavingComments = GameObject.Find("Leave Comment UI");
        profile = GameObject.Find("Profile UI");

        commentContent = comments.GetComponentInChildren<LayoutElement>().gameObject;
        commentLayout = comments.GetComponentInChildren<LayoutElement>();
        commentExceptionText = GameObject.Find("Comments Exception Text").GetComponent<TMP_Text>();
        leaveCommentButton = GameObject.Find("Leave Comment").GetComponent<Button>();

        userCommentContent = leavingComments.GetComponentInChildren<LayoutElement>().gameObject;
        userCommentLayout = leavingComments.GetComponentInChildren<LayoutElement>();

        profileTitle = GameObject.Find("Profile Title").GetComponent<TMP_Text>();
        reputation = GameObject.Find("Reputation").GetComponent<TMP_Text>();
        profileReturn = GameObject.Find("Profile Return").GetComponent<Button>();

        pfm = GameObject.Find("Post Feed Manager").GetComponent<PostFeedManager>();
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
            pfm.commentNum.text = "" + pfm.curPost.comments.Count;
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
        if(pfm.curPost.comments.Count == 0)
        {
            // set explanatory text for no comments
            if(pfm.curPost.userComments.Count > 0)
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
        commentLayout.preferredHeight = pfm.curPost.comments.Count * 100; // add space for comments in box
        foreach(Comment c in pfm.curPost.comments)
        {
            GameObject o = Instantiate(commentPrefab, commentContent.transform);
            TMP_Text[] commentBoxes = o.GetComponentsInChildren<TMP_Text>();
            foreach(TMP_Text t in commentBoxes)
            {
                if(t.gameObject.name == "Commenter")
                {
                    t.text = "@" + c.commenter;
                    // add button behavior for clicking on the username which brings the player to the user's profile
                    t.gameObject.GetComponent<Button>().onClick.AddListener(() => 
                    {
                        GoToProfileFromComment(c.commenter);
                    });
                }
                else if(t.gameObject.name == "Comment Text")
                {
                    t.text = c.commentText;
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
        foreach(Comment c in pfm.curPost.userComments)
        {
            // if the comment has not already been made, add a button
            if(!pfm.curPost.comments.Contains(c))
            {
                GameObject o = Instantiate(userCommentPrefab, userCommentContent.transform);
                o.GetComponentInChildren<TMP_Text>().text = c.commentText;
                o.GetComponent<Button>().onClick.AddListener(() => 
                {
                    pfm.AddCommentToPost(c); 

                    // add rep changes on comment
                    foreach(ReputationInfluencers r in pfm.curPost.reputationInfluencers)
                    {
                        // if the influence type is comment and the comment specified in the rep event matches the comment being made
                        if(r.args[0] == "comment" && pfm.curPost.userComments[int.Parse(r.args[3])].Equals(c))
                        {
                            // try to add the thing, if it fails it already exists so add change to the one that exists already
                            if(!pfm.reputations.TryAdd(r.args[2], int.Parse(r.args[1])))
                            {
                                // add rep if comment is made
                                pfm.reputations[r.args[2]] += int.Parse(r.args[1]);
                            }
                        }
                    }

                    // remove button
                    Destroy(o);
                    userCommentLayout.preferredHeight -= 100;
                });
                userCommentsContentSize += 100; // increase box size by 100 to fit a new button
            }
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
        reputation.text = "Reputation: " + repNum;

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

        // set up profile
        profileTitle.text = "@" + username;
        int repNum = 0;
        pfm.reputations.TryGetValue(username, out repNum);
        reputation.text = "Reputation: " + repNum;

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

        // set up profile
        profileTitle.text = "@" + commenterName;
        int repNum = 0;
        pfm.reputations.TryGetValue(commenterName, out repNum);
        reputation.text = "Reputation: " + repNum;

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
}
