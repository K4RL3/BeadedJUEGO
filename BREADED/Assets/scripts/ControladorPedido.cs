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

    void Start()
    {
        if (PedidoMemoria.NPCLlegoALaCaja)
        {
            if (puntoLlegada != null) transform.position = PedidoMemoria.PosicionNPCGuardada;

            // Si ya se tomó el pedido (revisando la lista de atendidos), destruimos la UI
            if (PedidoMemoria.NpcsAtendidos.Contains(gameObject.name))
            {
                if (burbujaDialogo != null) Destroy(burbujaDialogo);
                if (botonSiguiente != null) Destroy(botonSiguiente);
            }
            else
            {
                if (burbujaDialogo != null) burbujaDialogo.SetActive(true);
                if (botonSiguiente != null) botonSiguiente.SetActive(true);
            }
        }
        else
        {
            if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
            if (botonSiguiente != null) botonSiguiente.SetActive(false);
        }
    }

    void Update()
    {
        if (!PedidoMemoria.NPCLlegoALaCaja)
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
                PedidoMemoria.NPCLlegoALaCaja = true;

                if (burbujaDialogo != null) burbujaDialogo.SetActive(true);
                if (botonSiguiente != null) botonSiguiente.SetActive(true);
            }
        }
    }

    public void IrAEstacionPedido2()
    {
        PedidoMemoria.PosicionNPCGuardada = transform.position;
        SceneManager.LoadScene(nombreEscenaPedido2);
    }
}