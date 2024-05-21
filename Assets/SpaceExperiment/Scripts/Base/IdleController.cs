using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleController : MonoBehaviour
{
    Animator m_Animator;
    Vector3 move;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < 500.0f)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");
            move = new Vector3(h, 0, v);
        }
        else
        {
            move = new Vector3(0, 0, 0);
        }
        m_Animator.SetFloat("InputHorizontal", move.x);
        m_Animator.SetFloat("InputVertical", move.z);
    }
}
