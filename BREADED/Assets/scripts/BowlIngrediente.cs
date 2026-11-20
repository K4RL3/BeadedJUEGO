using UnityEngine;

public class BowlIngrediente : MonoBehaviour
{
    public GameObject prefabFruta;
    public bool ponerSoloUno = false;
    private SpawnerIngredientes spawner;

    void Start()
    {
        spawner = FindAnyObjectByType<SpawnerIngredientes>();
    }

    void OnMouseDown()
    {
        if (spawner != null && prefabFruta != null)
        {
            spawner.PrepararBowl(prefabFruta);
        }
    }

    void OnMouseUp()
    {
        if (spawner != null)
        {
            if (ponerSoloUno)
                spawner.DispararUnoSolo();
            else
                spawner.DispararLluviaDeFrutas();
        }
    }
}