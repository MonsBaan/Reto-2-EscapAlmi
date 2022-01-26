using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptSalaEspera : MonoBehaviour
{
    public GameObject prefabListaObj;
    public GameObject contentScroll;
    // Start is called before the first frame update
    public static int numJugadores = 0;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<ScriptSalaEsperaCliente>().enabled == true && GetComponent<ScriptSalaEsperaCliente>().listaJugadores.Count != numJugadores)
        {
            List<NetworkObject.NetworkObject.Jugador> listaJugadores = GetComponent<ScriptSalaEsperaCliente>().listaJugadores;
            numJugadores = listaJugadores.Count;



            foreach (var item in GameObject.FindGameObjectsWithTag("NombreLista"))
            {
                Destroy(item.gameObject);
            }

            foreach (var jugador in listaJugadores)
            {
                añadirJugador(jugador.nombre);
            }


        }
        else if(GetComponent<ServerSalaEspera>().enabled == true && GetComponent<ServerSalaEspera>().listaJugadores.Count != numJugadores)
        {
            List<NetworkObject.NetworkObject.Jugador> listaJugadores = GetComponent<ServerSalaEspera>().listaJugadores;
            numJugadores = listaJugadores.Count;

            foreach (var item in GameObject.FindGameObjectsWithTag("NombreLista"))
            {
                Destroy(item.gameObject);
            }

            foreach (var jugador in listaJugadores)
            {
                añadirJugador(jugador.nombre);
            }
        }
        
    }

    public void añadirJugador(string nombre)
    {
        GameObject jugadorNuevo = prefabListaObj;
        jugadorNuevo.GetComponent<Text>().text = nombre;
        Instantiate(jugadorNuevo, contentScroll.transform);

    }
}
