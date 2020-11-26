namespace ZOT.HORAS48.GUI
{
    partial class ComprobarTipoObra
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
            this.buttonAbrir = new System.Windows.Forms.Button();
            this.textBoxNorte = new System.Windows.Forms.TextBox();
            this.buttonNorte = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.buttonGuardar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAbrir
            // 
            this.buttonAbrir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.buttonAbrir.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAbrir.Location = new System.Drawing.Point(220, 316);
            this.buttonAbrir.Name = "buttonAbrir";
            this.buttonAbrir.Size = new System.Drawing.Size(139, 45);
            this.buttonAbrir.TabIndex = 22;
            this.buttonAbrir.Text = "EJECUTAR";
            this.buttonAbrir.UseVisualStyleBackColor = false;
            this.buttonAbrir.Click += new System.EventHandler(this.buttonAbrir_Click);
            // 
            // textBoxNorte
            // 
            this.textBoxNorte.Location = new System.Drawing.Point(101, 85);
            this.textBoxNorte.Name = "textBoxNorte";
            this.textBoxNorte.Size = new System.Drawing.Size(387, 20);
            this.textBoxNorte.TabIndex = 19;
            // 
            // buttonNorte
            // 
            this.buttonNorte.Location = new System.Drawing.Point(504, 83);
            this.buttonNorte.Name = "buttonNorte";
            this.buttonNorte.Size = new System.Drawing.Size(75, 23);
            this.buttonNorte.TabIndex = 16;
            this.buttonNorte.Text = "RUTA";
            this.buttonNorte.UseVisualStyleBackColor = true;
            this.buttonNorte.Click += new System.EventHandler(this.buttonNorte_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "RUTA";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label1.Location = new System.Drawing.Point(18, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 29);
            this.label1.TabIndex = 12;
            this.label1.Text = "48 HORAS";
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(655, 28);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(562, 410);
            this.dataGridView.TabIndex = 23;
            // 
            // buttonGuardar
            // 
            this.buttonGuardar.Location = new System.Drawing.Point(655, 457);
            this.buttonGuardar.Name = "buttonGuardar";
            this.buttonGuardar.Size = new System.Drawing.Size(75, 23);
            this.buttonGuardar.TabIndex = 24;
            this.buttonGuardar.Text = "Guardar";
            this.buttonGuardar.UseVisualStyleBackColor = true;
            this.buttonGuardar.Click += new System.EventHandler(this.buttonGuardar_Click);
            // 
            // ComprobarTipoObra
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1262, 542);
            this.Controls.Add(this.buttonGuardar);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.buttonAbrir);
            this.Controls.Add(this.textBoxNorte);
            this.Controls.Add(this.buttonNorte);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ComprobarTipoObra";
            this.Text = "ComprobarTipoObra";
            this.Load += new System.EventHandler(this.ComprobarTipoObra_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAbrir;
        private System.Windows.Forms.TextBox textBoxNorte;
        private System.Windows.Forms.Button buttonNorte;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button buttonGuardar;
    }
}