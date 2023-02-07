using System.Collections;
using UnityEngine;
using System;

public class BottleController : MonoBehaviour
{
    public Color[] bottleColors;
    public SpriteRenderer bottleMaskSR;
    public int bottleIndex;

    public AnimationCurve ScaleAndRotationRateCurve;
    public AnimationCurve FillRateCurve;
    public AnimationCurve RotationSpeedRateCurve;

    public float[] fillRates = new float[5] { -2.05f, -1.15f, -0.25f, 0.65f, 1.55f };
    public float[] ScaleAndRotationRates = new float[4] { 54.0f, 71.0f, 83.0f, 90.0f };

    public float timeToRotate = 0.6f;
    public float timeToMove = 1.0f;

    private int rotationIndex = 0;

    [Range(0, 4)]
    public int numberOfColorsInBottle = 4;

    public Color topColor;
    public int topColorLayers = 1;

    public BottleController BottleControllerRef;
    public int numberOfColorsToTransfer = 0;

    public Transform leftRotationPoint;
    public Transform rightRotationPoint;
    private Transform chosenRotationPoint;
    private float rotationDirectionMultiplier = 1.0f;

    Vector3 originalPosition;
    Vector3 startPosition;
    Vector3 endPosition;

    public LineRenderer lineRenderer;

    public Action animationForAutoSolution;

    void Start()
    {
        updateBottle();
    }

    public void updateBottle()
    {
        lineRenderer = FindObjectOfType<LineRenderer>();
        bottleMaskSR.material.SetFloat("_FillRate", fillRates[numberOfColorsInBottle]);
        originalPosition = transform.position;
        UpdateColorsOnShader();
        UpdateTopColorValues();
    }

    public void StartColorTransfer(Action animationCallBack = null)
    {
        animationForAutoSolution = animationCallBack;

        ChooseRotationPointAndDirection();
        numberOfColorsToTransfer = Mathf.Min(topColorLayers, 4 - BottleControllerRef.numberOfColorsInBottle);

        for (int i = 0; i < numberOfColorsToTransfer; i++)
        {
            BottleControllerRef.bottleColors[BottleControllerRef.numberOfColorsInBottle + i] = topColor;
        }

        CalculateRotationIndex(4 - BottleControllerRef.numberOfColorsInBottle);

        UpdateColorsOnShader();
        BottleControllerRef.UpdateColorsOnShader();

        transform.GetComponent<SpriteRenderer>().sortingOrder += 2;
        bottleMaskSR.sortingOrder += 2;

        StartCoroutine(MoveBottle());
    }

    IEnumerator MoveBottle()
    {
        startPosition = transform.position;
        if (chosenRotationPoint == leftRotationPoint)
        {
            endPosition = BottleControllerRef.rightRotationPoint.position;
        }
        else
        {
            endPosition = BottleControllerRef.leftRotationPoint.position;
        }

        float t = 0.0f;
        while (t <= timeToMove)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            t += Time.deltaTime * 2;

            yield return new WaitForEndOfFrame();
        }

        transform.position = endPosition;

        StartCoroutine(RotateBottle());
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

            transform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);

            bottleMaskSR.material.SetFloat("_ScaleAndRotationRate", ScaleAndRotationRateCurve.Evaluate(angleValue));

            if (fillRates[numberOfColorsInBottle] > FillRateCurve.Evaluate(angleValue) + 0.005f)
            {
                if (!lineRenderer.enabled)
                {
                    lineRenderer.startColor = topColor;
                    lineRenderer.endColor = topColor;

                    lineRenderer.SetPosition(0, chosenRotationPoint.position);
                    lineRenderer.SetPosition(1, chosenRotationPoint.position - Vector3.up * 6.0f);

                    lineRenderer.enabled = true;
                }
                bottleMaskSR.material.SetFloat("_FillRate", FillRateCurve.Evaluate(angleValue));
                BottleControllerRef.FillUp(FillRateCurve.Evaluate(lastAngleValue) - FillRateCurve.Evaluate(angleValue));
            }

            t += Time.deltaTime * RotationSpeedRateCurve.Evaluate(angleValue);
            lastAngleValue = angleValue;

            yield return new WaitForEndOfFrame();
        }
        angleValue = rotationDirectionMultiplier * ScaleAndRotationRates[rotationIndex];

        bottleMaskSR.material.SetFloat("_ScaleAndRotationRate", ScaleAndRotationRateCurve.Evaluate(angleValue));
        bottleMaskSR.material.SetFloat("_FillRate", FillRateCurve.Evaluate(angleValue));

        numberOfColorsInBottle -= numberOfColorsToTransfer;
        BottleControllerRef.numberOfColorsInBottle += numberOfColorsToTransfer;

        lineRenderer.enabled = false;

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

            transform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);

            bottleMaskSR.material.SetFloat("_ScaleAndRotationRate", ScaleAndRotationRateCurve.Evaluate(angleValue));
            lastAngleValue = angleValue;

            t += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        angleValue = 0.0f;
        transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMaskSR.material.SetFloat("_ScaleAndRotationRate", ScaleAndRotationRateCurve.Evaluate(angleValue));

        StartCoroutine(MoveBottleBack());
    }

    IEnumerator MoveBottleBack()
    {
        startPosition = transform.position;
        endPosition = originalPosition;

        float t = 0.0f;
        while (t <= timeToMove)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            t += Time.deltaTime * 2;

            yield return new WaitForEndOfFrame();
        }
        transform.position = endPosition;

        transform.GetComponent<SpriteRenderer>().sortingOrder -= 2;
        bottleMaskSR.sortingOrder -= 2;

        UpdateTopColorValues();
        BottleControllerRef.UpdateTopColorValues();

        if (animationForAutoSolution != null)
        {
            animationForAutoSolution();
        }
    }

    public void UpdateColorsOnShader()
    {
        bottleMaskSR.material.SetColor("_Color1", bottleColors[0]); // bottome color
        bottleMaskSR.material.SetColor("_Color2", bottleColors[1]);
        bottleMaskSR.material.SetColor("_Color3", bottleColors[2]);
        bottleMaskSR.material.SetColor("_Color4", bottleColors[3]); // top color
    }

    public void UpdateTopColorValues()
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

    public bool FillBottleCheck(Color colorToCheck)
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
