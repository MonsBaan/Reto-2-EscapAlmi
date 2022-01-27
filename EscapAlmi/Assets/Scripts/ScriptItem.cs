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
        if (other.tag.Equals("Player"))
        {
            if (this.gameObject.tag.Equals("Coin"))
            {
                GameObject.Find("Monedas").GetComponent<Text>().text = int.Parse(GameObject.Find("Monedas").GetComponent<Text>().text) + 31 + "";
            }

            if (this.gameObject.tag.Equals("Arena"))
            {
                string cantidad = GameObject.Find("Item1").transform.GetChild(0).gameObject.GetComponent<Text>().text;
                GameObject.Find("Item1").transform.GetChild(0).gameObject.GetComponent<Text>().text = int.Parse(cantidad) + 1 + "";
            }
            if (this.gameObject.tag.Equals("Botas"))
            {
                string cantidad = GameObject.Find("Item2").transform.GetChild(0).gameObject.GetComponent<Text>().text;
                GameObject.Find("Item2").transform.GetChild(0).gameObject.GetComponent<Text>().text = int.Parse(cantidad) + 1 + "";
            }
            if (this.gameObject.tag.Equals("Lupa"))
            {
                string cantidad = GameObject.Find("Item3").transform.GetChild(0).gameObject.GetComponent<Text>().text;
                GameObject.Find("Item3").transform.GetChild(0).gameObject.GetComponent<Text>().text = int.Parse(cantidad) + 1 + "";
            }
            if (this.gameObject.tag.Equals("RevientaMuros"))
            {
                string cantidad = GameObject.Find("Item4").transform.GetChild(0).gameObject.GetComponent<Text>().text;
                GameObject.Find("Item4").transform.GetChild(0).gameObject.GetComponent<Text>().text = int.Parse(cantidad) + 1 + "";
            }

            if (MainMenuController.esServer)
            {
                //GameObject.Find("Server").GetComponent<Server>().coin(GameObject.Find("Mapas").GetComponent<ElegirMapa>().indexMoneda(this.gameObject));
            }
            else
            {
                //GameObject.Find("Cliente").GetComponent<NetworkClient>()
            }

            this.gameObject.SetActive(false);
        }
    }
}
