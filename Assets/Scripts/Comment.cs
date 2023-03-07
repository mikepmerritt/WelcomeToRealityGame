using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Comment
{
    public string commenter;
    [TextArea(5, 10)]
    public string commentText;

    public bool Equals(Comment c)
    {
        return (c.commenter == commenter) && (c.commentText == commentText);
    }
}
