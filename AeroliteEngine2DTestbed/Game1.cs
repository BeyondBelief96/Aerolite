﻿using AeroliteEngine2DTestbed.Scenes;
using Flat;
using Flat.Graphics;
using Flat.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace AeroliteEngine2DTestbed
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private Scene _currentScene;
        private Screen _screen;
        private Sprites _sprites;
        private Shapes _shapes;

        private const double UpdatesPerSecond = 60d;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.SynchronizeWithVerticalRetrace = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromTicks((long)Math.Round((double)TimeSpan.TicksPerSecond / UpdatesPerSecond));
        }

        protected override void Initialize()
        {
            FlatUtil.SetRelativeBackBufferSize(_graphics, 0.85f);
            _screen = new Screen(this, 1920, 1080);
            _sprites = new Sprites(this);
            _shapes = new Shapes(this);
            
            _currentScene = new SolarSystemScene(this, _screen, _sprites, _shapes);
            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            FlatKeyboard keyboard = FlatKeyboard.Instance;
            FlatMouse mouse = FlatMouse.Instance;

            keyboard.Update();
            mouse.Update();

            if(keyboard.IsKeyAvailable)
            {
                if(keyboard.IsKeyClicked(Keys.Escape))
                {
                    Exit();
                }
            }

            _currentScene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _currentScene.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
