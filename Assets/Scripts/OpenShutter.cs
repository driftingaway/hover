using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OpenShutter : MonoBehaviour
{
    private Animator anim;
    public Material stars;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update() {
        if (Input.GetKeyDown("space"))
        {
            DeleteEarth();
        }

        if(Input.GetKeyDown("s")) 
        {
            ToggleShutter(1f);
        }
    }

    public void ToggleShutter(float direction)
    {
        float currentProgress = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if(currentProgress > 1 || currentProgress < 0)
        {
            if(direction == -1)
            {
                currentProgress = 1;
            }
            else
            {
                currentProgress = 0;
            }
        }
        
        anim.SetFloat("Direction", direction);
        anim.Play("OpenShutter", 0, currentProgress);
        DOTween.To(() => RenderSettings.reflectionIntensity, x=>RenderSettings.reflectionIntensity = x, 1f, 15f);
        DOTween.To(() => RenderSettings.ambientIntensity, x=>RenderSettings.ambientIntensity = x, .73f, 15f);
    }

    public void DeleteEarth() {
        RenderSettings.reflectionIntensity = 0;
        RenderSettings.ambientIntensity = 0;
        RenderSettings.skybox = stars;
    }
}
