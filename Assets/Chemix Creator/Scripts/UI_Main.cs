using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace UI
{
    public class UI_Main : MonoBehaviour
    {
        private GM.GM_Core gm;
		public Animator wrongAnimator;
		public GameObject KeyPanel;
		public InputField Key;
        public List<GameObject> MainMenuBtns;
        public GameObject NotLoginPanel;
        public GameObject Welcome;
		bool ToEdit = true;

        [DllImport("__Internal")]
        private static extern void gotoLogin();
        [DllImport("__Internal")]
        private static extern void GetCookie();

        // Use this for initialization
        void Start()
        {
			gm = GM.GM_Core.instance;
            //KeyPanel.SetActive(false);
#if UNITY_WEBGL && !UNITY_EDITOR
            GetCookie();
            if (!gm.isLogin)
            {
                foreach(GameObject g in MainMenuBtns)
                {
                    g.GetComponent<Button>().interactable = false;
                    g.GetComponent<Michsky.UI.FieldCompleteMainMenu.UIElementSound>().enabled = false;
                    g.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 100 / 255f);
                    g.transform.GetChild(2).GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1, 100 / 255f);
                    g.transform.GetChild(2).GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, 100 / 255f);
                }
                NotLoginPanel.SetActive(true);
            }
#endif
        }

        void Update()
        {
            if (NotLoginPanel != null)
            {
                if (NotLoginPanel.activeSelf)
                {
                    if (gm.isLogin)
                    {
                        foreach (GameObject g in MainMenuBtns)
                        {
                            g.GetComponent<Button>().interactable = true;
                            g.GetComponent<Michsky.UI.FieldCompleteMainMenu.UIElementSound>().enabled = true;
                            g.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                            g.transform.GetChild(2).GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1, 1);
                            g.transform.GetChild(2).GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, 1);
                        }
                        NotLoginPanel.SetActive(false);
                    }
                }
                else if (!Welcome.activeSelf)
                {
                    Welcome.GetComponent<Text>().text = "欢迎使用，" + gm.Name + "!";
                    Welcome.SetActive(true);
                }
            }
        }

        public void GoToLogin()
        {
            gotoLogin();
        }

        public void CreateNewExperiment_OnClick()
        {
			//Debug.Log(gm.IsGuest + " " + gm.Account + gm.Password);
			/*if (gm.IsGuest)
			{
				wrongAnimator.Play("Notification In");
				return;
			}*/
            gm.SwitchToScene("BuildExperiment");
        }

		public void EditExperiment_OnClick()
		{
			/*if (gm.IsGuest)
			{
				wrongAnimator.Play("Notification In");
				return;
			}*/
			ToEdit = true;
			KeyPanel.SetActive(true);
		}

        public void Test_OnClick()
        {
			ToEdit = false;
			KeyPanel.SetActive(true);
            
        }

		public void SendKey()
		{
			string key = Chemix.InviteUtility.ParseInvite(Key.text).ToString();
			WWWForm form = new WWWForm();
			form.AddField("invite", key);
			Chemix.Network.NetworkManager.Instance.Post(form, "scene/invite",
														(success, reply) =>
			{
				if (success)
				{
					gm.Invite = key;
					gm.experimentalSetup = JsonUtility.FromJson<Chemix.GameManager.ExperimentalSetup>(reply.Detail);
					gm.QuestionnaireMemo = gm.experimentalSetup.questionnaire;
					Debug.Log("Invite Number: " + gm.Invite);

					if (ToEdit)
					{
                        GM.GM_Core.instance.used = true;
						gm.SwitchToScene("BuildExperiment");
					}
					else
					{ 
						gm.SwitchToScene("CustomLab");
					}
				}
				else
				{
					wrongAnimator.Play("Notification In");
				}
			}
													   );
		}

		public void Leave()
		{ 
			KeyPanel.SetActive(false);
		}

        // Change to the edit platform
        public void EditPlatform_OnClick()
        {
            //Debug.Log(gm.IsGuest + " " + gm.Account + gm.Password);
            /*if (gm.IsGuest)
            {
                wrongAnimator.Play("Notification In");
                return;
            }*/
            gm.SwitchToScene("EditPlatformScene");
        }
    }
}
