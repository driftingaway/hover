using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class TileManager : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject instObjects;
    public Transform player;
    public AudioManager am;
    float speed;

    public float zSpawn = 30f;
    public float tileLength = 30f;
    public float numTiles = 200f;

    private List<GameObject> activeTiles = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        speed = am.BPM;
    }

    // move obstacles forward, everything else stays fixed
    void FixedUpdate()
    {
        Vector3 forwardMove = instObjects.transform.forward * speed * Time.fixedDeltaTime;
        instObjects.transform.position = instObjects.transform.position - forwardMove;
    }

    public void SpawnTile(int id, float noteOffset, string lane) 
    {
        float laneOffset = 0f;
        if(lane == "L")
        {
            laneOffset = -15f;
        }
        else if(lane == "R")
        {
            laneOffset = 15f;
        }

        // spawn tile
        GameObject newTile = Instantiate(tiles[id], new Vector3(laneOffset, 0f, player.position.z) + (transform.forward * 60f * noteOffset), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        newTile.transform.parent = instObjects.transform;
        activeTiles.Add(newTile);

        if(activeTiles.Count > 50)
        {
            Destroy(activeTiles[0]);
            activeTiles.RemoveAt(0);
        }
    } 
}
