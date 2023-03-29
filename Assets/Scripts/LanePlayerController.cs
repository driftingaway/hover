using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanePlayerController : MonoBehaviour
{
    public float speed = 200f;
    public Rigidbody rb;

    float rotationAmount = 10f;
    float laneChangeSpeed = 10f;

    enum Lane {Left, Middle, Right};
    Lane lane;

    Vector3 targetPosition;
    Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        lane = Lane.Middle;
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

        if(lane == Lane.Left)
        {
            targetPosition = new Vector3(-15f, rb.position.y, rb.position.z);
            targetRotation = Quaternion.Euler(0f, 0f, -rotationAmount);
        }
        else if(lane == Lane.Middle)
        {
            targetPosition = new Vector3(0f, rb.position.y, rb.position.z);
            targetRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if(lane == Lane.Right)
        {
            targetPosition = new Vector3(15f, rb.position.y, rb.position.z);
            targetRotation = Quaternion.Euler(0f, 0f, rotationAmount);
        }

        // Calculate the position to move the player towards using Vector3.Lerp
        Vector3 newPosition = Vector3.Lerp(rb.position, targetPosition + forwardMove, Time.deltaTime * laneChangeSpeed);

        // Set the y and z positions to the current position to keep the same height and depth
        newPosition.y = rb.position.y;
        newPosition.z = rb.position.z;

        // Move the player to the new position
        rb.MovePosition(newPosition);

        // Rotate the player's rigidbody
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, 10f * Time.fixedDeltaTime)); // Lerp the rotation to the target rotation
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            if(lane == Lane.Right) lane = Lane.Middle;
            else if(lane == Lane.Middle) lane = Lane.Left;
        } else if (Input.GetKeyDown(KeyCode.D)) {
            if(lane == Lane.Left) lane = Lane.Middle;
            else if(lane == Lane.Middle) lane = Lane.Right;
        }
    }
}

