using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using static Creature;

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

    

    //불 공격력 90, 자원: 5, 건물 피격
    #region 순위 바꾸기
    float RoundToDecimalPlace(float value, int decimalPlaces)
    {
        float factor = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(value * factor) / factor;
    }
    public void ChangeJson(int gameLevel, float clearTime) 
    {
        Debug.Log("JSON 바꾸기");

        clearTime = RoundToDecimalPlace(clearTime, 1);

        gameLevel -= 1;

        string dateStr = DateTimeUtils.GetCurrentDateTimeString(); ; 
        bool isChange = false;

        float tmpTime;
        string tmpStr;

        for(int i = 0; i < 3; i++)
        {
            if (clearTime < leaderBoardArray.leaderBoardArr[gameLevel].ClearTime[i]) //0일수록 빨리 끝낸 것
            {
                isChange = true;

                tmpTime = leaderBoardArray.leaderBoardArr[gameLevel].ClearTime[i];
                tmpStr = leaderBoardArray.leaderBoardArr[gameLevel].DateStr[i];

                leaderBoardArray.leaderBoardArr[gameLevel].ClearTime[i] = clearTime;
                leaderBoardArray.leaderBoardArr[gameLevel].DateStr[i] = dateStr;

                clearTime = tmpTime;
                dateStr = tmpStr;
            }
        }

        if (isChange)
        {
            Debug.Log("순위 변화가 일어남");
        }
    }
    #endregion

    #region 데이터 저장하기
    string userId = "veHlMdhxfhVKn3scykFjU9fzeEf2";
    public void SaveJson() //데이터 저장하기
    {
        Debug.Log("JSON 저장하기");

        string json = JsonUtility.ToJson(leaderBoardArray);

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("LeaderBoardArray").Child(userId).SetRawJsonValueAsync(json);

        Debug.Log("Save Data Success!");
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
    public float timing;

    public LeaderBoardInfo[] leaderBoardInfoArr;
    public GameManager gameManager;

    WaitForSeconds waitSec;
    private void Start()
    {
        waitSec = new WaitForSeconds(timing);
    }

    public void StartCor() 
    {
        StartCoroutine(UpdateCoroutine());
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (leaderBoardArray.isLoad)
            {
                //바꾸고 - 저장하고 - 보여주기

                ChangeJson(gameManager.gameLevel, gameManager.uiManager.curPlayTime);

                ShowJson();

                SaveJson();

                leaderBoardArray.isLoad = false;
                yield break;
            }

            Debug.Log("Finding..");
            yield return waitSec;
        }
    }


    public void ShowJson() 
    {
        Debug.Log("JSON 보여주기");

        gameManager.leaderBoardPanelArr.SetActive(true);

        //리더보드 텍스트 변경
        string tmpText = "";

        //int levelIndex, int orderIndex
        for (int levelIndex = 0; levelIndex < 3; levelIndex++) 
        {
            for (int orderIndex = 0; orderIndex < 3; orderIndex++)
            {
                tmpText =
            leaderBoardArray.leaderBoardArr[levelIndex].ClearTime[orderIndex].ToString() +
            "(" +
            leaderBoardArray.leaderBoardArr[levelIndex].DateStr[orderIndex]
            + ")";
                leaderBoardInfoArr[levelIndex].leaderScoreTextArr[orderIndex].text = tmpText;
            }
        }
    }
    #endregion

}