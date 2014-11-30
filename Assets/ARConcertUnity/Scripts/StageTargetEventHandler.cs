using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// 스테이지 타겟 생성/삭제 이벤트 처리
public class StageTargetEventHandler : MonoBehaviour, IUserDefinedTargetEventHandler
{
    [SerializeField]
    private ImageTargetBehaviour targetTemplate_Normal_Extended;    // 이미지 타겟 템플릿(원본 설정). 새 타겟을 생성할 때 이 원본을 복제하여 사용한다.
    [SerializeField]
    private ImageTargetBehaviour targetTemplate_Large_Extended;
    private StageTargetType lastTargetType = StageTargetType.Normal;    // 마지막으로 생성 시도한 타겟 타입
    private UserDefinedTargetBuildingBehaviour mTargetBuildingBehaviour;
    private ImageTracker mImageTracker;
    private DataSet mBuiltDataSet;
    //private ImageTargetBuilder.FrameQuality mFrameQuality = ImageTargetBuilder.FrameQuality.FRAME_QUALITY_NONE;
    private Trackable trackable; // 생성된 타겟 정보. 한 번에 하나만 가능
    private GameObject objTarget;   // 생성된 타겟 오브젝트

	public void Awake ()
    {
        mTargetBuildingBehaviour = GetComponent<UserDefinedTargetBuildingBehaviour>();
        if (mTargetBuildingBehaviour)
        {
            mTargetBuildingBehaviour.RegisterEventHandler(this);
            Debug.Log("Registering to the events of IUserDefinedTargetEventHandler");
        }
        trackable = null;
	}

    public void OnInitialized()
    {
        mImageTracker = TrackerManager.Instance.GetTracker<ImageTracker>();
        if (mImageTracker != null)
        {
            // create a new dataset
            mBuiltDataSet = mImageTracker.CreateDataSet();
            mImageTracker.ActivateDataSet(mBuiltDataSet);
        }
    }

    public void OnFrameQualityChanged(ImageTargetBuilder.FrameQuality frameQuality)
    {
        //mFrameQuality = frameQuality;
    }

    public void OnNewTrackableSource(TrackableSource trackableSource)
    {
        if (trackable != null) { return; }   // 이미 있으면 새로 생성하지 않는다.

        // deactivates the dataset first
        mImageTracker.DeactivateDataSet(mBuiltDataSet);

        // 타겟 템플릿을 복제해 실제 추적할 타겟을 만든다.
        ImageTargetBehaviour targetTemplate = GetTargetTemplate(lastTargetType);
        if (targetTemplate == null) { return; }
        GameObject objCopy = Instantiate(targetTemplate.gameObject) as GameObject;
        ImageTargetBehaviour imageTargetCopy = objCopy.GetComponent<ImageTargetBehaviour>();
        imageTargetCopy.gameObject.name = targetTemplate.TrackableName + "_Created";

        // 추적 정보를 생성하고, 생성된 정보를 유지
        DataSetTrackableBehaviour created = mBuiltDataSet.CreateTrackable(trackableSource, imageTargetCopy.gameObject);
        trackable = created.Trackable;
        objTarget = imageTargetCopy.gameObject;

        // activate the dataset again
        mImageTracker.ActivateDataSet(mBuiltDataSet);
    }

    // 새 타겟을 생성한다.
    public void BuildNewTarget(StageTargetType stageTargetType)
    {
        if (trackable != null)
        {
            Debug.Log("Target already created");
            return;
        }

        // 타겟 종류에 따른 템플릿 선택
        ImageTargetBehaviour targetTemplate = GetTargetTemplate(stageTargetType);
        if (targetTemplate == null) { return; }

        // 타겟 생성 시도
        lastTargetType = stageTargetType;
        string trackableName = targetTemplate.TrackableName;
        // Debug.Log("BuildNewTarget " + trackableName + " size:" + targetTemplate.GetSize());
        mTargetBuildingBehaviour.BuildNewTarget(trackableName, targetTemplate.GetSize().x);
    }

    // 생성된 스테이지 타겟정보와 타겟 오브젝트를 지운다.
    public void ClearTarget()
    {
        mImageTracker.DeactivateDataSet(mBuiltDataSet);
        
        mBuiltDataSet.Destroy(trackable, true);
        trackable = null;
        objTarget = null;
        
        mImageTracker.ActivateDataSet(mBuiltDataSet);
    }

    // 타겟이 생성되었는가?
    public bool IsTargetCreated()
    {
        return (trackable != null);
    }

    // 생성된 타겟 오브젝트를 구한다.
    public GameObject GetTargetObject()
    {
        return objTarget;
    }

    // 타입에 따른 템플릿 구하기
    private ImageTargetBehaviour GetTargetTemplate(StageTargetType stageTargetType)
    {
        if (stageTargetType == StageTargetType.Normal)
        {
            return targetTemplate_Normal_Extended;
        }
        else if (stageTargetType == StageTargetType.Large)
        {
            return targetTemplate_Large_Extended;
        }
        else
        {
            return null;
        }
    }
}
