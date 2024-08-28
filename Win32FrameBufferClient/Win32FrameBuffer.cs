// <copyright file="Class1.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using System.Diagnostics;
using System.Drawing;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Graphics.Gdi;

namespace Win32FrameBufferClient
{
    public class Win32FrameBuffer
    {
        private readonly IntPtr _mainWindowHandle;
        private Rectangle _imageSize;

        /// <summary>
        /// Initialise the class by finding the main window handle that we will be grabbing content from
        /// </summary>
        /// <param name="ProcessName">The name of the emulator executable (eg. dnplayer)</param>
        /// <param name="MainWindowName">The name of the window that the emulator has started (this is likely to be the name assigned for the emulator's vm)</param>
        public Win32FrameBuffer(string ProcessName, string MainWindowName)
        {
            _mainWindowHandle = IntPtr.Zero;
            _imageSize = new Rectangle(1, 34, 540, 960);
            Process[] processes = Process.GetProcessesByName(ProcessName);
            foreach (Process process in processes)
            {
                if (process.MainWindowTitle == MainWindowName)
                {
                    _mainWindowHandle = process.MainWindowHandle;
                    break;
                }
            }
            if (_mainWindowHandle == IntPtr.Zero)
            {
                throw new Exception("Unable to find the ProcessName/MainWindowName combination");
            }
        }

        /// <summary>
        /// Determines if the class was initialised successfully.  Destroy it if not.
        /// </summary>
        /// <returns>true if the window handle is populated.</returns>
        public bool IsValid()
        { 
            return _mainWindowHandle != IntPtr.Zero;
        }

        /// <summary>
        /// Resizes and optionally relocates the found window
        /// </summary>
        /// <param name="x">Top Left corner</param>
        /// <param name="y">Top Left corner</param>
        /// <param name="width">Width of the window, allow for any borders</param>
        /// <param name="height">Height of the window, allow for the title bar</param>
        /// <param name="relocate">true if you want the window to be relocated, otherwise it will just resize where it is</param>
        /// <returns></returns>
        [SupportedOSPlatform("Windows5.0")]
        public bool ResizeEmulator(int x, int y, int width, int height, bool relocate) 
        {
            bool worked;
            if (relocate)
            {
                worked = PInvoke.SetWindowPos((HWND)_mainWindowHandle, HWND.HWND_BOTTOM, x, y, width, height, SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
            }
            else
            {
                worked = PInvoke.SetWindowPos((HWND)_mainWindowHandle, HWND.HWND_BOTTOM, x, y, width, height, SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
            }
            return worked;
        }


        /// <summary>
        /// Gets the current location and size of the attached emulator window
        /// </summary>
        /// <param name="ClientSize">Returns a Rectangle with the size and location details</param>
        /// <returns>true if the size was retreived.</returns>
        [SupportedOSPlatform("Windows5.0")]
        public bool GetEmulatorLocationAndSize(out Rectangle ClientSize)
        {
            if (PInvoke.GetWindowRect((HWND)_mainWindowHandle, out RECT lpRect))
            {
                ClientSize = new Rectangle(lpRect.X ,lpRect.Y, lpRect.Width, lpRect.Height);
                return true;
            }
            else
            {
                ClientSize = new Rectangle();
                return false;
            }
        }


        /// <summary>
        /// Accesses the ImageSize that is used by ToImage().
        /// </summary>
        public Rectangle ImageSize
        {
            get => _imageSize;
            set => _imageSize = value;
        }



        /// <summary>
        /// Grabs the content of the window handle into an Image
        /// </summary>
        /// <param name="x">top left offset within the source window</param>
        /// <param name="y">top left offset within the source window</param>
        /// <param name="width">Width of the image to capture</param>
        /// <param name="height">Height of the image to capture</param>
        /// <returns>The image on the source window</returns>
        [SupportedOSPlatform("Windows5.0")]
        public Image ToImage(int x, int y, int width, int height)
        {
            HDC hdcSrc = PInvoke.GetWindowDC((HWND)_mainWindowHandle);
            HDC hdcDest = PInvoke.CreateCompatibleDC(hdcSrc);
            HBITMAP hBitmap = PInvoke.CreateCompatibleBitmap(hdcSrc, width, height);

            HGDIOBJ hOld = PInvoke.SelectObject(hdcDest, hBitmap);
            PInvoke.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, x, y, ROP_CODE.SRCCOPY);
            PInvoke.SelectObject(hdcDest, hOld);
            PInvoke.DeleteDC(hdcDest);
            _ = PInvoke.ReleaseDC((HWND)_mainWindowHandle, hdcSrc);

            Image image = Image.FromHbitmap(hBitmap);
            PInvoke.DeleteObject(hBitmap);
            return image;
        }

        /// <summary>
        /// Grabs the content of the window handle into an Image
        /// </summary>
        /// <param name="ImageSize">Rectagle that contains the x and y offsets in the source window, and the width and height of the image to capture</param>
        /// <returns>The image on the source window</returns>
        [SupportedOSPlatform("Windows5.0")]
        public Image ToImage(Rectangle ImageSize)
        {
            return ToImage(ImageSize.X, ImageSize.Y, ImageSize.Width, ImageSize.Height);
        }

        /// <summary>
        /// Grabs the content of the window handle into an Image
        /// This uses the ImageSize that is stored in the class for the x and y offsets in the source window, and the width and height of the image to capture
        /// </summary>
        /// <returns>The image on the source window</returns>
        [SupportedOSPlatform("Windows5.0")]
        public Image ToImage()
        {
            return ToImage(_imageSize);
        }

    }
}
