using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    private string click = "Click";
    private bool clickable = true;

    public delegate void Click(Vector3 clickPosition);
    public static event Click OnClicked;

    private void Update()
    {
        if (Input.GetButtonDown(click))
        {
            if (!EventSystem.current.IsPointerOverGameObject() && clickable)
            {
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                OnClicked(worldPosition);
                clickable = false;
                Invoke("WaitForNextInput", 0.45f);
            }
        }
    }

    private void WaitForNextInput()
    {
        clickable = true;
    }
}
