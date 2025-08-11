using Data;
using Ui;
using UnityEngine;

namespace NPC
{
    public class InfoGiver : NpcBehaviour
    {
        [Header("[Ref & Setting]")]
        [SerializeField] private MaterialStudy materialStudy;


        public override void Interact()
        {
            base.Interact();

            MaterialSlideshowUI.Instance.StartMaterial(materialStudy);
        }
        
    }
}


