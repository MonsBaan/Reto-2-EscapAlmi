using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RompeMuros : MonoBehaviour
{
    public int velocidad;
    private Vector3 posInicial;

    // Start is called before the first frame update
    void Start()
    {
        posInicial = this.gameObject.transform.position;
        InvokeRepeating("reiniciar", 5, 5);
        velocidad = 10;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Muro"))
        {
            other.gameObject.SetActive(false);
            reiniciar();
        }
    }

    public void reiniciar()
    {
        this.gameObject.SetActive(false);
        this.gameObject.transform.position = posInicial;
    }
}
