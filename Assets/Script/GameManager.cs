using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterStatus
{
    public string name;
    public int hp;
    public int attackPower;
    public int attackDistance;
    public float attackDelay;
    public float skillDistance;
    public float skillDelay;
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [Header("GameSetting\n\nMoster")]
    [SerializeField] private MonsterController goblin;
    public float monsterSpawnTime;
    public int monsterMaxActiveCount;
    public int monsterHp;
    public int monsterAttackPower;
    public int monsterAttackDelay;
    public float mosterAttackDistance;
    public float mosterSearchRange;
    [Header("Character")]
    public float characterSpawnTime;
    public List<CharacterStatus> characterStatusList;
    [Header("GameInfo")]
    [SerializeField] private List<CharController> charList;
    [SerializeField] private Transform monsterArea;

    public static GameManager Instance 
    {
        get { return instance == null ? null : instance; }
        private set { instance = value; }
    }

    private void Awake()
    {
        if (instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        Camera.main.transform.position = FirstChar().transform.position + new Vector3(0,0,-10);
    }

    public CharController NearChar(Vector2 pos)
	{
        CharController result = null;

        foreach(CharController character in charList)
		{
            if (Vector2.Distance(pos, character.transform.position) < mosterSearchRange && character.Sm.CurState != character.DicState[CharState.Die])
			{
                if (result == null || Vector2.Distance(result.transform.position, pos) > Vector2.Distance(character.transform.position, pos))
                    result = character;
			}
		}

        return result;
	}

    public MonsterController NearMonster(Vector2 pos, float range) 
    {
        MonsterController result = null;
        MonsterController monster = null;

        for (int i = 0; i< monsterArea.childCount; i++)
        {
            monster = null;

            if(monsterArea.GetChild(i).GetComponent<MonsterController>())
            {
                monster = monsterArea.GetChild(i).GetComponent<MonsterController>();

                if ( monster.Sm != null && monster.Sm.CurState == monster.DicState[MonsterState.Death])
                    monster = null;
            }

            // 첫번째 캐릭터는 탐색 범위와 상관없이 몬스터를 쫒는다.
            if(monster != null && (range == 0 || Vector2.Distance(pos, monster.transform.position) < range))
            {
                if(result == null || Vector2.Distance(pos, result.transform.position) > Vector2.Distance(pos, monster.transform.position))
                    result = monster;
            }
        }

        return result;
    }

    public CharController FirstChar()
    {
        CharController result = null;

        for(int i = 0; i < charList.Count; i++) 
        {
            if (charList[i].Sm.CurState != charList[i].DicState[CharState.Die])
            {
                result = charList[i];
                break;
            }
        }

        return result;
    }

    public void SpawnGoblin()
    {
        MonsterController _goblin  = null;

        for(int i = 0; i < monsterArea.childCount; i++) 
        {
            MonsterController monster = monsterArea.GetChild(i).GetComponent<MonsterController>();

            if (monster.Sm.CurState == monster.DicState[MonsterState.Death])
            {
                _goblin = monster;
                _goblin.Resurrection();
                break;
            }
        }

        if (_goblin == null)
            _goblin = Instantiate(goblin, monsterArea);

        _goblin.transform.position = FirstChar().transform.position + new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));
    }
}