using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientePedido : MonoBehaviour
{
    [Header("Movimiento")]
    public Transform puntoLlegada;
    public float velocidad = 2f;

    [Header("UI")]
    public GameObject burbujaDialogo;
    public GameObject botonPedir;

    [Header("Datos del Pedido")]
    public Sprite[] ingredientesQueQuiere;
    public string escenaSiguiente = "EstacionArmado";

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
            // Movimiento bloqueando la gravedad en Y
            Vector3 destino = new Vector3(puntoLlegada.position.x, transform.position.y, puntoLlegada.position.z);
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidad * Time.deltaTime);

            // Detección de llegada
            if (Vector3.Distance(transform.position, destino) < 0.01f)
            {
                yaLlego = true;
                if (burbujaDialogo != null) burbujaDialogo.SetActive(true);
                if (botonPedir != null) botonPedir.SetActive(true);
            }
        }
    }

    // Función llamada desde el botón
    public void TomarPedido()
    {
        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.AgregarPedido(this.gameObject.name, ingredientesQueQuiere);
            SceneManager.LoadScene(escenaSiguiente);
        }
        else
        {
            Debug.LogError("OrderManager no encontrado en la escena.");
        }
    }
}