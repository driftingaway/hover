using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShutter : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
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
    }
}
