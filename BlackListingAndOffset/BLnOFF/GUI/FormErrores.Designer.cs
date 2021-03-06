﻿namespace ZOT.BLnOFF.GUI
{
    partial class FormErrores
    {/// <summary>
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.advancedDataGridViewCol = new Zuby.ADGV.AdvancedDataGridView();
            this.advancedDataGridViewSearchToolBar1 = new Zuby.ADGV.AdvancedDataGridViewSearchToolBar();
            this.Borrarfiltros = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.advancedDataGridViewCol)).BeginInit();
            this.advancedDataGridViewSearchToolBar1.SuspendLayout();
            this.SuspendLayout();
            // 
            // advancedDataGridViewCol
            // 
            this.advancedDataGridViewCol.AllowDrop = true;
            this.advancedDataGridViewCol.AllowUserToAddRows = false;
            this.advancedDataGridViewCol.AllowUserToOrderColumns = true;
            this.advancedDataGridViewCol.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedDataGridViewCol.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.advancedDataGridViewCol.BackgroundColor = System.Drawing.Color.White;
            this.advancedDataGridViewCol.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.DodgerBlue;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.advancedDataGridViewCol.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.advancedDataGridViewCol.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.advancedDataGridViewCol.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.advancedDataGridViewCol.FilterAndSortEnabled = true;
            this.advancedDataGridViewCol.Location = new System.Drawing.Point(0, 30);
            this.advancedDataGridViewCol.Name = "advancedDataGridViewCol";
            this.advancedDataGridViewCol.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.advancedDataGridViewCol.RowHeadersVisible = false;
            this.advancedDataGridViewCol.Size = new System.Drawing.Size(1047, 506);
            this.advancedDataGridViewCol.TabIndex = 1;
            this.advancedDataGridViewCol.KeyDown += new System.Windows.Forms.KeyEventHandler(this.advancedDataGridViewCol_KeyDown);
            this.advancedDataGridViewCol.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.advancedDataGridViewCol_KeyPress);
            // 
            // advancedDataGridViewSearchToolBar1
            // 
            this.advancedDataGridViewSearchToolBar1.AllowMerge = false;
            this.advancedDataGridViewSearchToolBar1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.advancedDataGridViewSearchToolBar1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Borrarfiltros,
            this.toolStripSeparator1,
            this.toolStripButton1});
            this.advancedDataGridViewSearchToolBar1.Location = new System.Drawing.Point(0, 0);
            this.advancedDataGridViewSearchToolBar1.MaximumSize = new System.Drawing.Size(0, 27);
            this.advancedDataGridViewSearchToolBar1.MinimumSize = new System.Drawing.Size(0, 27);
            this.advancedDataGridViewSearchToolBar1.Name = "advancedDataGridViewSearchToolBar1";
            this.advancedDataGridViewSearchToolBar1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.advancedDataGridViewSearchToolBar1.Size = new System.Drawing.Size(1047, 27);
            this.advancedDataGridViewSearchToolBar1.TabIndex = 0;
            this.advancedDataGridViewSearchToolBar1.Search += new Zuby.ADGV.AdvancedDataGridViewSearchToolBarSearchEventHandler(this.advancedDataGridViewSearchToolBar1_Search);
            this.advancedDataGridViewSearchToolBar1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.advancedDataGridViewSearchToolBar1_ItemClicked_1);
            // 
            // Borrarfiltros
            // 
            this.Borrarfiltros.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Borrarfiltros.Image = global::ZOT.Properties.Resources.borrarfiltros;
            this.Borrarfiltros.Name = "Borrarfiltros";
            this.Borrarfiltros.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.Borrarfiltros.Size = new System.Drawing.Size(23, 24);
            this.Borrarfiltros.ToolTipText = "Borrar filtros";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.AccessibleName = "ayudabtn";
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::ZOT.Properties.Resources.help_icono;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripButton1.Size = new System.Drawing.Size(23, 24);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.ToolTipText = "AYUDA";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // FormErrores
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1047, 536);
            this.Controls.Add(this.advancedDataGridViewSearchToolBar1);
            this.Controls.Add(this.advancedDataGridViewCol);
            this.Name = "FormErrores";
            this.ShowIcon = false;
            this.Text = "Errores";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formColindancias_FormClosing);
            this.Load += new System.EventHandler(this.formColindancias_Load);
            ((System.ComponentModel.ISupportInitialize)(this.advancedDataGridViewCol)).EndInit();
            this.advancedDataGridViewSearchToolBar1.ResumeLayout(false);
            this.advancedDataGridViewSearchToolBar1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Zuby.ADGV.AdvancedDataGridView advancedDataGridViewCol;
        private Zuby.ADGV.AdvancedDataGridViewSearchToolBar advancedDataGridViewSearchToolBar1;
        private System.Windows.Forms.ToolStripButton Borrarfiltros;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}