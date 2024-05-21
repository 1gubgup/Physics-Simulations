using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectPage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadExperiment()
    {
        GM.GM_Core.instance.used = false;
        GM.GM_Core.instance.SwitchToScene("EditPlatformScene");
    }
}
