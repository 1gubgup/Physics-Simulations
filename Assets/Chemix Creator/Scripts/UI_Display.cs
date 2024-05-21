using Chemix.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Display : MonoBehaviour {

    public GameObject caveCameras;
    public GameObject caveLookCamera;
    public GameObject mainCamera;
    public GameObject VRCameras;
    public GameObject VRLookCamera;

    public Vector3 camInitPos;

    private FormulaLabel[] labels;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Back()
    {
        caveLookCamera.SetActive(false);
        VRLookCamera.SetActive(false);
        mainCamera.SetActive(true);
        
        if (camInitPos.x != 0 || camInitPos.y != 0)
        {
            Chemix.ChemixEngine.Instance.mainCamera.transform.position = camInitPos;
            camInitPos = new Vector3();
        }

        foreach (var obj in labels)
        {
            obj.gameObject.SetActive(true);
        }
        labels = new FormulaLabel[0];
    }

    public void toCave()
    {
        VRLookCamera.SetActive(false);
        mainCamera.SetActive(false);
        caveLookCamera.SetActive(true);

        MoveAwayMainCamera();
    }

    public void toVR()
    {
        caveLookCamera.SetActive(false);
        mainCamera.SetActive(false);
        VRLookCamera.SetActive(true);

        MoveAwayMainCamera();
    }

    public void MoveAwayMainCamera()
    {
        if (camInitPos.x == 0 && camInitPos.y == 0)
        {
            camInitPos = Chemix.ChemixEngine.Instance.mainCamera.transform.position;
            Chemix.ChemixEngine.Instance.mainCamera.transform.position = new Vector3(1000, 1000, 1000);

            labels = FindObjectsOfType<FormulaLabel>();
            foreach (var obj in labels)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }
}
