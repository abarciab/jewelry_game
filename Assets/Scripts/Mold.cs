using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartType { None, Band, Setting };

[CreateAssetMenu(fileName = "Mold", menuName = "Mold")]
public class Mold: ScriptableObject
{
    public GameObject MoldPrefab;
    public int MoldCastCost;

    public Part CompletedPart = new Part();
}
