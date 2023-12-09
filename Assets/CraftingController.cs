using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum CycleDir { None, Left, Right };

public class CraftingController : MonoBehaviour
{
    [SerializeField] private ResourceType _selectedType;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _pourTrigger = "Cast";

    [Header("Molds")]
    [SerializeField] private Transform _moldParent;
    private GameObject _instantiatedMold;
    private List<Mold> _molds = new List<Mold>();
    private int currentMold;

    public Mold GetCurrentPart()
    {
        return _molds[currentMold];
    }

    public ResourceType GetCurrentMaterial()
    {
        return _selectedType;
    }

    public void StartCraftingMode()
    {
        _molds = GameManager.i.GetMoldsCopy();
        CycleMold(CycleDir.None);
        CycleMetal(CycleDir.None);
    }

    public void StartPour()
    {
        if (!CanAffordCurrentMold()) return;

        GameManager.i.RemoveResource(_selectedType, _molds[currentMold].MoldCastCost);
        _animator.SetTrigger(_pourTrigger);

        var part = MakePartFromCurrentSettings();
        GameManager.i.Inventory.AddToInventory(part);
    }

    private Part MakePartFromCurrentSettings()
    {
        var part = new Part();
        part.Material = GetCurrentMaterial();
        part.Type = _molds[currentMold].CompletedPart.Type;
        part.Material = GetCurrentMaterial();
        return part;
    }

    public void CycleMetalLeft() => CycleMetal(CycleDir.Left);
    public void CycleMetalRight() => CycleMetal(CycleDir.Right);

    private void CycleMetal(CycleDir dir)
    {
        if (_selectedType == ResourceType.Tin && dir == CycleDir.Left) _selectedType = ResourceType.Gold; 
        else if (_selectedType == ResourceType.Gold && dir == CycleDir.Right) _selectedType = ResourceType.Tin; 
        else if (dir != CycleDir.None) _selectedType += dir == CycleDir.Left ? -1 : 1;
        UIManager.i.DisplaySelectedMetal(_selectedType);
    }


    public void CycleMoldLeft() => CycleMold(CycleDir.Left);
    public void CycleMoldRight() => CycleMold(CycleDir.Right);

    void CycleMold(CycleDir dir)
    {
        currentMold += dir == CycleDir.Left ? -1 : 1;
        if (currentMold < 0) currentMold = _molds.Count;
        if (currentMold >= _molds.Count) currentMold = 0;

        if (_instantiatedMold) Destroy(_instantiatedMold);
        _instantiatedMold = Instantiate(_molds[currentMold].MoldPrefab, _moldParent);
        UIManager.i.DisplaySelectedMold(_molds[currentMold].CompletedPart.MoldName, _molds[currentMold].MoldCastCost);
    }

    private bool CanAffordCurrentMold()
    {
        return GameManager.i.GetResourceAmount(_selectedType) >= _molds[currentMold].MoldCastCost;
    }
}
