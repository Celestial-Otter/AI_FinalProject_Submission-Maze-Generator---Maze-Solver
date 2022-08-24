//Hunter Chu and Edward Cao
//100701653 and 100697845
//March 28th, 2022

using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    //variables
    public Maze mazePrefab;
    private Maze mazeInstance;
    private bool gameStarted = false;
    private IntVector2 storedSize = new IntVector2(5, 5);

    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ai;
    [SerializeField] private Material endMat;

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private GameObject popupPanel;

    [SerializeField] private TMP_InputField sizeX;
    [SerializeField] private TMP_InputField sizeY;
    [SerializeField] private TMP_InputField iterationsAI;


    //enable and disable Unity events
    void OnEnable()
    {
        MazeCell.winEvent += WinDisplay;
        AI.winEvent += WinDisplay;
        Maze.generatedEvent += PopupDisplay;
    }

    void OnDisable()
    {
        MazeCell.winEvent -= WinDisplay;
        AI.winEvent -= WinDisplay;
        Maze.generatedEvent -= PopupDisplay;
    }


    // Start is called before the first frame update
    public void StartPanel()
    {
        startPanel.gameObject.SetActive(false);

        //test values, otherwise use defaults
        bool testX = int.TryParse(sizeX.text, out var x);
        bool testY = int.TryParse(sizeY.text, out var y);


        if (testX && x > 1)
        {
            storedSize.x = x;
        }
        
        if (testY && y > 1)
        {
            storedSize.z = y;
        }
        
        vcam.transform.position += 
            new Vector3(0.0f, Mathf.Max(storedSize.x,storedSize.z, 10), 0);
        BeginGame();
    }


    // Update is called once per frame
    void Update()
    {
        if (!gameStarted)
        {
            //regenerate the maze when space is pressed
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RestartMaze();
            }

            if (Input.GetKeyDown(KeyCode.G) && mazeInstance.MazeGenerated) //Test to see if getting the cell child works -IT WORKS-
            {

                GenerateRewards();
                StartAndEndGeneration();
            }
        }
    }


    private void BeginGame() //Function to start maze generation
    {
        mazeInstance = Instantiate(mazePrefab) as Maze;
        mazeInstance.size = new IntVector2(storedSize.x, storedSize.z);
        StartCoroutine(mazeInstance.Generate());
    }

    private void RestartMaze() //Function to restart maze generation
    {
        StopAllCoroutines();
        Destroy(mazeInstance.gameObject);
        if (popupPanel.gameObject.activeSelf)
        {
            popupPanel.gameObject.SetActive(false);
        }

        BeginGame();
    }

    
    #region Maze Management

    private void GenerateRewards()
    {

        for (int row = 0; row < mazeInstance.cells.GetLength(0); row++) //loop through the maze rows
        {
            for (int col = 0; col < mazeInstance.cells.GetLength(1); col++) //loop through the maze columns
            {
                for (int childLevel = 1; childLevel < 5; childLevel++) //loop through the children of the maze cell
                {
                    if (mazeInstance.cells[row, col].transform.GetChild(childLevel).GetComponent("MazePassage") != null) //check to see if the child is a passage
                    {
                        MazeDirection dir = mazeInstance.cells[row, col].transform.GetChild(childLevel).GetComponent<MazeCellEdge>().direction;
                        mazeInstance.cells[row, col].AssignInitialReward(dir, 0);

                        //Debug.Log("Cell: " + row + " " + col + "-" + dir + "Passage"
                        //          + mazeInstance.cells[row, col].Rewards[0] + mazeInstance.cells[row, col].Rewards[1]
                        //          + mazeInstance.cells[row, col].Rewards[2] + mazeInstance.cells[row, col].Rewards[3]);
                    }
                    //else if (mazeInstance.cells[row, col].transform.GetChild(childLevel).GetComponent("MazeWall") != null) //check to see if the child is a wall
                    //{
                    //    MazeDirection dir = mazeInstance.cells[row, col].transform.GetChild(childLevel).GetComponent<MazeCellEdge>().direction;
                    //    Debug.Log("Cell: " + row + " " + col + "-" + dir + "Wall");
                    //}
                    //else
                    //{
                    //    Debug.Log("Error");
                    //}
                }


                /*
                MazeDirection dir = mazeInstance.cells[row, col].transform.GetChild(1).GetComponent<MazeCellEdge>().direction;
                Debug.Log("Cell: " + row + " " + col + " " + dir);
                dir = mazeInstance.cells[row, col].transform.GetChild(2).GetComponent<MazeCellEdge>().direction;
                Debug.Log("Cell: " + row + " " + col + " " + dir);
                dir = mazeInstance.cells[row, col].transform.GetChild(3).GetComponent<MazeCellEdge>().direction;
                Debug.Log("Cell: " + row + " " + col + " " + dir);
                dir = mazeInstance.cells[row, col].transform.GetChild(4).GetComponent<MazeCellEdge>().direction;
                Debug.Log("Cell: " + row + " " + col + " " + dir);
                */
            }
        }
    }

    //generate start and end, also initialize game
    private void StartAndEndGeneration()
    {
        var xStart = 0;
        var yStart = 0;
        var xEnd = 0;
        var yEnd = 0;
        

        //generate start and end values until they are different
        while (xStart == xEnd && yStart == yEnd)
        {
            xStart = Random.Range(0, mazeInstance.cells.GetLength(0));
            yStart = Random.Range(0, mazeInstance.cells.GetLength(1));

            xEnd = Random.Range(0, mazeInstance.cells.GetLength(0));
            yEnd = Random.Range(0, mazeInstance.cells.GetLength(1));
        }

        var startCell = mazeInstance.cells[xStart, yStart];
        var endCell = mazeInstance.cells[xEnd, yEnd];

        //set material representing end location
        endCell.gameObject.transform.GetChild(0).GetComponent<Renderer>().material = endMat;
        endCell.EnableEndCollider();


        //Debug.Log(startCell.coordinates.x + " " + startCell.coordinates.z);
        //Debug.Log(endCell.coordinates.x + " " + endCell.coordinates.z);


        //set up player and AI locations
        player.SetActive(true); 
        ai.SetActive(true);

        player.GetComponent<Player>().Init(startCell);


        //test iterations value
        bool testIter = int.TryParse(iterationsAI.text, out var result);
        int iterations = testIter ? result : 1;

        ai.GetComponent<AI>().Init(startCell, endCell, iterations, mazeInstance.cells);

        popupPanel.gameObject.SetActive(false);
        gameStarted = true;
    }
    #endregion



    #region Game Menus


    //when game is won
    public void WinDisplay(bool playerWon)
    {

        winPanel.gameObject.SetActive(true);
        if (winText.text == "")
        {
            winText.text = playerWon ? "The player has won." : "The AI has won.";
        }
    }

    //display popup after panel
    public void PopupDisplay()
    {
        popupPanel.gameObject.SetActive(true);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

}
