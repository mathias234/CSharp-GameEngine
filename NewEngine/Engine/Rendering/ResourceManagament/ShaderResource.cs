using System;
using System.Collections.Generic;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.Shading;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.ResourceManagament {
    public class ShaderResource {
        private int _refCount;
        private List<int> _shaders;

        public int Program { get; }

        public Dictionary<string, int> Uniforms { get; set; }

        public List<string> UniformNames { get; set; }

        public List<string> UniformTypes { get; set; }


        public ShaderResource(string filename) {
            Uniforms = new Dictionary<string, int>();
            UniformNames = new List<string>();
            UniformTypes = new List<string>();
            _shaders = new List<int>();
            Program = GL.CreateProgram();


            var vertexShaderText = Shader.LoadShader(filename + ".vs");
            var fragmentShaderText = Shader.LoadShader(filename + ".fs");

            AddVertexShader(vertexShaderText);
            AddFragmentShader(fragmentShaderText);

            AddAllAttributes(vertexShaderText);
            AddAllAttributes(fragmentShaderText);

            CompileShader();

            AddAllUniforms(vertexShaderText);
            AddAllUniforms(fragmentShaderText);


            if (Program == 0) {
                LogManager.Error("Shader creation failed: could not find valid memory location in constructor");
            }
        }

        private void AddAllAttributes(string shaderText) {
            var attributeKeyword = "attribute";
            var attributeStartPosition = shaderText.IndexOf(attributeKeyword, StringComparison.Ordinal);
            var attribNumber = 0;
            while (attributeStartPosition != -1) {
                if (attributeStartPosition == 0 &&
                    (Char.IsWhiteSpace(shaderText[attributeStartPosition - 1]) ||
                     shaderText[attributeStartPosition - 1] == ';')
                    && Char.IsWhiteSpace(shaderText[attributeStartPosition + attributeKeyword.Length]))
                    continue;


                var begin = attributeStartPosition + attributeKeyword.Length + 1;
                var end = shaderText.IndexOf(";", begin, StringComparison.Ordinal);

                var attributeLine = shaderText.Substring(begin, end - begin + 1).Trim();

                var attributeName =
                    attributeLine.Substring(attributeLine.IndexOf(' ') + 1,
                        attributeLine.Length - (attributeLine.IndexOf(' ') + 2)).Trim();

                SetAttribLocation(attributeName, attribNumber);
                attribNumber++;

                attributeStartPosition = shaderText.IndexOf(attributeKeyword,
                    attributeStartPosition + attributeKeyword.Length, StringComparison.Ordinal);
            }
        }

        private void SetAttribLocation(string attributeName, int location) {
            GL.BindAttribLocation(Program, location, attributeName);
        }


        private void AddAllUniforms(string shaderText) {
            var structs = FindUniformStructs(shaderText);

            var uniformKeyWord = "uniform";
            var uniformStartLocation = shaderText.IndexOf(uniformKeyWord, StringComparison.Ordinal);
            while (uniformStartLocation != -1) {
                if (uniformStartLocation == 0 &&
                    (Char.IsWhiteSpace(shaderText[uniformStartLocation - 1]) ||
                     shaderText[uniformStartLocation - 1] == ';')
                    && Char.IsWhiteSpace(shaderText[uniformStartLocation + uniformKeyWord.Length]))
                    continue;

                var begin = uniformStartLocation + uniformKeyWord.Length + 1;
                var end = shaderText.IndexOf(";", begin, StringComparison.Ordinal);

                var uniformLine = shaderText.Substring(begin, end - begin + 1).Trim();

                var whiteSpacePos = uniformLine.IndexOf(' ');

                var uniformName =
                    uniformLine.Substring(whiteSpacePos + 1, uniformLine.Length - (uniformLine.IndexOf(' ') + 2)).Trim();
                var uniformType = uniformLine.Substring(0, whiteSpacePos).Trim();

                UniformNames.Add(uniformName);
                UniformTypes.Add(uniformType);
                AddUniform(uniformName, uniformType, structs);


                uniformStartLocation = shaderText.IndexOf(uniformKeyWord, uniformStartLocation + uniformKeyWord.Length,
                    StringComparison.Ordinal);
            }
        }

        private void AddUniform(string uniformName, string uniformType,
            Dictionary<string, List<GlslStruct>> structs) {
            var addThis = true;

            var structComponents = structs.ContainsKey(uniformType) ? structs[uniformType] : null;

            if (structComponents != null) {
                addThis = false;

                foreach (var glslStruct in structComponents) {
                    AddUniform(uniformName + "." + glslStruct.Name, glslStruct.type, structs);
                }
            }

            if (!addThis)
                return;

            var uniformLocation = GL.GetUniformLocation(Program, uniformName);

            if (uniformLocation == -1) {
                LogManager.Error("ERROR: Could not find uniform " + uniformName + " of type: " + uniformType);
            }

            if (!Uniforms.ContainsKey(uniformName))
                Uniforms.Add(uniformName, uniformLocation);
            else {
                Uniforms[uniformName] = uniformLocation;
            }
        }

        private Dictionary<string, List<GlslStruct>> FindUniformStructs(string shaderText) {
            var result = new Dictionary<string, List<GlslStruct>>();

            var structKeyword = "struct";
            var structStartLocation = shaderText.IndexOf(structKeyword, StringComparison.Ordinal);
            while (structStartLocation != -1) {
                if (structStartLocation == 0 &&
                    (Char.IsWhiteSpace(shaderText[structStartLocation - 1]) ||
                     shaderText[structStartLocation - 1] == ';')
                    && Char.IsWhiteSpace(shaderText[structStartLocation + structKeyword.Length]))
                    continue;

                var nameBegin = structStartLocation + structKeyword.Length + 1;
                var braceBegin = shaderText.IndexOf("{", nameBegin, StringComparison.Ordinal);
                var braceEnd = shaderText.IndexOf("}", braceBegin, StringComparison.Ordinal);

                //int end = shaderText.IndexOf(";", begin, StringComparison.Ordinal);


                var structName = shaderText.Substring(nameBegin, braceBegin - nameBegin).Trim();
                var structComponents = new List<GlslStruct>();

                var componentSemicolonPos = shaderText.IndexOf(";", braceBegin, StringComparison.Ordinal);
                while (componentSemicolonPos != -1 && componentSemicolonPos < braceEnd) {
                    var componentNameStart = componentSemicolonPos;

                    while (!Char.IsWhiteSpace(shaderText[componentNameStart - 1]))
                        componentNameStart--;

                    var componentTypeEnd = componentNameStart - 1;

                    var componentTypeStart = componentTypeEnd;

                    while (!Char.IsWhiteSpace(shaderText[componentTypeStart - 1]))
                        componentTypeStart--;

                    var componentName = shaderText.Substring(componentNameStart,
                        componentSemicolonPos - componentNameStart);

                    var componentType = shaderText.Substring(componentTypeStart,
                        componentTypeEnd - componentTypeStart);


                    var glslStruct = new GlslStruct();
                    glslStruct.Name = componentName;
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

                structStartLocation = shaderText.IndexOf(structKeyword, structStartLocation + structKeyword.Length,
                    StringComparison.Ordinal);
            }
            return result;
        }




        private void CompileShader() {
            GL.LinkProgram(Program);

            int programLinkStatus;
            GL.GetProgram(Program, ProgramParameter.LinkStatus, out programLinkStatus);

            if (programLinkStatus == 0) {
                LogManager.Error(GL.GetProgramInfoLog(Program));
            }

            GL.ValidateProgram(Program);

            int programValidateStatus;
            GL.GetProgram(Program, ProgramParameter.ValidateStatus, out programValidateStatus);

            if (programValidateStatus == 0) {
                LogManager.Error(GL.GetProgramInfoLog(Program));
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

            GL.AttachShader(Program, shader);
            _shaders.Add(shader);
        }

        public void AddReference() {
            _refCount++;
        }

        public bool RemoveReference() {
            _refCount--;
            return _refCount == 0;
        }


        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                foreach (var shader in _shaders) {
                    GL.DetachShader(Program, shader);
                    GL.DeleteShader(shader);
                }
                GL.DeleteProgram(Program);
            }
        }

        private class GlslStruct {
            public string Name;
            public string type;
        }
    }
}