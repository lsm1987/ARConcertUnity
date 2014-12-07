using UnityEngine;
using System.Collections;

// 로딩 씬
public class LoadingSystem : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadingLoop());
    }

    // 일정 시간 후 메인 씬으로 이동
    private IEnumerator LoadingLoop()
    {
        const float duration = 2.0f;
        yield return new WaitForSeconds(duration);
        Application.LoadLevel(1);
    }
}
