﻿using System;
using MiniRealms.Engine.Gfx;
using MiniRealms.Engine.LevelGens;
using MiniRealms.Engine.ScoreSystem;
using MiniRealms.Screens.Interfaces;
using MiniRealms.Screens.MainScreens;

namespace MiniRealms.Screens.GameScreens
{

    public class GameOverMenu : Menu
    {
        private readonly string _title;
        private readonly string _body;
        private readonly bool _youWon;
        private int _inputDelay = 60;

        public GameOverMenu(string title, string body, bool youWon)
        {
            _title = title;
            _body = body;
            _youWon = youWon;
        }

        public override void Init(McGame game, InputHandler input)
        {
            base.Init(game, input);

            ScoreBoardManager.AddItem(DateTime.Now, game.GameTime, Game.Player.Score, McGame.Difficulty.ShortName, _youWon);
        }

        public override void Tick()
        {
            if (_inputDelay > 0)
                _inputDelay--;
            else if (Input.Attack.Clicked || Input.Menu.Clicked)
            {
                Game.Level = null;
                Game.Player = null;
                Game.Levels = null;
                LevelGen.R = null;
                Game.ResetGame();
                Game.SetMenu(new TitleMenu());
            }
        }

        public override void Render(Screen screen)
        {
            Font.RenderFrame(screen, _title, 10, 4, 29, 10);
            int seconds = Game.GameTime / 60;
            int minutes = seconds / 60;
            int hours = minutes / 60;
            minutes %= 60;
            seconds %= 60;

            var timeString = hours > 0
                ? hours + "h" + (minutes < 10 ? "0" : "") + minutes + "m"
                : minutes + "m " + (seconds < 10 ? "0" : "") + seconds + "s";
            Font.Draw("Time:", screen, 11 * 8, 6 * 8, Color.Get(-1, 555, 555, 555));
            Font.Draw(timeString, screen, (11 + 5) * 8, 6 * 8, Color.Get(-1, 550, 550, 550));
            Font.Draw("Score:", screen, 11 * 8, 7 * 8, Color.Get(-1, 555, 555, 555));
            Font.Draw("" + Game.Player.Score, screen, (11 + 6) * 8, 7 * 8, Color.Get(-1, 550, 550, 550));
            Font.Draw(_body, screen, 11 * 8, 9 * 8, Color.Get(-1, 333, 333, 333));
        }
    }
}
