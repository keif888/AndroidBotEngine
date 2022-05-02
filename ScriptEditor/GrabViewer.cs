using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScriptEditor
{
    /// <summary>
    /// Class to show an image, and draw a rectangle on it, with the coordinates available for retrieval.
    /// </summary>
    public partial class GrabViewer : Form
    {
        private Point startPos;       // mouse-down position
        private Point currentPos;     // current mouse position
        private bool drawing;         // busy drawing
        private Rectangle rectangle;  // on panel rectangle

        /// <summary>
        /// Sets up the form with defaults
        /// </summary>
        public GrabViewer()
        {
            InitializeComponent();
            drawing = false;
            rectangle = new Rectangle();  // previous rectangles
            startPos = new Point(0, 0);
            currentPos = new Point(0, 0);
        }

        /// <summary>
        /// Loads the provided image into the form for display purposes
        /// </summary>
        /// <param name="newImage">An Image to be displayed</param>
        public void SetImage(Image newImage)
        {
            pnlGraphics.SuspendLayout();
            pbFrame.SuspendLayout();
            pbFrame.Image = newImage;
            pbFrame.Width = newImage.Width;
            pbFrame.Height = newImage.Height;
            pbFrame.ResumeLayout();
            pnlGraphics.ResumeLayout();
            drawing = false;
            startPos.X = 0;
            startPos.Y = 0;
            currentPos.X = 0;
            currentPos.Y = 0;
            rectangle = GetRectangle();
        }

        /// <summary>
        /// Generates a rectangle based on the selected area on the image
        /// </summary>
        /// <returns></returns>
        private Rectangle GetRectangle()
        {
            return new Rectangle(
                Math.Min(startPos.X, currentPos.X),
                Math.Min(startPos.Y, currentPos.Y),
                Math.Abs(startPos.X - currentPos.X),
                Math.Abs(startPos.Y - currentPos.Y));
        }

        /// <summary>
        /// Detect that the mouse button has been pushed, and initiate drawing of a box.
        /// Store the start point of the current box, and update the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PbFrame_MouseDown(object sender, MouseEventArgs e)
        {
            currentPos = startPos = e.Location;
            drawing = true;
            tbTopLeft.Text = e.Location.ToString();
        }

        /// <summary>
        /// Detect that the mouse button has been released.
        /// Store the end point of the current box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PbFrame_MouseUp(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                drawing = false;
                var rc = GetRectangle();
                if (rc.Width > 0 && rc.Height > 0) rectangle = rc;
                pbFrame.Invalidate();
                tbBottomRight.Text = e.Location.ToString();
            }
        }

        /// <summary>
        /// Detect Mouse movement, and update the bounding box if drawing it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PbFrame_MouseMove(object sender, MouseEventArgs e)
        {
            currentPos = e.Location;
            if (drawing) pbFrame.Invalidate();
            tbCurrent.Text = e.Location.ToString();
        }

        /// <summary>
        /// Paint the bounding box, if drawing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PbFrame_Paint(object sender, PaintEventArgs e)
        {
            if ((rectangle.Width > 0 && rectangle.Height > 0)) e.Graphics.DrawRectangle(Pens.Black, rectangle);
            if (drawing) e.Graphics.DrawRectangle(Pens.Red, GetRectangle());
        }

        /// <summary>
        /// Clear out the text boxes, and refresh the painted image to remove the box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReset_Click(object sender, EventArgs e)
        {
            tbTopLeft.Text = string.Empty;
            tbBottomRight.Text = string.Empty;
            drawing = false;
            pbFrame.Refresh();
        }

        /// <summary>
        /// Close the form and return when Capture is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCapture_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Return to the caller the Rectangle that was marked out.  
        /// Ensure that it is a Postitive box, regardless of how it is drawn with the UI.
        /// </summary>
        /// <returns></returns>
        public Rectangle GetCaptureBox()
        {
            if (rectangle.X < 0)
                rectangle.X = 0;
            if (rectangle.Y < 0)
                rectangle.Y = 0;
            if (rectangle.Height + rectangle.Y > pbFrame.Height)
                rectangle.Height = pbFrame.Height - rectangle.Y;
            if (rectangle.Width + rectangle.X > pbFrame.Width)
                rectangle.Width = pbFrame.Width - rectangle.X;
            return rectangle;
        }

        /// <summary>
        /// Copy the coordinates into the paste buffer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCopy_Click(object sender, EventArgs e)
        {
            Rectangle r = GetCaptureBox();
            Point p1, p2;
            p1 = r.Location;
            p2 = r.Location + r.Size;
            //ToDo: Reformat to allow main form to receive them neatly.
            Clipboard.SetText(string.Format("{0} - {1}", p1.ToString(), p2.ToString()));
        }
    }
}
