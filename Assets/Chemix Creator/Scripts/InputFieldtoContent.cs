using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldtoContent : MonoBehaviour
{
    private static readonly string no_breaking_space = "\u00A0";

    InputField inputField;
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<InputField>();
        text = transform.parent.GetComponent<Text>();
        inputField.onValueChanged.AddListener((value) =>
        {
            inputField.text = inputField.text.Replace(" ", no_breaking_space);
            text.text = inputField.text;
        });
    }
}
