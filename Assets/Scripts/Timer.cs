using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text timer;
    private float time = 480;
    private float minutes;
    private float seconds;

    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            UpdateTime();
        }
    }

    void UpdateTime()
    {
            minutes = Mathf.FloorToInt(time / 60);
            seconds = Mathf.FloorToInt(time % 60);
            timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
