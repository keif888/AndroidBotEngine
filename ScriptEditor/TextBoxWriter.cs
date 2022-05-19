// <copyright file="TestAction.cs" company="Keith Martin">
// Copyright (c) Keith Martin
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ScriptEditor
{
    internal class TextBoxWriter : System.IO.TextWriter
    {
        private TextBoxBase control;
        private StringBuilder? Builder;
        private StringBuilder bufferUntilNewLine;

        public TextBoxWriter(TextBox control)
        {
            this.control = control;
            control.HandleCreated += new EventHandler(OnHandleCreated);
        }

        public override void Write(char ch)
        {
            Write(ch.ToString());
        }

        private void BufferString(string s)
        {
            if (bufferUntilNewLine == null)
            {
                bufferUntilNewLine = new StringBuilder(80);
            }
            bufferUntilNewLine.Append(s);
        }

        public override void Write(string s)
        {
            BufferString(s);

            if (bufferUntilNewLine.ToString().EndsWith(Environment.NewLine))
            {
                if ((control.IsHandleCreated))
                    AppendText(bufferUntilNewLine.ToString());
                else
                    BufferText(bufferUntilNewLine.ToString());
                bufferUntilNewLine.Clear();
            }
        }

        public override void WriteLine(string s)
        {
            Write(s + Environment.NewLine);
        }

        private void BufferText(string s)
        {
            if (Builder == null)
                Builder = new StringBuilder();
            Builder.Append(s);
        }

        private void AppendText(string s)
        {
            if (Builder != null)
            {
                if (control.InvokeRequired)
                {
                    control.Invoke((MethodInvoker)delegate { control.AppendText(Builder.ToString()); });
                }
                else
                    control.AppendText(Builder.ToString());
                Builder = null;
            }

            if (control.InvokeRequired)
            {
                control.Invoke((MethodInvoker)delegate { control.AppendText(s); });
            }
            else
                control.AppendText(s);
        }

        private void OnHandleCreated(object sender, EventArgs e)
        {
            if (Builder != null)
            {
                if (control.InvokeRequired)
                {
                    control.Invoke((MethodInvoker)delegate { control.AppendText(Builder.ToString()); });
                }
                else
                    control.AppendText(Builder.ToString());
                Builder = null;
            }
        }

        public override System.Text.Encoding Encoding
        {
            get
            {
                return Encoding.Default;
            }
        }
    }
}