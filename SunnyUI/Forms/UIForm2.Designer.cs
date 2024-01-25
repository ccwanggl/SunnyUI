﻿namespace Sunny.UI
{
    partial class UIForm2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            this.UnRegister();

            if (hotKeys != null)
            {
                foreach (var hotKey in hotKeys.Values)
                {
                    Win32.User.UnregisterHotKey(System.IntPtr.Zero, hotKey.id);
                }
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // UIForm2
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Icon = Sunny.UI.Properties.Resources.SunnyUI;
            Name = "UIForm2";
            Padding = new System.Windows.Forms.Padding(0, 35, 0, 0);
            Text = "UIForm2";
            ResumeLayout(false);
        }

        #endregion
    }
}