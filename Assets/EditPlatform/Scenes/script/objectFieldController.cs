using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objectFieldController : MonoBehaviour
{
    public InputField objectNameInput;
    public InputField codeNameInput;
    public InputField posXInput;
    public InputField posYInput;
    public InputField posZInput;
    public InputField rotXInput;
    public InputField rotYInput;
    public InputField rotZInput;
    public InputField scaleXInput;
    public InputField scaleYInput;
    public InputField scaleZInput;
    public Material mat1;
    public Material mat2;
    public Material mat3;
    public Image image;
    public Sprite img1;
    public Sprite img2;
    public Sprite img3;

    private GameObject gameController;
    private string objectName;
    private string codeName;
    private string posX;
    private string posY;
    private string posZ;
    private string rotX;
    private string rotY;
    private string rotZ;
    private string scaleX;
    private string scaleY;
    private string scaleZ;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setData(Vector3 pos, Vector3 rot, Vector3 scale)
    {
        posX = pos.x.ToString();
        posY = pos.y.ToString();
        posZ = pos.z.ToString();
        rotX = rot.x.ToString();
        rotY = rot.y.ToString();
        rotZ = rot.z.ToString();
        scaleX = scale.x.ToString();
        scaleY = scale.y.ToString();
        scaleZ = scale.z.ToString();
        posXInput.text = posX;
        posYInput.text = posY;
        posZInput.text = posZ;
        rotXInput.text = rotX;
        rotYInput.text = rotY;
        rotZInput.text = rotZ;
        scaleXInput.text = scaleX;
        scaleYInput.text = scaleY;
        scaleZInput.text = scaleZ;
    }

    public void onNameAssign(string s)
    {
        if (s.Length == 0 )
        {
            objectNameInput.text = objectName;
        }
        else
        {
            objectName = s;
            gameController.GetComponent<Controller>().objectNameAssign(gameObject.transform, s);
        }
    }

    public void onCodeNameAssign(string s)
    {
        if (s.Length == 0)
        {
            codeName = null;
            codeNameInput.text = null;
            gameController.GetComponent<Controller>().codeNameAssign(gameObject.transform, null);
        }
        else
        {
            codeName = s;
            gameController.GetComponent<Controller>().codeNameAssign(gameObject.transform, s);
        }
    }

    public void onPosXAssign(string s)
    {
        if (s.Length == 0 )
        {
            posXInput.text = posX;
        }
        else
        {
            posX = s;
            float f = Convert.ToSingle(s);
            gameController.GetComponent<Controller>().PosXAssign(gameObject.transform, f);
        }
    }

    public void onPosYAssign(string s)
    {
        if (s.Length == 0 )
        {
            posYInput.text = posY;
        }
        else
        {
            posY = s;
            float f = Convert.ToSingle(s);
            gameController.GetComponent<Controller>().PosYAssign(gameObject.transform, f);
        }
    }

    public void onPosZAssign(string s)
    {
        if (s.Length == 0 )
        {
            posZInput.text = posZ;
        }
        else
        {
            posZ = s;
            float f = Convert.ToSingle(s);
            gameController.GetComponent<Controller>().PosZAssign(gameObject.transform, f);
        }
    }

    public void onRotXAssign(string s)
    {
        if (s.Length == 0 )
        {
            rotXInput.text = rotX;
        }
        else
        {
            rotX = s;
            float f = Convert.ToSingle(s);
            gameController.GetComponent<Controller>().RotXAssign(gameObject.transform, f);
        }      
    }

    public void onRotYAssign(string s)
    {
        if (s.Length == 0 )
        {
            rotYInput.text = rotY;
        }
        else
        {
            rotY = s;
            float f = Convert.ToSingle(s);
            gameController.GetComponent<Controller>().RotYAssign(gameObject.transform, f);
        }
    }

    public void onRotZAssign(string s)
    {
        if (s.Length == 0 )
        {
            rotZInput.text = rotZ;
        }
        else
        {
            rotZ = s;
            float f = Convert.ToSingle(s);
            gameController.GetComponent<Controller>().RotZAssign(gameObject.transform, f);
        }
    }

    public void onScaleXAssign(string s)
    {
        if (s.Length == 0 )
        {
            scaleXInput.text = scaleX;
        }
        else
        {
            scaleX = s;
            float f = Convert.ToSingle(s);
            gameController.GetComponent<Controller>().ScaleXAssign(gameObject.transform, f);
        }
    }

    public void onScaleYAssign(string s)
    {
        if (s.Length == 0 )
        {
            scaleYInput.text = scaleY;
        }
        else
        {
            scaleY = s;
            float f = Convert.ToSingle(s);
            gameController.GetComponent<Controller>().ScaleYAssign(gameObject.transform, f);
        }
    }

    public void onScaleZAssign(string s)
    {
        if (s.Length == 0 )
        {
            scaleZInput.text = scaleZ;
        }
        else
        {
            scaleZ = s;
            float f = Convert.ToSingle(s);
            gameController.GetComponent<Controller>().ScaleZAssign(gameObject.transform, f);
        } 
    }

    public void onDeleteClick()
    {
        gameController.GetComponent<Controller>().onDeleteClick(gameObject.transform);
    }

    public void onMatOneValueChanged(bool value)
    {
        if (value && gameController!=null)
        {
            gameController.GetComponent<Controller>().changeMat(gameObject.transform, mat1, 1);
            image.sprite = img1;
        }
    }

    public void onMatTwoValueChanged(bool value)
    {
        if (value && gameController!=null)
        {
            gameController.GetComponent<Controller>().changeMat(gameObject.transform, mat2, 2);
            image.sprite = img2;
        }
    }

    public void onMatThereValueChanged(bool value)
    {
        if (value && gameController != null)
        {
            gameController.GetComponent<Controller>().changeMat(gameObject.transform, mat3, 3);
            image.sprite = img3;
        }
    }

    public void onMoveTrackChanged(bool value)
    {
        if (gameController != null)
        {
            gameController.GetComponent<Controller>().setObjTracing(gameObject.transform, value);
        }
    }
}
