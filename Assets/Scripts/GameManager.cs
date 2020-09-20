using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    private void Awake()
    {
        if(gm == null)
        {
            gm = this;
        }
    }

    public enum GameState
    {
        Ready,
        Run,
        Pause,
        GameOver
    }

    public GameState gState;

    // 게임 상태 UI 오브젝트 변수
    public GameObject gameLabel;

    // 게임 상태 UI 텍스트 컴포넌트 변수
    Text gameText;

    PlayerMove player;

    public GameObject gameOption;
    
    // Start is called before the first frame update
    void Start()
    {
        gState = GameState.Ready;

        // 게임 상태 UI 오브젝트에서 Text 컴포넌트를 가져온다.
        gameText = gameLabel.GetComponent<Text>();

        // Text 변경 후 활성화
        gameText.text = "Ready...";
        gameText.color = new Color32(255, 185, 0, 255);
        gameLabel.SetActive(true);

        // 준비 완료 코루틴
        StartCoroutine(ReadyToStart());

        // 플레이어 오브젝트 확보
        player = GameObject.Find("Player").GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.hp <= 0)
        {
            // 플레이어의 애니메이션을 멈춘다.
            player.GetComponentInChildren<Animator>().SetFloat("MoveMotion", 0f); 
            
            // 게임 오버 문구 띄우기
            gameLabel.SetActive(true);
            gameText.text = "Game Over";
            gameText.color = new Color32(255, 0, 0, 255);

            // 상태 텍스트의 자식 오브젝트의 트랜스폼 컴포넌트를 가져옴
            Transform buttons = gameText.transform.GetChild(0);

            // 버튼 오브젝트를 활성화한다
            buttons.gameObject.SetActive(true);
            
            // 게임 오버 상태로 변경
            gState = GameState.GameOver;
        }
    }

    IEnumerator ReadyToStart()
    {
        // 2초간 대기
        yield return new WaitForSeconds(2f);

        // 상대 텍스트의 내용을 'Go!'로 한다
        gameText.text = "Go!";

        // 0.5초간 대기
        yield return new WaitForSeconds(0.5f);

        // 상태 텍스트를 비활성화
        gameLabel.SetActive(false);

        // 상태를 '게임 중'으로 변경
        gState = GameState.Run;
    }

    // 옵션 화면 켜기
    public void OpenOptionWindow()
    {
        // 옵션창 활성화
        gameOption.SetActive(true);

        // 게임 속도를 0배속으로 전환
        Time.timeScale = 0f;

        // 게임 상태를 일시 정지 상태로 변경
        gState = GameState.Pause;
    }

    // 계속하기 옵션
    public void CloseOptionWindow()
    {
        gameOption.SetActive(false);
        Time.timeScale = 1f;
        gState = GameState.Run;
    }

    public void RestartGame()
    {
        // 게임 속도를 1배속으로 전환
        Time.timeScale = 1f;

        // 현재 씬 번호를 다시 로드
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // 로딩 화면씬을 로드
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        // 게임 종료
        Application.Quit();
    }
}
