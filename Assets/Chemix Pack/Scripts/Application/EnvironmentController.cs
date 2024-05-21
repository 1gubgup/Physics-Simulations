using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chemix
{
    public class EnvironmentController : Singleton<EnvironmentController>
    {
        public Camera camera;
        public Light[] lights;
        public GameObject room;
        // Use this for initialization

        void Start()
        {
            if (Chemix.CustomMode && GM.GM_Core.instance)
            {
                var envInfo = GameManager.Instance.GetExperimentalSetup().envInfo;

                if (envInfo != null)
                {
                    Debug.Log("Chemix: Setup environment");
                    Vector3 cameraRotation = new Vector3(envInfo.cameraAngle, 0, 0);
                    camera.transform.rotation = Quaternion.Euler(cameraRotation);

                    Vector3 cameraPosition = camera.transform.position;
                    cameraPosition.y = envInfo.cameraHeight;
                    camera.transform.position = cameraPosition;
                    camera.fieldOfView = envInfo.cameraFOV;

                    room.SetActive(envInfo.useRoom);

                    if (envInfo.lightInfo != null)
                    {
                        for (int i = 0; i < lights.Length; i++)
                        {
                            var li = envInfo.lightInfo[i];
                            lights[i].color = li.color;
                            lights[i].intensity = li.intensity;
                            lights[i].transform.position = li.position;
                            lights[i].transform.rotation = li.rotation;
                        }
                    }
                }
            }
        }
    }
}