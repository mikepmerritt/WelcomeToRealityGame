using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommentChain
{
    public Comment initial;
    public List<Reply> postedReplies;
    [HideInInspector]
    public List<Reply> availableToPlayer;
    public List<Reply> pendingReplies;
    [HideInInspector]
    public int postDate = 1;
    
    public void UpdateChainBasedOnDate(int date)
    {
        // check pending posts for ones that can be posted
        for(int i = pendingReplies.Count - 1; i >= 0; i--)
        {
            // initialize reply from pending
            Reply reply = pendingReplies[i];

            // check if the post's predecessor is already in the comment chain
            // the predecessor is the post that needs to come before a given reply in the conversation
            bool predecessorPosted = reply.predecessor.Equals(initial); // check first comment in chain first
            // then check the rest in the chain
            foreach(Comment c in postedReplies)
            {
                if(reply.predecessor.Equals(c))
                {
                    predecessorPosted = true;
                }
            }

            if(predecessorPosted && (date - postDate) >= reply.availabilityDelay)
            {
                // check if it is a player reply and add to postable replies if so
                if(reply.commenter == "you")
                {
                    availableToPlayer.Add(reply);
                    pendingReplies.Remove(reply);
                }
                // otherwise post it
                else
                {
                    postedReplies.Add(reply);
                    pendingReplies.Remove(reply);
                }
            }
            
        }
    }

    public int CalculateSpaceNeeded()
    {
        int space = 75; // space for initial
        foreach(Comment c in postedReplies)
        {
            space += 75; // space for 1 reply
        }
        return space;
    }
}
