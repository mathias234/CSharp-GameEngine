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

        private Dictionary<string, Dictionary<string, ShaderResource>> _shaderMap = new Dictionary<string, Dictionary<string, ShaderResource>>();
        private string _filename;

        public Shader(string filename) {
            _filename = filename;

           var shaderPath = Path.Combine("./res/shaders", filename);


            if (File.Exists(shaderPath)) {
                // Load shader pack

                LoadShaderPackage(filename);
            }
        }

        // TODO: fix this
        ~Shader() {
            //LogManager.Debug("removing shader : " + _filename);
            //if (_resource.RemoveReference() && _filename != null) {
            //    _loadedShaders.Remove(_filename);
            //}
        }

        private List<string> shaderTypes;


        public List<string> GetShaderTypes
        {
            get
            {
                if (shaderTypes == null) {
                    shaderTypes = new List<string>();
                    foreach (var shaderResource in _shaderMap[_filename]) {
                        shaderTypes.Add(shaderResource.Key);
                    }
                }

                return shaderTypes;
            }
        }

        public void Bind(string pass = "") {
            if (_shaderMap[_filename].ContainsKey(pass))
                GL.UseProgram(_shaderMap[_filename][pass].Program);
            else {
                LogManager.Debug("could not find the pass " + pass + " in shader" + _filename);
            }
        }

        private void LoadShaderPackage(string filename) {
            try {
                var reader = new StreamReader(Path.Combine("./res/shaders", filename));

                string line;
                while ((line = reader.ReadLine()) != null) {
                    if (!line.StartsWith("pass"))
                        return;

                    var lineTokens = line.Split(' ');

                    var shaderType = lineTokens[1];
                    var shaderName = lineTokens[2];

                    ShaderResource resource;

                    if (_loadedShaders.ContainsKey(shaderName)) {
                        resource = _loadedShaders[shaderName];
                    }
                    else {
                        resource = new ShaderResource(shaderName);
                        _loadedShaders.Add(shaderName, resource);
                    }

                    if (_shaderMap.ContainsKey(filename)) {
                        _shaderMap[filename].Add(shaderType, resource);
                    }
                    else {
                        var tempMap = new Dictionary<string, ShaderResource>();
                        tempMap.Add(shaderType, resource);
                        _shaderMap.Add(filename, tempMap);
                    }
                }
            }
            catch (Exception e) {
                LogManager.Error(e.Message + e.StackTrace);
            }
        }

        public virtual void UpdateUniforms(Transform transform, Material material, RenderingEngine renderingEngine, string pass) {
            if (renderingEngine.MainCamera == null) {
                LogManager.Debug("No Camera");
                return;
            }

            ShaderResource resource = GetResourceFromPass(pass);

            var modelMatrix = transform.GetTransformation();

            var vpMatrix = renderingEngine.MainCamera.GetViewProjection();

            var mvpMatrix = modelMatrix * vpMatrix;

            var orthoMatrix = renderingEngine.MainCamera.GetOrtographicProjection();

            var cameraMatrix = renderingEngine.MainCamera.Transform.GetTransformationNoRot();
            var cameraPositionMatrix = cameraMatrix * renderingEngine.MainCamera.GetViewProjection();

            for (var i = 0; i < resource.UniformNames.Count; i++) {
                var uniformName = resource.UniformNames[i];
                var uniformType = resource.UniformTypes[i];

                if (uniformName.StartsWith("R_")) {
                    var unprefixedUniformName = uniformName.Substring(2);
                    if (unprefixedUniformName == "lightMatrix") {
                        SetUniform(uniformName, modelMatrix * renderingEngine.LightMatrix, pass);
                    }
                    else if (uniformType == "sampler2D") {
                        var samplerSlot = renderingEngine.GetSamplerSlot(unprefixedUniformName);
                        renderingEngine.GetTexture(unprefixedUniformName).Bind(samplerSlot, TextureTarget.Texture2D);
                        SetUniform(uniformName, samplerSlot, pass);
                    }
                    else if (uniformType == "samplerCube") {
                        var samplerSlot = renderingEngine.GetSamplerSlot(unprefixedUniformName);
                        renderingEngine.GetTexture(unprefixedUniformName)
                            .Bind(samplerSlot, TextureTarget.TextureCubeMap);
                        SetUniform(uniformName, samplerSlot, pass);
                    }
                    else if (uniformType == "vec3")
                        SetUniform(uniformName, renderingEngine.GetVector3(unprefixedUniformName), pass);
                    else if (uniformType == "vec4")
                        SetUniform(uniformName, renderingEngine.GetVector4(unprefixedUniformName), pass);
                    else if (uniformType == "float")
                        SetUniform(uniformName, renderingEngine.GetFloat(unprefixedUniformName), pass);
                    else if (uniformType == "DirectionalLight")
                        SetUniformDirectionalLight(uniformName, (DirectionalLight)renderingEngine.ActiveLight, pass);
                    else if (uniformType == "PointLight")
                        SetUniformPointLight(uniformName, (PointLight)renderingEngine.ActiveLight, pass);
                    else if (uniformType == "SpotLight")
                        SetUniformSpotLight(uniformName, (SpotLight)renderingEngine.ActiveLight, pass);
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
                        SetUniform(uniformName, samplerSlot, pass);
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
                        SetUniform(uniformName, samplerSlot, pass);
                    }
                }
                else if (uniformName.StartsWith("T_")) {
                    if (uniformName == "T_MVP")
                        SetUniform(uniformName, mvpMatrix, pass);
                    else if (uniformName == "T_VP")
                        SetUniform(uniformName, vpMatrix, pass);
                    else if (uniformName == "T_ORTHO")
                        SetUniform(uniformName, orthoMatrix, pass);
                    else if (uniformName == "T_model")
                        SetUniform(uniformName, modelMatrix, pass);
                    else if (uniformName == "T_cameraPos") {
                        SetUniform(uniformName, cameraPositionMatrix, pass);
                    }
                    else
                        LogManager.Error("Failed to update uniform: " + uniformName +
                                         ", not a valid argument of Transform");
                }

                else if (uniformName.StartsWith("C_")) {
                    if (uniformName == "C_eyePos") {
                        SetUniform(uniformName, renderingEngine.MainCamera.Transform.GetTransformedPosition(), pass);
                    }
                    else
                        LogManager.Error("Failed to update uniform: " + uniformName + ", not a valid argument of Camera");
                }
                else {
                    if (uniformType == "vec3") {
                        SetUniform(uniformName, material.GetVector3(uniformName), pass);
                    }
                    else if (uniformType == "vec4") {
                        SetUniform(uniformName, material.GetVector4(uniformName), pass);
                    }
                    else if (uniformType == "float") {
                        SetUniform(uniformName, material.GetFloat(uniformName), pass);
                    }
                    else
                        LogManager.Error("Failed to update uniform: " + uniformName + ", not a valid type in Material");
                }
            }
        }

        private ShaderResource GetResourceFromPass(string pass) {
            ShaderResource resource;

            if (_shaderMap[_filename].ContainsKey(pass))
                resource = _shaderMap[_filename][pass];
            else {
                return null;
            }

            return resource;
        }

        public void SetUniform(string uniformName, int value, string pass) {
            GL.Uniform1(GetResourceFromPass(pass).Uniforms[uniformName], value);
        }

        public void SetUniform(string uniformName, float value, string pass) {
            GL.Uniform1(GetResourceFromPass(pass).Uniforms[uniformName], value);
        }

        public void SetUniform(string uniformName, Vector3 value, string pass) {
            GL.Uniform3(GetResourceFromPass(pass).Uniforms[uniformName], value);
        }

        public void SetUniform(string uniformName, Vector4 value, string pass) {
            GL.Uniform4(GetResourceFromPass(pass).Uniforms[uniformName], value);
        }

        public void SetUniform(string uniformName, Color value, string pass) {
            GL.Uniform4(GetResourceFromPass(pass).Uniforms[uniformName], value);
        }

        public void SetUniform(string uniformName, Matrix4 value, string pass) {
            GL.UniformMatrix4(GetResourceFromPass(pass).Uniforms[uniformName], false, ref value);
        }

        public void SetUniformBaseLight(string uniformName, BaseLight baseLight, string pass) {
            SetUniform(uniformName + ".color", baseLight.Color, pass);
            SetUniform(uniformName + ".intensity", baseLight.Intensity, pass);
        }

        public void SetUniformDirectionalLight(string uniformName, DirectionalLight directionalLight, string pass) {
            SetUniformBaseLight(uniformName + ".base", directionalLight, pass);
            SetUniform(uniformName + ".direction", directionalLight.Direction, pass);
        }

        public void SetUniformPointLight(string uniformName, PointLight pointLight, string pass) {
            SetUniformBaseLight(uniformName + ".base", pointLight, pass);
            SetUniform(uniformName + ".atten.constant", pointLight.Attenuation.Constant, pass);
            SetUniform(uniformName + ".atten.linear", pointLight.Attenuation.Linear, pass);
            SetUniform(uniformName + ".atten.exponent", pointLight.Attenuation.Exponent, pass);
            SetUniform(uniformName + ".position", pointLight.Transform.GetTransformedPosition(), pass);
            SetUniform(uniformName + ".range", pointLight.Range, pass);
        }

        public void SetUniformSpotLight(string uniformName, SpotLight spotLight, string pass) {
            SetUniformPointLight(uniformName + ".pointLight", spotLight, pass);
            SetUniform(uniformName + ".direction", spotLight.Direction, pass);
            SetUniform(uniformName + ".cutoff", spotLight.Cutoff, pass);
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