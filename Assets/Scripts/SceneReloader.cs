using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    [Header("要加载的场景名称")]
    public string targetSceneName = "SIUSIUBOOM封面"; // 把你的开始界面场景名写在这

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}