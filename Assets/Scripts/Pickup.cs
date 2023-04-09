using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private LayerMask pickupMask;
    [SerializeField] private Camera playerCam;
    [SerializeField] private Transform pickupTarget;
    [Space]
    [SerializeField] private float pickupRange;
    public Rigidbody obj;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(obj)
            {
                obj = null;
                return;
            }

            Ray cameraRay = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if(Physics.Raycast(cameraRay, out RaycastHit hitInfo, pickupRange, pickupMask))
            {
                obj = hitInfo.rigidbody;
            }
        }
    }

    void FixedUpdate()
    {
        if(obj)
        {
            Vector3 directionToPoint = pickupTarget.position - obj.position;
            float distanceToPoint = directionToPoint.magnitude;

            obj.velocity = directionToPoint * 12f * distanceToPoint;
        }
    }
}
