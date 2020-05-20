using Engine.Inputs;
using Engine.Texts;
using Engine.UI.Button;
using Microsoft.Xna.Framework;
using System;

namespace Engine.UI
{
    public class FlxSpriteButton:FlxTypedButton<FlxSprite>, IFlxInput
    {
        public FlxSpriteButton(float x = 0, float y = 0, FlxSprite label = null, Action onClick = null):base(x, y, onClick)
        {
            for(int i = 0; i < LabelOffsets.Length; i++)
            {
                LabelOffsets[i].X -= 1;
                LabelOffsets[i].Y += 4;
            }
            Label = label;
        }

        public FlxSpriteButton CreateTextLabel(string text, string fontPath, Color? color = null, FlxTextAlign? alignment = null)
        {
            if(text != null)
            {
                FlxText flxText = new FlxText(fontPath, 0, 0, FrameWidth, text);
                flxText.SetFormat(null, color, alignment);
                flxText.Alpha = LabelAlphas[(int)Status];
                
                Label = flxText;

            }
            return this;
        }

    }
}
