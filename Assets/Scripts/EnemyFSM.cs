using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFSM : MonoBehaviour
{
    // 에너미 상태 상수
    enum EnemyState
    {   
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }

    EnemyState m_State;
    
    // 플레이어 발견 범위
    public float findDistance = 8f;

    // 플레이어 트랜스폼
    Transform player;

    // 공격 가능 범위
    public float attackDistance = 2f;

    // 이동 속도
    public float moveSpeed =5f;

    // 캐릭터 콘트롤러 컴포넌트
    CharacterController cc;

    // 누적 시간
    float currentTime = 0;

    // 공격 딜레이 시간
    float attackDelay = 2f;

    // 에너미의 공격력
    public int attackPower = 3;

    // 초기 위치 저장용 변수
    Vector3 originPos;

    // 초기 회전값 저장용 변수
    Quaternion originRot;

    // 이동 가능 범위
    public float moveDistance = 20f;

    public int weaponPower = 5;

    public int hp = 15;

    int maxHp = 15;

    public Slider hpSlider;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        m_State = EnemyState.Idle;
        
        player = GameObject.Find("Player").transform;

        cc = GetComponent<CharacterController>();

        // 초기 위치
        originPos = transform.position;

        // 초기 회전
        originRot = transform.rotation;

        anim = transform.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        hpSlider.value = (float)hp / (float)maxHp;

        switch(m_State)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
                //Damaged();
                break;
            case EnemyState.Die:
                //Die();
                break;
        }
    }

    void Idle()
    {
        if(Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State = EnemyState.Move;
            print("상태 전환: Idle -> Move");

            // animation 트리거
            anim.SetTrigger("IdleToMove");
        }
    }

    void Move()
    {
        if(Vector3.Distance(originPos, transform.position) > moveDistance)
        {
            m_State = EnemyState.Return;
            print("상태 전환: Move -> Return");
        }
        else if(Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;

            // 전방 방향을 이동 방향과 일치시킴
            transform.forward = dir;

            cc.Move(dir * moveSpeed * Time.deltaTime);
        }
        else
        {
            m_State = EnemyState.Attack;
            print("상태 전환: Move -> Attack");
            currentTime = attackDelay;
        }
    }

    void Attack()
    {
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            currentTime += Time.deltaTime;
            if(currentTime > attackDelay)
            {
                player.GetComponent<PlayerMove>().DamageAction(attackPower);
                currentTime = 0;
                print("공격");
            }
        }
        // 그렇지 않다면, 현재 상태를 이동으로 전환한다
        else
        {
            m_State = EnemyState.Move;
            print("상태 전환: Attack -> Move");
            currentTime = 0;
        }
    }

    void Return()
    {
        // 만일 초기 위치에서의 거리가 0.1f 이상이라면 초기 위치 쪽으로 이동한다
        if(Vector3.Distance(transform.position, originPos) > 0.1f)
        {
            Vector3 dir = (originPos - transform.position).normalized;

            cc.Move(dir * moveSpeed * Time.deltaTime);

            // 복귀 지점(원래 위치)로 전환
            transform.forward = dir;
        }
        else
        {
            // 원위치, 회전
            transform.position = originPos;
            transform.rotation = originRot;

            // hp를 다시 회복
            hp = maxHp;

            m_State = EnemyState.Idle;
            print("상태 전환: Return -> Idle");

            // 대기 애니메이션으로 전환하는 트렌지션 호출
            anim.SetTrigger("MoveToIdle");
        }
    }

    public void HitEnemy(int hitPower)
    {
        // 만일 이미 피격 상태이거나 사망 상태 또는 복귀 상태라면 아무런 처리도 하지 않고 함수를 종료한다.
        if (m_State == EnemyState.Damaged || m_State == EnemyState.Die || m_State == EnemyState.Return)
        {
            return;
        }

        hp -= hitPower;

        if(hp > 0)
        {
            m_State = EnemyState.Damaged;
            print("상태 전환: Any stat -> Damaged");
            Damaged();
        }
        else
        {
            m_State = EnemyState.Die;
            print("상태 전환: Any state -> Die");
            Die();
        }
    }

    void Damaged()
    {
        StartCoroutine(DamageProcess());
    }

    IEnumerator DamageProcess()
    {
        // 피격 모셥 시간만큼 기다린다.
        yield return new WaitForSeconds(0.5f);

        // 현재 상태를 이동 상태로 전환한다.
        m_State = EnemyState.Move;
        print("상태 전환: Damaged -> Move");
    }

    void Die()
    {
        // 진행중인 코루틴(데미지, 이동 등) 모두 종료
        StopAllCoroutines();

        // 죽음 상태 처리를 위한 코루틴 실행
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        // 캐릭터 컴포넌트 비활성화
        cc.enabled = false;

        // 2초 동안 기다린 후에 자기 자신을 제거
        yield return new WaitForSeconds(2f);
        print("소멸!");
        Destroy(gameObject);
    }
}
