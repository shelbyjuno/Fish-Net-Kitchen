using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodModel : MonoBehaviour
{
    public Transform foodPivot;
    private GameObject foodModel;

    public void SetFoodModel(GameObject food)
    {
        if(foodModel != null) Destroy(foodModel);
        if(food == null) return;

        foodModel = Instantiate(food, foodPivot ? foodPivot : transform);
        foodModel.SetActive(true);
    }
}
