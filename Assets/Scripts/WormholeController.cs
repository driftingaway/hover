using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormholeController : MonoBehaviour
{
    public Material worm;
    public AudioManager am;
    
    // Start is called before the first frame update
    void Start()
    {
        float temp = (am.BPM/200) * 1.5f;
        worm.SetFloat("_Details_1_scroll_speed", temp);
        worm.SetFloat("_Details_2_scroll_speed", temp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDetail1(float speed)
    {
        worm.SetFloat("_Details_1_scroll_speed", speed);
    }

    public void SetDetail2(float speed)
    {
        worm.SetFloat("_Details_2_scroll_speed", speed);
    }

    public IEnumerator Pulse(float start, float end, float duration)
    {
        float timeElapsed = 0;
        float value;
        Color color1, color2;

        while(timeElapsed < duration)
        {
            value = Mathf.Lerp(start, end, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            color1 = worm.GetColor("_Details_1_colour");
            color2 = worm.GetColor("_Details_2_colour");
            color1.a = value;
            color2.a = value;

            worm.SetColor("_Details_1_colour", color1);
            worm.SetColor("_Details_2_colour", color2);
            yield return null;
        }
    }

    public void InitColor(Color color)
    {
        worm.SetColor("_Details_1_colour", color*5);
        worm.SetColor("_Details_2_colour", color*5);
    }

    
}
