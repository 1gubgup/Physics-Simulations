using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothController : MonoBehaviour
{
    //用于渲染
    public GameObject flag;

    //系统参数
    public Vector3 springKs = new Vector3(10000, 10000, 10000);
    public float mass = 1;
    public float stepTime = 0.002f;
    //cloth尺寸及控制顶点个数
    public int sizeX = 12;
    public int sizeY = 12;
    public int vertexCountX = 32;
    public int vertexCountY = 32;
    private int totalVertexCount;
    private float distance;

    private ClothSimulation clothSim;

    //控制顶点的位置信息
    private Vector3[] position;
    private Vector3[] velocity;

    //开始模拟
    private bool begin = false;

    int count = 0;

    public void Initialize()
    {
        totalVertexCount = vertexCountX * vertexCountY;

        clothSim = new ClothSimulation();

        //设置系统参数
        clothSim.springKs = springKs;
        clothSim.mass = mass;
        clothSim.dt = stepTime;

        //计算顶点规模及弹簧原长
        distance = (float)sizeX / (float)(vertexCountX - 1);
        clothSim.size = new Vector3Int(vertexCountX, vertexCountY, totalVertexCount);
        clothSim.restLengths = new Vector3(distance, distance * Mathf.Sqrt(2), distance * 2);

        clothSim.positions = new Vector3[totalVertexCount];
        clothSim.velocities = new Vector3[totalVertexCount];

        //执行init
        for (int y = 0; y < vertexCountY; y++)
        {
            for (int x = 0; x < vertexCountX; x++)
            {
                Vector2Int id = new Vector2Int(x, y);
                clothSim.Init(id);
            }
        }

        position = clothSim.positions;
        velocity = clothSim.velocities;

        begin = true;
    }


    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            Initialize();
        }

        if (begin)
        {
            Vector3[] positionBuffer = position;
            Vector3[] velocityBuffer = velocity;

            //执行step
            for (int y = 0; y < vertexCountY; y++)
            {
                for (int x = 0; x < vertexCountX; x++)
                {
                    clothSim.positions = positionBuffer;
                    clothSim.velocities = velocityBuffer;

                    Vector2Int id = new Vector2Int(x, y);
                    clothSim.Step(id);

                    //获取顶点位置和速度
                    position[x * y] = clothSim.positions[x * y];
                    velocity[x * y] = clothSim.velocities[x * y];

                    Debug.Log(clothSim.velocities[x * y]);
                }
            }

            //绘制cloth
            CreatePlane(flag, 1);

            count++;
        }
    }

    public void CreatePlane(GameObject obj, int flag)
    {
        int vertexCountPerDim = vertexCountX;
        int[] indices = new int[6 * (vertexCountPerDim - 1) * (vertexCountPerDim - 1)];
        for (var x = 0; x < vertexCountPerDim - 1; x++)
        {
            for (var y = 0; y < vertexCountPerDim - 1; y++)
            {
                var vertexIndex = (y * vertexCountPerDim + x);
                var quadIndex = y * (vertexCountPerDim - 1) + x;
                var upVertexIndex = (vertexIndex + vertexCountPerDim);
                var offset = quadIndex * 6;
                indices[offset] = vertexIndex;
                indices[offset + 1] = (vertexIndex + 1);
                indices[offset + 2] = upVertexIndex;

                indices[offset + 3] = upVertexIndex;
                indices[offset + 4] = (vertexIndex + 1);
                indices[offset + 5] = (upVertexIndex + 1);
            }
        }

        //已有mesh，更新信息
        if (flag == 1)
        {
            Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
            mesh.vertices = position;
            mesh.triangles = indices;
            mesh.RecalculateNormals();
        }
        else
        {
            // 没有mesh，创建新mesh
            Mesh msh = new Mesh();
            msh.vertices = position;
            msh.triangles = indices;
            msh.RecalculateNormals();
            msh.RecalculateBounds();
            obj.AddComponent<MeshRenderer>();

            MeshFilter filter;
            filter = obj.AddComponent<MeshFilter>();
            filter.mesh = msh;

            obj.AddComponent<MeshCollider>();
            obj.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Custom_Floor") as Material;
            obj.GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    public void AddFlagButtonObjectOnClick()
    {
        Initialize();
    }
}


