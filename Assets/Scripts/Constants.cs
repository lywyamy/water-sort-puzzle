using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Constants : MonoBehaviour
{
    public const int EASY_NUMBER_OF_BOTTLES = 6;
    public const int MEDIUM_NUMBER_OF_BOTTLES = 9;
    public const int HARD_NUMBER_OF_BOTTLES = 12;
    public const int NUMBER_OF_EMPTY_BOTTLES = 2;
    public const int MAX_NUMBER_OF_COLORS_IN_BOTTLE = 4;
    public const int SEED = 3722;

    // Basic colors for easy level
    public static Color RED = new Color(0.796f, 0.192f, 0.165f); // CB312A
    public static Color ORANGE = new Color(0.918f, 0.58f, 0.294f); // EA944B
    public static Color PINK = new Color(0.922f, 0.396f, 0.502f); // EB6580
    public static Color TEAL = new Color(0.412f, 0.855f, 0.514f); // 69DA83
    public static Color SKYBLUE = new Color(0.349f, 0.659f, 0.902f); // 59A8E6
    public static Color PURPLE = new Color(0.482f, 0.184f, 0.608f); // 7B2F9B

    // Additional colors for medium level
    public static Color NAVYBLUE = new Color(0.267f, 0.208f, 0.792f); // 4435CA
    public static Color GRAY = new Color(0.416f, 0.431f, 0.443f); // 6A6E71
    public static Color GREEN = new Color(0.498f, 0.612f, 0.071f); // 7F9C12

    // Aditional colors for hard level
    public static Color YELLOW = new Color(0.949f, 0.867f, 0.376f); // F2DD60
    public static Color DARKGREEN = new Color(0.09f, 0.431f, 0.239f); // 176E3D
    public static Color BROWN = new Color(0.525f, 0.325f, 0.035f); // 865309
}
