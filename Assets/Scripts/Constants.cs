using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public const int EASY_NUMBER_OF_BOTTLES = 6;
    public const int MEDIUM_NUMBER_OF_BOTTLES = 9;
    public const int HARD_NUMBER_OF_BOTTLES = 12;
    public const int NUMBER_OF_EMPTY_BOTTLES = 2;

    // Basic colors for easy level
    public static Color red;
    public static Color orange;
    public static Color pink;
    public static Color teal;
    public static Color skyBlue;
    public static Color purple;

    // Additional colors for medium level
    public static Color navyBlue;
    public static Color gray;
    public static Color green;

    // Aditional colors for hard level
    public static Color yellow;
    public static Color darkGreen;
    public static Color brown;

    public Color[] colors = { red, orange, pink, teal, skyBlue, purple, navyBlue, gray, green, yellow, darkGreen, brown };
}
