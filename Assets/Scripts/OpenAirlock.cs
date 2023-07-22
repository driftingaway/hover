using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAirlock : MonoBehaviour
{
    private Animator anim;
    private BoxCollider box;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        box = GetComponentInChildren<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            ToggleAirlock(1);
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
            ToggleAirlock(-1);
        }
    }

    void ToggleAirlock(float direction)
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

        if(direction == -1)
        {
            box.enabled = true;
        }
        else
        {
            box.enabled = false;
        }

        anim.SetFloat("Direction", direction);
        anim.Play("OpenAirlock", 0, currentProgress);
    }
}
