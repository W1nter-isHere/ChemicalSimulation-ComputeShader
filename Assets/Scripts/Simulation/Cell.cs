using Unity.Mathematics;
using UnityEngine;

namespace Simulation
{
    public struct Cell
    {
        public uint2 Position;
        public Color Color;
        public Material Material;
    }

    public struct Chemical
    {
        public Material Material;
        public Color Color;
    }

    public struct Material
    {
        public static readonly Material Static = new(0, 0, 0, 0.25f);
        public static readonly Material Falling = new(0, 0, 0, 0.5f);
        public static readonly Material Liquid = new(0, 0, 0, 0.75f);
        public static readonly Material Gas = new(0, 0, 0, 1);
        public static readonly Material[] Materials = { Static, Falling, Liquid, Gas };

        public float R;
        public float G;
        public float B;
        public float State;

        private Material(float r, float g, float b, float state)
        {
            R = r;
            G = g;
            B = b;
            State = state;
        }
    }
}