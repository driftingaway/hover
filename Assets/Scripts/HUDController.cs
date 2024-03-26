using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HUDController : MonoBehaviour
{
    public RectTransform topBar, bottomBar;
    public Camera cam;
    public Sequence barSequence, barSequence2, fovSequence, rotateSeq;
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
        if(barSequence != null)
        {
            barSequence.Kill();
            barSequence2.Kill();
        }

        barSequence = DOTween.Sequence();
        barSequence2 = DOTween.Sequence();

        barSequence.Append(topBar.DOAnchorPosY(size, duration));
        barSequence2.Append(bottomBar.DOAnchorPosY(-size, duration));

        barSequence.Play();
        barSequence2.Play();
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

    public void RotateCamera()
    {
        if(rotateSeq != null)
        {
            rotateSeq.Kill();
        }
        rotateSeq = DOTween.Sequence();
        rotateSeq.Append(cam.transform.DORotate(new Vector3(0, 0, 0), 20f));
        rotateSeq.Append(cam.transform.DORotate(new Vector3(0, 0, -35), 20f));
        rotateSeq.Append(cam.transform.DORotate(new Vector3(0, 0, 0), 20f));
        rotateSeq.Append(cam.transform.DORotate(new Vector3(0, 0, 35), 20f));
        rotateSeq.Append(cam.transform.DORotate(new Vector3(0, 0, 0), 20f));
        rotateSeq.SetLoops(-1, LoopType.Yoyo);
    }
}
