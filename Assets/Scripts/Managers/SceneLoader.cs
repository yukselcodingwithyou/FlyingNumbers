using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string startSceneName = "Start";
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private string demoSceneName = "Demo";

    public void LoadStart() => SceneManager.LoadScene(startSceneName);
    public void LoadGame() => SceneManager.LoadScene(gameSceneName);
    public void LoadDemo() => SceneManager.LoadScene(demoSceneName);
    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
