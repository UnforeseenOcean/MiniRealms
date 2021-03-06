﻿using System;
using MiniRealms.Engine;
using MiniRealms.Engine.Gfx;
using MiniRealms.Entities;

namespace MiniRealms.Items
{

    public class ToolItem : Item
    {
        private readonly Random _random = new Random();

        //public const int MaxLevel = 5;

        private static readonly string[] LevelNames = { //
	        "Wood", "Rock", "Iron", "Gold", "Gem"//
	    };

        private static readonly int[] LevelColors = {//
	        Color.Get(-1, 100, 321, 431),//
			Color.Get(-1, 100, 321, 111),//
			Color.Get(-1, 100, 321, 555),//
			Color.Get(-1, 100, 321, 550),//
			Color.Get(-1, 100, 321, 055),//
	    };

        public ToolType ObjectType { get; }
        public int Level { get; }

        public ToolItem(ToolType objectType, int level)
        {
            ObjectType = objectType;
            Level = level;
        }

        public override int GetColor() => LevelColors[Level];

        public override int GetSprite() => ObjectType.Sprite + 5 * 32;

        public override void RenderIcon(Screen screen, int x, int y, int bits = 0)
        {
            screen.Render(x, y, GetSprite(), GetColor(), bits);
        }

        public override void RenderInventory(Screen screen, int x, int y)
        {
            screen.Render(x, y, GetSprite(), GetColor(), 0);
            Font.Draw(GetName(), screen, x + 8, y, Color.Get(-1, 555, 555, 555));
        }

        public override string GetName() => LevelNames[Level] + " " + ObjectType.Name;

        public override void OnTake(ItemEntity itemEntity)
        {
        }

        public override bool CanAttack() => true;

        public override int GetAttackDamageBonus(Entity e)
        {
            if (ObjectType == ToolType.Axe)
            {
                return (Level + 1) * 2 + _random.NextInt(4);
            }
            if (ObjectType == ToolType.Sword)
            {
                return (Level + 1) * 3 + _random.NextInt(2 + Level * Level * 2);
            }
            return 1;
        }

        public override bool Matches(Item item)
        {
            var toolItem = item as ToolItem;
            if (toolItem == null) return false;
            var other = toolItem;
            if (other.ObjectType != ObjectType) return false;
            return other.Level == Level;
        }
    }
}
