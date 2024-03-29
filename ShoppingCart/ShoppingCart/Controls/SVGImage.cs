﻿using SkiaSharp.Extended.Svg;
using SkiaSharp.Views.Forms;
using System;
using System.Reflection;
using Xamarin.Forms;

namespace habahabamall.Controls
{
    /// <summary>
    /// This is a helper class to render the SVG files.
    /// </summary>
    public class SVGImage : ContentView
    {
        // Bindable property to set the SVG image path
        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
            nameof(Source), typeof(string), typeof(SVGImage), default(string), propertyChanged: RedrawCanvas);

        private readonly SKCanvasView canvasView = new SKCanvasView();

        public SVGImage()
        {
            Padding = new Thickness(0);
            BackgroundColor = Color.Transparent;
            Content = canvasView;
            canvasView.PaintSurface += CanvasView_PaintSurface;
        }

        // Property to set the SVG image path
        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        /// <summary>
        /// Method to invaldate the canvas to update the image
        /// </summary>
        /// <param name="bindable">The target canvas</param>
        /// <param name="oldValue">Previous state</param>
        /// <param name="newValue">Updated state</param>
        public static void RedrawCanvas(BindableObject bindable, object oldValue, object newValue)
        {
            SVGImage sVGImage = bindable as SVGImage;
            sVGImage?.canvasView.InvalidateSurface();
        }

        /// <summary>
        /// This method update the canvas area with teh image
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The arguments</param>
        private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SkiaSharp.SKCanvas skCanvas = args.Surface.Canvas;
            skCanvas.Clear();

            if (string.IsNullOrEmpty(Source))
            {
                return;
            }

            // Get the assembly information to access the local image
            AssemblyName assembly = typeof(SVGImage).GetTypeInfo().Assembly.GetName();

            // Update the canvas with the SVG image
            using (System.IO.Stream stream = typeof(SVGImage).GetTypeInfo().Assembly
                .GetManifestResourceStream(assembly.Name + ".Images." + Source))
            {
                SKSvg skSVG = new SKSvg();
                _ = skSVG.Load(stream);
                SkiaSharp.SKImageInfo imageInfo = args.Info;
                skCanvas.Translate(imageInfo.Width / 2f, imageInfo.Height / 2f);
                SkiaSharp.SKRect rectBounds = skSVG.ViewBox;
                float xRatio = imageInfo.Width / rectBounds.Width;
                float yRatio = imageInfo.Height / rectBounds.Height;
                float minRatio = Math.Min(xRatio, yRatio);
                skCanvas.Scale(minRatio);
                skCanvas.Translate(-rectBounds.MidX, -rectBounds.MidY);
                skCanvas.DrawPicture(skSVG.Picture);
            }
        }
    }
}