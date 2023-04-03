using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Reply : Comment
{
    public Comment predecessor; // the comment that must come before it in the conversation
    public int availabilityDelay; // how many days after the predecessor the comment will be posted / postable
}
