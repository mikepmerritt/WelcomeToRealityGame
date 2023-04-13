using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Comment
{
    public string commenter;
    [TextArea(5, 10)]
    public string commentText;
    public List<Modifier> reputationChanges;
    // [HideInInspector]
    public int postDate = 1;

    public bool Equals(Comment c)
    {
        return (c.commenter == commenter) && (c.commentText == commentText);
    }
}

// helper class
[System.Serializable]
public class Modifier
{
    public string userToChange;
    public int amount;
}
