using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class AssemblyUI : MonoBehaviour
{
    [SerializeField] private Transform _previewPartParent;

    [Header("Part Selector")]
    [SerializeField] private Image _selectorPreviewImage;
    [SerializeField] private TextMeshProUGUI _selectorPreviewName;
    [SerializeField] private JewelryObjectCoordinator _currentPart;

    private PartType _currentPartType;
    private List<Part> _parts = new List<Part>();
    private int _currentPartIndex;
    private int _currentSlotIndex;
    private List<PartSlot> _slots = new List<PartSlot>();
    private Part _currentlySelectedPart;
    Quaternion _currentPreviewRotation;

    private void OnEnable()
    {
        NextStage();
    }

    private void NextStage()
    {
        _currentPartType = PartType.Band;
        _parts = GameManager.i.Inventory.GetPartsListByType(_currentPartType);
        _currentPartIndex = 0;
    }

    public void CycleSelectedPartLeft() => CycleSelectedPart(CycleDir.Left);
    public void CycleSelectedPartRight() => CycleSelectedPart(CycleDir.Right);
    private void CycleSelectedPart(CycleDir dir)
    {
        var oldIndex = _currentPartIndex;
        _currentPartIndex += dir == CycleDir.Right ? 1 : -1;
        if (_currentPartIndex < 0) _currentPartIndex = _parts.Count - 1;
        if (_currentPartIndex >= _parts.Count) _currentPartIndex = 0;

        if (oldIndex != _currentPartIndex) UpdateSelectedPart();
    }

    private void UpdateSelectedPart()
    {
        var selected = _parts[_currentPartIndex];
        _selectorPreviewImage.sprite = selected.previewImage;
        _selectorPreviewName.text = selected.Name;

        if (_currentPartType == PartType.Band) UpdatePreviewBasePart(selected);
    }

    private void UpdatePreviewBasePart(Part selected)
    {
        Destroy(_currentPart.gameObject);
        _currentPart.AddBasePart(selected.PartPrefab);
        _currentPreviewRotation = Quaternion.identity;
    }


}
