using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchController : MonoBehaviour
{
    public GameObject Prefab;
    private GameObject Torch;
    private Transform astronaut_transform;
    private Vector3 offset;
    private Vector3 rotation;
    private bool open;

    void Start()
    {
        astronaut_transform = GameObject.Find("Astronaut").GetComponent<Transform>();
        offset = new Vector3(0.0f, 7.0f, 80.0f);
        rotation = new Vector3(-87.0f, 0.0f, 0.0f);
        open = false;
    }

    void Update()
    {
        Vector3 position = astronaut_transform.position + offset;
        if (Input.GetKeyDown(KeyCode.L))
        {
            if(open == false)
            {
                Torch = Instantiate(Prefab, position, Quaternion.Euler(rotation));
                open = true;
            }
            else
            {
                Destroy(Torch);
                open = false;
            }
        }
        if (open == true)
        {
            Torch.transform.position = position;
        }
    }
}
