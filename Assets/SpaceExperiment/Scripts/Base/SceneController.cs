using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public string sceneFrom;
    public string sceneTo;

    public void toScene()
    {
        SceneManager.LoadScene(sceneTo);
    }

}
