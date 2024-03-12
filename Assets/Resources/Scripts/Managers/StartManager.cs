using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    public GameManager gameManager;

    UIManager UiManager;
    AudioManager audioManager;

    void Awake() 
    {
        UiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;

        //해상도 조정(x길이, y길이, 전체화면 여부), 남은 부분은 자동으로 검은색 처리
        //Screen.SetResolution(1920, 1080, false);
    }
    

    public void StartGame() 
    {
        gameObject.SetActive(false);

        //선택창 열기
        UiManager.SelectObj.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    /*
     
    
[CreateAssetMenu(fileName = "SingleInfoData", menuName = "Scriptable Ojbect/SingleInfo")]
public class SingleInfoData : ScriptableObject//스크립타블 오브젝트 상속
{
    public enum SingleInfoType 
    {
        Train, StarFall, Block, Fly, Dog
    }

    [Header("싱글 게임 타입")]
    public SingleInfoType singleType;

    [Header("싱글 게임 쉐이더")]
    public Material shader;
    [Header("싱글 게임 아이콘")]
    public Sprite icon;

    [Header("이동 할 씬 내부 이름")]
    public string sceneInnerTitle;
    [Header("이동 할 씬 외부 이름")]
    public string sceneOutterTitle;
    [Header("이동 할 씬 설명")]
    [TextArea]
    public string sceneDesc;

    [Header("이동 할 씬 목표 점수들")]
    public int Sscore;
    public int Ascore;
    public int Bscore;
    public int Cscore;
    public int Dscore;
    public int Escore;

    [Header("씬 레벨")]
    public int sceneLevel;
} 

     */
}
