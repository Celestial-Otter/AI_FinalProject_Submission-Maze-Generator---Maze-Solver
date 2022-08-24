//Hunter Chu and Edward Cao
//100701653 and 100697845
//March 28th, 2022

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Maze : MonoBehaviour
{
    //Declaration of Variables
    public MazeCell cellPrefab;
    public MazePassage passagePrefab;
    public MazeWall wallPrefab;
    public MazeCell[,] cells;
    public float generationStepDelay;
    public IntVector2 size;

    private bool mazeGenerated = false;
    public bool MazeGenerated => mazeGenerated;
    public static event Action generatedEvent;

    public MazeCell GetCell (IntVector2 coordinates) //function to get the position of a cell
    {
        return cells[coordinates.x, coordinates.z];
    }
    public IEnumerator Generate() //function to generate the maze
    {
        WaitForSeconds delay = new WaitForSeconds(generationStepDelay); //adds a delay so you can see the maze being generated in real time, can be removed or set to 0 to generate maze instantly
        cells = new MazeCell[size.x, size.z];
        List<MazeCell> activeCells = new List<MazeCell>();
        DoFirstGenerationStep(activeCells);
        while(activeCells.Count > 0)
        {
            yield return delay;
            DoNextGenerationStep(activeCells);
        }
        IntVector2 coordinates = RandomCoordinates;
        while(ContainsCoordinates(coordinates) && GetCell(coordinates) == null)
        {
            yield return delay;
            CreateCell(coordinates);
            coordinates += MazeDirections.RandomValue.ToIntVector2();
        }
        mazeGenerated = true;

        generatedEvent?.Invoke();
    }

    private MazeCell CreateCell (IntVector2 coordinates) //fucntion to create a mazecell tile
    {
        MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
        cells[coordinates.x, coordinates.z] = newCell;
        newCell.coordinates = coordinates;
        newCell.name = "Maze Cell " + coordinates.x + " " + coordinates.z;
        newCell.transform.parent = transform;
        newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
        return newCell;
    }

    public IntVector2 RandomCoordinates //returns a random coordinate in the maze
    {
        get
        {
            return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
        }
    }

    public bool ContainsCoordinates (IntVector2 coordinate) //check to see if coordinates are within the maze
    {
        return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
    }

    private void DoFirstGenerationStep (List<MazeCell> activeCells) //generate a mazecell at a random coordinate
    {
        activeCells.Add(CreateCell(RandomCoordinates));
    }

    private void DoNextGenerationStep (List<MazeCell> activeCells) //function to generate more maze cells
    {
        int currentIndex = activeCells.Count - 1;
        MazeCell currentCell = activeCells[currentIndex];
        if (currentCell.IsFullyInitialized) //check to see if the current cell is already fully initialized
        {
            activeCells.RemoveAt(currentIndex);
            return;
        }
        MazeDirection direction = currentCell.RandomUninitializedDirection; //choose a random direction to move in
        IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2(); //go in that random direction
        if(ContainsCoordinates(coordinates)) //check to see if the coordinates we want to go to are within the maze
        {
            MazeCell neighbor = GetCell(coordinates);
            if (neighbor == null) //if that maze node is empty
            {
                neighbor = CreateCell(coordinates); //create a cell
                CreatePassage(currentCell, neighbor, direction);
                activeCells.Add(neighbor); //add that cell to the active cells list
            }
            else
            {
                CreateWall(currentCell, neighbor, direction); //create a wall if there's already an initialized node there
            }

        }
        else
        {
            CreateWall(currentCell, null, direction); //create an edge wall because we've hit the edge of the maze
        }



            //MazeDirection dir = cells[0, 0].transform.GetChild(1).GetComponent<MazeCellEdge>().direction;

    }

    private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction) //function to create a passage
    {
        MazePassage passage = Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(cell, otherCell, direction);
        passage = Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(otherCell, cell, direction.GetOpposite());

    }

    private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction) //function to create a wall
    {
        MazeWall wall = Instantiate(wallPrefab) as MazeWall;
        wall.Initialize(cell, otherCell, direction);
        if(otherCell != null)
        {
            wall = Instantiate(wallPrefab) as MazeWall;
            wall.Initialize(otherCell, cell, direction.GetOpposite());
        }
    }
}
