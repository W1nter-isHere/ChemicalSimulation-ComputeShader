using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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

    [SerializeField] private RenderTexture _grid;
    [SerializeField] private RenderTexture _grid2;

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

        _grid = new RenderTexture(Screen.width / cellWidth, Screen.height / cellHeight, 24)
        {
            enableRandomWrite = true,
            filterMode = FilterMode.Point
        };
        _grid.Create();

        _grid2 = new RenderTexture(Screen.width / cellWidth, Screen.height / cellHeight, 24)
        {
            enableRandomWrite = true,
            filterMode = FilterMode.Point
        };
        _grid2.Create();
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        var threadGroupsX = Mathf.CeilToInt(_grid.width / (float)ThreadX);
        var threadGroupsY = Mathf.CeilToInt(_grid.height / (float)ThreadY);
        var resultTexture = _oddCycle ? _grid : _grid2;
        var inputTexture = _oddCycle ? _grid2 : _grid;

        clearScreenShader.SetTexture(0, "result", resultTexture);
        clearScreenShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        
        updateShader.SetTexture(0, "result", resultTexture);
        updateShader.SetTexture(0, "last_update", inputTexture);
        updateShader.SetFloat("cell_width", cellWidth);
        updateShader.SetFloat("cell_height", cellHeight);
        updateShader.SetFloat("screen_width", _grid.width);
        updateShader.SetFloat("screen_height", _grid.height);
        updateShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        
        if (_cellPositionsToAdd.Count > 0)
        {
            var toAddBuffer = new ComputeBuffer(_cellPositionsToAdd.Count, sizeof(uint) * 2 + sizeof(float) * 4);
            toAddBuffer.SetData(_cellPositionsToAdd);
            
            addCellShader.SetBuffer(0, "cells_to_add", toAddBuffer);
            addCellShader.SetTexture(0, "result", resultTexture);
            addCellShader.SetTexture(0, "last_update", inputTexture);
            addCellShader.Dispatch(0, _cellPositionsToAdd.Count, 1, 1);

            toAddBuffer.Dispose();
            _cellPositionsToAdd.Clear();
        }
        
        Graphics.Blit(resultTexture, dest);

        _oddCycle = !_oddCycle;
    }

    public void AddCell(Vector3 screenPosition, Color color)
    {
        var center = new int2(
            Mathf.FloorToInt(screenPosition.x / cellWidth),
            Mathf.FloorToInt(screenPosition.y / cellHeight)
        );

        foreach (var pos in CirclePositions(center, 4).Where(_ => Random.Range(0, 2) == 1))
        {
            _cellPositionsToAdd.Add(new Cell 
            {
                Position = pos,
                Color = new float4(color.r, color.g, color.b, color.a), 
            });
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

        return positions;
    }
}