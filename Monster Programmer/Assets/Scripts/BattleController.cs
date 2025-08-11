using Manager;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum FightEndType
{
    Win,
    Lose,
    Capture,
    Run
}

public class BattleController : MonoBehaviour
{
    [Header("[Ref]")]
    [SerializeField] private TextMeshProUGUI pokeballText;

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
    [SerializeField] public MonsterData selectedMonsterEnemy;

    [Header("Battle UI")]
    [SerializeField] private int currentQuestionIndex = 0; // Index of the current question
    [SerializeField] private Button attackButton; // Button to trigger the attack action
    [SerializeField] private Button captureButton; // Button to trigger the capture action
    [SerializeField] private Button runButton; // Button to trigger the run action
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private GameObject answerPanel;
    [SerializeField] private TextMeshProUGUI[] answerOptions = new TextMeshProUGUI[4];

    [Header("Timer")]
    [SerializeField] private float timerDuration = 10f; // Duration of the timer in seconds
    [SerializeField] private float timer; // Timer value
    [SerializeField] private TextMeshProUGUI timerText; // UI Text to display the timer

    [Header("Sound")]
    [SerializeField] private AudioClip rightAnswer;
    [SerializeField] private AudioClip wrongAnswer;
    [SerializeField] private AudioClip[] swingAndHit;

    [Header("VFX")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 5f;

    private Color originalColor;
    private Coroutine flashRoutine;

    //action for battle contoller
    public event Action<FightEndType> OnFightEnd;

    void Start()
    {
        SetPlayerMonster();
        SetEnemyMonster();
        AddListenerToButton();
        CheckCaptureBall();
    }

    #region Initialize

    private void AddListenerToButton()
    {
        attackButton.onClick.AddListener(Attack);
        captureButton.onClick.AddListener(UseCaptureBall);
        runButton.onClick.AddListener(
            () => {
                Debug.Log("Run action triggered!");
                OnFightEnd?.Invoke(FightEndType.Run);
            }
            );
    }

    public void SetPlayerMonster()
    {

        Debug.Log($"Monster selected: {GameData.selectedPlayerMonster.monsterName}");
        selectedMonsterPlayer = GameData.selectedPlayerMonster;

        // For Testing
        /*GameData.Instance.SelectPlayerMonster("Encapsulation-Cp-B");*/

        // Set the player monster sprite and name
        SetPlayerMonsterSprite(playerMonster, selectedMonsterPlayer.frontSpriteMonster, selectedMonsterPlayer.backSpriteMonster);
        playerMonsterName.text = selectedMonsterPlayer.monsterName;
        playerHpSlider.maxValue = selectedMonsterPlayer.defense;
        playerHpSlider.value = selectedMonsterPlayer.defense;

        // Set the player monster stats  
        speedPlayer = selectedMonsterPlayer.speed;
        attackPlayer = CalculatorDamage.GetAttackDamage(
            selectedMonsterPlayer.attack * LevelSystem.Instance.GetMultiplerValue(LevelSystem.TypeUpgrade.PlusAttackPercent)
            );
        defensePlayer = selectedMonsterPlayer.defense * LevelSystem.Instance.GetMultiplerValue(LevelSystem.TypeUpgrade.PlusDefendPercent);
    }

    private void SetPlayerMonsterSprite(Image _image, Sprite _monsterFront, Sprite _monsterBack)
    {
        if (_monsterBack == null)
        {
            _image.sprite = _monsterFront;
        }
        else
        {
            _image.sprite = _monsterBack;
        }
    }

    private void SetEnemyMonsterSprite(Image _image, Sprite _monsterFront, Sprite _monsterBack)
    {
        _image.sprite = _monsterFront;

        if (_monsterBack == null)
        {
            _image.rectTransform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            _image.rectTransform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void UpdateStatusPlayer()
    {
        playerHpSlider.value = defensePlayer;
        if (defensePlayer <= 0)
        {
            Debug.Log("Player defeated!");
            GameData.Instance.ownedMonsters.Remove(selectedMonsterPlayer); // Remove the monster from the player's collection
            GameData.Instance.SaveGame(); // Save the game state
            // Handle player defeat logic here

            OnFightEnd?.Invoke(FightEndType.Lose);
        }
    }

    public void SetEnemyMonster()
    {
        // For Testing
        Debug.Log($"Monster selected: {GameData.selectedEnemyMonster.monsterName}");
        selectedMonsterEnemy = GameData.selectedEnemyMonster; // Get the selected enemy monster

        // Set the enemy monster sprite and name  
        SetEnemyMonsterSprite(enemyMonster, selectedMonsterEnemy.frontSpriteMonster, selectedMonsterEnemy.backSpriteMonster);
        enemyMonsterName.text = selectedMonsterEnemy.monsterName;
        enemyHpSlider.maxValue = selectedMonsterEnemy.defense;
        enemyHpSlider.value = selectedMonsterEnemy.defense;

        // Set the enemy monster stats  
        speedEnemy = selectedMonsterEnemy.speed;
        attackEnemy = CalculatorDamage.GetAttackDamage(selectedMonsterEnemy.attack);
        defenseEnemy = selectedMonsterEnemy.defense;
    }

    public void UpdateStatusEnemy()
    {
        enemyHpSlider.value = defenseEnemy;
        if (enemyHpSlider.value <= 0)
        {
            Debug.Log("Enemy defeated!");
            // Handle enemy defeat logic here
            OnFightEnd?.Invoke(FightEndType.Win);
        }
    }

    #endregion

    #region Attack & Capture

    public void Attack()
    {
        StartTimer(); // Start the timer for the question
        if (selectedMonsterEnemy.questionData.Count <= 0)
        {
            Debug.Log("No more questions available.");
            return;
        }

        bool doneRandom = false;

        int maxTry = 20;
        int tryCount = 0;
        do
        {
            currentQuestionIndex = Random.Range(0, selectedMonsterEnemy.questionData.Count);
            doneRandom = selectedMonsterEnemy.questionData[currentQuestionIndex].SpecializeType == selectedMonsterEnemy.type ||
                selectedMonsterEnemy.questionData[currentQuestionIndex].SpecializeType == MonsterType.None;

            tryCount++;

        } while (doneRandom == false && tryCount < maxTry);

        if (tryCount >= maxTry)
        {
            currentQuestionIndex = 0;
        }

        questionPanel.gameObject.SetActive(true);
        answerPanel.SetActive(true);
        actionPanel.SetActive(false);

        //set question
        questionPanel.GetComponentInChildren<TextMeshProUGUI>().text 
            = selectedMonsterEnemy.questionData[currentQuestionIndex].QuestionText; // Set the question text  

        for (int i = 0; i < answerOptions.Length; i++)
        {
            answerOptions[i].text = selectedMonsterEnemy.questionData[currentQuestionIndex].Answers[i]; // Set the answer option images  
        }
    }

    public void ConfirmationAttack(int answer)
    {
        if (selectedMonsterEnemy.questionData[currentQuestionIndex].CheckAnswer(answer))
        {
            Debug.Log("Correct answer!");
            defenseEnemy -= attackPlayer; // Reduce enemy defense by player attack
            EffectAttack(true);
            UpdateStatusEnemy();

        }
        else
        {
            Debug.Log("Wrong answer!");
            defensePlayer -= attackEnemy; // Reduce player defense by enemy attack
            EffectAttack(false);
            UpdateStatusPlayer();
        }
        
        questionPanel.gameObject.SetActive(false);
        actionPanel.SetActive(true);
        answerPanel.SetActive(false);

        if (currentTimer != null)
        {
            StopCoroutine(currentTimer);
            currentTimer = null;
        }
    }

    #region Effect Attack

    private Image targetImage;
    private Vector3 originalPosition;

    public void EffectAttack(bool _isRight)
    {
        targetImage = _isRight ? enemyMonster : playerMonster;
        SoundManager.Instance?.PlaySoundEffect(_isRight ? rightAnswer : wrongAnswer);

        if (targetImage != null)
            originalColor = targetImage.color;

        if (targetImage != null)
            originalPosition = targetImage.rectTransform.localPosition;

        if (targetImage == null) return;

        // Kalau sedang kedip, hentikan dulu biar tidak bentrok
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashAndShake());
    }

    private IEnumerator FlashAndShake()
    {
        SoundManager.Instance?.PlaySoundEffect(swingAndHit[0]);

        yield return new WaitForSeconds(flashDuration);

        // Ubah ke warna damage
        targetImage.color = damageColor;
        SoundManager.Instance?.PlaySoundEffect(swingAndHit[1]);

        // Mulai shake
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Random.Range(-1f, 1f) * shakeStrength;
            float offsetY = Random.Range(-1f, 1f) * shakeStrength;

            targetImage.rectTransform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

            yield return null;
        }

        // Balik posisi & warna
        targetImage.rectTransform.localPosition = originalPosition;
        yield return new WaitForSeconds(flashDuration);

        targetImage.color = originalColor;
    }

    #endregion

    public void CheckCaptureBall()
    {
        captureButton.interactable = GameData.Instance.captureBalls > 0;
        pokeballText.text = GameData.Instance.captureBalls.ToString();
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
        pokeballText.text = GameData.Instance.captureBalls.ToString();
    }

    private void CaptureMonster()
    {
        float baseChance = GetBaseCaptureChance(selectedMonsterEnemy.rarity);
        float bonusFromDefense = GetBonusFromDefense(selectedMonsterEnemy);
        float getBonusFromLevel = LevelSystem.Instance != null ? LevelSystem.Instance.GetBonusCapture(selectedMonsterEnemy.rarity) : 1f;
        float finalChance = Mathf.Clamp01(baseChance * bonusFromDefense * getBonusFromLevel); // ex : 0.1 * 1.2 * 1.5

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
            //capture succesed
            OnFightEnd?.Invoke(FightEndType.Capture);
        }
        else
        {
            Debug.Log("Capture failed!");
            FightSceneManager.instance.SetCapture(false);
        }
    }

    #endregion

    #region Capture Rarity 
    private float GetBaseCaptureChance(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return 0.35f;      // 35%
            case Rarity.Rare: return 0.2f;        // 20%
            case Rarity.Legendary: return 0.1f;   // 10%
            default: return 0.3f;
        }
    }

    private float GetBonusFromDefense(MonsterData monster)
    {
        float currentDefense = defenseEnemy;        // Ambil dari slider HP musuh
        float maxDefense = monster.defense;

        float damageTakenRatio = 1f - (currentDefense / maxDefense); // 1- 0.5 = 0.5 
        float bonusFactor = 0.5f; // 30% max bonus dari HP yang kecil

        float finalBonus = 1 + damageTakenRatio;
        bool isHalfLess = currentDefense <= currentDefense / maxDefense;

        return isHalfLess ? finalBonus + bonusFactor : finalBonus;
    }

    #endregion

    #region Timer

    public void StartTimer()
    {
        if (currentTimer != null)
        {
            StopCoroutine(currentTimer);
            currentTimer = null;
        }

        timer = timerDuration; // Reset the timer to its duration  
        Timer(); // Start the timer countdown  
    }

    Coroutine currentTimer;

    private void Timer()
    {
        float speedFactor = Mathf.Clamp(speedEnemy - speedPlayer, -30f, 30f); // Calculate speed difference, clamped to avoid extreme values  
        timer = Mathf.Max(1f, timerDuration - speedFactor); // Adjust timer duration based on speed difference, minimum 1 second  
        currentTimer = StartCoroutine(Countdown());
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
        questionPanel.gameObject.SetActive(false);
        actionPanel.SetActive(true);
        answerPanel.SetActive(false);
    }

    #endregion

    private IEnumerator PlayAnimation(string animationName)
    {
        // Play the animation here
        // Example: animator.SetTrigger(animationName);
        yield return new WaitForSeconds(1f); // Wait for the animation to finish
    }

}
