using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HUDController : MonoBehaviour
{
    public RectTransform topBar, bottomBar;
    public Camera cam;
    private Sequence mySequence, mySequence2, fovSequence;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BlackBars(float size, float duration)
    {
        if(mySequence != null)
        {
            mySequence.Kill();
            mySequence2.Kill();
        }

        mySequence = DOTween.Sequence();
        mySequence2 = DOTween.Sequence();

        mySequence.Append(topBar.DOAnchorPosY(size, duration));
        mySequence2.Append(bottomBar.DOAnchorPosY(-size, duration));

        mySequence.Play();
        mySequence2.Play();
    }

    public void ChangeFOV(float fov, float duration)
    {
        if(fovSequence != null)
        {
            fovSequence.Kill();
        }
        fovSequence = DOTween.Sequence();
        fovSequence.Append(cam.DOFieldOfView(fov, duration));
        fovSequence.Play();
    }
}
