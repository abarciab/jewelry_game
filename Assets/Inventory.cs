using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable]
public class Part
{
    [SerializeField] private string _name;
    public PartType Type;
    public Sprite previewImage;
    [HideInInspector] public ResourceType Material;
    public int Stage;
    public GameObject PartPrefab;

    [Range(0, 1)] public float Rarity;
    [ConditionalField(nameof(Type), false, PartType.Band)] public int NumSettingSlots = 1;
    [ConditionalField(nameof(Type), false, PartType.Setting)] public int NumGemSlots = 1;

    public string Name { get { return _name.Replace("MATERIAL", Material.ToString());  } }
    public string MoldName { get { return _name.Replace("MATERIAL", "");  } }

}

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Part> _parts = new List<Part>();

    public void AddToInventory(Part newPart)
    {
        _parts.Add(newPart);
    }

    public List<Part> GetPartsListByType(PartType type)
    {
        var list = new List<Part>();
        foreach (var p in _parts) if (p.Type == type) list.Add(p);
        return list;
    }
}
