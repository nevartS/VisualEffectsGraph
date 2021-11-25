using UnityEngine;

namespace GrassPainter
{
    public class ShaderInteractor : MonoBehaviour
    {
        private void FixedUpdate()
        {
            Shader.SetGlobalVector("_DeltaPosition", transform.position);
        }
    }
}