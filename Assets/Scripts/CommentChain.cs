using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommentChain
{
    public Comment initial;
    public List<Comment> posted;
    public List<Reply> availableToPlayer;
    public List<Reply> pending;
    public int postDate;
    
    public void UpdateChainBasedOnDate(int date)
    {
        // check pending posts for 
        for(int i = pending.Count - 1; i >= 0; i--)
        {
            Reply reply = pending[i];
            if((date - postDate) >= reply.availabilityDelay)
            {
                // check if it is a player reply and add to postable replies if so
                if(reply.commenter == "you")
                {
                    availableToPlayer.Add(reply);
                    pending.Remove(reply);
                }
                // otherwise post it
                else
                {
                    posted.Add(reply);
                    pending.Remove(reply);
                }
            }
            
        }
    }
}
