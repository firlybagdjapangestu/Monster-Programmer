using Manager.Spawner;
using Ui;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour, IInteractable
{
    [Header("[Reference]")]
    public Transform spawnPos;

    [Header("[Setting]")]
    public int portalIndex;
    public int portalTargetIndex;

    [Space(7f)]
    public string sceneName;
    
    public void Interact()
    {
        //apply player teleport
        PlayerSpawnPoint.SetPortalTeleport(portalTargetIndex);

        Debug.Log("Pindah Scene");
        LoadingManager.Instance.LoadingNewScene(sceneName);
    }
}
