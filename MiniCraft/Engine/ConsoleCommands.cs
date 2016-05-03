﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using GameConsole.ManualInterpreter;
using MiniRealms.Engine.LevelGens;
using MiniRealms.Entities;
using MiniRealms.Items;
using MiniRealms.Items.Resources;
using MiniRealms.Levels.Tiles;
using MiniRealms.Sounds;

namespace MiniRealms.Engine
{
    public class ConsoleCommands
    {
        private readonly McGame _game;
        public readonly ManualInterpreter ManualInterpreter = new ManualInterpreter();

        public ConsoleCommands(McGame game)
        {
            _game = game;
            ManualInterpreter.RegisterCommand("game-seed", GameSeedCommand);
            ManualInterpreter.RegisterCommand("give-item", GiveItemCommand);
            ManualInterpreter.RegisterCommand("spawn-mob", SpawnMobCommand);
            ManualInterpreter.RegisterCommand("kill-me", KillMeCommand);
            ManualInterpreter.RegisterCommand("save-image", SaveWorldImageCommand);
            ManualInterpreter.RegisterCommand("play-sound", PlaySoundCommand);
            ManualInterpreter.RegisterCommand("fps", _ => $"Current FPS: {game.FpsCounterComponent.FrameRate}");


            //Normal commands XD
            ManualInterpreter.RegisterCommand("clear", _ =>
            { 
                game.Console.Clear();
            });
        }

        private string PlaySoundCommand(string[] strings)
        {
            if (strings.Length < 1 || strings[0] == "list-sounds")
            {
                return $"All Sounds: {string.Join(", ", Sound.AllSounds.Keys)}";
            }

            strings[0] = strings[0].ToLower();

            if (Sound.AllSounds.ContainsKey(strings[0]))
            {
                Sound.AllSounds[strings[0]].Play();
                return $"Playing sound {strings[0]}";
            }

            return "Sound does not exist";
        }

        private string GameSeedCommand(string[] arg)
        {
            return $"Current game seed is: {LevelGen.Seed}";
        }

        private string SaveWorldImageCommand(string[] arg)
        {
            var sp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Output.png");

            if (_game.Levels?[_game.CurrentLevel] == null)
            {
                return "World is null, or doesn't exist!";
            }


            byte[] map = _game.Levels[_game.CurrentLevel].Tiles;

            var bmp = new Bitmap(512, 512, PixelFormat.Format32bppRgb);
            int[] pixels = new int[512*512];
            for (int y = 0; y < 512; y++)
            {
                for (int x = 0; x < 512; x++)
                {
                    int i = x + y*512;

                    if (map[i] == Tile.Water.Id) pixels[i] = 0x000080;
                    if (map[i] == Tile.Grass.Id) pixels[i] = 0x208020;
                    if (map[i] == Tile.Rock.Id) pixels[i] = 0xa0a0a0;
                    if (map[i] == Tile.Dirt.Id) pixels[i] = 0x604040;
                    if (map[i] == Tile.Sand.Id) pixels[i] = 0xa0a040;
                    if (map[i] == Tile.Tree.Id) pixels[i] = 0x003000;
                    if (map[i] == Tile.Lava.Id) pixels[i] = 0xff2020;
                    if (map[i] == Tile.Cloud.Id) pixels[i] = 0xa0a0a0;
                    if (map[i] == Tile.StairsDown.Id) pixels[i] = 0xffffff;
                    if (map[i] == Tile.StairsUp.Id) pixels[i] = 0xffffff;
                    if (map[i] == Tile.CloudCactus.Id) pixels[i] = 0xff00ff;

                    bmp.SetPixel(x, y, Color.FromArgb(pixels[i]));
                }
            }

            bmp.Save(sp, ImageFormat.Png);

            return $"Saved world image to {sp}";
        }

        private string KillMeCommand(string[] arg)
        {
            if (_game.Player == null)
            {
                return "Player doesn't exist in current game";
            }

            _game.Player.Health = 0;
            return "Who you kill self D:";
        }

        private string SpawnMobCommand(string[] strings)
        {
            if (_game.Levels?[_game.CurrentLevel] == null)
            {
                return "World is null, or doesn't exist!";
            }

            if (strings.Length < 1)
            {
                return "Invalid command args";
            }

            switch (strings[0].ToLower())
            {
                case "zombie":
                    _game.Levels?[_game.CurrentLevel].Add(new Zombie(1)
                    {
                        X = _game.Player.X + 5,
                        Y = _game.Player.Y + 5
                    });
                    return "Summoned a zombie";
                case "slime":
                    _game.Levels?[_game.CurrentLevel].Add(new Slime(1)
                    {
                        X = _game.Player.X + 5,
                        Y = _game.Player.Y + 5
                    });
                    return "Summoned a slime";

                case "creeper":
                    _game.Levels?[_game.CurrentLevel].Add(new Creeper(1)
                    {
                        X = _game.Player.X + 5,
                        Y = _game.Player.Y + 5
                    });
                    return "Summoned a Creeper";
            }
            return "Invalid mod type";
        }

        public string GiveItemCommand(string[] strings)
        {
            if (_game.Player == null)
            {
                return "Player doesn't exist in current game";
            }

            if (strings.Length < 2)
            {
                return "Invalid command args";
            }

            var r = Resource.AllResources;

            var item = r.FirstOrDefault(i => string.Equals(i.Name, strings[0], StringComparison.CurrentCultureIgnoreCase));

            if (item == null)
            {
                return $"Item {strings[0]} doesn't exist!";
            }

            int amount;
            bool count = int.TryParse(strings[1], out amount);

            if (!count)
            {
                return "Invalid number";
            }

            _game.Player.Inventory.Add(new ResourceItem(item, amount));

            return $"Gave {amount} of item {strings[0]}";
        }
    }
}