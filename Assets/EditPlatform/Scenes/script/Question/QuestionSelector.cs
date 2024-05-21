using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionSelector : MonoBehaviour
{
    public List<GameObject> questions;
    public List<Button> buttons;

    private int currentQuestion = 0;
    // Start is called before the first frame update
    void Start()
    {
        // add listener for toggles
        int len = buttons.Count;
        for (int i = 0; i < len; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(delegate ()
            {
                onButtonClick(index);
            });
        }
        questions[currentQuestion].SetActive(true);
        buttons[currentQuestion].interactable = false;
    }
    
    private void onButtonClick(int index)
    {
        questions[currentQuestion].SetActive(false);
        buttons[currentQuestion].interactable = true;
        questions[index].SetActive(true);
        buttons[index].interactable = false;
        currentQuestion = index;
        
    }
}
