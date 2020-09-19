using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    // 이동 속도 변수
    public float moveSpeed = 7f;

    // CharacterController 변수
    CharacterController cc;

    // 중력 변수
    float gravity = -20f;

    // 수직 속력 변수
    float yVelocity = 0;

    // 점프력 변수
    public float jumpPower = 10f;

    // 점프 상태 변수
    public bool isJumping = false;

    public int hp = 20;

    // 최대 체력 변수
    int maxHp = 25;

    // hp 슬라이더 변수
    public Slider hpSlider;

    // Hit 효과 오브젝트
    public GameObject hitEffect;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();

        // 애니메이터 받아오기
        anim = GetComponentInChildren<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        // 게임 진행중 아니면 update 스킵
        if(GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // 사용자 입력
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 이동 방향 설정
        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized;

        // 이동 블렌딩 트리를 호출하고 벡터의 크기 값을 넘겨준다.
        anim.SetFloat("MoveMotion", dir.magnitude);

        // 카메라 기준으로 방향 변환
        dir = Camera.main.transform.TransformDirection(dir);

        // 점프중이고 바닥에 착지했다면
        if (isJumping && cc.collisionFlags == CollisionFlags.Below)
        {
            isJumping = false;
            yVelocity = 0;

        }
        
        // 점프 누르고 점프중 아니면
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            yVelocity = jumpPower;
            isJumping = true;
        }

        // 수직 속도에 중력 값 적용
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;

        // 이동. p = p0 + vt
        // transform.position += dir * moveSpeed * Time.deltaTime;
        cc.Move(dir * moveSpeed * Time.deltaTime);

        // 현재 플레이어 hp(%)를 hp 슬라이더의 value에 반영한다.
        hpSlider.value = (float)hp / (float)maxHp;
    }

    public void DamageAction(int damage)
    {
        hp -= damage;

        if (hp > 0)
        {
            // 피격 이펙트 코루틴을 시작한다
            StartCoroutine(PlayHitEffect());
        }
    }

    IEnumerator PlayHitEffect()
    {
        // 1. 피격 UI를 활성화
        hitEffect.SetActive(true);

        // 2. 0.3초간 대기한다.
        yield return new WaitForSeconds(0.3f);

        // 3. 피격 UI를 비활성화한다.
        hitEffect.SetActive(false);
    }
}
