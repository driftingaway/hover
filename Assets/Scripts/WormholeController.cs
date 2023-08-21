using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormholeController : MonoBehaviour
{
    public Material worm;
    public Material player;
    public Material circle;
    public AudioManager am;

    public IEnumerator SetSpeed(float speed1, float speed2, float transitionTime)
    {
        float timeElapsed = 0;
        float currentSpeed1 = worm.GetFloat("_Details_1_scroll_speed");
        float currentSpeed2 = worm.GetFloat("_Details_2_scroll_speed");
        float lerpSpeed1, lerpSpeed2;

        while(timeElapsed < transitionTime)
        {
            lerpSpeed1 = Mathf.Lerp(currentSpeed1, speed1, timeElapsed / transitionTime);
            lerpSpeed2 = Mathf.Lerp(currentSpeed2, speed2, timeElapsed / transitionTime);
            timeElapsed += Time.deltaTime;

            worm.SetFloat("_Details_1_scroll_speed", lerpSpeed1);
            worm.SetFloat("_Details_2_scroll_speed", lerpSpeed2);
            player.SetFloat("_Details_1_scroll_speed", lerpSpeed1);
            player.SetFloat("_Details_2_scroll_speed", lerpSpeed2);
            yield return null;
        }
    }

    public IEnumerator SetTiling(float tiling_x1, float tiling_y1, float tiling_x2, float tiling_y2, float transitionTime)
    {
        float timeElapsed = 0;
        Vector2 lerpTiling1, lerpTiling2;
        Vector2 tiling1 = worm.GetVector("_Details_1_tiling");
        Vector2 tiling2 = worm.GetVector("_Details_2_tiling");
        Vector2 newTiling1 = new Vector2(tiling_x1, tiling_y1);
        Vector2 newTiling2 = new Vector2(tiling_x2, tiling_y2);

        while(timeElapsed < transitionTime)
        {
            lerpTiling1 = Vector2.Lerp(tiling1, newTiling1, timeElapsed / transitionTime);
            lerpTiling2 = Vector2.Lerp(tiling2, newTiling2, timeElapsed / transitionTime);
            timeElapsed += Time.deltaTime;
            print(timeElapsed);
            worm.SetVector("_Details_1_tiling", lerpTiling1);
            worm.SetVector("_Details_2_tiling", lerpTiling2);
            yield return null;
        }
    }

    public IEnumerator Pulse(float start, float end, float duration)
    {
        float timeElapsed = 0;
        float value;
        Color color1w, color2w, color1p, color2p;

        while(timeElapsed < duration)
        {
            value = Mathf.Lerp(start, end, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            color1w = worm.GetColor("_Details_1_colour");
            color2w = worm.GetColor("_Details_2_colour");
            color1p = player.GetColor("_Details_1_colour");
            color2p = player.GetColor("_Details_2_colour");
            color1w.a = value;
            color2w.a = value;
            color1p.a = value;
            color2p.a = value;

            worm.SetColor("_Details_1_colour", color1w);
            worm.SetColor("_Details_2_colour", color2w);
            player.SetColor("_Details_1_colour", color1p);
            player.SetColor("_Details_2_colour", color2p);
            yield return null;
        }
    }

    public void InitColor(Color color1, Color color2)
    {
        worm.SetColor("_Details_1_colour", color1*5);
        worm.SetColor("_Details_2_colour", color2*5);
        //player.SetColor("_Details_1_colour", color*50);
        //player.SetColor("_Details_2_colour", color*50);
        circle.SetColor("_Wormhole_colour", color1*5);
    }

    
}
