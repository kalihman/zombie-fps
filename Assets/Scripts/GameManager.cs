using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        GameOver
    }

    public GameState gState;

    // 게임 상태 UI 오브젝트 변수
    public GameObject gameLabel;

    // 게임 상태 UI 텍스트 컴포넌트 변수
    Text gameText;

    PlayerMove player;
    
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
            // 게임 오버 문구 띄우기
            gameLabel.SetActive(true);
            gameText.text = "Game Over";
            gameText.color = new Color32(255, 0, 0, 255);
            
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
}
