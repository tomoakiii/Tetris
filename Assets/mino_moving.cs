using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mino_moving : MonoBehaviour
{
    public float previousFallTime, previousFreshTime;
    public float fallTime = 1f;
    public float freshTime = 0.2f;
    private static int stageWidth = 10, stageHeight = 20;
    public Vector3 position_change, rotationPoint;
    // Start is called before the first frame update
    
    private static Transform[,] grid = new Transform[stageWidth, stageHeight + 1];
 
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MinoMovement();
    }

    private void MinoMovement()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            position_change += new Vector3(-1, 0, 0);
            previousFreshTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            position_change += new Vector3(1, 0, 0);
            previousFreshTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            position_change += new Vector3(0, -1, 0);
            previousFreshTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TryRotate();
            previousFreshTime = Time.time;
        }
        else if (Time.time - previousFreshTime >= freshTime)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                position_change += new Vector3(-1, 0, 0);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                position_change += new Vector3(1, 0, 0);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                position_change += new Vector3(0, -1, 0);
            }
            previousFreshTime = Time.time;
        }
        else if (Time.time - previousFallTime >= fallTime)
        {
            position_change += new Vector3(0, -1, 0);
            previousFallTime = Time.time;
        }

        transform.position += position_change;
        if (!ValidMovement())
        {
            transform.position -= position_change;
            if (position_change.y <= -1)
            {
                AddToGrid();
                CheckLines();
                this.enabled = false;
                if (DeadCheck())
                {
                    bool b = UnityEditor.EditorUtility.DisplayDialog("Game Over", "Restart?", "Restart", "Exit");
                    if(b)
                    { 
                        NewGame();
                    }
                    else
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;//End Game
#else
                        Application.Quit();// End Game
#endif
                    }
                }
                else
                {
                    FindObjectOfType<SpawnMino>().NewMino();
                }
            }
        }
        position_change = new Vector3(0, 0, 0);
    }

    bool DeadCheck()
    {
        for (int x = 0; x < stageWidth; x++)
        {
            if (grid[x, stageHeight] != null)
            {
                return true;
            } 
        }
        return false;
    }

    void CheckLines()
    {
        for (int y = 0; y < stageHeight; y++)
        {
            if (isLineValid(y))
            {
                DeleteLine(y);
            }
        }
    }

    bool isLineValid(int y)
    {
        for (int x = 0; x < stageWidth; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }

    void TryRotate()
    {
        DEBUGLOG("before");
        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
        DEBUGLOG("after");
        if (!ValidMovement())
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
            DEBUGLOG("return");
        }
    }

    void DeleteLine(int y)
    {
        for (int x = 0; x < stageWidth; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
            for (int yy = y + 1; yy < stageHeight; yy++)
            {
                if (grid[x, yy] != null)
                {
                    grid[x, yy - 1] = grid[x, yy];
                    grid[x, yy] = null;
                    grid[x, yy - 1].transform.position += new Vector3(0, -1, 0);
                } 
            }
        }
    }

    bool ValidMovement()
    {
        foreach (Transform children in transform)
        {
            int roundX = Mathf.RoundToInt(children.transform.position.x);
            int roundY = Mathf.RoundToInt(children.transform.position.y);

            if (roundX < 0 || roundX >= stageWidth || roundY < 0 || roundY >= stageHeight)
            {
                return false;
            }
            else if (grid[roundX, roundY] != null)
            {
                return false;
            }
        }
        return true;
    }

     // 今回の追加
    void AddToGrid() 
    {
        foreach (Transform children in transform) 
        {
            int roundX = Mathf.RoundToInt(children.transform.position.x);
            int roundY = Mathf.RoundToInt(children.transform.position.y);
 
            grid[roundX, roundY] = children;
        }
    }

    void NewGame()
    {
        for (int x = 0; x < stageWidth; x++)
        {
            for (int y = 0; y < stageHeight + 1; y++)
            {
                grid[x, y] = null;
            }
        }
    }

    void DEBUGLOG(string Message) 
    {
        Debug.Log(Message);
        int n = 0;
        foreach (Transform children in transform) 
        {
            Debug.Log($"{n},x = {children.transform.position.x}, y = {children.transform.position.y}");
            n += 1;
        }
    }

}
