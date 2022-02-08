using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptItem : MonoBehaviour
{
    public float velocidadFlotar;
    public float velocidadRotacion;
    private Transform miTransform;
    private Vector3 direccion = Vector3.up;
    public int esteIndex;

    public float tiempoRespawn = 0;

    // Start is called before the first frame update
    void Start()
    {
        miTransform = this.transform;
    }

    private void FixedUpdate()
    {
        if(transform.position.y >= 1.5)
        {
            direccion = Vector3.down;
        }
        else if(transform.position.y <= 0.5)
        {
            direccion = Vector3.up;
        }

        miTransform.Translate(direccion * velocidadFlotar * Time.deltaTime);
        miTransform.RotateAround(Vector3.up, velocidadRotacion);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && other.gameObject.GetComponent<MovimientoJugador>().isActiveAndEnabled)
        {
            if (this.gameObject.tag.Equals("Coin"))
            {
                GameObject.Find("Monedas").GetComponent<Text>().text = int.Parse(GameObject.Find("Monedas").GetComponent<Text>().text) + 31 + "";

                if (MainMenuController.esServer)
                {
                    GameObject.Find("Server").GetComponent<Server>().itemGet(esteIndex, "moneda");
                    this.gameObject.SetActive(false);
                }
                else
                {
                    GameObject.Find("Cliente").GetComponent<NetworkClient>().itemGet(esteIndex, "moneda");
                }
            }

            if (this.gameObject.tag.Equals("Arena"))
            {
                powerup("Item1");
            }
            if (this.gameObject.tag.Equals("Botas"))
            {
                powerup("Item2");
            }
            if (this.gameObject.tag.Equals("Lupa"))
            {
                powerup("Item3");
            }
            if (this.gameObject.tag.Equals("RevientaMuros"))
            {
                powerup("Item4");
            }

        }
    }

    public void powerup(string itemName)
    {
        string cantidad = GameObject.Find(itemName).transform.GetChild(0).gameObject.GetComponent<Text>().text;
        GameObject.Find(itemName).transform.GetChild(0).gameObject.GetComponent<Text>().text = int.Parse(cantidad) + 1 + "";

        if (MainMenuController.esServer)
        {
            GameObject.Find("Server").GetComponent<Server>().itemGet(esteIndex, "powerup");
            //GameObject.Find("MapaActual(Clone)").GetComponent<ScriptLaberintoOK>().poolItems[esteIndex].SetActive(false);
            //desacPowerup();
            this.gameObject.SetActive(false);
        }
        else
        {
            GameObject.Find("Cliente").GetComponent<NetworkClient>().itemGet(esteIndex, "powerup");
        }
    }

    public void setIndex(int index)
    {
        esteIndex = index;
    }

}
