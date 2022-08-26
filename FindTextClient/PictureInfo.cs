// <copyright file="FindText.cs" company="Keith Martin, FeiYue">
// Copyright (c) Keith Martin, FeiYue
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

namespace FindTextClient
{
    public class PictureInfo
    {
        // [v, w, h, len1, len0, e1, e0 , mode, color, n, comment, seterr]
        /// <summary>
        /// The text that represents the graphical image to search for
        /// Stored as a series of characters 1 and 0.
        /// </summary>
        public string V { get; set; }

        /// <summary>
        /// The width of the graphical image to search for
        /// </summary>
        public int W { get; set; }

        /// <summary>
        /// The height of the graphical image to search for
        /// </summary>
        public int H { get; set; }

        /// <summary>
        /// The number of "1"'s within <see cref="V"/>
        /// </summary>
        public int Len1 { get; set; }

        /// <summary>
        /// The number of "0"'s within <see cref="V"/>
        /// </summary>
        public int Len0 { get; set; }

        /// <summary>
        /// Fault Tolerance to Text issues
        /// </summary>
        public int E1 { get; set; }

        /// <summary>
        /// Fault Tolerance to Background issues
        /// </summary>
        public int E0 { get; set; }

        /// <summary>
        /// The type of search that is to be done
        /// </summary>
        public FindText.FindMode Mode { get; set; }

        /// <summary>
        /// The colour that is being looked for
        /// </summary>
        public string Colour { get; set; }

        /// <summary>
        /// Used in working out if a bit is true or false, but not sure what it really is
        /// </summary>
        public uint N { get; set; }

        /// <summary>
        /// The comment that names this item being found.  Useful for OCR, if you have every character in the alphabet as separate find strings
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Seterr { get; set; }

        /// <summary>
        /// Constructor which populates all the parameters
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="len1"></param>
        /// <param name="len0"></param>
        /// <param name="e1"></param>
        /// <param name="e0"></param>
        /// <param name="mode"></param>
        /// <param name="colour"></param>
        /// <param name="n"></param>
        /// <param name="comment"></param>
        /// <param name="seterr"></param>
        public PictureInfo(string v, int w, int h, int len1, int len0, int e1, int e0, FindText.FindMode mode, string colour, uint n, string comment, int seterr)
        {
            this.V = v;
            this.W = w;
            this.H = h;
            this.Len1 = len1;
            this.Len0 = len0;
            this.E1 = e1;
            this.E0 = e0;
            this.Mode = mode;
            this.Colour = colour;
            this.N = n;
            this.Comment = comment;
            this.Seterr = seterr;
        }
    }
}
