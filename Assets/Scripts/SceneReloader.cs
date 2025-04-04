using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 重新加载当前场景
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}