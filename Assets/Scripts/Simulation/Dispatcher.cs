using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Random = UnityEngine.Random;

namespace Simulation
{
    public class Dispatcher : MonoBehaviour
    {
        public static Dispatcher Instance { get; private set; }
        private const int ThreadX = 8;
        private const int ThreadY = 8;
    
        [SerializeField] private ComputeShader updateShader;
        [SerializeField] private ComputeShader addCellShader;
        [SerializeField] private ComputeShader clearScreenShader;
    
        [SerializeField, Min(1)] private int cellWidth;
        [SerializeField, Min(1)] private int cellHeight;

        private RenderTexture _grid;
        private RenderTexture _grid2;
        private RenderTexture _materialMap;
        private RenderTexture _materialMap2;

        private bool _oddCycle;
        private List<Cell> _cellPositionsToAdd;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                return;
            }

            Destroy(gameObject);
        }

        private void Start()
        {
            _cellPositionsToAdd = new List<Cell>();

            // initialize textures
            _grid = CreateRenderTexture();
            _grid2 = CreateRenderTexture();
            _materialMap = CreateRenderTexture();
            _materialMap2 = CreateRenderTexture();
        }
        
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            // for updateShader and clearScreenShader because they have the same thread count
            var threadGroupsX = Mathf.CeilToInt(_grid.width / (float)ThreadX);
            var threadGroupsY = Mathf.CeilToInt(_grid.height / (float)ThreadY);
            
            // cycle through grid 1 and 2
            var resultTexture = _oddCycle ? _grid : _grid2;
            var inputTexture = _oddCycle ? _grid2 : _grid;
            var resultMaterial = _oddCycle ? _materialMap : _materialMap2;
            var inputMaterial = _oddCycle ? _materialMap2 : _materialMap;

            // clear result textures so we see a new frame
            clearScreenShader.SetTexture(0, "result", resultTexture);
            clearScreenShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
            clearScreenShader.SetTexture(0, "result", resultMaterial);
            clearScreenShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

            // update cells
            updateShader.SetTexture(0, "result", resultTexture);
            updateShader.SetTexture(0, "last_update", inputTexture);
            updateShader.SetTexture(0, "material_map", resultMaterial);
            updateShader.SetTexture(0, "last_material_map", inputMaterial);
            updateShader.SetFloat("cell_width", cellWidth);
            updateShader.SetFloat("cell_height", cellHeight);
            updateShader.SetFloat("screen_width", _grid.width);
            updateShader.SetFloat("screen_height", _grid.height);
            updateShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        
            // if we need to add new cells
            if (_cellPositionsToAdd.Count > 0)
            {
                // initialize buffer
                var toAddBuffer = new ComputeBuffer(_cellPositionsToAdd.Count, sizeof(uint) * 2 + sizeof(float) * 8);
                toAddBuffer.SetData(_cellPositionsToAdd);
            
                // add cells
                addCellShader.SetBuffer(0, "cells_to_add", toAddBuffer);
                addCellShader.SetTexture(0, "result", resultTexture);
                addCellShader.SetTexture(0, "last_update", inputTexture);
                addCellShader.SetTexture(0, "material_map", resultMaterial);
                addCellShader.Dispatch(0, _cellPositionsToAdd.Count, 1, 1);

                // clean up
                toAddBuffer.Dispose();
                _cellPositionsToAdd.Clear();
            }
  
            // render final result
            Graphics.Blit(resultTexture, dest);
            
            // cycle
            _oddCycle = !_oddCycle;
        }

        public void AddCell(Vector3 screenPosition, Chemical chemical, int radius)
        {
            var center = new int2(
                Mathf.FloorToInt(screenPosition.x / cellWidth),
                Mathf.FloorToInt(screenPosition.y / cellHeight)
            );

            var r = radius - 1;

            if (r == 0)
            {
                _cellPositionsToAdd.Add(new Cell 
                {
                    Position = new uint2(center),
                    Color = chemical.Color,
                    Material = chemical.Material,
                });
            }
            else
            {
                foreach (var pos in CirclePositions(center, r).Where(_ => Random.Range(0, 2) == 1))
                {
                    _cellPositionsToAdd.Add(new Cell 
                    {
                        Position = pos,
                        Color = chemical.Color,
                        Material = chemical.Material,
                    });
                }
            }
        }
    
        private static IEnumerable<uint2> CirclePositions(int2 center, int radius) {
            var positions = new List<uint2>();
        
            for (var r = 0; r < radius; r++) {
                for (var x = center.x - radius; x <= center.x + radius; x++) {
                    if (x < 0) continue;
                    for (var y = center.y - radius; y <= center.y + radius; y++) {
                        if (y < 0) continue;
                        if (Mathf.Sqrt((x - center.x) * (x - center.x) + (y - center.y) * (y - center.y)) <= r + 1 && Mathf.Sqrt((x - center.x) * (x - center.x) + (y - center.y) * (y - center.y)) > r) {
                            positions.Add(new uint2((uint)x, (uint)y));
                        }
                    }
                }
            }
            
            positions.Add(new uint2(center));
            return positions;
        }

        private RenderTexture CreateRenderTexture()
        {
            var t = new RenderTexture(Screen.width / cellWidth, Screen.height / cellHeight, 24, GraphicsFormat.R16G16B16A16_SFloat)
            {
                enableRandomWrite = true,
                filterMode = FilterMode.Point
            };
            t.Create();
            return t;
        }
    }
}