using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    // 调用这个方法并传入场景名称，即可加载指定场景
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

   
    // 可选：退出游戏
    public void QuitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}