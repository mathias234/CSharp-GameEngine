using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.Shading {
    public class Shader {
        private static Dictionary<string, ShaderResource> _loadedShaders = new Dictionary<string, ShaderResource>();

        private ShaderResource _resource;

        private string _filename;

        public Shader(string filename) {
            this._filename = filename;

            if (_loadedShaders.ContainsKey(filename)) {
                _resource = _loadedShaders[filename];
                _resource.AddReference();
            }
            else {
                _resource = new ShaderResource();

                string vertexShaderText = LoadShader(filename + ".vs");
                string fragmentShaderText = LoadShader(filename + ".fs");

                AddVertexShader(vertexShaderText);
                AddFragmentShader(fragmentShaderText);

                CompileShader();

                AddAllUniforms(vertexShaderText);
                AddAllUniforms(fragmentShaderText);

                _loadedShaders.Add(filename, _resource);
            }

        }

        ~Shader() {
            if (_resource.RemoveReference() && _filename != "") {
                _loadedShaders.Remove(_filename);
            }
        }

        public void Bind() {
            GL.UseProgram(_resource.Program);
        }

        public virtual void UpdateUniforms(Transform transform, Material material, RenderingEngine renderingEngine) {
            if (renderingEngine.MainCamera == null)
                return;

            Matrix4 modelMatrix = transform.GetTransformation();
            Matrix4 mvpMatrix = modelMatrix * renderingEngine.MainCamera.GetViewProjection();
            Matrix4 orthoMatrix = renderingEngine.MainCamera.GetOrtographicProjection();

            Matrix4 CameraMatrix = renderingEngine.MainCamera.Transform.GetTransformationNoRot();
            Matrix4 CameraPositionMatrix = CameraMatrix * renderingEngine.MainCamera.GetViewProjection();

            for (int i = 0; i < _resource.UniformNames.Count; i++) {
                string uniformName = _resource.UniformNames[i];
                string uniformType = _resource.UniformTypes[i];

                if (uniformName.StartsWith("R_")) {
                    string unprefixedUniformName = uniformName.Substring(2);
                    if (unprefixedUniformName == "lightMatrix") {
                        SetUniform(uniformName, modelMatrix * renderingEngine.LightMatrix);
                    }
                    else if (uniformType == "sampler2D") {
                        int samplerSlot = renderingEngine.GetSamplerSlot(unprefixedUniformName);
                        renderingEngine.GetTexture(unprefixedUniformName).Bind(samplerSlot, TextureTarget.Texture2D);
                        SetUniform(uniformName, samplerSlot);
                    }
                    else if (uniformType == "samplerCube") {
                        int samplerSlot = renderingEngine.GetSamplerSlot(unprefixedUniformName);
                        renderingEngine.GetTexture(unprefixedUniformName).Bind(samplerSlot, TextureTarget.TextureCubeMap);
                        SetUniform(uniformName, samplerSlot);
                    }
                    else if (uniformType == "vec3")
                        SetUniform(uniformName, renderingEngine.GetVector3(unprefixedUniformName));
                    else if (uniformType == "float")
                        SetUniform(uniformName, renderingEngine.GetFloat(unprefixedUniformName));
                    else if (uniformType == "DirectionalLight")
                        SetUniformDirectionalLight(uniformName, (DirectionalLight)renderingEngine.GetActiveLight);
                    else if (uniformType == "PointLight")
                        SetUniformPointLight(uniformName, (PointLight)renderingEngine.GetActiveLight);
                    else if (uniformType == "SpotLight")
                        SetUniformSpotLight(uniformName, (SpotLight)renderingEngine.GetActiveLight);
                    else
                        renderingEngine.UpdateUniformStruct(transform, material, this, uniformName, uniformType);
                }
                else if (uniformType == "sampler2D") {
                    Texture texture;

                    if (material.GetTexture(uniformName) == null) {
                        texture = new Texture("default.png");
                    }
                    else {
                        texture = material.GetTexture(uniformName);
                    }

                    int samplerSlot = renderingEngine.GetSamplerSlot(uniformName);
                    texture.Bind(samplerSlot, TextureTarget.Texture2D);
                    SetUniform(uniformName, samplerSlot);
                }
                else if (uniformType == "samplerCube") {
                    CubemapTexture texture;

                    if (material.GetCubemapTexture(uniformName) == null) {
                        texture = new CubemapTexture("default.png", "default.png", "default.png", "default.png", "default.png", "default.png");
                    }
                    else {
                        texture = material.GetCubemapTexture(uniformName);
                    }

                    int samplerSlot = renderingEngine.GetSamplerSlot(uniformName);
                    material.GetCubemapTexture(uniformName).Bind(samplerSlot);
                    SetUniform(uniformName, samplerSlot);
                }
                else if (uniformName.StartsWith("T_")) {
                    if (uniformName == "T_MVP")
                        SetUniform(uniformName, mvpMatrix);
                    else if (uniformName == "T_ORTHO")
                        SetUniform(uniformName, orthoMatrix);
                    else if (uniformName == "T_model")
                        SetUniform(uniformName, modelMatrix);
                    else if (uniformName == "T_cameraPos") {
                        SetUniform(uniformName, CameraPositionMatrix);
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
                    else if (uniformType == "float") {
                        SetUniform(uniformName, material.GetFloat(uniformName));
                    }
                    else
                        LogManager.Error("Failed to update uniform: " + uniformName + ", not a valid type in Material");
                }
            }
        }

        private void AddAllUniforms(string shaderText) {
            Dictionary<string, List<GLSLStruct>> structs = FindUniformStructs(shaderText);

            string uniformKeyWord = "uniform";
            int uniformStartLocation = shaderText.IndexOf(uniformKeyWord, StringComparison.Ordinal);
            while (uniformStartLocation != -1) {
                if (uniformStartLocation == 0 &&
                    (char.IsWhiteSpace(shaderText[uniformStartLocation - 1]) || shaderText[uniformStartLocation - 1] == ';')
                    && char.IsWhiteSpace((shaderText[uniformStartLocation + uniformKeyWord.Length])))
                    continue;

                int begin = uniformStartLocation + uniformKeyWord.Length + 1;
                int end = shaderText.IndexOf(";", begin, StringComparison.Ordinal);

                string uniformLine = shaderText.Substring(begin, end - begin + 1).Trim();

                int whiteSpacePos = uniformLine.IndexOf(' ');

                string uniformName = uniformLine.Substring(whiteSpacePos + 1, uniformLine.Length - (uniformLine.IndexOf(' ') + 2)).Trim();
                string uniformType = uniformLine.Substring(0, whiteSpacePos).Trim();

                _resource.UniformNames.Add(uniformName);
                _resource.UniformTypes.Add(uniformType);
                AddUniform(uniformName, uniformType, structs);


                uniformStartLocation = shaderText.IndexOf(uniformKeyWord, uniformStartLocation + uniformKeyWord.Length, StringComparison.Ordinal);
            }
        }

        private void AddUniform(string uniformName, string uniformType,
            Dictionary<string, List<GLSLStruct>> structs) {

            bool addThis = true;

            List<GLSLStruct> structComponents;
            if (structs.ContainsKey(uniformType))
                structComponents = structs[uniformType];
            else
                structComponents = null;

            if (structComponents != null) {
                addThis = false;

                foreach (var glslStruct in structComponents) {
                    AddUniform(uniformName + "." + glslStruct.name, glslStruct.type, structs);
                }
            }

            if (!addThis)
                return;

            var uniformLocation = GL.GetUniformLocation(_resource.Program, uniformName);

            if (uniformLocation == -1) {
                LogManager.Error("ERROR: Could not find uniform " + uniformName);
            }
            if (!_resource.Uniforms.ContainsKey(uniformName))
                _resource.Uniforms.Add(uniformName, uniformLocation);
            else {
                _resource.Uniforms[uniformName] = uniformLocation;
            }


        }

        private class GLSLStruct {
            public string name;
            public string type;
        }

        private Dictionary<string, List<GLSLStruct>> FindUniformStructs(string shaderText) {
            Dictionary<string, List<GLSLStruct>> result = new Dictionary<string, List<GLSLStruct>>();

            string structKeyword = "struct";
            int structStartLocation = shaderText.IndexOf(structKeyword, StringComparison.Ordinal);
            while (structStartLocation != -1) {
                if (structStartLocation == 0 &&
                    (char.IsWhiteSpace(shaderText[structStartLocation - 1]) || shaderText[structStartLocation - 1] == ';')
                    && char.IsWhiteSpace((shaderText[structStartLocation + structKeyword.Length])))
                    continue;

                int nameBegin = structStartLocation + structKeyword.Length + 1;
                int braceBegin = shaderText.IndexOf("{", nameBegin, StringComparison.Ordinal);
                int braceEnd = shaderText.IndexOf("}", braceBegin, StringComparison.Ordinal);

                //int end = shaderText.IndexOf(";", begin, StringComparison.Ordinal);


                string structName = shaderText.Substring(nameBegin, braceBegin - nameBegin).Trim();
                List<GLSLStruct> structComponents = new List<GLSLStruct>();

                int componentSemicolonPos = shaderText.IndexOf(";", braceBegin, StringComparison.Ordinal);
                while (componentSemicolonPos != -1 && componentSemicolonPos < braceEnd) {
                    int componentNameStart = componentSemicolonPos;

                    while (!char.IsWhiteSpace(shaderText[componentNameStart - 1]))
                        componentNameStart--;

                    int componentTypeEnd = componentNameStart - 1;

                    int componentTypeStart = componentTypeEnd;

                    while (!char.IsWhiteSpace(shaderText[componentTypeStart - 1]))
                        componentTypeStart--;

                    string componentName = shaderText.Substring(componentNameStart,
                        componentSemicolonPos - componentNameStart);

                    string componentType = shaderText.Substring(componentTypeStart,
                        componentTypeEnd - componentTypeStart);


                    GLSLStruct glslStruct = new GLSLStruct();
                    glslStruct.name = componentName;
                    glslStruct.type = componentType;
                    structComponents.Add(glslStruct);

                    componentSemicolonPos = shaderText.IndexOf(";",
                        componentSemicolonPos + 1, StringComparison.Ordinal);
                }

                if (!result.ContainsKey(structName))
                    result.Add(structName, structComponents);
                else {
                    result[structName] = structComponents;
                }

                structStartLocation = shaderText.IndexOf(structKeyword, structStartLocation + structKeyword.Length, StringComparison.Ordinal);
            }
            return result;
        }

        private void AddAllAttributes(string shaderText) {
            string attributeKeyword = "attribute";
            int attributeStartPosition = shaderText.IndexOf(attributeKeyword, StringComparison.Ordinal);
            int attribNumber = 0;
            while (attributeStartPosition != -1) {
                if (attributeStartPosition == 0 &&
                    (char.IsWhiteSpace(shaderText[attributeStartPosition - 1]) || shaderText[attributeStartPosition - 1] == ';')
                    && char.IsWhiteSpace((shaderText[attributeStartPosition + attributeKeyword.Length])))
                    continue;


                int begin = attributeStartPosition + attributeKeyword.Length + 1;
                int end = shaderText.IndexOf(";", begin, StringComparison.Ordinal);

                string attributeLine = shaderText.Substring(begin, end - begin + 1).Trim();

                string attributeName = attributeLine.Substring(attributeLine.IndexOf(' ') + 1, attributeLine.Length - (attributeLine.IndexOf(' ') + 2)).Trim();

                SetAttribLocation(attributeName, attribNumber);
                attribNumber++;

                attributeStartPosition = shaderText.IndexOf(attributeKeyword, attributeStartPosition + attributeKeyword.Length, StringComparison.Ordinal);
            }
        }



        private void AddVertexShader(string text) {
            AddProgram(text, ShaderType.VertexShader);
        }

        private void AddFragmentShader(string text) {
            AddProgram(text, ShaderType.FragmentShader);
        }

        private void AddGeometryShader(string text) {
            AddProgram(text, ShaderType.GeometryShader);
        }


        private void SetAttribLocation(string attributeName, int location) {
            GL.BindAttribLocation(_resource.Program, location, attributeName);
        }

        private void CompileShader() {
            GL.LinkProgram(_resource.Program);

            int programLinkStatus;
            GL.GetProgram(_resource.Program, ProgramParameter.LinkStatus, out programLinkStatus);

            if (programLinkStatus == 0) {
                LogManager.Error(GL.GetProgramInfoLog(_resource.Program));
            }

            GL.ValidateProgram(_resource.Program);

            int programValidateStatus;
            GL.GetProgram(_resource.Program, ProgramParameter.ValidateStatus, out programValidateStatus);

            if (programValidateStatus == 0) {
                LogManager.Error(GL.GetProgramInfoLog(_resource.Program));
            }

        }

        private void AddProgram(string text, ShaderType type) {
            var shader = GL.CreateShader(type);

            if (shader == 0) {
                LogManager.Error("Shader creation failed: could not find valid memory location when adding shader");
            }

            GL.ShaderSource(shader, text);
            GL.CompileShader(shader);

            int compileStatus;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out compileStatus);

            if (compileStatus == 0) {
                LogManager.Error(GL.GetShaderInfoLog(shader));
            }

            GL.AttachShader(_resource.Program, shader);
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

        private static string LoadShader(string filename) {
            StringBuilder shaderSource = new StringBuilder();
            string includeDirective = "#include";

            StreamReader reader;

            try {
                reader = new StreamReader(Path.Combine("./res/shaders", filename));

                string line;
                while ((line = reader.ReadLine()) != null) {
                    if (line.StartsWith(includeDirective)) {
                        shaderSource.Append(LoadShader(line.Substring(0, line.Length - 1).Remove(0, includeDirective.Length + 2)));
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
