/*
 
<����� ����>
    Monsters Pack 04
    Toon Projectiles
    Support package for Hovl Studio assets
    Toon Fantasy Nature
    POLYGON Nature - Low Poly 3D Art by Synty
    3D Scifi Base Vol 1
    GUI Pro - Casual Game
    Stylized Slash VFX
    Item Pickup VFX - UR
 
�̻�� �ڵ�

public void EnemyFirstRangeCalc()//��� ���� ��� ����, Ÿ���� ����
    {
        bool isLive = false;
        curRange = 9999;

        foreach (Transform obj in enemyCreatureFolder) 
        {

            if (obj.gameObject.layer == LayerMask.NameToLayer("Creature")) 
            {
                isLive = true;

                //������ �Ÿ�
                float tmpRange = (obj.position - transform.position).magnitude;
                if (tmpRange < curRange)
                {
                    curRange = tmpRange;
                    curTarget = obj;
                }
            } 
        }

        if (!isLive)//���� ���� ���ٸ�
        {
            curTarget = enemyTower;
            curRange = (curTarget.position - transform.position).magnitude - 2;//Ÿ���� �β� ���
        }
    }
 */
