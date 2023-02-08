using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{

    public void StartEasyLevel()
    {
        BottleSpawner.numberOfFullBottles = Constants.EASY_NUMBER_OF_BOTTLES;
        SceneManager.LoadScene("GamePlayScene");
    }

    public void StartMediumLevel()
    {
        BottleSpawner.numberOfFullBottles = Constants.MEDIUM_NUMBER_OF_BOTTLES;
        SceneManager.LoadScene("GamePlayScene");
    }

    public void StartHardLevel()
    {
        BottleSpawner.numberOfFullBottles = Constants.HARD_NUMBER_OF_BOTTLES;
        SceneManager.LoadScene("GamePlayScene");
    }
}
