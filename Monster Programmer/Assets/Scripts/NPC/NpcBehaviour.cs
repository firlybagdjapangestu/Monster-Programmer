using UnityEngine;

namespace NPC
{
    public class NpcBehaviour : MonoBehaviour, IInteractable
    {
        [Header("[Identity]")]
        public string NpcName;

        public virtual void Interact()
        {
            Debug.Log($"Interact with npc : {NpcName}");
        }
    }
}

