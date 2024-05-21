using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMove : MonoBehaviour
{

    public float mouseSense = 3f;

    public float normal_speed = 1f;

    public float shift_speed = 10f;

    public float x_s;
    public float x_b;
    public float y_s;
    public float y_b;
    public float z_s;
    public float z_b;

    public Vector3 initPosition;

    public Vector3 initRotation;

    private bool active = true;

    private bool mode = false;

    private int legacy = 0;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (shift_speed < normal_speed)
            shift_speed = normal_speed;
        if (x_b < x_s)
            x_b = x_s;
        if (y_b < y_s)
            y_b = y_s;
        if (z_b < z_s)
            z_b = z_s;
    }
#endif

    private void Start()
    {
        transform.position = initPosition;
        transform.eulerAngles = initRotation;
    }

    private void Update()
    {

        if (!active)
        {
            return;
        }

        if (legacy > 0)
        {
            legacy--;
        }

        if (Input.GetKey(KeyCode.Tab) && legacy == 0)
        {
            if (mode == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                mode = true;
                legacy = 25;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                mode = false;
                legacy = 25;
            }
        }

        if (Input.GetKey(KeyCode.Escape) && legacy == 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            mode = false;
            legacy = 25;
        }


        if (Cursor.visible)
        {
            return;
        }

        transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * Time.deltaTime * 50);

        Vector3 postion_temp = Vector3.zero;

        float speed = normal_speed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = shift_speed;
        }

        if (Input.GetKey(KeyCode.W))
        {
            postion_temp += transform.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            postion_temp -= transform.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            postion_temp -= transform.right;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            postion_temp += transform.right;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            postion_temp += transform.up;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            postion_temp -= transform.up;
        }

        transform.position += postion_temp * speed;

        transform.rotation *= Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * mouseSense, Vector3.right);

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + Input.GetAxis("Mouse X") * mouseSense, transform.eulerAngles.z);

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = initPosition;
            transform.eulerAngles = initRotation;
        }

        float x_temp = transform.position.x;
        float y_temp = transform.position.y;
        float z_temp = transform.position.z;
        if (x_temp > x_b)
        {
            x_temp = x_b;
        }
        else if (x_temp < x_s)
        {
            x_temp = x_s;
        }

        if (y_temp > y_b)
        {
            y_temp = y_b;
        }
        else if (y_temp < y_s)
        {
            y_temp = y_s;
        }

        if (z_temp > z_b)
        {
            z_temp = z_b;
        }
        else if (z_temp < z_s)
        {
            z_temp = z_s;
        }

        transform.position = new Vector3(x_temp, y_temp, z_temp);
    }

    public void Active()
    {
        active = true;
        mode = false;
    }

    public void NoActive()
    {
        active = false;
        mode = false;
    }
}
