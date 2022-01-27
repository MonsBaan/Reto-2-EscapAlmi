using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptLuz : MonoBehaviour
{
    public GameObject jugadorREAL;
    // Start is called before the first frame update
    private Transform miTransform;

    public GameObject mainCamera;
    private GameObject server, cliente;
    void Start()
    {
        miTransform = this.transform;
        mainCamera = GameObject.Find("Main Camera");
        server = mainCamera.GetComponent<CameraScript>().server;
        cliente = mainCamera.GetComponent<CameraScript>().cliente;
    }

    // Update is called once per frame
    void Update()
    {
        if (jugadorREAL == null)
        {
            if (server.activeSelf)
            {
                jugadorREAL = server.GetComponent<Server>().myPlayer;
            }
            else if (cliente.activeSelf)
            {
                jugadorREAL = cliente.GetComponent<NetworkClient>().jugadoresSimulados[int.Parse(cliente.GetComponent<NetworkClient>().idPlayer)];
            }
        }
        else
        {
            miTransform.LookAt(jugadorREAL.transform);

        }
    }
}
