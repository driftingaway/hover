using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionDetect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        print("u hella dead");
        if (col.gameObject.tag == "Obstacle")
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
