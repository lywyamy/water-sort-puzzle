using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleSpawner : MonoBehaviour

{
    public GameObject bottlePrefab;
    // public GameObject[] bottles;
    public BottleController bottleController;
    public System.Random random = new System.Random(Constants.SEED + 1);
    public Color[] colors;

    void Start()
    {
        populateColors();
        generateBottles(Constants.EASY_NUMBER_OF_BOTTLES);
    }

    void Update()
    {

    }

    void generateBottles(int numberOfFullBottles)
    {
        Color[] colorPool = generateColorPool(colors, numberOfFullBottles);
        int index = 0;

        for (int i = 0; i < numberOfFullBottles; i++)
        {
            GameObject bottleObject = Instantiate(bottlePrefab);
            bottleObject.transform.position = new Vector3(-12 + i * 2, 0, 0);

            BottleController bottle = bottleObject.GetComponent<BottleController>();
            bottle.numberOfColorsInBottle = 4;

            bottle.bottleColors = new Color[] { colorPool[index++], colorPool[index++], colorPool[index++], colorPool[index++] };
        }

        for (int i = 0; i < Constants.NUMBER_OF_EMPTY_BOTTLES; i++)
        {
            GameObject bottle = Instantiate(bottlePrefab);
            bottle.transform.position = new Vector3(-12 + i * 2, -5, 0);
            BottleController bottleController = bottle.GetComponent<BottleController>();
            bottleController.numberOfColorsInBottle = 0;
        }
    }

    private Color[] generateColorPool(Color[] colors, int numberOfFullBottles)
    {
        Color[] colorPool = new Color[Constants.MAX_NUMBER_OF_COLORS_IN_BOTTLE * numberOfFullBottles];
        int index = 0;

        for (int i = 0; i < numberOfFullBottles; i++)
        {
            for (int j = 0; j < Constants.MAX_NUMBER_OF_COLORS_IN_BOTTLE; j++)
            {
                colorPool[index++] = colors[i];
            }
        }

        //Shuffle the colors
        for (int i = 0; i < colorPool.Length - 1; i++)
        {
            int r = random.Next(i, colorPool.Length);
            (colorPool[r], colorPool[i]) = (colorPool[i], colorPool[r]);
        }

        return colorPool;
    }

    private void populateColors()
    {
        colors = new Color[] { Constants.RED, Constants.ORANGE, Constants.PINK, Constants.TEAL, Constants.SKYBLUE, Constants.PURPLE, Constants.NAVYBLUE, Constants.GRAY, Constants.GREEN, Constants.YELLOW, Constants.DARKGREEN, Constants.BROWN };
    }
}
