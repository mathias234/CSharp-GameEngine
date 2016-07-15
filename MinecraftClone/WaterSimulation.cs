using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Voxel.Map;
using NewEngine.Engine.Core;
using OpenTK;

namespace MinecraftClone {
    public class WaterSimulation {
        private float[,,] _newMass;
        //Water properties
        float _maxMass = 1.0f;
        float _maxCompress = 0.02f;
        float _minMass = 0.00001f;

        float _maxSpeed = 1; //max units of water moved out of one block to another, per timestep

        float _minFlow = 0.01f;

        public WaterSimulation() {
        }

        public bool simulate;

        public void Simulate() {
            while (true) {
                if (simulate) {
                    SimulateCompression();
                    simulate = false;
                }
            }
        }

        public bool IsBlockGround(int x, int y, int z) {
            return (GameCode.GetBlockAt(x, y, z).Type == BlockTypes.Grass || GameCode.GetBlockAt(x, y, z).Type == BlockTypes.Stone ||
                    GameCode.GetBlockAt(x, y, z).Type == BlockTypes.Sand);
        }

        void SimulateCompression() {
            var size = new Vector3(GameCode._chunkSize.X * (float)GameCode.InitialChunkAmount,
                                    GameCode._chunkSize.Y * GameCode.InitialChunkAmount,
                                    GameCode._chunkSize.Z * GameCode.InitialChunkAmount);

            _newMass = new float[(int)size.X, (int)size.Y, (int)size.Z];


            float flow;
            float remaining_mass;

            //Calculate and apply flow for each block
            for (int x = 0; x <= size.X - 1; x++) {
                for (int y = 0; y <= size.Y - 1; y++) {
                    for (int z = 0; z <= size.Z - 1; z++) {
                        //Skip inert ground blocks
                        if (IsBlockGround(x, y, z)) continue;

                        //Custom push-only flow
                        remaining_mass = GameCode.GetBlockAt(x, y, z).Mass;
                        if (remaining_mass <= 0) continue;

                        //The block below this one
                        if (!IsBlockGround(x, y - 1, z)) {
                            flow = get_stable_state_b(remaining_mass + GameCode.GetBlockAt(x, y - 1, z).Mass - GameCode.GetBlockAt(x, y - 1, z).Mass);

                            if (flow > _minFlow) {
                                flow *= 0.5f; //leads to smoother flow
                            }
                            flow = ClampFloat(flow, 0, Math.Min(_maxSpeed, remaining_mass));

                            _newMass[x, y, z] -= flow;
                            _newMass[x, y - 1, z] += flow;
                            remaining_mass -= flow;
                        }

                        if (remaining_mass <= 0) continue;

                        //Left
                        if (!IsBlockGround(x - 1, y, z)) {
                            //Equalize the amount of water in this block and it's neighbour
                            flow = (GameCode.GetBlockAt(x, y, z).Mass - GameCode.GetBlockAt(x - 1, y, z).Mass) / 4;
                            if (flow > _minFlow) {
                                flow *= 0.5f;
                            }
                            flow = ClampFloat(flow, 0, remaining_mass);

                            _newMass[x, y, z] -= flow;
                            _newMass[x - 1, y, z] += flow;
                            remaining_mass -= flow;
                        }

                        if (remaining_mass <= 0) continue;

                        //Right
                        if (!IsBlockGround(x + 1, y, z)) {
                            //Equalize the amount of water in this block and it's neighbour
                            flow = (GameCode.GetBlockAt(x, y, z).Mass - GameCode.GetBlockAt(x + 1, y, z).Mass) / 4;
                            if (flow > _minFlow) {
                                flow *= 0.5f;
                            }
                            flow = ClampFloat(flow, 0, remaining_mass);

                            _newMass[x, y, z] -= flow;
                            _newMass[x + 1, y, z] += flow;
                            remaining_mass -= flow;
                        }

                        if (remaining_mass <= 0) continue;

                        //forward
                        if (!IsBlockGround(x, y, z + 1)) {
                            //Equalize the amount of water in this block and it's neighbour
                            flow = (GameCode.GetBlockAt(x, y, z).Mass - GameCode.GetBlockAt(x, y, z + 1).Mass) / 4;
                            if (flow > _minFlow) {
                                flow *= 0.5f;
                            }
                            flow = ClampFloat(flow, 0, remaining_mass);

                            _newMass[x, y, z] -= flow;
                            _newMass[x, y, z + 1] += flow;
                            remaining_mass -= flow;
                        }

                        if (remaining_mass <= 0) continue;

                        //back
                        if (!IsBlockGround(x, y, z - 1)) {
                            //Equalize the amount of water in this block and it's neighbour
                            flow = (GameCode.GetBlockAt(x, y, z).Mass - GameCode.GetBlockAt(x, y, z - 1).Mass) / 4;
                            if (flow > _minFlow) {
                                flow *= 0.5f;
                            }
                            flow = ClampFloat(flow, 0, remaining_mass);

                            _newMass[x, y, z] -= flow;
                            _newMass[x, y, z - 1] += flow;
                            remaining_mass -= flow;
                        }

                        if (remaining_mass <= 0) continue;

                        //Up. Only compressed water flows upwards.
                        if (IsBlockGround(x, y + 1, z)) {
                            flow = remaining_mass - get_stable_state_b(remaining_mass + GameCode.GetBlockAt(x, y + 1, z).Mass);
                            if (flow > _minFlow) {
                                flow *= 0.5f;
                            }
                            flow = ClampFloat(flow, 0, Math.Min(_maxSpeed, remaining_mass));

                            _newMass[x, y, z] -= flow;
                            _newMass[x, y + 1, z] += flow;
                            remaining_mass -= flow;
                        }


                    }
                }
            }

            //Copy the new mass values to the mass array
            for (int x = 0; x <= size.X - 1; x++) {
                for (int y = 0; y <= size.Y - 1; y++) {
                    for (int z = 0; z <= size.Z - 1; z++) {
                        GameCode.GetBlockAt(x, y, z).Mass = _newMass[x, y, z];
                    }
                }
            }

            for (int x = 0; x < size.X; x++) {
                for (int y = 0; y < size.Y; y++) {
                    for (int z = 0; z < size.Z; z++) {
                        //Skip ground blocks
                        if (IsBlockGround(x, y, z)) continue;
                        //Flag/unflag water blocks
                        if (GameCode.GetBlockAt(x, y, z).Mass > _minMass) {
                            if (GameCode.GetBlockAt(x, y, z).Type == BlockTypes.Water) { }
                            else {
                                GameCode.SetBlockAt(x, y, z, BlockTypes.Water);
                            }
                        }
                        else {
                            if (GameCode.GetBlockAt(x, y, z).Type == BlockTypes.Air) { }
                            else {
                                GameCode.SetBlockAt(x, y, z, BlockTypes.Air);
                            }
                        }
                    }
                }
            }
        }

        // needs a custom class
        public static float ClampFloat(float val, float min, float max) {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        float get_stable_state_b(float totalMass) {
            if (totalMass <= 1) {
                return 1;
            }
            else if (totalMass < 2 * _maxMass + _maxCompress) {
                return (_maxMass * _maxMass + totalMass * _maxCompress) / (_maxMass + _maxCompress);
            }
            else {
                return (totalMass + _maxCompress) / 2;
            }
        }
    }
}
