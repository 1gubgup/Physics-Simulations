using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Button : MonoBehaviour {
    public GameObject stepPanel;
    public GameObject objectPanel;
    public GameObject settingsPanel;
    public GameObject completeButton;
    public GameObject systemUI;
    public GameObject coverUI;
    public GameObject UIcontroltext;
    public GameObject nopermissionPanel;
    public GameObject reportwaitPanel;
    public GameObject reportsuccessPanel;
    public GameObject reportfailPanel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenObjectMenu()
    {
        Camera.main.gameObject.GetComponent<Lab.Lab_Controller>().enabled = false;
        objectPanel.GetComponent<UI.UI_List>().ClickOnRootButton();
    }

    public void OpenSettingsMenu()
    {
        Camera.main.gameObject.GetComponent<Lab.Lab_Controller>().enabled = false;
        settingsPanel.GetComponent<UI.UI_Menu>().MoveAndChangeColor();
        GM.GM_Core.instance.setPicturePanel(!GM.GM_Core.instance.picturePanel.activeSelf);
    }

    public void FinishEditing()
    {
        if (GM.GM_Core.instance.GuestReject && GM.GM_Core.instance.IsGuest)
        {
            nopermissionPanel.SetActive(true);
        }
        else
        {
            Camera.main.gameObject.GetComponent<Lab.Lab_Controller>().enabled = false;
            completeButton.GetComponent<UI_Edit>().Complete();
        }
    }

    public void CloseNoPermissionMenu()
    {
        nopermissionPanel.SetActive(false);
    }

    public void CloseSuccess()
    {
        reportsuccessPanel.SetActive(false);
    }

    public void CloseFail()
    {
        reportfailPanel.SetActive(false);
    }

    public void OpenStepMenu()
    {
        stepPanel.SetActive(true);
        GM.GM_Core.instance.setReturnButton(false);
        GM.GM_Core.instance.setPicturePanel(false);
        Camera.main.gameObject.GetComponent<Lab.Lab_Controller>().enabled = false;
    }

    public void CoverUI()
    {
        if (systemUI.activeSelf)
        {
            systemUI.SetActive(false);
            GM.GM_Core.instance.setPicturePanel(false);
            coverUI.SetActive(true);
        }
        else
        {
            systemUI.SetActive(true);
            GM.GM_Core.instance.setPicturePanel(true);
            coverUI.SetActive(false);
        }
    }

    public void ReportComplete(bool value)
    {
        reportwaitPanel.SetActive(false);
        if (value)
        {
            reportsuccessPanel.SetActive(true);
        }
        else
        {
            reportfailPanel.SetActive(true);
        }
    }
}
