﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;

using Color = System.Drawing.Color;

namespace FDK
{
    public class CSixLaborsTextRenderer : IDisposable
    {
        public CSixLaborsTextRenderer( string fontpath, int pt )
        {
            Initialize(fontpath, pt, FontStyle.Regular);
        }

        public CSixLaborsTextRenderer(string fontpath, int pt, FontStyle style)
        {
            Initialize(fontpath, pt, style);
        }

        protected void Initialize(string fontpath, int pt, FontStyle style) 
        {

            this.pt = (pt * 1.5f);
            this.fontStyle = style;

            if (File.Exists(fontpath))
            {
                this.fontFamily = new FontCollection().Install(fontpath);
            }
            else if (SystemFonts.TryFind(fontpath, out this.fontFamily))
            {
                //システムフォント
            }
            else 
            {
                this.fontFamily = new FontCollection().Install(Assembly.GetExecutingAssembly().GetManifestResourceStream(@"FDK.mplus-1p-medium.ttf"));
            }
            this.font = this.fontFamily.CreateFont(this.pt, this.fontStyle);
        }

        public Image<Rgba32> DrawPrivateFont(string drawstr, Color fontColor)
        {
            return DrawPrivateFont(drawstr, CPrivateFont.DrawMode.Normal, fontColor, Color.White, Color.White, Color.White, 0);
        }

        public Image<Rgba32> DrawPrivateFont(string drawstr, Color fontColor, Color edgeColor, int edge_Ratio)
        {
            return DrawPrivateFont(drawstr, CPrivateFont.DrawMode.Edge, fontColor, edgeColor, Color.White, Color.White, edge_Ratio);
        }

        public Image<Rgba32> DrawPrivateFont(string drawstr, Color fontColor, Color gradationTopColor, Color gradataionBottomColor, int edge_Ratio)
        {
            return DrawPrivateFont(drawstr, CPrivateFont.DrawMode.Gradation, fontColor, Color.White, gradationTopColor, gradataionBottomColor, edge_Ratio);
        }

        public Image<Rgba32> DrawPrivateFont(string drawstr, Color fontColor, Color edgeColor, Color gradationTopColor, Color gradataionBottomColor, int edge_Ratio)
        {
            return DrawPrivateFont(drawstr, CPrivateFont.DrawMode.Edge | CPrivateFont.DrawMode.Gradation, fontColor, edgeColor, gradationTopColor, gradataionBottomColor, edge_Ratio);
        }

        protected Image<Rgba32> DrawPrivateFont(string drawstr, CPrivateFont.DrawMode drawmode, Color fontColor, Color edgeColor, Color gradationTopColor, Color gradationBottomColor, int edge_Ratio)
        {
            if(string.IsNullOrEmpty(drawstr))
            {
                //Error
                return new Image<Rgba32>(1, 1);
            }

            RendererOptions roption = new RendererOptions(this.font);
            FontRectangle size = TextMeasurer.Measure(drawstr, roption);

            Image<Rgba32> image = new Image<Rgba32>((int)size.Width + 20, (int)size.Height + 20);

            SixLabors.ImageSharp.Color fontColorL = SixLabors.ImageSharp.Color.FromRgba(fontColor.R, fontColor.G, fontColor.B, fontColor.A);
            SixLabors.ImageSharp.Color edgeColorL = SixLabors.ImageSharp.Color.FromRgba(edgeColor.R, edgeColor.G, edgeColor.B, edgeColor.A);
            SixLabors.ImageSharp.Color gradationTopColorL = SixLabors.ImageSharp.Color.FromRgba(gradationTopColor.R, gradationTopColor.G, gradationTopColor.B, gradationTopColor.A);
            SixLabors.ImageSharp.Color gradationBottomColorL = SixLabors.ImageSharp.Color.FromRgba(gradationBottomColor.R, gradationBottomColor.G, gradationBottomColor.B, gradationBottomColor.A);


            IBrush brush;
            if ((drawmode & CPrivateFont.DrawMode.Gradation) == CPrivateFont.DrawMode.Gradation)
            {
                brush = new LinearGradientBrush(new PointF(0, size.Top), new PointF(0, size.Height), GradientRepetitionMode.None, new ColorStop(0, gradationTopColorL), new ColorStop(1, gradationBottomColorL));
            }
            else
            {
                brush = new SolidBrush(fontColorL);
            }

            if (drawmode == CPrivateFont.DrawMode.Normal)
            {
                image.Mutate(ctx => ctx.DrawText(drawstr, this.font, brush, PointF.Empty));
            }
            else if ((drawmode & CPrivateFont.DrawMode.Edge) == CPrivateFont.DrawMode.Edge) 
            {
                DrawingOptions doption = new DrawingOptions();
                IPathCollection pathc = TextBuilder.GenerateGlyphs(drawstr, new PointF(10, 10), roption);
                image.Mutate(ctx => ctx.Draw(doption, new Pen(edgeColorL, this.pt * 5 / edge_Ratio), pathc));

                //どちらを使いましょう？
                //image.Mutate(ctx => ctx.DrawText(drawstr, this.font, brush, new PointF(10, 10)));
                image.Mutate(ctx => ctx.Fill(doption, brush, pathc));
            }

            return image;
        }

        public void Dispose() 
        {
        }

        private float pt = 12;
        private FontStyle fontStyle;
        private FontFamily fontFamily;
        private Font font;
    }
}