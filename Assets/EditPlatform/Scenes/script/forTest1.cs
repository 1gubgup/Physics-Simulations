using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forTest1 : MonoBehaviour
{
    private double g = 9.7964;
    private double height;
    private double velocity = 0;
    private float x;
    private float z;
    private double delta = 0.02;
    private bool flag = false;
    private Vector3 init_pos;
    private Vector3 init_rot;
    private Vector3 init_scale;
    // Start is called before the first frame update
    void Start()
    {
        init_pos = transform.position;
        init_rot = transform.eulerAngles;
        init_scale = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (flag)
        {
            gameObject.transform.position = new Vector3(x, (float)height, z);
            updateHeight();
        }
    }

    void updateHeight()
    {
        if (height <= 0)
        {
            velocity = -velocity;
        }
        height = height + velocity * delta - 0.5 * g * delta * delta;
        if (height <= 0)
        {
            height = 0;
        }
        velocity = velocity - g * delta;
    }

    public void start()
    {
        height = gameObject.transform.position.y;
        x = gameObject.transform.position.x;
        z = gameObject.transform.position.z;
        velocity = 0;
        updateHeight();
        flag = true;
    }

    public void end()
    {
        flag = false;
        transform.position = init_pos;
        transform.eulerAngles = init_rot;
        transform.localScale = init_scale;
        velocity = 0;
    }
}

