using UnityEngine;
using UnityEngine.SceneManagement;

public class ClienteCaminar : MonoBehaviour
{
    public Transform puntoLlegada;
    public float velocidad = 2f;

    public GameObject burbujaDialogo;
    public GameObject botonPedir;
    public string escenaPedido2 = "EstaciónPedidos2";

    bool yaLlego = false;

    void Start()
    {
        if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
        if (botonPedir != null) botonPedir.SetActive(false);
    }

    void Update()
    {
        if (!yaLlego)
        {
            Vector3 destino = new Vector3(puntoLlegada.position.x, transform.position.y, puntoLlegada.position.z);
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);

            if (Vector3.Distance(transform.position, destino) < 0.01f)
            {
                yaLlego = true;
                if (burbujaDialogo != null) burbujaDialogo.SetActive(true);
                if (botonPedir != null) botonPedir.SetActive(true);
            }
        }
    }

    public void TomarPedido()
    {
        SceneManager.LoadScene(escenaPedido2);
    }
}