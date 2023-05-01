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
    
    // copy a CommentChain without making any aliases
    // be forewarned, this was extremely painful due to the number of objects used.
    // this is necessary to prevent the post objects from overwriting their initial data 
    // by aliasing it with the runtime data (the variables starting with r)
    public CommentChain Clone()
    {
        CommentChain copy = new CommentChain();

        // initial
        copy.initial = new Comment();
        copy.initial.commenter = initial.commenter;
        copy.initial.commentText = initial.commentText;
        List<Modifier> copiedInitialMods = new List<Modifier>();
        foreach(Modifier m in initial.reputationChanges)
        {
            Modifier mc = new Modifier();
            mc.userToChange = m.userToChange;
            mc.amount = m.amount;
            copiedInitialMods.Add(mc);
        }
        copy.initial.reputationChanges = copiedInitialMods;

        // postedReplies
        List<Reply> prc = new List<Reply>();
        foreach(Reply r in postedReplies)
        {
            Reply rc = new Reply();
            rc.commenter = r.commenter;
            rc.commentText = r.commentText;
            List<Modifier> copiedReplyMods = new List<Modifier>();
            foreach(Modifier m in r.reputationChanges)
            {
                Modifier rmc = new Modifier();
                rmc.userToChange = m.userToChange;
                rmc.amount = m.amount;
                copiedInitialMods.Add(rmc);
            }
            rc.reputationChanges = copiedReplyMods;
            Comment prdc = new Comment(); // predecessor
            prdc.commenter = r.predecessor.commenter;
            prdc.commentText = r.predecessor.commentText;
            List<Modifier> copiedPredReplyMods = new List<Modifier>();
            foreach(Modifier m in r.predecessor.reputationChanges)
            {
                Modifier prdmc = new Modifier();
                prdmc.userToChange = m.userToChange;
                prdmc.amount = m.amount;
                copiedPredReplyMods.Add(prdmc);
            }
            prdc.reputationChanges = copiedPredReplyMods;
            rc.predecessor = prdc;
            rc.availabilityDelay = r.availabilityDelay;
            prc.Add(rc);
        }
        copy.postedReplies = prc;
        
        // availableToPlayer
        List<Reply> atpc = new List<Reply>();
        foreach(Reply r in availableToPlayer)
        {
            Reply rc = new Reply();
            rc.commenter = r.commenter;
            rc.commentText = r.commentText;
            List<Modifier> copiedReplyMods = new List<Modifier>();
            foreach(Modifier m in r.reputationChanges)
            {
                Modifier rmc = new Modifier();
                rmc.userToChange = m.userToChange;
                rmc.amount = m.amount;
                copiedInitialMods.Add(rmc);
            }
            rc.reputationChanges = copiedReplyMods;
            Comment prdc = new Comment(); // predecessor
            prdc.commenter = r.predecessor.commenter;
            prdc.commentText = r.predecessor.commentText;
            List<Modifier> copiedPredReplyMods = new List<Modifier>();
            foreach(Modifier m in r.predecessor.reputationChanges)
            {
                Modifier prdmc = new Modifier();
                prdmc.userToChange = m.userToChange;
                prdmc.amount = m.amount;
                copiedPredReplyMods.Add(prdmc);
            }
            prdc.reputationChanges = copiedPredReplyMods;
            rc.predecessor = prdc;
            rc.availabilityDelay = r.availabilityDelay;
            atpc.Add(rc);
        }
        copy.availableToPlayer = atpc;

        // pendingReplies
        List<Reply> pndrc = new List<Reply>();
        foreach(Reply r in pendingReplies)
        {
            Reply rc = new Reply();
            rc.commenter = r.commenter;
            rc.commentText = r.commentText;
            List<Modifier> copiedReplyMods = new List<Modifier>();
            foreach(Modifier m in r.reputationChanges)
            {
                Modifier rmc = new Modifier();
                rmc.userToChange = m.userToChange;
                rmc.amount = m.amount;
                copiedInitialMods.Add(rmc);
            }
            rc.reputationChanges = copiedReplyMods;
            Comment prdc = new Comment(); // predecessor
            prdc.commenter = r.predecessor.commenter;
            prdc.commentText = r.predecessor.commentText;
            List<Modifier> copiedPredReplyMods = new List<Modifier>();
            foreach(Modifier m in r.predecessor.reputationChanges)
            {
                Modifier prdmc = new Modifier();
                prdmc.userToChange = m.userToChange;
                prdmc.amount = m.amount;
                copiedPredReplyMods.Add(prdmc);
            }
            prdc.reputationChanges = copiedPredReplyMods;
            rc.predecessor = prdc;
            rc.availabilityDelay = r.availabilityDelay;
            pndrc.Add(rc);
        }
        copy.pendingReplies = pndrc;

        // return the non-aliased copy
        return copy;
    }

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
            if(predecessorPosted)
            {
                reply.predecessor.postDate = initial.postDate;
            }
            // then check the rest in the chain
            foreach(Comment c in postedReplies)
            {
                if(reply.predecessor.Equals(c))
                {
                    predecessorPosted = true;
                    reply.predecessor.postDate = c.postDate;
                }
            }

            if(predecessorPosted && (date - reply.predecessor.postDate) >= reply.availabilityDelay)
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
                reply.postDate = date;
            }
            
        }
    }

    public int CalculateSpaceNeeded()
    {
        // // TODO: allocate space dynamically - figure how much space per line is needed and how to calculate lines
        int space = 0;

        // // constants used for calculations
        // int oneLineSize = 45; // if the parent comment is only one line, the comment box height should be 45
        // int additionalLineSize = 21; // for each additional line of text in the comment, increase comment box height by 21
        // int charsPerLine = 100;

        // int numLines = initial.commentText.Length % charsPerLine;

        // space calculations
        // space += oneLineSize;
        // for(int i = 1; i < numLines; i++)
        // {
        //     space += additionalLineSize;
        // }

        space = 75;
        foreach(Comment c in postedReplies)
        {
            space += 75; // space for 1 reply
        }
        return space;
    }
}
