using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ShipController : MonoBehaviour
{
    public AudioManager am;
    public Camera cam;
    public float speed = 200f;
    public Rigidbody rb;
    public HUDController HUD;
    public CurveController curve;

    float horizontalInput;
    private float defaultFOV;

    public enum State {Trailing, Overdrive}
    public enum Ideology {FarLeft, Left, Center, Right, FarRight}
    public State state;
    Ideology lane;

    Vector3 targetPosition;
    Quaternion targetRotation, overheadTargetRotation;

    public bool charging = false;
    public ParticleSystem overdriveParticles;
    public GameObject borders;

    private Sequence shipRotSeq;
    public CameraShake cameraShake;
    private float od;
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
        lane = Ideology.Center;
        od = overdriveParticles.main.startLifetime.constant;
        HUD.RotateCamera();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.back * -horizontalInput * 100f * Time.fixedDeltaTime);
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        if(state == State.Overdrive)
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                if(lane == Ideology.Left)
                {
                    lane = Ideology.FarLeft;
                }
                else if(lane == Ideology.Center)
                {
                    lane = Ideology.Left;
                }
                else if(lane == Ideology.Right)
                {
                    lane = Ideology.Center;
                }
                else if(lane == Ideology.FarRight)
                {
                    lane = Ideology.Right;
                }
                SwitchLane(lane, .2f);
            }

            if(Input.GetKeyDown(KeyCode.D))
            {
                if(lane == Ideology.FarLeft)
                {
                    lane = Ideology.Left;
                }
                else if(lane == Ideology.Left)
                {
                    lane = Ideology.Center;
                }
                else if(lane == Ideology.Center)
                {
                    lane = Ideology.Right;
                }
                else if(lane == Ideology.Right)
                {
                    lane = Ideology.FarRight;
                }
                SwitchLane(lane, .2f);
            }
        }
        if(Input.GetButton("Fire") && canFire) {
            FIRE();
        }
    }

    private void SwitchLane(Ideology lane, float duration)
    {
        if(lane == Ideology.FarLeft)
        {
            rb.DOMoveX(-30, duration);
        }
        else if(lane == Ideology.Left)
        {
            rb.DOMoveX(-15, duration);
        }
        else if(lane == Ideology.Center)
        {
            rb.DOMoveX(0, duration);
        }
        else if(lane == Ideology.Right)
        {
            rb.DOMoveX(15, duration);
        }
        else if(lane == Ideology.FarRight)
        {
            rb.DOMoveX(30, duration);
        }
    }

    public void OverdriveChange(float duration)
    {
        if(state == State.Trailing)
        {
            state = State.Overdrive;
            borders.SetActive(true);
            floor.SetActive(true);
            HUD.ChangeFOV(155, duration);
            HUD.BlackBars(50, duration);
            //HUD.rotateSeq.Kill();
            cam.transform.DORotate(new Vector3(50, 0, 0), duration);
            cam.transform.DOMove(new Vector3(0, 8f, -6f), duration);
            curve.SetValues(.2f, .2f, duration);
            overdriveParticles.Play();
            am.HitFlash(Color.white, .15f);
        }
        else if(state == State.Overdrive)
        {
            state = State.Trailing;
            borders.SetActive(false);
            floor.SetActive(false);
            HUD.ChangeFOV(150, duration);
            HUD.BlackBars(100, duration);
            cam.transform.DORotate(new Vector3(0, 0, 0), duration).OnComplete(() => HUD.RotateCamera());
            cam.transform.DOMove(new Vector3(0, 4.5f, -8.15f), duration);
            lane = Ideology.Center;
            SwitchLane(lane, duration);
            curve.SetValues(.8f, .8f, duration);
            overdriveParticles.Stop();
        }
    }

    private void FIRE() {
        var projectileInst = Instantiate(projectile, transform);
        projectileInst.Fire(2, new Vector3(0,0,90));
        FMODUnity.RuntimeManager.PlayOneShot(FireEvent, transform.position);
        canFire = false;
    }
}
