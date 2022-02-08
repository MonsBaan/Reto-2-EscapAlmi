using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>
public class MazeSpawner : MonoBehaviour
{
    public enum MazeGenerationAlgorithm
    {
        PureRecursive,
        RecursiveTree,
        RandomTree,
        OldestTree,
        RecursiveDivision,
    }

    public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
    public bool FullRandom = false;
    public int RandomSeed = 12345;
    public GameObject Floor = null;
    public GameObject Wall = null;
    public GameObject WallBorder = null;
    public GameObject Pillar = null;
    public int Rows = 5;
    public int Columns = 5;
    public float CellWidth = 5;
    public float CellHeight = 5;
    public bool AddGaps = true;
    public GameObject[] PowerUps = null;
    public GameObject[] Coins = null;

    public List<GameObject> poolItems;
    public List<GameObject> poolCoins;

    private BasicMazeGenerator mMazeGenerator = null;

    void Start()
    {
        if (!FullRandom)
        {
            Random.seed = RandomSeed;
        }
        switch (Algorithm)
        {
            case MazeGenerationAlgorithm.PureRecursive:
                mMazeGenerator = new RecursiveMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveTree:
                mMazeGenerator = new RecursiveTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RandomTree:
                mMazeGenerator = new RandomTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.OldestTree:
                mMazeGenerator = new OldestTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveDivision:
                mMazeGenerator = new DivisionMazeGenerator(Rows, Columns);
                break;
        }
        mMazeGenerator.GenerateMaze();
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                float x = column * (CellWidth + (AddGaps ? .2f : 0));
                float z = row * (CellHeight + (AddGaps ? .2f : 0));
                MazeCell cell = mMazeGenerator.GetMazeCell(row, column);
                GameObject tmp;
                tmp = Instantiate(Floor, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0)) as GameObject;
                tmp.transform.parent = transform;

                if (cell.WallRight)
                {

                    if (column == Columns-1) //LIMITE DERECHO
                    {
                        tmp = Instantiate(WallBorder, new Vector3(x + CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;
                        tmp.name = "MuroRightInv";
                        tmp.tag = "MuroInvencible";
                        tmp.transform.parent = transform;
                    }
                    else
                    {
                        tmp = Instantiate(Wall, new Vector3(x + CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;
                        tmp.name = "MuroRight";
                        tmp.transform.parent = transform;

                    }

                }
                if (cell.WallFront)
                {

                    if (row == Rows-1) //LIMITE FRONTAL
                    {
                        tmp = Instantiate(WallBorder, new Vector3(x, 0, z + CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                        tmp.name = "MuroFrontInv";
                        tmp.tag = "MuroInvencible";
                        tmp.transform.parent = transform;
                    }
                    else
                    {
                        tmp = Instantiate(Wall, new Vector3(x, 0, z + CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
                        tmp.name = "MuroFront";
                        tmp.transform.parent = transform;
                    }

                }
                if (cell.WallLeft)
                {
                    if (column == 0) //LIMITE IZQUIERDO
                    {
                        tmp = Instantiate(WallBorder, new Vector3(x - CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 270, 0)) as GameObject;
                        tmp.name = "MuroLeftInv";
                        tmp.tag = "MuroInvencible";
                        tmp.transform.parent = transform;
                    }
                    else
                    {
                        tmp = Instantiate(Wall, new Vector3(x - CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 270, 0)) as GameObject;
                        tmp.name = "MuroLeft";
                        tmp.transform.parent = transform;

                    }

                }
                if (cell.WallBack)
                {
                    if (row == 0) //LIMITE ATRAS
                    {
                        tmp = Instantiate(WallBorder, new Vector3(x, 0, z - CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 180, 0)) as GameObject;
                        tmp.name = "MuroBackInv";
                        tmp.tag = "MuroInvencible";
                        tmp.transform.parent = transform;
                    }
                    else
                    {
                        tmp = Instantiate(Wall, new Vector3(x, 0, z - CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 180, 0)) as GameObject;
                        tmp.name = "MuroBack";
                        tmp.transform.parent = transform;
                    }

                }
                if (cell.IsGoal && PowerUps != null)
                {
                    int numRandom = Random.Range(0, 4);

                    GameObject item = Instantiate(PowerUps[numRandom], new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
                    item.GetComponent<ScriptItem>().setIndex(poolItems.Count);
                    poolItems.Add(item);
                    item.transform.parent = transform;
                }
                else
                {
                    GameObject moneda = Instantiate(Coins[0], new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
                    moneda.GetComponent<ScriptItem>().setIndex(poolCoins.Count);
                    poolCoins.Add(moneda);
                    moneda.transform.parent = transform;
                }
            }
        }
        if (Pillar != null)
        {
            for (int row = 0; row < Rows + 1; row++)
            {
                for (int column = 0; column < Columns + 1; column++)
                {
                    float x = column * (CellWidth + (AddGaps ? .2f : 0));
                    float z = row * (CellHeight + (AddGaps ? .2f : 0));
                    GameObject tmp = Instantiate(Pillar, new Vector3(x - CellWidth / 2, 0, z - CellHeight / 2), Quaternion.identity) as GameObject;
                    tmp.transform.parent = transform;
                }
            }
        }
    }
}
