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
    public List<CommentChain> commentChains;
    public List<CommentChain> postableComments;
    public bool liked = false;
    public bool shared = false;
    public bool saved = false;
    public List<string> increaseOnLike, decreaseOnLike, increaseOnShare, decreaseOnShare;
    [HideInInspector]
    public bool rLiked, rShared, rSaved;
    [HideInInspector]
    public List<CommentChain> rComments, rPostableComments;
}
