using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Button gameStartBtn;
    [SerializeField] private TextMeshProUGUI stageText;

    public bool GameOver { get; private set; }
    public int Stage { get; private set; }
    public int stageExp;
    public int gold;

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
        GameOver = true;
        // 고블린 소환 함수
        StartCoroutine(SpawnGoblin());
    }

    private void FixedUpdate()
    {
        // 카메라가 첫번째 캐릭터를 따라 이동
        if (FirstChar() != null)
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position,FirstChar().transform.position + new Vector3(0,0,-10), 2);
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

            if(monsterArea.GetChild(i).GetComponent<MonsterController>() && monsterArea.GetChild(i).gameObject.activeSelf == true)
            {
                monster = monsterArea.GetChild(i).GetComponent<MonsterController>();

                if ( monster.Sm != null && monster.Sm.CurState == monster.DicState[MonsterState.Death])
                    monster = null;
            }

            // 첫번째 캐릭터는 탐색 범위와 상관없이 몬스터를 쫒는다.
            if(monster != null && (range == 0 || Vector2.Distance(pos, monster.transform.position) <= range))
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
        int activeMonsterNum = 0;

        for(int i = 0; i < monsterArea.childCount; i++)
		{
            MonsterController monster = monsterArea.GetChild(i).GetComponent<MonsterController>();

            if (monster != null && monster.gameObject.activeSelf == true && monster.Sm.CurState != monster.DicState[MonsterState.Death])
                activeMonsterNum++;
		}

        if(GameOver == false && activeMonsterNum < 5)
		{
            MonsterController _goblin = null;

            // 풀링
            for (int i = 0; i < monsterArea.childCount; i++)
            {
                MonsterController monster = monsterArea.GetChild(i).GetComponent<MonsterController>();

                if (monster.Sm.CurState == monster.DicState[MonsterState.Death] || monster.gameObject.activeSelf == false)
                {
                    _goblin = monster;

                    // 비활성화된 고블리 활성화
                    if (_goblin.gameObject.activeSelf == false)
                        _goblin.gameObject.SetActive(true);

                    bool boss = false;

                    if(stageExp > 10)
					{
                        stageExp -= 10;
                        boss = true;
					}

                    _goblin.Resurrection(boss);
                    break;
                }
            }

            if (_goblin == null)
			{
                _goblin = Instantiate(goblin, monsterArea);

                bool boss = false;

                if (stageExp > 10)
                {
                    stageExp -= 10;
                    boss = true;
                }

                _goblin.SetStatus(boss);
            }

            _goblin.transform.position = FirstChar().transform.position + new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));

        }

        yield return new WaitForSeconds(monsterSpawnTime);

        StartCoroutine(SpawnGoblin());
    }

    // 캐릭터가 모두 사망시 게임 오버
    public void CheckGameOver()
	{
        foreach(CharController character in charList)
		{
            if(character.Sm.CurState != character.DicState[CharState.Die])
			{
                GameOver = false;
                return;
			}
		}

        GameOver = true;
        gameStartBtn.gameObject.SetActive(true);
	}

    // 살아있는 캐릭터 중 최대체력 대비 체력이 가장 적은 캐릭터 반환
    public CharController GetLowHpChar(Vector2 pos , float distance)
	{
        CharController result = null;

        foreach (CharController character in charList)
		{
            if (character.hp < character.status.hp && Vector2.Distance(pos, character.transform.position) <= distance && character.Sm.CurState != character.DicState[CharState.Die])
                result = result == null || result.hp / result.status.hp > character.hp / character.status.hp ? character : result;
		}

        return result;
	}

    // 살아있는 몬스터 중 인자로 받은 위치에서 범위 내 모든 몬스터를 반환
    public List<MonsterController> GetDistanceMonsters(Vector2 pos, float distance)
	{
        List<MonsterController> list = new List<MonsterController>();

        for (int i = 0; i < monsterArea.childCount; i++)
        {
            MonsterController monster = monsterArea.GetChild(i).GetComponent<MonsterController>();

            if (monster.Sm.CurState != monster.DicState[MonsterState.Death] && Vector2.Distance(pos, monster.transform.position) <= distance)
                list.Add(monster);
        }

        return list;
	}

    public void GameStart()
	{
        GameOver = false;
        Stage = 1;
        stageText.text = $"Stage{Stage}";
        stageExp = 0;
        gold = 0;

        // 몬스터 비활성화
        for(int i =  0; i < monsterArea.childCount; i++)
		{
            monsterArea.GetChild(i).gameObject.SetActive(false);
		}

        // 캐릭터 초기화
        for(int i = 0; i<charList.Count; i++)
		{
            charList[i].transform.position = new Vector3(i, 0, 0);
            charList[i].ResetChar();
		}

        gameStartBtn.gameObject.SetActive(false);
	}

    public void NextStage()
	{
        Stage++;
        stageText.text = $"Stage{Stage}";
	}
}