using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Navegador2 : MonoBehaviour
{
    public string nombreEscenaCocina = "EstaciónArmado";

    [Header("Textos de los Botones")]
    public TextMeshProUGUI textoBotonAtras;
    public TextMeshProUGUI textoBotonAdelante;

    void Start()
    {
        // Al entrar a AtendiendoClientes, configuramos los textos
        if (textoBotonAtras != null) textoBotonAtras.text = "Volver";
        if (textoBotonAdelante != null) textoBotonAdelante.text = "Armado";
    }

    public void BotonAtras()
    {
        Debug.Log("Ya estamos en la primera estación (Atendiendo Clientes)");
    }

    public void BotonAdelante()
    {
        NavegadorEstacion.estacionInicial = 0;
        SceneManager.LoadScene(nombreEscenaCocina);
    }
}