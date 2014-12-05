using UnityEngine;
using System.Collections;

public class MainSystem : MonoBehaviour
{
    private static MainSystem instance;
    public static MainSystem Instance
    {
        get { return instance; }
    }

    // 전체 흐름 상태
    public enum State
    {
        NoTarget,   // 스테이지로 사용할 타겟이 아직 없음
        Stage,      // 스테이지 생성되어있음
    }

    private State state = State.NoTarget;
    [SerializeField]
    private GameObject prefabStage; // 일반 스테이지
    private Stage stage;
    public Stage Stage
    {
        get { return stage; }
    }
    public bool HasStage
    {
        get { return stage != null; }
    }
    private StageTargetType stageTargetType = StageTargetType.Normal; // 스테이지 생성시 지정한 타입
    [SerializeField]
    private StageTargetEventHandler stageTargetEventHandler;

    private void Awake()
    {
        instance = this;
        UISystem uiSystem = FindObjectOfType<UISystem>();
        uiSystem.Initialize();

        SetState(State.NoTarget);
    }

    /*
    // 테스트용 UI
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 350), GUI.skin.box);
        GUILayout.BeginVertical();

        if (GUILayout.Button("Normal", GUILayout.Height(50)))
        {
            LoadStage(StageTargetType.Normal);
        }
        if (GUILayout.Button("Large", GUILayout.Height(50)))
        {
            LoadStage(StageTargetType.Large);
        }
        if (GUILayout.Button("Clear", GUILayout.Height(50)))
        {
            ClearStage();
        }
        if (GUILayout.Button("Quit", GUILayout.Height(50)))
        {
            Application.Quit();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    */

    private void SetState(State paramState)
    {
        // 기존 상태
        switch (state)
        {
            case State.NoTarget:
                {
                    break;
                }
        }

        state = paramState;
        
        // 새 상태
        switch(state)
        {
            case State.NoTarget:
                {
                    break;
                }
        }
    }

    // 스테이지 생성 준비작업
    public void LoadStage(StageTargetType stageTargetType_)
    {
        if (stage != null) { return; }
        SetStageTargetType(stageTargetType_);

        // 원점에 스테이지 객체 생성. 어차피 타겟 오브젝트를 원점으로 사용할 것이므로 타겟 하위에 생성하지는 않는다.
        // AR 타겟의 추적 상태가 먼저 바뀌고 스테이지가 생성되는 것 막기 위해 스테이지를 명시적으로 먼저 생성
        GameObject obj = Instantiate(
            prefabStage
            , Vector3.zero
            , Quaternion.identity) as GameObject;
        stage = obj.GetComponent<Stage>();

        stage.SetVisible(false); // 안보이는 상태로 생성 후, 마커 추적이 가능해지면 보임

        // AR 타겟 생성
        if (stageTargetEventHandler != null)
        {
            stageTargetEventHandler.BuildNewTarget(stageTargetType);
        }

        SetState(State.Stage);
    }

    // 무대 생성되면 함께 생성되는 오브젝트가 너무 많아, 무대 비우기는 씬 리로드로 처리
    public void ClearStage()
    {
        Application.LoadLevel(0);
    }

    // 스테이지 상태 변경, 씬 오브젝트 정리
    private void SetStageTargetType(StageTargetType stageTargetType_)
    {
        if (stageTargetType == stageTargetType_) { return; }        
        stageTargetType = stageTargetType_;
    }

    // 타겟 추적상태 변경 시
    public void OnTrackableStateChanged(bool visible)
    {
        if (stage != null)
        {
            stage.SetVisible(visible);
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
