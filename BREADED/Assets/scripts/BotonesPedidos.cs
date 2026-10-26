using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BotonesPedidos : MonoBehaviour
{
    public string nombreEscenaCocina = "EstaciónArmado";

    [Header("Textos de los Botones")]
    public TextMeshProUGUI textoBotonAtras;
    public TextMeshProUGUI textoBotonAdelante;

    void Start()
    {
        // Al entrar a AtendiendoClientes, configuramos los textos
        if (textoBotonAtras != null) textoBotonAtras.text = "Volver";
        if (textoBotonAdelante != null) textoBotonAdelante.text = "Armar";
    }

    public void BotonAtras()
    {
        Debug.Log("Ya estamos en la primera estación");
    }

    public void BotonAdelante()
    {
        NavegadorEstacion.estacionInicial = 0;
        SceneManager.LoadScene(nombreEscenaCocina);
    }
}