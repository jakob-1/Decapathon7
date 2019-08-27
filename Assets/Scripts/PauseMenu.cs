using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public bool paused = false;

    private GameObject pauseMenuObj;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenuObj = GameObject.FindGameObjectWithTag("PauseMenu");
        pauseMenuObj.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        menuKeyBoardControl();
    }

    void menuKeyBoardControl()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                unpauseGame();
            }
            else
            {
                pauseGame();
            }
        }
    }

    public void resumeButton()
    {
        unpauseGame();
    }

    public void pauseGame()
    {
        Time.timeScale = 0;
        paused = true;
        pauseMenuObj.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void unpauseGame()
    {
        Time.timeScale = 1;
        paused = false;
        pauseMenuObj.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void quit()
    {
        Application.Quit();
    }
}