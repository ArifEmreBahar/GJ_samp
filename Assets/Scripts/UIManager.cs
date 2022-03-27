using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameManager gameManager;
    public Text heightText;
    public Text widthText;
    public Text colorText;

    private int height = 5;
    private int width = 5;
    private int colorVariant = 5;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetHeight(float height)
    {
        this.height = (int)height;
        heightText.text = this.height.ToString();
    }

    public void SetWidth(float width)
    {
        this.width = (int)width;
        widthText.text = this.width.ToString();
    }

    public void SetColorVariant(float colorVar)
    {
        colorVariant = (int)colorVar;
        colorText.text = this.colorVariant.ToString();
    }

    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }

    public void IsAnim(bool enable)
    {
        gameManager.SetAnim(enable);
    }
  
    public void StartGame()
    {
        gameManager.StartGame(width, height, colorVariant);
    }
}
