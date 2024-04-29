using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject rawModel;
    [SerializeField] private GameObject cookedModel;
    [SerializeField] private Food cookedFood;

    [SerializeField] private float minCookTime = 5;
    [SerializeField] private float maxCookTime = 10;

    public float GetMinCookTime() => minCookTime;
    public float GetMaxCookTime() => maxCookTime;
    
    public GameObject GetRawModel() => rawModel;
    public GameObject GetCookedModel() => cookedModel;

    public Food GetCookedFood() => cookedFood;
}
