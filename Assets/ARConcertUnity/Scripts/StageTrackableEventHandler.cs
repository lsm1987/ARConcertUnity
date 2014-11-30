using UnityEngine;
using System.Collections;

// 스테이지 타겟 추적 시 이벤트
public class StageTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    public MainSystem mainSystem;   // 이벤트를 넘겨줄 메인 시스템
    private TrackableBehaviour mTrackableBehaviour;

	private void Awake()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
	}

    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        // Debug.Log(previousStatus.ToString() + " -> " + newStatus.ToString() + " ----------------");
        bool prevVisible = Util.IsTrackableStatusVisible(previousStatus);
        bool newVisible = Util.IsTrackableStatusVisible(newStatus);

        if (newVisible)
        {
            // 현재 상태가 보임 가능
            mainSystem.OnTrackableStateChanged(true);
        }
        else
        {
            mainSystem.OnTrackableStateChanged(false);
        }
    }
}
