using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plateable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject plateModel;

    public GameObject GetPlateModel() => plateModel ? plateModel : GetComponent<Food>().GetDefaultModel();
}
