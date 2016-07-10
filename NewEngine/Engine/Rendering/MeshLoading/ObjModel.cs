using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NewEngine.Engine.Core;
using OpenTK;

namespace NewEngine.Engine.Rendering.MeshLoading {
    public class ObjModel {
        private List<Vector3> _positions;
        private List<Vector2> _texCoords;
        private List<Vector3> _normals;
        private List<ObjIndex> _indices;

        private bool hasTexCoords;
        private bool hasNormals;

        public ObjModel(string filename) {
            _positions = new List<Vector3>();
            _texCoords = new List<Vector2>();
            _normals = new List<Vector3>();
            _indices = new List<ObjIndex>();

            try {
                var reader = new StreamReader(filename);
                string line;

                while ((line = reader.ReadLine()) != null) {
                    string[] tokens = line.Split(' ');
                    tokens = Util.RemoveEmptyStrings(tokens);

                    switch (tokens[0]) {
                        case "v":
                            _positions.Add(new Vector3(tokens[1].ParseInvariantFloat(), tokens[2].ParseInvariantFloat(), tokens[3].ParseInvariantFloat()));
                            break;
                        case "vt":
                            _texCoords.Add(new Vector2(tokens[1].ParseInvariantFloat(), tokens[2].ParseInvariantFloat()));
                            break;
                        case "vn":
                            _normals.Add(new Vector3(tokens[1].ParseInvariantFloat(), tokens[2].ParseInvariantFloat(), tokens[3].ParseInvariantFloat()));
                            break;
                        case "f":
                            for (int i = 0; i < tokens.Length - 3; i++) {
                                _indices.Add(ParseObjIndex(tokens[1]));
                                _indices.Add(ParseObjIndex(tokens[2 + i]));
                                _indices.Add(ParseObjIndex(tokens[3 + i]));
                            }
                            break;
                    }
                }
            }
            catch (Exception e) {
                LogManager.Error("Failed to load model " + e.Message + " at " +filename);
                LogManager.Error(e.StackTrace);
            }
        }

        public IndexedModel ToIndexedModel() {
            IndexedModel result = new IndexedModel();
            IndexedModel normalModel = new IndexedModel();
            //Dictionary<int, int> indexMap = new Dictionary<int, int>();
            Dictionary<ObjIndex, int?> resultIndexMap = new Dictionary<ObjIndex, int?>();
            Dictionary<int, int?> normalIndexMap = new Dictionary<int, int?>();
            Hashtable indexMap = new Hashtable();

            for (int i = 0; i < _indices.Count; i++) {
                ObjIndex currentIndex = _indices[i];

                Vector3 currentPosition = _positions[currentIndex.vertexIndex];
                Vector2 currentTexCoord;
                Vector3 currentNormal;

                if (hasTexCoords)
                    currentTexCoord = _texCoords[currentIndex.texCoordIndex];
                else {
                    currentTexCoord = new Vector2(0);
                }

                if (hasNormals)
                    currentNormal = _normals[currentIndex.normalIndex];
                else {
                    currentNormal = new Vector3(0);
                }

                int? modelVertexIndex;
                resultIndexMap.TryGetValue(currentIndex, out modelVertexIndex);

                if (modelVertexIndex == null) {
                    modelVertexIndex = result.Positions.Count;
                    resultIndexMap.Add(currentIndex, modelVertexIndex);

                    result.Positions.Add(currentPosition);
                    result.TexCoords.Add(currentTexCoord);
                    if (hasNormals)
                        result.Normals.Add(currentNormal);
                }


                int? normalModelVertexIndex;
                normalIndexMap.TryGetValue(currentIndex.vertexIndex, out normalModelVertexIndex);

                if (normalModelVertexIndex == null) {
                    normalModelVertexIndex = normalModel.Positions.Count;
                    normalIndexMap.Add(currentIndex.vertexIndex, normalModelVertexIndex);

                    normalModel.Positions.Add(currentPosition);
                    normalModel.TexCoords.Add(currentTexCoord);
                        normalModel.Normals.Add(currentNormal);
                }

                result.Indices.Add(modelVertexIndex);
                normalModel.Indices.Add(normalModelVertexIndex);

                if (!indexMap.ContainsKey(modelVertexIndex))
                    indexMap.Add(modelVertexIndex, normalModelVertexIndex);
                else {
                    indexMap[modelVertexIndex] = normalModelVertexIndex;
                }

            }
            if (!hasNormals) {
                normalModel.CalculateNormals();

                for (int i = 0; i < result.Positions.Count; i++) {
                    result.Normals.Add(normalModel.Normals[(int)indexMap[i]]);
                }
            }

            return result;
        }

        private ObjIndex ParseObjIndex(string token) {
            string[] values = token.Split('/');
            ObjIndex result = new ObjIndex();
            result.vertexIndex = int.Parse(values[0]) -1;

            if (values.Length > 1) {
                hasTexCoords = true;
                result.texCoordIndex = int.Parse(values[1])-1;

                if (values.Length > 2) {
                    hasNormals = true;
                    result.normalIndex = int.Parse(values[2])-1;
                }
            }
            return result;
        }

        //public List<Vector3> Normals
        //{
        //    get { return _normals; }
        //}

        //public List<Vector2> TexCoords
        //{
        //    get { return _texCoords; }
        //}

        //public List<Vector3> Positions
        //{
        //    get { return _positions; }
        //}

        //public List<ObjIndex> Indices
        //{
        //    get { return _indices; }
        //}
    }
}
