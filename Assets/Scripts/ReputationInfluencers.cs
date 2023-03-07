using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReputationInfluencers
{
    // This array holds all the information on what the reputation changes are for a given post
    // string 0: interaction type (like/share/comment)
    // string 1: numerical change
    // string 2: username of the person it affects
    // string 3 (optional): comment id
    public string[] args;
}
