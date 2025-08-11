using System;
using System.Collections;
using Ui;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager 
{
    public class FightSceneManager : MonoBehaviour
    {
        public static FightSceneManager instance;

        [Header("[Reference]")]
        public BattleController battleController;
        public GameObject PanelSuccesedCapture;
        public GameObject PanelFailCapture;
        public GameObject PanelWin;
        public GameObject PanelLose;
        public GameObject PanelRun;

        [Header("[Reward]")]
        [SerializeField] private int[] rewardList = { 50, 100, 200 };

        public bool GameEnd { get; private set; } = false;

        public static string LastWorld { get; set; }

        private void OnEnable()
        {
            battleController.OnFightEnd += BattleController_OnFightEnd;
        }

        private void OnDisable()
        {
            battleController.OnFightEnd -= BattleController_OnFightEnd;
        }


        private void Awake()
        {
            instance = this;

            GameEnd = false;
        }

        #region Role

        private void BattleController_OnFightEnd(FightEndType obj)
        {
            battleController.StopAllCoroutines();

            switch (obj)
            {
                case FightEndType.Win:
                    EndGame(true);
                    break;
                case FightEndType.Lose:
                    EndGame(false);
                    break;
                case FightEndType.Capture:
                    SetCapture(true);
                    break;
                case FightEndType.Run:
                    RunFromFight();
                    break;
                default:
                    RunFromFight();
                    break;
            }
        }

        public void SetCapture(bool succesed)
        {
            if (succesed)
            {
                if (GameEnd)
                    return;

                GameEnd = true;

                PanelSuccesedCapture.SetActive(true);
                StartCoroutine(DelayAction(
                    () =>
                    {
                        BackToGameplay();
                        AddReward();

                    }
                    , 2
                    ));
                return;
            }

            PanelFailCapture.SetActive(true);
            StartCoroutine(DelayAction(
                () => PanelFailCapture.SetActive(false), 
                1f
                ));
        }

        public void EndGame(bool status)
        {
            if (GameEnd)
                return;

            GameEnd = true;

            if (status)
            {
                PanelWin.SetActive(true);
                StartCoroutine(DelayAction(
                    () =>
                    {
                        BackToGameplay();
                        AddReward();
                    }
                    , 2f
                    ));
            }
            else
            {
                PanelLose.SetActive(true);
                StartCoroutine(DelayAction(
                    () =>
                    {
                        BackToGameplay();
                    }
                    , 2f
                    ));
            }
        }

        public void RunFromFight()
        {
            if (GameEnd)
                return;

            GameEnd = true;

            PanelRun.SetActive(true);
            StartCoroutine(DelayAction(
                () =>
                {
                    BackToGameplay();
                }
                , 2f
                ));
        }

        #endregion

        private void AddReward()
        {
            int _reward = 0;
            switch (battleController.selectedMonsterEnemy.rarity)
            {
                case Rarity.Common:
                    _reward = rewardList[0];
                    break;
                case Rarity.Rare:
                    _reward = rewardList[1];

                    break;
                case Rarity.Legendary:
                    _reward = rewardList[2];

                    break;
                default:
                    break;
            }

            //add reward
            LevelSystem.Instance.AddXpBattle();
            CurrencyUiManager.AddSomeCoin(_reward);
            GameData.Instance.Coins += _reward;
            GameData.Instance.SaveGame();
        }

        private void BackToGameplay()
        {
            SceneManager.LoadScene(LastWorld);
        }

        private IEnumerator DelayAction(Action _func, float delay)
        {
            yield return new WaitForSeconds(delay);

            _func?.Invoke();
        }
    }

}


