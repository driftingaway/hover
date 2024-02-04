using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class Pattern : ScriptableObject
{
    public float speed1 = 1, speed2 = 1, tiling_x1 = 1, tiling_y1 = 1, tiling_x2 = 1, tiling_y2 = 1;
}
