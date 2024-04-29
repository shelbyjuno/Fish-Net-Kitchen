using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Food : NetworkBehaviour
{
    enum FoodColor { Red, Green, Orange, Yellow, Blue, Purple, Pink, Brown, Black, White }

    [Header("Food Settings")]
    [SerializeField] private string foodName;
    [SerializeField] private FoodColor foodColor = FoodColor.White;
    [SerializeField] private Vector3 foodSize = new Vector3(0.0f, 0.1f, 0.0f);

    [Header("References")]
    [SerializeField] private GameObject defaultModel;
    [SerializeField] private Plateable plateable;
    [SerializeField] private Choppable choppable;
    [SerializeField] private Cookable cookable;

    [ExecuteInEditMode]
    private void OnEnable()
    {
        TryGetComponent(out Choppable choppable);
        TryGetComponent(out Cookable cookable);
    }

    public GameObject GetDefaultModel() => defaultModel;
    
    public Plateable GetPlateable() => plateable;
    public bool IsPlateable() => plateable != null;
    
    public Choppable GetChoppable() => choppable;
    public bool IsChoppable() => choppable != null;

    public Cookable GetCookable() => cookable;
    public bool IsCookable() => cookable != null;

    public Vector3 GetSize() => foodSize;

    public string GetName() => foodName;
    
    public string GetColoredName()
    {
        string color = "white";

        switch(foodColor)
        {
            case FoodColor.Red: color = "#FF0000"; break;
            case FoodColor.Green: color = "#00FF00"; break;
            case FoodColor.Orange: color = "#FFA500"; break;
            case FoodColor.Yellow: color = "#FFFF00"; break;
            case FoodColor.Blue: color = "#0000FF"; break;
            case FoodColor.Purple: color = "#800080"; break;
            case FoodColor.Pink: color = "#FFC0CB"; break;
            case FoodColor.Brown: color = "#A52A2A"; break;
            case FoodColor.Black: color = "#000000"; break;
            case FoodColor.White: color = "#FFFFFF"; break;
        }

        return $"<color={color}>{foodName}</color>";
    }
}
