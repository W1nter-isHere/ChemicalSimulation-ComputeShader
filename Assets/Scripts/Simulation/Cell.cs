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
        public static readonly Chemical Hydrogen = new() { Name = "Hydrogen", Material = Material.Gas, Color = new Color(0.52f, 1, 0.99f, 0.5f)};
        public static readonly Chemical Sodium = new() { Name = "Sodium", Material = Material.Falling, Color = new Color(0.88f, 0.9f, 0.9f)};
        public static readonly Chemical Water = new() { Name = "Water", Material = Material.Liquid, Color = new Color(0.2f, 0.5f, 1)};
        public static readonly Chemical[] DefaultChemicals = { Hydrogen, Sodium, Water };

        public string Name;
        public Material Material;
        public Color Color;
    }

    public struct Material
    {
        public static readonly Material Static = new(0, 0, 0.25f);
        public static readonly Material Falling = new(0, 0, 0.50f);
        public static readonly Material Liquid = new(0, 0, 0.75f);
        public static readonly Material Gas = new(0, 0, 1.00f);

        public float R;
        public float G;
        public float RandWeight;
        public float State;

        private Material(float r, float g, float state)
        {
            R = r;
            G = g;
            RandWeight = 0;
            State = state;
        }
    }
}