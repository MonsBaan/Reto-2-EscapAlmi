using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RompeMuros : MonoBehaviour
{
    public int velocidad;
    private Vector3 posInicial;
    private Transform padre;
    private float time;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        padre = this.transform.parent;
        this.transform.parent = null;
        posInicial = this.gameObject.transform.position;
        //InvokeRepeating("reiniciar", 5, 5);
        time = 5;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Translate(Vector3.forward * velocidad * Time.deltaTime);

        if (time > 0)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                reiniciar();
            }
        }
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
        this.transform.parent = padre;
        this.gameObject.transform.localRotation = Quaternion.EulerAngles(0, 0, 0);
        this.gameObject.transform.localPosition = Vector3.zero;
        this.gameObject.SetActive(false);
    }
}
