using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class ImportFile : MonoBehaviour
{
    //与jslib的通信函数，详见Plugins文件夹下的.jslib文件
    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern void ClickSelectFileBtn();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnBtnImportClick()
    {
        ClickSelectFileBtn();
    }
}
