using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using AssetKits.ParticleImage;

public class FireManager : MonoBehaviour
{
    #region 날짜 양식
    public class DateTimeUtils
    {
        public static string GetCurrentDateTimeString()
        {
            DateTime now = DateTime.Now;
            //return now.ToString("yyMMdd HH:mm");
            return now.ToString("yy-MM-dd HH:mm");
        }
    }
    #endregion

    #region 순위 양식
    [System.Serializable]
    public class LeaderBoard
    {
        public string[] DateStr = new string[3];//난이도 마다 3개의 날짜
        public float[] ClearTime = new float[3];//난이도 마다 3개의 시간
    }

    [Serializable]
    public class LeaderBoardArray
    {
        public LeaderBoard[] leaderBoardArr = new LeaderBoard[3];//난이도마다 한개씩 존재
        public bool isLoad = false;

        public LeaderBoardArray()
        {
            // 각 LeaderBoard 객체 초기화
            for (int i = 0; i < leaderBoardArr.Length; i++)
            {
                leaderBoardArr[i] = new LeaderBoard();
            }
        }
    }
    public LeaderBoardArray leaderBoardArray = new LeaderBoardArray();
    #endregion

    #region 데이터 순위 바꾸기
    float RoundToDecimalPlace(float value, int decimalPlaces)
    {
        float factor = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(value * factor) / factor;
    }

    public bool CheckJson(int gameLevel, float clearTime, bool isSave) 
    {
        Debug.Log("JSON 내용 바꾸기");

        clearTime = RoundToDecimalPlace(clearTime, 1);

        gameLevel -= 1;

        string dateStr = playerName + "(" + DateTimeUtils.GetCurrentDateTimeString() + ")";

        //등급 갱신이 됐는지 여부
        bool isRankChange = false;

        float tmpTime;
        string tmpStr;

        for(int i = 0; i < 3; i++)
        {
            if (clearTime < leaderBoardArray.leaderBoardArr[gameLevel].ClearTime[i]) //0일수록 빨리 끝낸 것
            {
                isRankChange = true;
                if (isSave)//변화를 허용함
                {
                    //기존의 것을 일단 저장함
                    tmpTime = leaderBoardArray.leaderBoardArr[gameLevel].ClearTime[i];
                    tmpStr = leaderBoardArray.leaderBoardArr[gameLevel].DateStr[i];

                    //새롭게 갱신함
                    leaderBoardArray.leaderBoardArr[gameLevel].ClearTime[i] = clearTime;
                    leaderBoardArray.leaderBoardArr[gameLevel].DateStr[i] = dateStr;

                    clearTime = tmpTime;
                    dateStr = tmpStr;
                }
                else //변화를 허용하지 않음
                {
                    //이름 입력 활성화
                    inputField.SetActive(true);
                }
            }
        }

        return isRankChange;
    }
    #endregion

    #region 데이터 저장하기
    string userId = "veHlMdhxfhVKn3scykFjU9fzeEf2";
    public void SaveJson() 
    {
        Debug.Log("JSON 저장하기");

        string json = JsonUtility.ToJson(leaderBoardArray);

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("LeaderBoardArray").Child(userId).SetRawJsonValueAsync(json);
    }
    #endregion

    #region 데이터 불러오기
    public void LoadJson()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        //유저 ID를 통해 업적 데이터에 대한 참조 획득
        DatabaseReference achievementsRef = reference.Child("LeaderBoardArray").Child(userId);//playerId

        //데이터베이스로부터 데이터를 읽어옴
        achievementsRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("데이터베이스로부터 데이터를 읽는데 실패함: " + task.Exception.Flatten().InnerExceptions);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;

                if (dataSnapshot != null && dataSnapshot.Exists)
                {
                    // 스냅샷으로부터 JSON 데이터를 가져옴
                    string json = dataSnapshot.GetRawJsonValue();

                    leaderBoardArray = JsonUtility.FromJson<LeaderBoardArray>(json);
                    Debug.Log("JSON 불러옴");
                }
                else
                {
                    Debug.Log("해당 유저에 대한 정보가 없음");
                }
            }
        });
    }
    #endregion

    #region 이름 입력
    public void NameInput()
    {
        //종이 넘기는 효과음
        gameManager.audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        playerName = inputField.GetComponent<InputField>().text;
    }
    #endregion

    public float timing;
    public GameManager gameManager;

    string playerName = "";
    public GameObject leaderBoardPanel;
    public GameObject inputField;

    #region 코루틴
    WaitForSeconds waitSec;
    Coroutine FireCor;
    private void Start()
    {
        waitSec = new WaitForSeconds(timing);
    }

    public void StartCor() 
    {
        //불러오고
        gameManager.fireManager.LoadJson();

        //불러오기 완료 까지의 코루틴
        FireCor = StartCoroutine(UpdateCoroutine());
    }
    
    public void StopCor() //전투 초기화 할 때마다 수행
    {
        //리더보드 비활성화
        leaderBoardPanel.SetActive(false);
        inputField.SetActive(false);

        //로딩중 해제
        leaderBoardArray.isLoad = false;

        //플레이어 이름을 비워둠
        playerName = "";

        //와이파이 아이콘 초기화
        wifiImage.enabled = false;

        if (FireCor != null)
        {
            Debug.Log("파이어 코루틴이 진행중이라면 종료 시킴");
            StopCoroutine(FireCor);
        }
        else 
        {
            Debug.Log("파이어 코루틴이 이미 종료됨");
        }
    }

    //랭킹에 이름을 넣기 위함
    

    IEnumerator UpdateCoroutine()
    {
        //와이파이 이미지 활성화
        wifiImage.enabled = true;

        while (true)
        {
            if (leaderBoardArray.isLoad)//로드를 했으면(인터넷이 연결돼야 불러옴)
            {
                //랭킹 변화가 있으면
                if (CheckJson(gameManager.gameLevel, gameManager.uiManager.curPlayTime, false))//여기서 이름 인풋 필드 활성화
                {
                    if (gameManager.OnlineCheck() && playerName != "")//그리고 온라인이면
                    {
                        //바꾸고
                        CheckJson(gameManager.gameLevel, gameManager.uiManager.curPlayTime, true);

                        //보여주고
                        ShowJson(gameManager.gameLevel);

                        //저장하고
                        SaveJson();

                        leaderBoardArray.isLoad = false;
                        playerName = "";

                        wifiImage.enabled = false;

                        yield break;
                    }
                }
                else 
                {
                    //보여주기
                    ShowJson(gameManager.gameLevel);

                    leaderBoardArray.isLoad = false;

                    wifiImage.enabled = false;

                    yield break;
                }
            }

            //와이파이 아이콘 변경
            if (gameManager.OnlineCheck())
            {
                wifiImage.sprite = wifiSpriteArr[1];
            }
            else 
            {
                wifiImage.sprite = wifiSpriteArr[0];
            }

            yield return waitSec;
        }
    }
    public Image wifiImage;
    public Sprite[] wifiSpriteArr;
    #endregion

    #region JSON 보여주기
    public Text[] leaderBoardScoreTextArr;
    public Button[] leaderBoardScoreButtonArr;
    public void ShowJson(int levelIndex) 
    {
        Debug.Log(levelIndex + ": JSON 보여주기");

        //종이 넘기는 효과음
        gameManager.audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        //패널 보이도록
        leaderBoardPanel.SetActive(true);

        levelIndex -= 1;

        for (int buttonIndex = 0; buttonIndex < leaderBoardScoreButtonArr.Length; buttonIndex++) 
        {
            leaderBoardScoreButtonArr[buttonIndex].interactable = true;
        }
        leaderBoardScoreButtonArr[levelIndex].interactable = false;

        //리더보드 텍스트 변경
        string tmpText = "";

        for (int orderIndex = 0; orderIndex < 3; orderIndex++)//저장된 것을 어떻게 불러올지 불러오는 것
        {
            tmpText = leaderBoardArray.leaderBoardArr[levelIndex].ClearTime[orderIndex].ToString() + "초 ";//클리어 타임
            tmpText +=leaderBoardArray.leaderBoardArr[levelIndex].DateStr[orderIndex];//이름과 시간


            leaderBoardScoreTextArr[orderIndex].text = tmpText;
        }
    }
    #endregion

    #region 게임 랭킹 초기화(인스펙터에서 사용)
    [ContextMenu("ClearJson")]
    private void ClearJson()//Json 초기화, 인스펙터 창에서 확인
    {
        leaderBoardArray.isLoad = true;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                leaderBoardArray.leaderBoardArr[i].ClearTime[j] = 9999;
                leaderBoardArray.leaderBoardArr[i].DateStr[j] = "-";
            }
        }
        SaveJson();
    }
    #endregion

    #region 빨강 타워 날려버림(랭킹 실험용)
    [ContextMenu("DeleteRedTower")]
    private void DeleteRedTower()
    {
        //타워에 맥뎀 입혀버리기
        gameManager.redTowerManager.DamageControl(gameManager.redTowerManager.maxHealth);
    }
    #endregion
}