//Hunter Chu and Edward Cao
//100701653 and 100697845
//March 28th, 2022

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    //variables
    private MazeCell start;
    private MazeCell end;
    private int iterations = 50000;
    private int moveLimit = 10000;
    private int moves = 0;
    private float gamma = 0.8f;

    private MazeCell[,] cells;

    //length for qTable
    //condense 2D array to 1D array with x + (y * height)
    private int mazeHeight;
    private float[][] qTable;


    public static event Action<bool> winEvent;


    public float GetReward(MazeCell currentState, int action) //function to get the reward for an action at a mazecell
    {
        return currentState.Rewards[action].Item2;
    }

    //check if goal is reached
    public bool GoalStateIsReached(GameObject currentState)
    {
        return currentState.gameObject.Equals(end.gameObject);
    }

    //initialize AI
    public void Init(MazeCell startCell, MazeCell endCell, int iter, MazeCell[,] mazeCells)
    {
        gameObject.transform.position = startCell.transform.position
                                        + new Vector3(0, 0.585f, 0);

        start = startCell;
        end = endCell;

        iterations = iter;
        cells = mazeCells;

        mazeHeight = cells.GetLength(1);

        //initialize q-table
        qTable = new float[mazeCells.GetLength(0) * mazeCells.GetLength(1)][];
        for (int i = 0; i < qTable.Length; i++)
        {
            qTable[i] = new float[] { 0, 0, 0, 0 };
        }

        var validMoves = ValidRewards(cells[endCell.coordinates.x, endCell.coordinates.z].Rewards);

        for (int i = 0; i < validMoves.Count; i++)
        {
            qTable[end.coordinates.x + end.coordinates.z * mazeHeight][(int)validMoves[i].Item1] = 100.0f;
        }


        TrainAI();

        RunAI();
    }

    //starts training process
    public void TrainAI()
    {
        //runs multiple iterations
        for (int i = 0; i < iterations; i++)
        {
            InitializeRun(start);
        }

    }

    //starts an iteration of training
    private void InitializeRun(MazeCell initialState)
    {
        var currentState = initialState;

        //grabs next state until goal is reached
        //will reset after a certain move limit
        while (true)
        {
            currentState = TakeAction(currentState);
            if (GoalStateIsReached(currentState.gameObject) || moves > moveLimit)
            {
                break;
            }
            moves++;
            //Debug.Log(moves);
        }
    }

    //get next action
    private MazeCell TakeAction(MazeCell currentState)
    {
        //gets valid actions and randomly selects one
        var validActions = ValidRewards(currentState.Rewards);
        int nextRand = Random.Range(0, validActions.Count);


        //find the attempted action, then move to that tile and find its max
        var currentReward = validActions[nextRand].Item2;

        //get next tile
        int x = currentState.coordinates.x;
        int y = currentState.coordinates.z;
        switch (validActions[nextRand].Item1)
        {
            case (MazeDirection.NORTH):
                y += 1;
                break;
            case (MazeDirection.EAST):
                x += 1;
                break;
            case (MazeDirection.SOUTH):
                y -= 1;
                break;
            case (MazeDirection.WEST):
                x -= 1;
                break;
        }

        int nextTile = x + y * mazeHeight;

        //get max reward of next tile
        var nextReward = qTable[nextTile].Max();

        //calculate value and update q-table
        //gamma discounts the future reward value
        var calcedState = currentReward + (gamma * nextReward);
        qTable[currentState.coordinates.x + currentState.coordinates.z * mazeHeight]
            [(int)validActions[nextRand].Item1] = calcedState;

        //Debug.Log(x + " " + y);
        MazeCell nextCell = cells[x, y];
        return nextCell;
    }


    //grabs all valid rewards (rewards that can be obtained)
    private List<Tuple<MazeDirection, float>> ValidRewards(Tuple<MazeDirection, float>[] rewards)
    {
        var validRewards = new List<Tuple<MazeDirection, float>>();

        foreach (var t in rewards)
        {
            if (t.Item2 >= 0)
            {
                validRewards.Add(t);
            }
        }

        //foreach (var t in validRewards)
        //{
        //    Debug.Log(t.Item1 + " " + t.Item2);
        //}

        return validRewards;
    }

    //post-training
    public void RunAI()
    {
        var state = start.gameObject;
        //var steps = 0;
        var actions = new List<GameObject>();


        //foreach (var t in qTable)
        //{
        //    Debug.Log(t[0] + " " + t[1] + " " + t[2] + " " + t[3]);
        //}


        while (true)
        {
            //steps++;

            int direction = qTable[state.GetComponent<MazeCell>().coordinates.x + state.GetComponent<MazeCell>().coordinates.z * mazeHeight].ToList().IndexOf(
                qTable[state.GetComponent<MazeCell>().coordinates.x + state.GetComponent<MazeCell>().coordinates.z * mazeHeight].Max());


            //convert direction
            int x = state.GetComponent<MazeCell>().coordinates.x;
            int y = state.GetComponent<MazeCell>().coordinates.z;
            switch (direction)
            {
                //north, east, south, west, in that order
                case (0):
                    y += 1;
                    break;
                case (1):
                    x += 1;
                    break;
                case (2):
                    y -= 1;
                    break;
                case (3):
                    x -= 1;
                    break;
            }

            //move to next cell and add to list
            GameObject action = cells[x, y].gameObject;
            state = action;
            
            actions.Add(action);
            if (GoalStateIsReached(action))
            {
                break;
            }

        }

        //Debug.Log(steps);
        //foreach (var t in actions)
        //{
        //    Debug.Log(t.GetComponent<MazeCell>().coordinates.x + " " + t.GetComponent<MazeCell>().coordinates.z);
        //}

        StartCoroutine(AIMovement(actions));
    }


    //coroutine to display movement
    IEnumerator AIMovement(List<GameObject> actionList)
    {
        for (int i = 0; i < actionList.Count; i++)
        {
            yield return new WaitForSeconds(2.0f);
            gameObject.transform.position = actionList[i].transform.position;
        }

        //if it reaches the end, AI wins
        winEvent?.Invoke(false);
    }
}
