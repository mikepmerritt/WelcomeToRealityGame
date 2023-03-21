using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostFeedManager : MonoBehaviour
{
    public List<Post> dailyPosts, allPosts;
    public Dictionary<string, int> reputations;
    public Color defaultColor, likedColor, sharedColor, savedColor;
    public int sizePerPost;
    public GameObject postPrefab, feed;
    public UIManager uim;
    public Post curPost; // for UI manager to use with comments
    public TMP_Text commentNum; // for UI manager to use when a comment is added

    void Start()
    {
        feed = GameObject.Find("Post Feed").GetComponentInChildren<LayoutElement>().gameObject;
        uim = GameObject.Find("UI Manager").GetComponent<UIManager>();

        reputations = new Dictionary<string, int>();
        FetchPosts();
        dailyPosts = allPosts;

        if(dailyPosts.Count > 0)
        {
            // insert all posts into the feed
            foreach(Post p in dailyPosts)
            {
                CreatePostInFeed(p);
            }
        }
        else
        {
            Debug.LogError("No posts provided!");
        }
    }

    public void CreatePostInFeed(Post p)
    {
        GameObject o = Instantiate(postPrefab, feed.transform);
        Debug.Log(o.transform.childCount);
        for(int i = 0; i < o.transform.childCount; i++)
        {
            GameObject c = o.transform.GetChild(i).gameObject;

            // determine which one of the seven children this object is
            // Username, Post Image, Like, Share, Save, Comment, or Post Text
            if(c.name == "Username")
            {
                // add username text
                c.GetComponent<TMP_Text>().text = "@" + p.username;

                // add transition to profile page for clicking username
                c.GetComponent<Button>().onClick.AddListener(() =>
                {
                    uim.GoToProfileFromFeed(p.username);
                });
            }
            else if(c.name == "Post Image")
            {
                // add post image
                c.GetComponent<Image>().sprite = p.postImage;
            }
            else if(c.name == "Like")
            {
                // add like functionality
                c.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // change the liked status
                    p.liked = !p.liked;

                    // recolor the button
                    if(p.liked)
                    {
                        c.GetComponent<Image>().color = likedColor;
                    }
                    else
                    {
                        c.GetComponent<Image>().color = defaultColor;
                    }

                    // check if liking this has any effects
                    // if so, apply them
                    foreach(ReputationInfluencers r in p.reputationInfluencers)
                    {
                        if(r.args[0] == "like")
                        {
                            // try to add the reputation to the dictionary, if it fails it already exists so add change to the one that exists already
                            if(!reputations.TryAdd(r.args[2], int.Parse(r.args[1])))
                            {
                                // add rep if post is liked
                                if(p.liked)
                                {
                                    reputations[r.args[2]] += int.Parse(r.args[1]);
                                }
                                // remove rep if the post is unliked
                                else
                                {
                                    reputations[r.args[2]] -= int.Parse(r.args[1]);
                                }
                                    
                            }
                        }
                    }
                });
            }
            else if(c.name == "Share")
            {
                // add share functionality
                c.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // change the shared status
                    p.shared = !p.shared;

                    // recolor the button
                    if(p.shared)
                    {
                        c.GetComponent<Image>().color = sharedColor;
                    }
                    else
                    {
                        c.GetComponent<Image>().color = defaultColor;
                    }

                    // check if sharing this has any effects
                    // if so, apply them
                    foreach(ReputationInfluencers r in p.reputationInfluencers)
                    {
                        if(r.args[0] == "share")
                        {
                            // try to add the reputation to the dictionary, if it fails it already exists so add change to the one that exists already
                            if(!reputations.TryAdd(r.args[2], int.Parse(r.args[1])))
                            {
                                // add rep if post is shared
                                if(p.shared)
                                {
                                    reputations[r.args[2]] += int.Parse(r.args[1]);
                                }
                                // remove rep if the post is unshared
                                else
                                {
                                    reputations[r.args[2]] -= int.Parse(r.args[1]);
                                }
                                
                            }
                        }
                    }
                });
            }
            else if(c.name == "Save")
            {
                // add save functionality
                c.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // change the saved status
                    p.saved = !p.saved;

                    // recolor the button
                    if(p.saved)
                    {
                        c.GetComponent<Image>().color = savedColor;
                    }
                    else
                    {
                        c.GetComponent<Image>().color = defaultColor;
                    }

                    // TODO: add whatever functionality saving is supposed to have here
                });
            }
            else if(c.name == "Comment")
            {
                // fetch reference for the counter and set initial value
                TMP_Text counter = c.GetComponentInChildren<TMP_Text>();
                counter.text = "" + p.comments.Count;

                // add transition to comments screen
                c.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // set curPost as this post so the comments are fetched and stored in the right post
                    curPost = p; 
                    // set commentNum to the number for this post so it is updated accordingly
                    commentNum = counter;
                    uim.GoToComments();
                });
            }
            else if(c.name == "Post Text")
            {
                // add post text
                c.GetComponent<TMP_Text>().text = p.postText;
            }
            else
            {
                Debug.LogWarning("Feed Population Error: Child " + c.name + " has no intended behavior");
            }

        }
    }

    // add a comment to a post, used by UI manager to add comments to posts
    public void AddCommentToPost(Comment c)
    {
        curPost.comments.Add(c);
    }

    public void FetchPosts()
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

            // TODO: this is to remove user comments on subsequent replays, find a better way to do this if possible or necessary!
            // Also, this forces us to re-run to cleanse the posts, so really look into a solution.
            foreach(Comment uc in p.userComments)
            {
                // same as if(p.comments.Contains(c)) but for my unique comment objects
                for(int i = p.comments.Count - 1; i >= 0; i--)
                {
                    Comment c = p.comments[i];
                    if(uc.Equals(c))
                    {
                        p.comments.Remove(c);
                    }
                }
            }
            allPosts.Add(p);
        }

        // resize the feed content box to have enough space to fit all of the posts
        GameObject.Find("Post UI").GetComponentInChildren<LayoutElement>().minHeight = fetchedPosts.Length * sizePerPost;
    }
}
