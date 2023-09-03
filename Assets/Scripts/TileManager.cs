using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

[System.Serializable]
public class TileManager : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject instObjects, backInstObjects;
    public Transform player;
    public AudioManager am;
    float speed, secPerBeat;

    float songPos;
    float songPositionInBeatsPrecise;

    private List<GameObject> activeTiles = new List<GameObject>();
    public List<float> notePos = new List<float>();
    public List<float> notes = new List<float>();
    public AnimationCurve curve;

    // Start is called before the first frame update
    void Awake()
    {
        speed = am.BPM/2;
    }

    // move obstacles forward, everything else stays fixed
    void FixedUpdate()
    {
        //Vector3 forwardMove = instObjects.transform.forward * speed * Time.fixedDeltaTime * bezierMult;
        //Vector3 backwardMove = backInstObjects.transform.forward * speed * Time.fixedDeltaTime;
        //instObjects.transform.position = instObjects.transform.position - forwardMove;
        //backInstObjects.transform.position = backInstObjects.transform.position + backwardMove;
    }

    void Update()
    {
        print(instObjects.transform.position.z);
    }

    public void Bezier(float pos, float duration) {
        instObjects.transform.DOMoveZ(pos*speed, duration).SetEase(curve);;
    }

    public List<float> SpawnNotes(List<float> notes)
    {
        foreach(float note in notes)
        {
            print(note);
            print(speed);
            GameObject newTile = Instantiate(tiles[0], new Vector3(0f, 0f, note), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
            newTile.transform.parent = instObjects.transform;
            notePos.Add(newTile.transform.position.z);
            print(newTile.transform.position.z);
        }
        return notePos;
    }

    public void SpawnNote(float note)
    {
        GameObject newTile = Instantiate(tiles[0], new Vector3(0f, 0f, instObjects.transform.position.z + note*speed), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        newTile.transform.parent = instObjects.transform;
        activeTiles.Add(newTile);

        if(activeTiles.Count > 50)
        {
            Destroy(activeTiles[0]);
            activeTiles.RemoveAt(0);
        }
    }

    public void SpawnTile(int id, float note, float xOffset) 
    {
        // spawn tile
        GameObject newTile = Instantiate(tiles[id], new Vector3(xOffset, 0f, note * speed), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
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
