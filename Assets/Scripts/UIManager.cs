using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject posts, comments;

    void Start()
    {
        posts = GameObject.Find("Post UI");
        comments = GameObject.Find("Comments UI");
        GoToPosts();
    }

    public void GoToPosts()
    {
        posts.SetActive(true);
        comments.SetActive(false);
    }

    public void GoToComments()
    {
        comments.SetActive(true);
        posts.SetActive(false);
    }
}
