namespace ZOT.HORAS48.GUI
{
    partial class borrradosporque
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
            this.dataGridViewporque = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewfaltanEstados = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewporque)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewfaltanEstados)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewporque
            // 
            this.dataGridViewporque.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewporque.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridViewporque.Location = new System.Drawing.Point(24, 59);
            this.dataGridViewporque.Name = "dataGridViewporque";
            this.dataGridViewporque.Size = new System.Drawing.Size(279, 295);
            this.dataGridViewporque.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(477, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Problemas encontrados";
            // 
            // dataGridViewfaltanEstados
            // 
            this.dataGridViewfaltanEstados.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridViewfaltanEstados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewfaltanEstados.Location = new System.Drawing.Point(370, 59);
            this.dataGridViewfaltanEstados.Name = "dataGridViewfaltanEstados";
            this.dataGridViewfaltanEstados.Size = new System.Drawing.Size(397, 295);
            this.dataGridViewfaltanEstados.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(367, 357);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(436, 54);
            this.label2.TabIndex = 3;
            this.label2.Text = "Si se detectan Estados sin Tipo deberas copiarlos y\r\n rellenarlos en el Excel \"Ta" +
    "bla\"\r\no ejecutar la herramienta para detectar Estados sin Tipo.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(93, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 18);
            this.label3.TabIndex = 4;
            this.label3.Text = "Borrado porque";
            // 
            // borrradosporque
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridViewfaltanEstados);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewporque);
            this.Name = "borrradosporque";
            this.Text = "Borrardo porque + Información";
            this.Load += new System.EventHandler(this.borrradosporque_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewporque)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewfaltanEstados)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewporque;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridViewfaltanEstados;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}