﻿// <copyright file="Histogram.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using System;
using System.Drawing;
using System.Linq;
using System.Text;
using BotEngineClient.ExtensionMethods;

// For Reference:
// https://github.com/xnafan/Simple-image-comparison/blob/master/SimpleImageComparisonClassLibrary/Histogram.cs

namespace BotEngineClient
{
    /// <summary>
    /// A class which facilitates working with RGB histograms
    /// It encapsulates a Bitmap and lets you get information about the Bitmap
    /// </summary>
    public class Histogram
    {

        #region Properties
        static readonly Pen[] p = new Pen[] { Pens.Red, Pens.Green, Pens.Blue };
        /// <summary>
        /// The red values in an image
        /// </summary>
        public byte[] Red { get; private set; } = new byte[256];

        /// <summary>
        /// The green values in an image
        /// </summary>
        public byte[] Green { get; private set; } = new byte[256];

        /// <summary>
        /// The blue values in an image
        /// </summary>
        public byte[] Blue { get; private set; } = new byte[256];
        #endregion

        #region Factory methods for constructing the histograms

        /// <summary>
        /// Constructs a new Histogram from a bitmap
        /// </summary>
        /// <param name="bitmap">The image to work with</param>
        public static Histogram FromBitmap(Bitmap bitmap)
        {
            return CalculateHistogram(bitmap);
        }

        /// <summary>
        /// Constructs a new Histogram from a file, given its path
        /// </summary>
        /// <param name="filePath">The path to the image to work with</param>
        public static Histogram FromImageFile(string bitmapFilePath)
        {
            return FromBitmap((Bitmap)Image.FromFile(bitmapFilePath));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the values in the histogram
        /// </summary>
        private static Histogram CalculateHistogram(Bitmap bitmap)
        {
            Histogram histogram = new Histogram();
            using (Bitmap newBmp = (Bitmap)bitmap.GetResizedVersion(16, 16))
            {
                Color c;
                for (int x = 0; x < newBmp.Width; x++)
                {
                    for (int y = 0; y < newBmp.Height; y++)
                    {
                        c = newBmp.GetPixel(x, y);
                        histogram.Red[c.R]++;
                        histogram.Green[c.G]++;
                        histogram.Blue[c.B]++;
                    }
                }

            }
            return histogram;
        }


        /// <summary>
        /// Gets a bitmap with the RGB histograms
        /// </summary>
        /// <returns>Three histograms for R, G and B values in the Histogram</returns>
        public Bitmap Visualize()
        {
            int oneColorHeight = 100;
            int margin = 10;

            float[] maxValues = new float[] { Red.Max(), Green.Max(), Blue.Max() };
            byte[][] values = new byte[][] { Red, Green, Blue };


            Bitmap histogramBitmap = new Bitmap(276, oneColorHeight * 3 + margin * 4);
            Graphics g = Graphics.FromImage(histogramBitmap);
            g.FillRectangle(Brushes.White, 0, 0, histogramBitmap.Width, histogramBitmap.Height);
            int yOffset = margin + oneColorHeight;

            for (int i = 0; i < 256; i++)
            {
                for (int color = 0; color < 3; color++)
                {
                    g.DrawLine(p[color], margin + i, yOffset * (color + 1), margin + i, yOffset * (color + 1) - (values[color][i] / maxValues[color]) * oneColorHeight);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                g.DrawString(p[i].Color.ToKnownColor() + ", max value: " + maxValues[i], SystemFonts.SmallCaptionFont, Brushes.Silver, margin + 11, yOffset * i + margin + margin + 1);
                g.DrawString(p[i].Color.ToKnownColor() + ", max value: " + maxValues[i], SystemFonts.SmallCaptionFont, Brushes.Black, margin + 10, yOffset * i + margin + margin);
                g.DrawRectangle(p[i], margin, yOffset * i + margin, 256, oneColorHeight);
            }
            g.Dispose();

            return histogramBitmap;
        }


        /// <summary>
        /// Gives a human-readable representation of the RGB values in the histogram
        /// </summary>
        /// <returns>a human-readable representation of the RGB values in the histogram</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 256; i++)
            {
                sb.Append(string.Format("RGB {0,3} : ", i) + string.Format("({0,3},{1,3},{2,3})", Red[i], Green[i], Blue[i]));
                sb.AppendLine();
            }

            return sb.ToString();
        }


        /// <summary>
        /// Gets the variance between two histograms (http://en.wikipedia.org/wiki/Variance) as a percentage of the maximum possible variance: 256 (for a white image compared to a black image)
        /// </summary>
        /// <param name="histogram">the histogram to compare this one to</param>
        /// <returns>A percentage which tells how different the two histograms are</returns>
        public float GetVariance()
        {
            //
            double diffRed = 0, diffGreen = 0, diffBlue = 0;
            for (int i = 0; i < 256; i++)
            {
                diffRed += Math.Pow(Red[i] - Red[i], 2);
                diffGreen += Math.Pow(Green[i] - Green[i], 2);
                diffBlue += Math.Pow(Blue[i] - Blue[i], 2);
            }

            diffRed /= 256;
            diffGreen /= 256;
            diffBlue /= 256;
            const double maxDiff = 512;
            return (float)(diffRed / maxDiff + diffGreen / maxDiff + diffBlue / maxDiff) / 3;
        }
        #endregion

    }
}
