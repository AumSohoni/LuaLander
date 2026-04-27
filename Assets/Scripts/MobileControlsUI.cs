using UnityEngine;

public class MobileControlsUI : MonoBehaviour
{
    [SerializeField] private bool showInEditorForTesting = true;

    private void Awake()
    {
        bool show = Application.isMobilePlatform || (showInEditorForTesting && Application.isEditor);
        gameObject.SetActive(show);
    }
}
