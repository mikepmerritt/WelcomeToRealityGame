using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// objects used
[System.Serializable]
public enum InteractionType
{
    like,
    share,
    comment
}

[System.Serializable]
public class ReputationChange
{
    [SerializeField]
    public string username;
    [SerializeField]
    public int change;
}

[System.Serializable]
public class ReputationInfluencers
{
    // This array holds all the information on what the reputation changes are for a given post
    // string 0: interaction type (like/share/comment)
    // string 1: numerical change
    // string 2: username of the person it affects
    // string 3: time cost
    // string 4 (optional): comment number
    // public string[] args;

    

    public InteractionType type;
    public List<ReputationChange> reputationChanges;
    
    public int timeCost;
    public int commentIndex;
}
