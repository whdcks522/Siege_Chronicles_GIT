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
            return now.ToString("yy-MM-dd HH:mm");//return now.ToString("yyMMdd HH:mm");
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

        public LeaderBoardArray()
        {
            // 각 LeaderBoard 객체 초기화
            for (int i = 0; i < 3; i++)
            {
                leaderBoardArr[i] = new LeaderBoard();
            }
        }
    }
    public LeaderBoardArray leaderBoardArray = new LeaderBoardArray();
    #endregion

    

    private void Start()=> waitSec = new WaitForSeconds(timing);

    #region 데이터 순위 바꾸기
    float RoundToDecimalPlace(float value, int decimalPlaces)
    {
        float factor = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(value * factor) / factor;
    }

    public void CheckJson(int gameLevel, float clearTime, bool isSave) 
    {
        Debug.Log("JSON 내용 바꾸기");//확인중..

        clearTime = RoundToDecimalPlace(clearTime, 1);

        gameLevel -= 1;

        string dateStr = playerName + "(" + DateTimeUtils.GetCurrentDateTimeString() + ")";

        float tmpTime;
        string tmpStr;

        for(int i = 0; i < 3; i++)
        {
            if (clearTime < leaderBoardArray.leaderBoardArr[gameLevel].ClearTime[i]) //변화가 일어난 경우
            {
                if (isSave)//리더보드 변화를 허용함
                {
                    //기존의 것을 일단 저장함
                    tmpTime = leaderBoardArray.leaderBoardArr[gameLevel].ClearTime[i];
                    tmpStr = leaderBoardArray.leaderBoardArr[gameLevel].DateStr[i];

                    //새롭게 갱신함
                    leaderBoardArray.leaderBoardArr[gameLevel].ClearTime[i] = clearTime;
                    leaderBoardArray.leaderBoardArr[gameLevel].DateStr[i] = dateStr;

                    //한칸씩 밀기
                    clearTime = tmpTime;
                    dateStr = tmpStr;
                }
                else //리더보드 변화를 허용하지 않음
                {
                    //이름 입력 활성화
                    inputField.SetActive(true);
                    isLoad += 1;

                    break;
                }
            }
        }
    }
    #endregion

    #region 데이터 저장하기
    string userId = "veHlMdhxfhVKn3scykFjU9fzeEf2";//리더보드 내용을 저장할 내 아이디
    public void SaveJson() 
    {
        Debug.Log("JSON 저장하기");

        string json = JsonUtility.ToJson(leaderBoardArray);

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("LeaderBoardArray").Child(userId).SetRawJsonValueAsync(json);
    }
    #endregion

    #region 데이터 불러오기
    public void LoadJson()//작동될때까지 기다리더라
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        //유저 ID를 통해 업적 데이터에 대한 참조 획득
        DatabaseReference achievementsRef = reference.Child("LeaderBoardArray").Child(userId);

        //데이터베이스로부터 데이터를 읽어옴
        achievementsRef.GetValueAsync().ContinueWith(task =>
        {
            Debug.Log("데이터 불러오는 중...");

            if (isLoad == -1) // 로딩을 취소할 수 있는 플래그 확인(온라인에서만 중지 가능하더라(중첩됨))
            {
                Debug.Log("데이터 로딩 취소");
                return;
            }

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
                    isLoad += 1;

                    Debug.Log("데이터 불러옴(단계): " + isLoad);
                }
                else
                {
                    Debug.Log("해당 유저에 대한 정보가 없음");
                }
            }
        });
    }
    #endregion

    readonly float timing = 0.0004f;
    public GameManager gameManager;

    string playerName = "";
    public GameObject leaderBoardPanel;
    public GameObject inputField;

    #region 코루틴
    WaitForSeconds waitSec;
    Coroutine FireCor;



    #region 파이어베이스 설정 초기화(전투 초기화 할 때마다 수행)
    public void StopCor()
    {
        //리더보드 비활성화
        leaderBoardPanel.SetActive(false);
        inputField.SetActive(false);

        //로딩중 해제
        isLoad = -1;

        //플레이어 이름을 비워둠
        playerName = "";

        //와이파이 아이콘 초기화
        wifiObj.SetActive(false);

        if (FireCor != null)
        {
            //Debug.Log("파이어 코루틴이 진행중이라면 종료 시킴");
            StopCoroutine(FireCor);
        }
        else 
        {
            //Debug.Log("파이어 코루틴이 이미 종료됨");
        }
    }
    #endregion

    //랭킹에 이름을 넣기 위함

    public void StartCor() //승리 하는 경우 호출됨
    {
        //불러오고(오프라인 후 온라인 으로 바꿔도 인식 되나?)
        //gameManager.fireManager.LoadJson();

        isLoad = 0;

        LoadJson();

        //불러오기 완료 까지의 코루틴
        FireCor = StartCoroutine(UpdateCoroutine());
    }
    /*
    -1: 전투 중
    0: 막 클리어
    1: 데이터 불러오기 성공
    2: 랭킹 변화가 있음
    4: 1번만 데이터 불러오기 + 변화 있는 경우
    */
    public int isLoad = -1;//0은 아직 안불림, 1은 갱신한 것 확인, 2는 새로 저장함(기록 없어질수도 있음)
    IEnumerator UpdateCoroutine()
    {
        //와이파이 이미지 활성화
        wifiObj.SetActive(true);

        wifiObj.GetComponent<Image>().sprite = wifiSpriteArr[0];
        wifiObj.transform.GetChild(0).GetComponent<Animator>().SetBool("isFlash", true);

        wifiObj.transform.GetChild(0).GetComponent<Text>().text = "Offline";
        wifiObj.transform.GetChild(0).GetComponent<Text>().color = gameManager.uiManager.textYellow;


        while (true)
        {
            if (isLoad >= 1)//로드를 했으면(인터넷이 연결돼야 불러옴)
            {
                //여기서 이름 인풋 필드 활성화(변화를 허용하지 않음, 변화 여부만 확인)
                if(isLoad == 1)
                    CheckJson(gameManager.gameLevel, gameManager.uiManager.curPlayTime, false);//신기록 갱신한 경우 isLoad 증가

                if (isLoad >= 2)//신기록 갱신한 경우 
                {
                    if (gameManager.OnlineCheck() && playerName != "")//그리고 온라인이면(인풋 필드로 이름 삽입)
                    {
                        //랭킹 새로 불러오기
                        if (isLoad == 2)
                        {
                            isLoad += 1;
                            LoadJson();
                        }

                        if (isLoad >= 4) 
                        {
                            //바꾸고
                            CheckJson(gameManager.gameLevel, gameManager.uiManager.curPlayTime, true);

                            //패널 보이도록
                            leaderBoardPanel.SetActive(true);

                            //보여주고
                            ShowJson(gameManager.gameLevel);

                            //저장하고
                            SaveJson();

                            isLoad = -1;//savejSon아래 있어야 할 듯
                            playerName = "";

                            wifiObj.SetActive(false);

                            yield break;
                        }   
                    }
                }
                else //랭킹 변화가 없으면
                {
                    //패널 보이도록
                    leaderBoardPanel.SetActive(true);

                    //보여주기
                    ShowJson(gameManager.gameLevel);

                    isLoad = -1;

                    wifiObj.SetActive(false);

                    yield break;
                }

            }//로드를 했으면(인터넷이 연결돼야 불러옴)

            //주기적인 와이파이 아이콘 변경
            if (gameManager.OnlineCheck())//인터넷에 연결된 경우
            {
                if (wifiObj.GetComponent<Image>().sprite != wifiSpriteArr[1])//막 여기로 바뀐 경우, 애니메이션 재생 
                {
                    wifiObj.transform.GetChild(0).GetComponent<Animator>().SetBool("isFlash", true);
                }

                wifiObj.GetComponent<Image>().sprite = wifiSpriteArr[1];

                wifiObj.transform.GetChild(0).GetComponent<Text>().text = "Online";
                wifiObj.transform.GetChild(0).GetComponent<Text>().color = gameManager.uiManager.textGreen;
            }
            else //인터넷에 연결되지 않은 경우
            {
                if (wifiObj.GetComponent<Image>().sprite != wifiSpriteArr[0])//막 여기로 바뀐 경우, 애니메이션 재생 
                {
                    wifiObj.transform.GetChild(0).GetComponent<Animator>().SetBool("isFlash", true);
                }

                wifiObj.GetComponent<Image>().sprite = wifiSpriteArr[0];

                wifiObj.transform.GetChild(0).GetComponent<Text>().text = "Offline";
                wifiObj.transform.GetChild(0).GetComponent<Text>().color = gameManager.uiManager.textYellow;
            }

            yield return waitSec;
        
        }//while
    }
    public GameObject wifiObj;
    public Sprite[] wifiSpriteArr;
    #endregion

    #region JSON 보여주기
    public Text[] leaderBoardScoreTextArr;
    public Button[] leaderBoardScoreButtonArr;
    public void ShowJson(int levelIndex) //어떤 단계의 리더 보드 내용을 보여줄지
    {
        //Debug.Log(levelIndex + ": JSON 보여주기");

        levelIndex -= 1;

        //종이 넘기는 효과음
        gameManager.audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        //난이도 버튼 상호작용 관리
        for (int buttonIndex = 0; buttonIndex < 3; buttonIndex++) 
            leaderBoardScoreButtonArr[buttonIndex].interactable = true;
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

    #region 이름 입력
    public void NameInput()
    {
        //종이 넘기는 효과음
        gameManager.audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        playerName = inputField.GetComponent<InputField>().text;

        //와이파이 아이콘 플래시 애니메이션 재생
        wifiObj.transform.GetChild(0).GetComponent<Animator>().SetBool("isFlash", true);
    }
    #endregion

    #region 게임 랭킹 초기화(인스펙터에서 사용)
    [ContextMenu("ClearJson")]
    private void ClearJson()//Json 초기화, 인스펙터 창에서 확인
    {
        //leaderBoardArray.isLoa2d = 0;

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

    #region 빨강 타워 날려버림(인스펙터에서 사용)
    [ContextMenu("DeleteRedTower")]
    private void DeleteRedTower()
    {
        //타워에 맥뎀 입혀버리기
        gameManager.redTowerManager.DamageControl(gameManager.redTowerManager.maxHealth);
    }
    #endregion

    #region 시간 배율 초기화(인스펙터에서 사용)
    [ContextMenu("Reset_TimeScale")]
    
    private void Reset_TimeScale()
    {
        Time.timeScale = gameManager.defaultTimeScale;
    }
    #endregion
}