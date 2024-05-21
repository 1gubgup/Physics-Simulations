using GM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Substance : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (GM.GM_Core.instance == null)
			return;
        this.GetComponentInChildren<Dropdown>().options = GM.GM_Core.instance.options;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
