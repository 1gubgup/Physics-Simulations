using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Toogle : MonoBehaviour
{
    Toggle toogle;
    ColorBlock cb = new ColorBlock();
    // Start is called before the first frame update
    void Start()
    {
        toogle = this.gameObject.GetComponent<Toggle>();
        cb = toogle.colors;
    }

    // Update is called once per frame
    void Update()
    {
        if (toogle.isOn)
        {
            cb.normalColor = Color.blue;
            cb.selectedColor = Color.blue;
            toogle.colors = cb;
        }
        else
        {
            cb.normalColor = Color.white;
            cb.selectedColor = Color.white;
            toogle.colors = cb;
        }
    }
}
