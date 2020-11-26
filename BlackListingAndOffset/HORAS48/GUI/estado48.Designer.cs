namespace ZOT.HORAS48.GUI
{
    partial class estado48
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
            this.labelejecutando = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelejecutando
            // 
            this.labelejecutando.AutoSize = true;
            this.labelejecutando.Location = new System.Drawing.Point(127, 64);
            this.labelejecutando.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelejecutando.Name = "labelejecutando";
            this.labelejecutando.Size = new System.Drawing.Size(70, 16);
            this.labelejecutando.TabIndex = 0;
            this.labelejecutando.Text = "ESTADO";
            this.labelejecutando.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // estado48
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(319, 150);
            this.Controls.Add(this.labelejecutando);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "estado48";
            this.ShowIcon = false;
            this.Text = "estado48";
            this.Load += new System.EventHandler(this.estado48_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelejecutando;
    }
}