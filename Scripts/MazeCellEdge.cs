//Hunter Chu and Edward Cao
//100701653 and 100697845
//March 28th, 2022

using UnityEngine;

public abstract class MazeCellEdge : MonoBehaviour
{
    public MazeCell cell, otherCell;
    public MazeDirection direction;


    public void Initialize (MazeCell cell, MazeCell otherCell, MazeDirection direction) //function to initialize an edge
    {
        this.cell = cell;
        this.otherCell = otherCell;
        this.direction = direction;
        cell.SetEdge(direction, this);
        transform.parent = cell.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = direction.ToRotation();
    }


}
