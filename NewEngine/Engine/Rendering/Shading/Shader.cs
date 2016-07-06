using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NewEngine.Engine.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering.Shading {
    public class Shader {
        private RenderingEngine _renderingEngine;
        private int _program;

        private Dictionary<string, int> _uniforms;

        public Shader() {
            _program = GL.CreateProgram();
            _uniforms = new Dictionary<string, int>();

            if (_program == 0) {
                LogManager.Error("Shader creation failed: could not find valid memory location in constructor");
            }
        }


        public void Bind() {
            GL.UseProgram(_program);
        }

        public virtual void UpdateUniforms(Transform transform, Material material) {
            
        }

        public void AddUniform(string uniform) {
            var uniformLocation = GL.GetUniformLocation(_program, uniform);

            if (uniformLocation == -1) {
                LogManager.Error("ERROR: Could not find uniform " + uniform);
            }

            _uniforms.Add(uniform, uniformLocation);
        }

        public void AddVertexShaderFromFile(string text) {
            AddProgram(LoadShader(text), ShaderType.VertexShader);
        }

        public void AddFragmentShaderFromFile(string text) {
            AddProgram(LoadShader(text), ShaderType.FragmentShader);

        }

        public void AddGeometryShaderFromFile(string text) {
            AddProgram(LoadShader(text), ShaderType.GeometryShader);
        }

        public void AddVertexShader(string text) {
            AddProgram(text, ShaderType.VertexShader);
        }

        public void AddFragmentShader(string text) {
            AddProgram(text, ShaderType.FragmentShader);

        }

        public void AddGeometryShader(string text) {
            AddProgram(text, ShaderType.GeometryShader);
        }

        public void CompileShader() {
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

        public void AddProgram(string text, ShaderType type) {
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
        public void SetUniform(string uniformName, Color value) {
            GL.Uniform4(_uniforms[uniformName], value);
        }
        public void SetUniform(string uniformName, Matrix4 value) {
            GL.UniformMatrix4(_uniforms[uniformName], false, ref value);
        }

        private static string LoadShader(string filename) {
            var shaderSource = "";

            try {
                shaderSource = File.ReadAllText(Path.Combine("./res/shaders", filename));
            }
            catch (Exception e) {
                LogManager.Error(e.Message);
            }

            return shaderSource;
        }

        public RenderingEngine RenderingEngine
        {
            get { return _renderingEngine; }
            set { _renderingEngine = value; }
        }
    }
}
