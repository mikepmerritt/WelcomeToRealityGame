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
    public PostManager pm;
    public GameObject commentPrefab, userCommentPrefab;
    public TMP_Text profileTitle, reputation;

    void Start()
    {
        posts = GameObject.Find("Post UI");
        comments = GameObject.Find("Comments UI");
        leavingComments = GameObject.Find("Leave Comment UI");
        profile = GameObject.Find("Profile UI");

        commentContent = comments.GetComponentInChildren<LayoutElement>().gameObject;
        commentLayout = comments.GetComponentInChildren<LayoutElement>();

        userCommentContent = leavingComments.GetComponentInChildren<LayoutElement>().gameObject;
        userCommentLayout = leavingComments.GetComponentInChildren<LayoutElement>();

        profileTitle = GameObject.Find("Profile Title").GetComponent<TMP_Text>();
        reputation = GameObject.Find("Reputation").GetComponent<TMP_Text>();

        pm = GameObject.Find("Post Manager").GetComponent<PostManager>();
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
        pm.commentNum.text = "" + pm.curPost.comments.Count;
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

        // add comments to comments box
        commentLayout.preferredHeight = pm.curPost.comments.Count * 100; // add space for comments in box
        foreach(Comment c in pm.curPost.comments)
        {
            GameObject o = Instantiate(commentPrefab, commentContent.transform);
            TMP_Text[] commentBoxes = o.GetComponentsInChildren<TMP_Text>();
            foreach(TMP_Text t in commentBoxes)
            {
                if(t.gameObject.name == "Commenter")
                {
                    t.text = "@" + c.commenter;
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
        foreach(Comment c in pm.curPost.userComments)
        {
            // if the comment has not already been made, add a button
            if(!pm.curPost.comments.Contains(c))
            {
                GameObject o = Instantiate(userCommentPrefab, userCommentContent.transform);
                o.GetComponentInChildren<TMP_Text>().text = c.commentText;
                o.GetComponent<Button>().onClick.AddListener(() => 
                {
                    pm.AddCommentToPost(c); 
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

        // set up profile
        profileTitle.text = "@" + pm.curPost.username;
        int repNum = 0;
        pm.reputations.TryGetValue(pm.curPost.username, out repNum);
        reputation.text = "Reputation: " + repNum;
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
}
