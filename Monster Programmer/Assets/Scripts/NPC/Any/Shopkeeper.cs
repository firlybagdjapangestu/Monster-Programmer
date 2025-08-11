using Ui;
using UnityEngine;

namespace NPC
{
    public class Shopkeeper : NpcBehaviour, IInteractable
    {
        [Header("[Ref & Setting]")]
        [SerializeField] private string notifMsg = "";
        [SerializeField] private int pokeballCount = 5;
        [SerializeField] private int pokebalPrice = 50;


        public override void Interact()
        {
            base.Interact();

            NotificationManager.NotifButton leftButton = new NotificationManager.NotifButton("No",
                () => OnBuyPokeball(false)
                );

            NotificationManager.NotifButton rightButton = new NotificationManager.NotifButton("Yes",
                () => OnBuyPokeball(true)
                );

            //active panel two notif
            NotificationManager.Instance.SetNotifTwoButton(
                notifMsg,
                leftButton,
                rightButton
                );
        }


        private void OnBuyPokeball(bool status)
        {
            if (!status)
            {
                NotificationManager.Instance.CloseAllPanel();
                return;
            }

            //try buy item
            if (!IsMoneyEnough(pokebalPrice))
            {
                Debug.Log("Not Enough Coins");
                return;
            }

            //buy item
            Debug.Log("Succees Buy");
            GameData.Instance.captureBalls += pokeballCount;
            GameData.Instance.Coins -= pokebalPrice;
            GameData.Instance.SaveGame();

            //disable panel
            NotificationManager.Instance.CloseAllPanel();
        }

        private bool IsMoneyEnough(int coins)
        {
            return coins <= GameData.Instance.Coins;
        }
    }

}
    
