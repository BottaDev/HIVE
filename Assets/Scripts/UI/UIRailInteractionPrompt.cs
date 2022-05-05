using UnityEngine;

public class UIRailInteractionPrompt : MonoBehaviour
{
    [SerializeField] private LayerMask triggerMask;
    [SerializeField] private Rails rail;
    [SerializeField] private string message;
    [SerializeField] private Color messageColor;

    private void Awake()
    {
        EventManager.Instance.Subscribe("OnPlayerRailAttached", DeactivateUIMessage);
    }

    private void DeactivateUIMessage(params object[] p)
    {
        EventManager.Instance.Trigger("OnEliminateUIMessage", message);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerMask.CheckLayer(other.gameObject.layer)) return;

        rail.WaitForInput(true);
        if (!rail.active && !rail.p.hookshot.Pulling)
        {
            EventManager.Instance.Trigger("OnSendUIMessage", message, messageColor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!triggerMask.CheckLayer(other.gameObject.layer)) return;

        rail.WaitForInput(false);
        //prompt.SetActive(false);
        EventManager.Instance.Trigger("OnEliminateUIMessage", message);
    }
}
