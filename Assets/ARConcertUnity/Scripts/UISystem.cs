using UnityEngine;
using System.Collections;

public class UISystem : MonoBehaviour
{
    private static UISystem instance;
    public static UISystem Instance
    {
        get { return instance; }
    }

	public void Initialize()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
