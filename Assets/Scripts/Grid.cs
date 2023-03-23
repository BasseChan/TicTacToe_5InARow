using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Grid : MonoBehaviour
{
    public GameObject cellPrefab;
    /*public GridLayoutGroup gridLayout;*/
    private Cell[] allCells;
    public Slider slider;
    public GameObject container;
    private int width;
    private int height;
    private int[,] gridArrayValue;
    public bool isYourMove = false;
    public TextMeshProUGUI info;

    void Start()
    {
        //Build();
    }

    public void Build()
    {
        if (allCells == null)
        {
            allCells = new Cell[400];
            container.GetComponent<GridLayoutGroup>().cellSize = new Vector2(25, 25);
            for (int i = 0; i < 400; i++)
            {
                GameObject newCell = Instantiate(cellPrefab, transform);
                allCells[i] = newCell.GetComponent<Cell>();
                allCells[i].grid = this;
            }
        }
    }

    public void NewBoard()
    {
        info.text = "";
        isYourMove = true;
        width = (int)slider.value;
        height = width;
        gridArrayValue = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                gridArrayValue[i, j] = 0;
            }
        }
        for (int i = 0; i < width * height; i++)
        {
            allCells[i].SetField(0);
            allCells[i].x = i % width;
            allCells[i].y = i / width;
        }
        for (int i = width * height; i < 400; i++)
        {
            allCells[i].SetField(1);
        }
        float widthConteiner = container.GetComponent<RectTransform>().rect.width;
        Vector2 newSize = new Vector2(widthConteiner / width, widthConteiner / height);
        container.GetComponent<GridLayoutGroup>().cellSize = newSize;
    }

    [System.Obsolete]
    public void Move(int x, int y)
    {
        //allCells[x + y * width + 1].SetField(2);
        isYourMove = false;
        gridArrayValue[x, y] = 1;
        if (CheckIfWin(x, y))
        {
            info.text = "Wygrana!";
            info.color = Color.yellow;
        }
        else
        {
            if (CheckIfDraw())
            {
                info.text = "  Remis";
                info.color = Color.gray;
            }
            else
                MoveAI();
        }
    }

    private bool CheckIfWin(int x, int y)
    {
        int value = gridArrayValue[x, y];
        int counter = 0;
        int maxI = Mathf.Min(x, width - 5);
        // check horizontal
        int border = Mathf.Min(x, width - 5);
        for (int i = Mathf.Max(0, x - 4); i <= border;)
        {
            for (counter = 0; counter < 5; counter++)
            {
                if (gridArrayValue[i, y] != value)
                    counter = 6;
                i++;
            }
            if (counter == 5)
                return true;
        }
        // check vertical
        border = Mathf.Min(y, height - 5);
        for (int i = Mathf.Max(0, y - 4); i <= border;)
        {
            for (counter = 0; counter < 5; counter++)
            {
                if (gridArrayValue[x, i] != value)
                    counter = 6;
                i++;
            }
            if (counter == 5)
                return true;
        }
        // up diagonal
        border = Mathf.Max(0, x - width + 5, y - height + 5);
        for (int i = Mathf.Min(4, x, y); i >= border;)
        {
            for (counter = 0; counter < 5; counter++)
            {
                if (gridArrayValue[x - i, y - i] != value)
                    counter = 6;
                i--;
            }
            if (counter == 5)
                return true;
        }
        // down diagonal
        border = Mathf.Max(0, x - width + 5, 4 - y);
        for (int i = Mathf.Min(4, x, height - y - 1); i >= border;)
        {
            for (counter = 0; counter < 5; counter++)
            {
                if (gridArrayValue[x - i, y + i] != value)
                    counter = 6;
                i--;
            }
            if (counter == 5)
                return true;
        }
        return false;
    }

    private bool CheckIfDraw()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if (gridArrayValue[i, j] == 0)
                    return false;
        return true;
    }

    [System.Obsolete]
    private void MoveAI()
    {
        ArrayList queue = GetToCheckList(2);
        ArrayList goodMoves = new();
        ArrayList averageMoves = new();
        for (int layer = 1; layer <= 4; layer++)
        {
            for (int player = 2; player > 0; player--)
            {
                for (int j = 0; j < queue.Count; j++)
                {
                    Vector2Int current = (Vector2Int)queue[j];
                    int grade = CheckIfWillWin(current.x, current.y, player, layer, new ArrayList());
                    if (grade == 2)
                        goodMoves.Add(current);
                    if (grade == 1)
                        averageMoves.Add(current);
                }
                if (goodMoves.Count > 0)
                {
                    MarkRandomAI(goodMoves);
                    return;
                }
            }
        }
        if (averageMoves.Count > 0)
        {
            MarkRandomAI(averageMoves);
            return;
        }
        queue = GetToCheckList(1);
        //Debug.Log(queue.Count);
        MarkRandomAI(queue);
    }

    [System.Obsolete]
    private void MarkRandomAI(ArrayList moves)
    {
        int index = Random.RandomRange(0, moves.Count);
        Vector2Int coords = (Vector2Int)moves[index];
        gridArrayValue[coords.x, coords.y] = 2;
        allCells[coords.x + coords.y * width].SetField(2);
        if (CheckIfWin(coords.x, coords.y))
        {
            info.text = " Pora¿ka";
            info.color = Color.red;
        }
        else
        {
            if (CheckIfDraw())
            {
                info.text = "  Remis";
                info.color = Color.gray;
            }
            else
                isYourMove = true;
        }
    }

    private ArrayList GetToCheckList(int maxRange)
    {
        ArrayList queue = new ArrayList();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (IsSensible(i, j, maxRange))
                {
                    queue.Add(new Vector2Int(i, j));
                }
            }
        }
        return queue;
    }

    private bool IsSensible(int x, int y, int maxRange)
    {
        if (gridArrayValue[x, y] == 0)
        {
            for (int k = 1; k <= maxRange; k++)
            {
                if (x >= k)
                {
                    if (gridArrayValue[x - k, y] != 0)
                        return true;
                    if (y >= k && gridArrayValue[x - k, y - k] != 0)
                        return true;
                    if (y < height - k && gridArrayValue[x - k, y + k] != 0)
                        return true;
                }
                if (x < width - k)
                {
                    if (gridArrayValue[x + k, y] != 0)
                        return true;
                    if (y >= k && gridArrayValue[x + k, y - k] != 0)
                        return true;
                    if (y < height - k && gridArrayValue[x + k, y + k] != 0)
                        return true;
                }
                if (y >= k && gridArrayValue[x, y - k] != 0)
                    return true;
                if (y < height - k && gridArrayValue[x, y + k] != 0)
                    return true;
            }
        }
        return false;
    }

    private bool IsSensible(int x, int y, int maxRange, int player)
    {
        if (gridArrayValue[x, y] == 0)
        {
            for (int k = 1; k <= maxRange; k++)
            {
                if (x >= k)
                {
                    if (gridArrayValue[x - k, y] != 0)
                        return true;
                    if (y >= k && gridArrayValue[x - k, y - k] == player)
                        return true;
                    if (y < height - k && gridArrayValue[x - k, y + k] == player)
                        return true;
                }
                if (x < width - k)
                {
                    if (gridArrayValue[x + k, y] == player)
                        return true;
                    if (y >= k && gridArrayValue[x + k, y - k] == player)
                        return true;
                    if (y < height - k && gridArrayValue[x + k, y + k] == player)
                        return true;
                }
                if (y >= k && gridArrayValue[x, y - k] == player)
                    return true;
                if (y < height - k && gridArrayValue[x, y + k] == player)
                    return true;
            }
        }
        return false;
    }

    private int CheckIfWillWin(int x, int y, int player)
    {
        //Debug.Log(1 + " " + x + " " + y);
        gridArrayValue[x, y] = player;
        bool isWinning = CheckIfWin(x, y);
        gridArrayValue[x, y] = 0;
        if (isWinning)
            return 2;
        return 0;
    }

    private int CheckIfWillWin(int x, int y, int player, int layer, ArrayList previous)
    {
        if (layer == 1)
            return CheckIfWillWin(x, y, player);
        if (!IsSensible(x, y, 2, player))
            return 0;
        gridArrayValue[x, y] = player;
        int otherPlayer = 1;
        if (player == 1)
            otherPlayer = 2;
        int counter = 0;

        ArrayList valid = new ArrayList();
        previous.Add(new Vector2Int(x, y));

        int border = Mathf.Max(x - 5, 0);
        int zeroCounter = 0;
        for (int i = x - 1; i >= border; i--)
        {
            int value = gridArrayValue[i, y];
            if (value == 0)
            {
                zeroCounter++;
                if (IsValidToCheckWithPrevious(i, y, previous, otherPlayer))
                {
                    for (int iLayer = layer - 1; iLayer >= 1; iLayer--)
                    {
                        if (CheckIfWillWin(i, y, player, iLayer, previous) == 2)
                        {
                            counter++;
                            i = border - 1;
                            iLayer = 0;
                            valid.Add(new Vector2Int(i, y));
                            if (counter > 2)
                            {
                                gridArrayValue[x, y] = 0;
                                previous.RemoveAt(previous.Count - 1);
                                return 2;
                            }
                        }
                    }
                }
                if (zeroCounter == 2)
                    i = border - 1;
            }
            else
                if (value != player)
                i = border - 1;
        }

        border = Mathf.Max(y - 5, 0);
        zeroCounter = 0;
        for (int i = y - 1; i >= border; i--)
        {
            int value = gridArrayValue[x, i];
            if (value == 0)
            {
                zeroCounter++;
                if (IsValidToCheckWithPrevious(x, i, previous, otherPlayer))
                {
                    for (int iLayer = layer - 1; iLayer >= 1; iLayer--)
                    {
                        if (CheckIfWillWin(x, i, player, iLayer, previous) == 2)
                        {
                            counter++;
                            i = border - 1;
                            iLayer = 0;
                            valid.Add(new Vector2Int(x, i));
                            if (counter > 2)
                            {
                                gridArrayValue[x, y] = 0;
                                previous.RemoveAt(previous.Count - 1);
                                return 2;
                            }
                        }
                    }
                }
                if (zeroCounter == 2)
                    i = border - 1;
            }
            else
                if (value != player)
                i = border - 1;
        }

        border = Mathf.Min(x + 4, width - 1);
        zeroCounter = 0;
        for (int i = x + 1; i <= border; i++)
        {
            int value = gridArrayValue[i, y];
            if (value == 0)
            {
                zeroCounter++;
                if (IsValidToCheckWithPrevious(i, y, previous, otherPlayer))
                {
                    for (int iLayer = layer - 1; iLayer >= 1; iLayer--)
                    {
                        if (CheckIfWillWin(i, y, player, iLayer, previous) == 2)
                        {
                            counter++;
                            i = border + 1;
                            iLayer = 0;
                            valid.Add(new Vector2Int(i, y));
                            if (counter > 2)
                            {
                                gridArrayValue[x, y] = 0;
                                previous.RemoveAt(previous.Count - 1);
                                return 2;
                            }
                        }
                    }
                }
                if (zeroCounter == 2)
                    i = border + 1;
            }
            else
                if (value != player)
                i = border + 1;
        }

        border = Mathf.Min(y + 4, width - 1);
        zeroCounter = 0;
        for (int i = y + 1; i <= border; i++)
        {
            int value = gridArrayValue[x, i];
            if (value == 0)
            {
                zeroCounter++;
                if (IsValidToCheckWithPrevious(x, i, previous, otherPlayer))
                {
                    for (int iLayer = layer - 1; iLayer >= 1; iLayer--)
                    {
                        if (CheckIfWillWin(x, i, player, iLayer, previous) == 2)
                        {
                            counter++;
                            i = border + 1;
                            iLayer = 0;
                            valid.Add(new Vector2Int(x, i));
                            if (counter > 2)
                            {
                                gridArrayValue[x, y] = 0;
                                previous.RemoveAt(previous.Count - 1);
                                return 2;
                            }
                        }
                    }
                }
                if (zeroCounter == 2)
                    i = border + 1;
            }
            else
                if (value != player)
                i = border + 1;
        }

        border = Mathf.Min(x, y, 4);
        zeroCounter = 0;
        for (int i = 1; i <= border; i++)
        {
            int value = gridArrayValue[x - i, y - i];
            if (value == 0)
            {
                zeroCounter++;
                if (IsValidToCheckWithPrevious(x - i, y - i, previous, otherPlayer))
                {
                    for (int iLayer = layer - 1; iLayer >= 1; iLayer--)
                    {
                        if (CheckIfWillWin(x - i, y - i, player, iLayer, previous) == 2)
                        {
                            counter++;
                            i = border + 1;
                            iLayer = 0;
                            valid.Add(new Vector2Int(x - i, y - i));
                            if (counter > 2)
                            {
                                gridArrayValue[x, y] = 0;
                                previous.RemoveAt(previous.Count - 1);
                                return 2;
                            }
                        }
                    }
                }
                if (zeroCounter == 2)
                    i = border + 1;
            }
            else
                if (value != player)
                i = border + 1;
        }

        border = Mathf.Min(width - x - 1, height - y - 1, 4);
        zeroCounter = 0;
        for (int i = 1; i <= border; i++)
        {
            int value = gridArrayValue[x + i, y + i];
            if (value == 0)
            {
                zeroCounter++;
                if (IsValidToCheckWithPrevious(x + i, y + i, previous, otherPlayer))
                {
                    for (int iLayer = layer - 1; iLayer >= 1; iLayer--)
                    {
                        if (CheckIfWillWin(x + i, y + i, player, iLayer, previous) == 2)
                        {
                            counter++;
                            i = border + 1;
                            iLayer = 0;
                            valid.Add(new Vector2Int(x + i, y + i));
                            if (counter > 2)
                            {
                                gridArrayValue[x, y] = 0;
                                previous.RemoveAt(previous.Count - 1);
                                return 2;
                            }
                        }
                    }
                }
                if (zeroCounter == 2)
                    i = border + 1;
            }
            else
                if (value != player)
                i = border + 1;
        }

        border = Mathf.Min(x, height - y - 1, 4);
        zeroCounter = 0;
        for (int i = 1; i <= border; i++)
        {
            int value = gridArrayValue[x - i, y + i];
            if (value == 0)
            {
                zeroCounter++;
                if (IsValidToCheckWithPrevious(x - i, y + i, previous, otherPlayer))
                {
                    for (int iLayer = layer - 1; iLayer >= 1; iLayer--)
                    {
                        if (CheckIfWillWin(x - i, y + i, player, iLayer, previous) == 2)
                        {
                            counter++;
                            i = border + 1;
                            iLayer = 0;
                            valid.Add(new Vector2Int(x - i, y + i));
                            if (counter > 2)
                            {
                                gridArrayValue[x, y] = 0;
                                previous.RemoveAt(previous.Count - 1);
                                return 2;
                            }
                        }
                    }
                }
                if (zeroCounter == 2)
                    i = border + 1;
            }
            else
                if (value != player)
                i = border + 1;
        }

        border = Mathf.Min(width - x - 1, y, 4);
        zeroCounter = 0;
        for (int i = 1; i <= border; i++)
        {
            int value = gridArrayValue[x + i, y - i];
            if (value == 0)
            {
                zeroCounter++;
                if (IsValidToCheckWithPrevious(x + i, y - i, previous, otherPlayer))
                {
                    for (int iLayer = layer - 1; iLayer >= 1; iLayer--)
                    {
                        if (CheckIfWillWin(x + i, y - i, player, iLayer, previous) == 2)
                        {
                            counter++;
                            i = border + 1;
                            iLayer = 0;
                            valid.Add(new Vector2Int(x + i, y - i));
                            if (counter > 2)
                            {
                                gridArrayValue[x, y] = 0;
                                previous.RemoveAt(previous.Count - 1);
                                return 2;
                            }
                        }
                    }
                }
                if (zeroCounter == 2)
                    i = border + 1;
            }
            else
                if (value != player)
                i = border + 1;
        }



        /*if (counter == 2)
        {
            Vector2Int first = (Vector2Int)valid[0];
            Vector2Int second = (Vector2Int)valid[1];
            Debug.Log("a " + second.x + " " + second.y);
            Debug.Log("b " + first.x + " " + first.y);
            if (!checkIfOnOneLine(first.x, first.y, second.x, second.y, x, y))
            {
                gridArrayValue[x, y] = 0;
                previous.RemoveAt(previous.Count - 1);
                return 2;
            }
            gridArrayValue[first.x, first.y] = otherPlayer;
            for (int iLayer = layer - 1; iLayer <= 1; iLayer--)
            {
                if (checkIfWillWin(second.x, second.y, player, iLayer, averageMoves, previous) == 2)
                {
                    gridArrayValue[first.x, first.y] = 0;
                    gridArrayValue[x, y] = 0;
                    previous.RemoveAt(previous.Count - 1);
                    return 2;
                }
            }
            gridArrayValue[first.x, first.y] = 0;
            gridArrayValue[second.x, second.y] = otherPlayer;
            for (int iLayer = layer - 1; iLayer <= 1; iLayer--)
            {
                if (checkIfWillWin(first.x, first.y, player, iLayer, averageMoves, previous) == 2)
                {
                    gridArrayValue[second.x, second.y] = 0;
                    gridArrayValue[x, y] = 0;
                    previous.RemoveAt(previous.Count - 1);
                    return 2;
                }
            }
        }*/
        gridArrayValue[x, y] = 0;
        return counter;
    }

    private bool IsValidToCheck(int x, int y, int xPrev, int yPrev, int otherPlayer)
    {
        if (x == xPrev)
        {
            if (y > yPrev)
            {
                if (y < yPrev + 5)
                {
                    for (int i = yPrev + 1; i < y; i++)
                        if (gridArrayValue[x, i] == otherPlayer)
                            return false;
                    return true;
                }
            }
            else
            {
                if (yPrev < y + 5)
                {
                    for (int i = y + 1; i < yPrev; i++)
                        if (gridArrayValue[x, i] == otherPlayer)
                            return false;
                    return true;
                }
            }
            return false;
        }
        if (y == yPrev)
        {
            if (x > xPrev)
            {
                if (x < xPrev + 5)
                {
                    for (int i = xPrev + 1; i < x; i++)
                        if (gridArrayValue[i, y] == otherPlayer)
                            return false;
                    return true;
                }
            }
            else
            {
                if (xPrev < x + 5)
                {
                    for (int i = x + 1; i < xPrev; i++)
                        if (gridArrayValue[i, y] == otherPlayer)
                            return false;
                    return true;
                }
            }
            return false;
        }
        if (y - yPrev == x - xPrev)
        {
            if (x > xPrev)
            {
                if (x < xPrev + 5)
                {
                    for (int i = 1; i < x - xPrev; i++)
                        if (gridArrayValue[xPrev + i, yPrev + i] == otherPlayer)
                            return false;
                    return true;
                }
            }
            else
            {
                if (xPrev < x + 5)
                {
                    for (int i = 1; i < xPrev - x; i++)
                        if (gridArrayValue[x + i, y + i] == otherPlayer)
                            return false;
                    return true;
                }
            }
            return false;
        }
        if (y - yPrev == xPrev - x)
        {
            if (x > xPrev)
            {
                if (x < xPrev + 5)
                {
                    for (int i = 1; i < x - xPrev; i++)
                        if (gridArrayValue[xPrev + i, y + i] == otherPlayer)
                            return false;
                    return true;
                }
            }
            else
            {
                if (xPrev < x + 5)
                {
                    for (int i = 1; i < xPrev - x; i++)
                        if (gridArrayValue[x + i, yPrev + i] == otherPlayer)
                            return false;
                    return true;
                }
            }
            return false;
        }
        return false;
    }

    private bool IsValidToCheckWithPrevious(int x, int y, ArrayList previous, int otherPlayer)
    {
        if (previous.Count <= 1)
            return true;
        for (int i = 0; i < previous.Count - 1; i++)
        {
            Vector2Int prev = (Vector2Int)previous[i];
            if (IsValidToCheck(x, y, prev.x, prev.y, otherPlayer))
                return true;
        }
        return false;
    }

    /*public void Build()
    {
        boardSize = (int)slider.value;
        cells = new Cell[boardSize, boardSize];
        float width = container.GetComponent<RectTransform>().rect.width;
        Vector2 newSize = new Vector2(width / boardSize, width / boardSize);
        container.GetComponent<GridLayoutGroup>().cellSize = newSize;
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                GameObject newCell = Instantiate(cellPrefab, transform);
                cells[i, j] = newCell.GetComponent<Cell>();
            }
        }
    }

    public void Refresh()
    {
        
    }

    public void DestroyBoard()
    {
        if (cells != null)
        {
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    Destroy(cells[i, j]);
                }
            }
        }
    }*/
}
