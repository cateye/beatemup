using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //limit gameplay to 60 FPS. to avoid battery hog
        Application.targetFrameRate = 60;  
    }

    public void GoToGame()
    {
        SceneManager.LoadScene("Game");
    }
}
