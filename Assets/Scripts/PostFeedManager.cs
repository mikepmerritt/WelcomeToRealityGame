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
    public int sizePerPost, dailyTime, timePerDay;
    public GameObject postPrefab, feed;
    public UIManager uim;
    public Post curPost; // for UI manager to use with comments
    public TMP_Text commentNum; // for UI manager to use when a comment is added
    public int currentDay;
    public TMP_Text dayTracker;
    public List<string> blockedUsers;

    void Start()
    {
        // set current day to 0 initially so we can increment to 1
        currentDay = 0;

        // fetch important objects from scene
        feed = GameObject.Find("Post Feed").GetComponentInChildren<LayoutElement>().gameObject;
        uim = GameObject.Find("UI Manager").GetComponent<UIManager>();
        dayTracker = GameObject.Find("Day Tracker").GetComponent<TMP_Text>();

        // set up empty dictionary for relationships
        reputations = new Dictionary<string, int>();

        // set up blocked users list
        blockedUsers = new List<string>();
        
        // call function to increment to day 1 and populate feed
        RefreshFeedForNewDay();
    }

    public void CreatePostInFeed(Post p)
    {
        // make an empty post object in the feed
        GameObject o = Instantiate(postPrefab, feed.transform);

        // go through all of the children for the empty post prefab and fill them out for the post
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
                // recolor button if liked previously
                if(p.rLiked)
                {
                    c.GetComponent<Image>().color = likedColor;
                }
                else
                {
                    c.GetComponent<Image>().color = defaultColor;
                }

                // add like functionality
                c.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // change the liked status
                    p.rLiked = !p.rLiked;

                    // recolor the button
                    if(p.rLiked)
                    {
                        c.GetComponent<Image>().color = likedColor;
                    }
                    else
                    {
                        c.GetComponent<Image>().color = defaultColor;
                    }

                    // increase reps on like
                    foreach(string name in p.increaseOnLike)
                    {
                        // try to add the reputation to the dictionary, if it fails it already exists so add change to the one that exists already
                        if(!reputations.TryAdd(name, 1))
                        {
                            // add rep if post is liked
                            if(p.rLiked)
                            {
                                reputations[name] += 1;
                            }
                            // remove rep if the post is unliked
                            else
                            {
                                reputations[name] -= 1;
                            }
                        }
                    }
                    // decrease reps on like
                    foreach(string name in p.decreaseOnLike)
                    {
                        // try to add the reputation to the dictionary, if it fails it already exists so add change to the one that exists already
                        if(!reputations.TryAdd(name, 1))
                        {
                            // add -rep if post is liked
                            if(p.rLiked)
                            {
                                reputations[name] -= 1;
                            }
                            // remove -rep if the post is unliked
                            else
                            {
                                reputations[name] += 1;
                            }
                        }
                    }
                    
                    // time costs
                    dailyTime -= 1;
                    uim.UpdateTime();
                });
            }
            else if(c.name == "Share")
            {
                // recolor button if shared previously
                if(p.rShared)
                {
                    c.GetComponent<Image>().color = sharedColor;
                }
                else
                {
                    c.GetComponent<Image>().color = defaultColor;
                }

                // add share functionality
                c.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // change the shared status
                    p.rShared = !p.rShared;

                    // recolor the button
                    if(p.rShared)
                    {
                        c.GetComponent<Image>().color = sharedColor;
                    }
                    else
                    {
                        c.GetComponent<Image>().color = defaultColor;
                    }

                    // increase reps on share
                    foreach(string name in p.increaseOnShare)
                    {
                        // try to add the reputation to the dictionary, if it fails it already exists so add change to the one that exists already
                        if(!reputations.TryAdd(name, 1))
                        {
                            // add rep if post is shared
                            if(p.rShared)
                            {
                                reputations[name] += 1;
                            }
                            // remove rep if the post is unshared
                            else
                            {
                                reputations[name] -= 1;
                            }
                        }
                    }
                    // decrease reps on share
                    foreach(string name in p.decreaseOnShare)
                    {
                        // try to add the reputation to the dictionary, if it fails it already exists so add change to the one that exists already
                        if(!reputations.TryAdd(name, 1))
                        {
                            // add -rep if post is shared
                            if(p.rShared)
                            {
                                reputations[name] -= 1;
                            }
                            // remove -rep if the post is unshared
                            else
                            {
                                reputations[name] += 1;
                            }
                        }
                    }
                    
                    // time costs
                    dailyTime -= 1;
                    uim.UpdateTime();
                });
            }
            else if(c.name == "Save")
            {
                // recolor button if saved previously
                if(p.rSaved)
                {
                    c.GetComponent<Image>().color = savedColor;
                }
                else
                {
                    c.GetComponent<Image>().color = defaultColor;
                }

                // add save functionality
                c.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // change the saved status
                    p.rSaved = !p.rSaved;

                    // recolor the button
                    if(p.rSaved)
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
                counter.text = "" + p.rComments.Count;

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
    public void AddCommentToPost(CommentChain c)
    {
        curPost.rComments.Add(c);
    }

    public List<Post> FetchPosts(int day)
    {
        List<Post> returnedPosts = new List<Post>();

        Object[] fetchedPosts = Resources.LoadAll("Posts/Day" + day, typeof(Post));
        foreach (Object o in fetchedPosts)
        {
            // load post as post object
            Post p = (Post) o;

            // initialize runtime variables (allows us to change objects during runtime without losing data)
            p.rLiked = p.liked;
            p.rShared = p.shared;
            p.rSaved = p.saved;
            p.rCommentedToday = false;
            p.rComments = new List<CommentChain>();
            p.rPostableComments = new List<CommentChain>();

            // copy comment lists without aliasing
            foreach(CommentChain c in p.commentChains)
            {
                // clone comment chain
                CommentChain cc = c.Clone();
                // set initial comment's post date to this day
                cc.initial.postDate = day;
                // add comment with modified date
                p.rComments.Add(cc);
            }
            foreach(CommentChain c in p.postableComments)
            {
                p.rPostableComments.Add(c.Clone());
            }

            // if the user is not blocked, add the post
            if(!blockedUsers.Contains(p.username))
            {
                allPosts.Add(p);
                returnedPosts.Add(p);
            }
        }

        return returnedPosts;
    }

    public void RefreshFeedForNewDay()
    {
        // increase day
        currentDay++;
        dayTracker.text = "Day " + currentDay;

        // go to post feed if not there already
        // skip on day 1 because that does nothing and creates a lot of null pointers
        if(currentDay > 1)
        {
            uim.GoToPosts();
        }

        // refresh daily time
        dailyTime = timePerDay;
        uim.UpdateTime();

        // remove all posts from feed
        for(int i = feed.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(feed.transform.GetChild(i).gameObject);
        }

        // get rid of irrelevant posts (unsaved)
        for(int i = dailyPosts.Count - 1; i >= 0; i--)
        {
            // blocked
            if(blockedUsers.Contains(dailyPosts[i].username))
            {
                dailyPosts.Remove(dailyPosts[i]);
            }
            // not saved or commented on
            else if(!dailyPosts[i].rSaved && !dailyPosts[i].rCommentedToday)
            {
                dailyPosts.Remove(dailyPosts[i]);
            }
            else
            {
                // unmark one-time save flag
                dailyPosts[i].rCommentedToday = false;
            }
        }

        List<Post> fetchedPosts = FetchPosts(currentDay);
        foreach(Post p in fetchedPosts)
        {
            dailyPosts.Add(p);
        }

        // go to top of feed
        feed.GetComponent<RectTransform>().position = new Vector3(feed.GetComponent<RectTransform>().position.x, 0, feed.GetComponent<RectTransform>().position.z);

        // resize the feed content box to have enough space to fit all of the posts
        feed.GetComponent<LayoutElement>().minHeight = dailyPosts.Count * sizePerPost;

        // recalculate the comments for each post
        foreach(Post p in dailyPosts)
        {
            foreach(CommentChain c in p.rComments)
            {
                c.UpdateChainBasedOnDate(currentDay);
            }
        }

        // add new posts to feed
        foreach(Post p in dailyPosts)
        {
            CreatePostInFeed(p);
        }
    }
}
