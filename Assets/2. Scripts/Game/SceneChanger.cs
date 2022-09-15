using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void LoadIntroScene(float delay)
    {
        Invoke(nameof(PrivateLoadIntroScene), delay);
    }

    private void PrivateLoadIntroScene()
    {
        SceneManager.LoadScene("Intro");
    }
}
