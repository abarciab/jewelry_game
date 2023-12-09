using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartSlotDetails : PartSlot
{
    public Vector3 offset;
}

public class PartObjectCoordinator : MonoBehaviour
{
    [SerializeField] private List<PartSlotDetails> _slots = new List<PartSlotDetails>();

    public List<PartSlot> GetSlots()
    {
        return new List<PartSlot>(_slots);
    }
}
