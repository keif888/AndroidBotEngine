// <copyright file="FindText.cs" company="Keith Martin, FeiYue">
// Copyright (c) Keith Martin, FeiYue
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using System;


namespace FindTextClient
{
    /// <summary>
    /// Class to store the image that is being searched
    /// </summary>
    public class StoredScreenShot
    {
        /// <summary>
        /// A handle to the compatible bitmap (DDB)
        /// </summary>
        public IntPtr HBM { get; set; }
        /// <summary>
        /// X coordinate of the bitmap
        /// </summary>
        public int Zx { get; set; }
        /// <summary>
        /// Y coordinate of the bitmap
        /// </summary>
        public int Zy { get; set; }
        /// <summary>
        /// Height of the bitmap
        /// </summary>
        public int Zh { get; set; }
        /// <summary>
        /// Width of the bitmap
        /// </summary>
        public int Zw { get; set; }
        /// <summary>
        /// The number of bits needed for a single row in the bitmap
        /// </summary>
        public int Stride { get; set; }
        /// <summary>
        /// Pointer to the bit values within the bitmap pointed to by HBM
        /// </summary>
        public IntPtr Scan0 { get; set; }
        /// <summary>
        /// Old Width, used if ingesting a new image, so upsizing can be done when required
        /// </summary>
        public int Oldzw { get; internal set; }
        /// <summary>
        /// Old Height, used if ingesting a new image, so upsizing can be done when required
        /// </summary>
        public int Oldzh { get; internal set; }

        /// <summary>
        /// Creator for StoredScreenShot
        /// </summary>
        public StoredScreenShot()
        {
            HBM = IntPtr.Zero;
            Scan0 = IntPtr.Zero;
        }

        /// <summary>
        /// Destructor for StoredScreenShot.  Ensure that the HBM from GDI is deleted on cleanup
        /// </summary>
        ~StoredScreenShot()
        {
            if (HBM != IntPtr.Zero)
            {
                GDIFunctions.DeleteObject(HBM);
                HBM = IntPtr.Zero;
            }
        }

    }
}
