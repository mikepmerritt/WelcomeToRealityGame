using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject locked, unlocked;
    public Animator ani;

    public void Start()
    {
        locked.SetActive(true);
        unlocked.SetActive(false);
    }

    public void Scroll()
    {
        locked.SetActive(false);
        unlocked.SetActive(true);
        ani.Play("scroll");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
