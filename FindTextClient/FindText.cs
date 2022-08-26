// <copyright file="FindText.cs" company="Keith Martin, FeiYue">
// Copyright (c) Keith Martin, FeiYue
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;


namespace FindTextClient
{
    /// <summary>
    /// The following class is based on the Autohotkey FindText class.
    /// As documented here:
    /// 
    ///
    ////;===========================================
    ////;  FindText - Capture screen image into text and then find it
    ////;  https://autohotkey.com/boards/viewtopic.php?f=6&t=17834
    ////;
    ////;  Author  : FeiYue
    ////;  Version : 8.5
    ////;  Date    : 2021-04-22
    ////;
    ////;===========================================
    ////;
    /// 
    /// </summary>
    public class FindText
    {

        [DllImport(@"FindTextDLL.dll", EntryPoint = "PicFindC", CallingConvention = CallingConvention.StdCall)]
        public static extern int PicFindC(
        int mode, uint c, uint n, int dir
        , IntPtr Bmp, int Stride, int zw, int zh
        , int sx, int sy, int sw, int sh
        , sbyte[] ss, uint[] s1, uint[] s0
        , string text, int w, int h, int err1, int err0
        , uint[] allpos, int allpos_max);





        /// <summary>
        /// Constructor for FindText, sets all the defaults, and creates the Regex's used
        /// </summary>
        public FindText()
        {
            bind = new BindType();
            bits = new StoredScreenShot();
            infoDictionary = new Dictionary<string, PictureInfo>();
            regexComment = new Regex(@"<([^>]*)>");
            regexSquareBrackets = new Regex(@"\[([^\]]*)]");
            regexColourClean = new Regex(@"[*#\s]");
            regexFileMatch = new Regex(@"[^\s\w/]");
            regexWhiteSpace = new Regex(@"\s");
            Cursor = IntPtr.Zero;
        }

        // Destructor for class to ensure that hBM is deleted.
        ~FindText()
        {
            if (bits.HBM != IntPtr.Zero)
            {
                GDIFunctions.DeleteObject(bits.HBM);
                bits.HBM = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Declare class global variables
        /// </summary>
        private readonly StoredScreenShot bits;
        private readonly Dictionary<string, PictureInfo> infoDictionary;
        private readonly BindType bind;
        private readonly Regex regexComment;
        private readonly Regex regexSquareBrackets;
        private readonly Regex regexColourClean;
        private readonly Regex regexFileMatch;
        private readonly Regex regexWhiteSpace;
        private IntPtr Cursor;

        /// <summary>
        /// Describes the 6 types of matching that are supported
        /// </summary>
        public enum FindMode : int
        {
            colourMode = 0, // 0
            greyThresholdMode, // 1
            greyDifferenceMode, // 2
            colourPositionMode, // 3
            colourDifferenceMode, // 4
            multiColourMode // 5
        }

        // 1 ==> Top to Bottom ( Left to Right )
        // 2 ==> Top to Bottom ( Right to Left )
        // 3 ==> Bottom to Top ( Left to Right )
        // 4 ==> Bottom to Top ( Right to Left )
        // 5 ==> Left to Right ( Top to Bottom )
        // 6 ==> Left to Right ( Bottom to Top )
        // 7 ==> Right to Left ( Top to Bottom )
        // 8 ==> Right to Left ( Bottom to Top )

        /// <summary>
        /// Describes the 8 directions that are used to search for content in images
        /// </summary>
        public enum FindDirection : int
        {
            topToBottomLeftToRight = 1,
            topToBottomRightToLeft,
            bottomToTopLeftToRight,
            bottomToTopRightToLeft,
            leftToRightTopToBottom,
            leftToRightBottomToTop,
            rightToLeftTopToBottom,
            rightToLeftBottomToTop,
            center
        }

        //public static List<SearchResult> SearchText(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, float faultToleranceText, float faultToleranceBackground, string searchText)
        //{
        //    return SearchText(topLeftX, topLeftY, bottomRightX, bottomRightY, faultToleranceText, faultToleranceBackground, searchText, true, true, false, 20, 10, FindDirection.topToBottomLeftToRight);
        //}

        //public static List<SearchResult> SearchText(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, float faultToleranceText, float faultToleranceBackground, string searchText, bool takeScreenShot)
        //{
        //    return SearchText(topLeftX, topLeftY, bottomRightX, bottomRightY, faultToleranceText, faultToleranceBackground, searchText, takeScreenShot, true, false, 20, 10, FindDirection.topToBottomLeftToRight);
        //}

        //public static List<SearchResult> SearchText(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, float faultToleranceText, float faultToleranceBackground, string searchText, bool takeScreenShot, bool findAll)
        //{
        //    return SearchText(topLeftX, topLeftY, bottomRightX, bottomRightY, faultToleranceText, faultToleranceBackground, searchText, takeScreenShot, findAll, false, 20, 10, FindDirection.topToBottomLeftToRight);
        //}

        //public static List<SearchResult> SearchText(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, float faultToleranceText, float faultToleranceBackground, string searchText, bool takeScreenShot, bool findAll, bool joinText)
        //{
        //    return SearchText(topLeftX, topLeftY, bottomRightX, bottomRightY, faultToleranceText, faultToleranceBackground, searchText, takeScreenShot, findAll, joinText, 20, 10, FindDirection.topToBottomLeftToRight);
        //}

        public List<SearchResult>? SearchText(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, float faultToleranceText, float faultToleranceBackground, string searchText, bool takeScreenShot, bool findAll, bool joinText, int? offsetXNullable, int? offsetYNullable)
        {
            int offsetX = 20, offsetY = 10;
            if (offsetXNullable != null)
            {
                offsetX = (int)offsetXNullable;
            }
            if (offsetYNullable != null)
            { 
                offsetY = (int)offsetYNullable;
            }    
            return SearchText(topLeftX, topLeftY, bottomRightX, bottomRightY, faultToleranceText, faultToleranceBackground, searchText, takeScreenShot, findAll, joinText, offsetX, offsetY, FindDirection.topToBottomLeftToRight);
        }


        //FindText(x1:=0, y1:=0, x2:=0, y2:=0, err1:=0, err0:=0
        //  , text:="", ScreenShot:=1, FindAll:=1
        //  , JoinText:=0, offsetX:=20, offsetY:=10, dir:=1)
        /// <summary>
        /// Searches for the image that searchText represents within an already loaded image, or on the screen 
        /// </summary>
        /// <param name="topLeftX">Top Left X of the bounding box within the image to search</param>
        /// <param name="topLeftY">Top Left Y of the bounding box within the image to search</param>
        /// <param name="bottomRightX">Bottom Right X of the bounding box within the image to search</param>
        /// <param name="bottomRightY">Bottom Right Y of the bounding box within the image to search</param>
        /// <param name="faultToleranceText">The percentage of errors that are allowed within the foreground</param>
        /// <param name="faultToleranceBackground">The percentage of errors that are allowed within the background</param>
        /// <param name="searchText">The text string that describes what to search for.  This is an encoded string.</param>
        /// <param name="takeScreenShot">Set to true, to take a screen shot before searching it.  False to use one loaded before.</param>
        /// <param name="findAll">True to find all occurences of the searchText within the bounding box</param>
        /// <param name="joinText">True to indicate that the searchText contains multiple strings that join to make one image</param>
        /// <param name="offsetX">Used to prevent overlaps of searched images?</param>
        /// <param name="offsetY">Used to prevent overlaps of searched images?</param>
        /// <param name="direction">The direction to run the search</param>
        /// <returns></returns>
        public List<SearchResult>? SearchText(int topLeftX = 0, int topLeftY = 0, int bottomRightX = 0, int bottomRightY = 0,
            float faultToleranceText = 0.0f, float faultToleranceBackground = 0.0f, string searchText = "",
            bool takeScreenShot = true, bool findAll = true, bool joinText = false, int offsetX = 20, int offsetY = 10, FindDirection direction = FindDirection.topToBottomLeftToRight)
        {
            //            local
            //SetBatchLines, % (bch:= A_BatchLines) ? "-1" : "-1"
            //  centerX:= Round(x1 + x2)//2, centerY:=Round(y1+y2)//2
            int centerX = (int)Math.Floor(Math.Round((topLeftX + bottomRightX) / 2.0));
            int centerY = (int)Math.Floor(Math.Round((topLeftY + bottomRightY) / 2.0));
            //  if (x1 * x1 + y1 * y1 + x2 * x2 + y2 * y2 <= 0)
            //            n:= 150000, x:= y:= -n, w:= h:= 2 * n
            //  else
            //            x:= Min(x1, x2), y:= Min(y1, y2), w:= Abs(x2 - x1) + 1, h:= Abs(y2 - y1) + 1
            int n, x, y, w, h;
            if ((topLeftX * topLeftX) + (topLeftY * topLeftY) + (bottomRightX * bottomRightX) + (bottomRightY * bottomRightY) <= 0)
            {
                n = 150000;
                x = -n;
                y = x;
                w = 2 * n;
                h = w;
            }
            else
            {
                x = Math.Min(topLeftX, bottomRightX);
                y = Math.Min(topLeftY, bottomRightY);
                w = Math.Abs(bottomRightX - topLeftX) + 1;
                h = Math.Abs(bottomRightY - topLeftY) + 1;
            }

            //  bits:= this.GetBitsFromScreen(x, y, w, h, ScreenShot, zx, zy, zw, zh)
            //  , info:=[]

            int zx = 0, zy = 0, zw = 0, zh = 0;
            StoredScreenShot localBits = GetBitsFromScreen(ref x, ref y, ref w, ref h, takeScreenShot, ref zx, ref zy, ref zw, ref zh);
            List<PictureInfo> info = new List<PictureInfo>();

            //  Loop, Parse, text, |
            //    if IsObject(j:= this.PicInfo(A_LoopField))
            //      info.Push(j)

            string[] splitText = searchText.Split("|");
            foreach (string item in splitText)
            {
                PictureInfo? tempItem = PicInfo(item);
                if (tempItem != null)
                {
                    info.Add(tempItem);
                }
            }

            //  if (w < 1 or h<1 or!(num:= info.MaxIndex()) or!bits.Scan0)
            //  {
            //                SetBatchLines, % bch %
            //    return 0
            //  }
            int num = info.Count;
            if (w < 1 || h < 1 || num == 0 || localBits.Scan0 == null)
            {
                return null;
            }

            //        arr:=[], in:={
            //            zx: zx, zy: zy, zw: zw, zh: zh
            //          , sx: x - zx, sy: y - zy, sw: w, sh: h}, k:= 0

            List<SearchResult> arr = new List<SearchResult>();
            PicFindInput picFindInput = new PicFindInput(zx, zy, zw, zh, x - zx, y - zy, w, h, string.Empty);
            int k = 0;

            //  For i, j in info
            //     k:= Max(k, j.2 * j.3), in.comment.= j.11

            foreach (PictureInfo item in info)
            {
                k = Math.Max(k, item.W * item.H);
                picFindInput.Comment += item.Comment;
            }

            //  VarSetCapacity(s1, k * 4), VarSetCapacity(s0, k * 4)
            //  , VarSetCapacity(ss, 2 * (w + 2) * (h + 2))
            //  , FindAll:= (dir = 9 ? 1 : FindAll)
            //  , JoinText:= (num = 1 ? 0 : JoinText)
            //  , allpos_max:= (FindAll or JoinText ? 10240 : 1)
            //  , VarSetCapacity(allpos, allpos_max * 8)

            uint[] s0 = new uint[k * 4];
            uint[] s1 = new uint[k * 4];
            sbyte[] ss = new sbyte[2 * (w + 2) * (h + 2)];
            if (direction == FindDirection.center) findAll = true;
            if (num == 1) joinText = false;
            int allpos_max = findAll || joinText ? 10240 : 1;
            List<GDIFunctions.POINT> allpos = new List<GDIFunctions.POINT>();

            //  Loop, 2
            //  {
            //                if (err1 = 0 and err0 = 0) and(num > 1 or A_Index > 1)
            //      err1:= 0.05, err0:= 0.05
            //    Loop, % JoinText ? 1 : num
            //    {
            //                    this.PicFind(arr, in, info, A_Index, err1, err0
            //                      , FindAll, JoinText, offsetX, offsetY, dir
            //                      , bits, ss, s1, s0, allpos, allpos_max)
            //      if (!FindAll and arr.MaxIndex())
            //        Break
            //    }
            //                if (err1 != 0 or err0!= 0 or arr.MaxIndex() or info.1.12)
            //      Break
            //  }

            for (int iCount = 1; iCount < 3; iCount++)
            {
                if ((Math.Abs(faultToleranceText) < 0.001 && Math.Abs(faultToleranceBackground) < 0.001) && (num > 1 || iCount > 1))
                {
                    faultToleranceText = 0.05f;
                    faultToleranceBackground = 0.05f;
                }
                int numberOfLoops = joinText ? 1 : num;
                for (int iCount2 = 0; iCount2 < numberOfLoops; iCount2++)
                {
                    PicFind(arr, picFindInput, info, iCount2, faultToleranceText, faultToleranceBackground, findAll, joinText, offsetX, offsetY, direction, localBits, ref ss, ref s1, ref s0, ref allpos, allpos_max);
                    if (arr.Count >= allpos_max)
                        break;
                }
                if (faultToleranceText != 0 || faultToleranceBackground != 0 || arr.Count == 0 || info[0].Seterr != 0)
                {
                    break;
                }
            }

            //            if (dir = 9)
            //            arr:= this.Sort2(arr, centerX, centerY)

            if (direction == FindDirection.center)
            {
                arr = DistanceSort(arr, centerX, centerY);
            }

            //  SetBatchLines, % bch %
            //  return arr.MaxIndex() ? arr : 0

            if (arr.Count == 0)
                return null;
            else
                return arr;
        }

        public OCRResults OCR(List<SearchResult> searchResults, int offsetX = 20, int offsetY = 20, int overlapW = 0)
        {
            int x, y, xw, minX = 0, maxX = 0, minY = 0, maxY = 0, leftX, leftY = 0, leftH = 0, leftW = 0, ocrX = int.MinValue, ocrY = int.MinValue, dx = int.MinValue;
            string leftOCR = string.Empty, ocrText = string.Empty;

            int index = 0;
            foreach (SearchResult item in searchResults)
            {
                x = item.TopLeftX;
                xw = x + item.Width;
                minX = index == 0 || x < minX ? x : minX;
                maxX = index == 0 || xw > maxX ? xw : maxX;
                index++;
            }

            while (index > 0 && minX <= maxX)
            {
                leftX = int.MinValue;
                foreach (SearchResult item in searchResults)
                {
                    x = item.TopLeftX;
                    y = item.TopLeftY;
                    if (x < minX || (ocrY > int.MinValue && Math.Abs(y - ocrY) > offsetY))
                        continue;
                    if (leftX == int.MinValue || x < leftX)
                    {
                        leftX = x;
                        leftY = y;
                        leftW = item.Width;
                        leftH = item.Height;
                        leftOCR = item.Id;
                    }
                }
                if (leftX == int.MinValue)
                    break;
                if (ocrX == int.MinValue)
                {
                    ocrX = leftX;
                    minY = leftY;
                    maxY = leftY + leftH;
                }
                ocrText += ocrText.Length > 0 && leftX > dx ? "*" : "";
                ocrText += leftOCR;
                minX = leftX + leftW - (overlapW > leftW / 2 ? leftW / 2 : overlapW);
                dx = leftX + leftW + offsetX;
                ocrY = leftY;
                if (leftY < minY) minY = leftY;
                if (leftY + leftH > maxY) maxY = leftY + leftH;
            }
            return new OCRResults(ocrText, ocrX, minY, minX - ocrX, maxY - minY);
        }




        private static void PicFind(List<SearchResult> arr, PicFindInput picFindInput, List<PictureInfo> info, int index, float err1, float err0, bool findAll, bool joinText, int offsetX, int offsetY, FindDirection dir, StoredScreenShot bits, ref sbyte[] ss, ref uint[] s1, ref uint[] s0, ref List<GDIFunctions.POINT> allpos, int allpos_max)
        {
            int x, y;
            //num:=info.MaxIndex(), j:=info[index]
            //, text:=j.1, w:=j.2, h:=j.3
            //, e1:=(!j.12 ? Floor(j.4*err1) : j.6)
            //, e0:=(!j.12 ? Floor(j.5*err0) : j.7)
            //, mode:=j.8, color:=j.9, n:=j.10, comment:=j.11
            //, sx:=in.sx, sy:=in.sy, sw:=in.sw, sh:=in.sh
            int num = info.Count;
            PictureInfo j = info[index];
            string text = j.V;
            int w = j.W, h = j.H;
            int e1 = (j.Seterr == 0 ? (int)Math.Floor(j.Len1 * err1) : j.E1);
            int e0 = (j.Seterr == 0 ? (int)Math.Floor(j.Len0 * err0) : j.E0);
            FindMode mode = j.Mode;
            string colour = j.Colour;
            uint n = j.N;
            string comment = j.Comment;
            int sx = picFindInput.Sx;
            int sy = picFindInput.Sy;
            int sw = picFindInput.Sw;
            int sh = picFindInput.Sh;

            //if (JoinText and index>1)
            if (joinText && index > 1)
            {
                //{
                //  x:=in.x, y:=in.y, sw:=Min(x+offsetX+w,sx+sw), sx:=x, sw-=sx
                //  , sh:=Min(y+offsetY+h,sy+sh), sy:=Max(y-offsetY,sy), sh-=sy
                x = picFindInput.X; y = picFindInput.Y; sw = Math.Min(x + offsetX + w, sx + sw); sx = x; sw -= sx;
                sh = Math.Min(y + offsetY + h, sy + sh); sy = Math.Max(y - offsetY, sy); sh -= sy;
                //}
            }
            //if (mode=3)
            //  color:=(color//w)*bits.Stride+Mod(color,w)*4
            if (mode == FindMode.colourPositionMode)
                colour = ((Int32.Parse(colour) / w) * bits.Stride + (Int32.Parse(colour) % w) * 4).ToString();
            //ok:=!bits.Scan0 ? 0 : DllCall(&MyFunc
            //  , "int",mode, "uint",color, "uint",n, "int",dir
            //  , "Ptr",bits.Scan0, "int",bits.Stride
            //  , "int",in.zw, "int",in.zh
            //  , "int",sx, "int",sy, "int",sw, "int",sh
            //  , "Ptr",&ss, "Ptr",&s1, "Ptr",&s0
            //  , "AStr",text, "int",w, "int",h, "int",e1, "int",e0
            //  , "Ptr",&allpos, "int",allpos_max)

            uint[] allpos2 = new uint[allpos_max * 2];  // Make sure to allocate space for X and Y.
            if (!uint.TryParse(colour, out uint parsedColour))
            {
                try
                {
                    parsedColour = Convert.ToUInt32(colour, 16);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to parse colour from {0} with exception {1}", colour, ex.Message));
                }
            }
            int ok = PicFindC((int)mode, parsedColour, n, (int)dir, bits.Scan0, bits.Stride, picFindInput.Zw, picFindInput.Zh, sx, sy, sw, sh, ss, s1, s0, text, w, h, e1, e0, allpos2, allpos_max);

            //pos:=[]
            //Loop, % ok
            //  pos[A_Index]:=NumGet(allpos, 8*A_Index-8, "uint")
            //    | NumGet(allpos, 8*A_Index-4, "uint")<<32
            for (int i = 0; i < ok; i++)
            {
                GDIFunctions.POINT found = new GDIFunctions.POINT
                {
                    x = (int)allpos2[i * 2],
                    y = (int)allpos2[(i * 2) + 1]
                };
                allpos.Add(found);
            }

            //Loop, % ok
            int currentPos = arr.Count;
            for (int i = currentPos; i < ok + currentPos; i++)
            {
                //{
                //  x:=pos[A_Index]&0xFFFFFFFF, y:=pos[A_Index]>>32
                x = allpos[i].x;
                y = allpos[i].y;
                //  if (!JoinText)
                //  {
                if (!joinText)
                {
                    //    x1:=x+in.zx, y1:=y+in.zy
                    //    , arr.Push( {1:x1, 2:y1, 3:w, 4:h
                    //    , x:x1+w//2, y:y1+h//2, id:comment} )
                    //  }
                    int x1 = x + picFindInput.Zx, y1 = y + picFindInput.Y;
                    arr.Add(new SearchResult(x1, y1, w, h, x1 + (w / 2), y1 + (h / 2), comment));
                }
                //  else if (index=1)
                else if (index == 1)
                {
                    //  {
                    //    in.x:=x+w, in.y:=y, in.minY:=y, in.maxY:=y+h
                    picFindInput.X = x + w; picFindInput.Y = y; picFindInput.MinY = y; picFindInput.MaxY = y + h;
                    //    Loop, % num-1
                    for (int iCounter = 1; iCounter < num - 1; iCounter++)
                    {
                        // We have dynamic growable list, so no need to exit when the list is full
                        //      if !this.PicFind(arr, in, info, A_Index+1, err1, err0
                        //      , FindAll, JoinText, offsetX, offsetY, 5
                        //      , bits, ss, s1, s0, allpos, 1)
                        //        Continue, 2
                        //    x1:=x+in.zx, y1:=in.minY+in.zy
                        //    , w1:=in.x-x, h1:=in.maxY-in.minY
                        //    , arr.Push( {1:x1, 2:y1, 3:w1, 4:h1
                        //    , x:x1+w1//2, y:y1+h1//2, id:in.comment} )
                        PicFind(arr, picFindInput, info, iCounter, err1, err0, findAll, joinText, offsetX, offsetY, FindDirection.leftToRightTopToBottom, bits, ref ss, ref s1, ref s0, ref allpos, 1);
                        int x1 = x + picFindInput.Zx, y1 = picFindInput.MinY + picFindInput.Zy,
                        w1 = picFindInput.X - x, h1 = picFindInput.MaxY - picFindInput.MinY;
                        arr.Add(new SearchResult(x1, y1, w1, h1, x1 + (w1 / 2), y1 + (h1 / 2), picFindInput.Comment));
                    }
                    //  }
                }
                else
                {
                    //  else
                    //  {
                    //    in.x:=x+w, in.y:=y
                    //    , (y<in.minY && in.minY:=y)
                    //    , (y+h>in.maxY && in.maxY:=y+h)
                    //    return 1
                    //  }
                    picFindInput.X = x + w; picFindInput.Y = y;
                    picFindInput.MinY = Math.Min(y, picFindInput.MinY);
                    picFindInput.MaxY = Math.Max(y + h, picFindInput.MaxY);
                    return;
                }
                //  if (!FindAll and arr.MaxIndex())
                //    return
                //}
            }
            return;
        }

        public void LoadBitmap(Bitmap bitmap, ref int zx, ref int zy, ref int w, ref int h)
        {
            IntPtr hBM2 = bitmap.GetHbitmap();
            int x = 0, y = 0, zw = 0, zh = 0;
            _ = GetBitsFromScreen(ref x, ref y, ref w, ref h, true, ref zx, ref zy, ref zw, ref zh);

            IntPtr hBM = bits.HBM;
            GetBitmapWH(hBM2, ref w, ref h);
            CopyHBM(hBM, 0, 0, hBM2, 0, 0, w, h);
            GDIFunctions.DeleteObject(hBM2);
            return;
        }

        public void LoadImage(Image image, ref int zx, ref int zy, ref int w, ref int h)
        {
            Bitmap loadedBitmap = new Bitmap(image);
            LoadBitmap(loadedBitmap, ref zx, ref zy, ref w, ref h);
            loadedBitmap.Dispose();
            return;
        }

        public void LoadFile(string file, ref int zx, ref int zy, ref int w, ref int h)
        {
            if (file.Length == 0 || !File.Exists(file))
            {
                return;
            }
            Bitmap loadedBitmap = new Bitmap(file);
            LoadBitmap(loadedBitmap, ref zx, ref zy, ref w, ref h);
            loadedBitmap.Dispose();
            return;
        }


        private int GetBitmapWH(IntPtr hBM, ref int w, ref int h)
        {
            GDIFunctions.BITMAP bm = new GDIFunctions.BITMAP();
            int r = GDIFunctions.GetObject(hBM, Marshal.SizeOf(bm), ref bm);
            w = bm.bmWidth;
            h = Math.Abs(bm.bmHeight);
            return r;
        }

        private StoredScreenShot GetBitsFromScreen(ref int x, ref int y, ref int w, ref int h, bool takeScreenShot, ref int zx, ref int zy, ref int zw, ref int zh)
        {
            //          local
            //static Ptr:= "Ptr"
            //bits:= this.bits
            //if (!ScreenShot)
            //          {
            //          zx:= bits.zx, zy:= bits.zy, zw:= bits.zw, zh:= bits.zh
            //  if IsByRef(x)
            //    w:= Min(x + w, zx + zw), x:= Max(x, zx), w -= x
            //    , h:= Min(y + h, zy + zh), y:= Max(y, zy), h -= y
            //  return bits
            //}
            if (!takeScreenShot)
            {
                zx = bits.Zx; zy = bits.Zy; zw = bits.Zw; zh = bits.Zh;
                w = Math.Min(x + w, zx + zw); x = Math.Max(x, zx); w -= x;
                h = Math.Min(y + h, zy + zh); y = Math.Max(y, zy); h -= y;
                return bits;
            }

            //      bch:= A_BatchLines, cri:= A_IsCritical
            //Critical
            //if (id:= this.BindWindow(0, 0, 1))
            IntPtr id = (IntPtr)BindWindow(IntPtr.Zero, 0, true);
            if (id != IntPtr.Zero)
            {
                GDIFunctions.Rect rectangle = default;
                //{
                //              WinGet, id, ID, ahk_id % id %
                //              WinGetPos, zx, zy, zw, zh, ahk_id % id %
                //}
                GDIFunctions.GetWindowRect(id, ref rectangle);
                zx = rectangle.Left;
                zy = rectangle.Top;
                zw = rectangle.Right;
                zh = rectangle.Bottom;
            }
            //          if (!id)
            //          {
            else
            {
                //              SysGet, zx, 76
                //            SysGet, zy, 77
                //            SysGet, zw, 78
                //            SysGet, zh, 79
                zx = System.Windows.Forms.SystemInformation.VirtualScreen.X;
                zy = System.Windows.Forms.SystemInformation.VirtualScreen.Y;
                zw = System.Windows.Forms.SystemInformation.VirtualScreen.Width;
                zh = System.Windows.Forms.SystemInformation.VirtualScreen.Height;
                //          }
            }
            // bits.zx:= zx, bits.zy:= zy, bits.zw:= zw, bits.zh:= zh
            //, w:= Min(x + w, zx + zw), x:= Max(x, zx), w -= x
            //, h:= Min(y + h, zy + zh), y:= Max(y, zy), h -= y
            bits.Zx = zx; bits.Zy = zy; bits.Zw = zw; bits.Zh = zh;
            w = Math.Min(x + w, zx + zw); x = Math.Max(x, zx); w -= x;
            h = Math.Min(y + h, zy + zh); y = Math.Max(y, zy); h -= y;
            //if (zw > bits.oldzw or zh> bits.oldzh or!bits.hBM)
            if (zw > bits.Oldzw || zh > bits.Oldzh || bits.HBM != IntPtr.Zero)
            {
                //{
                //          hBM:= bits.hBM
                IntPtr hBM = bits.HBM;
                //  , bits.hBM:= this.CreateDIBSection(zw, zh, bpp:= 32, ppvBits)
                IntPtr ppvBits = IntPtr.Zero;
                short bpp = 32;
                bits.HBM = InternalCreateDIBSection(zw, zh, bpp, ref ppvBits, out _);
                //  , bits.Scan0:= (!bits.hBM ? 0 : ppvBits)
                bits.Scan0 = bits.HBM == IntPtr.Zero ? IntPtr.Zero : ppvBits;
                //  , bits.Stride:= ((zw * bpp + 31)//32)*4
                bits.Stride = ((zw * bpp + 31) / 32) * 4;
                //  , bits.oldzw:= zw, bits.oldzh:= zh
                bits.Oldzw = zw; bits.Oldzh = zh;
                //  , DllCall("DeleteObject", Ptr, hBM)
                GDIFunctions.DeleteObject(hBM);
                //}
            }
            //          if (w < 1 or h<1 or!bits.hBM)
            if (w < 1 || h < 1 || bits.HBM != IntPtr.Zero)
            {
                //{
                //              Critical, % cri %
                //              SetBatchLines, % bch %
                //  return bits
                //}
                return bits;
            }
            // This can't be true, as GetBitsFromScreen2 doesn't exist in code base
            //          if IsFunc(k:= "GetBitsFromScreen2")
            //            and % k % (bits, x - zx, y - zy, w, h)
            //{
            //          zx:= bits.zx, zy:= bits.zy, zw:= bits.zw, zh:= bits.zh
            //  Critical, % cri %
            //  SetBatchLines, % bch %
            //  return bits
            //}
            //      mDC:= DllCall("CreateCompatibleDC", Ptr, 0, Ptr)
            IntPtr mDC = GDIFunctions.CreateCompatibleDC(IntPtr.Zero);
            //oBM:= DllCall("SelectObject", Ptr, mDC, Ptr, bits.hBM, Ptr)
            IntPtr oBM = GDIFunctions.SelectObject(mDC, bits.HBM);
            //if (id)
            if (id != IntPtr.Zero)
            {
                //          {
                //              if (mode:= this.BindWindow(0, 0, 0, 1))< 2
                long mode = BindWindow(IntPtr.Zero, 0, false, true);
                if (mode < 2)
                {
                    //  {
                    //              hDC2:= DllCall("GetDCEx", Ptr, id, Ptr, 0, "int", 3, Ptr)
                    IntPtr hDC2 = GDIFunctions.GetDCEx(id, IntPtr.Zero, GDIFunctions.DeviceContextValues.Window & GDIFunctions.DeviceContextValues.Cache);
                    //    DllCall("BitBlt", Ptr, mDC, "int", x - zx, "int", y - zy, "int", w, "int", h
                    //      , Ptr, hDC2, "int", x - zx, "int", y - zy, "uint", 0xCC0020 | 0x40000000)
                    GDIFunctions.BitBlt(mDC, x - zx, y - zy, w, h, hDC2, x - zx, y - zy, GDIFunctions.TernaryRasterOperations.CAPTUREBLT | GDIFunctions.TernaryRasterOperations.SRCCOPY);
                    //    DllCall("ReleaseDC", Ptr, id, Ptr, hDC2)
                    GDIFunctions.ReleaseDC(id, hDC2);
                    //  }
                }
                //  else
                else
                {
                    // {
                    //    hBM2:= this.CreateDIBSection(zw, zh)
                    IntPtr hBM2 = InternalCreateDIBSection(zw, zh);
                    //    mDC2:= DllCall("CreateCompatibleDC", Ptr, 0, Ptr)
                    IntPtr mDC2 = GDIFunctions.CreateCompatibleDC(IntPtr.Zero);
                    //    oBM2:= DllCall("SelectObject", Ptr, mDC2, Ptr, hBM2, Ptr)
                    IntPtr oBM2 = GDIFunctions.SelectObject(mDC2, hBM2);
                    //    DllCall("PrintWindow", Ptr, id, Ptr, mDC2, "uint", (mode > 3) * 3)
                    GDIFunctions.PrintWindow(id, mDC2, (uint)((mode > 3) ? 0 : 3));
                    //    DllCall("BitBlt", Ptr, mDC, "int", x - zx, "int", y - zy, "int", w, "int", h
                    //      , Ptr, mDC2, "int", x - zx, "int", y - zy, "uint", 0xCC0020 | 0x40000000)
                    GDIFunctions.BitBlt(mDC, x - zx, y - zy, w, h, mDC2, x - zx, y - zy, GDIFunctions.TernaryRasterOperations.SRCCOPY | GDIFunctions.TernaryRasterOperations.CAPTUREBLT);
                    //    DllCall("SelectObject", Ptr, mDC2, Ptr, oBM2)
                    GDIFunctions.SelectObject(mDC2, oBM2);
                    //    DllCall("DeleteDC", Ptr, mDC2)
                    GDIFunctions.DeleteDC(mDC2);
                    //    DllCall("DeleteObject", Ptr, hBM2)
                    GDIFunctions.DeleteObject(hBM2);
                    //  }
                }
                //          }
            }
            //          else
            else
            {
                //          {
                //          win:= DllCall("GetDesktopWindow", Ptr)
                IntPtr win = GDIFunctions.GetDesktopWindow();
                //            hDC:= DllCall("GetWindowDC", Ptr, win, Ptr)
                IntPtr hDC = GDIFunctions.GetWindowDC(win);
                //            DllCall("BitBlt", Ptr, mDC, "int", x - zx, "int", y - zy, "int", w, "int", h
                //    , Ptr, hDC, "int", x, "int", y, "uint", 0xCC0020 | 0x40000000)
                GDIFunctions.BitBlt(mDC, x - zx, y - zy, w, h, hDC, x - zx, y - zy, GDIFunctions.TernaryRasterOperations.SRCCOPY | GDIFunctions.TernaryRasterOperations.CAPTUREBLT);
                //            DllCall("ReleaseDC", Ptr, win, Ptr, hDC)
                GDIFunctions.ReleaseDC(win, hDC);
                //          }
            }
            //          if this.CaptureCursor(0, 0, 0, 0, 0, 1)
            //            this.CaptureCursor(mDC, zx, zy, zw, zh)
            if (CaptureCursor(IntPtr.Zero, 0, 0, 0, 0, true) != IntPtr.Zero)
                CaptureCursor(mDC, zx, zy, zw, zh);
            //DllCall("SelectObject", Ptr, mDC, Ptr, oBM)
            GDIFunctions.SelectObject(mDC, oBM);
            //DllCall("DeleteDC", Ptr, mDC)
            GDIFunctions.DeleteDC(mDC);
            //Critical, % cri %
            //SetBatchLines, % bch %
            //return bits
            return bits;
        }



        // Use FindText.CaptureCursor(0,0,0,0,0,true) to Capture Cursor
        // Use FindText.CaptureCursor(0,0,0,0,0,false) to Cancel Capture Cursor

        //CaptureCursor(hDC:=0, zx:=0, zy:=0, zw:=0, zh:=0, get_cursor:=0)
        IntPtr CaptureCursor(IntPtr hDC, int zx, int zy, int zw, int zh, bool get_cursor = false)
        {
            //{
            //  local
            //  if (get_cursor)
            //    return this.Cursor
            if (get_cursor)
                return this.Cursor;

            //  if (hDC=1 or hDC=0) and (zw=0)
            //  {
            //    this.Cursor:=hDC
            //    return
            //  }
            if (hDC == IntPtr.Zero && zw == 0)
            {
                this.Cursor = hDC;
                return hDC;
            }


            GDIFunctions.CURSORINFO pci = default;
            //  Ptr:=(A_PtrSize ? "Ptr":"UInt"), PtrSize:=(A_PtrSize=8 ? 8:4)
            //  VarSetCapacity(mi, 40, 0), NumPut(16+PtrSize, mi, "int")
            //  DllCall("GetCursorInfo", Ptr,&mi)
            GDIFunctions.GetCursorInfo(ref pci);
            //  bShow   := NumGet(mi, 4, "int")
            //  hCursor := NumGet(mi, 8, Ptr)
            int bShow = pci.flags;
            IntPtr hCursor = pci.hCursor;
            //  x := NumGet(mi, 8+PtrSize, "int")
            //  y := NumGet(mi, 12+PtrSize, "int")
            int x = pci.ptScreenPos.x;
            int y = pci.ptScreenPos.y;
            //  if (!bShow) or (x<zx or y<zy or x>=zx+zw or y>=zy+zh)
            //    return
            if ((bShow != GDIFunctions.CURSOR_SHOWING) || (x < zx || y < zy || x >= zx + zw || y >= zy + zh))
                return IntPtr.Zero;
            //  VarSetCapacity(ni, 40, 0)
            //  DllCall("GetIconInfo", Ptr,hCursor, Ptr,&ni)
            GDIFunctions.GetIconInfo(hCursor, out GDIFunctions.ICONINFO ni);
            //  xCenter  := NumGet(ni, 4, "int")
            int xCenter = ni.xHotspot;
            //  yCenter  := NumGet(ni, 8, "int")
            int yCenter = ni.yHotspot;
            //  hBMMask  := NumGet(ni, (PtrSize=8?16:12), Ptr)
            IntPtr hBMMask = ni.hbmMask;
            //  hBMColor := NumGet(ni, (PtrSize=8?24:16), Ptr)
            IntPtr hBMColor = ni.hbmColor;
            //  DllCall("DrawIconEx", Ptr,hDC
            //    , "int",x-xCenter-zx, "int",y-yCenter-zy, Ptr,hCursor
            //    , "int",0, "int",0, "int",0, "int",0, "int",3)
            GDIFunctions.DrawIconEx(hDC, x - xCenter - zx, y - yCenter - zy, IntPtr.Zero, 0, 0, 0, IntPtr.Zero, 3);
            //  DllCall("DeleteObject", Ptr,hBMMask)
            GDIFunctions.DeleteObject(hBMMask);
            //  DllCall("DeleteObject", Ptr,hBMColor)
            GDIFunctions.DeleteObject(hBMColor);
            //}
            return hCursor;
        }


        // Bind the window so that it can find images when obscured
        // by other windows, it's equivalent to always being
        // at the front desk.Unbind Window using FindText.BindWindow(0)

        private long BindWindow(IntPtr bind_id, int bind_mode = 0, bool get_id = false, bool get_mode = false)
        {
            //local
            //bind:=this.bind
            //BindType bind = this.bind;
            //if (get_id)
            //return bind.id
            if (get_id)
                return (long)bind.Id;

            //if (get_mode)
            //return bind.mode
            if (get_mode)
                return bind.Mode;

            //if (bind_id)
            //{
            //  bind.id:=bind_id, bind.mode:=bind_mode, bind.oldStyle:=0
            if (bind_id != IntPtr.Zero)
            {
                bind.Id = bind_id;
                bind.Mode = bind_mode;
                bind.OldStyle = 0;
                //  if (bind_mode & 1)
                //  {
                if ((bind_mode & 1) > 0)
                {
                    //    WinGet, oldStyle, ExStyle, ahk_id %bind_id%
                    uint oldStyle = GDIFunctions.GetWindowLong(bind_id, GDIFunctions.GWL_EXSTYLE);
                    //    bind.oldStyle:=oldStyle
                    bind.OldStyle = oldStyle;
                    //    WinSet, Transparent, 255, ahk_id %bind_id%
                    GDIFunctions.SetWindowLong(bind_id, GDIFunctions.GWL_EXSTYLE, 255);
                    GDIFunctions.SetWindowLong(bind_id, GDIFunctions.GWL_EXSTYLE, oldStyle ^ GDIFunctions.WS_EX_LAYERED);
                    GDIFunctions.SetLayeredWindowAttributes(bind_id, 0, 255, GDIFunctions.LWA_ALPHA);
                    //    Loop, 30
                    //    {
                    //      Sleep, 100
                    //      WinGet, i, Transparent, ahk_id %bind_id%
                    //    }
                    //    Until (i=255)
                    for (int k = 0; k < 30; k++)
                    {
                        System.Threading.Thread.Sleep(100);
                        GDIFunctions.GetLayeredWindowAttributes(bind_id, out _, out byte bAlpha, out _);
                        if (bAlpha == 255)
                            break;
                    }
                    //  }
                }
                //}
            }
            //else
            else
            {
                //{
                //bind_id:=bind.id
                //if (bind.mode & 1)
                //    WinSet, ExStyle, % bind.oldStyle, ahk_id %bind_id%
                if ((bind.Mode & 1) > 0)
                {
                    //bind.id:=0, bind.mode:=0, bind.oldStyle:=0
                    //}
                    bind.Id = IntPtr.Zero;
                    bind.Mode = 0;
                    bind.OldStyle = 0;
                }
            }
            return 0;
        }

        private static double GetDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        private static List<SearchResult> DistanceSort(List<SearchResult> dataToSort, int centerX, int centerY)
        {
            List<DistanceType> toSort = new List<DistanceType>();
            List<SearchResult> results = new List<SearchResult>();
            foreach (SearchResult item in dataToSort)
            {
                DistanceType newItem = new DistanceType(item, GetDistance(item.X, item.Y, centerX, centerY));
                toSort.Add(newItem);
            }
            toSort.Sort();
            foreach (DistanceType item in toSort)
            {
                results.Add(item.Point);
            }
            return results;
        }

        /// <summary>
        /// Splits apart a text string.
        /// Text strings have the following format:
        /// | - Starts a new text string
        /// < - Starts a comment
        /// > - Ends a comment
        /// [ - starts an optional Tolerance number
        /// ] - ends an optional Tolerance number
        /// , - indicates that another optional Tolerance Number is on it's way
        /// [ - starts an optional Tolerance number
        /// ] - ends an optional Tolerance number
        /// xx - Colour Mode (* = greyThresholdMode, ** = greyDiferenceMode, ## multiColourMode, etc. None of These indicates colourMode)
        /// nnn - colour (can be hex string with leading 0x, or two hex strings with -, no leading 0x.
        /// $ - start of Text String details
        /// nnn - Width of text string
        /// . - end of width
        /// a-zA-Z0-9 - Base 64 ish text string
        /// 
        /// Please note that in multiColourMode, after the $ may be a file name.
        /// 
        /// </summary>
        /// <param name="text">The text string to understand</param>
        /// <returns></returns>
        public PictureInfo? PicInfo(string text)
        {
            int w = 0, h = 0, len0 = 0, len1 = 0;
            uint n = 0;
            //          local
            //static info:=[], Ptr:= "Ptr"
            PictureInfo? info = null;
            //if !InStr(text, "$")
            //  return
            if (!text.Contains("$"))
                return info;

            //key:= (r:= StrLen(text)) < 1000 ? text
            //  : DllCall("ntdll\RtlComputeCrc32", "uint", 0
            //  , Ptr, &text, "uint", r * (1 + !!A_IsUnicode), "uint")

            int r = text.Length;
            Crc32 crc32 = new Crc32();
            string key = Convert.ToBase64String(crc32.ComputeHash(Encoding.UTF8.GetBytes(text)));

            //if (info[key])
            //              return info[key]
            if (this.infoDictionary.ContainsKey(key))
            {
                return infoDictionary[key];
            }

            //v:= text, comment:= "", seterr:= e1:= e0:= 0
            //    ; You Can Add Comment Text within The <>

            string v = text;
            string comment = string.Empty;
            int seterr = 0, e1 = 0, e0 = 0;

            //if RegExMatch(v, "<([^>]*)>", r)
            //  v:= StrReplace(v, r), comment:= Trim(r1)


            if (regexComment.IsMatch(v))
            {
                MatchCollection matches = regexComment.Matches(v);
                foreach (Match? match in matches)
                {
                    if (match != null)
                    {
                        comment = match.Groups[1].Value.Trim();
                        v = v.Replace(match.Value, "");
                    }
                }
            }

            //  ; You can Add two fault - tolerant in the[], separated by commas
            //if RegExMatch(v, "\[([^\]]*)]", r)
            //{
            //          v:= StrReplace(v, r), r:= StrSplit(r1, ",")
            //  , seterr:= 1, e1:= r.1, e0:= r.2
            //}

            if (regexSquareBrackets.IsMatch(v))
            {
                MatchCollection matches = regexSquareBrackets.Matches(v);
                if (matches.Count == 1)
                {
                    v = v.Replace(matches[0].Value, "");
                    seterr = 1;
                    int.TryParse(matches[0].Groups[0].Value, out e1);
                }
                else
                {
                    v = v.Replace(matches[0].Value + "," + matches[1].Value, "");
                    seterr = 1;
                    int.TryParse(matches[0].Groups[0].Value, out e1);
                    int.TryParse(matches[1].Groups[0].Value, out e0);
                }
            }

            //      color:= StrSplit(v, "$").1, v:= Trim(SubStr(v, InStr(v, "$") + 1))

            string colour = v.Split("$")[0];
            v = v[(v.IndexOf("$") + 1)..].Trim();

            //mode:= InStr(color, "##") ? 5
            //  : InStr(color, "-") ? 4 : InStr(color, "#") ? 3
            //  : InStr(color, "**") ? 2 : InStr(color, "*") ? 1 : 0

            FindMode mode = colour.Contains("##") ? FindMode.multiColourMode
                : colour.Contains("-") ? FindMode.colourDifferenceMode
                : colour.Contains("#") ? FindMode.colourPositionMode
                : colour.Contains("**") ? FindMode.greyDifferenceMode
                : colour.Contains("*") ? FindMode.greyThresholdMode
                : FindMode.colourMode;

            //color:= RegExReplace(color, "[*#\s]")

            colour = regexColourClean.Replace(colour, "");

            //if (mode = 5)
            //          {

            if (mode == FindMode.multiColourMode)
            {
                //              if (v~= "[^\s\w/]") and FileExist(v)  ; ImageSearch
                //              {
                if (regexFileMatch.IsMatch(v) && File.Exists(v))
                {
                    //                  if !(hBM:= LoadPicture(v))
                    //                    return
                    Bitmap loadedBitmap = new Bitmap(v);
                    if (loadedBitmap == null)
                        return null;

                    //    this.GetBitmapWH(hBM, w, h)
                    //    if (w < 1 or h<1)
                    //      return

                    w = loadedBitmap.Width;
                    h = loadedBitmap.Height;
                    IntPtr hBM = loadedBitmap.GetHbitmap();

                    if (w < 1 || h < 1)
                        return null;

                    //    hBM2:= this.CreateDIBSection(w, h, 32, Scan0)
                    //    this.CopyHBM(hBM2, 0, 0, hBM, 0, 0, w, h)

                    IntPtr Scan0 = IntPtr.Zero;
                    IntPtr hBM2 = InternalCreateDIBSection(w, h, 32, ref Scan0, out GDIFunctions.BITMAPINFO bi);
                    CopyHBM(hBM2, 0, 0, hBM, 0, 0, w, h);

                    //    DllCall("DeleteObject", Ptr, hBM)
                    loadedBitmap.Dispose();
                    GDIFunctions.DeleteObject(hBM);

                    //    if (!Scan0)
                    //                      return
                    if (Scan0 == IntPtr.Zero)
                        return null;
                    //    c1:= NumGet(Scan0 + 0, "uint") & 0xFFFFFF
                    uint c1 = ReadUInt32(Scan0, 0) & 0xFFFFFF;
                    //    c2:= NumGet(Scan0 + (w - 1) * 4, "uint") & 0xFFFFFF
                    uint c2 = ReadUInt32(Scan0, (w - 1) * 4) & 0xFFFFFF;
                    //    c3:= NumGet(Scan0 + (w * h - w) * 4, "uint") & 0xFFFFFF
                    uint c3 = ReadUInt32(Scan0, (w * h - w) * 4) & 0xFFFFFF;
                    //    c4:= NumGet(Scan0 + (w * h - 1) * 4, "uint") & 0xFFFFFF
                    uint c4 = ReadUInt32(Scan0, (w * h - 1) * 4) & 0xFFFFFF;
                    //    if (c1 != c2 or c1!= c3 or c1!= c4)
                    //      c1:= -1
                    if (c1 != c2 || c1 != c3 || c1 != c4)
                        c1 = 0xFFFFFFFF;
                    //    VarSetCapacity(v, w * h * 18 * (1 + !!A_IsUnicode)), i:= -4, n:= 0
                    //    ListLines, % (lls:= A_ListLines) ? "Off" : "Off"
                    //    SetFormat, IntegerFast, d
                    v = string.Empty;
                    int i = -4;
                    n = 0;
                    //    Loop, % h %
                    //    {
                    //                  y:= A_Index - 1
                    for (int y = 0; y < h; y++)
                    {
                        //      Loop, % w %
                        for (int innerLoop = 0; innerLoop < w; innerLoop++)
                        {
                            //        if (c:= NumGet(Scan0 + (i += 4), "uint") & 0xFFFFFF)!= c1
                            //          v.= ((A_Index - 1) | y << 16) "/" c "/", n++
                            uint c = ReadUInt32(Scan0, (i += 4)) & 0xFFFFFF;
                            if (c != c1)
                            {
                                v += (innerLoop | y << 16) + "/" + c + "/";
                                n++;
                            }
                        }
                        //    }
                    }
                    //                  ListLines, % lls %
                    //                  DllCall("DeleteObject", Ptr, hBM2)
                    GDIFunctions.DeleteObject(hBM2);
                    //  }
                }
                //  else
                else
                //              {
                {
                    //              v:= Trim(StrReplace(RegExReplace(v, "\s"), ",", "/"), "/")
                    v = regexWhiteSpace.Replace(v, "").Replace(",", "/").Trim('/');
                    //    r:= StrSplit(v, "/"), n:= r.MaxIndex()//3
                    string[] rv = v.Split("/");
                    n = (uint)rv.Length / 3;

                    //    if (!n)
                    //                      return
                    if (n == 0)
                        return null;
                    //                    VarSetCapacity(v, n * 18 * (1 + !!A_IsUnicode))
                    v = string.Empty;
                    //    x1:= x2:= r.1, y1:= y2:= r.2
                    int x2, y2;
                    if (!int.TryParse(rv[0], out int x1))
                    {
                        return null;
                    }
                    x2 = x1;
                    if (!int.TryParse(rv[1], out int y1))
                    {
                        return null;
                    }
                    y2 = y1;

                    //    ListLines, % (lls:= A_ListLines) ? "Off" : "Off"
                    //    SetFormat, IntegerFast, d
                    //    Loop, % n + (i:= -2) * 0
                    int i = -3;  //  Zero based vs 1 based
                    for (int iCount = 1; iCount <= n; iCount++)
                    {
                        //      x:= r[i += 3], y:= r[i + 1]
                        //      , (x < x1 && x1:= x), (x > x2 && x2:= x)
                        //      , (y < y1 && y1:= y), (y > y2 && y2:= y)
                        int.TryParse(rv[i += 3], out int x);
                        int.TryParse(rv[i + 1], out int y);
                        if (x < x1) x1 = x;
                        if (x > x2) x2 = x;
                        if (y < y1) y1 = y;
                        if (y > y2) y2 = y;
                    }
                    //    Loop, % n + (i:= -2) * 0
                    i = -3;  //  Zero based vs 1 based
                    for (int iCount = 1; iCount <= n; iCount++)
                    {
                        //      v.= (r[i += 3] - x1) | (r[i + 1] - y1) << 16. "/"
                        v += (int.Parse(rv[i += 3]) - x1) | (int.Parse(rv[i + 1]) - y1) << 16;
                        v += "/";
                        //      .Floor("0x" StrReplace(r[i + 2], "0x")) & 0xFFFFFF. "/"
                        v += int.Parse(rv[i + 2].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber) & 0xFFFFFF;
                        v += "/";
                    }
                    //    ListLines, % lls %
                    //    w:= x2 - x1 + 1, h:= y2 - y1 + 1
                    //  }
                    w = x2 - x1 + 1;
                    h = y2 - y1 + 1;
                }
                //          len1:= n, len0:= 0
                //}
                len1 = (int)n;
                len0 = 0;
            }
            //          else
            else
            {
                //          {
                //          r:= StrSplit(v, "."), w:= r.1
                //            , v:= this.base64tobit(r.2), h:= StrLen(v)//w
                string[] rv = v.Split(".");
                w = int.Parse(rv[0]);
                v = Base64tobit(rv[1]);
                h = v.Length / w;

                //  if (w < 1 or h<1 or StrLen(v)!= w * h)
                //    return
                if (w < 1 || h < 1 || v.Length != w * h)
                    return null;

                //            if (mode = 4)
                //              {
                if (mode == FindMode.colourDifferenceMode)
                {
                    //              r:= StrSplit(StrReplace(color, "0x"), "-")
                    //    , color:= Round("0x" r.1), n:= Round("0x" r.2)
                    rv = colour.Replace("0x", "").Split("-");
                    colour = int.Parse(rv[0], System.Globalization.NumberStyles.HexNumber).ToString();
                    n = uint.Parse(rv[1], System.Globalization.NumberStyles.HexNumber);
                    //  }
                }
                //              else
                //              {
                else
                {
                    //              r:= StrSplit(color, "@")
                    if (colour.Contains("@"))
                    {
                        rv = colour.Split("@");
                        //                , color:= r.1, n:= Round(r.2, 2) + (!r.2)
                        colour = rv[0];
                        double tempN = Math.Round(double.Parse(rv[1]), 2) + (Math.Round(double.Parse(rv[1]), 0) == 0 ? 1 : 0);
                        //    , n:= Floor(512 * 9 * 255 * 255 * (1 - n) * (1 - n))
                        n = (uint)Math.Floor(512 * 9 * 255 * 255 * (1 - tempN) * (1 - tempN));
                    }
                    else
                    {
                        n = 0;
                    }
                    //              }
                }
                //              StrReplace(v, "1", "", len1), len0:= StrLen(v) - len1
                len1 = CountInString(v, '1');
                len0 = v.Length - len1;
                //          }
            }
            //      e1:= Floor(len1 * e1), e0:= Floor(len0 * e0)
            e1 = len1 * e1;
            e0 = len0 * e0;
            info = new PictureInfo(v, w, h, len1, len0, e1, e0, mode, colour, n, comment, seterr);
            //return info[key]:=[v, w, h, len1, len0, e1, e0
            //  , mode, color, n, comment, seterr]
            infoDictionary.Add(key, info);
            return infoDictionary[key];
        }


        private uint ReadUInt32(IntPtr source, int offset)
        {
            byte[] byteTarget = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                byteTarget[i] = Marshal.ReadByte(source, offset + i);
            }
            return BitConverter.ToUInt32(byteTarget, 0);
        }

        private int CountInString(string source, char needle)
        {
            int length = source.Length;
            int count = 0;
            for (int n = length - 1; n >= 0; n--)
            {
                if (source[n] == needle)
                    count++;
            }
            return count;
        }

        private void CopyHBM(IntPtr hBM1, int x1, int y1, IntPtr hBM2, int x2, int y2, int w2, int h2)
        {
            //local
            //static Ptr:="Ptr"
            //mDC1:=DllCall("CreateCompatibleDC", Ptr,0, Ptr)
            IntPtr mDC1 = GDIFunctions.CreateCompatibleDC(IntPtr.Zero);
            //oBM1:=DllCall("SelectObject", Ptr,mDC1, Ptr,hBM1, Ptr)
            IntPtr oBM1 = GDIFunctions.SelectObject(mDC1, hBM1);
            //mDC2:=DllCall("CreateCompatibleDC", Ptr,0, Ptr)
            IntPtr mDC2 = GDIFunctions.CreateCompatibleDC(IntPtr.Zero);
            //oBM2:=DllCall("SelectObject", Ptr,mDC2, Ptr,hBM2, Ptr)
            IntPtr oBM2 = GDIFunctions.SelectObject(mDC2, hBM2);
            //DllCall("BitBlt", Ptr,mDC1
            //  , "int",x1, "int",y1, "int",w2, "int",h2, Ptr,mDC2
            //  , "int",x2, "int",y2, "uint",0xCC0020)
            GDIFunctions.BitBlt(mDC1, x1, y1, w2, h2, mDC2, x2, y2, GDIFunctions.TernaryRasterOperations.SRCCOPY);
            //DllCall("SelectObject", Ptr,mDC2, Ptr,oBM2)
            GDIFunctions.SelectObject(mDC2, oBM2);
            //DllCall("DeleteDC", Ptr,mDC2)
            GDIFunctions.DeleteDC(mDC2);
            //DllCall("SelectObject", Ptr,mDC1, Ptr,oBM1)
            GDIFunctions.SelectObject(mDC1, oBM1);
            //DllCall("DeleteDC", Ptr,mDC1)
            GDIFunctions.DeleteDC(mDC1);
        }

        public static string Bit2base64(string s)
        {
            string tempS = Regex.Replace(s, "[^01]+", string.Empty);
            string One5Zero = "100000";
            tempS += One5Zero.Substring(0, 6 - (tempS.Length % 6));
            tempS = Regex.Replace(tempS, ".{6}", "|$0");
            string decode = "0123456789+/ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            string pattern;
            string replacement;
            for (int i = 0; i < decode.Length; i++)
            {
                pattern = "|";
                pattern += (i >> 5 & 1) == 0 ? "0" : "1";
                pattern += (i >> 4 & 1) == 0 ? "0" : "1";
                pattern += (i >> 3 & 1) == 0 ? "0" : "1";
                pattern += (i >> 2 & 1) == 0 ? "0" : "1";
                pattern += (i >> 1 & 1) == 0 ? "0" : "1";
                pattern += (i & 1) == 0 ? "0" : "1";
                replacement = decode[i].ToString();
                tempS = tempS.Replace(pattern, replacement);
            }
            return tempS;
        }

        public static string Base64tobit(string s)
        {
            string decode = "0123456789+/ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            string pattern;
            string replacement;
            for (int i = 0; i < decode.Length; i++)
            {
                pattern = "[" + decode[i] + "]";
                replacement = (i >> 5 & 1) == 0 ? "0" : "1";
                replacement += (i >> 4 & 1) == 0 ? "0" : "1";
                replacement += (i >> 3 & 1) == 0 ? "0" : "1";
                replacement += (i >> 2 & 1) == 0 ? "0" : "1";
                replacement += (i >> 1 & 1) == 0 ? "0" : "1";
                replacement += (i & 1) == 0 ? "0" : "1";
                s = Regex.Replace(s, pattern, replacement);
            }
            return Regex.Replace(Regex.Replace(s, "10*$", ""), "[^01]+", "");
        }


        private IntPtr InternalCreateDIBSection(int zw, int zh)
        {
            short bpp = 32;
            IntPtr pvvBits = IntPtr.Zero;
            return InternalCreateDIBSection(zw, zh, bpp, ref pvvBits, out _);
        }

        private static IntPtr InternalCreateDIBSection(int w, int h, short bpp, ref IntPtr pvvBits, out GDIFunctions.BITMAPINFO bi)
        {
            //          local
            //VarSetCapacity(bi, 40, 0), NumPut(40, bi, 0, "int")
            //, NumPut(w, bi, 4, "int"), NumPut(-h, bi, 8, "int")
            //, NumPut(1, bi, 12, "short"), NumPut(bpp, bi, 14, "short")
            //return DllCall("CreateDIBSection", "Ptr", 0, "Ptr", &bi
            //  , "int", 0, "Ptr*", ppvBits:= 0, "Ptr", 0, "int", 0, "Ptr")
            bi = new GDIFunctions.BITMAPINFO();
            bi.biSize = Marshal.SizeOf(bi);
            bi.biWidth = w;
            bi.biHeight = -h;
            bi.biPlanes = 1;
            bi.biBitCount = bpp;
            bi.biCompression = 0; // BI_RGB
            bi.biSizeImage = 0;
            bi.biXPelsPerMeter = 0;
            bi.biYPelsPerMeter = 0;
            bi.biClrUsed = 0;
            bi.biClrImportant = 0;
            IntPtr hdc = IntPtr.Zero;
            IntPtr dib = GDIFunctions.CreateDIBSection(hdc, ref bi, 0, out pvvBits, IntPtr.Zero, 0);
            return dib;
        }

    }

    public class OCRResults
    {
        public OCRResults(string ocrText, int X, int Y, int W, int H)
        {
            OcrText = ocrText;
            OcrX = X;
            MinY = Y;
            this.W = W;
            this.H = H;
        }

        public string OcrText { get; }
        public int OcrX { get; }
        public int MinY { get; }
        public int W { get; }
        public int H { get; }
    }

    public class BindType
    {
        public IntPtr Id { get; set; }
        public int Mode { get; set; }
        public uint OldStyle { get; internal set; }
    }

    public class PicFindInput
    {
        public int Zx { get; set; }
        public int Zy { get; set; }
        public int Zw { get; set; }
        public int Zh { get; set; }
        public int Sx { get; set; }
        public int Sy { get; set; }
        public int Sw { get; set; }
        public int Sh { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }
        public string Comment { get; set; }

        public PicFindInput(int zx, int zy, int zw, int zh, int sx, int sy, int sw, int sh, string comment)
        {
            this.Zx = zx;
            this.Zy = zy;
            this.Zw = zw;
            this.Zh = zh;
            this.Sx = sx;
            this.Sy = sy;
            this.Sw = sw;
            this.Sh = sh;
            this.Comment = comment;
        }
    }
}
