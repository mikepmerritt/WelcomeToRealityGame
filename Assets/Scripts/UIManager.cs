using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject posts, comments, leavingComments;
    public GameObject commentContent, userCommentContent;
    public LayoutElement commentLayout, userCommentLayout;
    public PostManager pm;
    public GameObject commentPrefab, userCommentPrefab;

    void Start()
    {
        posts = GameObject.Find("Post UI");
        comments = GameObject.Find("Comments UI");
        leavingComments = GameObject.Find("Leave Comment UI");
        commentContent = comments.GetComponentInChildren<LayoutElement>().gameObject;
        commentLayout = comments.GetComponentInChildren<LayoutElement>();
        userCommentContent = leavingComments.GetComponentInChildren<LayoutElement>().gameObject;
        userCommentLayout = leavingComments.GetComponentInChildren<LayoutElement>();
        pm = GameObject.Find("Post Manager").GetComponent<PostManager>();
        GoToPosts();
    }

    public void GoToPosts()
    {
        posts.SetActive(true);
        comments.SetActive(false);
        leavingComments.SetActive(false);
        
        // clear all comments so they don't stick around for other posts
        foreach(Transform childTransform in commentContent.transform)
        {
            GameObject.Destroy(childTransform.gameObject);
        }
    }

    public void GoToComments()
    {
        comments.SetActive(true);
        posts.SetActive(false);
        leavingComments.SetActive(false);

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
        leavingComments.SetActive(true);
        posts.SetActive(false);
        comments.SetActive(false);
        
        // clear all comments so they don't stick around on screen
        foreach(Transform childTransform in commentContent.transform)
        {
            GameObject.Destroy(childTransform.gameObject);
        }

        // generate user comment buttons
        int userCommentsContentSize = 0;
        foreach(Comment c in pm.curPost.userComments)
        {
            // if the comment has not already been made, add a button
            if(!pm.curPost.comments.Contains(c))
            {
                GameObject o = Instantiate(userCommentPrefab, userCommentContent.transform);
                o.GetComponentInChildren<TMP_Text>().text = c.commentText;
                userCommentsContentSize += 100; // increase box size by 100 to fit a new button
            }
        }

        userCommentLayout.preferredHeight = userCommentsContentSize;
    }
}
