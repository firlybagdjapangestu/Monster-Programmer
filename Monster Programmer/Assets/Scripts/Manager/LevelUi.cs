using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class LevelUi : MonoBehaviour
    {
        [Header("[Reference]")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Image imageCurrentXp;

        [Header("Ref : list Benefit")]
        [SerializeField] private GameObject benefitTemplate;
        [SerializeField] private Transform spawnParent;

        LevelSystem lvMg => LevelSystem.Instance;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            LevelSystem.Instance?.CheckLevelUp(OnLevelUp);
            ListBenefit();
        }

        private void OnLevelUp(bool levelUp)
        {
            if (levelUp)
            {
                Debug.Log("Player Level Up");
            }

            Debug.Log("Player Xp : " + LevelSystem.Instance.currentXp + "/" + LevelSystem.Instance.NeedXp() + " : " + (LevelSystem.Instance.currentXp/ LevelSystem.Instance.NeedXp()));

            RefreshUiLevel();
        }

        private void RefreshUiLevel()
        {
            imageCurrentXp.fillAmount = (float)LevelSystem.Instance.currentXp / (float)LevelSystem.Instance.NeedXp();
            levelText.text = "Level " + GameData.Instance.level.ToString();
        }

        private void ListBenefit()
        {
            DestroyAllChildren(spawnParent);
            int len = lvMg.AllUpgrades.Length;

            GameObject g;

            for (int i = 0; i < len; i++)
            {
                int index = i;
                g = Instantiate(benefitTemplate, spawnParent);

                g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    (index + 1).ToString();

                g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    "Lvl. " + (index + 1).ToString() + " |";

                if (index == 0)
                {
                    g.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "-";
                }
                else
                {
                   g.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    $"{GetVisualText(lvMg.AllUpgrades[index].Upgrade)} : +{lvMg.AllUpgrades[index].value * 100}%";
                }

               

                g.transform.GetChild(2).gameObject.SetActive(index >= LevelSystem.Instance.Level);

            }
        }

        private string GetVisualText(LevelSystem.TypeUpgrade _up)
        {
            switch (_up)
            {
                case LevelSystem.TypeUpgrade.PlusAttackPercent: return $"Attack";
                case LevelSystem.TypeUpgrade.PlusDefendPercent: return $"Defend";
                case LevelSystem.TypeUpgrade.PlusChanceRarePercent: return $"Chance Capture Monster Rare";
                case LevelSystem.TypeUpgrade.PlusChanceLegenadryPercent: return $"Chance Capture Monster Legendary";
                default: return "-";
            }
        }

        public void DestroyAllChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
    }
}


