using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridFormatter : MonoBehaviour
{
    public GameObject container;
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Format()
    {
        int boardSize = (int)slider.value;
        float width = container.GetComponent<RectTransform>().rect.width;
        Vector2 newSize = new Vector2(width / boardSize, width / boardSize);
        container.GetComponent<GridLayoutGroup>().cellSize = newSize;
    }
}
