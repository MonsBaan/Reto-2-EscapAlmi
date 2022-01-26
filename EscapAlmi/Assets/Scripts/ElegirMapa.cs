using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ElegirMapa : MonoBehaviour
{
    public GameObject server;
    public GameObject cliente;
    public GameObject[] arrayMapas;
    public static int indexMapa;
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

    public void cargarMapa(int index)
    {
        Instantiate(arrayMapas[index]);
    }
}
