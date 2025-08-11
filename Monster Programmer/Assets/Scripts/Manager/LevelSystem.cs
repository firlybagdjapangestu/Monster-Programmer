using System;
using UnityEngine;

namespace Manager 
{
    public class LevelSystem : MonoBehaviour
    {
        public static LevelSystem Instance;

        [Header("[Reference]")]
        [SerializeField] private GameData gameData;

        [Header("[Setting]")]
        [SerializeField] private int xpPerBattle = 25;           // Rata-rata XP per pertarungan
        [SerializeField] private float growthFactor = 1.2f;      // Faktor pertumbuhan XP tiap level

        [Header("[Max Level Setting]")]
        [SerializeField] private UpgradeAttribute[] upgradeAttributes = new UpgradeAttribute[0];

        public int Level => gameData.level;
        public int currentXp => gameData.currentXp;
        public UpgradeAttribute[] AllUpgrades => upgradeAttributes;

        #region Class & Enum

        public enum TypeUpgrade
        {
            PlusAttackPercent,
            PlusDefendPercent,
            PlusChanceRarePercent,
            PlusChanceLegenadryPercent
        }

        [System.Serializable]
        public class UpgradeAttribute
        {
            public TypeUpgrade Upgrade;
            public float value;
        }

        #endregion

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public float GetMultiplerValue(TypeUpgrade _type)
        {
            float valueGet = 0f;

            // Loop dari level 1 sampai level sekarang (inclusive), tapi dibatasi panjang array
            int maxIndex = Mathf.Min(Level, upgradeAttributes.Length);

            for (int i = 0; i < maxIndex; i++)
            {
                if (upgradeAttributes[i].Upgrade == _type)
                {
                    valueGet += upgradeAttributes[i].value;
                }
            }

            return 1f + valueGet; // Default multiplier 1
        }

        public void AddXpBattle()
        {
            if (Level >= upgradeAttributes.Length)
            {
                Debug.Log("Has Max Level");
                gameData.currentXp = 0;
                return;
            }

            Debug.Log("Give Xp To Player");
            gameData.currentXp += xpPerBattle;
        }

        public void CheckLevelUp(Action<bool> _levelUp)
        {
            if (Level >= upgradeAttributes.Length)
            {
                Debug.Log("Has Max Level");
                _levelUp?.Invoke(false);
                return;
            }

            int xpNeed = NeedXp();
            if (currentXp < xpNeed)
            {
                _levelUp?.Invoke(false);
                return;
            }

            gameData.currentXp -= xpNeed;
            if (gameData.currentXp < 0)
                gameData.currentXp = 0;

            gameData.level += 1;
            gameData.SaveGame();
            _levelUp?.Invoke(true);

        }

        public bool IsMaxLevel()
        {
            return Level >= upgradeAttributes.Length;
        }

        public int NeedXp()
        {
            int xpNeeded = GetXpToNextLevel(
                Level,
                xpPerBattle,
                growthFactor
                );

            return xpNeeded;
        }

        private int GetXpToNextLevel(int level, int xpPerBattle, float growthFactor)
        {
            int baseXp = xpPerBattle * (level + 1);
            return Mathf.RoundToInt(baseXp * Mathf.Pow(growthFactor, level - 1));
        }

        internal float GetBonusCapture(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common: return 1f;
                case Rarity.Rare: return GetMultiplerValue(TypeUpgrade.PlusChanceRarePercent);
                case Rarity.Legendary:return GetMultiplerValue(TypeUpgrade.PlusChanceLegenadryPercent);
                default: return 1f;
            }
        }
    }


    public class CalculatorDamage
    {
        private static float attackPercent = 0.4f;
        public static float GetAttackDamage(float _attack)
        {
            return _attack * attackPercent;
        }
    }

}
