using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCursor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.x -= Screen.width / 2;
        mousePos.y -= Screen.height / 2;
        transform.position = (mousePos / (1 + (transform.localScale.z / 30)));
        transform.position = new Vector3(transform.position.x + Screen.width / 2, transform.position.y + Screen.height / 2, transform.position.z);
        //Debug.Log(transform.position);
    }
}
