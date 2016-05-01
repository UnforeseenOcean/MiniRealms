﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniCraft.Entities;
using MiniCraft.Gfx;
using MiniCraft.Levels;
using MiniCraft.Levels.Tiles;
using MiniCraft.Screens;
using MiniCraft.Sounds;

namespace MiniCraft
{
    //https://github.com/Kivutar/tethical
    //http://ffhacktics.com/smf/index.php?topic=6809.0
    //https://github.com/rmcn/PythonEmbrace

    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class McGame : Game
    {
        public const string Name = "Minicraft";
        public const int Height = 120;
        public const int Width = 160;
        public const int Scale = 4;

        private Texture2D _image;
        private Color[] _pixels;
        private bool _running;

        private Color[] _colors;
        private int _currentLevel = 3;
        public int GameTime;


        public bool HasWon;
        private InputHandler _input;

        private Level _level;
        private Level[] _levels = new Level[5];
        private Screen _lightScreen;

        public Menu Menu;
        private int _pendingLevelChange;
        public Player Player;
        private int _playerDeadTime;
        private Screen _screen;
        private SpriteBatch _spriteBatch;
        private int _tickCount;
        private int _wonTimer;

        public McGame()
        {
            var deviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = Height*Scale,
                PreferredBackBufferWidth = Width*Scale
            };

            Content.RootDirectory = "Content";

            Window.Title = Name;
        }

        public bool HasFocus() => IsActive;

        public void SetMenu(Menu menu)
        {
            Menu = menu;
            menu?.Init(this, _input);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            _running = false;
            base.OnExiting(sender, args);
        }

        public void ResetGame()
        {
            _playerDeadTime = 0;
            _wonTimer = 0;
            GameTime = 0;
            HasWon = false;

            _levels = new Level[5];
            _currentLevel = 3;

            _levels[4] = new Level(128, 128, 1, null);
            _levels[3] = new Level(128, 128, 0, _levels[4]);
            _levels[2] = new Level(128, 128, -1, _levels[3]);
            _levels[1] = new Level(128, 128, -2, _levels[2]);
            _levels[0] = new Level(128, 128, -3, _levels[1]);

            _level = _levels[_currentLevel];
            Player = new Player(this, _input);
            Player.FindStartPos(_level);

            _level.Add(Player);

            for (var i = 0; i < 5; i++)
            {
                _levels[i].TrySpawn(5000);
            }
        }

        protected override void LoadContent()
        {
            _running = true;

            var pp = 0;
            _colors = new Color[256];

            for (var r = 0; r < 6; r++)
            {
                for (var g = 0; g < 6; g++)
                {
                    for (var b = 0; b < 6; b++)
                    {
                        var rr = r*255/5;
                        var gg = g*255/5;
                        var bb = b*255/5;
                        var mid = (rr*30 + gg*59 + bb*11)/100;

                        var r1 = (rr + mid*1)/2*230/255 + 10;
                        var g1 = (gg + mid*1)/2*230/255 + 10;
                        var b1 = (bb + mid*1)/2*230/255 + 10;
                        _colors[pp++] = new Color(r1, g1, b1);
                    }
                }
            }

            //while (pp < 256)
            //{
            //    colors[pp++] = Color.Black;
            //}

            var spriteSheet = Content.Load<Texture2D>("Textures/icons");
            _screen = new Screen(Width, Height, new SpriteSheet(spriteSheet));
            _lightScreen = new Screen(Width, Height, new SpriteSheet(spriteSheet));

            Sound.Initialize(Content);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixels = new Color[Width*Height];
            _image = new Texture2D(GraphicsDevice, Width, Height);
            _input = new InputHandler();

            ResetGame();
            SetMenu(new TitleMenu());
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!_running) return;
            //assuming 60 updates from monogame
            Tick();

            base.Update(gameTime);
        }

        protected new virtual void Tick()
        {
            _tickCount++;
            if (!HasFocus())
            {
                _input.ReleaseAll();
            }
            else
            {
                if (!Player.Removed && !HasWon) GameTime++;

                _input.Tick();
                if (Menu != null)
                {
                    Menu.Tick();
                }
                else
                {
                    if (Player.Removed)
                    {
                        _playerDeadTime++;
                        if (_playerDeadTime > 60)
                        {
                            SetMenu(new DeadMenu());
                        }
                    }
                    else
                    {
                        if (_pendingLevelChange != 0)
                        {
                            SetMenu(new LevelTransitionMenu(_pendingLevelChange));
                            _pendingLevelChange = 0;
                        }
                    }
                    if (_wonTimer > 0)
                    {
                        if (--_wonTimer == 0)
                        {
                            SetMenu(new WonMenu());
                        }
                    }
                    _level.Tick();
                    Tile.TickCount++;
                }
            }
        }

        private void RenderFocusNagger()
        {
            var msg = "Click to focus!";
            var xx = (Width - msg.Length*8)/2;
            var yy = (Height - 8)/2;
            var w = msg.Length;
            var h = 1;

            _screen.Render(xx - 8, yy - 8, 0 + 13*32, ColorHelper.Get(-1, 1, 5, 445), 0);
            _screen.Render(xx + w*8, yy - 8, 0 + 13*32, ColorHelper.Get(-1, 1, 5, 445), 1);
            _screen.Render(xx - 8, yy + 8, 0 + 13*32, ColorHelper.Get(-1, 1, 5, 445), 2);
            _screen.Render(xx + w*8, yy + 8, 0 + 13*32, ColorHelper.Get(-1, 1, 5, 445), 3);
            for (var x = 0; x < w; x++)
            {
                _screen.Render(xx + x*8, yy - 8, 1 + 13*32, ColorHelper.Get(-1, 1, 5, 445), 0);
                _screen.Render(xx + x*8, yy + 8, 1 + 13*32, ColorHelper.Get(-1, 1, 5, 445), 2);
            }
            for (var y = 0; y < h; y++)
            {
                _screen.Render(xx - 8, yy + y*8, 2 + 13*32, ColorHelper.Get(-1, 1, 5, 445), 0);
                _screen.Render(xx + w*8, yy + y*8, 2 + 13*32, ColorHelper.Get(-1, 1, 5, 445), 1);
            }

            Font.Draw(msg, _screen, xx, yy,
                _tickCount/20%2 == 0 ? ColorHelper.Get(5, 333, 333, 333) : ColorHelper.Get(5, 555, 555, 555));
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var xScroll = Player.X - _screen.W/2;
            var yScroll = Player.Y - (_screen.H - 8)/2;
            if (xScroll < 16) xScroll = 16;
            if (yScroll < 16) yScroll = 16;
            if (xScroll > _level.W*16 - _screen.W - 16) xScroll = _level.W*16 - _screen.W - 16;
            if (yScroll > _level.H*16 - _screen.H - 16) yScroll = _level.H*16 - _screen.H - 16;
            if (_currentLevel > 3)
            {
                var col = ColorHelper.Get(20, 20, 121, 121);
                for (var y = 0; y < 14; y++)
                    for (var x = 0; x < 24; x++)
                    {
                        _screen.Render(x*8 - ((xScroll/4) & 7), y*8 - ((yScroll/4) & 7), 0, col, 0);
                    }
            }

            _level.RenderBackground(_screen, xScroll, yScroll);
            _level.RenderSprites(_screen, xScroll, yScroll);

            if (_currentLevel < 3)
            {
                _lightScreen.Clear(0);
                _level.RenderLight(_lightScreen, xScroll, yScroll);
                _screen.Overlay(_lightScreen, xScroll, yScroll);
            }

            RenderGui();

            if (!HasFocus()) RenderFocusNagger();

            for (var y = 0; y < _screen.H; y++)
            {
                for (var x = 0; x < _screen.W; x++)
                {
                    var cc = _screen.Pixels[x + y*_screen.W];
                    if (cc < 255) _pixels[x + y*Width] = _colors[cc];
                }
            }
            _image.SetData(_pixels);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_image, new Rectangle(0, 0, Width*Scale, Height*Scale), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void RenderGui()
        {
            for (var y = 0; y < 2; y++)
            {
                for (var x = 0; x < 20; x++)
                {
                    _screen.Render(x*8, _screen.H - 16 + y*8, 0 + 12*32, ColorHelper.Get(000, 000, 000, 000), 0);
                }
            }

            for (var i = 0; i < 10; i++)
            {
                _screen.Render(i*8, _screen.H - 16, 0 + 12*32,
                    i < Player.Health ? ColorHelper.Get(000, 200, 500, 533) : ColorHelper.Get(000, 100, 000, 000), 0);

                if (Player.StaminaRechargeDelay > 0)
                {
                    _screen.Render(i*8, _screen.H - 8, 1 + 12*32,
                        Player.StaminaRechargeDelay/4%2 == 0
                            ? ColorHelper.Get(000, 555, 000, 000)
                            : ColorHelper.Get(000, 110, 000, 000), 0);
                }
                else
                {
                    _screen.Render(i*8, _screen.H - 8, 1 + 12*32,
                        i < Player.Stamina ? ColorHelper.Get(000, 220, 550, 553) : ColorHelper.Get(000, 110, 000, 000),
                        0);
                }
            }
            Player.ActiveItem?.RenderInventory(_screen, 10*8, _screen.H - 16);

            Menu?.Render(_screen);
        }

        public void ChangeLevel(int dir)
        {
            _level.Remove(Player);
            _currentLevel += dir;
            _level = _levels[_currentLevel];
            Player.X = (Player.X >> 4)*16 + 8;
            Player.Y = (Player.Y >> 4)*16 + 8;
            _level.Add(Player);
        }

        public void ScheduleLevelChange(int dir)
        {
            _pendingLevelChange = dir;
        }

        public void Won()
        {
            _wonTimer = 60*3;
            HasWon = true;
        }
    }
}