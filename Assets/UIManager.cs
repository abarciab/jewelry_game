using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager i;
    private void Awake() { i = this; }

    [SerializeField] private GameObject _topBar;
    [SerializeField] private GameObject _navigationButtons;
    [SerializeField] private GameObject _forgedItemParent;
    private CameraController _cameraController;

    [Header("Crafting")]
    [SerializeField] private TextMeshProUGUI _newForgedItemName;
    [SerializeField] private TextMeshProUGUI _selectedMetal;
    [SerializeField] private TextMeshProUGUI _selectedMold;
    [SerializeField] private TextMeshProUGUI _selectedCost;
    [SerializeField] private Image _newForgedItemImage;
    [SerializeField] private Image _newForgedItemMaterialTrim;

    [Header("Colors")]
    [SerializeField] private List<Color> _materialColors = new List<Color>();

    private CraftingController _craftingController;

    public void DisplayNewForgedItem()
    {
        var newPartData = _craftingController.GetCurrentPart();
        var material = _craftingController.GetCurrentMaterial();
        print((int)material);
        var color = _materialColors[(int)material - 3];

        _newForgedItemImage.color = color;
        _newForgedItemName.color = color;
        _newForgedItemMaterialTrim.GetComponent<Outline>().effectColor = color;
        _newForgedItemName.text = newPartData.CompletedPart.Name;
        _newForgedItemImage.sprite = newPartData.CompletedPart.previewImage;

        _forgedItemParent.SetActive(true);
    }

    public void DisplaySelectedMetal(ResourceType type)
    {
        _selectedMetal.text = type.ToString();
    }

    public void DisplaySelectedMold(string moldName, int moldCost)
    {
        _selectedMold.text = moldName.Replace("MATERIAL", "").ToLower();
        _selectedCost.text = moldCost.ToString();
    }

    private void Start()
    {
        _cameraController = FindObjectOfType<CameraController>();
        _craftingController = FindObjectOfType<CraftingController>();
    }

    public void EnterCraftingMode()
    {       
        _craftingController.StartCraftingMode();
        _navigationButtons.SetActive(false);
        _cameraController.EnterCraftingMode();
    }

    public void EnterEnvironmentMode()
    {
        _navigationButtons.SetActive(true);
        _cameraController.EnterEnvironmentMode();
    }
}
