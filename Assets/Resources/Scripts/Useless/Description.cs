/*
 
<사용한 에셋>
    Monsters Pack 04
    Toon Projectiles
    Support package for Hovl Studio assets
    Toon Fantasy Nature
    POLYGON Nature - Low Poly 3D Art by Synty
    3D Scifi Base Vol 1
    GUI Pro - Casual Game
    Stylized Slash VFX
    Item Pickup VFX - UR
 
미사용 코드

public void EnemyFirstRangeCalc()//모든 적을 잡고 나서, 타워로 향함
    {
        bool isLive = false;
        curRange = 9999;

        foreach (Transform obj in enemyCreatureFolder) 
        {

            if (obj.gameObject.layer == LayerMask.NameToLayer("Creature")) 
            {
                isLive = true;

                //적과의 거리
                float tmpRange = (obj.position - transform.position).magnitude;
                if (tmpRange < curRange)
                {
                    curRange = tmpRange;
                    curTarget = obj;
                }
            } 
        }

        if (!isLive)//남은 적이 없다면
        {
            curTarget = enemyTower;
            curRange = (curTarget.position - transform.position).magnitude - 2;//타워의 두께 계산
        }
    }
 */
