using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopRoom : MonoBehaviour
{
    public ShaderFloatLerper effect;

    public void FadeEmissive()
    {
        effect.StartEffect();
    }
}
