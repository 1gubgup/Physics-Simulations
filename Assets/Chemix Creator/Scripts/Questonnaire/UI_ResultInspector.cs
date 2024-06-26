﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Questionnaire;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace UI
{
	public class UI_ResultInspector : MonoBehaviour
	{
		public UI_QuestionNumberList NumList;
		public Text CurrentNumber;
		public string NumberFormat = "第{0:D}题";
		public Text CurrentQuestionType;
		public string QuestionTypeFormat = "题型：{0}";
		public Text Content;
		public GameObject QuestionNumberButton;
		public Text PeopleCount;
		public string PeopleCountFormat = "人数：{0:D}人";
		public Text Data;

		int currentIdx = 0;
		List<string> datas = null;
		private Questionnaire.Questionnaire questionnaire = null;
		private List<AnswerSheet> answerSheets = null;

		// Use this for initialization
		void Start()
		{
			//gameObject.SetActive(false);
			Debug.Log("start");
			//Init();
		}

		public void Init()
		{
			if (questionnaire == null)
			{
				questionnaire = GM.GM_Core.instance.QuestionnaireMemo;
				if (questionnaire == null || questionnaire.Count() <= 0)
				{
					Debug.Log("Byebye");
					return;
				}
			}
			if (answerSheets == null)
			{
				answerSheets = new List<AnswerSheet>();
				WWWForm form = new WWWForm();
				form.AddField("account", GM.GM_Core.instance.Account);
				form.AddField("password", GM.GM_Core.instance.Password);
				form.AddField("invite", GM.GM_Core.instance.Invite);
				Chemix.Network.NetworkManager.Instance.PostList(form, "scene/getsubmits", (success, gameReply) => 
				{
					if (gameReply.Values.Length == 0)
                    {
						return;
                    }
					Debug.Log("Reply : " + gameReply.Values);

					foreach (string s in gameReply.Values)
					{
						answerSheets.Add(JsonUtility.FromJson<AnswerSheet>(s));
						Debug.Log(s + JsonUtility.FromJson<AnswerSheet>(s).answers[0].Result);
					}
					datas = new List<string>();
					for (int i = 0; i < questionnaire.Count(); i++)
					{
						AddQuestionUI(questionnaire[i], i);
						List<ValueAnswer> r = new List<ValueAnswer>();
						for (int j = 0; j < answerSheets.Count; j++)
						{
							r.Add(answerSheets[j][i]);
						}
						datas.Add(questionnaire[i].GetCustomData(r));
					}
					SelectNewQuestion(NumList[0]);
					PeopleCount.text = string.Format(PeopleCountFormat, answerSheets.Count);
				}
				                                               );
			}

			gameObject.SetActive(true);
		}

		// Update is called once per frame
		void Update()
		{
		
		}
		
		void AddQuestionUI(Question q, int index)
		{
			GameObject newNum = Instantiate(QuestionNumberButton);
			newNum.GetComponent<Button>().onClick.AddListener(() => SelectQuestion(newNum));
			newNum.GetComponent<UI_QuestionNumberListItem>().Index = index;
			NumList.Add(newNum);
		}
		
		void SetCurrentNumber(int index)
		{
			CurrentNumber.text = string.Format(NumberFormat, index + 1);
		}
		
		void SetCurrentQuestionType(QuestionType t)
		{
			CurrentQuestionType.text = string.Format(QuestionTypeFormat, t);
		}
		
		void SetQuestionContent(Question q)
		{

			Content.text = string.Format("{0}\n{1}", q.QuestionContent, q.GetCustomContent());
		}

		void SetData(int index)
		{
			Data.text = datas[index];
		}

		void SelectNewQuestion(GameObject item)
		{
			NumList[currentIdx].GetComponent<UI_QuestionNumberListItem>().OnNotSelected();
			currentIdx = item.GetComponent<UI_QuestionNumberListItem>().Index;
			item.GetComponent<UI_QuestionNumberListItem>().OnSelected();
			Question q = questionnaire[currentIdx];
			SetCurrentNumber(currentIdx);
			SetCurrentQuestionType(q.TypeName);
			SetQuestionContent(q);
			SetData(currentIdx);
		}
			
		void SelectQuestion(GameObject item)
		{
			SelectNewQuestion(item);
		}
			
		public void Leave()
        {
            GM.GM_Core.instance.setReturnButton(true);
            GM.GM_Core.instance.setPicturePanel(true);
            gameObject.SetActive(false);
		}

		public void ShowMyself()
        {
			Debug.Log("ShowMySelf");
            GM.GM_Core.instance.setPicturePanel(false);
            GM.GM_Core.instance.setReturnButton(false);
            Init();
		}
	}
}