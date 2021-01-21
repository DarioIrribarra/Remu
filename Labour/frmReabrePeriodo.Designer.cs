namespace Labour
{
    partial class frmReabrePeriodo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReabrePeriodo));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.btnReabrirPeriodo = new DevExpress.XtraEditors.SimpleButton();
            this.txtAbierto = new DevExpress.XtraEditors.TextEdit();
            this.txtReabre = new DevExpress.XtraEditors.TextEdit();
            this.memoEdit1 = new DevExpress.XtraEditors.MemoEdit();
            this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtMesAbre = new System.Windows.Forms.TextBox();
            this.txtMesAbierto = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtAbierto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReabre.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(24, 25);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(105, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "PERIODO ABIERTO:";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(24, 45);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(99, 13);
            this.labelControl2.TabIndex = 0;
            this.labelControl2.Text = "PERIODO REABRE:";
            // 
            // btnSalir
            // 
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.ImageOptions.Image")));
            this.btnSalir.Location = new System.Drawing.Point(490, 20);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(41, 38);
            this.btnSalir.TabIndex = 11;
            this.btnSalir.TabStop = false;
            this.btnSalir.ToolTip = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnReabrirPeriodo
            // 
            this.btnReabrirPeriodo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReabrirPeriodo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnReabrirPeriodo.ImageOptions.Image")));
            this.btnReabrirPeriodo.Location = new System.Drawing.Point(24, 90);
            this.btnReabrirPeriodo.Name = "btnReabrirPeriodo";
            this.btnReabrirPeriodo.Size = new System.Drawing.Size(85, 35);
            this.btnReabrirPeriodo.TabIndex = 12;
            this.btnReabrirPeriodo.Text = "Reabrir";
            this.btnReabrirPeriodo.Click += new System.EventHandler(this.btnReabrirPeriodo_Click);
            // 
            // txtAbierto
            // 
            this.txtAbierto.Location = new System.Drawing.Point(144, 20);
            this.txtAbierto.Name = "txtAbierto";
            this.txtAbierto.Properties.ReadOnly = true;
            this.txtAbierto.Size = new System.Drawing.Size(70, 20);
            this.txtAbierto.TabIndex = 13;
            // 
            // txtReabre
            // 
            this.txtReabre.Location = new System.Drawing.Point(144, 45);
            this.txtReabre.Name = "txtReabre";
            this.txtReabre.Properties.ReadOnly = true;
            this.txtReabre.Size = new System.Drawing.Size(70, 20);
            this.txtReabre.TabIndex = 13;
            // 
            // memoEdit1
            // 
            this.memoEdit1.EditValue = "Tenga en consideración que si abre periodo anterior toda la informacion del perio" +
    "do abierto se perderá.";
            this.memoEdit1.Location = new System.Drawing.Point(25, 131);
            this.memoEdit1.Name = "memoEdit1";
            this.memoEdit1.Properties.ReadOnly = true;
            this.memoEdit1.Size = new System.Drawing.Size(506, 44);
            this.memoEdit1.TabIndex = 15;
            // 
            // separatorControl1
            // 
            this.separatorControl1.Location = new System.Drawing.Point(6, 70);
            this.separatorControl1.Name = "separatorControl1";
            this.separatorControl1.Size = new System.Drawing.Size(541, 23);
            this.separatorControl1.TabIndex = 14;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtMesAbre);
            this.groupBox1.Controls.Add(this.txtMesAbierto);
            this.groupBox1.Controls.Add(this.memoEdit1);
            this.groupBox1.Controls.Add(this.btnSalir);
            this.groupBox1.Controls.Add(this.btnReabrirPeriodo);
            this.groupBox1.Controls.Add(this.separatorControl1);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Controls.Add(this.txtAbierto);
            this.groupBox1.Controls.Add(this.txtReabre);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(553, 229);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            // 
            // txtMesAbre
            // 
            this.txtMesAbre.Location = new System.Drawing.Point(220, 45);
            this.txtMesAbre.Name = "txtMesAbre";
            this.txtMesAbre.ReadOnly = true;
            this.txtMesAbre.Size = new System.Drawing.Size(100, 21);
            this.txtMesAbre.TabIndex = 16;
            // 
            // txtMesAbierto
            // 
            this.txtMesAbierto.Location = new System.Drawing.Point(220, 20);
            this.txtMesAbierto.Name = "txtMesAbierto";
            this.txtMesAbierto.ReadOnly = true;
            this.txtMesAbierto.Size = new System.Drawing.Size(100, 21);
            this.txtMesAbierto.TabIndex = 16;
            // 
            // frmReabrePeriodo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 251);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmReabrePeriodo";
            this.Text = "Reabrir periodo";
            this.Load += new System.EventHandler(this.frmReabrePeriodo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtAbierto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReabre.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnReabrirPeriodo;
        private DevExpress.XtraEditors.TextEdit txtAbierto;
        private DevExpress.XtraEditors.TextEdit txtReabre;
        private DevExpress.XtraEditors.SeparatorControl separatorControl1;
        private DevExpress.XtraEditors.MemoEdit memoEdit1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtMesAbre;
        private System.Windows.Forms.TextBox txtMesAbierto;
    }
}