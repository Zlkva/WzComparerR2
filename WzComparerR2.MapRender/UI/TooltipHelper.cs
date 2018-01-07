using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using WzComparerR2.Common;
using WzComparerR2.Rendering;

namespace WzComparerR2.MapRender.UI
{
    public static class TooltipHelper
    {
        public static TextBlock PrepareTextBlock(XnaFont font, string text, ref Vector2 pos, Color color)
        {
            Vector2 size = font.MeasureString(text);

            TextBlock block = new TextBlock();
            block.Font = font;
            block.Text = text;
            block.Position = pos;
            block.ForeColor = color;
           
            pos.X += size.X;
            return block;
        }

        public static TextBlock PrepareTextLine(XnaFont font, string text, ref Vector2 pos, Color color, ref float maxWidth)
        {
            Vector2 size = font.MeasureString(text);

            TextBlock block = new TextBlock();
            block.Font = font;
            block.Text = text;
            block.Position = pos;
            block.ForeColor = color;

            maxWidth = Math.Max(pos.X + size.X, maxWidth);
            pos.X = 0;
            pos.Y += font.Height;

            if (size.Y >= font.Height)
            {
                pos.Y += size.Y - font.BaseFont.Size;
            }

            return block;
        }

        public static TextBlock[] PrepareFormatText(XnaFont font, string formatText, ref Vector2 pos, int width, ref float maxWidth)
        {
            var layouter = new TextLayouter();
            int y = (int)pos.Y;
            var blocks = layouter.LayoutFormatText(font, formatText, width, ref y);
            for(int i = 0; i < blocks.Length; i++)
            {
                blocks[i].Position.X += pos.X;
                var blockWidth = blocks[i].Font.MeasureString(blocks[i].Text).X;
                maxWidth = Math.Max(maxWidth, blocks[i].Position.X + blockWidth);
            }
            pos.X = 0;
            pos.Y = y;
            return blocks;
        }

        public static TextBlock[] Prepare(LifeInfo info, MapRenderFonts fonts, out Vector2 size)
        {
            var blocks = new List<TextBlock>();
            var current = Vector2.Zero;
            size = Vector2.Zero;

            blocks.Add(PrepareTextLine(fonts.TooltipContentFont, "Level: " + info.level + (info.boss ? " (Boss)" : null), ref current, Color.White, ref size.X));
            blocks.Add(PrepareTextLine(fonts.TooltipContentFont, "HP/MP: " + info.maxHP + " / " + info.maxMP, ref current, Color.White, ref size.X));
            blocks.Add(PrepareTextLine(fonts.TooltipContentFont, "PAD/MAD: " + info.PADamage + " / " + info.MADamage, ref current, Color.White, ref size.X));
            blocks.Add(PrepareTextLine(fonts.TooltipContentFont, "PDr/MDr: " + info.PDRate + "% / " + info.MDRate + "%", ref current, Color.White, ref size.X));
            blocks.Add(PrepareTextLine(fonts.TooltipContentFont, "Acc/Eva: " + info.acc + " / " + info.eva, ref current, Color.White, ref size.X));
            blocks.Add(PrepareTextLine(fonts.TooltipContentFont, "KB: " + info.pushed, ref current, Color.White, ref size.X));
            blocks.Add(PrepareTextLine(fonts.TooltipContentFont, "Exp: " + info.exp, ref current, Color.White, ref size.X));
            if (info.undead) blocks.Add(PrepareTextLine(fonts.TooltipContentFont, "undead: 1", ref current, Color.White, ref size.X));
            StringBuilder sb;
            if ((sb = GetLifeElemAttrString(ref info.elemAttr)).Length > 0)
                blocks.Add(PrepareTextLine(fonts.TooltipContentFont, "elem: " + sb.ToString(), ref current, Color.White, ref size.X));
            size.Y = current.Y;

            return blocks.ToArray();
        }

        public static StringBuilder GetLifeElemAttrString(ref LifeInfo.ElemAttr elemAttr)
        {
            StringBuilder sb = new StringBuilder(14);
            sb.Append(GetElemResistanceString("Ice", elemAttr.I));
            sb.Append(GetElemResistanceString("Lightning", elemAttr.L));
            sb.Append(GetElemResistanceString("Fire", elemAttr.F));
            sb.Append(GetElemResistanceString("Poison", elemAttr.S));
            sb.Append(GetElemResistanceString("Holy", elemAttr.H));
            sb.Append(GetElemResistanceString("Dark", elemAttr.D));
            sb.Append(GetElemResistanceString("Physical", elemAttr.P));
            return sb;
        }

        public static string GetElemResistanceString(string elemName, LifeInfo.ElemResistance resist)
        {
            string e = null;
            switch (resist)
            {
                case LifeInfo.ElemResistance.Immune: e = "× "; break;
                case LifeInfo.ElemResistance.Resist: e = "△ "; break;
                case LifeInfo.ElemResistance.Normal: e = null; break;
                case LifeInfo.ElemResistance.Weak: e = "◎ "; break;
            }
            return e != null ? (elemName + e) : null;
        }

        public static string GetPortalTypeString(int pType)
        {
            switch (pType)
            {
                case 0: return "Map Starting Point";
                case 1: return "General Portal (Hidden)";
                case 2: return "General Portal";
                case 3: return "General Portal (Contact)";
                case 6: return "Timed Portal";
                case 7: return "Scripted Portal";
                case 8: return "Scripted Portal (Hidden)";
                case 9: return "Scripted Portal (Contact)";
                case 10: return "Map Warp Portal";
                case 12: return "Bounce Portal";
                default: return null;
            }
        }

        public struct TextBlock
        {
            public Vector2 Position;
            public Color ForeColor;
            public XnaFont Font;
            public string Text;
        }

        public class TextLayouter : XnaFontRenderer
        {
            public TextLayouter() : base(null)
            {

            }

            List<TextBlock> blocks;

            public TextBlock[] LayoutFormatText(XnaFont font, string s, int width, ref int y)
            {
                this.blocks = new List<TextBlock>();
                base.DrawFormatString(s, font, width, ref y, font.Height);
                return this.blocks.ToArray();
            }

            protected override void Flush(StringBuilder sb, int startIndex, int length, int x, int y, string colorID)
            {
                this.blocks.Add(new TextBlock()
                {
                    Position = new Vector2(x,y),
                    ForeColor = this.GetColor(colorID),
                    Font = this.font,
                    Text = sb.ToString(startIndex, length),
                });
            }
        }
    }
}
