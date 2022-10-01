using UnityEngine;
using UnityEngine.SceneManagement;
public class RestartScene : MonoBehaviour
{
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
