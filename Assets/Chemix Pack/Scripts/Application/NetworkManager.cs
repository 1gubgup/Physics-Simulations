using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Chemix.Network
{
    public class Reply
    {
        public bool Success;
        public string Detail;
    }

    public class ListReply
    {
        public bool Success;
        public string[] Values;
    }

    public class NetworkManager : Singleton<NetworkManager>
    {
        public delegate void OnReply(bool success, Reply reply);
        public delegate void OnListReply(bool success, ListReply gameReply);

        [System.Serializable]
        public class SerialClass
        {
            public Vector3 v3;
            public float f;
            public string s;
            public int i;
            public List<string> ss;
        }

        public void Ping()
        {
            Debug.Log("NetworkManager: try ping...");
            StartCoroutine(PingRequest());
        }

        IEnumerator PingRequest()
        {
            UnityWebRequest uwr = UnityWebRequest.Get(hosturl + "/");
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError)
            {
                Debug.Log("NetworkManager/Fail: " + uwr.error);
            }
            else
            {
                Debug.Log("NetworkManager/Success: " + uwr.downloadHandler.text);
            }
        }

        public void Post(WWWForm form, string suburl, OnReply onReply)
        {
            Debug.LogFormat("POST/{0}: {1}", suburl, form.data);
            StartCoroutine(PostRequest(form, suburl, onReply));
        }

        public void Post(WWWForm form, string hosturl, string suburl, OnReply onReply, bool hasHost = true)
        {
            Debug.LogFormat("POST/{0}: {1}", hosturl, form.data);
            StartCoroutine(PostRequest(form, hosturl, suburl, onReply, true));
        }

        public void PostList(WWWForm form, string suburl, OnListReply onListReply)
        {
            StartCoroutine(PostListRequest(form, suburl, onListReply));
        }

        IEnumerator PostListRequest(WWWForm form, string suburl, OnListReply onListReply)
        {
            UnityWebRequest uwr = UnityWebRequest.Post(hosturl + "/" + suburl, form);
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError)
            {
                Debug.LogFormat("POST/{0}: Error. {1}", suburl, uwr.error);
                if (onListReply != null)
                {
                    onListReply.Invoke(false, new ListReply());
                }
            }
            else
            {
                Debug.LogFormat("POST/{0}: {1}", suburl, uwr.downloadHandler.text);
                var reply = JsonUtility.FromJson<ListReply>(uwr.downloadHandler.text);
                if (onListReply != null)
                {
                    if (reply.Success)
                    {
                        onListReply.Invoke(true, reply);
                    }
                    else
                    {
                        onListReply.Invoke(false, reply);
                    }
                }
            }
        }

        IEnumerator PostRequest(WWWForm form, string suburl, OnReply onReply)
        {
            UnityWebRequest uwr = UnityWebRequest.Post(hosturl + "/" + suburl, form);
            yield return uwr.SendWebRequest();
            
            if (uwr.isNetworkError)
            {
                Debug.LogFormat("POST/{0}: Error. {1}", suburl, uwr.error);
                if (onReply != null)
                {
                    onReply.Invoke(false, new Reply());
                }
            }
            else
            {
                Debug.LogFormat("POST/{0}: {1}", suburl, uwr.downloadHandler.text);
                var reply = JsonUtility.FromJson<Reply>(uwr.downloadHandler.text);
                if (onReply != null)
                {
                    if (reply.Success)
                    {
                        onReply.Invoke(true, reply);
                    }
                    else
                    {
                        onReply.Invoke(false, reply);
                    }
                }
            }
        }

        //全域名POST请求，目前专用于课程网站实验报告
        IEnumerator PostRequest(WWWForm form, string host, string suburl,OnReply onReply, bool isHost = true)
        {
            UnityWebRequest uwr = UnityWebRequest.Post(host + "/" + suburl, form);
            uwr.timeout = 20;
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError)
            {
                Debug.LogFormat("POST/{0}: Error. {1}", suburl, uwr.error);
                if (onReply != null)
                {
                    onReply.Invoke(false, new Reply());
                }
            }
            else
            {
                Debug.LogFormat("POST/{0}: {1}", suburl, uwr.downloadHandler.text);
                //课程网站那边的成功响应
                Reply reply = new Reply();
                reply.Detail = uwr.downloadHandler.text;
                if (reply.Detail == "[\"success\"]") reply.Success = true;
                else reply.Success = false;
                if (onReply != null)
                {
                    if (reply.Success)
                    {
                        onReply.Invoke(true, reply);
                    }
                    else
                    {
                        onReply.Invoke(false, reply);
                    }
                }
            }
        }

        // test
        public void Login()
        {
            WWWForm form = new WWWForm();
            form.AddField("account", account);
            form.AddField("password", password);
            Post(form, "login", null);
        }

        public void Signup()
        {
            WWWForm form = new WWWForm();
            form.AddField("account", account);
            form.AddField("password", password);
            form.AddField("email", email);
            Post(form, "signup", null);
        }

        public void SaveData()
        {
            WWWForm form = new WWWForm();
            form.AddField("account", account);
            form.AddField("password", password);
            form.AddField("key", key);
            form.AddField("value", value);
            StartCoroutine(PostRequest(form, "scene/save", null));
        }

        public void SaveScene()
        {
            WWWForm form = new WWWForm();
            form.AddField("account", account);
            form.AddField("password", password);
            form.AddField("key", "scene");
            form.AddField("value", JsonUtility.ToJson(setupReply));
            StartCoroutine(PostRequest(form, "scene/save", OnSaveSuccess));
        }

        public void LoadData()
        {
            WWWForm form = new WWWForm();
            form.AddField("account", account);
            form.AddField("password", password);
            form.AddField("key", key);
            StartCoroutine(PostRequest(form, "scene/load", null));
        }

        public void GetScene()
        {
            WWWForm form = new WWWForm();
            form.AddField("invite", invite);
            StartCoroutine(PostRequest(form, "scene/invite", OnInviteSuccess));
        }

        public void Submit()
        {
            WWWForm form = new WWWForm();
            form.AddField("invite", invite);
            form.AddField("value", value);
            StartCoroutine(PostListRequest(form, "scene/submit", null));
        }

        public void GetSubmits()
        {
            WWWForm form = new WWWForm();
            form.AddField("account", account);
            form.AddField("password", password);
            form.AddField("invite", invite);
            StartCoroutine(PostListRequest(form, "scene/getsubmits", null));
        }

        public void TestInterface()
        {
            formulaInfos = GameManager.GetAllFormula();
            eventInfos = TaskFlow.GetAllEventInfos();
        }

        public void TestSerialization()
        {
            StartCoroutine(TestSerial());
        }

        IEnumerator TestSerial()
        {
            // save
            WWWForm form = new WWWForm();
            form.AddField("account", account);
            form.AddField("password", password);
            form.AddField("key", key);
            Debug.Log("NM: Serialize");
            form.AddField("value", JsonUtility.ToJson(setup)); // eventInfos
            Debug.Log("NM: Send json");
            yield return PostRequest(form, "scene/save", OnSaveSuccess);
            // invite
            form = new WWWForm();
            form.AddField("invite", invite);
            Debug.Log("NM: Try get json");
            yield return PostRequest(form, "scene/invite", OnInviteSuccess);
        }

        public void SetDefaultKey()
        {
            WWWForm form = new WWWForm();
            form.AddField("account", "a");
            form.AddField("password", "a");
            form.AddField("key", "default");
            form.AddField("value", value);
            StartCoroutine(PostRequest(form, "scene/save", null));
        }

        public void GetDefaultKey()
        {
            WWWForm form = new WWWForm();
            form.AddField("account", "a");
            form.AddField("password", "a");
            form.AddField("key", "default");
            StartCoroutine(PostListRequest(form, "scene/load", OnLoadDefaultKey));
        }

        public void EncryptInvite()
        {
            invite = InviteUtility.InviteFrom(int.Parse(invite));
        }

        void OnLoadDefaultKey(bool success, ListReply reply)
        {
            if (success)
            {
                WWWForm form = new WWWForm();
                form.AddField("invite", reply.Values[0]);
                Debug.Log(reply.Values[0]);
                StartCoroutine(PostRequest(form, "scene/invite", null));
            }
        }

        void OnSaveSuccess(bool success, Reply reply)
        {
            invite = reply.Detail;
        }

        void OnInviteSuccess(bool success, Reply reply)
        {
            setupReply = JsonUtility.FromJson<GameManager.ExperimentalSetup>(reply.Detail);
        }

        [SerializeField]
        private string hosturl = "hailvital.com";

        [Header("User")]
        [SerializeField]
        private string account;
        [SerializeField]
        private string password;
        [SerializeField]
        private string email;

        [Header("SaveLoad")]
        [SerializeField]
        private string key;
        [SerializeField]
        private string value;
        [SerializeField]
        private string invite;

        [Header("Test Interface")]
        [SerializeField]
        private List<GameManager.FormulaInfo> formulaInfos;
        [SerializeField]
        private List<TaskFlow.EventInfo> eventInfos;

        [Header("Test Scene")]
        [SerializeField]
        private GameManager.ExperimentalSetup setup;
        [SerializeField]
        private GameManager.ExperimentalSetup setupReply;
    }
}