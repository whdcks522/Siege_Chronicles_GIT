using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject cameraObj;
    public int i;

    #region 난사 발사
    IEnumerator genocideShot(string bulletName, GameObject host, GameObject target, int index)
    {
        yield return null;

        Vector2 tmpVec = new Vector2(Mathf.Sin(Mathf.PI * i / index * 2.0f), Mathf.Cos(Mathf.PI * i / index * 2.0f));
    }
    #endregion
}
