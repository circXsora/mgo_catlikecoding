using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {

    public void SwitchScene()
    {
        var index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene((index + 1) % SceneManager.sceneCountInBuildSettings);
    }
}
