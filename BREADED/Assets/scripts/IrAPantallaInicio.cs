using UnityEngine;
using UnityEngine.SceneManagement;

public class IrAPantallaInicio : MonoBehaviour
{
    [Header("Nombre exacto de la escena de inicio")]
    public string nombreEscenaInicio = "PantallaInicio";

    [Header("Opcional: Reiniciar estado de vasos al volver")]
    public bool reiniciarEstadoVasos = true;

    public void CargarPantallaInicio()
    {
        if (reiniciarEstadoVasos)
        {
            Vasos.vasoYaElegido = false;
            Vasos.eraChico = false;
        }

        Time.timeScale = 1f;

        if (ManejadorAudio.Instance != null)
        {
            ManejadorAudio.Instance.PlayBoton();
        }

        if (!string.IsNullOrWhiteSpace(nombreEscenaInicio))
        {
            SceneManager.LoadScene(nombreEscenaInicio);
        }
        else
        {
            Debug.LogWarning("IrAPantallaInicio: Falta asignar nombreEscenaInicio.");
        }
    }
}