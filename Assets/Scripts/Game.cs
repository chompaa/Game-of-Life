using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject cell;
    GameObject[,] cells;

    public int numberOfCells;

    public float percentageAlive;
    public float updateTime;

    public Color aliveColor;
    public Color deadColor;

    /*
        Any live cell with two or three live neighbours survives.
        Any dead cell with three live neighbours becomes a live cell.
        All other live cells die in the next generation. Similarly, all other dead cells stay dead.
    */

    void Start()
    {
        cells = new GameObject[numberOfCells, numberOfCells];

        for (int row = 0; row < numberOfCells; row++)
        {
            for (int col = 0; col < numberOfCells; col++)
            {
                cells[row, col] = Instantiate(cell, new Vector3(row, col, 0), Quaternion.identity);

                if (Random.value > percentageAlive / 100)
                {
                    SetState(cells[row, col], Cell.States.Dead);
                }
                else
                {
                    SetState(cells[row, col], Cell.States.Alive);
                }
            }
        }

        mainCamera.transform.position = new Vector3(numberOfCells / 2 - 0.5f, numberOfCells / 2 - 0.5f, - 10);
        mainCamera.GetComponent<Camera>().orthographicSize = numberOfCells / 2 + 1;

        // start the game!
        InvokeRepeating(nameof(Evolve), 0, updateTime);
    }

    void SetState(GameObject cell, Cell.States state)
    {
        cell.GetComponent<Cell>().state = state;
        cell.GetComponent<SpriteRenderer>().color = state == Cell.States.Alive ? aliveColor : deadColor;
    }

    void Evolve()
    {
        Cell.States[,] queuedStates = new Cell.States[numberOfCells, numberOfCells];

        for (int row = 0; row < numberOfCells; row++)
        {
            for (int col = 0; col < numberOfCells; col++)
            {
                Cell.States state = cells[row, col].GetComponent<Cell>().state;
                int neighbours = CheckAliveNeighbours(row, col);
                queuedStates[row, col] = state;

                // don't update the state yet, it will yield unwanted results
                if (state == Cell.States.Alive)
                {
                    if (neighbours != 2 && neighbours != 3)
                    {
                        queuedStates[row, col] = Cell.States.Dead;
                    }
                }
                else
                {
                    if (neighbours == 3)
                    {
                        queuedStates[row, col] = Cell.States.Alive;
                    }
                }
            }
        }

        // ... now update the state
        for (int row = 0; row < numberOfCells; row++)
        {
            for (int col = 0; col < numberOfCells; col++)
            {
                Cell.States state = cells[row, col].GetComponent<Cell>().state;

                if (state != queuedStates[row, col])
                {
                    SetState(cells[row, col], queuedStates[row, col]);
                }
            }
        }
    }

    int CheckAliveNeighbours(int cellRow, int cellCol)
    {
        int neighbours = 0;

        for (int row = - 1; row <= 1; row++)
        {
            for (int col = - 1; col <= 1; col++)
            {
                int rowSum = row + cellRow;
                int colSum = col + cellCol;

                if (rowSum >= 0 && rowSum < numberOfCells && colSum >= 0 && colSum < numberOfCells)
                {
                    // make sure we don't count our current cell
                    if(rowSum != cellRow || colSum != cellCol)
                    {
                        if (cells[rowSum, colSum].GetComponent<Cell>().state == Cell.States.Alive)
                        {
                            neighbours++;
                        }
                    }
                }
            }
        }

        return neighbours;
    }
}
