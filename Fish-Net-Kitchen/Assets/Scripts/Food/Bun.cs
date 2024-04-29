using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bun : MonoBehaviour
{
    [SerializeField] private GameObject bottomBun;
    [SerializeField] private GameObject topBun;

    public GameObject GetBottomBun() => bottomBun;
    public GameObject GetTopBun() => topBun;
}
