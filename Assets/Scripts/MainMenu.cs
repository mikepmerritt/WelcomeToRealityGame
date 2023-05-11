using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject locked, unlocked;
    public Animator ani;
    public Sprite citizenSticker, partialSticker, recindedSticker, sovereignSticker;
    public Image citizenImage, partialImage, recindedImage, sovereignImage;

    public void Start()
    {
        locked.SetActive(true);
        unlocked.SetActive(false);

        if(EndScreen.citizen)
        {
            citizenImage.sprite = citizenSticker;
        }
        if(EndScreen.partial)
        {
            partialImage.sprite = partialSticker;
        }
        if(EndScreen.recinded)
        {
            recindedImage.sprite = recindedSticker;
        }
        if(EndScreen.sovereign)
        {
            sovereignImage.sprite = sovereignSticker;
        }
    }

    public void Scroll()
    {
        locked.SetActive(false);
        unlocked.SetActive(true);
        ani.Play("scroll");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("BackstoryScene");
    }
}
