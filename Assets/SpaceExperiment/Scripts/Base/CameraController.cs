using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player_transform;
    Transform camera_transform;
    Vector3 offset_player_look;
    Vector3 offset_player_follow;
    Vector3 offset_player_side;
    Vector3 offset_follow;
    Vector3 offset_side;
    string type;

    void Start()
    {
        camera_transform = this.transform;
        offset_player_look = new Vector3(0.0f, 15.0f, 5.0f);
        offset_player_follow = new Vector3(0.0f, 15.0f, 0.0f);
        offset_player_side = new Vector3(0.0f, 15.0f, 20.0f);
        offset_follow = new Vector3(0.0f, 20.0f, -40.0f);
        offset_side = new Vector3(40.0f, 20.0f, 0.0f);
        type = "look";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            switch (type)
            {
                case "look":
                    {
                        type = "side";
                        break;
                    }
                case "follow":
                    {
                        type = "look";
                        break;
                    }
                case "side":
                    {
                        type = "follow";
                        break;
                    }
            }
        }

        switch (type)
        {
            case "look":
                {
                    transform.position = player_transform.position + offset_player_look;
                    transform.rotation = player_transform.rotation;
                    break;
                }
            case "follow":
                {
                    transform.LookAt(player_transform.position + offset_player_follow);
                    transform.position = Vector3.Lerp(transform.position, player_transform.position + offset_follow, Time.deltaTime * 2);
                    break;
                }
            case "side":
                {
                    transform.LookAt(player_transform.position + offset_player_side);
                    transform.position = Vector3.Lerp(transform.position, player_transform.position + offset_side, Time.deltaTime * 2);
                    break;
                }
        }
    }
}
