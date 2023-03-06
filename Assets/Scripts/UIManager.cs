using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject posts, comments, leavingComments;
    public GameObject commentContent;
    public LayoutElement layout;
    public PostManager pm;
    public GameObject commentPrefab;

    void Start()
    {
        posts = GameObject.Find("Post UI");
        comments = GameObject.Find("Comments UI");
        leavingComments = GameObject.Find("Leave Comment UI");
        commentContent = comments.GetComponentInChildren<LayoutElement>().gameObject;
        layout = comments.GetComponentInChildren<LayoutElement>();
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
        layout.preferredHeight = pm.curPost.comments.Count * 100; // add space for comments in box
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
    }
}
