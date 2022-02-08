using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    private Transform jugador;

    private void Start()
    {
           
    }

    private void OnEnable()
    {
        jugador = this.transform.parent;
        Invoke("desaparece", 2);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if (MainMenuController.esServer)
            {
                Debug.Log(other.gameObject.GetComponent<JugadorSimuladoScript>().getJugadorId());
                GameObject.Find("Server").GetComponent<Server>().efectoArena(other.gameObject.GetComponent<JugadorSimuladoScript>().getJugadorId());
            }
            else
            {
                GameObject.Find("Cliente").GetComponent<NetworkClient>().efectoArena(other.gameObject.GetComponent<JugadorSimuladoScript>().getJugadorId());
            }
        }
    }

    public void desaparece()
    {
        this.gameObject.SetActive(false);
    }
}
