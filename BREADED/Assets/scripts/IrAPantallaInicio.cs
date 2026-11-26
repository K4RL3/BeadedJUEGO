using UnityEngine;
using UnityEngine.SceneManagement;

public class IrAPantallaInicio : MonoBehaviour
{
    [Header("Nombre exacto de las escenas")]
    public string nombreEscenaInicio = "PantallaInicio";
    public string nombreEscenaEstacionPedidos1 = "EstaciónPedidos1";

    [Header("Opcional: Reiniciar estado de vasos al cambiar de escena")]
    public bool reiniciarEstadoVasos = true;

    public void CargarPantallaInicio()
    {
        CargarEscena(nombreEscenaInicio, "nombreEscenaInicio");
    }

    public void CargarEstacionPedidos1()
    {
        CargarEscena(nombreEscenaEstacionPedidos1, "nombreEscenaEstacionPedidos1");
    }

    private void CargarEscena(string nombreEscena, string nombreCampoInspector)
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

        if (!string.IsNullOrWhiteSpace(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogWarning($"IrAPantallaInicio: Falta asignar {nombreCampoInspector}.");
        }
    }
}