using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public AudioManager am;
    public float speed = 200f;
    public Rigidbody rb;

    float rotationAmount = 100f;
    float horizontalInput, verticalInput;
    public bool charging = false;

    Vector3 targetPosition;
    Quaternion targetRotation;

    public ParticleSystem chargeParticles;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        speed = am.BPM;
        rotationAmount = am.BPM;
        chargeParticles = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 forwardMove = transform.forward * speed * Time.fixedDeltaTime;
        Vector3 horizontalMove = transform.right * horizontalInput * speed * Time.fixedDeltaTime;
        Vector3 verticalMove = transform.up * verticalInput * speed * Time.fixedDeltaTime;

        //Use Mathf.Clamp to limit the horizontal position
        float clampedHorizontalPosition = Mathf.Clamp(rb.position.x + horizontalMove.x, -30f, 30f);
        float clampedVerticalPosition = Mathf.Clamp(rb.position.y + verticalMove.y, -30f, 30f);
        //horizontalMove = new Vector3(clampedHorizontalPosition - rb.position.x, 0f, 0f);
        //verticalMove = new Vector3(0f, clampedVerticalPosition - rb.position.y, 0f);

        rb.MovePosition(rb.position + forwardMove);

        // Rotate player based on input
        transform.Rotate(Vector3.back * horizontalInput * rotationAmount * Time.fixedDeltaTime);
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        //print(rb.rotation);

        //var emissionModule = chargeParticles.emission;
        if(charging)
        {
            chargeParticles.Play();
        }
        else
        {
            chargeParticles.Stop();
        }
    }
}