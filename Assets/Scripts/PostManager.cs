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

    // interaction variables
    // TODO: there will be a lot more in here later, but for now we just want the rest of the buttons to do something
    public GameObject like, share, save;
    public Color defaultColor, likedColor, sharedColor, savedColor;

    void Start()
    {
        username = GameObject.Find("Username").GetComponent<TMP_Text>();
        postText = GameObject.Find("Post Text").GetComponent<TMP_Text>();
        image = GameObject.Find("Post Image").GetComponent<Image>();

        like = GameObject.Find("Like");
        share = GameObject.Find("Share");
        save = GameObject.Find("Save");

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
            ApplyInteractionColors();
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

    // interaction functions
    // for now, just change colors
    public void OnLike()
    {
        curPost.liked = !curPost.liked;
        ApplyInteractionColors();
    }

    public void OnShare()
    {
        curPost.shared = !curPost.shared;
        ApplyInteractionColors();
    }

    public void OnSave()
    {
        curPost.saved = !curPost.saved;
        ApplyInteractionColors();
    }

    public void ApplyInteractionColors()
    {
        // liked
        if(curPost.liked)
        {
            like.GetComponent<Image>().color = likedColor;
        }
        else
        {
            like.GetComponent<Image>().color = defaultColor;
        }
        // shared
        if(curPost.shared)
        {
            share.GetComponent<Image>().color = sharedColor;
        }
        else
        {
            share.GetComponent<Image>().color = defaultColor;
        }
        // saved
        if(curPost.saved)
        {
            save.GetComponent<Image>().color = savedColor;
        }
        else
        {
            save.GetComponent<Image>().color = defaultColor;
        }
    }
}
