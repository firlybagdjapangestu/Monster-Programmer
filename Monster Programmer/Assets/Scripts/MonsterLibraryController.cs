using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class MonsterLibraryController : MonoBehaviour
{
    [SerializeField] private Button[] monsterButton;
    [SerializeField] private Image[] monsterImage;
    [SerializeField] private TextMeshProUGUI[] monsterNameText;

    [SerializeField] private Image monsterImageSelected;
    [SerializeField] private TextMeshProUGUI monsterNameTextSelected;
    [SerializeField] private Slider statusSpeedMonster;
    [SerializeField] private Slider statusAttackMonster;
    [SerializeField] private Slider statusDefenseMonster;

    [SerializeField] private float topSpeed;
    [SerializeField] private float topAttack;
    [SerializeField] private float topDefense;

    [SerializeField] private Button confirmationButton;


    [SerializeField] private Button commonButton;
    [SerializeField] private Button rareButton;
    [SerializeField] private Button legendaryButton;

    private void Start()
    {
        // Load default category, bisa ganti sesuai UI buttonmu  
        ShowMonstersByRarity(Rarity.Common);

        // Setup button listeners for rarity buttons  
        commonButton.onClick.AddListener(() => ShowMonstersByRarity(Rarity.Common));
        rareButton.onClick.AddListener(() => ShowMonstersByRarity(Rarity.Rare));
        legendaryButton.onClick.AddListener(() => ShowMonstersByRarity(Rarity.Legendary));
    }

    public void ShowMonstersByRarity(Rarity rarity)
    {
        if (GameData.Instance == null || GameData.Instance.availableMonsters == null)
        {
            Debug.LogError("GameData or availableMonsters is not set.");
            return;
        }

        MonsterData[] filtered = GameData.Instance.availableMonsters
            .Where(m => m.rarity == rarity)
            .ToArray();

        SetupMonsterInfo(filtered);
    }





    private void SetupMonsterInfo(MonsterData[] monsters)
    {
        for (int i = 0; i < monsters.Length && i < monsterButton.Length; i++)
        {
            int index = i;
            monsterImage[i].sprite = monsters[i].frontSpriteMonster;
            monsterNameText[i].text = monsters[i].monsterName;

            bool isOwned = GameData.Instance.ownedMonsters.Exists(m => m.monsterID == monsters[i].monsterID);
            monsterButton[i].interactable = isOwned;

            monsterButton[i].onClick.RemoveAllListeners();
            if (isOwned)
            {
                monsterButton[i].onClick.AddListener(() => ChoiceMonster(monsters[index]));
            }
        }

        // Clear unused buttons
        for (int i = monsters.Length; i < monsterButton.Length; i++)
        {
            monsterImage[i].sprite = null;
            monsterNameText[i].text = string.Empty;
            monsterButton[i].onClick.RemoveAllListeners();
            monsterButton[i].interactable = false;
        }
    }

    public void ChoiceMonster(MonsterData monsterData)
    {
        monsterImageSelected.sprite = monsterData.frontSpriteMonster;
        monsterNameTextSelected.text = monsterData.monsterName;
        statusSpeedMonster.value = monsterData.speed / topSpeed;
        statusAttackMonster.value = monsterData.attack / topAttack;
        statusDefenseMonster.value = monsterData.defense / topDefense;

        confirmationButton.onClick.RemoveAllListeners();
        confirmationButton.onClick.AddListener(() => ConfirmationChoiceMonster(monsterData.monsterID));
    }

    public void ConfirmationChoiceMonster(string id)
    {
        GameData.Instance.SelectPlayerMonster(id);
    }
}
