//Hunter Chu and Edward Cao
//100701653 and 100697845
//March 28th, 2022

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using Random = UnityEngine.Random;


public class MazeCell : MonoBehaviour
{
    //variables
    public IntVector2 coordinates;
    private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];
    private int initializedEdgeCount;

    //get colliders; first is floor collision and second is win entrance
    private BoxCollider[] colliders;

    //N, E, S, W
    private Tuple<MazeDirection, float>[] rewards =
    {
        new Tuple<MazeDirection, float>(MazeDirection.NORTH, -1),
        new Tuple<MazeDirection, float>(MazeDirection.EAST, -1), 
        new Tuple<MazeDirection, float>(MazeDirection.SOUTH, -1), 
        new Tuple<MazeDirection, float>(MazeDirection.WEST, -1)
    };
    public Tuple<MazeDirection, float>[] Rewards => rewards;

    public bool IsFullyInitialized => initializedEdgeCount == MazeDirections.Count;


    public static event Action<bool> winEvent;


    public MazeCellEdge GetEdge(MazeDirection direction) //returns direction of an edge
    {
        return edges[(int)direction];
    }

    public void SetEdge(MazeDirection direction, MazeCellEdge edge) //sets an edge of the maze cell
    {
        edges[(int)direction] = edge;
        initializedEdgeCount += 1;
    }

    public MazeDirection RandomUninitializedDirection
    {
        get
        {
            int skips = Random.Range(0, MazeDirections.Count - initializedEdgeCount); //set a random amount of empty edges
            for (int i = 0; i < MazeDirections.Count; i++) //cycle through the remaining directions of that cell
            {
                if (edges[i] == null) //if the edge is empty
                {
                    if (skips == 0) //check to see if there the cell has any more empty edges it can use
                    {
                        return (MazeDirection)i;
                    }
                    skips -= 1;
                }
            }
            throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
        }
    }

    //assign possible future rewards to list
    public void AssignInitialReward(MazeDirection direction, float rewardVal)
    {
        rewards[(int)direction] = new Tuple<MazeDirection, float>(direction, rewardVal);

    }

    //enable end tile collision
    public void EnableEndCollider()
    {
        colliders = gameObject.GetComponents<BoxCollider>();
        colliders[1].enabled = true;
    }

    //player wins if they move over end tile
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            winEvent?.Invoke(true);
        }

    }

}
