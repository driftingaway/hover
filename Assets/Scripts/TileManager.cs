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
    public Material lineMaterial;
    float speed, secPerBeat;

    float songPos;
    float songPositionInBeatsPrecise;

    private List<GameObject> activeTiles = new List<GameObject>();
    public List<float> notePos = new List<float>();
    public List<float> notes = new List<float>();
    public AnimationCurve curve;

    // Start is called before the first frame update
    void Start()
    {
        speed = am.BPM;
        secPerBeat = am.secPerBeat;
    }

    // move obstacles forward, everything else stays fixed
    void FixedUpdate()
    {
        //print(speed);
        Vector3 forwardMove = instObjects.transform.forward * speed * Time.fixedDeltaTime;
        //Vector3 backwardMove = backInstObjects.transform.forward * speed * Time.fixedDeltaTime;
        instObjects.transform.position = instObjects.transform.position - forwardMove;
        //backInstObjects.transform.position = backInstObjects.transform.position + backwardMove;
    }

    void Update()
    {
        //print(instObjects.transform.position.z);
    }

    public void Bezier(float pos, float duration) {
        instObjects.transform.DOMoveZ(pos*speed, duration).SetEase(curve);
    }

    public void SpawnNote(Note note)
    {
        GameObject newTile = Instantiate(tiles[note.type], new Vector3(0f, 0f, 0f) + (transform.forward * 60f * 8f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        if(note.type == 2 || note.type == 3)
        {
            SpawnLine(newTile, note);
        }
        newTile.transform.parent = instObjects.transform;
        activeTiles.Add(newTile);

        if(activeTiles.Count > 50)
        {
            Destroy(activeTiles[0]);
            activeTiles.RemoveAt(0);
        }
    }

    private void SpawnLine(GameObject newTile, Note note)
    {
        //draw held note line
        LineRenderer lRend = newTile.AddComponent<LineRenderer>();
        lRend.material = lineMaterial;
        lRend.useWorldSpace = false;
        lRend.startWidth = 3f;
        lRend.endWidth = 3f;
        //lRend.alignment = LineAlignment.TransformZ;

        float end = speed * secPerBeat * note.end;
        Debug.Log(end);
        lRend.positionCount = 20;
        //Debug.Log(lRend.positionCount);
        float increment = end / (lRend.positionCount - 1);
        //Debug.Log(end);
        for(int i = 0; i < lRend.positionCount; i++)
        {
            lRend.SetPosition(i, new Vector3(0, 0, i*increment));
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
