using UnityEngine;

public class UIRailInteractionPrompt : MonoBehaviour
{
    public GameObject prompt;
    [SerializeField] private Rails rail;

    private void OnTriggerEnter(Collider other)
    {
        rail.WaitForInput(true);
        if (!rail.active && rail.p.grapple.Pulling)
        {
            prompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        rail.WaitForInput(false);
        prompt.SetActive(false);
    }
}
