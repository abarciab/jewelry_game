using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Resource
{
    [HideInInspector] public string name;
    public ResourceType Type;
    public int Amount { get; private set; }
    [SerializeField] private TextMeshProUGUI _displayTMP;
    [SerializeField] public Material Material;

    public void ChangeAmount(int delta)
    {
        Amount += delta;
        _displayTMP.text = Amount.ToString();
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager i;

    [Header("Resources")]
    [SerializeField] List<Resource> _resources = new List<Resource>();

    [Header("Molds")]
    [SerializeField, DisplayInspector] private List<Mold> molds = new List<Mold>();

    [Header("Misc")]
    public Inventory Inventory;
    [SerializeField] UIManager _ui;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Fade fade;
    [SerializeField] MusicPlayer music;

    public List<Mold> GetMoldsCopy()
    {
        return new List<Mold>(molds);
    }

    private void OnValidate()
    {
        foreach (var r in _resources) r.name = r.Type + ": " + r.Amount;
    }

    private void Start()
    {
        fade.Hide();
    }

    public Material GetMaterialByType(ResourceType type) => _resources.Where(r => r.Type == type).First().Material;

    public void RemoveResource(ResourceType type, int amount) => ChangeResourceAmount(type, -amount);
    public void AddResource(ResourceType type, int amount) => ChangeResourceAmount(type, amount);
    private void ChangeResourceAmount(ResourceType type, int amount)
    {
        var resource = _resources.Where(r => r.Type == type).First();
        resource.ChangeAmount(amount);
    }

    public int GetResourceAmount(ResourceType type)
    {
        return _resources.Where(r => r.Type == type).First().Amount;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    void TogglePause()
    {
        if (Time.timeScale == 0) Resume();
        else Pause();
    }

    private void Awake()
    {
        i = this;
    }


    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        AudioManager.i.Resume();
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        AudioManager.i.Pause();
    }

    [ButtonMethod]
    public void LoadMenu()
    {
        Resume();
        StartCoroutine(FadeThenLoadScene(0));
    }

    [ButtonMethod]
    public void EndGame()
    {
        Resume();
        StartCoroutine(FadeThenLoadScene(2));
    }

    IEnumerator FadeThenLoadScene(int num)
    {
        fade.Appear(); 
        music.FadeOutCurrent(fade.fadeTime);
        yield return new WaitForSeconds(fade.fadeTime + 0.5f);
        Destroy(AudioManager.i.gameObject);
        SceneManager.LoadScene(num);
    }

}
