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
        public bool isLoad = false;

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

    //public ParticleImage Congrateparticle;
    public void ChangeJson(int gameLevel, float clearTime) 
    {
        Debug.Log("JSON �ٲٱ�");

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
        Debug.Log("JSON �����ϱ�");

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
                    Debug.Log("JSON �ҷ���");
                }
                else
                {
                    Debug.Log("�ش� ������ ���� ������ ����");
                }
            }
        });
    }
    public float timing;

    
    public GameManager gameManager;

    WaitForSeconds waitSec;
    Coroutine FireCor;
    private void Start()
    {
        waitSec = new WaitForSeconds(timing);
    }

    public void StartCor() 
    {
        FireCor = StartCoroutine(UpdateCoroutine());
    }

    public GameObject leaderBoardPanel;
    public void StopCor() 
    {
        //�������� ��Ȱ��ȭ
        leaderBoardPanel.SetActive(false);

        //�ε��� ����
        leaderBoardArray.isLoad = false;

        if (FireCor != null)
        {
            Debug.Log("���̾� �ڷ�ƾ�� �������̶�� ���� ��Ŵ");
            StopCoroutine(FireCor);
        }
        else 
        {
            Debug.Log("���̾� �ڷ�ƾ�� �̹� �����");
        }
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (leaderBoardArray.isLoad)
            {
                if (gameManager.OnlineCheck()) 
                {
                    //�ٲٰ� - �����ϰ� - �����ֱ�
                    leaderBoardPanel.SetActive(true);

                    ChangeJson(gameManager.gameLevel, gameManager.uiManager.curPlayTime);

                    ShowJson(gameManager.gameLevel);

                    SaveJson();

                    leaderBoardArray.isLoad = false;
                    yield break;
                }  
            }

            Debug.Log("JSON �ҷ����� ���� ����ȭ ��..");
            yield return waitSec;
        }
    }

    public Text[] leaderBoardScoreTextArr;
    public Button[] leaderBoardScoreButtonArr;
    public void ShowJson(int levelIndex) 
    {
        Debug.Log(levelIndex + ": JSON �����ֱ�");

        //���� �ѱ�� ȿ����
        gameManager.audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        levelIndex -= 1;

        for (int buttonIndex = 0; buttonIndex < leaderBoardScoreButtonArr.Length; buttonIndex++) 
        {
            leaderBoardScoreButtonArr[buttonIndex].interactable = true;
        }
        leaderBoardScoreButtonArr[levelIndex].interactable = false;

        //�������� �ؽ�Ʈ ����
        string tmpText = "";

        for (int orderIndex = 0; orderIndex < 3; orderIndex++)
        {
            tmpText = leaderBoardArray.leaderBoardArr[levelIndex].ClearTime[orderIndex].ToString();//Ŭ���� Ÿ��
            tmpText += "(" +leaderBoardArray.leaderBoardArr[levelIndex].DateStr[orderIndex]+ ")";//�ð�


            leaderBoardScoreTextArr[orderIndex].text = tmpText;
        }
    }
    #endregion

    public void onClick(int index) 
    {
        Debug.Log(index);
    }

    [ContextMenu("ClearJson")]
    private void ClearJson()//Json �ʱ�ȭ, �ν����� â���� Ȯ��
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

}