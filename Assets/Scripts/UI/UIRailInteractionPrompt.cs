using UnityEngine;

public class UIRailInteractionPrompt : MonoBehaviour
{
    [SerializeField] private LayerMask triggerMask;
    [SerializeField] private Rails rail;
    [SerializeField] private string message;
    [SerializeField] private Color messageColor;

    private void Awake()
    {
        EventManager.Instance.Subscribe(EventManager.Events.OnPlayerRailAttached, DeactivateUIMessage);
    }

    private void DeactivateUIMessage(params object[] p)
    {
        EventManager.Instance.Trigger(EventManager.Events.OnEliminateUIMessage, message);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerMask.CheckLayer(other.gameObject.layer)) return;

        rail.WaitForInput(true);
        if (!rail.active && !rail.p.grapple.Pulling)
        {
            EventManager.Instance.Trigger(EventManager.Events.OnSendUIMessage, message, messageColor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!triggerMask.CheckLayer(other.gameObject.layer)) return;

        rail.WaitForInput(false);
        //prompt.SetActive(false);
        EventManager.Instance.Trigger(EventManager.Events.OnEliminateUIMessage, message);
    }
}
