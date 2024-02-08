using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public int combo = 1;
    public TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseScore()
    {
        score += (100 * combo);
        text.text = score.ToString();
    }

    public void IncreaseCombo()
    {
        combo += 1;
    }

    public void ResetCombo()
    {
        combo = 1;
    }

    public void BreakWall()
    {
        
    }

}
