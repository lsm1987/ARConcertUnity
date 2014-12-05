using UnityEngine;
using System.Collections;

public class UIMainMenu : MonoBehaviour
{
    // 일반 스테이지 생성
    public void OnNormalClicked()
    {
        if (!MainSystem.Instance.HasStage)
        {
            MainSystem.Instance.LoadStage(StageTargetType.Normal);
        }
    }

    // 큰 스테이지 생성
    public void OnLargeClicked()
    {
        if (!MainSystem.Instance.HasStage)
        {
            MainSystem.Instance.LoadStage(StageTargetType.Large);
        }
    }

    // 스테이지 비우기
	public void OnClearClicked()
    {
        if (MainSystem.Instance.HasStage)
        {
            MainSystem.Instance.ClearStage();
        }
    }

    // 종료
    public void OnQuitClicked()
    {
        MainSystem.Instance.QuitApplication();
    }
}
