// <copyright file="FindText.cs" company="Keith Martin, FeiYue">
// Copyright (c) Keith Martin, FeiYue
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

namespace FindTextClient
{
    /// <summary>
    /// Custom class to hold the results of a successful search
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SearchResult()
        {
            this.Width = 0;
            this.Height = 0;
            this.TopLeftX = 0;
            this.TopLeftY = 0;
            this.X = 0;
            this.Y = 0;
            this.Id = string.Empty;
        }

        /// <summary>
        /// Constructor when you want to initialise the values
        /// </summary>
        /// <param name="x">Top Left Corner X Coord</param>
        /// <param name="y">Top Left Corner Y Coord</param>
        /// <param name="w">Width of found item</param>
        /// <param name="h">Height of found item</param>
        /// <param name="centerX">Center X coord of found item</param>
        /// <param name="centerY">Center Y coord of found item</param>
        /// <param name="comment">The comment from the search string that found this item</param>
        public SearchResult(int x, int y, int w, int h, int centerX, int centerY, string comment)
        {
            TopLeftX = x;
            TopLeftY = y;
            Width = w;
            Height = h;
            X = centerX;
            Y = centerY;
            Id = comment;
        }

        /// <summary>
        /// Top Left X coord 
        /// </summary>
        public int TopLeftX { get; set; }

        /// <summary>
        /// Top Left Y coord
        /// </summary>
        public int TopLeftY { get; set; }

        /// <summary>
        /// Width of found item
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of found item
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Centre X coord of the found item
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Centre Y Coord of the found item
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Comment string from the FindString that located this item
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Convert the data into a human readable string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Rectangle ({0}, {1})-({2}, {3}), Center ({4}, {5}), id = {6}", TopLeftX, TopLeftY, Width, Height, X, Y, Id);
        }

    }
}
