using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public AudioSource ding;
    public int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        ding = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseScore()
    {
        score += 1;
        ding.Play();
    }

    public void BreakWall()
    {
        
    }

}
