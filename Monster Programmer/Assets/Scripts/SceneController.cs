using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour, IInteractable
{
    public string sceneName;
    public void Interact()
    {
        Debug.Log("Pindah Scene");
        SceneManager.LoadScene(sceneName);
    }
}
