using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    public GameObject firePosition;
    public GameObject bombFactory;

    public float throwPower = 15f;

    public GameObject bulletEffect;
    ParticleSystem ps;

    public int weaponPower = 5;

    Animator anim;

    enum WeaponMode
    {
        Normal,
        Sniper
    }

    bool ZoomMode = false;

    WeaponMode wMode;

    public Text wModeText;

    public GameObject[] eff_Flash;

    // 무기 아이콘 스프라이트 변수
    public GameObject weapon01;
    public GameObject weapon02;
    
    // 크로스헤어 스프라이트 변수
    public GameObject crosshair01;
    public GameObject crosshair02;
    
    // 마우스 오른쪽 버튼 클릭 아이콘 스프라이트 변수
    public GameObject Weapon01_R;
    public GameObject Weapon02_R;

    public GameObject crosshair02_zoom;

    // Start is called before the first frame update
    void Start()
    {
        ps = bulletEffect.GetComponent<ParticleSystem>();

        anim = GetComponentInChildren<Animator>();

        wMode = WeaponMode.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        // 게임 진행중 아니면 update 스킵
        if(GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            switch (wMode)
            {
                case WeaponMode.Normal:
                    // 수류탄 오브젝트를 생성하고, 수류탄의 생성 위치를 발사 위치로 한다
                    GameObject bomb = Instantiate(bombFactory);
                    bomb.transform.position = firePosition.transform.position;

                    // 수류탄 오브젝트의 리지드보디 컴포넌트를 가져온다
                    Rigidbody rb = bomb.GetComponent<Rigidbody>();

                    // 카메라의 정면 방향으로 수류탄에 물리적인 힘을 가한다.
                    rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
                    break;
                case WeaponMode.Sniper:
                    // 줌 모드 상태가 아니라면 줌 모드로 변경 
                    if (!ZoomMode)
                    {
                        Camera.main.fieldOfView = 15f;
                        ZoomMode = true;
                        
                        // 줌 모드일 때 크로스헤어를 변경한다
                        crosshair02_zoom.SetActive(true);
                        crosshair02.SetActive(false);
                    }
                    // 그렇지 않으면 원래 상태로 변경, 줌 모드 해제
                    else
                    {
                        Camera.main.fieldOfView = 60f;
                        ZoomMode = false;
                        
                        // 크로스헤어를 스나이퍼 모드로 돌려놓는다
                        crosshair02_zoom.SetActive(false);
                        crosshair02.SetActive(true);
                    }
                    break;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (anim.GetFloat("MoveMotion") == 0)
            {
                anim.SetTrigger("Attack");
            }

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitInfo = new RaycastHit();

            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyFSM eFSM = hitInfo.transform.GetComponent<EnemyFSM>();
                    eFSM.HitEnemy(weaponPower);
                }
                else
                {
                    bulletEffect.transform.position = hitInfo.point;
                    bulletEffect.transform.forward = hitInfo.normal;
                    ps.Play();
                }
            }

            StartCoroutine(ShootEffectOn(0.05f));
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            wMode = WeaponMode.Normal;
            
            // 스나이퍼 모드에서 일반 모드 키를 눌렀을 때 Weapon01_R_zoom은 비활성화, 줌 모드 해제
            crosshair02_zoom.SetActive(false);

            // 카메라의 화면을 다시 원래대로 돌림
            Camera.main.fieldOfView = 60f;

            wModeText.text = "Normal Mode";
            
            // 1번 스프라이프는 활성화, 2번은 비활성화
            weapon01.SetActive(true);
            weapon02.SetActive(false);
            crosshair01.SetActive(true);
            crosshair02.SetActive(false);
            
            // Weapon01_R는 활성화되고, Weapon02_R는 비활성화
            Weapon01_R.SetActive(true);
            Weapon02_R.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            wMode = WeaponMode.Sniper;

            wModeText.text = "Sniper Mode";
            
            // 1번 스프라이프는 비활성화, 2번은 활성화
            weapon01.SetActive(false);
            weapon02.SetActive(true);
            crosshair01.SetActive(false);
            crosshair02.SetActive(true);
            
            // Weapon01_R는 비활성화되고, Weapon02_R는 활성화
            Weapon01_R.SetActive(false);
            Weapon02_R.SetActive(true);
        }
    }

    IEnumerator ShootEffectOn(float duration)
    {
        // 랜덤 숫자
        int num = Random.Range(0, eff_Flash.Length - 1);

        // 이펙트 오브젝트 배열에서 뽑힌 숫자에 해당하는 이펙트 오브젝트 활성화
        eff_Flash[num].SetActive(true);

        // 지정한 시간만큼 기다림
        yield return new WaitForSeconds(duration);

        // 이펙트 오브젝트를 다시 비활성화
        eff_Flash[num].SetActive(false);
    }
}
