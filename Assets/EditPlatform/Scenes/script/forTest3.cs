using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class forTest3 : MonoBehaviour
{
    double theta = 0.5 * Mathf.PI;
    double w = 0;
    double g = 9.8;
    double l = 100;

    public GameObject sphere;
    public GameObject cube;

    private Vector3 init_pos_s;
    private Vector3 init_rot_s;
    private Vector3 init_pos_c;
    private Vector3 init_rot_c;

    private bool flag;
    // Start is called before the first frame update
    void Start()
    {
        init_pos_s = sphere.transform.position;
        init_rot_s = sphere.transform.eulerAngles;
        init_pos_c = cube.transform.position;
        init_rot_c = cube.transform.eulerAngles;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (flag)
        {
            w = w + (-g / l * Mathf.Sin((float)theta)) * Time.deltaTime;
            sphere.transform.RotateAround(init_pos_s + new Vector3((float)l,0,0), Vector3.back, (float)w * Time.deltaTime * Mathf.Rad2Deg);
            cube.transform.RotateAround(init_pos_s + new Vector3((float)l, 0, 0), Vector3.back, (float)w * Time.deltaTime * Mathf.Rad2Deg);
            theta = theta + w * Time.deltaTime;
        }
    }

    public void start()
    {
        flag = true;
    }

    public void end()
    {
        flag = false;
        sphere.transform.position = init_pos_s;
        sphere.transform.eulerAngles = init_rot_s;
        cube.transform.position = init_pos_c;
        cube.transform.eulerAngles = init_rot_c;
        theta = 0.5 * Mathf.PI;
        w = 0;
    }
}
