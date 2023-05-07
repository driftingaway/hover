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
    public GameObject following;

    public float zSpawn = 30f;
    public float tileLength = 30f;
    public float numTiles = 200f;

    private List<List<GameObject>> activeTiles = new List<List<GameObject>>();

    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> list1 = new List<GameObject>();
        List<GameObject> list2 = new List<GameObject>();
        List<GameObject> list3 = new List<GameObject>();
        List<GameObject> list4 = new List<GameObject>();
        List<GameObject> list5 = new List<GameObject>();
        List<GameObject> list6 = new List<GameObject>();
        List<GameObject> list7 = new List<GameObject>();
        List<GameObject> list8 = new List<GameObject>();
        activeTiles.Add(list1);
        activeTiles.Add(list2);
        activeTiles.Add(list3);
        activeTiles.Add(list4);
        activeTiles.Add(list5);
        activeTiles.Add(list6);
        activeTiles.Add(list7);
        activeTiles.Add(list8);

        //for(int i = 0; i <= numTiles; i++)
        //{
        //    SpawnGround();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if(player.position.z > zSpawn - tileLength * numTiles)
        { 
            //SpawnGround();
            //DeleteGround();
        }
        Vector3 p = following.transform.position;
        p.z = player.position.z;
        following.transform.position = p;
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

    public void SpawnTile(int id, float noteOffset, float rotation, string lane) 
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
        // execute block of code here
        GameObject newTile = Instantiate(tiles[id], new Vector3(laneOffset, 0f, player.position.z) + (transform.forward * 60f * noteOffset), Quaternion.Euler(new Vector3(0f, 0f, rotation)));
        //print(newTile.transform.position.z);
        newTile.transform.parent = instObjects.transform;
        activeTiles[id].Add(newTile);

        if(activeTiles[id].Count > 50)
        {
            Destroy(activeTiles[id][0]);
            activeTiles[id].RemoveAt(0);
        }
    } 

    public void DestroyWall()
    {
        Destroy(activeTiles[7][0]);
        activeTiles[7].RemoveAt(0);
    }
}
