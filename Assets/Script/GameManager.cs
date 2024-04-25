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
    [SerializeField] List<CharController> charList;

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

    public CharController NearChar(Vector2 pos)
	{
        CharController result = null;

        foreach(CharController character in charList)
		{
            if (Vector2.Distance(pos, character.transform.position) < mosterSearchRange)
			{
                if (result == null || Vector2.Distance(result.transform.position, pos) > Vector2.Distance(character.transform.position, pos))
                    result = character;
			}
		}

        return result;
	}
}
