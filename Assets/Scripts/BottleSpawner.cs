using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleSpawner : MonoBehaviour

{
    public GameObject bottlePrefab;
    public GameObject[] bottles;
    public BottleController bottleController;
    public System.Random random = new System.Random();

    void Start()
    {
        generateBottles(Constants.EASY_NUMBER_OF_BOTTLES);
    }

    void Update()
    {

    }

    void generateBottles(int numberOfFullBottles)
    {
        for (int i = 0; i < numberOfFullBottles; i++)
        {
            GameObject bottle = Instantiate(bottlePrefab);
            bottle.transform.position = new Vector3(-12 + i * 2, 0, 0);
        }

        for (int i = 0; i < Constants.NUMBER_OF_EMPTY_BOTTLES; i++)
        {
            GameObject bottle = Instantiate(bottlePrefab);
            bottle.transform.position = new Vector3(-12 + i * 2, -5, 0);
            BottleController bottleController = bottle.GetComponent<BottleController>();
            bottleController.numberOfColorsInBottle = 0;
        }
    }
}
