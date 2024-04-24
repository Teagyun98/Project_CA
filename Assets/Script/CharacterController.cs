using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJob
{

}

public class CharacterController : MonoBehaviour
{
    // 직업을 무기로 생각하고 스트래티지 패턴으로 구현
        // 직업 변경시 Set에 프로퍼티를 사용하여 GameManager에서 직업에 맞는 스텟으로 초기화 시켜주기
    // 플레이어는 State Machine 패턴으로 각각 구분해서 구현
}
