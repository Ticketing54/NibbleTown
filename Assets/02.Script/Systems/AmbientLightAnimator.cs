using UnityEngine;

public class AmbientLightAnimator : MonoBehaviour
{
    [SerializeField] private Color ambientColor = new Color(0.5f, 0.5f, 0.5f);

    private void Update()
    {
        RenderSettings.ambientLight = ambientColor;
    }
}
