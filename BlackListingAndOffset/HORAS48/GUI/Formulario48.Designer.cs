namespace ZOT.HORAS48.GUI
{
    partial class Formulario48
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Formulario48));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonNorte = new System.Windows.Forms.Button();
            this.buttonSur = new System.Windows.Forms.Button();
            this.buttonCyL = new System.Windows.Forms.Button();
            this.textBoxNorte = new System.Windows.Forms.TextBox();
            this.textBoxSur = new System.Windows.Forms.TextBox();
            this.textBoxCyL = new System.Windows.Forms.TextBox();
            this.buttonAbrirNorte = new System.Windows.Forms.Button();
            this.dataGridFinal = new System.Windows.Forms.DataGridView();
            this.tmpejecucion = new System.Windows.Forms.Label();
            this.buttonExportar = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxnombrefichero = new System.Windows.Forms.TextBox();
            this.labelNombreFichero = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.filaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFinal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filaBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label1.Location = new System.Drawing.Point(82, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "48 HORAS";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(84, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "NORTE";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(84, 261);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Castilla y León";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(84, 178);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "SUR";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // buttonNorte
            // 
            this.buttonNorte.Location = new System.Drawing.Point(568, 89);
            this.buttonNorte.Name = "buttonNorte";
            this.buttonNorte.Size = new System.Drawing.Size(75, 23);
            this.buttonNorte.TabIndex = 5;
            this.buttonNorte.Text = "Norte";
            this.buttonNorte.UseVisualStyleBackColor = true;
            this.buttonNorte.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonSur
            // 
            this.buttonSur.Location = new System.Drawing.Point(568, 173);
            this.buttonSur.Name = "buttonSur";
            this.buttonSur.Size = new System.Drawing.Size(75, 23);
            this.buttonSur.TabIndex = 6;
            this.buttonSur.Text = "Sur";
            this.buttonSur.UseVisualStyleBackColor = true;
            this.buttonSur.Click += new System.EventHandler(this.buttonSur_Click);
            // 
            // buttonCyL
            // 
            this.buttonCyL.Location = new System.Drawing.Point(568, 256);
            this.buttonCyL.Name = "buttonCyL";
            this.buttonCyL.Size = new System.Drawing.Size(75, 23);
            this.buttonCyL.TabIndex = 7;
            this.buttonCyL.Text = "CyL";
            this.buttonCyL.UseVisualStyleBackColor = true;
            this.buttonCyL.Click += new System.EventHandler(this.buttonCyL_Click);
            // 
            // textBoxNorte
            // 
            this.textBoxNorte.Location = new System.Drawing.Point(165, 91);
            this.textBoxNorte.Name = "textBoxNorte";
            this.textBoxNorte.Size = new System.Drawing.Size(387, 20);
            this.textBoxNorte.TabIndex = 8;
            this.textBoxNorte.TextChanged += new System.EventHandler(this.textBoxNorte_TextChanged);
            // 
            // textBoxSur
            // 
            this.textBoxSur.Location = new System.Drawing.Point(165, 175);
            this.textBoxSur.Name = "textBoxSur";
            this.textBoxSur.Size = new System.Drawing.Size(387, 20);
            this.textBoxSur.TabIndex = 9;
            this.textBoxSur.TextChanged += new System.EventHandler(this.textBoxSur_TextChanged);
            // 
            // textBoxCyL
            // 
            this.textBoxCyL.Location = new System.Drawing.Point(165, 258);
            this.textBoxCyL.Name = "textBoxCyL";
            this.textBoxCyL.Size = new System.Drawing.Size(387, 20);
            this.textBoxCyL.TabIndex = 10;
            this.textBoxCyL.TextChanged += new System.EventHandler(this.textBoxCyL_TextChanged);
            // 
            // buttonAbrirNorte
            // 
            this.buttonAbrirNorte.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.buttonAbrirNorte.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAbrirNorte.Location = new System.Drawing.Point(284, 322);
            this.buttonAbrirNorte.Name = "buttonAbrirNorte";
            this.buttonAbrirNorte.Size = new System.Drawing.Size(139, 45);
            this.buttonAbrirNorte.TabIndex = 11;
            this.buttonAbrirNorte.Text = "EJECUTAR";
            this.buttonAbrirNorte.UseVisualStyleBackColor = false;
            this.buttonAbrirNorte.Click += new System.EventHandler(this.buttonAbrirNorte_Click);
            // 
            // dataGridFinal
            // 
            this.dataGridFinal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridFinal.Location = new System.Drawing.Point(746, 12);
            this.dataGridFinal.Name = "dataGridFinal";
            this.dataGridFinal.Size = new System.Drawing.Size(987, 498);
            this.dataGridFinal.TabIndex = 12;
            // 
            // tmpejecucion
            // 
            this.tmpejecucion.AutoSize = true;
            this.tmpejecucion.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tmpejecucion.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.tmpejecucion.Location = new System.Drawing.Point(642, 480);
            this.tmpejecucion.MaximumSize = new System.Drawing.Size(30, 30);
            this.tmpejecucion.MinimumSize = new System.Drawing.Size(70, 30);
            this.tmpejecucion.Name = "tmpejecucion";
            this.tmpejecucion.Size = new System.Drawing.Size(70, 30);
            this.tmpejecucion.TabIndex = 13;
            this.tmpejecucion.Text = "0";
            this.tmpejecucion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonExportar
            // 
            this.buttonExportar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.buttonExportar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonExportar.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExportar.ForeColor = System.Drawing.Color.Black;
            this.buttonExportar.Location = new System.Drawing.Point(1089, 525);
            this.buttonExportar.Name = "buttonExportar";
            this.buttonExportar.Size = new System.Drawing.Size(357, 62);
            this.buttonExportar.TabIndex = 14;
            this.buttonExportar.Text = "EXPORTAR EXCEL";
            this.buttonExportar.UseVisualStyleBackColor = false;
            this.buttonExportar.Visible = false;
            this.buttonExportar.Click += new System.EventHandler(this.buttonExportar_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(501, 488);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(135, 16);
            this.label5.TabIndex = 15;
            this.label5.Text = "Tiempo de ejecución";
            // 
            // textBoxnombrefichero
            // 
            this.textBoxnombrefichero.Location = new System.Drawing.Point(883, 567);
            this.textBoxnombrefichero.MaximumSize = new System.Drawing.Size(50, 20);
            this.textBoxnombrefichero.MinimumSize = new System.Drawing.Size(200, 20);
            this.textBoxnombrefichero.Name = "textBoxnombrefichero";
            this.textBoxnombrefichero.Size = new System.Drawing.Size(200, 20);
            this.textBoxnombrefichero.TabIndex = 16;
            this.textBoxnombrefichero.Text = "SALIDA_REVISION_NIR96";
            this.textBoxnombrefichero.Visible = false;
            this.textBoxnombrefichero.TextChanged += new System.EventHandler(this.textBox1_TextChanged_1);
            // 
            // labelNombreFichero
            // 
            this.labelNombreFichero.AutoSize = true;
            this.labelNombreFichero.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNombreFichero.Location = new System.Drawing.Point(880, 546);
            this.labelNombreFichero.Name = "labelNombreFichero";
            this.labelNombreFichero.Size = new System.Drawing.Size(134, 18);
            this.labelNombreFichero.TabIndex = 17;
            this.labelNombreFichero.Text = "Nombre del fichero";
            this.labelNombreFichero.Visible = false;
            // 
            // button1
            // 
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(548, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(95, 51);
            this.button1.TabIndex = 18;
            this.button1.Text = "AYUDA";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_3);
            // 
            // filaBindingSource
            // 
            this.filaBindingSource.DataSource = typeof(ZOT.HORAS48.Code.Fila);
            // 
            // Formulario48
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1788, 622);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelNombreFichero);
            this.Controls.Add(this.textBoxnombrefichero);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonExportar);
            this.Controls.Add(this.tmpejecucion);
            this.Controls.Add(this.dataGridFinal);
            this.Controls.Add(this.buttonAbrirNorte);
            this.Controls.Add(this.textBoxCyL);
            this.Controls.Add(this.textBoxSur);
            this.Controls.Add(this.textBoxNorte);
            this.Controls.Add(this.buttonCyL);
            this.Controls.Add(this.buttonSur);
            this.Controls.Add(this.buttonNorte);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Formulario48";
            this.Text = "Formulario48";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridFinal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filaBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonNorte;
        private System.Windows.Forms.Button buttonSur;
        private System.Windows.Forms.Button buttonCyL;
        private System.Windows.Forms.TextBox textBoxNorte;
        private System.Windows.Forms.TextBox textBoxSur;
        private System.Windows.Forms.TextBox textBoxCyL;
        private System.Windows.Forms.Button buttonAbrirNorte;
        private System.Windows.Forms.BindingSource filaBindingSource;
        private System.Windows.Forms.DataGridView dataGridFinal;
        private System.Windows.Forms.Label tmpejecucion;
        private System.Windows.Forms.Button buttonExportar;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxnombrefichero;
        private System.Windows.Forms.Label labelNombreFichero;
        internal System.Windows.Forms.Button button1;
    }
}