using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BattleController : MonoBehaviour
{
    [Header("Player Monster")]
    [SerializeField] private Image playerMonster;
    [SerializeField] private TextMeshProUGUI playerMonsterName;
    [SerializeField] private Slider playerHpSlider;
    [SerializeField] private float speedPlayer;
    [SerializeField] private float attackPlayer;
    [SerializeField] private float defensePlayer;
    [SerializeField] private MonsterData selectedMonsterPlayer;

    [Header("Enemy Monster")]
    [SerializeField] private Image enemyMonster;
    [SerializeField] private TextMeshProUGUI enemyMonsterName;
    [SerializeField] private Slider enemyHpSlider;
    [SerializeField] private float speedEnemy;
    [SerializeField] private float attackEnemy;
    [SerializeField] private float defenseEnemy;
    [SerializeField] private MonsterData selectedMonsterEnemy;

    [Header("Battle UI")]
    [SerializeField] private int currentQuestionIndex = 0; // Index of the current question
    [SerializeField] private Button attackButton; // Button to trigger the attack action
    [SerializeField] private Button captureButton; // Button to trigger the capture action
    [SerializeField] private Button runButton; // Button to trigger the run action
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private Image question;
    [SerializeField] private GameObject answerPanel;
    [SerializeField] private Image[] answerOptions = new Image[4];

    [Header("Timer")]
    [SerializeField] private float timerDuration = 10f; // Duration of the timer in seconds
    [SerializeField] private float timer; // Timer value
    [SerializeField] private TextMeshProUGUI timerText; // UI Text to display the timer

    void Start()
    {
        SetPlayerMonster();
        SetEnemyMonster();
        AddListenerToButton();
        CheckCaptureBall(); 
    }

    private void AddListenerToButton()
    {
        attackButton.onClick.AddListener(Attack);
        captureButton.onClick.AddListener(UseCaptureBall);
        runButton.onClick.AddListener(() => Debug.Log("Run action triggered!"));
    }

    public void SetPlayerMonster()
    {
        
        Debug.Log($"Monster selected: {GameData.selectedPlayerMonster.monsterName}");
        selectedMonsterPlayer = GameData.selectedPlayerMonster;

        // For Testing
        /*GameData.Instance.SelectPlayerMonster("Encapsulation-Cp-B");*/

        // Set the player monster sprite and name  
        playerMonster.sprite = selectedMonsterPlayer.backSpriteMonster;
        playerMonsterName.text = selectedMonsterPlayer.monsterName;
        playerHpSlider.maxValue = selectedMonsterPlayer.defense;
        playerHpSlider.value = selectedMonsterPlayer.defense;

        // Set the player monster stats  
        speedPlayer = selectedMonsterPlayer.speed;
        attackPlayer = selectedMonsterPlayer.attack;
        defensePlayer = selectedMonsterPlayer.defense;
    }

    public void UpdateStatusPlayer()
    {
        playerHpSlider.value = defensePlayer;
        if (playerHpSlider.value <= 0)
        {
            Debug.Log("Player defeated!");
            GameData.Instance.ownedMonsters.Remove(selectedMonsterPlayer); // Remove the monster from the player's collection
            GameData.Instance.SaveGame(); // Save the game state
            // Handle player defeat logic here
        }
    }

    public void SetEnemyMonster()
    {
        // For Testing
        GameData.Instance.SelectEnemyMonster("Encapsulation-Cp-B");
        Debug.Log($"Monster selected: {GameData.selectedEnemyMonster.monsterName}");
        selectedMonsterEnemy = GameData.selectedEnemyMonster; // Get the selected enemy monster

        // Set the enemy monster sprite and name  
        enemyMonster.sprite = selectedMonsterEnemy.frontSpriteMonster;
        enemyMonsterName.text = selectedMonsterEnemy.monsterName;
        enemyHpSlider.maxValue = selectedMonsterEnemy.defense;
        enemyHpSlider.value = selectedMonsterEnemy.defense;

        // Set the enemy monster stats  
        speedEnemy = selectedMonsterEnemy.speed;
        attackEnemy = selectedMonsterEnemy.attack;
        defenseEnemy = selectedMonsterEnemy.defense;
    }

    public void UpdateStatusEnemy()
    {
        enemyHpSlider.value = defenseEnemy;
        if (enemyHpSlider.value <= 0)
        {
            Debug.Log("Enemy defeated!");
            // Handle enemy defeat logic here
        }
    }


    public void Attack()
    {
        StartTimer(); // Start the timer for the question
        if (currentQuestionIndex >= selectedMonsterEnemy.questionData.Count)
        {
            Debug.Log("No more questions available.");
            return;
        }
        question.gameObject.SetActive(true);
        answerPanel.SetActive(true);
        actionPanel.SetActive(false);

        question.sprite = selectedMonsterEnemy.questionData[currentQuestionIndex].questionText; // Set the question text  
        for (int i = 0; i < answerOptions.Length; i++)
        {
            answerOptions[i].sprite = selectedMonsterEnemy.questionData[currentQuestionIndex].answerOptions[i]; // Set the answer option images  
        }
        currentQuestionIndex++;
    }

    public void ConfirmationAttack(int answer)
    {
        if (currentQuestionIndex - 1 < 0 || currentQuestionIndex - 1 >= selectedMonsterEnemy.questionData.Count)
        {
            Debug.Log("Invalid question index.");
            return;
        }

        int correctAnswerIndex = selectedMonsterEnemy.questionData[currentQuestionIndex - 1].correctAnswerIndex;
        if (answer == correctAnswerIndex)
        {
            Debug.Log("Correct answer!");
            defenseEnemy -= attackPlayer; // Reduce enemy defense by player attack
            UpdateStatusEnemy();

        }
        else
        {
            Debug.Log("Wrong answer!");
            defensePlayer -= attackEnemy; // Reduce player defense by enemy attack
            UpdateStatusPlayer();
        }
        
        question.gameObject.SetActive(false);
        actionPanel.SetActive(true);
        answerPanel.SetActive(false);
    }

    public void CheckCaptureBall()
    {
        captureButton.interactable = GameData.Instance.captureBalls > 0;
    }

    public void UseCaptureBall()
    {
        if (GameData.Instance.captureBalls > 0)
        {
            GameData.Instance.captureBalls--;
            CaptureMonster();
        }
        else
        {
            Debug.Log("No capture balls left!");
        }

        captureButton.interactable = GameData.Instance.captureBalls > 0;
    }

    private void CaptureMonster()
    {
        float baseChance = GetBaseCaptureChance(selectedMonsterEnemy.rarity);
        float bonusFromDefense = GetBonusFromDefense(selectedMonsterEnemy);
        float finalChance = Mathf.Clamp01(baseChance + bonusFromDefense);

        float roll = Random.Range(0f, 1f);

        Debug.Log($"Trying to capture {selectedMonsterEnemy.monsterName} (Rarity: {selectedMonsterEnemy.rarity})");
        Debug.Log($"Base Chance: {baseChance * 100}%");
        Debug.Log($"Bonus from Defense: {bonusFromDefense * 100}%");
        Debug.Log($"Final Chance: {finalChance * 100}% | Rolled: {roll * 100}%");

        if (roll <= finalChance)
        {
            GameData.Instance.ownedMonsters.Add(selectedMonsterEnemy);
            GameData.Instance.SaveGame();
            Debug.Log("Capture successful!");
        }
        else
        {
            Debug.Log("Capture failed!");
        }
    }

    private float GetBaseCaptureChance(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return 0.6f;      // 60%
            case Rarity.Rare: return 0.3f;        // 30%
            case Rarity.Legendary: return 0.1f;   // 10%
            default: return 0.3f;
        }
    }

    private float GetBonusFromDefense(MonsterData monster)
    {
        float currentDefense = enemyHpSlider.value;        // Ambil dari slider HP musuh
        float maxDefense = enemyHpSlider.maxValue;

        float damageTakenRatio = 1f - (currentDefense / maxDefense);
        float bonusFactor = 0.3f; // 30% max bonus dari HP yang kecil

        return damageTakenRatio * bonusFactor;
    }

    public void StartTimer()
    {
        timer = timerDuration; // Reset the timer to its duration  
        Timer(); // Start the timer countdown  
    }


    private void Timer()
    {
        float speedFactor = Mathf.Clamp(speedEnemy - speedPlayer, -30f, 30f); // Calculate speed difference, clamped to avoid extreme values  
        timer = Mathf.Max(1f, timerDuration - speedFactor); // Adjust timer duration based on speed difference, minimum 1 second  
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime; // Decrease the timer by the time elapsed  
            timerText.text = Mathf.Ceil(timer).ToString(); // Update the timer text  
            yield return null; // Wait for the next frame  
        }

        // Timer has run out, handle unanswered question  
        Debug.Log("Time's up! Player failed to answer.");
        defensePlayer -= attackEnemy; // Reduce player defense by enemy attack  
        UpdateStatusPlayer();

        // Reset UI  
        question.gameObject.SetActive(false);
        actionPanel.SetActive(true);
        answerPanel.SetActive(false);
    }


    private IEnumerator PlayAnimation(string animationName)
    {
        // Play the animation here
        // Example: animator.SetTrigger(animationName);
        yield return new WaitForSeconds(1f); // Wait for the animation to finish
    }

}
