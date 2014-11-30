using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Stage : MonoBehaviour
{
    public Transform Trans
    {
        get;
        private set;
    }
    [SerializeField]
    private GameObject prefabPawn;
    public GameObject Pawn { get; private set; }
    private bool isVisible = true; // 보임 여부

    private void Awake()
    {
        Trans = transform;
        
        // 폰을 스테이지의 자식 오브젝트로 생성. 방향 주의
        GameObject objPawn = Instantiate(prefabPawn, Vector3.zero, Quaternion.Euler(0.0f, 180.0f, 0.0f)) as GameObject;
        objPawn.transform.parent = Trans;
        Pawn = objPawn;
    }

    private void OnDestroy()
    {
    }

    // 자식 오브젝트들(폰, 바닥, 부속품 등)의 보임 여부 지정
    public void SetVisible(bool visible)
    {
        if (isVisible == visible) { return; }
        isVisible = visible;

        Renderer[] rendererComponents = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = visible;
        }
    }

    // 지정한 경로의 프리팹을 스테이지의 자식으로 생성한다.
    // 비활성화 상태로 생성하며, 스테이지가 보임 상태가 아니라면 렌더러를 비활성화한다.
    public GameObject InstantiatePrefab(string source)
    {
        UnityEngine.Object prefab = Resources.Load(source);
        if (prefab == null)
        {
            Debug.LogError("[Stage] InstantiatePrefab() Invalid prefab source");
            return null;
        }

        GameObject obj = GameObject.Instantiate(prefab) as GameObject;
        obj.transform.parent = Trans; // 스테이지의 하위 오브젝트로
        
        if (!isVisible)
        {
            Renderer[] rendererComponents = obj.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = isVisible;
            }
        }

        obj.SetActive(false);
        return obj;
    }
}
