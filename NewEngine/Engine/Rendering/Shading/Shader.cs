using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.Shading {
    public class Shader {
        private static Dictionary<string, ShaderResource> _loadedShaders = new Dictionary<string, ShaderResource>();

        private string _filename;

        private ShaderResource _resource;

        public Shader(string filename) {
            _filename = filename;

            if (_loadedShaders.ContainsKey(filename)) {
                _resource = _loadedShaders[filename];
                _resource.AddReference();
            }
            else {
                _resource = new ShaderResource(filename);
                _loadedShaders.Add(filename, _resource);
            }
        }

        ~Shader() {
            LogManager.Debug("removing shader : " + _filename);
            if (_resource.RemoveReference() && _filename != null) {
                _loadedShaders.Remove(_filename);
            }
        }

        public void Bind() {
            GL.UseProgram(_resource.Program);
        }

        public virtual void UpdateUniforms(Transform transform, Material material, RenderingEngine renderingEngine) {
            if (renderingEngine.MainCamera == null) {
                LogManager.Debug("No Camera");
                return;
            }


            var modelMatrix = transform.GetTransformation();

            var vpMatrix = renderingEngine.MainCamera.GetViewProjection();

            var mvpMatrix = modelMatrix * vpMatrix;

            var orthoMatrix = renderingEngine.MainCamera.GetOrtographicProjection();

            var cameraMatrix = renderingEngine.MainCamera.Transform.GetTransformationNoRot();
            var cameraPositionMatrix = cameraMatrix * renderingEngine.MainCamera.GetViewProjection();

            for (var i = 0; i < _resource.UniformNames.Count; i++) {
                var uniformName = _resource.UniformNames[i];
                var uniformType = _resource.UniformTypes[i];

                if (uniformName.StartsWith("R_")) {
                    var unprefixedUniformName = uniformName.Substring(2);
                    if (unprefixedUniformName == "lightMatrix") {
                        SetUniform(uniformName, modelMatrix * renderingEngine.LightMatrix);
                    }
                    else if (uniformType == "sampler2D") {
                        var samplerSlot = renderingEngine.GetSamplerSlot(unprefixedUniformName);
                        renderingEngine.GetTexture(unprefixedUniformName).Bind(samplerSlot, TextureTarget.Texture2D);
                        SetUniform(uniformName, samplerSlot);
                    }
                    else if (uniformType == "samplerCube") {
                        var samplerSlot = renderingEngine.GetSamplerSlot(unprefixedUniformName);
                        renderingEngine.GetTexture(unprefixedUniformName)
                            .Bind(samplerSlot, TextureTarget.TextureCubeMap);
                        SetUniform(uniformName, samplerSlot);
                    }
                    else if (uniformType == "vec3")
                        SetUniform(uniformName, renderingEngine.GetVector3(unprefixedUniformName));
                    else if(uniformType == "vec4")
                        SetUniform(uniformName, renderingEngine.GetVector4(unprefixedUniformName));
                    else if (uniformType == "float")
                        SetUniform(uniformName, renderingEngine.GetFloat(unprefixedUniformName));
                    else if (uniformType == "DirectionalLight")
                        SetUniformDirectionalLight(uniformName, (DirectionalLight)renderingEngine.ActiveLight);
                    else if (uniformType == "PointLight")
                        SetUniformPointLight(uniformName, (PointLight)renderingEngine.ActiveLight);
                    else if (uniformType == "SpotLight")
                        SetUniformSpotLight(uniformName, (SpotLight)renderingEngine.ActiveLight);
                    else
                        renderingEngine.UpdateUniformStruct(transform, material, this, uniformName, uniformType);
                }
                else if (uniformType == "sampler2D") {
                    if (material.GetTexture(uniformName) == null) {
                        LogManager.Error("texture does not exist");
                    }
                    else {
                        var texture = material.GetTexture(uniformName);
                        var samplerSlot = renderingEngine.GetSamplerSlot(uniformName);
                        texture.Bind(samplerSlot, TextureTarget.Texture2D);
                        SetUniform(uniformName, samplerSlot);
                    }
                }
                else if (uniformType == "samplerCube") {
                    if (material.GetCubemapTexture(uniformName) == null) {
                        LogManager.Error("cubemap texture does not exist");
                    }
                    else {
                        var texture = material.GetCubemapTexture(uniformName);

                        var samplerSlot = renderingEngine.GetSamplerSlot(uniformName);
                        texture.Bind(samplerSlot);
                        SetUniform(uniformName, samplerSlot);
                    }
                }
                else if (uniformName.StartsWith("T_")) {
                    if (uniformName == "T_MVP")
                        SetUniform(uniformName, mvpMatrix);
                    else if (uniformName == "T_VP")
                        SetUniform(uniformName, vpMatrix);
                    else if (uniformName == "T_ORTHO")
                        SetUniform(uniformName, orthoMatrix);
                    else if (uniformName == "T_model")
                        SetUniform(uniformName, modelMatrix);
                    else if (uniformName == "T_cameraPos") {
                        SetUniform(uniformName, cameraPositionMatrix);
                    }
                    else
                        LogManager.Error("Failed to update uniform: " + uniformName +
                                         ", not a valid argument of Transform");
                }

                else if (uniformName.StartsWith("C_")) {
                    if (uniformName == "C_eyePos") {
                        SetUniform(uniformName, renderingEngine.MainCamera.Transform.GetTransformedPosition());
                    }
                    else
                        LogManager.Error("Failed to update uniform: " + uniformName + ", not a valid argument of Camera");
                }
                else {
                    if (uniformType == "vec3") {
                        SetUniform(uniformName, material.GetVector3(uniformName));
                    }
                    else if (uniformType == "vec4") {
                        SetUniform(uniformName, material.GetVector4(uniformName));
                    }
                    else if (uniformType == "float") {
                        SetUniform(uniformName, material.GetFloat(uniformName));
                    }
                    else
                        LogManager.Error("Failed to update uniform: " + uniformName + ", not a valid type in Material");
                }
            }
        }

        public void SetUniform(string uniformName, int value) {
            GL.Uniform1(_resource.Uniforms[uniformName], value);
        }

        public void SetUniform(string uniformName, float value) {
            GL.Uniform1(_resource.Uniforms[uniformName], value);
        }

        public void SetUniform(string uniformName, Vector3 value) {
            GL.Uniform3(_resource.Uniforms[uniformName], value);
        }

        public void SetUniform(string uniformName, Vector4 value) {
            GL.Uniform4(_resource.Uniforms[uniformName], value);
        }

        public void SetUniform(string uniformName, Color value) {
            GL.Uniform4(_resource.Uniforms[uniformName], value);
        }

        public void SetUniform(string uniformName, Matrix4 value) {
            GL.UniformMatrix4(_resource.Uniforms[uniformName], false, ref value);
        }

        public void SetUniformBaseLight(string uniformName, BaseLight baseLight) {
            SetUniform(uniformName + ".color", baseLight.Color);
            SetUniform(uniformName + ".intensity", baseLight.Intensity);
        }

        public void SetUniformDirectionalLight(string uniformName, DirectionalLight directionalLight) {
            SetUniformBaseLight(uniformName + ".base", directionalLight);
            SetUniform(uniformName + ".direction", directionalLight.Direction);
        }

        public void SetUniformPointLight(string uniformName, PointLight pointLight) {
            SetUniformBaseLight(uniformName + ".base", pointLight);
            SetUniform(uniformName + ".atten.constant", pointLight.Attenuation.Constant);
            SetUniform(uniformName + ".atten.linear", pointLight.Attenuation.Linear);
            SetUniform(uniformName + ".atten.exponent", pointLight.Attenuation.Exponent);
            SetUniform(uniformName + ".position", pointLight.Transform.GetTransformedPosition());
            SetUniform(uniformName + ".range", pointLight.Range);
        }

        public void SetUniformSpotLight(string uniformName, SpotLight spotLight) {
            SetUniformPointLight(uniformName + ".pointLight", spotLight);
            SetUniform(uniformName + ".direction", spotLight.Direction);
            SetUniform(uniformName + ".cutoff", spotLight.Cutoff);
        }

        public static string LoadShader(string filename) {
            var shaderSource = new StringBuilder();
            var includeDirective = "#include";
            try {
                var reader = new StreamReader(Path.Combine("./res/shaders", filename));

                string line;
                while ((line = reader.ReadLine()) != null) {
                    if (line.StartsWith(includeDirective)) {
                        var match = Regex.Match(line, @"\""([^""]*)\""");

                        shaderSource.Append(LoadShader(match.Groups[1].Value));
                    }
                    else {
                        shaderSource.Append(line).Append("\n");
                    }
                }
            }
            catch (Exception e) {
                LogManager.Error(e.Message + e.StackTrace);
            }

            return shaderSource.ToString();
        }
    }
}