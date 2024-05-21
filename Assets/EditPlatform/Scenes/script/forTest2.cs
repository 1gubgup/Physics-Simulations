using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forTest2 : MonoBehaviour
{
    private Vector3 target = new Vector3(0.0f, 0.0f, 0.0f);
    A theA;
    private bool flag = false;
    private Vector3 init_pos;
    private Vector3 init_rot;
    private Vector3 init_scale;
    // Start is called before the first frame update
    void Start()
    {
        theA = new A(gameObject);
        init_pos = transform.position;
        init_rot = transform.eulerAngles;
        init_scale = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (flag)
        {
            theA.Rotate(target);
        }
    }

    public void start()
    {
        flag = true;
    }

    public void end()
    {
        flag = false;
        transform.position = init_pos;
        transform.eulerAngles = init_rot;
        transform.localScale = init_scale;
    }
}

class A
{
    public GameObject ag;
    public A(GameObject g)
    {
        ag = g;
    }
    public void Rotate(Vector3 target)
    {
        ag.transform.RotateAround(target, Vector3.up, 30 * Time.deltaTime);
    }
}
