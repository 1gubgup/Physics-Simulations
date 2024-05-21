using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideClose : MonoBehaviour
{
    public GameObject Guide;
    public GameObject Info;
    public GameObject Instru;

    public GameObject thisObject;

    void Update()
    {
        if (Guide.activeSelf || Info.activeSelf || Instru.activeSelf)
        {
            thisObject.SetActive(false);
        }
    }
}
