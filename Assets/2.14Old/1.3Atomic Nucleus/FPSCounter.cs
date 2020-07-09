using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour
{

    public int Fps { get; private set; }

    private void Update()
    {
        Fps = (int) (1 / Time.unscaledDeltaTime);
    }
}
