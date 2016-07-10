using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Voxel.Map;

namespace MinecraftClone {
   public class WaterSimulation {

        //public float[,,] Mass;
        //private float[,,] _newMass;

        ////Water properties
        //float _maxMass = 1.02f;
        //float _maxCompress = 0.2f;
        //float _minMass = 0.01f;

        //float _maxSpeed = 1; //max units of water moved out of one block to another, per timestep

        //float _minFlow = 0.1f;

        //void Simulate() {
        //    SimulateCompression();
        //}

        //public bool IsBlockGround(int x, int y, int z) {
        //    return (Map.GetVoxel(x, y, z).Type == BlockTypes.Grass || Map.GetVoxel(x, y, z).Type == BlockTypes.Stone ||
        //            Map.GetVoxel(x, y, z).Type == BlockTypes.Sand);
        //}

        //void SimulateCompression() {
        //    float flow;
        //    float remaining_mass;

        //    //Calculate and apply flow for each block
        //    for (int x = 0; x <= Scale.X - 1; x++) {
        //        for (int y = 0; y <= Scale.Y - 1; y++) {
        //            for (int z = 0; z <= Scale.Z - 1; z++) {
        //                //Skip inert ground blocks
        //                if (IsBlockGround(x, y, z)) continue;

        //                //Custom push-only flow
        //                remaining_mass = Mass[x, y, z];
        //                if (remaining_mass <= 0) continue;

        //                //The block below this one
        //                if (!IsBlockGround(x, y - 1, z)) {
        //                    flow = get_stable_state_b(remaining_mass + Mass[x, y - 1, z]) - Mass[x, y - 1, z];

        //                    if (flow > _minFlow) {
        //                        flow *= 0.5f; //leads to smoother flow
        //                    }
        //                    flow = ClampFloat(flow, 0, Math.Min(_maxSpeed, remaining_mass));

        //                    _newMass[x, y, z] -= flow;
        //                    _newMass[x, y - 1, z] += flow;
        //                    remaining_mass -= flow;
        //                }

        //                if (remaining_mass <= 0) continue;

        //                //Left
        //                if (!IsBlockGround(x - 1, y, z)) {
        //                    //Equalize the amount of water in this block and it's neighbour
        //                    flow = (Mass[x, y, z] - Mass[x - 1, y, z]) / 4;
        //                    if (flow > _minFlow) {
        //                        flow *= 0.5f;
        //                    }
        //                    flow = ClampFloat(flow, 0, remaining_mass);

        //                    _newMass[x, y, z] -= flow;
        //                    _newMass[x - 1, y, z] += flow;
        //                    remaining_mass -= flow;
        //                }

        //                if (remaining_mass <= 0) continue;

        //                //Right
        //                if (!IsBlockGround(x + 1, y, z)) {
        //                    //Equalize the amount of water in this block and it's neighbour
        //                    flow = (Mass[x, y, z] - Mass[x + 1, y, z]) / 4;
        //                    if (flow > _minFlow) {
        //                        flow *= 0.5f;
        //                    }
        //                    flow = ClampFloat(flow, 0, remaining_mass);

        //                    _newMass[x, y, z] -= flow;
        //                    _newMass[x + 1, y, z] += flow;
        //                    remaining_mass -= flow;
        //                }

        //                if (remaining_mass <= 0) continue;

        //                //forward
        //                if (!IsBlockGround(x, y, z + 1)) {
        //                    //Equalize the amount of water in this block and it's neighbour
        //                    flow = (Mass[x, y, z] - Mass[x, y, z + 1]) / 4;
        //                    if (flow > _minFlow) {
        //                        flow *= 0.5f;
        //                    }
        //                    flow = ClampFloat(flow, 0, remaining_mass);

        //                    _newMass[x, y, z] -= flow;
        //                    _newMass[x, y, z + 1] += flow;
        //                    remaining_mass -= flow;
        //                }

        //                if (remaining_mass <= 0) continue;

        //                //back
        //                if (!IsBlockGround(x, y, z - 1)) {
        //                    //Equalize the amount of water in this block and it's neighbour
        //                    flow = (Mass[x, y, z] - Mass[x, y, z - 1]) / 4;
        //                    if (flow > _minFlow) {
        //                        flow *= 0.5f;
        //                    }
        //                    flow = ClampFloat(flow, 0, remaining_mass);

        //                    _newMass[x, y, z] -= flow;
        //                    _newMass[x, y, z - 1] += flow;
        //                    remaining_mass -= flow;
        //                }

        //                if (remaining_mass <= 0) continue;

        //                //Up. Only compressed water flows upwards.
        //                if (IsBlockGround(x, y + 1, z)) {
        //                    flow = remaining_mass - get_stable_state_b(remaining_mass + Mass[x, y + 1, z]);
        //                    if (flow > _minFlow) {
        //                        flow *= 0.5f;
        //                    }
        //                    flow = ClampFloat(flow, 0, Math.Min(_maxSpeed, remaining_mass));

        //                    _newMass[x, y, z] -= flow;
        //                    _newMass[x, y + 1, z] += flow;
        //                    remaining_mass -= flow;
        //                }


        //            }
        //        }
        //    }

        //    //Copy the new mass values to the mass array
        //    for (int x = 0; x < Scale.X; x++) {
        //        for (int y = 0; y < Scale.Y; y++) {
        //            for (int z = 0; z < Scale.Z; z++) {
        //                Mass[x, y, z] = _newMass[x, y, z];
        //            }
        //        }
        //    }

        //    bool needsRegenration = false;

        //    for (int x = 0; x <= Scale.X; x++) {
        //        for (int y = 0; y <= Scale.Y; y++) {
        //            for (int z = 0; z < Scale.Z; z++) {
        //                //Skip ground blocks
        //                if (IsBlockGround(x, y, z)) continue;
        //                //Flag/unflag water blocks
        //                if (Mass[x, y, z] > _minMass) {
        //                    if (Map.GetVoxel(x, y, z).Type == BlockTypes.Water) { }
        //                    else {
        //                        Map.SetVoxel(x, y, z, BlockTypes.Water);
        //                        needsRegenration = true;
        //                    }
        //                }
        //                else {
        //                    if (Map.GetVoxel(x, y, z).Type == BlockTypes.Air) { }
        //                    else {
        //                        Map.SetVoxel(x, y, z, BlockTypes.Air);
        //                        needsRegenration = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //// needs a custom class
        //public static float ClampFloat(float val, float min, float max) {
        //    if (val.CompareTo(min) < 0) return min;
        //    else if (val.CompareTo(max) > 0) return max;
        //    else return val;
        //}

        //float get_stable_state_b(float totalMass) {
        //    if (totalMass <= 1) {
        //        return 1;
        //    }
        //    else if (totalMass < 2 * _maxMass + _maxCompress) {
        //        return (_maxMass * _maxMass + totalMass * _maxCompress) / (_maxMass + _maxCompress);
        //    }
        //    else {
        //        return (totalMass + _maxCompress) / 2;
        //    }
        //}
    }
}
