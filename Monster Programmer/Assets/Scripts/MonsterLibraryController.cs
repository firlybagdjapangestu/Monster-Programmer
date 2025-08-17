using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Manager;

public class MonsterLibraryController : MonoBehaviour
{
    [Header("[Reference]")]
    [SerializeField] private GameObject buttonTemplate;
    [SerializeField] private Transform parentSpawn;

    [Header("[Another]")]
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

        ChoiceMonster(GameData.Instance.defaultMonster);
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
        if (GameData.Instance.ownedMonsters == null || GameData.Instance.ownedMonsters.Count <= 0)
        {
            GameData.Instance.SetDefaultMonster();
        }

        DestroyAllChildren(parentSpawn);
        GameObject g;
        Button actionButton;

        for (int i = 0; i < monsters.Length; i++)
        {
            int index = i;
            g = Instantiate(buttonTemplate, parentSpawn);

            g.transform.GetChild(0).GetComponent<Image>().sprite = monsters[i].frontSpriteMonster;
            g.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = monsters[i].monsterName;

            bool isOwned = GameData.Instance.ownedMonsters.Exists(m => m.monsterID == monsters[i].monsterID);
            actionButton = g.GetComponent<Button>();
            actionButton.interactable = isOwned;

            actionButton.onClick.RemoveAllListeners();
            if (isOwned)
            {
                actionButton.onClick.AddListener(() => ChoiceMonster(monsters[index]));
                actionButton.onClick.AddListener(() => SoundManager.Instance?.PlaySoundEffect(0));
            }
        }

    }

    public void ChoiceMonster(MonsterData monsterData)
    {
        monsterImageSelected.sprite = monsterData.frontSpriteMonster;
        monsterNameTextSelected.text = monsterData.monsterName;
        statusSpeedMonster.value = monsterData.speed / topSpeed;
        statusAttackMonster.value = 
            (monsterData.attack * LevelSystem.Instance.GetMultiplerValue(LevelSystem.TypeUpgrade.PlusAttackPercent)) / topAttack;
        statusDefenseMonster.value = 
            (monsterData.defense * LevelSystem.Instance.GetMultiplerValue(LevelSystem.TypeUpgrade.PlusDefendPercent)) / topDefense;

        confirmationButton.onClick.RemoveAllListeners();
        confirmationButton.onClick.AddListener(() => ConfirmationChoiceMonster(monsterData.monsterID));
    }

    public void ConfirmationChoiceMonster(string id)
    {
        GameData.Instance.SelectPlayerMonster(id);
    }

    public void DestroyAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
