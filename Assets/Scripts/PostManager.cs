using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostManager : MonoBehaviour
{
    public List<Post> dailyPosts, allPosts;
    public int curPostIndex;
    public TMP_Text username, postText;
    public Image image;
    public Post curPost;

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
            curPostIndex = 0;
            ChangePost(curPostIndex);
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
            // TODO: this is to prevent likes saving across playthroughs, find a better way to do this!
            // Also, this forces us to re-run to cleanse the posts, so really look into a solution.
            Post p = (Post) o;
            p.liked = false;
            p.shared = false;
            p.saved = false;

            allPosts.Add(p);
        }
    }

    public void ChangePost(int postIndex) 
    {
        if(postIndex < dailyPosts.Count && postIndex >= 0)
        {
            curPostIndex = postIndex;
            username.text = "@" + dailyPosts[curPostIndex].username;
            postText.text = dailyPosts[curPostIndex].postText;
            image.sprite = dailyPosts[curPostIndex].postImage;
            curPost = dailyPosts[curPostIndex];
        }
        else
        {
            Debug.LogError("Invalid index passed in (" + dailyPosts.Count + " posts available, got index " + postIndex + ")");
        }
    }

    public void NextPost()
    {
        ChangePost(curPostIndex + 1);
    }

    public void PreviousPost()
    {
        ChangePost(curPostIndex - 1);
    }
}
