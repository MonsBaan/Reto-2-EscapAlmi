using NetworkObject.NetworkMessages;
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
    public GameObject textoSalida;

    public float tiempoRespawnItems = 10;
    public float secTotal = 0;

    public float sec, min = 0;

    public static GameObject fin;

    // Start is called before the first frame update
    void Start()
    {
        if (MainMenuController.esServer)
        {
            server.SetActive(true);

            indexMapa = Random.Range(0, arrayMapas.Length);
            arrayMapas[indexMapa].name = "MapaActual";

            Instantiate(arrayMapas[indexMapa]);
        }
        else
        {
            cliente.SetActive(true);
        }


    }

    private void Update()
    {
        timerItems();

        if (!MainMenuController.esServer)
            return;

        min = Mathf.FloorToInt(secTotal / 60F);
        sec = Mathf.FloorToInt(secTotal - min * 60);

        if (min >= 5)
        {
            GameObject.Find("Server").GetComponent<Server>().terminarPartida();

        }
        else if (min >= 4)
        {
            GameObject.Find("Tiempo").GetComponent<Text>().color = Color.red;
            textoSalida.GetComponent<Text>().color = Color.red;
            textoSalida.GetComponent<Text>().text = "Se Acaba El Tiempo!!";
        }
        else if (min >= 1)
        {
            Color greenColor;
            ColorUtility.TryParseHtmlString("#0AC742", out greenColor);

            GameObject.Find("Tiempo").GetComponent<Text>().color = greenColor;
            textoSalida.GetComponent<Text>().color = greenColor;
            textoSalida.SetActive(true);
            fin.SetActive(true);

        }
        else if (min < 1)
        {
            fin.SetActive(false);
            textoSalida.SetActive(false);
        }

        string niceTime = string.Format("{0:0}:{1:00}", min, sec);
        GameObject.Find("Tiempo").GetComponent<Text>().text = niceTime;

        secTotal += Time.deltaTime;

        TiempoMsg tiempo = new TiempoMsg();
        tiempo.min = min;
        tiempo.sec = sec;
        for (int i = 0; i < server.GetComponent<Server>().m_connections.Length; i++)
        {
            server.GetComponent<Server>().SendToClient(JsonUtility.ToJson(tiempo), server.GetComponent<Server>().m_connections[i]);
        }

    }

    public void changeTiempo(float min, float sec)
    {
        string niceTime = string.Format("{0:0}:{1:00}", min, sec);
        GameObject.Find("Tiempo").GetComponent<Text>().text = niceTime;

        if (min >= 4)
        {
            GameObject.Find("Tiempo").GetComponent<Text>().color = Color.red;
            textoSalida.GetComponent<Text>().color = Color.red;
        }
        else if (min >= 1)
        {
            Color greenColor;
            ColorUtility.TryParseHtmlString("#0AC742", out greenColor);

            GameObject.Find("Tiempo").GetComponent<Text>().color = greenColor;
            textoSalida.GetComponent<Text>().color = greenColor;
            textoSalida.SetActive(true);
            fin.SetActive(true);

        }
        else if (min < 1)
        {
            fin.SetActive(false);
            textoSalida.SetActive(false);
        }
    }

    public void cargarMapa(int index)
    {
        arrayMapas[indexMapa].name = "MapaActual";
        Instantiate(arrayMapas[index]);
    }

    public void timerItems()
    {
        foreach (var item in GameObject.Find("MapaActual(Clone)").GetComponent<ScriptLaberintoOK>().poolItems)
        {
            if (!item.activeSelf)
            {
                if (item.GetComponent<ScriptItem>().tiempoRespawn >= tiempoRespawnItems)
                {
                    item.GetComponent<ScriptItem>().tiempoRespawn = 0;
                    item.SetActive(true);
                }
                else
                {
                    item.GetComponent<ScriptItem>().tiempoRespawn += Time.deltaTime;
                }
            }
        }
    }
}
