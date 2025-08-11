using UnityEngine;

namespace Manager.Spawner 
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        [Header("[Reference]")]
        [SerializeField] private GameObject playerObject;
        [SerializeField] private SceneController[] portals = new SceneController[0];

        private static bool doTeleport;
        private static int targetIndex;

        private static bool doReposition;
        private static Vector2 lastPlayerPos;

        private void Start()
        {
            RepositionPlayer();
            RepositionPlayerAfterFight();
        }

        private void RepositionPlayer()
        {
            if (!doTeleport)
                return;

            //find teleport target
            if (portals.Length <= 0)
                portals = FindObjectsByType<SceneController>(FindObjectsSortMode.None);
            
            SceneController targetPortal = null;

            for (int i = 0; i < portals.Length; i++)
            {
                if (portals[i].portalIndex == targetIndex)
                {
                    targetPortal = portals[i];
                    break;
                }
            }

            if (targetPortal == null || playerObject == null)
                return;

            //reposition player
            playerObject.transform.position = targetPortal.spawnPos.position;

            doTeleport = false;
        }

        private void RepositionPlayerAfterFight()
        {
            if (!doReposition)
                return;

            if (playerObject == null)
                return;

            //reposition player
            playerObject.transform.position = lastPlayerPos;

            doReposition = false;
        }

        public static void SetPortalTeleport(int targetIndexPortal)
        {
            targetIndex = targetIndexPortal;
            doTeleport = true;
        }

        public static void RepositionPlayer(Vector2 _lastPost)
        {
            lastPlayerPos = _lastPost;
            doReposition = true;
        }
    }
}


