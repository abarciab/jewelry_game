using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum ResourceType { None, Cash, Gems, Tin, Copper, Silver, Gold }
public class Node : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [Header("Stats")]
    [SerializeField] private ResourceType _type;
    [SerializeField] private int _currentValue = 10;
    [SerializeField] private float _mineTime = 3;
    private float mineCooldown;

    [Header("Effects")]
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private int _numParticles = 10;

    [Header("UI")]
    [SerializeField] private Slider _countdownSlider;
    [SerializeField] private GameObject _countdownParent;
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private TextMeshProUGUI _moneyGainText;

    [Header("Misc")]
    [SerializeField] private List<Renderer> _oreRenderer;

    private void Start()
    {
        var mat = GameManager.i.GetMaterialByType(_type);
        foreach (var renderer in _oreRenderer) renderer.material = mat;

        var particleRenderer = _particles.GetComponent<ParticleSystemRenderer>();
        particleRenderer.material = mat;
    }

    private void Update()
    {
        _countdownParent.SetActive(mineCooldown > 0);
        if (mineCooldown > 0) {
            _countdownText.text = Mathf.CeilToInt(mineCooldown).ToString();
            _countdownSlider.value = mineCooldown / _mineTime;
            mineCooldown -= Time.deltaTime;
            if (mineCooldown < 0) Mine();
            return;
        }

        if (Input.touchCount == 0) return;
        
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began) CheckForObjectTapped(touch.position);
    }

    void CheckForObjectTapped(Vector2 touchPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hitInfo;

        bool hit = Physics.Raycast(ray, out hitInfo);
        if (hit && hitInfo.collider.gameObject == gameObject) StartMining();
    }

    private void StartMining()
    {
        mineCooldown = _mineTime;
    }

    [ButtonMethod]
    public void Mine()
    {
        _animator.SetTrigger("shake");
        FindObjectOfType<CameraController>().ShakeFixed();
        GameManager.i.AddResource(_type, _currentValue);
        _particles.Emit(_numParticles);
        _moneyGainText.text = "+ " + _currentValue;
        _moneyGainText.gameObject.SetActive(true);
    }
}
