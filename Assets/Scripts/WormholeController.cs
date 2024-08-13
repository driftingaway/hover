using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WormholeController : MonoBehaviour
{
    public GameObject scene1;
    public GameObject scene2;
    public Material worm;
    public Material player;
    public Material circle;
    public Material walls;
    public Material bg;
    public AudioManager am;
    private int scene = 0; 

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

        Sequence mySequence3 = DOTween.Sequence();
        mySequence3.Append(walls.DOFade(startIntensity, "_Color", 0));
        mySequence3.Append(walls.DOFade(endIntensity, "_Color", fixedDuration*2f));

        mySequence.SetLoops(count, LoopType.Restart);
        mySequence2.SetLoops(count, LoopType.Restart);
        mySequence3.SetLoops(count, LoopType.Restart);
    }

    public void InitColor(Color color1, Color color2)
    {
        worm.SetColor("_Details_1_colour", color1*5);
        worm.SetColor("_Details_2_colour", color2*5);
        walls.SetColor("_Color", color1*2);
        bg.SetColor("_Color",color1);
    }

    public void SceneTransition() 
    {
        if(scene == 0) {
            scene = 1;
            scene1.SetActive(false);
            scene2.SetActive(true);
        } else if(scene == 1) {
            scene = 0;
            scene1.SetActive(true);
            scene2.SetActive(false);
        }
    }

    
}
