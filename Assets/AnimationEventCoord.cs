using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCoord : MonoBehaviour
{
    [SerializeField] private int numParticles;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private UIManager _uiManager;

    public void EmitParticles()
    {
        _particleSystem.Emit(numParticles);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void DisplayNewForgedItem()
    {
        _uiManager.DisplayNewForgedItem();
    }

}
