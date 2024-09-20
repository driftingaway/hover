using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFire : MonoBehaviour
{
    public GameObject projectile;
    public float speed = 4;
    float elapsed = 0f;
    float random;
    public Vector3 direction;
    public float low, high;

    void Start()
    {
        random = Random.Range(low, high);  //random float between 3 and 5
    }
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= random) {
            elapsed = elapsed % random;
            Fire();
        }
    }
    void Fire()
    {
        random = Random.Range(low, high);
        Rigidbody p = Instantiate(projectile.GetComponent<Rigidbody>(), transform.position, transform.rotation);
        p.velocity = direction * speed;
        Destroy(p.gameObject,2);
    }
}
