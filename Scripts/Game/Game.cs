using SFML.Window;
using SFML.System;
using SFML.Audio;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using static SFML_NET_3D.Data;
using static SFML_NET_3D.Constants;
using static SFML_NET_3D.Utility;

namespace SFML_NET_3D
{
    class Game
    {
        Box[,,] boxes;
        List<Box> boxList;
        Vector3f boxCount = new Vector3f(5, 5,5);
        Vector3f boxSize = new Vector3f(30, 30, 30);

        public Game()
        {
            Awake();
            Run();
        }

        private void Awake()
        {
            window.SetFramerateLimit(30);
            window.Closed += OnClosed;
            window.KeyPressed += OnKeyPressed;

            Initialize();
        }

        private void Initialize()
        {
            Renderer.Clear();

            boxes = new Box[(int)boxCount.X, (int)boxCount.Y, (int)boxCount.Z];
            for (int x = 0; x < (int)boxCount.X; x++)
                for (int y = 0; y < (int)boxCount.Y; y++)
                    for (int z = 0; z < (int)boxCount.Z; z++)
                        SetBoxState(x, y, z);

            boxList = new List<Box>();

            foreach (var box in boxes)
                boxList.Add(box);
        }

        private void SetBoxState(int x, int y, int z)
        {
            byte color = (byte)new Random().Next(255);

            boxes[x, y, z] = new Box
            (
                size: boxSize,
                position: new Vector3f
                (
                    x * boxSize.X - ((boxCount.X - 1) * boxSize.X) / 2,
                    y * boxSize.Y - ((boxCount.Y - 1) * boxSize.Y) / 2,
                    z * boxSize.Z - ((boxCount.Z - 1) * boxSize.Z) / 2
                ),
                rotation: new Vector3f(0, 0, 0),
                fillColor: new Color(color, (byte)((color * 5) % 255), (byte)((color * 5) % 255)),
                type: PrimitiveType.Quads
            );
        }

        private void Update()
        {
            foreach (var box in boxList)
            {
                box.Rotation = new Vector3f
                (
                   Map(Mouse.GetPosition(window).X, 0, winSizeX, MathF.PI, -MathF.PI),
                   Map(Mouse.GetPosition(window).Y, 0, winSizeY, MathF.PI, -MathF.PI),
                   box.Rotation.Z
                );
                box.Update();
            }
        }

        private void Display()
        {
            Renderer.Render();
            window.Display();
            window.Clear(new Color(25, 25, 25));
        }

        private void LateUpdate()
        {
            foreach (var box in boxList)
            {
                box.LateUpdate();
            }
        }

        private void Run()
        {
            while (window.IsOpen)
            {
                HandleEvent();
                Update();
                Display();
                LateUpdate();
            }
        }

        #region EVENTS 
        private void HandleEvent()
        {
            window.DispatchEvents();
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape || e.Code == Keyboard.Key.F4)
            {
                window.Close();
            }
            if (e.Code == Keyboard.Key.F5)
            {
                Initialize();
            }
            if (e.Code == Keyboard.Key.F6)
            {
                if (winViewMode == ViewMode.Perspective)
                    winViewMode = ViewMode.Orthographic;
                else
                    winViewMode = ViewMode.Perspective;
            }
        }
        private void OnClosed(object sender, EventArgs e)
        {
            window.Close();
        }
        #endregion
    }
}