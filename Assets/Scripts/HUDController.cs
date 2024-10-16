using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using DG.Tweening;

public class HUDController : MonoBehaviour
{
    public RectTransform topBar, bottomBar;
    public Camera cam;
    public Sequence barSequence, barSequence2, fovSequence, rotateSeq;
    public CanvasGroup canvas;
    public GameObject invertColor;
    public GameObject songTitle;
    public GameObject code;
    public WormholeController worm;

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
        rotateSeq.Append(cam.transform.DORotate(new Vector3(0, 0, -20), 1.3f));
        rotateSeq.Append(cam.transform.DORotate(new Vector3(0, 0, -25), 1.1f));
        rotateSeq.Append(cam.transform.DORotate(new Vector3(0, 0, -20), 1.7f));
        rotateSeq.Append(cam.transform.DORotate(new Vector3(0, 0, -15), 1.2f));
        rotateSeq.Append(cam.transform.DORotate(new Vector3(0, 0, -25), 1.5f));
        rotateSeq.SetLoops(-1, LoopType.Yoyo);
    }

    public IEnumerator ImpactFrame()
    {
        invertColor.SetActive(true);
        yield return new WaitForSeconds(0.03f);
        invertColor.SetActive(false);
    }

    public IEnumerator TitleDrop(float duration)
    {
        songTitle.SetActive(true);
        worm.ToggleScene(false, true);
        code.SetActive(true);
        yield return new WaitForSeconds(duration);
        code.SetActive(false);
        songTitle.SetActive(false);
        worm.ToggleScene(true, false);
    }
}
