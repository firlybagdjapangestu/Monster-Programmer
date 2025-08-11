using Manager;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }

        [Header("[Ref & Setting]")]
        [SerializeField] private NotifTwoButton notifTwoButton;

        private List<GameObject> panelOpenList = new List<GameObject>();

        [Serializable]
        private struct NotifTwoButton
        {
            public GameObject panel;
            public TextMeshProUGUI textUi;
            public Button buttonLeft;
            public Button buttonRight;
        }

        public class NotifButton
        {
            public string text;
            public Action action;

            public NotifButton(string _text, Action _callback)
            {
                text = _text;
                action = _callback;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Hapus jika sudah ada instance lain
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // Tetap hidup di scene berikutnya
        }

        public void SetNotifTwoButton(string _msg, NotifButton _left, NotifButton _right)
        {
            if (notifTwoButton.panel == null)
                return;

            //set text
            notifTwoButton.textUi.text = _msg;
            notifTwoButton.buttonLeft.GetComponentInChildren<TextMeshProUGUI>()?.SetText(_left.text);
            notifTwoButton.buttonRight.GetComponentInChildren<TextMeshProUGUI>()?.SetText(_right.text);

            //apply action
            ApplyActionToButton(
                notifTwoButton.buttonLeft,
                () =>
                {
                    _left.action.Invoke();
                }
                );

            ApplyActionToButton(
                notifTwoButton.buttonRight,
                () =>
                {
                    _right.action.Invoke();
                }
                );

            //active panel
            notifTwoButton.panel.SetActive(true);
            panelOpenList.Add(notifTwoButton.panel);
        }

        public void CloseAllPanel()
        {
            foreach (var item in panelOpenList)
            {
                item.SetActive(false);
            }

            panelOpenList.Clear();
        }

        private void ApplyActionToButton(Button _btn, Action _action)
        {
            _btn.onClick.RemoveAllListeners();
            _btn.onClick.AddListener(
                () => _action.Invoke()
                );
            _btn.onClick.AddListener(
                () => SoundManager.Instance?.PlaySoundEffect(0)
                );

            

        }
    }
}

