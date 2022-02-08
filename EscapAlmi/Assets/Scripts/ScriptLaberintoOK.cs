using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptLaberintoOK : MonoBehaviour
{
    public GameObject[] PowerUps = null;
    public GameObject[] Coins = null;

    public List<GameObject> poolItems;
    public List<GameObject> poolCoins;

    // Start is called before the first frame update
    void Start()
    {
        this.PowerUps = GetComponent<MazeSpawner>().PowerUps;
        this.Coins = GetComponent<MazeSpawner>().Coins;
        this.poolItems = GetComponent<MazeSpawner>().poolItems;
        this.poolCoins = GetComponent<MazeSpawner>().poolCoins;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
