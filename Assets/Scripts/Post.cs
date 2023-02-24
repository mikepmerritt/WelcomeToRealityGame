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
    // TODO: make these comments objects too
    public List<string> comments;
}
