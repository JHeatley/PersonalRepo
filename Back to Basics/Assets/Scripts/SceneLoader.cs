using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string levelToLoad;

    public void LoadScene()
    {
        SceneManager.LoadSceneAsync(levelToLoad);
    }

    public void QuitGame()
    {
        Debug.Log("Game is quitting. Bye!");
        Application.Quit();
    }

    public void SaveGame()
    {
        GameEvents.OnSaveInitiated();
    }

    public void DeleteAllSaves()
    {
        SaveManager.DeleteAllSaves();
    }
}
