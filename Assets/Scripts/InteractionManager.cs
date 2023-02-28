using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    // TODO: there will be a lot more in here later, but for now we just want the rest of the buttons to do something
    public GameObject like, share, save;
    public Color defaultColor, likedColor, sharedColor, savedColor;
    public PostManager pm;

    void Start()
    {
        like = GameObject.Find("Like");
        share = GameObject.Find("Share");
        save = GameObject.Find("Save");
        pm = GameObject.Find("Post Manager").GetComponent<PostManager>();
    }

    public void OnLike()
    {
        pm.curPost.liked = !pm.curPost.liked;
        if(pm.curPost.liked)
        {
            like.GetComponent<Image>().color = likedColor;
        }
        else
        {
            like.GetComponent<Image>().color = defaultColor;
        }
    }

    public void OnShare()
    {
        pm.curPost.shared = !pm.curPost.shared;
        if(pm.curPost.shared)
        {
            share.GetComponent<Image>().color = sharedColor;
        }
        else
        {
            share.GetComponent<Image>().color = defaultColor;
        }
    }

    public void OnSave()
    {
        pm.curPost.saved = !pm.curPost.saved;
        if(pm.curPost.saved)
        {
            save.GetComponent<Image>().color = savedColor;
        }
        else
        {
            save.GetComponent<Image>().color = defaultColor;
        }
    }

}
