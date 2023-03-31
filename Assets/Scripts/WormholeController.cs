using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormholeController : MonoBehaviour
{
    public Material worm;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDetail1(float speed)
    {
        worm.SetFloat("_Details_1_scroll_speed", speed);
    }

    public void SetDetail2(float speed)
    {
        worm.SetFloat("_Details_2_scroll_speed", speed);
    }
}
