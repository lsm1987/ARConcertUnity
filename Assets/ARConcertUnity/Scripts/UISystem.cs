using UnityEngine;
using System.Collections;

public class UISystem : MonoBehaviour
{
    private static UISystem instance;
    public static UISystem Instance
    {
        get { return instance; }
    }

    [SerializeField]
    private GameObject _uiMain;
    private const string _openTransitionName = "Open"; // Animator에서 사용하는 param 이름
    private const string _closedStateName = "Closed";
    private int _openParameterId;   // "Open" param을 숫자로 찾기 위한 해시코드
    private float _lastTouched; // 마지막으로 터치가 감지된 시간
    private const float _menuHideTimeFromLastTouch = 5.0f; // 마지막 터치 이후 이 시간이 흐르면 메뉴 감춤

	public void Initialize()
    {
        instance = this;
        _openParameterId = Animator.StringToHash(_openTransitionName);
        OpenUI(_uiMain);
        _lastTouched = Time.time;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void Update()
    {
        if (Input.touchCount != 0 || Input.GetMouseButton(0))
        {
            _lastTouched = Time.time; // 터치 감지중
            if (!_uiMain.activeSelf)
            {
                OpenUI(_uiMain);
            }
        }
        else
        {
            if (_uiMain.activeSelf && (Time.time - _lastTouched) > _menuHideTimeFromLastTouch)
            {
                // 숨김 적용하지 않았는데 터치가 한참 전에 있었다면 숨김
                CloseUI(_uiMain);
            }
        }
    }

    // 지정한 UI 활성화
    private void OpenUI(GameObject ui)
    {
        ui.SetActive(true);
        Animator anim = ui.GetComponent<Animator>();
        anim.SetBool(_openParameterId, true);
    }

    // 지정한 UI를 애니메이션으로 화면 밖으로 밀어내고, 애니메이션이 끝나면 비활성화
    private void CloseUI(GameObject ui)
    {
        Animator anim = ui.GetComponent<Animator>();
        anim.SetBool(_openParameterId, false);
        StartCoroutine(DisablePanelDeleyed(anim));
    }

    private IEnumerator DisablePanelDeleyed(Animator anim)
    {
        bool closedStateReached = false; // Closed 상태로 완전히 전이되었는가?
        bool wantToClose = true;    // 코루틴 도중에 anim 값바뀜 체크?
        while (!closedStateReached && wantToClose)
        {
            if (!anim.IsInTransition(0))
            {
                closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(_closedStateName);
            }

            wantToClose = !anim.GetBool(_openParameterId);

            yield return new WaitForEndOfFrame();
        }

        if (wantToClose)
        {
            anim.gameObject.SetActive(false);
        }
    }
}
