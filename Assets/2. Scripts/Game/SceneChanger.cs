using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
    }

    public void LoadIntroScene(float delay)
    {
        Invoke(nameof(PrivateLoadIntroScene), delay);
    }

    private void PrivateLoadIntroScene()
    {
        SceneManager.LoadScene("Intro");
    }
}
