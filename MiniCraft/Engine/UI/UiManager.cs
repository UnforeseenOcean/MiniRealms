﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniRealms.Engine.Gfx;

namespace MiniRealms.Engine.UI
{
    public class UiManager
    {
        public readonly McGame Game;
        public readonly InputHandler InputHandler;
        private readonly List<UiObject> _uiObjects;

        public UiManager(McGame game, InputHandler inputHandler)
        {
            Game = game;
            InputHandler = inputHandler;
            _uiObjects = new List<UiObject>();
        }

        public void Tick()
        {
            foreach (var item in _uiObjects)
            {
                item.Tick();
            }
        }

        public void Render(Screen screen)
        {
            foreach (var item in _uiObjects)
            {
                item.Render(screen);
            }
        }

        public void Clean()
        {
            _uiObjects.Clear();
        }

        public void Add(UiObject item)
        {
            _uiObjects.Add(item);
        }
    }
}
