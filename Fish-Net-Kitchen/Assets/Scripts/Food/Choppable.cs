using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Choppable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject normalModel;
    [SerializeField] private GameObject partiallyChoppedModel;
    [SerializeField] private GameObject fullyChoppedModel;

    [Header("Chopping Settings")]
    [SerializeField] private Food choppedFood;
    [SerializeField] private int minChop = 3;
    [SerializeField] private int maxChop = 5;

    public int GetMinChop() => minChop;
    public int GetMaxChop() => maxChop;

    public GameObject GetNormalModel() => normalModel;
    public GameObject GetPartiallyChoppedModel() => partiallyChoppedModel;
    public GameObject GetFullyChoppedModel() => fullyChoppedModel;
    public Food GetChoppedFood() => choppedFood;
}
