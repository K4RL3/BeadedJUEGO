using UnityEngine;
using TMPro;

public class BotonesPedidos : MonoBehaviour
{
    // Índice de tu escena de la cocina (según tu Build Settings)
    private int indiceEscenaCocina = 4;

    [Header("Textos de los Botones")]
    public TextMeshProUGUI textoBotonAtras;
    public TextMeshProUGUI textoBotonAdelante;

    void Start()
    {
        if (textoBotonAtras != null) textoBotonAtras.text = "Volver";
        if (textoBotonAdelante != null) textoBotonAdelante.text = "Armar";
    }

    public void BotonAtras()
    {
        Debug.Log("Ya estamos en la primera estación");
        // (Si quisieras volver al menú, usarías TransicionEscenasUI.Instance.DisolverSalida(0))
    }

    public void BotonAdelante()
    {
        // Le decimos al Navegador que abra en la estación 0 (Mesa)
        NavegadorEstacion.estacionInicial = 0;

        // Hacemos la transición hacia la cocina
        if (TransicionEscenasUI.Instance != null)
        {
            TransicionEscenasUI.Instance.DisolverSalida(indiceEscenaCocina);
        }
    }
}