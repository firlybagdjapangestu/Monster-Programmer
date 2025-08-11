using System.Collections;
using TMPro;
using UnityEngine;


namespace Ui 
{
    public class CurrencyUiManager : MonoBehaviour
    {
        [Header("[Ref]")]
        [SerializeField] private TextMeshProUGUI currencyText;

        public static bool AddCoinOnStart;
        public static int CoinOnStart;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GameData.Instance.OnDataChange += UpdateCurrencyUi;
            StartCoroutine(DelayAction());
            AddCoinStart();
        }

        private void OnDisable()
        {
            GameData.Instance.OnDataChange -= UpdateCurrencyUi;
        }

        private void AddCoinStart()
        {
            if (!AddCoinOnStart)
                return;

            // UI effect
        }

        public static void AddSomeCoin(int coins)
        {
            CoinOnStart = coins;
            AddCoinOnStart = true;
        }

        public void EarnSomeCoins()
        {
            UpdateCurrencyUi();
        }

        public void UpdateCurrencyUi()
        {
            currencyText.text = GameData.Instance.Coins.ToString();
        }

        IEnumerator DelayAction()
        {
            yield return new WaitForEndOfFrame();

            yield return new WaitForSeconds(0.01f);

            UpdateCurrencyUi();
        }
    }

}

