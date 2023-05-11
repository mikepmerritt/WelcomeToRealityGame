using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{
    public MainMenu mm;

    public void FinishedScrolling()
    {
        mm.StartGame();
    }
}
