﻿using MiniRealms.Engine.Gfx;
using MiniRealms.Entities;

namespace MiniRealms.Items
{
    public class PowerGloveItem : Item
    {
        public override int GetColor() => Color.Get(-1, 100, 320, 430);

        public override int GetSprite() => 7 + 4 * 32;

        public override void RenderIcon(Screen screen, int x, int y, int bits = 0)
        {
            screen.Render(x, y, GetSprite(), GetColor(), bits);
        }

        public override void RenderInventory(Screen screen, int x, int y)
        {
            screen.Render(x, y, GetSprite(), GetColor(), 0);
            Font.Draw(GetName(), screen, x + 8, y, Color.Get(-1, 555, 555, 555));
        }

        public override string GetName() => "Pow glove";

        public override bool Interact(Player player, Entity entity, int attackDir)
        {
            var furniture = entity as Furniture;
            if (furniture == null) return false;
            Furniture f = furniture;
            f.Take(player);
            return true;
        }
    }
}
