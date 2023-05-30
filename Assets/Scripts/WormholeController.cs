using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormholeController : MonoBehaviour
{
    public Material worm;
    public Material player;
    public AudioManager am;

    public void SetSpeed(float speed1, float speed2)
    {
        worm.SetFloat("_Details_1_scroll_speed", speed1);
        worm.SetFloat("_Details_2_scroll_speed", speed2);
        player.SetFloat("_Details_1_scroll_speed", speed1);
        player.SetFloat("_Details_2_scroll_speed", speed2);
    }

    public void SetTiling(float tiling_x1, float tiling_y1, float tiling_x2, float tiling_y2)
    {
        worm.SetVector("_Details_1_tiling", new Vector2(tiling_x1, tiling_y1));
        worm.SetVector("_Details_2_tiling", new Vector2(tiling_x2, tiling_y2));
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
    }

    
}
