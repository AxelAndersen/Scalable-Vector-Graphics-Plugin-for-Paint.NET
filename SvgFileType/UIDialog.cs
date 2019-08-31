﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace SvgFileTypePlugin
{
    public partial class UiDialog : Form
    {
        public int Dpi => (int) nudDpi.Value;
        public int CanvasW => (int) canvasw.Value;
        public int CanvasH => (int) canvash.Value;
        public bool KeepAspectRatio => cbKeepAR.Checked;
        public bool ImportOpacity => cbOpacity.Checked;
        public bool ImportHiddenLayers => cbLayers.Checked;
        public bool ImportGroupBoundariesAsLayers => cbPSDSupport.Checked;
        public event EventHandler OkClick;

        private const string VersionString = "1.0";
        private const int BigImageSize = 1280;
        private Size _sizeHint;
        private bool _changedProgramatically;
        private int _originalPdi = 96;

        public UiDialog()
        {
            InitializeComponent();
            warningBox.Image = SystemIcons.Warning.ToBitmap();
            Text = "SVG Import Plug-in v" + VersionString;
        }

        public LayersMode LayerMode
        {
            get
            {
                if (rbAll.Checked)
                {
                    return LayersMode.All;
                }

                if (rbFlat.Checked)
                {
                    return LayersMode.Flat;
                }

                return LayersMode.Groups;
            }
        }

        public void SetSvgInfo(
            int viewportw,
            int viewporth,
            int viewboxx = 0,
            int viewboxy = 0,
            int viewboxw = 0,
            int viewboxh = 0,
            int dpi = 96)
        {
            _originalPdi = dpi;
            if (viewportw > 0)
                vpw.Text = viewportw.ToString();
            if (viewporth > 0)
                vph.Text = viewporth.ToString();
            if (viewboxx > 0)
                vbx.Text = viewboxx.ToString();
            if (viewboxy > 0)
                vby.Text = viewboxy.ToString();
            if (viewboxw > 0)
                vbw.Text = viewboxw.ToString();
            if (viewboxh > 0)
                vbh.Text = viewboxh.ToString();

            if (viewportw > 0 && viewporth > 0)
                _sizeHint = new Size(viewportw, viewporth);
            else if (viewboxx > 0 && viewboxy > 0)
                _sizeHint = new Size(viewboxx, viewboxy);
            else if (viewboxw > 0 && viewboxh > 0)
                _sizeHint = new Size(viewboxw, viewboxh);
            else
                _sizeHint = new Size(500, 500);

            nudDpi.Value = dpi;
            _changedProgramatically = true;

            if (_sizeHint.Width > BigImageSize || _sizeHint.Height > BigImageSize)
            {
                warningBox.Visible = true;
                // Set default size from numeric default input and keep aspect ratio.
                // Default is 500
                canvash.Value = canvasw.Value * _sizeHint.Height / _sizeHint.Width;
            }
            else
            {
                warningBox.Visible = false;
                // Keep original image size and show warning
                canvasw.Value = _sizeHint.Width;
                canvash.Value = _sizeHint.Height;
            }

            _changedProgramatically = false;

            ResolveControlsVisibility();
        }

        private void canvasw_ValueChanged(object sender, EventArgs e)
        {
            if (_changedProgramatically)
                return;

            warningBox.Visible = false;

            if (!KeepAspectRatio)
                return;

            canvash.Value = canvasw.Value * _sizeHint.Height / _sizeHint.Width;
        }

        private void canvash_ValueChanged(object sender, EventArgs e)
        {
            if (_changedProgramatically)
                return;

            warningBox.Visible = false;

            if (!KeepAspectRatio)
                return;

            canvasw.Value = canvash.Value * _sizeHint.Width / _sizeHint.Height;
        }

        private void cbKeepAR_CheckedChanged(object sender, EventArgs e)
        {
            canvasw_ValueChanged(sender, e);
        }

        private void btnUseOriginal_Click(object sender, EventArgs e)
        {
            _changedProgramatically = true;
            warningBox.Visible = false;
            // Keep original image size and show warning
            canvasw.Value = _sizeHint.Width;
            canvash.Value = _sizeHint.Height;
            nudDpi.Value = _originalPdi;
            _changedProgramatically = false;
        }

        private void ResolvePropertiesVisibility(object sender, EventArgs e)
        {
            ResolveControlsVisibility();
        }

        private void ResolveControlsVisibility()
        {
            cbOpacity.Enabled = cbLayers.Enabled = cbPSDSupport.Enabled = !rbFlat.Checked;
            cbPSDSupport.Enabled = rbAll.Checked;
        }

        private void linkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(
                @"https://github.com/otuncelli/Scalable-Vector-Graphics-Plugin-for-Paint.NET");
        }

        public void ReportProgress(int value)
        {
            if (progress.InvokeRequired)
            {
                progress.BeginInvoke((Action) (() =>
                {
                    progress.Value = value;
                    UpdateProgressLabel();
                }));

                return;
            }

            progress.Value = value;
            UpdateProgressLabel();
        }

        private void UpdateProgressLabel()
        {
            lbProgress.Text = progress.Value + " of " + progress.Maximum;
        }

        public void SetMaxProgress(int max)
        {
            if (progress.InvokeRequired)
            {
                progress.BeginInvoke((Action) (() =>
                {
                    progress.Maximum = max;
                    UpdateProgressLabel();
                }));

                return;
            }

            progress.Maximum = max;
            UpdateProgressLabel();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            btnOk.Enabled = gr1.Enabled = gr2.Enabled = gr3.Enabled = false;
            OkClick?.Invoke(sender, e);
        }
    }
}