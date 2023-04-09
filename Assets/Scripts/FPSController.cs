using UnityEngine;
using System.Collections;

public class FPSController : MonoBehaviour {
	
	// public vars
	public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;
	public float walkSpeed = 6;
	public float jumpForce = 220;
	
	// System vars
	bool grounded;
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	Vector3 moveDir;
	float verticalInput, horizontalInput;
	float verticalLookRotation;
	Transform cameraTransform;
	Rigidbody rb;
    Camera cam;
	
	void Awake() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
        cam = GetComponentInChildren<Camera>();
		cameraTransform = cam.transform;
		rb = GetComponent<Rigidbody>();
	}
	
	void Update() {
		// Look rotation:
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation,-60,60);
		cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;
		
		// Calculate movement:
	    horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw("Vertical");
	}
	
	void FixedUpdate() {
		// Apply movement to rigidbody
		moveDir = transform.forward * verticalInput + transform.right * horizontalInput;
		rb.AddForce(moveDir.normalized * walkSpeed);
	}
}
