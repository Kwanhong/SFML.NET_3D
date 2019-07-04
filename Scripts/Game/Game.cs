using SFML.Window;
using SFML.System;
using SFML.Audio;
using SFML.Graphics;
using System;
using static sfml.net_3d.Data;

namespace sfml.net_3d
{
    class Game
    {
        public Game()
        {
            Initialize();
            Run();
        }

        private void Initialize()
        {
            window.SetFramerateLimit(30);
            window.Closed += OnClosed;
            window.KeyPressed += OnKeyPressed;
        }

        private void Run()
        {
            while (window.IsOpen)
            {
                HandleEvent();
                Update();
                Display();
            }
        }

        private void HandleEvent()
        {
            window.DispatchEvents();
        }

        private void Update()
        {
            
        }

        private void Display()
        {


            window.Display();
            window.Clear(new Color(25,25,25));
        }

        #region EVENTS 
        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                window.Close();
            }
        }
        private void OnClosed(object sender, EventArgs e)
        {
            window.Close();
        }
        #endregion
    }
}