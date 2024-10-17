using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("ī�޶� ��ü")]
    public Transform cameraObj;
    [Header("ī�޶� ��ġ")]
    public Transform cameraCloud;
    [Header("ī�޶� �ٶ󺸴� ����")]
    public Transform cameraGround;

    Transform blueTower;//�Ķ� ���� ���� ����
    TowerManager blueTowerManager;//�Ķ� ���� ��ũ��Ʈ
    Transform redTower;//���� ���� ���� ����

    int mul = 45;//ī�޶� ȸ�� �ӵ�
    int curRot = -160;//���� ī�޶� ȸ����
    int addRot = 0;//��ư���� ȸ���� �� ����ϴ� ����
    int fly = 50;//ī�޶� �ϴÿ��� ��� ����
    Vector3 cameraVec;//ī�޶� ȸ���� ����

    [Header("���� UI")]
    public Slider PlayerResourceSlider;//�÷��̾��� �ڿ� �����̴�
    public Text PlayerResourceText;//�÷��̾��� �ڿ� �ؽ�Ʈ

    //�ֱٿ� ����� ������ ������
    public SpellData curSpellData;

    [Header("ũ���� ���� �ؽ�Ʈ")]
    public Text creatureCountText;
    public Animator creatureCountAnim;//Ÿ�� �Ŵ������� ���

    [Header("�ؽ�Ʈ �� ��ȭ")]
    public Color textYellow;
    public Color textRed;
    public Color textGreen;
    Color textWhite;
    public Color textBlue;

    [Header("�Ŵ���")]
    public SelectManager selectManager;
    public GameManager gameManager;
    AudioManager audioManager;


    private void Awake()
    {
        audioManager = gameManager.audioManager;
        blueTower = gameManager.blueTower;
        blueTowerManager = blueTower.GetComponent<TowerManager>();  
        redTower = gameManager.redTower;

        //���� �� ��, ī�޶� �� ��ġ Ȯ��
        cameraGround.transform.position = (blueTower.position + redTower.transform.position) / 2f;
        cameraCloud.transform.position = Vector3.up * fly + cameraGround.position;

        //ī�޶� �ʱ�ȭ
        CameraControl();

        //�÷��̾� �ڿ� �ؽ�Ʈ�� ����
        textWhite = PlayerResourceText.color;

        if (settingBackground.activeSelf) 
        {
            settingBackground.SetActive(false);
        }
    }

    [Header("���� ��ư ��� �ִϸ��̼�")]
    public Animator[] spellBtnAnim;//���� ��ư ��� �ִϸ��̼�
    public bool[] spellBtnAnimBool;//���� ��ư ��� �ִϸ��̼� �㰡
    private void Update()//fixed�ϸ�, ��Ŀ���� ���� ��� ������
    {
        if (addRot != 0)
        {
            CameraControl();
        }

        //�ڿ� ������ ����
        PlayerResourceSlider.value = blueTowerManager.curTowerResource / blueTowerManager.maxTowerResource;
        PlayerResourceText.text = blueTowerManager.curTowerResource.ToString("F1") + "/" + blueTowerManager.maxTowerResource.ToString("F0");

        if (blueTowerManager.curTowerResource >= blueTowerManager.maxTowerResource) PlayerResourceText.color = textGreen;
        else PlayerResourceText.color = textWhite;

        //���� �ڿ� ���� �����ֱ�
        for (int i = 0; i < spellBtnArr.Length; i++)
        {
            if (spellBtnArr[i].spellData != null)
            {
                //���� ��ư ���� ä���
                int value = spellBtnArr[i].spellData.isSale ? 
                    spellBtnArr[i].spellData.spellValue - blueTowerManager.curBankIndex :
                    spellBtnArr[i].spellData.spellValue;

                spellBtnArr[i].spellBtnIcon.fillAmount = blueTowerManager.curTowerResource / value;

                if (spellBtnArr[i].spellBtnIcon.fillAmount >= 1 && spellBtnAnimBool[i]) //�� �� ���
                {
                    //���� ��ư �ִϸ��̼� �۵�
                    spellBtnAnim[i].SetBool("isFlash", true);
                    spellBtnAnimBool[i] = false;

                    //���� ��ư �ؽ�Ʈ �� ��ȭ
                    spellBtnArr[i].spellBtnValue.color = textGreen;

                    //���̴� Ȱ��ȭ
                    spellBtnArr[i].spellBtnShader.gameObject.SetActive(true);
                }
                else if (spellBtnArr[i].spellBtnIcon.fillAmount < 1)//������ ��
                {
                    spellBtnAnimBool[i] = true;//���� ��ư ���ڰ� '����' �� �غ� �Ϸ�

                    //���� ��ư �ؽ�Ʈ �� ��ȭ
                    spellBtnArr[i].spellBtnValue.color = textBlue;

                    if(spellBtnArr[i].spellData.isSale && blueTowerManager.curBankIndex > 0)
                    {
                        spellBtnArr[i].spellBtnValue.color = textRed;
                    }

                    //���̴� ��Ȱ��ȭ
                    spellBtnArr[i].spellBtnShader.gameObject.SetActive(false);
                }
            }
        }

        //��Ŀ�������� ��ų ���� �̵�
        if (clickSphere.gameObject.activeSelf)
            ShowWeaponArea();

        //���� ��ư Ȱ��ȭ ����, ������ �ְ� ������ �ƴϸ鼭 ��ȣ�ۿ� ������ ��
        if (bankBtn.GetComponent<Button>().interactable)
        {
            bankBtn.fillAmount = blueTowerManager.curTowerResource / gameManager.BankValueArr[blueTowerManager.curBankIndex];

            if (bankBtn.fillAmount >= 1 && alreadyBankTouch)
            {
                //���� �ִϸ��̼� Ȱ��ȭ
                bankAnim.SetBool("isFlash", true);
                alreadyBankTouch = false;

                //�� �� ���, �ʷϻ� ����
                bankText.color = textGreen;
            }
            else if (bankBtn.fillAmount < 1)
            {
                alreadyBankTouch = true;

                //���� �ִ� ���, ����� ����
                bankText.color = textYellow;
            }
        }

        if (!settingBackground.activeSelf) //�ִ� �ð� ���⸦ ����
        {
            curPlayTime += Time.deltaTime;
            curPlayTimeText.text = curPlayTime.ToString("F1");
        }
    }
    public float curPlayTime;
    public Text curPlayTimeText;

    #region UI ���� �ʱ�ȭ
    public void resetUI() 
    {
        //��Ŀ�� �ʱ�ȭ
        FocusOff(false);

        curSpellData = null;

        //ũ���� �� ���� �ʱ�ȭ
        blueTowerManager.CreatureCountText();

        curPlayTime = 0;

        //�÷��̾� ���� �ʱ�ȭ
        bankText.text = "Lv." + (blueTowerManager.curBankIndex + 1) + "(" + gameManager.BankValueArr[blueTowerManager.curBankIndex] + ")";
        bankAnim.SetBool("isFlash", true);
        bankBtn.GetComponent<Button>().interactable = true;

        //��� �ʱ�ȭ
        if (speed == 1) 
        {
            SpeedControl(false);
        }
        SpeedAnim.SetBool("isFlash", true);

        //���� ��ư �ִϸ��̼� ����
        for (int i = 0; i < spellBtnArr.Length; i++) 
        {
            if (spellBtnArr[i].spellData != null) 
            {
                //���� ��ư �ִϸ��̼� �۵�
                spellBtnAnim[i].SetBool("isFlash", true);
                spellBtnAnimBool[i] = false;

                //�� �ʱ�ȭ
                spellBtnArr[i].spellBtnValue.text = spellBtnArr[i].spellData.spellValue.ToString();
            }    
        }

        //ī�޶� ȸ�� �ʱ�ȭ
        curRot = -160;
        CameraControl();

        //���� ȭ�� ��Ȱ��ȭ
        SettingControl(false);
    }
    #endregion

    #region ��� ����
    [Header("��� �ؽ�Ʈ�� �ִϸ��̼�")]
    int speed = 0;
    public Text SpeedControlText;
    public Animator SpeedAnim;
    public void SpeedControl(bool isSfx)
    {
        if (isSfx) 
        {
            //�ӵ� ���� ȿ���� ���
            audioManager.PlaySfx(AudioManager.Sfx.SpeedSfx);

            SpeedAnim.SetBool("isFlash", true);
        }

        //�ӵ� ����
        speed++;
        speed = (speed % 2);

        //���� �ؽ�Ʈ �� ����
        if (speed == 0) SpeedControlText.color = textYellow;
        else SpeedControlText.color = textRed;

        //�ð� ����
        Time.timeScale = (speed + 1);

        //���� ��ȯ
        SpeedControlText.text = "x" + (speed + 1);

        
    }
    #endregion

    #region ���� ����

    [Header("���� ���� UI")]
    public Image bankBtn;//���� �̹���
    public Text bankText;//���� �ؽ�Ʈ
    public Animator bankAnim;//���� �ִϸ��̼�
    bool alreadyBankTouch = true;//�� ä���� ���� �ִϸ��̼��� ����
    public void BankControl()//���� ��ư Ŭ��
    {
        bankAnim.SetBool("isFlash", true);

        if (blueTowerManager.curTowerResource >= gameManager.BankValueArr[blueTowerManager.curBankIndex])//����� ����� ���
        {
            //���� ȿ����
            audioManager.PlaySfx(AudioManager.Sfx.BankSfx);

            //��� ó��
            blueTowerManager.curTowerResource -= gameManager.BankValueArr[blueTowerManager.curBankIndex];

            //���� ��� ����
            blueTowerManager.curBankIndex++;

            if (blueTowerManager.curBankIndex != gameManager.BankValueArr.Length)//0, 1, 2, 3�� �� Ŭ���� ���
            {
                //���� �ؽ�Ʈ ����
                bankText.text = "Lv." + (blueTowerManager.curBankIndex + 1) + "(" + gameManager.BankValueArr[blueTowerManager.curBankIndex] + ")";
            }
            else if (blueTowerManager.curBankIndex == gameManager.BankValueArr.Length)//4�� �� Ŭ���� ���
            {
                //���� �ؽ�Ʈ ����
                bankText.text = "Lv.5(-)";
                //�̹��� ä���
                bankBtn.fillAmount = 1;
                //��ư Ŭ�� ��Ȱ��ȭ
                bankBtn.GetComponent<Button>().interactable = false;

                bankText.color = textRed;
            }

            //���� ��ư �ִϸ��̼� ����
            for (int i = 0; i < spellBtnArr.Length; i++)
            {
                if (spellBtnArr[i].spellData != null)
                {
                    //�� �ʱ�ȭ
                    int value = spellBtnArr[i].spellData.isSale ?
                    spellBtnArr[i].spellData.spellValue - blueTowerManager.curBankIndex :
                    spellBtnArr[i].spellData.spellValue;

                    spellBtnArr[i].spellBtnValue.text = value.ToString();
                }
            }
        }
        else if (blueTowerManager.curTowerResource < gameManager.BankValueArr[blueTowerManager.curBankIndex]) //����� ���ڸ� ���
        {
            //��� ���� ȿ����
            audioManager.PlaySfx(AudioManager.Sfx.SpellFailSfx);
        }
    }
    #endregion


    #region ��ư���� ī�޶� ���� ����
    public void CameraSpin(int _spin) => addRot = _spin;
    Quaternion cameraRotation;
    void CameraControl()//ī�޶� ���ۿ� ���� ȭ�� ����
    {
        //���̵� ȭ��ǥ ��ư ������ ī�޶� ȸ��
        curRot += addRot * 2;

        //ī�޶� ��ġ ����(������ ���� ����)
        cameraVec = mul * new Vector3(Mathf.Sin(Mathf.PI * curRot / 360), 0, Mathf.Cos(Mathf.PI * curRot / 360));
        cameraObj.position = cameraCloud.position + cameraVec;

        //���Ͱ� ī�޶� ���ϵ��� ����
        cameraObj.LookAt((blueTower.position + redTower.position) / 2f);

        // ��ü A���� B�� �ٶ󺸴� ȸ�� ���ϱ�
        cameraVec = cameraGround.transform.position - cameraObj.transform.position;
        cameraRotation = Quaternion.LookRotation(cameraVec);

        //Ÿ�� UI�� ȸ�� ����
        blueTowerManager.miniCanvas.transform.rotation = cameraRotation;
        gameManager.redTowerManager.miniCanvas.transform.rotation = cameraRotation;
    }
    #endregion


    #region ���� ������ ���� ��ư Ŭ��

    [Header("���� �����ư �迭")]
    public SpellButton[] spellBtnArr = new SpellButton[4];
    Vector3 clickScaleVec;//�ּ��� ��ų ������ �����ִµ� ���Ǵ� ����

    public void OnClick(int _index) //���� ȭ�鿡�� ���� 4���� ��ư �� 1���� Ŭ����
    {
        //�ִϸ��̼� �۵�
        spellBtnAnim[_index].SetBool("isFlash", true);

        //Ŭ���� ��ư�� ���� ����
        SpellData spellData = spellBtnArr[_index].GetComponent<SpellButton>().spellData;
        //�ش� ������ ��� ���(������ ������ ����)
        int value = spellData.isSale ? spellData.spellValue - blueTowerManager.curBankIndex: spellData.spellValue;

        if (blueTowerManager.curTowerResource >= value && curSpellData == null)//����� ����ϸ鼭 ��Ŀ�� ���� �ƴ϶��
        {
            if (spellData.spellType == SpellData.SpellType.Creature)//ũ���ĸ� ���� ���
            {
                if (blueTowerManager.CreatureCountCheck())//��ȯ ������ ���
                {
                    //�ڿ� ����
                    blueTowerManager.curTowerResource -= value;

                    //�ش� ũ���� ��ȯ
                    blueTowerManager.SpawnCreature(spellData.spellPrefab.name);

                    //���� ���� ȿ����
                    audioManager.PlaySfx(AudioManager.Sfx.SpellSuccessSfx);
                }
                else 
                {
                    //���� ���� ȿ����
                    audioManager.PlaySfx(AudioManager.Sfx.SpellFailSfx);
                }
            }
            else //if(spellData.spellType == SpellData.SpellType.Weapon) //�ּ��� ���� ���
            {
                //���� ���� ȿ����
                audioManager.PlaySfx(AudioManager.Sfx.SpellSuccessSfx);

                //��� ����
                blueTowerManager.curTowerResource -= value;

                if (spellData.isFocus) 
                {
                    //���� ������ �ӽ� ����
                    curSpellData = spellData;

                    //Ŭ�� ����Ʈ�� ���͸��� ��ȭ
                    clickMat.SetColor("_AlphaColor", spellData.focusColor);

                    //Ŭ�� ����Ʈ�� ũ�� ��ȭ
                    float size = spellData.spellPrefab.transform.localScale.x;
                    Bullet bullet = spellData.spellPrefab.GetComponent<Bullet>();

                    if (bullet.endBullet != null)//�ڽ��� ������, �ڽ��� ũ��� ����
                        size = bullet.endBullet.transform.localScale.x;

                    clickScaleVec = new Vector3(size, size, size);
                    clickSphere.localScale = clickScaleVec;
                }
                else if(!spellData.isFocus)
                {
                    blueTowerManager.WeaponSort(spellData.spellPrefab.name);
                } 
            }
        }
        else if (blueTowerManager.curTowerResource < value) //����� ���ڸ� ���
        {
            //���� ���� ȿ����
            audioManager.PlaySfx(AudioManager.Sfx.SpellFailSfx);
        }
    }
    #endregion

    #region ���� ��ư ���
    [Header("���� ���")]
    public GameObject settingBackground;//���� â ���
    public GameObject victoryTitle;//�¸� �� ���� �ؽ�Ʈ
    public GameObject defeatTitle;//�й� �� ���� �ؽ�Ʈ

    public Button setBtn;//�̾��ϱ� ��ư(���� ���� ��, ��Ȱ��ȭ)
    public Animator setBtnAnim;//���� ��ư �÷��� �ִϸ��̼�
    public Animator[] setBtnAnimArr;//���ÿ��� ����� ��ư���� �ִϸ��̼� 
    public void SettingControl(bool isOpen)//���� Ȱ��ȭ ����
    {
        if(isOpen)//��Ŀ�� �ʱ�ȭ
        {
            if (curSpellData != null)//��Ŀ�� �����̶��
            {
                //�ڿ� ��ȯ
                blueTowerManager.curTowerResource += curSpellData.spellValue;
                curSpellData = null;

                //���� ���� ��Ȱ��ȭ
                clickSphere.gameObject.SetActive(false);
                //�� Ȱ��ȭ
                worldLight.color = brightColor;
            }
            else 

            FocusOff(false); //������ �̰� �ϳ�
        }

        //�̹��� ����
        settingBackground.SetActive(isOpen);

        //�ð� ����
        if (isOpen)
        {
            Time.timeScale = 0.001f;

            //���� ��ư �ִϸ��̼� ���(��ư ��ȣ�ۿ� ����)
            for (int index = 0; index <= 3; index++)
                setBtnAnimArr[index].SetTrigger("isActivate");
        }
        else //���� ��� �ð� ���� �ʱ�ȭ
        {
            SpeedControl(false);
            SpeedControl(false);
        }

        //���� �ؽ�Ʈ �÷��� �ִϸ��̼� ���
        setBtnAnim.SetBool("isFlash", true);
    }

    #region ���� ȭ�� ��ư Ŭ��
    public void SettingBtnClick() 
    {
        if (settingBackground.activeSelf)//���� ȭ���� ���� ���¶�� 
        {
            SettingControl(false);
        }
        else //���� ȭ���� ���� ���¿��� �����ٸ�
        {
            SettingControl(true);
        }
        //���� ȿ���� ���
        playSfxPaper();
    }
    #endregion

    public void playSfxPaper()//���� ��ư ������ ��, ȿ������ ����
    {
        //������ ȿ����
        audioManager.PlaySfx(AudioManager.Sfx.PaperSfx);
    }
    #endregion

    #region ��Ŀ�� ���� ��ȯ
    [Header("������ ��")]
    public Light worldLight;
    public Color brightColor;//���� ������ ���� ���� �� ����
    public Color paleColor;//���� ���� ���� ���� �� ����


    public void FocusOn()//���� ������� �ּ� ������ ����(��Ŀ�� ����)
    {
        //Debug.LogWarning("On_1: " + gameManager.redTowerManager.curHealth);

        if (!settingBackground.activeSelf)
        {
            //Debug.LogWarning("On_2: " + gameManager.redTowerManager.curHealth);
            if (curSpellData != null)
            {
                //Debug.LogWarning("On_3: " + gameManager.redTowerManager.curHealth);

                //���� ���� Ȱ��ȭ
                clickSphere.gameObject.SetActive(true);
                //�� ��Ȱ��ȭ
                worldLight.color = paleColor;
                //�ð� ����
                Time.timeScale = 0.2f;
            }
        }
    }
    public void FocusOff(bool isReturn) //�ڿ� ��ȯ ����(��Ŀ�� ����)
    {
        //Debug.LogWarning("Off_1: " + gameManager.redTowerManager.curHealth);

        if (!settingBackground.activeSelf)//����� �� �����ִ� ��쿡��
        {
            //Debug.LogWarning("Off_2: " + gameManager.redTowerManager.curHealth);

            if (curSpellData != null && isReturn)
            {
                if (!clickSphere.gameObject.activeSelf)
                {
                    //�ڿ� ��ȯ
                    blueTowerManager.curTowerResource += curSpellData.spellValue;
                    curSpellData = null;
                }
                else if (clickSphere.gameObject.activeSelf)
                {
                    //�ּ�(��ų) ���
                    blueTowerManager.WeaponSort(curSpellData.spellPrefab.name);
                    curSpellData = null;
                }
            }

            //���� ���� ��Ȱ��ȭ
            clickSphere.gameObject.SetActive(false);
            //�� Ȱ��ȭ
            worldLight.color = brightColor;
            //�ð� ����ȭ
            SpeedControl(false);
            SpeedControl(false);
        }
    }
    #endregion


    #region �ּ�(��ų) ���� ǥ��;

    [Header("��Ŀ�� ���� ��ҵ�")]
    public Transform clickSphere;//Ŭ���� ���� ���� ����
    public Material clickMat;//Ŭ���� ���� ���� ������ ���͸��� 

    void ShowWeaponArea()//ȭ�� ������ ��ġ�� ������
    {
        int layerMask = LayerMask.GetMask("MainMap"); // "MainMap" ���̾�� �浹�ϵ��� ����

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//ī�޶󿡼� ������ ���� ������ ������ Ȯ��
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            // Ʈ���Ŵ� �����Ѵ�
            clickSphere.position = hit.point;

        //Ÿ�� ���̴� ����
        blueTowerManager.RadarControl(clickSphere.position);
    }
    #endregion

    #region ���� �� ��, ���͸��� �� ����(�� ��긦 ����)

    private void OnDisable()
    {
        clickMat.SetColor("_AlphaColor", textYellow);
    }
    #endregion
}
