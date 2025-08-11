using System.Collections.Generic;
using UnityEngine;


namespace Manager.Spawner
{
    public class MonsterSpawnManager : MonoBehaviour
    {
        [Header("[Reference]")]
        [SerializeField] private Transform[] spawnTransform;
        [SerializeField] private GameObject monsterObject;

        private Transform spawnTrPosition;

        [Header("[Setting]")]
        [SerializeField] private MapMonster mapTypeSpawn;
        [SerializeField] private int maxSpawnMonster = 3;
        [SerializeField] private Vector2 randomPosRange;

        private List<MonsterData> canSpawnMonsters = new List<MonsterData>();

        void Start()
        {
            FindWhatMonsterCanSpawn();
            SpawnMonster();
        }

        private void FindWhatMonsterCanSpawn()
        {
            canSpawnMonsters.Clear();
            canSpawnMonsters = GameData.Instance.availableMonsters.FindAll(monster => monster.spawnMap == mapTypeSpawn);
        }

        private void SpawnMonster()
        {
            for (int i = 0; i < maxSpawnMonster; i++)
            {
                MonsterData _monster = canSpawnMonsters[Random.Range(0, canSpawnMonsters.Count)];
                DoSpawn(_monster);
            }
        }

        private void DoSpawn(MonsterData _data)
        {
            spawnTrPosition = spawnTransform[Random.Range(0,spawnTransform.Length)];

            float randX = Random.Range(spawnTrPosition.position.x - randomPosRange.x / 2, spawnTrPosition.position.x + randomPosRange.x / 2);
            float randY = Random.Range(spawnTrPosition.position.y - randomPosRange.y / 2, spawnTrPosition.position.y + randomPosRange.y / 2);
            Vector2 _randPos = new Vector2(randX, randY);

            GameObject g = Instantiate(monsterObject, _randPos, Quaternion.identity);
            MonsterController monster = g.GetComponent<MonsterController>();

            monster.GetRandomMonster(canSpawnMonsters);
        }

        private void OnDrawGizmos()
        {
            if (spawnTransform == null)
                return;

            for (int i = 0; i < spawnTransform.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(spawnTransform[i].position, randomPosRange);
            }

            
        }
    }
}

