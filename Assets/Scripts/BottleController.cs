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

    public float timeToRotate = 1.0f;

    private int rotationIndex = 0;

    [Range(0, 4)]
    public int numberOfColorsInBottle = 4;

    public Color topColor;
    public int topColorLayers = 1;

    public BottleController BottleControllerRef;
    public bool justThisBottle = false;
    private int numerberOfColorsToTransfer = 0;

    public Transform leftRotationPoint;
    public Transform rightRotationPoint;
    private Transform chosenRotationPoint;
    private float rotationDirectionMultiplier = 1.0f;

    void Start()
    {
        bottleMaskSR.material.SetFloat("_FillRate", fillRates[numberOfColorsInBottle]);
        UpdateColorsOnShader();
        UpdateTopColorValues();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P) && justThisBottle)
        {
            UpdateTopColorValues();

            if (BottleControllerRef.FillBottleCheck(topColor))
            {
                ChooseRotationPointAndDirection();

                numerberOfColorsToTransfer = Mathf.Min(topColorLayers, 4 - BottleControllerRef.numberOfColorsInBottle);
                for (int i = 0; i < numerberOfColorsToTransfer; i++)
                {
                    BottleControllerRef.bottleColors[BottleControllerRef.numberOfColorsInBottle + i] = topColor;
                }
                BottleControllerRef.UpdateColorsOnShader();
            }
            CalculateRotationIndex(4 - BottleControllerRef.numberOfColorsInBottle);
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

    IEnumerator RotateBottle()
    {
        float t = 0;
        float lerpValue;
        float angleValue;
        float lastAngleValue = 0;

        while (t < timeToRotate)
        {
            lerpValue = t / timeToRotate;
            angleValue = Mathf.Lerp(0.0f, rotationDirectionMultiplier * ScaleAndRotationRates[rotationIndex], lerpValue);

            // transform.eulerAngles = new Vector3(0, 0, angleValue);
            transform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);

            bottleMaskSR.material.SetFloat("_ScaleAndRotationRate", ScaleAndRotationRateCurve.Evaluate(angleValue));

            if (fillRates[numberOfColorsInBottle] > FillRateCurve.Evaluate(angleValue))
            {
                bottleMaskSR.material.SetFloat("_FillRate", FillRateCurve.Evaluate(angleValue));
                BottleControllerRef.FillUp(FillRateCurve.Evaluate(lastAngleValue) - FillRateCurve.Evaluate(angleValue));
            }

            t += Time.deltaTime * RotationSpeedRateCurve.Evaluate(angleValue);
            lastAngleValue = angleValue;

            yield return new WaitForEndOfFrame();
        }
        angleValue = rotationDirectionMultiplier * ScaleAndRotationRates[rotationIndex];
        // transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMaskSR.material.SetFloat("_ScaleAndRotationRate", ScaleAndRotationRateCurve.Evaluate(angleValue));
        bottleMaskSR.material.SetFloat("_FillRate", FillRateCurve.Evaluate(angleValue));

        numberOfColorsInBottle -= numerberOfColorsToTransfer;
        BottleControllerRef.numberOfColorsInBottle += numerberOfColorsToTransfer;

        StartCoroutine(RotateBottleBack());
    }

    IEnumerator RotateBottleBack()
    {
        float t = 0;
        float lerpValue;
        float angleValue;
        float lastAngleValue = rotationDirectionMultiplier * ScaleAndRotationRates[rotationIndex];

        while (t < timeToRotate)
        {
            lerpValue = t / timeToRotate;
            angleValue = Mathf.Lerp(rotationDirectionMultiplier * ScaleAndRotationRates[rotationIndex], 0.0f, lerpValue);
            // transform.eulerAngles = new Vector3(0, 0, angleValue);
            transform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);

            bottleMaskSR.material.SetFloat("_ScaleAndRotationRate", ScaleAndRotationRateCurve.Evaluate(angleValue));
            lastAngleValue = angleValue;

            t += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        UpdateTopColorValues();
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

            rotationIndex = 3 - (numberOfColorsInBottle - topColorLayers);
        }
    }

    private bool FillBottleCheck(Color colorToCheck)
    {
        if (numberOfColorsInBottle == 0)
        {
            return true;
        }
        else if (numberOfColorsInBottle == 4)
        {
            return false;
        }
        else if (topColor.Equals(colorToCheck))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CalculateRotationIndex(int numberOfEmptySapcesInSecondBottle)
    {
        rotationIndex = 3 - (numberOfColorsInBottle - Mathf.Min(numberOfEmptySapcesInSecondBottle, topColorLayers));
    }

    private void FillUp(float fillRateToAdd)
    {
        bottleMaskSR.material.SetFloat("_FillRate", bottleMaskSR.material.GetFloat("_FillRate") + fillRateToAdd);
    }

    private void ChooseRotationPointAndDirection()
    {
        if (transform.position.x > BottleControllerRef.transform.position.x)
        {
            chosenRotationPoint = leftRotationPoint;
            rotationDirectionMultiplier = -1.0f;
        }
        else
        {
            chosenRotationPoint = rightRotationPoint;
            rotationDirectionMultiplier = 1.0f;
        }
    }
}
