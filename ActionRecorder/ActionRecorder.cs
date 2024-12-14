// <copyright file="ActionRecorder.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using System.Drawing;
using System.Runtime.InteropServices;
using MediaFoundation;
using MediaFoundation.ReadWrite;
using static MediaFoundation.Interop.MFAsyncCallback;


namespace ActionRecorderClient
{
    public class ActionRecorder
    {
        // Holds the writer for adding content to a video
        private IMFSinkWriter? _writer = null;
        // Holds the stream index for the writer.  As we only have a video stream, it's a single value.
        private int _streamIndex = 0;
        // Holds the current timestamp in the video, in 100-nanosecond units.
        private long _rtStart = 0;
        // ToDo: Implement VFR (Variable Frame Rate).
        // But this is difficult, as you need to add the previous bitmap with the time between when it arrived and when the new bitmap arrived.

        /// <summary>
        /// Stores the previous frame to support variable frame rate.
        /// </summary>
        private Bitmap? _previousFrame = null;
        /// <summary>
        /// Stores when the previous frame arrived.
        /// </summary>
        private DateTime _previousFrameDateTime = DateTime.MinValue;

        private bool _isVariableFrameRate = false;
        /// <summary>
        /// Enables VariableFrameRate IF recording has not started.
        /// </summary>
        public bool VariableFrameRate
        {
            get { return _isVariableFrameRate; }
            set { if (_writer == null) { _isVariableFrameRate = value; } }
        }


        /// <summary>
        /// Stores the video width to use for this recording.
        /// </summary>
        private int _videoWidth = 540;

        public int VideoWidth
        {
            get { return _videoWidth; }
            set { _videoWidth = value; }
        }

        /// <summary>
        /// Stores the video height to use for this recording.
        /// </summary>
        private int _videoHeight = 960;

        public int VideoHeight
        {
            get { return _videoHeight; }
            set { _videoHeight = value; }
        }

        /// <summary>
        /// Stores the frames per second for this video.
        /// </summary>
        private int _videoFPS = 1;

        public int VideoFPS
        {
            get { return _videoFPS; }
            set { if (value > 0) { _videoFPS = value; _videoFrameDuration = 10000000 / _videoFPS; } }
        }

        /// <summary>
        /// Stores the amound of time a frame will be shown for.  It is in 100 nanoseconds.
        /// 10,000,000 = 1 Second.
        /// </summary>
        private long _videoFrameDuration = 10000000;


        /// <summary>
        /// Stores whether the MediaFoundation has been started.
        /// </summary>
        private bool _isMFStarted = false;
        public bool IsMFStarted
        {
            get { return _isMFStarted; }
            private set { _isMFStarted = value; }
        }


        /// <summary>
        /// Returns true when recording has already started.
        /// </summary>
        public bool IsRecordingStarted
        {
            get { return (_writer != null); }
        }


        /// <summary>
        /// Stores the name of the file that will be written to.
        /// </summary>
        private string _videoFileName = string.Empty;

        public string VideoFileName
        {
            get { return _videoFileName; }
            private set { _videoFileName = value; }
        }

        /// <summary>
        /// Constructs the base ActionRecorder
        /// </summary>
        /// <param name="VideoHeight">The height of the images that are being added in pixels</param>
        /// <param name="VideoWidth">The width of the images that are being added in pixels</param>
        /// <param name="VideoFramesPerSecond">The number of frames per second to be recorded</param>
        /// <param name="VideoFileName">The fully qualified output file name (for an MP4 H264 output file)</param>
        public ActionRecorder(int VideoHeight, int VideoWidth, int VideoFramesPerSecond, string VideoFileName)
        {
            _videoFileName = VideoFileName;
            _videoHeight = VideoHeight;
            _videoWidth = VideoWidth;
            VideoFPS = VideoFramesPerSecond;
            IsMFStarted = false;
        }

        /// <summary>
        /// Constructs the base ActionRecorder
        /// You will have to manually set the VideoHeight and VideoWidth, or use the SetHeightAndWidthFromImage method.
        /// </summary>
        /// <param name="VideoFramesPerSecond">The number of frames per second to be recorded</param>
        /// <param name="VideoFileName">The fully qualified output file name (for an MP4 H264 output file)</param>
        public ActionRecorder(int VideoFramesPerSecond, string VideoFileName)
        {
            _videoFileName = VideoFileName;
            VideoFPS = VideoFramesPerSecond;
            IsMFStarted = false;
        }


        /// <summary>
        /// Stops any active recording and MediaFoundation on destruction of the class.
        /// </summary>
        ~ActionRecorder()
        {
            StopRecording();
            StopMF();
        }

        /// <summary>
        /// Using an Image get the Height and Width
        /// </summary>
        /// <param name="exampleImage">An example image (or even the 1st frame to save).  It's only used to get Height and Width.</param>
        public void SetHeightAndWidthFromImage(Image exampleImage)
        { _videoHeight = exampleImage.Height; _videoWidth = exampleImage.Width; }


        /// <summary>
        /// Start MediaFoundation.  This MUST be called before StartRecording.
        /// </summary>
        public void StartMF()
        {
            HResult hr = HResult.S_OK;
            if (!_isMFStarted)
            {
                hr = MFExtern.MFStartup(0x00020070, MFStartup.Full); // MF_VERSION set to 0x00020070, but????
            }
            if (hr == HResult.S_OK)
            {
                _isMFStarted = true;
            }
        }


        /// <summary>
        /// Creates the output file for the recording, and initiates the writer.
        /// </summary>
        /// <exception cref="Exception">MediaFoundation is not started.</exception>
        /// <exception cref="Exception">VideoFileName not populated.</exception>
        /// <exception cref="Exception">Unable to use provided file name.</exception>
        /// <exception cref="Exception">MFCreateSinkWriterFromURL failed, result = nnn</exception>
        /// <exception cref="Exception">_writer is NULL, something went pear shaped.</exception>
        /// <exception cref="Exception">Unexpected exception encountered.</exception>
        /// <exception cref="Exception">Setting up the MF stuff failed, result = nnn</exception>
        public void StartRecording()
        {
            // Define the expected output bit rate (will be updated dynamically)
            uint VideoBitRate = 2000;
            // Define the input and output formats
            Guid VideoInputFormat = MediaFoundation.MFMediaType.RGB32;
            Guid VideoEncodingFormat = MediaFoundation.MFMediaType.H264;
            // Define the output and output MediaTypes
            IMFMediaType? pMediaTypeOut = null, pMediaTypeIn = null;

            if (!IsMFStarted)
            {
                throw new Exception("MediaFoundation is not started.");
            }
            if (string.IsNullOrEmpty(_videoFileName))
            {
                throw new Exception("VideoFileName not populated.");
            }
            if (_writer != null)
            {
                throw new Exception("Recording has already started.");
            }
            try
            {
                File.WriteAllText(_videoFileName, String.Empty);
                File.Delete(_videoFileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to use provided file name.", ex);
            }
            // Allocate variable to hold result from all the MF calls
            HResult hr;
            // Create the _writer
            try
            {
                hr = MFExtern.MFCreateSinkWriterFromURL(_videoFileName, null, null, out _writer);
                if (hr != HResult.S_OK)
                {
                    throw new Exception("MFCreateSinkWriterFromURL failed, result = " + hr.ToString());
                }
                if (_writer == null)
                {
                    throw new Exception("_writer is NULL, something went pear shaped.");
                }
            }
            catch (Exception ex)
            {
                if (_writer != null)
                {
                    // Cleanup as we should be null!
                    Marshal.ReleaseComObject(_writer);
                    _writer = null;
                }
                throw new Exception("Unexpected exception encountered.", ex);
            }
            // Set the output media type.
            hr = MFExtern.MFCreateMediaType(out pMediaTypeOut);
            if (hr == HResult.S_OK && pMediaTypeOut != null)
            {
                hr = pMediaTypeOut.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Video);
            }
            if (hr == HResult.S_OK && pMediaTypeOut != null)
            {
                hr = pMediaTypeOut.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, VideoEncodingFormat);
            }
            if (hr == HResult.S_OK && pMediaTypeOut != null)
            {
                hr = pMediaTypeOut.SetUINT32(MFAttributesClsid.MF_MT_AVG_BITRATE, VideoBitRate);
            }
            if (hr == HResult.S_OK && pMediaTypeOut != null)
            {
                hr = pMediaTypeOut.SetUINT32(MFAttributesClsid.MF_MT_INTERLACE_MODE, (int)MFVideoInterlaceMode.Progressive);
            }
            if (hr == HResult.S_OK && pMediaTypeOut != null)
            {
                hr = MFExtern.MFSetAttributeSize(pMediaTypeOut, MFAttributesClsid.MF_MT_FRAME_SIZE, VideoWidth, VideoHeight);
            }
            if (hr == HResult.S_OK && pMediaTypeOut != null)
            {
                hr = MFExtern.MFSetAttributeRatio(pMediaTypeOut, MFAttributesClsid.MF_MT_FRAME_RATE, _videoFPS, 1);
            }
            if (hr == HResult.S_OK && pMediaTypeOut != null)
            {
                hr = MFExtern.MFSetAttributeRatio(pMediaTypeOut, MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
            }
            if (hr == HResult.S_OK && pMediaTypeOut != null)
            {
                hr = _writer.AddStream(pMediaTypeOut, out _streamIndex);
            }
            // Set the input media type.
            if (hr == HResult.S_OK)
            {
                hr = MFExtern.MFCreateMediaType(out pMediaTypeIn);
            }
            if (hr == HResult.S_OK && pMediaTypeIn != null)
            {
                hr = pMediaTypeIn.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Video);
            }
            if (hr == HResult.S_OK && pMediaTypeIn != null)
            {
                hr = pMediaTypeIn.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, VideoInputFormat);
            }
            if (hr == HResult.S_OK && pMediaTypeIn != null)
            {
                hr = pMediaTypeIn.SetUINT32(MFAttributesClsid.MF_MT_INTERLACE_MODE, (int)MFVideoInterlaceMode.Progressive);
            }
            if (hr == HResult.S_OK && pMediaTypeIn != null)
            {
                hr = MFExtern.MFSetAttributeSize(pMediaTypeIn, MFAttributesClsid.MF_MT_FRAME_SIZE, VideoWidth, VideoHeight);
            }
            if (hr == HResult.S_OK && pMediaTypeIn != null)
            {
                hr = MFExtern.MFSetAttributeRatio(pMediaTypeIn, MFAttributesClsid.MF_MT_FRAME_RATE, _videoFPS, 1);
            }
            if (hr == HResult.S_OK && pMediaTypeIn != null)
            {
                hr = MFExtern.MFSetAttributeRatio(pMediaTypeIn, MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
            }
            if (hr == HResult.S_OK && pMediaTypeIn != null)
            {
                hr = _writer.SetInputMediaType(_streamIndex, pMediaTypeIn, null);
            }

            // Tell the sink writer to start accepting data.
            if (hr == HResult.S_OK)
            {
                hr = _writer.BeginWriting();
            }
            if (pMediaTypeIn != null)
            { Marshal.ReleaseComObject(pMediaTypeIn); }
            if (pMediaTypeOut != null)
            { Marshal.ReleaseComObject(pMediaTypeOut); }
            if (hr != HResult.S_OK)
            {
                throw new Exception("Setting up the MF stuff failed, result = " + hr.ToString());
            }
        }

        /// <summary>
        /// Converts an input Bitmap into a Byte Array.
        /// </summary>
        /// <param name="bmp">The Bitmap that you want as a Byte Array</param>
        /// <returns>A byte array of the Bitmap content</returns>
        private byte[] BmpToByte(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            // Save to memory using the Jpeg format
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            // read to end
            byte[] bmpBytes = ms.GetBuffer();
            bmp.Dispose();
            ms.Close();
            return bmpBytes;
        }

        /// <summary>
        /// Adds the provided sourceImage onto the already started recording.
        /// </summary>
        /// <param name="source">The Image to add into the recording</param>
        /// <param name="stamp">If this is populated, then it will be stamped onto the output frame</param>
        /// <exception cref="Exception">MediaFoundation is not started.</exception>
        /// <exception cref="Exception">InitiateMFWriterAndSink hasn't been done.</exception>
        /// <exception cref="Exception">Either memory allocation or MFCopyImage borked.</exception>
        public void AddImageAsFrame(Image source, string stamp)
        {
            if (!_isMFStarted)
            {
                throw new Exception("MediaFoundation is not started.");
            }

            if (_writer == null)
            {
                throw new Exception("InitiateMFWriterAndSink hasn't been done.");
            }

            IMFSample? pSample = null;
            HResult hr = HResult.E_FAIL;
            IMFMediaBuffer? pBuffer = null;
            int cbWidth = 4 * VideoWidth;
            int cbBuffer = cbWidth * VideoHeight;
            IntPtr pData = IntPtr.Zero;

            // Convert to Bitmap
            Bitmap bitmap = (Bitmap)source;

            if (!string.IsNullOrEmpty(stamp))
            {
                // Write stamp on bitmap
                RectangleF rectf = new RectangleF(70, 90, 90, 50);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.DrawString(stamp, new Font("Tahoma", 8), Brushes.White, rectf);
            }
            long VFD = _videoFrameDuration;

            if (_isVariableFrameRate && _previousFrame == null)
            {
                _previousFrame = bitmap;
                _previousFrameDateTime = DateTime.Now;
                return;
            }
            else if (_isVariableFrameRate && _previousFrame != null)
            {
                Bitmap tempBitmap = _previousFrame;
                _previousFrame = bitmap;
                bitmap = tempBitmap;
                VFD = ((long)(DateTime.Now - _previousFrameDateTime).TotalNanoseconds) / 100;
            }

            // Convert to byte array
            byte[] imgArray = BmpToByte(bitmap);
            try
            {
                IntPtr unmanagedPointer = Marshal.AllocHGlobal(cbBuffer);
                // Copy to the memory assigned to the pointer, skipping the managed bitmap header at the start.
                Marshal.Copy(imgArray, imgArray.Length - cbBuffer, unmanagedPointer, cbBuffer);

                // Create a new memory buffer.
                hr = MFExtern.MFCreateMemoryBuffer(cbBuffer, out pBuffer);
                if (hr == HResult.S_OK)
                {
                    hr = pBuffer.Lock(out pData, out int pcbMaxLength, out int pcbCurrentLength);
                }
                if (hr == HResult.S_OK)
                {
                    // The following assumes you are using a newer codec as it is not reversing the input which older codecs need due to BMP's.
                    hr = MFExtern.MFCopyImage(
                                                pData,                      // Destination buffer.
                                                cbWidth,                    // Destination stride.
                                                unmanagedPointer,           // First row in source image.
                                                cbWidth,                    // Source stride.
                                                cbWidth,                    // Image width in bytes.
                                                VideoHeight                 // Image height in pixels.
                                                );
                    /*
                     * The following is what needs to be done with an older codec eg. WMV3.  The symptom is that the video is inverted.
                     * Supposedly you can use the MF_MT_DEFAULT_STRIDE but I couldn't work out how to use it...
                     * https://learn.microsoft.com/en-gb/windows/win32/medfound/mf-mt-default-stride-attribute
                    IntPtr reversedpData = IntPtr.Add(pData, (VideoHeight - 1) * cbWidth);
                    hr = MFExtern.MFCopyImage(
                                                reversedpData,                      // Destination buffer.
                                                (cbWidth * -1),                     // Destination stride.
                                                unmanagedPointer,                   // First row in source image.
                                                cbWidth,                            // Source stride.
                                                cbWidth,                            // Image width in bytes.
                                                VideoHeight                         // Image height in pixels.
                                                );
                    */


                }
                // Free the unmanaged pointer memory now that it's been copied.
                Marshal.FreeHGlobal(unmanagedPointer);
            }
            catch (Exception ex)
            {
                throw new Exception("Either memory allocation or MFCopyImage borked.", ex);
            }
            finally
            {
                // Unlock the buffer
                if (pBuffer != null)
                {
                    pBuffer.Unlock();
                }
            }
            // Set the data length of the buffer.
            if (hr == HResult.S_OK)
            {
                hr = pBuffer.SetCurrentLength(cbBuffer);
            }

            // Create a media sample and add the buffer to the sample.
            if (hr == HResult.S_OK)
            {
                hr = MFExtern.MFCreateSample(out pSample);
            }
            if (hr == HResult.S_OK && pSample != null)
            {
                hr = pSample.AddBuffer(pBuffer);
            }

            // Set the time stamp and the duration.
            if (hr == HResult.S_OK && pSample != null)
            {
                hr = pSample.SetSampleTime(_rtStart);
            }
            if (hr == HResult.S_OK && pSample != null)
            {
                hr = pSample.SetSampleDuration(VFD);
            }

            // Send the sample to the Sink Writer.
            if (hr == HResult.S_OK && pSample != null)
            {
                hr = _writer.WriteSample(_streamIndex, pSample);
            }

            if (pSample != null)
            {
                Marshal.ReleaseComObject(pSample);
            }
            if (pBuffer != null)
            {
                Marshal.ReleaseComObject(pBuffer);
            }

            _rtStart += VFD;
        }

        /// <summary>
        /// Adds the provided sourceImage onto the already started recording.
        /// But without a stamp...
        /// </summary>
        /// <param name="source">The Image to add into the recording</param>
        /// <exception cref="Exception">MediaFoundation is not started.</exception>
        /// <exception cref="Exception">InitiateMFWriterAndSink hasn't been done.</exception>
        /// <exception cref="Exception">Either memory allocation or MFCopyImage borked.</exception>
        public void AddImageAsFrame(Image source)
        {
            AddImageAsFrame(source, string.Empty);
        }


        /// <summary>
        /// If a recording is in progress this will stop it, and write the video file out.
        /// </summary>
        public void StopRecording()
        {
            if (IsMFStarted && _writer != null)
            {
                if (_isVariableFrameRate && _previousFrame != null)
                {
                    AddImageAsFrame(_previousFrame);
                }
                HResult hr = _writer.Finalize_();
                Marshal.ReleaseComObject(_writer);
                _writer = null;
            }
        }

        /// <summary>
        /// If a recording is in progress this will stop it, and write the video file out.
        /// If MediaFoundation is started, this will stop it.
        /// </summary>
        public void StopMF()
        {
            StopRecording();
            if (IsMFStarted)
            {
                MFExtern.MFShutdown();
                IsMFStarted = false;
            }
        }
    }
}
