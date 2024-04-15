using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
 enum RoomItems
{
    BACKGROUND,
    WALL,
    BENCH,
    TABLE,
    COLLECTABLE,
    BOOKSHELF,
    TORCH,
    DOOR,
    CRATE
}

public class ImageProcessor : MonoBehaviour
{
    [SerializeField] private Texture2D _generatedDungeon;
    [SerializeField] private List<Texture2D> _generatedRooms;
    [SerializeField] private GameObject _backgroundObject;
    [SerializeField] private GameObject _roomObject;
    [SerializeField] private List<GameObject> _roomObjects;
    [SerializeField] private List<GameObject> _wallEdges;
    private int _width;
    private int _height;

    [SerializeField] private float _offset = 1.2f;

    private List<Vector2> _roomCoords;
    private List<GameObject> _processedRooms;


    private void Start()
    {
        _processedRooms = new List<GameObject>();
    }
    public int ProcessDungeon(int[][] dungeonData)
    {
        _roomCoords = new List<Vector2>();

        _width = dungeonData[0].Length;
        _height = dungeonData.Length;

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {

                int tPix = dungeonData[i][j];

                if (tPix == 255)
                {
                    _roomCoords.Add(new Vector2(i * 16, j * 16));
                    Debug.Log($"Neighbors at {i},{j}");
                    FindNeighbors(dungeonData, i, j);
                }
            }
        }
        return _roomCoords.Count;
    }

    public void ProcessRoom(int[][] roomData)
    {

        _width = roomData[0].Length;
        _height = roomData.Length;

        int rot = 0;
        GameObject g = new GameObject();


        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                int item = roomData[i][j];
                item = Mathf.Clamp(item, 0, 8);
                if (item == (int)RoomItems.DOOR)
                {
                    item = (int)RoomItems.BACKGROUND;
                }
                //More Model correction
                if (j == _width - 1)
                {
                    if (item != (int)RoomItems.WALL)
                    {
                        item = (int)RoomItems.WALL;
                    }
                }
                if (i == 0)
                {
                    if(item != (int)RoomItems.WALL)
                    {
                        item = (int)RoomItems.WALL;
                    }

                    rot = 90;

                    if (j == 0)
                    {
                        Debug.LogWarning("0,0");
                        item = 9;
                        rot = 90;
                    }
                    if (j == _width - 1)
                    {
                        Debug.LogWarning("0, width - 1");
                        item = 10;
                        rot = 0;
                    }
                }
                if (j == 0)
                {
                    if (item != (int)RoomItems.WALL)
                    {
                        item = (int)RoomItems.WALL;
                    }

                    rot = 180;

                    if (i == 0)
                    {
                        Debug.LogWarning("0,0");
                        item = 9;
                        rot = 90;
                    }

                    if(i == _width - 1)
                    {
                        item = 11;
                        rot = 0;
                    }
                }
                if (i == _width - 1)
                {
                    if (item != (int)RoomItems.WALL)
                    {
                        item = (int)RoomItems.WALL;
                    }

                    rot = - 90;

                    if (j == 0)
                    {
                        Debug.LogWarning("0,0");
                        item = 11;
                        rot = 180;
                    }

                    if (j == _width - 1)
                    {
                        item = 12;
                        rot = -90;
                    }
                }

                var obj = Instantiate(_roomObjects[item], g.transform);
                obj.transform.position = new Vector2(i, j);
                obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot));
                obj.name += $"{i},{j}";

                rot = 0;

            }
        }
        _processedRooms.Add(g);
        g.transform.position = _roomCoords[0];
        g.name = $"Room({_roomCoords[0].x / 16},{_roomCoords[0].y / 16})";
        _roomCoords.RemoveAt(0);

        Camera.main.transform.position = new Vector3(_processedRooms[0].transform.position.x, _processedRooms[0].transform.position.y, Camera.main.transform.position.z);
    }

    private void FindNeighbors(int[][] array, int i, int j)
    {
        List<Vector2> deltas = new List<Vector2>{ new Vector2(0, -1), new Vector2 (-1, 0), new Vector2(1, 0), new Vector2 (0, 1)};
        foreach (var delta in deltas)
        {
            if (i + delta.x < 0 || i + delta.x >= array.Length ||
                j + delta.y < 0 || j + delta.y >= array[0].Length)
                continue;

            if(array[i + (int)delta.x][j + (int)delta.y] == 255) Debug.Log($"Neighbour found at ({i + (int)delta.x},{j + (int)delta.y})");
            //Console.WriteLine("{0}", array[i + (int)delta.x][j + (int)delta.y]);
        }
    }

}
