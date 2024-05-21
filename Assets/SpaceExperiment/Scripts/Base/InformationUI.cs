using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationUI : MonoBehaviour
{
    public GameObject player;
    public Text info;

    private int count;
    private float deltaTime;

    private void Update()
    {
        count++;
        deltaTime += Time.deltaTime;

        if (deltaTime >= 0.3f)
        {
            count = 0;
            deltaTime = 0;

            Vector3 pos = player.transform.position;
            Vector3 euler = player.transform.rotation.eulerAngles;
            if (euler.x > 180)
            {
                euler.x -= 360;
            }
            if (euler.y > 180)
            {
                euler.y -= 360;
            }
            if (euler.z > 180)
            {
                euler.z -= 360;
            }
            float speed = player.GetComponent<Rigidbody>().velocity.magnitude;
            info.text =       
                  "Position: " + pos + "\n"
                + "Rotation: " + euler + "\n"
                + "Speed: " + Mathf.Ceil(speed) + "\n";
        }
    }
}
