using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    public Color[] bottleColors;
    public SpriteRenderer bottleMaskSR;

    public AnimationCurve ScaleAndRotationRateCurve;
    public AnimationCurve FillRateCurve;

    void Start()
    {
        UpdateColorsOnShader();
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
            bottleMaskSR.material.SetFloat("_FillRate", FillRateCurve.Evaluate(angleValue));

            t += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        angleValue = 90.0f;
        transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMaskSR.material.SetFloat("_ScaleAndRotationRate", ScaleAndRotationRateCurve.Evaluate(angleValue));
        bottleMaskSR.material.SetFloat("_FillRate", FillRateCurve.Evaluate(angleValue));
    }
}
