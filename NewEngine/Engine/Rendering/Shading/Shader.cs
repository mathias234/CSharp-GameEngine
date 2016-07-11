using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using NewEngine.Engine.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.Shading {
    public class Shader {
        private int _program;

        private Dictionary<string, int> _uniforms;

        public Shader(string filename) {
            _program = GL.CreateProgram();
            _uniforms = new Dictionary<string, int>();

            if (_program == 0) {
                LogManager.Error("Shader creation failed: could not find valid memory location in constructor");
            }

            string vertexShaderText = LoadShader(filename + ".vs");
            string fragmentShaderText = LoadShader(filename + ".fs");

            AddVertexShader(vertexShaderText);
            AddFragmentShader(fragmentShaderText);

            CompileShader();

            AddAllUniforms(vertexShaderText);
            AddAllUniforms(fragmentShaderText);
        }


        public void Bind() {
            GL.UseProgram(_program);
        }

        public virtual void UpdateUniforms(Transform transform, Material material, RenderingEngine renderingEngine) {

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


                AddUniformWithStructCheck(uniformName, uniformType, structs);

                uniformStartLocation = shaderText.IndexOf(uniformKeyWord, uniformStartLocation + uniformKeyWord.Length, StringComparison.Ordinal);
            }
        }

        private void AddUniformWithStructCheck(string uniformName, string uniformType,
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
                    AddUniformWithStructCheck(uniformName + "." + glslStruct.name, glslStruct.type, structs);
                }
            }

            if(addThis)
                AddUniform(uniformName);

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
                if(structStartLocation == 0 &&
                    (char.IsWhiteSpace(shaderText[structStartLocation-1]) || shaderText[structStartLocation-1] == ';')
                    && char.IsWhiteSpace((shaderText[structStartLocation+structKeyword.Length])))
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


        private void AddUniform(string uniform) {
            var uniformLocation = GL.GetUniformLocation(_program, uniform);

            if (uniformLocation == -1) {
                LogManager.Error("ERROR: Could not find uniform " + uniform);
            }
            if (!_uniforms.ContainsKey(uniform))
                _uniforms.Add(uniform, uniformLocation);
            else {
                _uniforms[uniform] = uniformLocation;
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
            GL.BindAttribLocation(_program, location, attributeName);
        }

        private void CompileShader() {
            GL.LinkProgram(_program);

            int programLinkStatus;
            GL.GetProgram(_program, ProgramParameter.LinkStatus, out programLinkStatus);

            if (programLinkStatus == 0) {
                LogManager.Error(GL.GetProgramInfoLog(_program));
            }

            GL.ValidateProgram(_program);

            int programValidateStatus;
            GL.GetProgram(_program, ProgramParameter.ValidateStatus, out programValidateStatus);

            if (programValidateStatus == 0) {
                LogManager.Error(GL.GetProgramInfoLog(_program));
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

            GL.AttachShader(_program, shader);
        }

        public void SetUniform(string uniformName, int value) {
            GL.Uniform1(_uniforms[uniformName], value);
        }
        public void SetUniform(string uniformName, float value) {
            GL.Uniform1(_uniforms[uniformName], value);
        }
        public void SetUniform(string uniformName, Vector3 value) {
            GL.Uniform3(_uniforms[uniformName], value);
        }
        public void SetUniform(string uniformName, Vector4 value) {
            GL.Uniform4(_uniforms[uniformName], value);
        }
        public void SetUniform(string uniformName, Color value) {
            GL.Uniform4(_uniforms[uniformName], value);
        }
        public void SetUniform(string uniformName, Matrix4 value) {
            GL.UniformMatrix4(_uniforms[uniformName], false, ref value);
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
