using UnityEngine;
using UnityEngine.UI;

public class ToggleBehavior : MonoBehaviour
{
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
    }

    private void OnDestroy()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            ToggleChangeEvent.ToggleChanged(toggle.name);
            Debug.Log("Toggle changed: " + toggle.name);
        }
    }
}
