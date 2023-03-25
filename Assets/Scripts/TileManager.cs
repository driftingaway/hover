using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject instObjects;
    public Transform player;

    public float zSpawn = 0f;
    public float tileLength = 30f;
    public float numTiles = 200f;

    private List<List<GameObject>> activeTiles = new List<List<GameObject>>();

    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> list1 = new List<GameObject>();
        List<GameObject> list2 = new List<GameObject>();
        List<GameObject> list3 = new List<GameObject>();
        activeTiles.Add(list1);
        activeTiles.Add(list2);
        activeTiles.Add(list3);

        for(int i = 0; i <= numTiles; i++)
        {
            SpawnGround();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(player.position.z > zSpawn - (numTiles * tileLength))
        {
            SpawnGround();
            DeleteGround();
        }
    }

    public void SpawnGround()
    {
        GameObject newTile = Instantiate(tiles[0], transform.forward * zSpawn, transform.rotation);
        newTile.transform.parent = instObjects.transform;
        activeTiles[0].Add(newTile);
        zSpawn += tileLength;
    }

    public void DeleteGround()
    {
        Destroy(activeTiles[0][0]);
        activeTiles[0].RemoveAt(0);
    }

    public void SpawnTile(int id) 
    {
        // execute block of code here
        GameObject newTile = Instantiate(tiles[id], transform.forward * zSpawn, transform.rotation);
        newTile.transform.parent = instObjects.transform;
        activeTiles[id].Add(newTile);

        if(activeTiles[id].Count > 50)
        {
            Destroy(activeTiles[id][0]);
            activeTiles[id].RemoveAt(0);
        }
    } 
}
