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
        if (textoBotonAtras != null) textoBotonAtras.text = "";
        if (textoBotonAdelante != null) textoBotonAdelante.text = "Armar";
    }

    public void BotonAtras()
    {
        // 🎵 SONIDO DEL BOTÓN
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        Debug.Log("Ya estamos en la primera estación");
    }

    public void BotonAdelante()
    {
        // 🎵 SONIDO DEL BOTÓN
        if (ManejadorAudio.Instance != null) ManejadorAudio.Instance.PlayBoton();

        NavegadorEstacion.estacionInicial = 0;
        SceneManager.LoadScene(nombreEscenaCocina);
    }
}