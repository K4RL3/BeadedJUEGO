using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorPedido : MonoBehaviour
{
    [Header("Movimiento del NPC")]
    public Transform puntoLlegada;
    public float velocidadCaminata = 2f;

    [Header("UI del Pedido")]
    public GameObject burbujaDialogo;
    public GameObject botonSiguiente;

    [Header("Escenas")]
    public string nombreEscenaPedido2 = "EstaciónPedidos2";

    bool yaLlego = false;
    bool yaFueAtendido = false; // Bandera para saber si ya pidió

    void Start()
    {
        // Revisamos si este NPC ya fue atendido antes
        if (PedidoMemoria.NpcsAtendidos.Contains(gameObject.name))
        {
            yaFueAtendido = true; // Bloqueamos la lógica
            if (puntoLlegada != null) transform.position = puntoLlegada.position;
            yaLlego = true;

            if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
            if (botonSiguiente != null) botonSiguiente.SetActive(false);
            return;
        }

        if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
        if (botonSiguiente != null) botonSiguiente.SetActive(false);
    }

    void Update()
    {
        // Si ya fue atendido, el Update no hace absolutamente nada
        if (yaFueAtendido) return;

        if (!yaLlego)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                puntoLlegada.position,
                velocidadCaminata * Time.deltaTime
            );

            float distancia = Vector3.Distance(transform.position, puntoLlegada.position);

            if (distancia < 0.01f)
            {
                transform.position = puntoLlegada.position;
                yaLlego = true;

                if (burbujaDialogo != null) burbujaDialogo.SetActive(true);
                if (botonSiguiente != null) botonSiguiente.SetActive(true);
            }
        }
    }

    public void IrAEstacionPedido2()
    {
        SceneManager.LoadScene(nombreEscenaPedido2);
    }
}