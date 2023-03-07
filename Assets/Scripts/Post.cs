using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Post")]
public class Post : ScriptableObject
{
    // TODO: make an object to store the user who makes the post
    public string username;
    public Sprite postImage;
    [TextArea(15, 20)]
    public string postText;
    public List<Comment> comments;
    public List<Comment> userComments;
    public bool liked = false;
    public bool shared = false;
    public bool saved = false;
    
    public List<ReputationInfluencers> reputationInfluencers;
}
