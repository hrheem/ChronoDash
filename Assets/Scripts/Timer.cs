using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using DG.Tweening;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    float timeRemaining = 11;


    // Update is called once per frame
    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = ((int)timeRemaining).ToString();
            //timerText.DOText(timeRemaining, 0f, true, ScrambleMode.None);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
