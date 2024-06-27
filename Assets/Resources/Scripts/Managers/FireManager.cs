using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;

public class FireManager : MonoBehaviour
{
    #region ��¥ ���
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

    #region ���� ���
    [System.Serializable]
    public class LeaderBoard
    {
        public string[] DateStr = new string[3];//���̵� ���� 3���� ��¥
        public float[] ClearTime = new float[3];//���̵� ���� 3���� �ð�
    }

    [Serializable]
    public class LeaderBoardArray
    {
        public LeaderBoard[] leaderBoardArr = new LeaderBoard[3];//���̵����� �Ѱ��� ����

        public LeaderBoardArray()
        {
            // �� LeaderBoard ��ü �ʱ�ȭ
            for (int i = 0; i < leaderBoardArr.Length; i++)
            {
                leaderBoardArr[i] = new LeaderBoard();
            }
        }
    }
    public LeaderBoardArray leaderBoardArray = new LeaderBoardArray();
    #endregion

    

    //�� ���ݷ� 90, �ڿ�: 5, �ǹ� �ǰ�
    #region ���� �ٲٱ�
    float RoundToDecimalPlace(float value, int decimalPlaces)
    {
        float factor = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(value * factor) / factor;
    }
    public void ChangeJson(int gameLevel, float clearTime) 
    {
        clearTime = RoundToDecimalPlace(clearTime, 1);

        gameLevel -= 1;

        string dateStr = DateTimeUtils.GetCurrentDateTimeString(); ; 
        bool isChange = false;

        float tmpTime;
        string tmpStr;

        for(int i = 0; i < 3; i++)
        {
            if (clearTime < leaderBoardArray.leaderBoardArr[gameLevel].ClearTime[i]) //0�ϼ��� ���� ���� ��
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
            Debug.Log("���� ��ȭ�� �Ͼ");
        }
    }
    #endregion

    #region ������ �����ϱ�
    string userId = "veHlMdhxfhVKn3scykFjU9fzeEf2";
    public void SaveJson() //������ �����ϱ�
    {
        string json = JsonUtility.ToJson(leaderBoardArray);

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("LeaderBoardArray").Child(userId).SetRawJsonValueAsync(json);

        Debug.Log("Save Data Success!");
    }
    #endregion

    #region ������ �ҷ�����
    public void LoadJson()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        //���� ID�� ���� ���� �����Ϳ� ���� ���� ȹ��
        DatabaseReference achievementsRef = reference.Child("LeaderBoardArray").Child(userId);//playerId

        //�����ͺ��̽��κ��� �����͸� �о��
        achievementsRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("�����ͺ��̽��κ��� �����͸� �дµ� ������: " + task.Exception.Flatten().InnerExceptions);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;

                if (dataSnapshot != null && dataSnapshot.Exists)
                {
                    // ���������κ��� JSON �����͸� ������
                    string json = dataSnapshot.GetRawJsonValue();

                    leaderBoardArray = JsonUtility.FromJson<LeaderBoardArray>(json);

                    /*
                    ShowJson(0, 0);
                    ShowJson(0, 1);
                    ShowJson(0, 2);
                    ShowJson(1, 0);
                    ShowJson(1, 1);
                    ShowJson(1, 2);
                    ShowJson(2, 0);
                    ShowJson(2, 1);
                    ShowJson(2, 2);
                    */

                    //ShowJson();
                    // UI ���� �۾��� ���� �����忡�� ����

                    
                }
                else
                {
                    Debug.Log("�ش� ������ ���� ������ ����");
                }
            }
        });

        //Invoke("ShowJson", timing);
    }
    public float timing;

    public LeaderBoardInfo[] leaderBoardInfoArr;

    

    public void ShowJson() 
    {
        //�������� �ؽ�Ʈ ����
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