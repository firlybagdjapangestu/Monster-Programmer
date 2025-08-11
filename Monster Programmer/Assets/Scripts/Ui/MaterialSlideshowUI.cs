using Data;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui 
{
    public class MaterialSlideshowUI : MonoBehaviour
    {
        public static MaterialSlideshowUI Instance;

        [Header("UI Elements")]
        [SerializeField] private GameObject panelMaterial;
        [SerializeField] private TextMeshProUGUI subjectText; // atau Text jika pakai UI Text biasa
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;

        [Header("Material Study")]
        [SerializeField] private MaterialStudy studyData;

        private int currentIndex = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Hapus jika sudah ada instance lain
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            previousButton.onClick.AddListener(OnPrevious);
            nextButton.onClick.AddListener(OnNext);
        }

        public void StartMaterial(MaterialStudy _material)
        {
            

            studyData = _material;

            if (studyData == null || studyData.AllSubject.Length == 0)
            {
                subjectText.text = "No content available.";
                previousButton.interactable = false;
                nextButton.interactable = false;
                return;
            }

            currentIndex = 0;

            UpdateUI();

            panelMaterial.SetActive(true);

            VideoManager.Instance.ChangeVideo(_material.Clip);
            VideoManager.Instance.StartVideo();
        }

        private void OnPrevious()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                UpdateUI();
                SoundManager.Instance?.PlaySoundEffect(0);

            }
        }

        private void OnNext()
        {
            if (currentIndex < studyData.AllSubject.Length - 1)
            {
                currentIndex++;
                UpdateUI();
                SoundManager.Instance?.PlaySoundEffect(0);

            }
        }

        private void UpdateUI()
        {
            subjectText.text = studyData.AllSubject[currentIndex];

            // Update button interactability
            previousButton.interactable = currentIndex > 0;
            nextButton.interactable = currentIndex < studyData.AllSubject.Length - 1;
        }
    }
}



