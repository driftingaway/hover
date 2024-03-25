using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShipController : MonoBehaviour
{
    public AudioManager am;
    public Camera cam;
    public float speed = 200f;
    public Rigidbody rb;
    public HUDController HUD;

    float horizontalInput;
    private float defaultFOV;

    public enum State {Trailing, Overdrive}
    public enum Ideology {FarLeft, Left, Center, Right, FarRight}
    public State state;
    Ideology lane;

    Vector3 targetPosition;
    Quaternion targetRotation, overheadTargetRotation;

    public bool charging = false;
    public ParticleSystem chargeParticles;
    public GameObject borders;

    private Sequence shipRotSeq;
    public CameraShake cameraShake;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        speed = am.BPM;
        state = State.Trailing;
        chargeParticles = gameObject.GetComponentInChildren<ParticleSystem>();
        lane = Ideology.Center;
        ShipRotation();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    void Update()
    {
        if(charging)
        {
            chargeParticles.Play();
        }
        else
        {
            chargeParticles.Stop();
        }
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
                SwitchLane(lane);
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
                SwitchLane(lane);
            }
        }
    }

    private void ShipRotation()
    {
        if(shipRotSeq != null)
        {
            shipRotSeq.Kill();
        }
        shipRotSeq = DOTween.Sequence();
        shipRotSeq.Append(cam.transform.DORotate(new Vector3(0, 0, 0), 20f));
        shipRotSeq.Append(cam.transform.DORotate(new Vector3(0, 0, -35), 20f));
        shipRotSeq.Append(cam.transform.DORotate(new Vector3(0, 0, 0), 20f));
        shipRotSeq.Append(cam.transform.DORotate(new Vector3(0, 0, 35), 20f));
        shipRotSeq.Append(cam.transform.DORotate(new Vector3(0, 0, 0), 20f));
        shipRotSeq.SetLoops(-1, LoopType.Yoyo);
    }

    private void SwitchLane(Ideology lane)
    {
        if(lane == Ideology.FarLeft)
        {
            rb.DOMoveX(-30, .2f);
        }
        else if(lane == Ideology.Left)
        {
            rb.DOMoveX(-15, .2f);
        }
        else if(lane == Ideology.Center)
        {
            rb.DOMoveX(0, .2f);
        }
        else if(lane == Ideology.Right)
        {
            rb.DOMoveX(15, .2f);
        }
        else if(lane == Ideology.FarRight)
        {
            rb.DOMoveX(30, .2f);
        }
    }

    public void OverdriveChange(float duration)
    {
        Debug.Log("OVERDRIVE!");
        if(state == State.Trailing)
        {
            state = State.Overdrive;
            borders.SetActive(true);
            HUD.ChangeFOV(155, duration);
            HUD.BlackBars(40, duration);
            shipRotSeq.Kill();
            cam.transform.DORotate(new Vector3(0, 0, 0), duration);
        }
        else if(state == State.Overdrive)
        {
            state = State.Trailing;
            lane = Ideology.Center;
            SwitchLane(lane);
            borders.SetActive(false);
            HUD.ChangeFOV(150, duration);
            HUD.BlackBars(100, duration);
            ShipRotation();
        }
    }
}
