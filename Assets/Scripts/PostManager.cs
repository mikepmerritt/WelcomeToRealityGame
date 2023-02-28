using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostManager : MonoBehaviour
{
    public List<Post> dailyPosts, allPosts;
    public int curPost;
    public TMP_Text username, postText;
    public Image image;

    void Start()
    {
        username = GameObject.Find("Username").GetComponent<TMP_Text>();
        postText = GameObject.Find("Post Text").GetComponent<TMP_Text>();
        image = GameObject.Find("Post Image").GetComponent<Image>();

        // TODO: need to change in the future when GenerateDailyPosts is updated
        GenerateDailyPosts();
        dailyPosts = allPosts;

        if(dailyPosts.Count > 0)
        {
            curPost = 0;
            changePost(curPost);
        }
        else
        {
            Debug.LogError("No posts provided!");
        }
    }

    public void GenerateDailyPosts()
    {
        // TODO: in the future, a lot more will happen in here
        // This is where we will check post preconditions and make a random selection of which posts to use for a day
        // For the time being, this will just return a testing set of posts, but more to come later!

        Object[] fetchedPosts = Resources.LoadAll("Posts", typeof(Post));
        foreach (Object o in fetchedPosts)
        {
            allPosts.Add((Post) o);
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
