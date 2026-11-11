using UnityEngine;

public class BowlIngrediente : MonoBehaviour
{
    public GameObject prefabFruta;
    private SpawnerIngredientes spawner;

    void Start()
    {
        spawner = FindAnyObjectByType<SpawnerIngredientes>();
    }

    void OnMouseDown()
    {
        // Al presionar, solo "preparamos" la fruta
        if (spawner != null && prefabFruta != null)
        {
            spawner.PrepararBowl(prefabFruta);
        }
    }

    void OnMouseUp()
    {
        // Al soltar, disparamos la lluvia de frutas
        if (spawner != null)
        {
            spawner.DispararLluviaDeFrutas();
        }
    }
}