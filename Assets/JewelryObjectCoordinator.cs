using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartSlot
{
    public int StageID;
    public PartType Type;
    public PartObjectCoordinator slotParent;
    public int SlotID;
    [HideInInspector] public bool filled;
}

public class JewelryObjectCoordinator : MonoBehaviour
{
    [SerializeField] private List<PartObjectCoordinator> _partObjects = new List<PartObjectCoordinator>();
    private List<PartSlot> _openSlots = new List<PartSlot>();

    public void AddBasePart(GameObject prefab)
    {
        foreach (var p in _partObjects) Destroy(p.gameObject);
        _partObjects.Clear();
        _openSlots.Clear();

        var newBase = Instantiate(prefab, transform);
        newBase.transform.localRotation = Quaternion.identity;
        var partObj = newBase.GetComponent<PartObjectCoordinator>();
        _partObjects.Add(partObj);

        _openSlots = partObj.GetSlots();
    }

    public void AddNewPart(GameObject prefab, PartSlot selectedSlot)
    {
        var partObj = prefab.GetComponent<PartObjectCoordinator>();
        _partObjects.Add(partObj);
        _openSlots.AddRange(partObj.GetSlots());

        foreach (var slot in _openSlots) if (slot == selectedSlot) slot.filled = true;
    }
}
