using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShipController : MonoBehaviour
{
    public AudioManager am;
    public Camera trailingCam, overheadCam, backwardsCam;
    public float speed = 200f;
    public Rigidbody rb;
    public RectTransform topBar, bottomBar;

    float rotationAmount = 100f;
    float horizontalInput;
    private float defaultFOV;

    public enum State {Trailing, Overhead, Backwards};
    State state;

    Vector3 targetPosition;
    Quaternion targetRotation, overheadTargetRotation;

    public bool charging = false;
    public ParticleSystem chargeParticles;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        speed = am.BPM;
        state = State.Trailing;
        chargeParticles = gameObject.GetComponentInChildren<ParticleSystem>();
        ShipRotation();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (state == State.Backwards) 
        {
            transform.Rotate(Vector3.back * horizontalInput * rotationAmount * Time.fixedDeltaTime);
        }
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.G))
        {
            Switch(0);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Switch(1);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Switch(2);
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

    public void Switch(int newState)
    {
        if(newState == 0)
        {
            state = State.Trailing;
            trailingCam.enabled = true;
            overheadCam.enabled = false;
            backwardsCam.enabled = false;
        }
        else if(newState == 1)
        {
            state = State.Overhead;
            trailingCam.enabled = false;
            overheadCam.enabled = true;
            backwardsCam.enabled = false;
            transform.rotation = Quaternion.identity;
        }
        else if(newState == 2)
        {
            state = State.Backwards;
            trailingCam.enabled = false;
            overheadCam.enabled = false;
            backwardsCam.enabled = true;
        }
    }

    public void ChangeFOV(float duration)
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(trailingCam.DOFieldOfView(155, duration));
        mySequence.Append(trailingCam.DOFieldOfView(150, 0.1f));

        Sequence mySequence2 = DOTween.Sequence();
        mySequence2.Append(topBar.DOAnchorPosY(60, duration));
        mySequence2.Append(topBar.DOAnchorPosY(100, 0.1f));

        Sequence mySequence3 = DOTween.Sequence();
        mySequence3.Append(bottomBar.DOAnchorPosY(-60, duration));
        mySequence3.Append(bottomBar.DOAnchorPosY(-100, 0.1f));

        mySequence.Play();
        mySequence2.Play();
        mySequence3.Play();
    }

    private void ShipRotation()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(trailingCam.transform.DORotate(new Vector3(0, 0, 0), 20f));
        mySequence.Append(trailingCam.transform.DORotate(new Vector3(0, 0, -35), 20f));
        mySequence.Append(trailingCam.transform.DORotate(new Vector3(0, 0, 0), 20f));
        mySequence.Append(trailingCam.transform.DORotate(new Vector3(0, 0, 35), 20f));
        mySequence.Append(trailingCam.transform.DORotate(new Vector3(0, 0, 0), 20f));
        mySequence.SetLoops(-1, LoopType.Yoyo);
    }
}
