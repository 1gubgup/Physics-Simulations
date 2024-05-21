using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Chemix.Network;
using static Chemix.GameManager;
using Chemix;
using System.Runtime.InteropServices;

namespace GM
{
    public class PictureInfo
    {
        public string Name
        {
            get;
            set;
        }

        public string Des
        {
            get;
            set;
        }

        public byte[] Byte
        {
            get;
            set;
        }
    };

    public enum ExpType
    {
        EditPlatform,
        BuildScene,
    }

    public class GM_Core : MonoBehaviour
    {

        public static GM_Core instance = null;

        public GameObject mainmenuButton;
        public GameObject catchscreenButton;
        public GameObject picturePanel;
        public GameObject scrollContent;
        public GameObject picPrefab;
        public GameObject mainMenu;
        public GameObject experimentMenu;

        public string testAccount = "a";
        public string testPassword = "b";

        public bool used = false;
        public bool isLogin = false;

        public string curSceneName = "Main";
        private GM_Settings settings;
        private UI_Account accountManager;
        private string picfold = "screens";
        private int picID = 1;

        public bool GuestReject = false;
		public bool IsGuest = false;
        public string Name = "guest";
		public string Account = "guest";
		public string Password = "guest";
		public string Invite = "";

        //private string ReportServiceHostURL = "localhost:8081";
        private string ReportServiceHostURL = "https://ilabs.sjtu.edu.cn/lesson9";
        private string ReportSubURL = "report/uploadReport";

        //private string ExpServiceHostURL = "localhost:8083";
        private string ExpServiceHostURL = "https://ilabs.sjtu.edu.cn/lesson9";
        private string SaveExpSubURL = "experiment/experiment/saveExperiment";
        private string LoadExpSubURL = "experiment/experiment/getExperiment";

        public Questionnaire.Questionnaire QuestionnaireMemo;

        public Dictionary<string, bool> eventDic;
        public Dictionary<string, int> eventID;
        public List<string> eventIdList;
        public List<Dropdown.OptionData> options;
        public Dictionary<int, PictureInfo> pictures;
        //public List<string> substanceType = new List<string> { "空" };

        public List<int> selectIDs;
        public List<PictureInfo> selectPics;

        //public System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();//测试执行时间

        #region PhysicsExtension
        public string sceneString;
        public ExperimentalScene experimentalScene
        {
            get;
            set;
        } = new ExperimentalScene();
        #endregion

        [DllImport("__Internal")]
        private static extern void StartExperiment(string str);
        void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);

            //DontDestroyOnLoad(gameObject);
            InitGame();
        }

        void InitGame()
        {
        }

        // Use this for initialization
        void Start()
        {
            mainMenu.SetActive(true);
            experimentMenu.SetActive(false);
            //Debug.Log("GM_CORE: start");
            eventDic = new Dictionary<string, bool>();
            eventID = new Dictionary<string, int>();
            eventIdList = new List<string>();
            pictures = new Dictionary<int, PictureInfo>();
            selectIDs = new List<int>();
            selectPics = new List<PictureInfo>();
            List<TaskFlow.EventInfo> eventInfos = TaskFlow.GetAllEventInfos();
            int i = 0;
            foreach (TaskFlow.EventInfo ei in eventInfos)
            {
                eventDic.Add(ei.chineseName, ei.eventOrCondition);
                eventID.Add(ei.chineseName, i);
                eventIdList.Add(ei.chineseName);
                i++;
            }

            List<string> substanceType = new List<string> { "空" };
            List<GameManager.FormulaInfo> formulaInfos = GameManager.GetAllFormula();
            foreach (GameManager.FormulaInfo info in formulaInfos)
            {
                substanceType.Add(info.name);
                //Debug.Log(info.name);
            }

            Directory.CreateDirectory(picfold);

            options = new List<Dropdown.OptionData>();
            foreach (string t in substanceType)
            {
                //Debug.Log(t.ToString());
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = t.ToString();
                options.Add(option);
            }

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (catchscreenButton.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    catchscreenButton.GetComponent<Button>().onClick.Invoke();
                }
            }
#endif
        }

        private void OnDestroy()
        {
            Directory.Delete(picfold, true);
        }

        public void beginExperiment()
        {
            StartExperiment("1");
        }
        public void closeExperiment()
        {
            StartExperiment("0");
        }
        public void isQuit()
        {
            Controller controller = GameObject.Find("Controller").GetComponent<Controller>();
            controller.isQuitUI.SetActive(true);
        }

        public void toBackScene()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            closeExperiment();
#endif
            //unload
            if (SceneManager.GetSceneByName(curSceneName).isLoaded)
                SceneManager.UnloadSceneAsync(curSceneName);
            
            mainmenuButton.SetActive(false);
            catchscreenButton.SetActive(false);
            picturePanel.SetActive(false);
            mainMenu.SetActive(true);
            experimentMenu.SetActive(false);
        }
        public void SwitchToScene(string sceneName)
        {
            //unload
            if (SceneManager.GetSceneByName(curSceneName).isLoaded)
                SceneManager.UnloadSceneAsync(curSceneName);

            //load async
            StartCoroutine(LoadSceneJob(sceneName));
            curSceneName = sceneName;

            if (sceneName == "LoginScene" || sceneName == "SignupScene" || sceneName == "MainMenu")
            {
                mainmenuButton.SetActive(false);
                catchscreenButton.SetActive(false);
                picturePanel.SetActive(false);
                mainMenu.SetActive(true);
                experimentMenu.SetActive(false);
            }
            else
            {
                mainmenuButton.SetActive(true);
                catchscreenButton.SetActive(true);
                picturePanel.SetActive(true);
                mainMenu.SetActive(false);
                experimentMenu.SetActive(true);
                if (sceneName == "CustomLab" && GuestReject && IsGuest)
                {
                    //catchscreenButton.SetActive(false);
                    //picturePanel.SetActive(false);
                }
            }
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        IEnumerator LoadSceneJob(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            //update lightings
            if (GameObject.Find("GM_Settings") != null)
            {
                settings = GameObject.Find("GM_Settings").GetComponent<GM.GM_Settings>();
                RenderSettings.skybox = settings.skybox;
            }

        }

        public void CaptureScreenByUnity()
        {
            string pictureName = "screen" + (picID) + ".jpg";
            string pictureDes = "未添加描述";
            PictureInfo newPic = new PictureInfo();
            newPic.Name = pictureName;
            newPic.Des = pictureDes;
            pictures.Add(picID, newPic);
            ScreenCapture.CaptureScreenshot(picfold + "/" + pictureName);
            StartCoroutine(Addpic(picfold + "/" + pictureName, pictureDes, picID));
            picID++;
        }

        IEnumerator Addpic(string imgPath,string description,int picID)
        {
            while (!File.Exists(imgPath)) { yield return new WaitForSeconds(0.1f); }
            GameObject p = Instantiate(picPrefab, scrollContent.transform);
            byte[] imgByte = LoadImage.SetImageToByte(imgPath);
            Texture2D tex = LoadImage.GetTextureByByte(imgByte);
            p.GetComponentInChildren<RawImage>().texture = tex;
            pictures[picID].Byte = imgByte;
            p.transform.Find("ID").GetComponent<Text>().text = picID.ToString();
            Debug.Log("capture successfully");
        }

        public void UpdatePicDes(int id,string des)
        {
            //Debug.Log(id + " " + des);
            pictures[id].Des = des;
        }

        public void ReportOpenPic()
        {
            picturePanel.GetComponent<ObjectPartController>().pick();
            foreach (Transform t in scrollContent.transform)
            {
                t.Find("Toggle").gameObject.SetActive(true);
            }
        }

        public void SelectPic(int id, bool value)
        {
            if (value) selectIDs.Add(id);
            else selectIDs.Remove(id);
        }

        public void UploadPic()
        {
            foreach (Transform t in scrollContent.transform)
            {
                t.Find("Toggle").gameObject.SetActive(false);
            }
            for(int i = 0; i < selectIDs.Count; ++i)
            {
                selectPics.Add(pictures[selectIDs[i]]);
            }
            //Debug.Log(selectIDs.Count);
            //Debug.Log(selectPics.Count);
            GameObject.Find("ReportPanel").GetComponent<ReportController>().SelectComplete();
            selectPics.Clear();
        }

        public void deletePic(int id)
        {
            Debug.Log(id);
            if (selectIDs.Contains(id)) selectIDs.Remove(id);
            pictures.Remove(id);
            //没有在文件夹中删除图片，可以删掉，必要性不大，觉得不删更好。
            //如果确定要删除的话注意一下，可能会有个隐性bug：在实验报告里上传的图片被删除应不应该一同消失
        }

        public bool Login(string un, string pwd, UI_Account acc)
        {
            WWWForm form = new WWWForm();
            form.AddField("account", un);
            form.AddField("password", pwd);
            accountManager = acc;
            NetworkManager.Instance.Post(form, "login", OnLoginComplete);
            return true;
        }

        public void OnLoginComplete(bool success, Reply reply)
        {
            if (success)
            {
                Debug.LogFormat("Login success! {0}", reply.Detail);
				IsGuest = false;
                isLogin = true;
				Account = accountManager.account;
				Password = accountManager.password;
                SwitchToScene("MainMenu");
            }
            else
            {
                accountManager.loginFallback(false);
                Debug.Log("Login failed!");
            }
        }

        public void Signup(string un, string pwd, string email, UI_Account acc)
        {
            WWWForm form = new WWWForm();
            form.AddField("account", un);
            form.AddField("password", pwd);
            form.AddField("email", email);
            accountManager = acc;
            NetworkManager.Instance.Post(form, "signup", OnSignupComplete);
            //SwitchToScene("LoginScene");
        }

        public void OnSignupComplete(bool success, Reply reply)
        {
            if (success)
            {
                Debug.LogFormat("Signup success! {0}", reply.Detail);
                SwitchToScene("LoginScene");
            }
            else
            {
                accountManager.signupFallback(false);
                Debug.Log("Signup failed!");
            }
        }

        public void Report(Report report)
        {
            WWWForm form = new WWWForm();
            form.AddField("sid", Account);
            form.AddField("Title", report.Title);
            form.AddField("Purpose", report.Purpose);
            form.AddField("PictureNum", report.Num);
            form.AddField("PictureList", report.listJson);
            form.AddField("GradeJson", report.scoreJson);

            // report type
            if(curSceneName == "EditPlatformScene")
            {
                form.AddField("type", ((int)ExpType.EditPlatform));
            }
            else if(curSceneName == "BuildExperiment")
            {
                form.AddField("type", ((int)ExpType.BuildScene));
            }

            NetworkManager.Instance.Post(form, ReportServiceHostURL, ReportSubURL, OnReportComplete, true);
        }

        public void OnReportComplete(bool success, Reply reply)
        {
            if (curSceneName == "EditPlatformScene")
            {
                Controller controller = GameObject.Find("Controller").GetComponent<Controller>();
                controller.ReportComplete();
                if (success)
                {
                    Debug.LogFormat("report success! {0}", reply.Detail);
                    controller.SetException("实验报告提交成功.");
                }
                else
                {
                    Debug.LogFormat("report failed! {0}", reply.Detail);
                    controller.SetException("实验报告提交失败. 请联系管理员确定原因.");
                }
            }
            else if (curSceneName == "BuildExperiment")
            {
                UI_Button controller = GameObject.Find("Controller").GetComponent<UI_Button>();
                controller.ReportComplete(success);
            }
        }

        public void SaveScene(string scene, bool isQuiting)
        {
            WWWForm form = new WWWForm();
            form.AddField("sid", Account);
            form.AddField("scene", scene);
            if (curSceneName == "EditPlatformScene")
            {
                form.AddField("type", ((int)ExpType.EditPlatform));
            }
            else if (curSceneName == "BuildExperiment")
            {
                form.AddField("type", ((int)ExpType.BuildScene));
            }
            if (isQuiting)
            {
                NetworkManager.Instance.Post(form, ExpServiceHostURL, SaveExpSubURL, OnSaveAndQuitSceneComplete, true);
            }
            else
            {
                NetworkManager.Instance.Post(form, ExpServiceHostURL, SaveExpSubURL, OnSaveSceneComplete, true);
            }
            
        }

        public void OnSaveAndQuitSceneComplete(bool success, Reply reply)
        {
            Controller controller = GameObject.Find("Controller").GetComponent<Controller>();
            controller.ReportComplete();
            if (success)
            {
                //controller.SetException("实验保存成功.");
                toBackScene();
            }
            else
            {
                controller.SetException("实验保存失败. 请联系管理员确定原因.");
            }
        }

        public void OnSaveSceneComplete(bool success, Reply reply)
        {
            Controller controller = GameObject.Find("Controller").GetComponent<Controller>();
            controller.ReportComplete();
            if (success)
            {
                controller.SetException("实验保存成功.");
                //toBackScene();
            }
            else
            {
                controller.SetException("实验保存失败. 请联系管理员确定原因.");
            }
        }

        public void LoadScene()
        {
            WWWForm form = new WWWForm();
            form.AddField("sid", Account);
            // this project only has edit platform experiment
            // if has more than one experiments, need to check the getting type
            form.AddField("type", ((int)ExpType.EditPlatform));
            
            NetworkManager.Instance.Post(form, ExpServiceHostURL, LoadExpSubURL, OnLoadSceneComplete, true);
        }

        public void OnLoadSceneComplete(bool success, Reply reply)
        {
            string scene = reply.Detail;
            string key = scene.Substring(0, 6);
            scene = scene.Substring(6);
            if (key == "scene:" && scene!="")
            {
                sceneString = scene;
                instance.used = true;
                instance.SwitchToScene("EditPlatformScene");
            }
            else
            {
                loadNewExperiment();
            }
        }

        public void setReturnButton(bool input)
        {
            mainmenuButton.SetActive(input);
        }

        public void setPicturePanel(bool input)
        {
            picturePanel.SetActive(input);
        }

        public void DealwithCookie(string data)
        {
            int i = data.IndexOf('~');
            string name = data.Substring(0, i);
            string id = data.Substring(i + 1);
            Name = name;
            Account = id;
            Debug.Log(Name + " " + id);
            isLogin = true;
        }

        public void UpdateUserInfoByReact(string id)
        {
            if (id == "guest")
            {
                Account = id;
                isLogin = false;
            }
            else
            {
                Account = id;
                isLogin = true;
            }
            //Name = name;
            //Account = id;
            //Debug.Log(Name + " " + id); 
        }

        public void loadNewExperiment()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            beginExperiment();
#endif
            instance.used = false;
            instance.SwitchToScene("EditPlatformScene");
        }
        public void loadSavedExperiment()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            beginExperiment();
#endif
            LoadScene();
            //instance.used = true;
            //instance.SwitchToScene("EditPlatformScene");
        }

        public void SetKeyboardInput(string flag)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if(flag=="0")
                WebGLInput.captureAllKeyboardInput = false;
            else
                WebGLInput.captureAllKeyboardInput = true;
#endif
        }

#region ChemixExtension
        public ExperimentalSetup experimentalSetup
        {
            get;
            set;
        } = new ExperimentalSetup();

        public InstrumentsListAsset instrumentListAsset
        {
            get { return m_InstrumentListAsset; }
        }

        [SerializeField]
        private InstrumentsListAsset m_InstrumentListAsset;
#endregion
    }
}
