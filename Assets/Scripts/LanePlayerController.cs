using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanePlayerController : MonoBehaviour
{
    public AudioManager am;
    public Camera trailingCam, overheadCam;
    public float speed = 200f;
    public Rigidbody rb;

    float rotationAmount = -10f;
    float laneChangeSpeed = 25f;

    enum Lane {L, M, R};
    Lane lane;

    enum State {Trailing, Overhead};
    State state;

    Vector3 targetPosition;
    Quaternion targetRotation, overheadTargetRotation;

    public bool charging = false;
    public ParticleSystem chargeParticles;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        lane = Lane.M;
        speed = am.BPM;
        state = State.Trailing;
        chargeParticles = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 forwardMove = transform.forward * speed * Time.fixedDeltaTime;
        //Vector3 horizontalMove = transform.right * horizontalInput * speed * Time.fixedDeltaTime;

        // Use Mathf.Clamp to limit the horizontal position
        //float clampedHorizontalPosition = Mathf.Clamp(rb.position.x + horizontalMove.x, -maxHorizontalPosition, maxHorizontalPosition);
        //horizontalMove = new Vector3(clampedHorizontalPosition - rb.position.x, 0f, 0f);

        rb.MovePosition(rb.position + forwardMove);

        if(lane == Lane.L)
        {
            targetPosition = new Vector3(-15f, rb.position.y, rb.position.z);
            targetRotation = Quaternion.Euler(0f, 0f, -rotationAmount);
            overheadTargetRotation = Quaternion.Euler(0f, 0f, -rotationAmount / 10);
        }
        else if(lane == Lane.M)
        {
            targetPosition = new Vector3(0f, rb.position.y, rb.position.z);
            targetRotation = Quaternion.Euler(0f, 0f, 0f);
            overheadTargetRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if(lane == Lane.R)
        {
            targetPosition = new Vector3(15f, rb.position.y, rb.position.z);
            targetRotation = Quaternion.Euler(0f, 0f, rotationAmount);
            overheadTargetRotation = Quaternion.Euler(0f, 0f, rotationAmount / 10);
        }

        // Calculate the position to move the player towards using Vector3.Lerp
        Vector3 newPosition = Vector3.Lerp(rb.position, targetPosition + forwardMove, Time.deltaTime * laneChangeSpeed);

        // Set the y and z positions to the current position to keep the same height and depth
        newPosition.y = rb.position.y;
        newPosition.z = rb.position.z;

        // Move the player to the new position
        rb.MovePosition(newPosition);

        if(state == State.Trailing)
        {
            // Rotate the player's rigidbody
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, 100f * Time.fixedDeltaTime)); // Lerp the rotation to the target rotation
        }
        else if(state == State.Overhead)
        {
            // Rotate the player's rigidbody
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, overheadTargetRotation, 100f * Time.fixedDeltaTime)); // Lerp the rotation to the target rotation
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            if(charging)
            {
                lane = Lane.L;
                charging = false;
            }
            else
            {
                if(lane == Lane.R) lane = Lane.M;
                else if(lane == Lane.M) lane = Lane.L;
            }
        } else if (Input.GetKeyDown(KeyCode.D)) {
            if(charging)
            {
                lane = Lane.R;
                charging = false;
            }
            else
            {
                if(lane == Lane.L) lane = Lane.M;
                else if(lane == Lane.M) lane = Lane.R;
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Switch();
        }

        if(charging)
        {
            chargeParticles.Play();
        }
        else
        {
            chargeParticles.Stop();
        }
    }

    public void Switch()
    {
        if(state == State.Trailing)
        {
            state = State.Overhead;
            trailingCam.enabled = false;
            overheadCam.enabled = true;
        }
        else if(state == State.Overhead)
        {
            state = State.Trailing;
            trailingCam.enabled = true;
            overheadCam.enabled = false;
        }
    }
}
