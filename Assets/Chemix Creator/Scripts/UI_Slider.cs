using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    public class UI_Slider : MonoBehaviour
    {
        public GameObject rotationIndicator;
        public GameObject axisIndicator;
        public GameObject sceneCamera;
        public GameObject lightSource;
        public GameObject showColor;
        public GameObject targetText;
        public TMPro.TextMeshProUGUI showPosition;

        public enum State { HEIGHT, ANGLE, R, G, B, Intensity, Size, FOV, POS_X, POS_Y, POS_Z, ROT_X, ROT_Y };
        public State sliderType;

        private Vector3 srcTextScale;

        // Use this for initialization
        void Start()
        {
            UpdateSliders();
            if (rotationIndicator)
            {
                rotationIndicator.SetActive(false);
            }
            if (axisIndicator)
            {
                axisIndicator.SetActive(false);
            }
        }

        public void UpdateSliders()
        {
            if (sliderType == State.HEIGHT)
            {
                gameObject.GetComponent<Slider>().value = sceneCamera.transform.position.y;
                AdjustCameraHeight();
            }
            else if (sliderType == State.ANGLE)
            {
                gameObject.GetComponent<Slider>().value = sceneCamera.transform.eulerAngles.x;
                AdjustCameraAngle();
            }
            else if (sliderType == State.Intensity)
            {
                gameObject.GetComponent<Slider>().value = lightSource.GetComponent<Light>().intensity;
                AdjustLightIntensity();
            }
            else if (sliderType == State.Size)
            {
                gameObject.GetComponent<Slider>().value = 1;
                srcTextScale = targetText.transform.localScale;
            }
            else if (sliderType == State.FOV)
            {
                gameObject.GetComponent<Slider>().value = sceneCamera.GetComponent<Camera>().fieldOfView;
                AdjustCameraFOV();
            }
            else if (sliderType == State.POS_X)
            {
                gameObject.GetComponent<Slider>().value = lightSource.transform.position.x;
                AdjustLightPosition();
            }
            else if (sliderType == State.POS_Y)
            {
                gameObject.GetComponent<Slider>().value = lightSource.transform.position.y;
                AdjustLightPosition();
            }
            else if (sliderType == State.POS_Z)
            {
                gameObject.GetComponent<Slider>().value = lightSource.transform.position.z;
                AdjustLightPosition();
            }
            else if (sliderType == State.ROT_X)
            {
                gameObject.GetComponent<Slider>().value = lightSource.transform.rotation.eulerAngles.x;
                AdjustLightRotation();
            }
            else if (sliderType == State.ROT_Y)
            {
                var angleY = lightSource.transform.rotation.eulerAngles.y;
                if (angleY > 180.0f)
                {
                    angleY -= 360.0f;
                }
                gameObject.GetComponent<Slider>().value = angleY;
                //Debug.Log($"{lightSource.name}'s is {gameObject.GetComponent<Slider>().value}");
                AdjustLightRotation();
            }
            else
            {
                Color srcColor;
                if (lightSource != null)
                {
                    srcColor = lightSource.GetComponent<Light>().color;
                    if (showColor)
                    {
                        showColor.GetComponent<Image>().color = srcColor;
                    }
                }
                else if (targetText != null)
                {
                    //srcColor = targetText.GetComponent<Renderer>().material.GetColor("_Color");
                    srcColor = targetText.GetComponent<TextMesh>().color;
                    //Debug.Log(srcColor.r);
                }
                else
                {
                    return;
                }
                if (sliderType == State.R)
                {
                    gameObject.GetComponent<Slider>().value = srcColor.r * 255;
                }
                else if (sliderType == State.G)
                {
                    gameObject.GetComponent<Slider>().value = srcColor.g * 255;
                }
                else if (sliderType == State.B)
                {
                    gameObject.GetComponent<Slider>().value = srcColor.b * 255;
                }
                AdjustLightColor();
            }
        }

        public void AdjustCameraHeight()
        {
            Vector3 oldPos = sceneCamera.transform.position;
            float input = gameObject.GetComponent<Slider>().value;
            oldPos.y = input;
            sceneCamera.transform.position = oldPos;
        }

        public void AdjustCameraAngle()
        {
            Vector3 oldRot = sceneCamera.transform.eulerAngles;
            float input = gameObject.GetComponent<Slider>().value;
            oldRot.x = input;
            sceneCamera.transform.eulerAngles = oldRot;
        }

        public void AdjustCameraFOV()
        {
            float input = gameObject.GetComponent<Slider>().value;
            sceneCamera.GetComponent<Camera>().fieldOfView = input;
        }

        public void AdjustLightColor()
        {
            //Color srcLightColor = lightSource.GetComponent<Light>().color;
            Color sliderColor = new Color(0, 0, 0);
            Color oldLightColor;
            if (lightSource != null)
            {
                oldLightColor = lightSource.GetComponent<Light>().color;
            }
            else if (targetText != null)
            {
                //Debug.Log(targetText.name);
                //oldLightColor = targetText.GetComponent<Renderer>().material.GetColor("_Color");
                oldLightColor = targetText.GetComponent<TextMesh>().color;
            }
            else
            {
                return;
            }
            if (sliderType == State.R)
            {
                float value = gameObject.GetComponent<Slider>().value / 255;
                oldLightColor.r = sliderColor.r = value;
            }
            else if (sliderType == State.G)
            {
                float value = gameObject.GetComponent<Slider>().value / 255;
                oldLightColor.g = sliderColor.g = value;
            }
            else if (sliderType == State.B)
            {
                float value = gameObject.GetComponent<Slider>().value / 255;
                oldLightColor.b = sliderColor.b = value;
            }
            ColorBlock cb = gameObject.GetComponent<Slider>().colors;
            cb.pressedColor = sliderColor;
            gameObject.GetComponent<Slider>().colors = cb;

            if (lightSource != null)
            {
                lightSource.GetComponent<Light>().color = oldLightColor;
                if (showColor)
                {
                    showColor.GetComponent<Image>().color = oldLightColor;
                }
            }
            else if (targetText != null)
                targetText.GetComponent<TextMesh>().color = oldLightColor;
            //targetText.GetComponent<Renderer>().material.SetColor("_Color", oldLightColor);
        }

        public void AdjustLightIntensity()
        {
            lightSource.GetComponent<Light>().intensity = gameObject.GetComponent<Slider>().value;
        }

        public void AdjustLightPosition()
        {
            float value = gameObject.GetComponent<Slider>().value;
            var pos = lightSource.transform.position;
            if (sliderType == State.POS_X)
            {
                pos.x = value;
            }
            else if (sliderType == State.POS_Y)
            {
                pos.y = value;
            }
            else if (sliderType == State.POS_Z)
            {
                pos.z = value;
            }
            lightSource.transform.position = pos;

            if (showPosition)
            {
                // Trick: So that it matches with the axis indicator
                pos.y = -pos.y;
                showPosition.text = $"{pos}";
            }

            if (axisIndicator)
            {
                axisIndicator.SetActive(true);
                StopCoroutine("WaitAndDisableAxisIndicator");
                StartCoroutine("WaitAndDisableAxisIndicator");
            }
        }

        public void AdjustLightRotation()
        {
            float value = gameObject.GetComponent<Slider>().value;
            var rot = lightSource.transform.rotation.eulerAngles;
            if (sliderType == State.ROT_X)
            {
                rot.x = value;
            }
            else if (sliderType == State.ROT_Y)
            {
                rot.y = value;
            }

            var rotation = Quaternion.Euler(rot);
            lightSource.transform.rotation = rotation;

            if (showPosition)
            {
                showPosition.text = $"{rot}";
            }

            if (rotationIndicator)
            {
                rotationIndicator.SetActive(true);
                rotationIndicator.transform.rotation = rotation;
                StopCoroutine("WaitAndDisableRotationIndicator");
                StartCoroutine("WaitAndDisableRotationIndicator");
            }
        }

        IEnumerator WaitAndDisableRotationIndicator()
        {
            yield return new WaitForSeconds(1);
            rotationIndicator.SetActive(false);
        }

        IEnumerator WaitAndDisableAxisIndicator()
        {
            yield return new WaitForSeconds(1);
            axisIndicator.SetActive(false);
        }

        public void AdjustTextSize()
        {
            srcTextScale = targetText.GetComponent<Lab_Text>().srcScale;
            targetText.transform.localScale = srcTextScale * gameObject.GetComponent<Slider>().value;
        }
    }
}
