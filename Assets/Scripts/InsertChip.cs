using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertChip : MonoBehaviour
{
    public Transform ChipSlot;
    public Transform ChipPos;
    private Pickup pickup;
    public GameObject player;

    public bool orangeSlotted = false;
    public bool blueSlotted = false;
    public bool purpleSlotted = false;
    public bool slotted = false;
    public string slottedName;

    // Start is called before the first frame update
    void Start()
    {
        pickup = player.GetComponent<Pickup>();
    }

    // Update is called once per frame
    void Update()
    {
        if(orangeSlotted || blueSlotted || purpleSlotted)
        {
            slotted = true;
        }
        else
        {
            slotted = false;
        }

        if(pickup.obj)
        {
            if(orangeSlotted && pickup.obj.name == "OrangeChip")
            {
                orangeSlotted = false;
            }

            if(blueSlotted && pickup.obj.name == "BlueChip")
            {
                blueSlotted = false;
            }

            if(purpleSlotted && pickup.obj.name == "PurpleChip")
            {
                purpleSlotted = false;
            }
            //print(ChipPos);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.name == "OrangeChip" && !orangeSlotted && !slotted)
        {
            orangeSlotted = true;
            helper();
        }

        if(col.gameObject.name == "BlueChip" && !blueSlotted && !slotted)
        {
            blueSlotted = true;
            helper();
        }

        if(col.gameObject.name == "PurpleChip" && !purpleSlotted && !slotted)
        {
            purpleSlotted = true;
            helper();
        }

        void helper()
        {
            pickup.obj = null;
            slottedName = col.gameObject.name;
            col.transform.position = ChipPos.position;
            col.transform.rotation = ChipPos.rotation;
        }
    }

    // Hold chip in place (necessary to combat weird physics from station rotation)
    void OnCollisionStay(Collision col)
    {
        if(slotted && col.gameObject.name == slottedName)
        {
            col.transform.position = ChipPos.position;
            col.transform.rotation = ChipPos.rotation;
        }
    }
}
