using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Game1
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private const int dedicatedVarbytes = 5;
        private static byte clientID;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D pixel;
        KeyboardState oldK, newK;
        MouseState oldM, newM;

        private static Character[] charList = new Character[0];
        private static Color color = Color.Red;
        private static Point location = new Point(128, 128);

        public Game()
        {
            Window.Title = "GameClient";
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 256;
            graphics.PreferredBackBufferWidth = 256;
        }
        protected override void Initialize()
        {
            LoopConnect();
            oldK = Keyboard.GetState();
            oldM = Mouse.GetState();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixel = Content.Load<Texture2D>("Pixel2");
        }
        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            newK = Keyboard.GetState();
            newM = Mouse.GetState();

            if (newK.IsKeyDown(Keys.Up))
                location.Y--;
            if (newK.IsKeyDown(Keys.Down))
                location.Y++;
            if (newK.IsKeyDown(Keys.Left))
                location.X--;
            if (newK.IsKeyDown(Keys.Right))
                location.X++;

            oldK = newK;
            oldM = newM;
            SendLoop();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            foreach (Character cha in charList) cha.Draw(spriteBatch, pixel); 
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private static void LoopConnect()
        {
            while (!_clientSocket.Connected)
            {
                try
                {
                    _clientSocket.Connect(IPAddress.Loopback, 100);
                }
                catch (SocketException)
                { }
            }
            color = Color.White;
            byte[] receivedBuf = new byte[1024];
            int rec = _clientSocket.Receive(receivedBuf);
            byte[] data = new byte[rec];
            Array.Copy(receivedBuf, data, rec);
            clientID = data[0];
        }
        private static void SendLoop()
        {
                byte[] buffer = new byte[dedicatedVarbytes] { clientID, (byte)location.X, (byte)location.Y, 0, 0 };
                _clientSocket.Send(buffer);
                byte[] receivedBuf = new byte[1024];
                int rec = _clientSocket.Receive(receivedBuf);
                byte[] data = new byte[rec];
                Array.Copy(receivedBuf, data, rec);
                charList = new Character[rec / 3];
                for (int i = 0; i < rec / 3; i++)
                {
                    charList[i] = new Character(data[3 * i], data[3 * i + 1], data[3 * i + 2]);
                }  
        }
        private static byte[] Append(byte[] A, byte[] B)
        {
            byte[] O = new byte[A.Length + B.Length];
            Array.Copy(A, O, A.Length);
            for (int i = 0; i < B.Length; i++)
            {
                O[i + A.Length] = B[i];
            }
            return O;
        }
    }
}
