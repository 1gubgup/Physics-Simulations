using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class ClothSimulation
{
    //顶点位置
    public Vector3[] positions;
    //顶点速度
    public Vector3[] velocities;

    //x表示横向顶点数量,y表示纵向顶点数量,z = x * y
    public Vector3Int size;

    //弹性系数.xyz分别对应结构弹簧、剪力弹簧、弯曲弹簧
    public Vector3 springKs;

    //弹簧在松弛状态下的长度.xyz分别对应结构弹簧、剪力弹簧、弯曲弹簧
    public Vector3 restLengths;

    //单个顶点的质量
    public float mass;

    //单次迭代时间间隔
    public float dt;

    //结构弹簧的4个方向
    Vector2Int[] SpringDirs = {
    //结构力
    new Vector2Int(1,0),
    new Vector2Int(0,1),
    new Vector2Int(-1,0),
    new Vector2Int(0,-1),
    //剪力
    new Vector2Int(-1,-1),
    new Vector2Int(-1,1),
    new Vector2Int(1,-1),
    new Vector2Int(1,1),
    //弯矩力
    new Vector2Int(-2,0),
    new Vector2Int(2,0),
    new Vector2Int(0,2),
    new Vector2Int(0,-2),
    };

    int getIndex(Vector2Int id)
    {
        return id.y * size.x + id.x;
    }

    Vector3 getPosition(int index)
    {
        return positions[index];
    }

    Vector3 getPosition(Vector2Int id)
    {
        return positions[getIndex(id)];
    }

    Vector3 getVelocity(int index)
    {
        return velocities[index];
    }


    void setVelocity(int index, Vector3 vel)
    {
        velocities[index] = vel;
    }

    void setPosition(int index, Vector3 pos)
    {
        positions[index] = pos;
    }

    bool isValidateId(Vector2Int id)
    {
        return id.x >= 0 && id.x < size.x && id.y >= 0 && id.y < size.y;
    }

    //弹性力计算.springType 0,1,2分别代表结构弹簧、剪力弹簧、弯曲弹簧
    //Vector3 getSpring(Vector3 p, Vector3 q, int springType)
    //{
    //    Vector3 dp = p - q;
    //    float len = dp.magnitude;
    //    if (Mathf.Approximately(len, 0.0f) || len < 0.001f)
    //    {
    //        return Vector3.zero; // 如果长度为零，返回零向量以避免除以零错误
    //    }
    //    float restL = restLengths[springType];
    //    return dp * (springKs[springType] * (restL / len - 1));
    //}

    Vector3 getSpring(Vector3 p, Vector3 q, int springType)
    {
        Vector3 dp = p - q;
        float currentLength = dp.magnitude;
        if (Mathf.Approximately(currentLength, 0.0f))
        {
            return Vector3.zero;
        }
        float restLength = restLengths[springType];
        float forceMagnitude = springKs[springType] * (currentLength - restLength);
        Vector3 force = (dp / currentLength) * -forceMagnitude;
        return force;
    }


    Vector3 calculateF(Vector2Int id, Vector3 position, Vector3 velocity)
    {
        int index = getIndex(id);
        Vector3 f = Vector3.zero;

        //重力
        Vector3 fg = new Vector3(0, -9.8f * 20, 0) * mass;
        f += fg;

        //弹性力
        for (int i = 0; i < 12; i++)
        {
            Vector2Int nId = id + SpringDirs[i];
            int nIndex = getIndex(nId);
            if (isValidateId(nId))
            {
                Vector3 nPos = getPosition(nIndex);
                f += getSpring(position, nPos, i / 4);
            }
        }

        //阻尼力
        Vector3 fd = -0.5f * velocity;
        f += fd;
        return f;
    }


    public void Init(Vector2Int id)
    {
        //初始化顶点位置和速度
        int index = getIndex(id);
        positions[index] = new Vector3(id.x * restLengths.x, 0, id.y * restLengths.x);
        velocities[index] = Vector3.zero;
    }

    public void Step(Vector2Int id)
    {
        //固定两个顶点
        if (id.y == 0 && (id.x == 0 || id.x == size.x - 1))
        {
            return;
        }

        int index = getIndex(id);

        //计算受力和加速度
        Vector3 f = calculateF(id, getPosition(index), getVelocity(index));
        Vector3 a = f / mass;

        //更新速度
        Vector3 velocity = getVelocity(index);
        velocity = velocity + a * dt;
        setVelocity(index, velocity);

        //更新位置
        Vector3 position = getPosition(index);
        position += velocity * dt;
        setPosition(index, position);
    }
}
