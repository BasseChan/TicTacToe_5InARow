using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cell : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public Button button;
    public Grid grid;
    public int x;
    public int y;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetField(int opt)
    {
        if (opt == 0)
        {
            buttonText.text = "";
            button.interactable = true;
            return;
        }
        if (opt == 1)
        {
            buttonText.text = "X";
            button.interactable = false;
            return;
        }
        if (opt == 2)
        {
            buttonText.text = "O";
            button.interactable = false;
            return;
        }
    }

    public void Mark()
    {
        if (grid.isYourMove)
            SetField(1);
    }

    [System.Obsolete]
    public void Move()
    {
        if (grid.isYourMove)
            grid.Move(x, y);
    }
}
