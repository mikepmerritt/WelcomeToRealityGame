using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostManager : MonoBehaviour
{
    public List<Post> dailyPosts;
    public int curPost;
    public TMP_Text username, postText;
    public Image image;
    void Start()
    {
        username = GameObject.Find("Username").GetComponent<TMP_Text>();
        postText = GameObject.Find("Post Text").GetComponent<TMP_Text>();
        image = GameObject.Find("Post Image").GetComponent<Image>();
        if(dailyPosts.Count > 0)
        {
            curPost = 0;
        }
        else
        {
            Debug.LogError("No posts provided!");
        }
    }

    public void changePost(int postIndex) 
    {
        if(postIndex < dailyPosts.Count && postIndex >= 0)
        {
            curPost = postIndex;
            username.text = dailyPosts[curPost].username;
            postText.text = dailyPosts[curPost].postText;
            image.sprite = dailyPosts[curPost].postImage;
        }
        else
        {
            Debug.LogError("Invalid index passed in (" + dailyPosts.Count + " posts available, got index " + postIndex + ")");
        }
    }

    public void nextPost()
    {
        changePost(curPost + 1);
    }

    public void previousPost()
    {
        changePost(curPost - 1);
    }
}
