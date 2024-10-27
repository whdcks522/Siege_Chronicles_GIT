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
            return now.ToString("yy-MM-dd HH:mm");//return now.ToString("yyMMdd HH:mm");
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
            for (int i = 0; i < 3; i++)
            {
                leaderBoardArr[i] = new LeaderBoard();
            }
        }
    }
    public LeaderBoardArray leaderBoardArray = new LeaderBoardArray();
    #endregion

    

    private void Start()=> waitSec = new WaitForSeconds(timing);

    #region ������ ���� �ٲٱ�
    float RoundToDecimalPlace(float value, int decimalPlaces)
    {
        float factor = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(value * factor) / factor;
    }

    public void CheckJson(int gameLevel, float clearTime, bool isSave)
    {
        Debug.Log("�������� ���� ���� Ȯ��");//Ȯ����..

        clearTime = RoundToDecimalPlace(clearTime, 1);

        gameLevel -= 1;

        string dateStr = playerName + "(" + DateTimeUtils.GetCurrentDateTimeString() + ")";

        float tmpTime;
        string tmpStr;

        for(int i = 0; i < 3; i++)
        {
            if (clearTime < leaderBoardArray.leaderBoardArr[gameLevel].ClearTime[i]) //��ȭ�� �Ͼ ���
            {
                if (isSave)//�������� ��ȭ�� �����
                {
                    //������ ���� �ϴ� ������
                    tmpTime = leaderBoardArray.leaderBoardArr[gameLevel].ClearTime[i];
                    tmpStr = leaderBoardArray.leaderBoardArr[gameLevel].DateStr[i];

                    //���Ӱ� ������
                    leaderBoardArray.leaderBoardArr[gameLevel].ClearTime[i] = clearTime;
                    leaderBoardArray.leaderBoardArr[gameLevel].DateStr[i] = dateStr;

                    //��ĭ�� �б�
                    clearTime = tmpTime;
                    dateStr = tmpStr;
                }
                else //�������� ��ȭ�� ������� ����
                {
                    //�̸� �Է� Ȱ��ȭ
                    inputField.SetActive(true);
                    isLoad += 1;

                    break;
                }
            }
        }
    }
    #endregion

    #region ������ �����ϱ�
    string userId = "veHlMdhxfhVKn3scykFjU9fzeEf2";//�������� ������ ������ �� ���̵�
    public void SaveJson() 
    {
        Debug.Log("�������� �����ϱ�");

        string json = JsonUtility.ToJson(leaderBoardArray);

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("LeaderBoardArray").Child(userId).SetRawJsonValueAsync(json);
    }
    #endregion

    #region ������ �ҷ�����
    public void LoadJson()//�۵��ɶ����� ��ٸ�����
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        //���� ID�� ���� ���� �����Ϳ� ���� ���� ȹ��
        DatabaseReference achievementsRef = reference.Child("LeaderBoardArray").Child(userId);

        //�����ͺ��̽��κ��� �����͸� �о��
        achievementsRef.GetValueAsync().ContinueWith(task =>
        {
            Debug.Log("�������� �ҷ����� ��...");

            if (isLoad == -1) // �ε��� ����� �� �ִ� �÷��� Ȯ��(�¶��ο����� ���� �����ϴ���(��ø��))
            {
                Debug.Log("�������� �ε� ���");
                return;
            }

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
                    isLoad += 1;

                    Debug.Log("�������� �ҷ���(isLoad): " + isLoad);
                }
                else
                {
                    Debug.Log("�ش� ������ ���� ������ ����");
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

    #region �ڷ�ƾ
    WaitForSeconds waitSec;
    Coroutine FireCor;



    #region ���̾�̽� ���� �ʱ�ȭ(���� �ʱ�ȭ �� ������ ����)
    public void StopCor()
    {
        //�������� ��Ȱ��ȭ
        leaderBoardPanel.SetActive(false);
        inputField.SetActive(false);

        //�ε��� ����
        isLoad = -1;

        //�÷��̾� �̸��� �����
        playerName = "";

        //�������� ������ �ʱ�ȭ
        wifiObj.SetActive(false);

        if (FireCor != null)
        {
            //Debug.Log("���̾� �ڷ�ƾ�� �������̶�� ���� ��Ŵ");
            StopCoroutine(FireCor);
        }
        else 
        {
            //Debug.Log("���̾� �ڷ�ƾ�� �̹� �����");
        }
    }
    #endregion

    //��ŷ�� �̸��� �ֱ� ����

    public void StartCor() //�¸� �ϴ� ��� ȣ���
    {
        //�ҷ�����(�������� �� �¶��� ���� �ٲ㵵 �ν� �ǳ�?)
        //gameManager.fireManager.LoadJson();

        isLoad = 0;

        LoadJson();

        //�ҷ����� �Ϸ� ������ �ڷ�ƾ
        FireCor = StartCoroutine(UpdateCoroutine());
    }
    /*
    -1: ���� ��
    0: �� Ŭ����
    1: ������ �ҷ����� ����
    2: ��ŷ ��ȭ�� ����
    3: ��ŷ ��ȭ�� �ִ� ���, �ٽ� �ε� ��ٸ�����
    4: 1���� ������ �ҷ����� + ��ȭ �ִ� ���
    */
    public int isLoad = -1;//0�� ���� �ȺҸ�, 1�� ������ �� Ȯ��, 2�� ���� ������(��� ���������� ����)
    readonly string offLineStr = "Offline";
    readonly string onLineStr = "Online";
    IEnumerator UpdateCoroutine()
    {
        //�������� �̹��� Ȱ��ȭ
        wifiObj.SetActive(true);

        wifiObj.GetComponent<Image>().sprite = wifiSpriteArr[0];
        wifiObj.transform.GetChild(0).GetComponent<Animator>().SetBool("isFlash", true);

        wifiObj.transform.GetChild(0).GetComponent<Text>().text = offLineStr;
        wifiObj.transform.GetChild(0).GetComponent<Text>().color = gameManager.uiManager.textYellow;


        while (true)
        {
            if (isLoad >= 1)//�ε带 ������(���ͳ��� ����ž� �ҷ���)
            {
                //���⼭ �̸� ��ǲ �ʵ� Ȱ��ȭ(��ȭ�� ������� ����, ��ȭ ���θ� Ȯ��)
                if(isLoad == 1)
                    CheckJson(gameManager.gameLevel, gameManager.uiManager.curPlayTime, false);//�ű�� ������ ��� isLoad ����

                if (isLoad >= 2)//�ű�� ������ ��� 
                {
                    if (gameManager.OnlineCheck() && playerName != "")//�׸��� �¶����̸�(��ǲ �ʵ�� �̸� ����)
                    {
                        //��ŷ ���� �ҷ�����
                        if (isLoad == 2)//�ε��ϴµ� �ð��� �ɷ���
                        {
                            isLoad += 1;
                            LoadJson();
                        }

                        if (isLoad >= 4) 
                        {
                            //�ٲٰ�
                            CheckJson(gameManager.gameLevel, gameManager.uiManager.curPlayTime, true);

                            //�г� ���̵���
                            leaderBoardPanel.SetActive(true);

                            //�����ְ�
                            ShowJson(gameManager.gameLevel);

                            //�����ϰ�
                            SaveJson();

                            isLoad = -1;//savejSon�Ʒ� �־�� �� ��
                            playerName = "";

                            wifiObj.SetActive(false);

                            yield break;
                        }   
                    }
                }
                else //��ŷ ��ȭ�� ������
                {
                    //�г� ���̵���
                    leaderBoardPanel.SetActive(true);

                    //�����ֱ�
                    ShowJson(gameManager.gameLevel);

                    isLoad = -1;

                    wifiObj.SetActive(false);

                    yield break;
                }

            }//�ε带 ������(���ͳ��� ����ž� �ҷ���)

            //�ֱ����� �������� ������ ����
            if (gameManager.OnlineCheck())//���ͳݿ� ����� ���
            {
                if (wifiObj.GetComponent<Image>().sprite != wifiSpriteArr[1])//�� ����� �ٲ� ���, �ִϸ��̼� ��� 
                {
                    wifiObj.transform.GetChild(0).GetComponent<Animator>().SetBool("isFlash", true);
                }

                wifiObj.GetComponent<Image>().sprite = wifiSpriteArr[1];

                wifiObj.transform.GetChild(0).GetComponent<Text>().text = onLineStr;
                wifiObj.transform.GetChild(0).GetComponent<Text>().color = gameManager.uiManager.textGreen;
            }
            else //���ͳݿ� ������� ���� ���
            {
                if (wifiObj.GetComponent<Image>().sprite != wifiSpriteArr[0])//�� ����� �ٲ� ���, �ִϸ��̼� ��� 
                {
                    wifiObj.transform.GetChild(0).GetComponent<Animator>().SetBool("isFlash", true);
                }

                wifiObj.GetComponent<Image>().sprite = wifiSpriteArr[0];

                wifiObj.transform.GetChild(0).GetComponent<Text>().text = offLineStr;
                wifiObj.transform.GetChild(0).GetComponent<Text>().color = gameManager.uiManager.textYellow;
            }

            yield return waitSec;
        
        }//while
    }
    public GameObject wifiObj;
    public Sprite[] wifiSpriteArr;
    #endregion

    #region JSON �����ֱ�
    public Text[] leaderBoardScoreTextArr;
    public Button[] leaderBoardScoreButtonArr;
    public void ShowJson(int levelIndex) //� �ܰ��� ���� ���� ������ ��������
    {
        //Debug.Log(levelIndex + ": JSON �����ֱ�");

        levelIndex -= 1;

        //���� �ѱ�� ȿ����
        gameManager.audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        //���̵� ��ư ��ȣ�ۿ� ����
        for (int buttonIndex = 0; buttonIndex < 3; buttonIndex++) 
            leaderBoardScoreButtonArr[buttonIndex].interactable = true;
        leaderBoardScoreButtonArr[levelIndex].interactable = false;

        //�������� �ؽ�Ʈ ����
        string tmpText = "";

        for (int orderIndex = 0; orderIndex < 3; orderIndex++)//����� ���� ��� �ҷ����� �ҷ����� ��
        {
            tmpText = leaderBoardArray.leaderBoardArr[levelIndex].ClearTime[orderIndex].ToString() + "�� ";//Ŭ���� Ÿ��
            tmpText +=leaderBoardArray.leaderBoardArr[levelIndex].DateStr[orderIndex];//�̸��� �ð�

            leaderBoardScoreTextArr[orderIndex].text = tmpText;
        }
    }
    #endregion

    #region �̸� �Է�
    public void NameInput()
    {
        //���� �ѱ�� ȿ����
        gameManager.audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);

        playerName = inputField.GetComponent<InputField>().text;

        //�������� ������ �÷��� �ִϸ��̼� ���
        wifiObj.transform.GetChild(0).GetComponent<Animator>().SetBool("isFlash", true);
    }
    #endregion

    #region ���� ��ŷ �ʱ�ȭ(�ν����Ϳ��� ���)
    [ContextMenu("ClearJson")]
    private void ClearJson()//Json �ʱ�ȭ, �ν����� â���� Ȯ��
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

    #region ���� Ÿ�� ��������(�ν����Ϳ��� ���)
    [ContextMenu("DeleteRedTower")]
    private void DeleteRedTower()
    {
        //Ÿ���� �Ƶ� ����������
        gameManager.redTowerManager.DamageControl(gameManager.redTowerManager.maxHealth);
    }
    #endregion

    #region �ð� ���� �ʱ�ȭ(�ν����Ϳ��� ���)
    [ContextMenu("Reset_TimeScale")]
    
    private void Reset_TimeScale()
    {
        Time.timeScale = gameManager.defaultTimeScale;
    }
    #endregion
}