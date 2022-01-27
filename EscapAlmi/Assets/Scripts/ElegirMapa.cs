using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ElegirMapa : MonoBehaviour
{
    public GameObject server;
    public GameObject cliente;
    public GameObject[] arrayMapas;
    public static int indexMapa;

    public float sec;
    public int min;

    // Start is called before the first frame update
    void Start()
    {
        if (MainMenuController.esServer)
        {
            server.SetActive(true);

            indexMapa = Random.Range(0, arrayMapas.Length);
            Instantiate(arrayMapas[indexMapa]);
        }
        else
        {
            cliente.SetActive(true);
        }
    }

    private void Update()
    {
        sec += Time.deltaTime;
        if (sec >= 60)
        {
            sec = 0;
            min++;
        }
        string secStr = sec.ToString("#");
        if (sec <= 9)
        {
            secStr = "0" + secStr;
        }
        GameObject.Find("Tiempo").GetComponent<Text>().text = min + ":" + secStr;
    }

    public void cargarMapa(int index)
    {
        Instantiate(arrayMapas[index]);
    }

    public int indexMoneda(GameObject moneda)
    {
        return arrayMapas[indexMapa].GetComponent<MazeSpawner>().poolCoins.IndexOf(moneda);
    }
}
