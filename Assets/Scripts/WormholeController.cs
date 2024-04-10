using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WormholeController : MonoBehaviour
{
    public Material worm;
    public Material player;
    public Material circle;
    public AudioManager am;

    public void SetPattern(float speed1, float speed2, float tiling_x1, float tiling_y1, float tiling_x2, float tiling_y2)
    {
        worm.SetFloat("_Details_1_scroll_speed", speed1);
        worm.SetFloat("_Details_2_scroll_speed", speed2);
        worm.SetVector("_Details_1_tiling", new Vector2(tiling_x1, tiling_y1));
        worm.SetVector("_Details_2_tiling", new Vector2(tiling_x1, tiling_y1));
    }

    public void Strobe(int count, float duration, float startIntensity, float endIntensity)
    {
        float fixedDuration = duration / count;

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(worm.DOFade(startIntensity, "_Details_1_colour", 0));
        mySequence.Append(worm.DOFade(endIntensity, "_Details_1_colour", fixedDuration));

        Sequence mySequence2 = DOTween.Sequence();
        mySequence2.Append(worm.DOFade(startIntensity, "_Details_2_colour", 0));
        mySequence2.Append(worm.DOFade(endIntensity, "_Details_2_colour", fixedDuration));

        mySequence.SetLoops(count, LoopType.Restart);
        mySequence2.SetLoops(count, LoopType.Restart);
    }

    public void InitColor(Color color1, Color color2)
    {
        worm.SetColor("_Details_1_colour", color1*5);
        worm.SetColor("_Details_2_colour", color2*5);
    }

    
}
