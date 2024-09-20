using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ShipController : MonoBehaviour
{
    public AudioManager am;
    public Camera cam;
    public Camera topDownCam;
    public float speed = 200f;
    public Rigidbody rb;
    public HUDController HUD;
    public CurveController curve;

    float horizontalInput;
    private float defaultFOV;

    public enum State {Trailing, TopDown, Overdrive}
    public State state;

    public CameraShake cameraShake;
    public GameObject floor;
    public Projectile projectile;

    public bool canFire;
    public FMODUnity.EventReference FireEvent;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        speed = am.BPM;
        state = State.Trailing;
        HUD.RotateCamera();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(state == State.Trailing) {
            transform.Rotate(Vector3.back * -horizontalInput * 100f * Time.fixedDeltaTime);
        }
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        if(state == State.TopDown) {
            if(Input.GetButton("Fire") && canFire) {
                FIRE();
            }
        }
    }

    public void Boss() {
        state = State.TopDown;
        cam.enabled = false;
        topDownCam.enabled = true;
    }

    private void FIRE() {
        var projectileInst = Instantiate(projectile, transform);
        projectileInst.Fire(2, new Vector3(0,0,90));
        FMODUnity.RuntimeManager.PlayOneShot(FireEvent, transform.position);
        canFire = false;
    }
}
