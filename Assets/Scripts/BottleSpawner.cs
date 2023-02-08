using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BottleSpawner : MonoBehaviour

{
    public GameObject bottlePrefab;
    private static int seed = 1;
    public System.Random random = new System.Random(seed);
    public Color[] colors;
    public static int numberOfFullBottles = 6;
    public int numberOfEmptyBottles = 2;
    public List<BottleController> currentState;

    private List<Color[]> initialCorlorState;
    public GameController gameController;

    private bool solved;

    public List<UserAction> solutionSteps;
    public int currentStep;

    public Text levelNumber;

    void Start()
    {
        levelNumber.text = seed.ToString();
        solved = false;
        currentState = new List<BottleController>();
        initialCorlorState = new List<Color[]>();
        populateColors();
        generateBottles(numberOfFullBottles);
    }

    public void generateBottles(int numberOfFullBottles)
    {
        Color[] colorPool = generateColorPool(colors, numberOfFullBottles);
        int index = 0;

        int numberOfBottlesOnFirstRow = (numberOfFullBottles + 2 + 1) / 2; // initial number of empty bottles is 2, adding 1 for rounding
        int numberOfBottlesOnSecondRow = numberOfFullBottles + numberOfEmptyBottles - numberOfBottlesOnFirstRow;
        float firstInterval = calculateInterval(numberOfBottlesOnFirstRow);
        float secondInteval = calculateInterval(numberOfBottlesOnSecondRow);

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
            bottle.bottleIndex = i;
            bottle.numberOfColorsInBottle = Constants.MAX_NUMBER_OF_COLORS_IN_BOTTLE;

            Color[] bottleColors = new Color[Constants.MAX_NUMBER_OF_COLORS_IN_BOTTLE];
            Color[] copyOfBottleColors = new Color[Constants.MAX_NUMBER_OF_COLORS_IN_BOTTLE];

            for (int j = 0; j < Constants.MAX_NUMBER_OF_COLORS_IN_BOTTLE; j++, index++)
            {
                bottleColors[j] = colorPool[index];
                copyOfBottleColors[j] = colorPool[index];

            }

            bottle.bottleColors = bottleColors;
            currentState.Add(bottle);

            initialCorlorState.Add(copyOfBottleColors);
        }

        for (int i = 0; i < numberOfEmptyBottles; i++)
        {
            GameObject bottleObject = Instantiate(bottlePrefab);
            bottleObject.transform.position = new Vector3(Constants.LEFT_X + (i + numberOfBottlesOnSecondRow - numberOfEmptyBottles) * secondInteval, Constants.SECOND_ROW_Y, 0);

            BottleController bottle = bottleObject.GetComponent<BottleController>();
            bottle.bottleIndex = numberOfFullBottles + i;

            bottle.numberOfColorsInBottle = 0;

            currentState.Add(bottle);
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

    public void resetGame()
    {
        resetGameDisplay();
        gameController.userActionTracker.Clear();
    }

    private void resetGameDisplay()
    {
        for (int i = 0; i < initialCorlorState.Count; i++)
        {
            Color[] originalColors = initialCorlorState[i];
            Color[] copyOfOriginalColors = new Color[Constants.MAX_NUMBER_OF_COLORS_IN_BOTTLE];

            for (int j = 0; j < Constants.MAX_NUMBER_OF_COLORS_IN_BOTTLE; j++)
            {
                copyOfOriginalColors[j] = originalColors[j];
            }

            currentState[i].bottleColors = copyOfOriginalColors;
            currentState[i].numberOfColorsInBottle = Constants.MAX_NUMBER_OF_COLORS_IN_BOTTLE;
            currentState[i].updateBottle();
        }

        for (int i = 0; i < numberOfEmptyBottles; i++)
        {
            currentState[numberOfFullBottles + i].numberOfColorsInBottle = 0;
            currentState[numberOfFullBottles + i].updateBottle();
        }
    }

    public void undoGame()
    {
        List<UserAction> userActions = gameController.userActionTracker;

        if (userActions.Count > 0)
        {
            int lastMoveSourceBottleIndex = userActions[userActions.Count - 1].sourceIndex;
            int lastMoveDestinationBottleIndex = userActions[userActions.Count - 1].destinationIndex;
            int numberOfWaterMoved = userActions[userActions.Count - 1].numberOfWaterMoved;

            for (int i = 0; i < numberOfWaterMoved; i++)
            {
                currentState[lastMoveSourceBottleIndex].bottleColors[currentState[lastMoveSourceBottleIndex].numberOfColorsInBottle + i] = currentState[lastMoveDestinationBottleIndex].topColor;
            }

            currentState[lastMoveSourceBottleIndex].numberOfColorsInBottle += numberOfWaterMoved;
            currentState[lastMoveDestinationBottleIndex].numberOfColorsInBottle -= numberOfWaterMoved;

            currentState[lastMoveSourceBottleIndex].updateBottle();
            currentState[lastMoveDestinationBottleIndex].updateBottle();

            userActions.RemoveAt(userActions.Count - 1);
        }
    }

    public void addExtraEmptyBottle()
    {
        if (numberOfEmptyBottles == 2)
        {
            // Create an empty bottle and append it to the currentState
            GameObject bottleObject = Instantiate(bottlePrefab);

            BottleController bottle = bottleObject.GetComponent<BottleController>();
            bottle.numberOfColorsInBottle = 0;

            numberOfEmptyBottles++;
            bottle.bottleIndex = numberOfFullBottles + numberOfEmptyBottles - 1;

            currentState.Add(bottle);

            // Adjust the position of each bottle on the 2nd row
            adjustSecondRowSpacing();
        }
    }

    public void solveGameAndPlayAnimation()
    {
        resetGame();

        if (numberOfEmptyBottles == 3)
        {
            numberOfEmptyBottles--;
            adjustSecondRowSpacing();
        }

        solutionSteps = solve();
        currentStep = 0;

        resetGameDisplay();

        playAnimationForEachStep();
    }

    public void playAnimationForEachStep()
    {
        if (currentStep < solutionSteps.Count)
        {
            BottleController firstBottle = currentState[solutionSteps[currentStep].sourceIndex].GetComponent<BottleController>();
            BottleController secondBottle = currentState[solutionSteps[currentStep].destinationIndex].GetComponent<BottleController>();

            currentStep++;

            firstBottle.BottleControllerRef = secondBottle;
            firstBottle.StartColorTransfer(playAnimationForEachStep);
        }

    }

    private List<UserAction> solve()
    {
        List<UserAction> solutionSteps = gameController.userActionTracker;
        solutionSteps.Clear();

        for (int i = 0; i < numberOfFullBottles + numberOfEmptyBottles; i++)
        {
            solveHelper(i, solutionSteps);
            if (solved)
            {
                return solutionSteps;
            }
        }

        return null;
    }

    private void solveHelper(int start, List<UserAction> solutionSteps)
    {
        if (checkSuccess())
        {
            solved = true;
            return;
        }

        int[] nextAvailableMove = findNextAvailableMove(start);

        if (nextAvailableMove == null)
        {
            return;
        }

        int destinationIndex = nextAvailableMove[0];
        int waterSize = nextAvailableMove[1];
        UserAction newAction = new UserAction(start, destinationIndex, waterSize);
        solutionSteps.Add(newAction);

        BottleController sourceBottle = currentState[start].GetComponent<BottleController>();
        BottleController destinationBottle = currentState[destinationIndex].GetComponent<BottleController>();

        for (int i = 0; i < waterSize; i++)
        {
            destinationBottle.bottleColors[destinationBottle.numberOfColorsInBottle + i] = sourceBottle.topColor;
        }

        sourceBottle.numberOfColorsInBottle -= waterSize;
        destinationBottle.numberOfColorsInBottle += waterSize;

        sourceBottle.updateBottle();
        destinationBottle.updateBottle();

        for (int j = start; j < numberOfFullBottles + numberOfEmptyBottles + start; j++)
        {
            if (j < numberOfFullBottles + numberOfEmptyBottles)
            {
                solveHelper(j, solutionSteps);
            }
            else
            {
                solveHelper(j - numberOfFullBottles - numberOfEmptyBottles, solutionSteps);
            }
        }

        if (!solved)
        {
            undoGame();
        }

        return;
    }

    private int[] findNextAvailableMove(int sourceIndex)
    {
        for (int destinationIndex = 0; destinationIndex < numberOfFullBottles + numberOfEmptyBottles; destinationIndex++)
        {
            if (destinationIndex == sourceIndex) // can't pour water from the source bottle to itself
            {
                continue;
            }
            else if (currentState[sourceIndex].numberOfColorsInBottle == 0) // can't pour water out of an empty bottle
            {
                continue;
            }
            else if (currentState[sourceIndex].topColorLayers == currentState[sourceIndex].numberOfColorsInBottle && currentState[destinationIndex].numberOfColorsInBottle == 0) // if a bottle has one single color (not necessarily full), no need to pour into an empty bottle
            {
                continue;
            }
            else if (currentState[sourceIndex].numberOfColorsInBottle > 0 && currentState[destinationIndex].numberOfColorsInBottle > 0 && !currentState[sourceIndex].topColor.Equals(currentState[destinationIndex].topColor)) // the top colors are not the same
            {
                continue;
            }
            else if (currentState[sourceIndex].topColorLayers > 4 - currentState[destinationIndex].numberOfColorsInBottle) // can't empty the top colors
            {
                continue;
            }
            else
            {
                return new int[] { destinationIndex, currentState[sourceIndex].topColorLayers };
            }
        }

        return null;
    }

    private bool checkSuccess()
    {
        for (int i = 0; i < numberOfFullBottles + numberOfEmptyBottles; i++)
        {
            BottleController bottle = currentState[i].GetComponent<BottleController>();

            // each bottle must be either empty or filled with one single color
            if (bottle.numberOfColorsInBottle == 0)
            {
                continue;
            }
            else if (bottle.numberOfColorsInBottle == Constants.MAX_NUMBER_OF_COLORS_IN_BOTTLE && bottle.topColorLayers == Constants.MAX_NUMBER_OF_COLORS_IN_BOTTLE)
            {
                continue;
            }
            else
            {
                return false;
            }

        }

        return true;
    }

    private void adjustSecondRowSpacing()
    {
        int numberOfBottlesOnFirstRow = (numberOfFullBottles + 2 + 1) / 2;
        int numberOfBottlesOnSecondRow = numberOfFullBottles + numberOfEmptyBottles - numberOfBottlesOnFirstRow;
        float secondInteval = calculateInterval(numberOfBottlesOnSecondRow);

        for (int i = numberOfBottlesOnFirstRow; i < numberOfFullBottles + numberOfEmptyBottles; i++)
        {
            currentState[i].transform.position = new Vector3(Constants.LEFT_X + (i - numberOfBottlesOnFirstRow) * secondInteval, Constants.SECOND_ROW_Y, 0);
        }
    }

    public void OpenGameOverScene()
    {
        SceneManager.LoadScene("GameOverScene");
    }
}
