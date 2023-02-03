using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleSpawner : MonoBehaviour

{
    public GameObject bottlePrefab;
    private static int seed = 0;
    public System.Random random = new System.Random(seed);
    public Color[] colors;
    public int numberOfFullBottles = 9;
    public BottleController[] initialState;

    void Start()
    {
        populateColors();
        generateBottles(numberOfFullBottles);
    }

    void Update()
    {

    }

    void generateBottles(int numberOfFullBottles)
    {
        Color[] colorPool = generateColorPool(colors, numberOfFullBottles);
        int index = 0;

        int numberOfBottlesOnFirstRow = (numberOfFullBottles + Constants.NUMBER_OF_EMPTY_BOTTLES + 1) / 2;
        int numberOfBottlesOnSecondRow = numberOfFullBottles + Constants.NUMBER_OF_EMPTY_BOTTLES - numberOfBottlesOnFirstRow;
        float firstInterval = calculateInterval(numberOfBottlesOnFirstRow);
        float secondInteval = calculateInterval(numberOfBottlesOnSecondRow);

        initialState = new BottleController[numberOfFullBottles + Constants.NUMBER_OF_EMPTY_BOTTLES];

        for (int i = 0; i < numberOfFullBottles; i++)
        {
            GameObject bottleObject = Instantiate(bottlePrefab);

            if (i < numberOfBottlesOnFirstRow)
            {
                bottleObject.transform.position = new Vector3(Constants.LEFT_X + i * firstInterval, Constants.FIRST_ROW_Y, 0);
            }
            else
            {
                bottleObject.transform.position = new Vector3(Constants.LEFT_X + (i - numberOfBottlesOnFirstRow) * secondInteval, Constants.SECOND_ROW_Y, 0);
            }

            BottleController bottle = bottleObject.GetComponent<BottleController>();
            bottle.numberOfColorsInBottle = 4;
            bottle.bottleColors = new Color[] { colorPool[index++], colorPool[index++], colorPool[index++], colorPool[index++] };

            initialState[i] = bottle;
        }

        for (int i = 0; i < Constants.NUMBER_OF_EMPTY_BOTTLES; i++)
        {
            GameObject bottleObject = Instantiate(bottlePrefab);
            bottleObject.transform.position = new Vector3(Constants.LEFT_X + (i + numberOfBottlesOnSecondRow - Constants.NUMBER_OF_EMPTY_BOTTLES) * secondInteval, Constants.SECOND_ROW_Y, 0);

            BottleController bottle = bottleObject.GetComponent<BottleController>();
            bottle.numberOfColorsInBottle = 0;

            initialState[numberOfFullBottles + i] = bottle;
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

        //Shuffle the colors and increment the random seed
        for (int i = 0; i < colorPool.Length - 1; i++)
        {
            int r = random.Next(i, colorPool.Length);
            (colorPool[r], colorPool[i]) = (colorPool[i], colorPool[r]);
        }
        seed += 1;

        return colorPool;
    }

    private void populateColors()
    {
        colors = new Color[] { Constants.RED, Constants.ORANGE, Constants.PINK, Constants.TEAL, Constants.SKYBLUE, Constants.PURPLE, Constants.NAVYBLUE, Constants.GRAY, Constants.GREEN, Constants.YELLOW, Constants.DARKGREEN, Constants.BROWN };
    }

    private float calculateInterval(int numberOfBottlesOnThisRow)
    {
        int range = Constants.RIGHT_X - Constants.LEFT_X;
        int numberOfInterval = numberOfBottlesOnThisRow - 1;
        float interval = (float)range / numberOfInterval;

        return interval;
    }
}
