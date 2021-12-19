using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    public static GameObject CreateInstance(GameObject original, GameObject parent, bool isActive)
    {
        GameObject instance = Instantiate(original, parent.transform);
        instance.SetActive(isActive);
        return instance;
    }
}
