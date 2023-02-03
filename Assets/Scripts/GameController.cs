using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public BottleController FirstBottle;
    public BottleController SecondBottle;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<BottleController>() != null)
                {
                    if (FirstBottle == null)
                    {
                        FirstBottle = hit.collider.GetComponent<BottleController>();
                        GameObject bottleObject = hit.collider.gameObject;
                        FirstBottle.transform.position += new Vector3(0.0f, 0.5f, 0.0f);
                    }
                    else
                    {
                        if (FirstBottle == hit.collider.GetComponent<BottleController>())
                        {
                            FirstBottle.transform.position -= new Vector3(0.0f, 0.5f, 0.0f);
                            FirstBottle = null;
                        }
                        else
                        {
                            SecondBottle = hit.collider.GetComponent<BottleController>();
                            FirstBottle.BottleControllerRef = SecondBottle;

                            FirstBottle.UpdateTopColorValues();
                            SecondBottle.UpdateTopColorValues();

                            if (SecondBottle.FillBottleCheck(FirstBottle.topColor))
                            {
                                FirstBottle.StartColorTransfer();
                            }
                            else
                            {
                                FirstBottle.transform.position -= new Vector3(0.0f, 0.5f, 0.0f);
                            }

                            FirstBottle = null;
                            SecondBottle = null;
                        }
                    }
                }
            }
        }

    }
}
