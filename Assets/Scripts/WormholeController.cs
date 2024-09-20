using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WormholeController : MonoBehaviour
{
    public Material worm;
    public Material player;
    public Material circle;
    public Material walls;
    public Material bg;
    public Material code;
    public AudioManager am;
    public List<GameObject> sceneList = new List<GameObject>();
    private int sceneId = 0; 

    public void InitWormhole() {
        ToggleScene(false, true);
        sceneList[0].SetActive(true);
    }

    public void ToggleScene(bool toggle, bool all) {
        if(all) {
            foreach(GameObject scene in sceneList) {
                scene.SetActive(toggle);
            }
        } else {
            sceneList[sceneId].SetActive(toggle);
        }
    }

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

        Sequence mySequence4 = DOTween.Sequence();
        mySequence4.Append(code.DOFade(startIntensity, "_Color", 0));
        mySequence4.Append(code.DOFade(endIntensity, "_Color", fixedDuration*2f));

        mySequence.SetLoops(count, LoopType.Restart);
        mySequence2.SetLoops(count, LoopType.Restart);
        mySequence3.SetLoops(count, LoopType.Restart);
        mySequence4.SetLoops(count, LoopType.Restart);
    }

    public void SetColor(Color color1, Color color2)
    {
        worm.SetColor("_Details_1_colour", color1*5);
        worm.SetColor("_Details_2_colour", color2*5);
        walls.SetColor("_Color", color1*2);
        code.SetColor("_Color", color1*2);
        bg.SetColor("_Color",color1);
    }

    public void SceneTransition(bool boss) 
    {
        if(!boss) {
            sceneList[sceneId].SetActive(false);
            sceneId += 1;
            sceneId = sceneId % sceneList.Count;
            sceneList[sceneId].SetActive(true);
        }
    } 
}
