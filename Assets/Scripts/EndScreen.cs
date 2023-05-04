using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    public TMP_Text results;
    public void Start()
    {
        if(PostFeedManager.finalGrades > 80 && PostFeedManager.finalRep > 0 && !PostFeedManager.hated)
        {
            results.text = "You receive a full scholarship to your state university AND a school award! The plaque says \"Citizen Award\" and they thank you for being such a valuable and upstanding member of your school community.";
        }
        else if(PostFeedManager.finalGrades > 80)
        {
            results.text = "You receive a partial scholarship to your state university! This will be such a great step towards an affordable college experience.";
        }
        else if(PostFeedManager.finalRep > 0 && !PostFeedManager.hated)
        {
            results.text = "You got into your state university! No scholarship was awarded, but at least you get to go where you wanted. On top of that, your class saw fit to name you Prom Sovereign!! It's nice to be appreciated by your peers.";
        }
        else
        {
            results.text = "Oh no… your university acceptance was rescinded?? You didn't even know that was possible! The email says that they found some questionable information about you online. Maybe you should have been more careful about what you posted on your social media...";
        }
    }
}
