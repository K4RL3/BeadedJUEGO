using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class InicioForzado
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void ForzarInicio()
    {
        // Si NO estamos en el menú principal, lo cargamos
        if (SceneManager.GetActiveScene().name != "PantallaInicio")
        {
            SceneManager.LoadScene("PantallaInicio");
        }
    }
}