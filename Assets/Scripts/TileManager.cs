using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

[System.Serializable]
public class TileManager : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject[] terrainTiles;
    public GameObject instObjects;
    public Transform player;
    public AudioManager am;
    public Material lineMaterial;
    float speed, secPerBeat, speedMult;
    Vector3 forwardMove;

    private List<GameObject> activeTiles = new List<GameObject>();
    public List<float> notePos = new List<float>();
    public List<float> notes = new List<float>();
    public AnimationCurve curve;

    private float prevSpawnPoint = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void InitChart() {
        speed = am.BPM;
        secPerBeat = am.secPerBeat;
        speedMult = am.speedMult;
        Destroy(instObjects);
        instObjects = new GameObject("instObjects");
        activeTiles.Clear();
        SpawnTerrain(terrainTiles[0]);
    }

    // move obstacles forward, everything else stays fixed
    void FixedUpdate()
    {
        forwardMove = instObjects.transform.forward * speed * speedMult * Time.fixedDeltaTime;
        instObjects.transform.position = instObjects.transform.position - forwardMove;
    }

    void Update()
    {
        if(instObjects.transform.position.z <= prevSpawnPoint - 2900f) {
            SpawnTerrain(terrainTiles[0]);
        }
    }

    public void SpawnNote(Note note)
    {
        //Debug.Log("Spawn Note");
        GameObject newTile = Instantiate(tiles[note.type], new Vector3(note.xPos, 0f, 0f) + (transform.forward * 60f * 8f * speedMult), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        if(note.type == 1)
        {
            SpawnLine(newTile, note);
        }
        newTile.transform.parent = instObjects.transform;
        activeTiles.Add(newTile);

        if(activeTiles.Count > 5000)
        {
            Destroy(activeTiles[0]);
            activeTiles.RemoveAt(0);
        }
    }

    private void SpawnLine(GameObject newTile, Note note)
    {
        LineRenderer lRend = newTile.AddComponent<LineRenderer>();
        lRend.material = lineMaterial;
        lRend.useWorldSpace = false;
        lRend.startWidth = 3f;
        lRend.endWidth = 3f;
        lRend.positionCount = 1000;
        
        float end = speed * speedMult * secPerBeat * note.length;
        float increment = end / (lRend.positionCount - 1);

        for(int i = 0; i < lRend.positionCount; i++)
        {
            lRend.SetPosition(i, new Vector3(0, 0, i*increment));
        }
    }

    private void SpawnTerrain(GameObject terrain) {
        GameObject newTile = Instantiate(terrainTiles[0], new Vector3(0f, 0f, 1000f), Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        newTile.transform.parent = instObjects.transform;
        prevSpawnPoint = instObjects.transform.position.z + 1000;
    }
}
