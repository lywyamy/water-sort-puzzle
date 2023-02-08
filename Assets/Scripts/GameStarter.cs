using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    public void OpenGamePlayScene()
    {
        SceneManager.LoadScene("GamePlayScene");
    }
}
