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
    private bool isVisible = true; // 보임 여부
    public bool ignoreFastForward = true;
    // 생성시킬 프리팹
    public GameObject musicPlayerPrefab;
    public GameObject[] prefabsNeedsActivation;
    public GameObject[] prefabsOnTimeline;
    public GameObject[] miscPrefabs;
    // 프리팹으로부터 생성된 오브젝트
    private GameObject musicPlayer;
    private GameObject[] objectsNeedsActivation;
    private GameObject[] objectsOnTimeline;

    private void Awake()
    {
        Trans = transform;
        
        // 프리팹 생성
        musicPlayer = (GameObject)Instantiate(musicPlayerPrefab);
        
        objectsNeedsActivation = new GameObject[prefabsNeedsActivation.Length];
        for (var i = 0; i < prefabsNeedsActivation.Length; i++)
        {
            objectsNeedsActivation[i] = (GameObject)Instantiate(prefabsNeedsActivation[i]);
        }

        objectsOnTimeline = new GameObject[prefabsOnTimeline.Length];
        for (var i = 0; i < prefabsOnTimeline.Length; i++)
        {
            objectsOnTimeline[i] = (GameObject)Instantiate(prefabsOnTimeline[i]);
        }

        foreach (var p in miscPrefabs)
        {
            Instantiate(p);
        }
    }

    private void OnDestroy()
    {
    }

    public void StartMusic()
    {
        foreach (var source in musicPlayer.GetComponentsInChildren<AudioSource>())
        {
            source.Play();
        }
    }

    public void ActivateProps()
    {
        foreach (var o in objectsNeedsActivation)
        {
            o.BroadcastMessage("ActivateProps");
        }
    }

    public void SwitchCamera(int index)
    {
        // do nothing
    }

    public void StartAutoCameraChange()
    {
        // do nothing
    }

    public void StopAutoCameraChange()
    {
        // do nothing
    }

    public void FastForward(float second)
    {
        if (!ignoreFastForward)
        {
            FastForwardAnimator(GetComponent<Animator>(), second, 0);
            foreach (var go in objectsOnTimeline)
            {
                foreach (var animator in go.GetComponentsInChildren<Animator>())
                {
                    FastForwardAnimator(animator, second, 0.5f);
                }
            }
        }
    }

    private void FastForwardAnimator(Animator animator, float second, float crossfade)
    {
        for (var layer = 0; layer < animator.layerCount; layer++)
        {
            var info = animator.GetCurrentAnimatorStateInfo(layer);
            if (crossfade > 0.0f)
            {
                animator.CrossFade(info.nameHash, crossfade / info.length, layer, info.normalizedTime + second / info.length);
            }
            else
            {
                animator.Play(info.nameHash, layer, info.normalizedTime + second / info.length);
            }
        }
    }

    public void EndPerformance()
    {
        MainSystem.Instance.ClearStage();
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
