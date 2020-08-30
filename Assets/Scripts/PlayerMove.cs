using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();  
    }

    // Update is called once per frame
    void Update()
    {
        // 사용자 입력
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 이동 방향 설정
        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized;

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
    }
}
