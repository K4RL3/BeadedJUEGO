using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorPedido : MonoBehaviour
{
    [Header("UI del Pedido")]
    public GameObject burbujaDialogo;
    public GameObject botonSiguiente;

    [Header("Escenas")]
    public string nombreEscenaPedido2 = "EstaciónPedidos2";

    void Start()
    {
        // 1. SI YA FUE ATENDIDO, APAGAMOS SU UI Y EL SCRIPT
        if (PedidoMemoria.NpcsAtendidos.Contains(gameObject.name))
        {
            if (burbujaDialogo != null) burbujaDialogo.SetActive(false);
            if (botonSiguiente != null) botonSiguiente.SetActive(false);
            this.enabled = false;
            return;
        }

        // 2. SI NO HA SIDO ATENDIDO, MOSTRAMOS LA UI DE INMEDIATO
        if (burbujaDialogo != null) burbujaDialogo.SetActive(true);
        if (botonSiguiente != null) botonSiguiente.SetActive(true);
    }

    public void IrAEstacionPedido2()
    {
        
        SceneManager.LoadScene(nombreEscenaPedido2);
    }
}