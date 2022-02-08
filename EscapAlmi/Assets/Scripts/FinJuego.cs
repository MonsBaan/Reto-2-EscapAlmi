using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinJuego : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ElegirMapa.fin = this.gameObject;
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") && other.gameObject.GetComponent<MovimientoJugador>().isActiveAndEnabled)
        {
            if (MainMenuController.esServer)
            {
                GameObject.Find("Server").GetComponent<Server>().finPlayer();
                other.tag = "PlayerFin";
            }
            else
            {
                GameObject.Find("Cliente").GetComponent<NetworkClient>().finPlayer();
                other.tag = "PlayerFin";
            }
        }
    }
}
