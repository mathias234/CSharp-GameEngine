using System.Collections.Generic;
using System.Drawing;
using BEPUutilities;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;

namespace NewEngine.Engine.Rendering {
    public class Material : MappedValues {
        public Material(Texture diffuse, float specularIntensity = 0.5f, float specularPower = 32.0f, Texture normalMap = null, Texture dispMap = null, float dispScale = 0.0f, float dispMapOffset = 0.0f) : base() {
            SetTexture("diffuse", diffuse);

            if (normalMap == null)
                SetTexture("normalMap", new Texture("default_normal.png"));
            else
                SetTexture("normalMap", normalMap);

            if (dispMap == null)
                SetTexture("dispMap", new Texture("default_disp.jpg"));
            else
                SetTexture("dispMap", dispMap);

            SetFloat("dispMapScale", dispScale);

            float baseBias = GetFloat("dispMapScale") / 2.0f;

            SetFloat("dispMapBias", -baseBias + baseBias * dispMapOffset);

            SetFloat("specularIntensity", specularIntensity);
            SetFloat("specularPower", specularPower);
        }
    }
}
