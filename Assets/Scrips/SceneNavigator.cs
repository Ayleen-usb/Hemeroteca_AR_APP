using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToRecorrido()
    {
        LoadScene("06-Recorrido");
    }

    public void GoToMenu()
    {
        LoadScene("00-Inicio de sesion");
    }

    public void GoBack()
    {
        int index = SceneManager.GetActiveScene().buildIndex - 1;
        if (index >= 0)
            SceneManager.LoadScene(index);
    }
}