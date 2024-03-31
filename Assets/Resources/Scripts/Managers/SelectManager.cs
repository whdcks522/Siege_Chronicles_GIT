using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static SpellData;

public class SelectManager : MonoBehaviour
{
    [Header("���õ� ���� ��ư")]
    public SpellButton selectedSpellBtn;

    [Header("���õ� ���� ������")]
    public Image selectedSpellIcon;

    [Header("���õ� ���� �̸�")]
    public Text selectedSpellName;

    [Header("���õ� ���� Ÿ��")]
    public Text selectedSpellType;

    [Header("���õ� ���� ���")]
    public Text selectedSpellValue;

    [Header("���õ� ���� ����")]
    public Text selectedSpellDesc;

    [Header("�����ư �迭")]
    public SpellButton[] spellBtnArr = new SpellButton[4];

    [Header("������ �г�")]
    public GameObject rightPanel;


    [Header("���� �����̴�")]
    public Slider levelSlider;

    [Header("�Ŵ���")]
    public GameManager gameManager;
    UIManager uiManager;
    AudioManager audioManager;
    ObjectManager objectManager;
    
    void Awake()
    {
        uiManager = gameManager.uiManager;
        audioManager = gameManager.audioManager;
        objectManager = gameManager.objectManager;

        //������ �г� ��Ȱ��ȭ
        rightPanel.SetActive(false);
    }

    #region ���� ����, ���� �ۼ�Ʈ �����ֱ� ����
    [Header("�ε� ������")]
    public GameObject loadIcon;
    public int maxCreatureCount; //Ÿ�� �Ŵ����� ui�� ���ļ� ����
    public void StartGame()
    {
        /*
        //�ε� ������ Ȱ��ȭ
        loadIcon.gameObject.SetActive(true);

        //UI ��Ȱ��ȭ
        gameObject.SetActive(false);

        //���� ���� ����
        gameManager.gameLevel = (int)levelSlider.value;
        
        //battleUI�� ���� ����
        for (int i = 0; i < spellBtnArr.Length; i++)
        {
            if (spellBtnArr[i].spellData != null)//������ �ִ� ��� �̹��� ����
            {
                //��ư�� ���絥���� ����
                uiManager.spellBtnArr[i].spellData = spellBtnArr[i].spellData;
                spellBtnArr[i].IconChange(uiManager.spellBtnArr[i]);

                
                //������Ʈ Ǯ���� ���� �̸� ����
                if (spellBtnArr[i].spellData.spellType == SpellType.Creature)//����ü�� ���
                {
                    for (int j = 0; j < 4; j++)
                    {
                        GameObject obj = objectManager.CreateObj(spellBtnArr[i].spellData.spellPrefab.name, ObjectManager.PoolTypes.CreaturePool);
                        Creature creature = obj.GetComponent<Creature>();
                        //Ȱ�� ���� ����
                        creature.BeforeRevive(Creature.TeamEnum.Blue, gameManager);//���� ���ϸ� �� �� ���ٰ� ������
                        SuperAgent superAgent = obj.GetComponent<SuperAgent>();
                        //superAgent.useBullet
                    }
                }
                else if (spellBtnArr[i].spellData.spellType == SpellType.Weapon)//������ ���
                {
                    int mul = 1;
                    if (spellBtnArr[i].spellData.spellPrefab.name == gameManager.Gun.name)
                        mul = 3;

                    for (int j = 0; j < 4 * mul; j++)
                    {
                        GameObject obj = objectManager.CreateObj(spellBtnArr[i].spellData.spellPrefab.name, ObjectManager.PoolTypes.BulletPool);
                        Bullet bullet = obj.GetComponent<Bullet>();
                        if (bullet.endBullet != null)//�ڽ� �Ѿ˵� ����
                            objectManager.CreateObj(bullet.endBullet.name, ObjectManager.PoolTypes.BulletPool);
                    }
                }
                
            }
            else if (spellBtnArr[i].spellData == null)//���� ��� ��ư ��Ȱ��ȭ
            {
                uiManager.spellBtnArr[i].ButtonOff();
            }
        }
        //���� ȯ�� �ʱ�ȭ
        gameManager.RetryGame();

        */
        // �ε� ������ Ȱ��ȭ
        loadIcon.gameObject.SetActive(true);

        // ���� ���� ����
        gameManager.gameLevel = (int)levelSlider.value;

        // ���� ���� �Լ� ȣ��
        StartCoroutine(StartGameCoroutine());
        //StartActualGame();
    }
    #endregion


    // ���� ���� �ڷ�ƾ
    private IEnumerator StartGameCoroutine()
    {
        // ���⼭ ���� ���� �� �ʿ��� �۾����� �����մϴ�.

        // ���÷� 5�ʰ��� ���� �ε� �ð��� �ݴϴ�.
        float timer = 0f;
        float loadingTime = 15f; // 5�� ���� �ε�
        while (timer < loadingTime)
        {
            timer += Time.deltaTime;
            // ���� �� ������Ʈ
            Debug.Log((int)(timer / loadingTime * 100));
            yield return null;
        }

        // �ε��� ������ ������ ������ �����ϴ� �Լ� ȣ��
        StartActualGame();
    }


    // ���� ���� ���� �Լ�
    private void StartActualGame()
    {
        //battleUI�� ���� ����
        for (int i = 0; i < spellBtnArr.Length; i++)
        {
            if (spellBtnArr[i].spellData != null)//������ �ִ� ��� �̹��� ����
            {
                //��ư�� ���絥���� ����
                uiManager.spellBtnArr[i].spellData = spellBtnArr[i].spellData;
                spellBtnArr[i].IconChange(uiManager.spellBtnArr[i]);


                //������Ʈ Ǯ���� ���� �̸� ����
                if (spellBtnArr[i].spellData.spellType == SpellType.Creature)//����ü�� ���
                {
                    for (int j = 0; j < 4; j++)
                    {
                        GameObject obj = objectManager.CreateObj(spellBtnArr[i].spellData.spellPrefab.name, ObjectManager.PoolTypes.CreaturePool);
                        Creature creature = obj.GetComponent<Creature>();
                        //Ȱ�� ���� ����
                        creature.BeforeRevive(Creature.TeamEnum.Blue, gameManager);//���� ���ϸ� �� �� ���ٰ� ������
                        SuperAgent superAgent = obj.GetComponent<SuperAgent>();
                        //superAgent.useBullet
                    }
                }
                else if (spellBtnArr[i].spellData.spellType == SpellType.Weapon)//������ ���
                {
                    int mul = 1;
                    if (spellBtnArr[i].spellData.spellPrefab.name == gameManager.Gun.name)
                        mul = 3;

                    for (int j = 0; j < 4 * mul; j++)
                    {
                        GameObject obj = objectManager.CreateObj(spellBtnArr[i].spellData.spellPrefab.name, ObjectManager.PoolTypes.BulletPool);
                        Bullet bullet = obj.GetComponent<Bullet>();
                        if (bullet.endBullet != null)//�ڽ� �Ѿ˵� ����
                            objectManager.CreateObj(bullet.endBullet.name, ObjectManager.PoolTypes.BulletPool);
                    }
                }

            }
            else if (spellBtnArr[i].spellData == null)//���� ��� ��ư ��Ȱ��ȭ
            {
                uiManager.spellBtnArr[i].ButtonOff();
            }
        }
        //���� ȯ�� �ʱ�ȭ
        gameManager.RetryGame();

        // UI ��Ȱ��ȭ
        gameObject.SetActive(false);
    }



    /*
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)//�񵿱������� scene �ε�(�� �ɸ��� ���)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // �ε��� ������ �ٷ� Ȱ��ȭ���� ����

        // �ε��� ���� ������ ���
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0.9�� �ε��� ������ ���� ��

            //loadText.text = "�ε� ��: " + Mathf.Floor(progress * 100) + "%";
            if (progress >= 1f)
            {
                asyncLoad.allowSceneActivation = true; // Ȱ��ȭ
            }

            yield return null;
        }
    }
    */
}
