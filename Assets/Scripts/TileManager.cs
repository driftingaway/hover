using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class TileManager : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject instObjects, backInstObjects;
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
        Vector3 backwardMove = backInstObjects.transform.forward * speed * Time.fixedDeltaTime;
        instObjects.transform.position = instObjects.transform.position - forwardMove;
        backInstObjects.transform.position = backInstObjects.transform.position + backwardMove;
    }

    public void SpawnTile(int id, float noteOffset, float xOffset) 
    {
        // spawn tile
        GameObject newTile = Instantiate(tiles[id], new Vector3(xOffset, 0f, player.position.z) + (transform.forward * 60f * noteOffset), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        if(id == 2)
        {
            newTile.transform.parent = backInstObjects.transform;
        }
        else
        {
            newTile.transform.parent = instObjects.transform;
        }

        activeTiles.Add(newTile);

        if(activeTiles.Count > 50)
        {
            Destroy(activeTiles[0]);
            activeTiles.RemoveAt(0);
        }
    } 
}
