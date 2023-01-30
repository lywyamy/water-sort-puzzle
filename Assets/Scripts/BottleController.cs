using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    public Color[] bottleColors;
    public SpriteRenderer bottleMaskSR;

    public AnimationCurve ScaleAndRotationRateCurve;
    public AnimationCurve FillRateCurve;
    public AnimationCurve RotationSpeedRateCurve;

    public float[] fillRates;
    public float[] ScaleAndRotationRates;

    private int rotationIndex = 0;

    [Range(0, 4)]
    public int numberOfColorsInBottle = 4;

    public Color topColor;
    public int topColorLayers = 1;


    void Start()
    {
        bottleMaskSR.material.SetFloat("_FillRate", fillRates[numberOfColorsInBottle]);
        UpdateColorsOnShader();
        UpdateTopColorValues();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            StartCoroutine(RotateBottle());
        }
    }

    void UpdateColorsOnShader()
    {
        bottleMaskSR.material.SetColor("_Color1", bottleColors[0]);
        bottleMaskSR.material.SetColor("_Color2", bottleColors[1]);
        bottleMaskSR.material.SetColor("_Color3", bottleColors[2]);
        bottleMaskSR.material.SetColor("_Color4", bottleColors[3]);
    }

    public float timeToRotate = 1.0f;

    IEnumerator RotateBottle()
    {
        float t = 0;
        float lerpValue;
        float angleValue;

        while (t < timeToRotate)
        {
            lerpValue = t / timeToRotate;
            angleValue = Mathf.Lerp(0.0f, 90.0f, lerpValue);

            transform.eulerAngles = new Vector3(0, 0, angleValue);
            bottleMaskSR.material.SetFloat("_ScaleAndRotationRate", ScaleAndRotationRateCurve.Evaluate(angleValue));

            if (fillRates[numberOfColorsInBottle] > FillRateCurve.Evaluate(angleValue))
            {
                bottleMaskSR.material.SetFloat("_FillRate", FillRateCurve.Evaluate(angleValue));

            }

            t += Time.deltaTime * RotationSpeedRateCurve.Evaluate(angleValue);

            yield return new WaitForEndOfFrame();
        }
        angleValue = 90.0f;
        transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMaskSR.material.SetFloat("_ScaleAndRotationRate", ScaleAndRotationRateCurve.Evaluate(angleValue));
        bottleMaskSR.material.SetFloat("_FillRate", FillRateCurve.Evaluate(angleValue));

        StartCoroutine(RotateBottleBack());
    }

    IEnumerator RotateBottleBack()
    {
        float t = 0;
        float lerpValue;
        float angleValue;

        while (t < timeToRotate)
        {
            lerpValue = t / timeToRotate;
            angleValue = Mathf.Lerp(90.0f, 0.0f, lerpValue);

            transform.eulerAngles = new Vector3(0, 0, angleValue);
            bottleMaskSR.material.SetFloat("_ScaleAndRotationRate", ScaleAndRotationRateCurve.Evaluate(angleValue));

            t += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        angleValue = 0.0f;
        transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMaskSR.material.SetFloat("_ScaleAndRotationRate", ScaleAndRotationRateCurve.Evaluate(angleValue));
    }

    void UpdateTopColorValues()
    {
        if (numberOfColorsInBottle != 0)
        {
            topColorLayers = 1;
            topColor = bottleColors[numberOfColorsInBottle - 1];

            if (numberOfColorsInBottle == 4)
            {
                if (bottleColors[3].Equals(bottleColors[2]))
                {
                    topColorLayers = 2;

                    if (bottleColors[2].Equals(bottleColors[1]))
                    {
                        topColorLayers = 3;

                        if (bottleColors[1].Equals(bottleColors[0]))
                        {
                            topColorLayers = 4;
                        }
                    }
                }
            }
            else if (numberOfColorsInBottle == 3)
            {
                if (bottleColors[2].Equals(bottleColors[1]))
                {
                    topColorLayers = 2;

                    if (bottleColors[1].Equals(bottleColors[0]))
                    {
                        topColorLayers = 3;
                    }
                }
            }
            else if (numberOfColorsInBottle == 2)
            {
                if (bottleColors[1].Equals(bottleColors[0]))
                {
                    topColorLayers = 2;
                }
            }
        }
    }
}
