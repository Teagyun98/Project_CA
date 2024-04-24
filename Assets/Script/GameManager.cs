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
    public int skillDistance;
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
    public int mosterAttackDistance;
    [Header("Character")]
    public float characterSpawnTime;
    public List<CharacterStatus> characterStatusList;

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
}
