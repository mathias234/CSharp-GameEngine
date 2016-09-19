using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NewEngine.Engine.Core;
using OpenTK;

namespace NewEngine.Engine.Rendering.MeshLoading.Obj {
    public class ObjModel {
        private bool _hasNormals;

        private bool _hasTexCoords;
        private List<ObjIndex> _indices;
        private List<Vector3> _normals;
        private List<Vector3> _positions;
        private List<Vector2> _texCoords;

        public ObjModel(string filename) {
            _positions = new List<Vector3>();
            _texCoords = new List<Vector2>();
            _normals = new List<Vector3>();
            _indices = new List<ObjIndex>();

            try {
                var reader = new StreamReader(filename);
                string line;

                while ((line = reader.ReadLine()) != null) {
                    var tokens = line.Split(' ');
                    tokens = Util.RemoveEmptyStrings(tokens);

                    switch (tokens[0]) {
                        case "v":
                            _positions.Add(new Vector3(tokens[1].ParseInvariantFloat(), tokens[2].ParseInvariantFloat(),
                                tokens[3].ParseInvariantFloat()));
                            break;
                        case "vt":
                            _texCoords.Add(new Vector2(tokens[1].ParseInvariantFloat(), tokens[2].ParseInvariantFloat()));
                            break;
                        case "vn":
                            _normals.Add(new Vector3(tokens[1].ParseInvariantFloat(), tokens[2].ParseInvariantFloat(),
                                tokens[3].ParseInvariantFloat()));
                            break;
                        case "f":
                            for (var i = 0; i < tokens.Length - 3; i++) {
                                _indices.Add(ParseObjIndex(tokens[1]));
                                _indices.Add(ParseObjIndex(tokens[2 + i]));
                                _indices.Add(ParseObjIndex(tokens[3 + i]));
                            }
                            break;
                    }
                }
            }
            catch (Exception e) {
                LogManager.Error("Failed to load model " + e.Message + " at " + filename);
                LogManager.Error(e.StackTrace);
            }
        }

        public IndexedModel ToIndexedModel() {
            var result = new IndexedModel();
            var normalModel = new IndexedModel();
            var resultIndexMap = new Dictionary<ObjIndex, int?>();
            var normalIndexMap = new Dictionary<int, int?>();
            var indexMap = new Hashtable();

            for (var i = 0; i < _indices.Count; i++) {
                var currentIndex = _indices[i];

                var currentPosition = _positions[currentIndex.VertexIndex];

                var currentTexCoord = _hasTexCoords ? _texCoords[currentIndex.TexCoordIndex] : new Vector2(0);

                var currentNormal = _hasNormals ? _normals[currentIndex.NormalIndex] : new Vector3(0);

                int? modelVertexIndex;
                resultIndexMap.TryGetValue(currentIndex, out modelVertexIndex);

                if (modelVertexIndex == null) {
                    modelVertexIndex = result.Positions.Count;
                    resultIndexMap.Add(currentIndex, modelVertexIndex);

                    result.Positions.Add(currentPosition);
                    result.TexCoords.Add(currentTexCoord);
                    if (_hasNormals)
                        result.Normals.Add(currentNormal);
                }


                int? normalModelVertexIndex;
                normalIndexMap.TryGetValue(currentIndex.VertexIndex, out normalModelVertexIndex);

                if (normalModelVertexIndex == null) {
                    normalModelVertexIndex = normalModel.Positions.Count;
                    normalIndexMap.Add(currentIndex.VertexIndex, normalModelVertexIndex);

                    normalModel.Positions.Add(currentPosition);
                    normalModel.TexCoords.Add(currentTexCoord);
                    normalModel.Normals.Add(currentNormal);
                    normalModel.Tangents.Add(new Vector3(1));
                }

                result.Indices.Add(modelVertexIndex);
                normalModel.Indices.Add(normalModelVertexIndex);

                if (!indexMap.ContainsKey(modelVertexIndex))
                    indexMap.Add(modelVertexIndex, normalModelVertexIndex);
                else {
                    indexMap[modelVertexIndex] = normalModelVertexIndex;
                }
            }
            if (!_hasNormals) {
                normalModel.CalculateNormals();

                for (var i = 0; i < result.Positions.Count; i++) {
                    result.Normals.Add(normalModel.Normals[(int) indexMap[i]]);
                }
            }

            normalModel.CalculateTangents();

            for (var i = 0; i < result.Positions.Count; i++) {
                result.Tangents.Add(normalModel.Tangents[(int) indexMap[i]]);
            }

            for (var i = 0; i < result.Positions.Count; i++) {
                result.TexCoords[i] = new Vector2(result.TexCoords[i].X, -result.TexCoords[i].Y);
            }

            return result;
        }

        private ObjIndex ParseObjIndex(string token) {
            var values = token.Split('/');
            var result = new ObjIndex {VertexIndex = int.Parse(values[0]) - 1};

            if (values.Length <= 1) return result;
            _hasTexCoords = true;
            result.TexCoordIndex = int.Parse(values[1]) - 1;

            if (values.Length <= 2) return result;
            _hasNormals = true;
            result.NormalIndex = int.Parse(values[2]) - 1;
            return result;
        }
    }
}