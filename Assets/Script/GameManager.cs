using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터 기본 스탯 클래스
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
    // 싱글톤
    private static GameManager instance;

    public static GameManager Instance
    {
        get { return instance == null ? null : instance; }
        private set { instance = value; }
    }

    // 몬스터 기초값
    [Header("GameSetting\n\nMoster")]
    [SerializeField] private MonsterController goblin;
    public float monsterSpawnTime;
    public int monsterMaxActiveCount;
    public int monsterHp;
    public int monsterAttackPower;
    public int monsterAttackDelay;
    public float mosterAttackDistance;
    public float mosterSearchRange;
    // 캐릭터 기초값
    [Header("Character")]
    public float characterSpawnTime;
    public List<CharacterStatus> characterStatusList;

    [Header("GameInfo")]
    // 캐릭터 리스트
    [SerializeField] private List<CharController> charList;
    // 몬스터 스폰 Transform
    [SerializeField] private Transform monsterArea;

    private void Awake()
    {
        // 싱글톤
        if (instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // 고블린 소환 함수
        StartCoroutine(SpawnGoblin());
    }

    private void FixedUpdate()
    {
        // 카메라가 첫번째 캐릭터를 따라 이동
        Camera.main.transform.position = FirstChar().transform.position + new Vector3(0,0,-10);
    }

    // 가까이 있는 캐릭터 반환 함수
    public CharController NearChar(Vector2 pos)
	{
        // 캐릭터 리스트에서 죽지 않은 캐릭터 중 인자르 받은 위치에서 몬스터 탐색범위 안의 가장 가까운 캐릭터 반환
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

    // 가까이 있는 몬스터 반환 함수
    public MonsterController NearMonster(Vector2 pos, float range) 
    {
        // MonsterArea에서 살아있는 몬스터 중 인자로 받은 위치와 탐색 범위 안에서 가장 가까운 몬스터 반환
        MonsterController result = null;
        MonsterController monster;

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

    // 캐릭터 리스트에서 살아있는 첫번째 캐릭터 반환
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

    // 고블린 스폰 코루틴
    public IEnumerator SpawnGoblin()
    {
        MonsterController _goblin  = null;

        // 풀링
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

        yield return new WaitForSeconds(monsterSpawnTime);

        StartCoroutine(SpawnGoblin());
    }
}