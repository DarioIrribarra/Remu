namespace Labour
{
    partial class frmCierreMes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCierreMes));
            this.txtPeriodoActual = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.BtnCerrarMes = new DevExpress.XtraEditors.SimpleButton();
            this.txtNuevoPeriodo = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitFormRemuneraciones), true, true);
            this.txtNuevo = new DevExpress.XtraEditors.TextEdit();
            this.txtActual = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodoActual.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNuevoPeriodo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNuevo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtActual.Properties)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtPeriodoActual
            // 
            this.txtPeriodoActual.Location = new System.Drawing.Point(117, 32);
            this.txtPeriodoActual.Name = "txtPeriodoActual";
            this.txtPeriodoActual.Properties.ReadOnly = true;
            this.txtPeriodoActual.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtPeriodoActual_Properties_BeforeShowMenu);
            this.txtPeriodoActual.Size = new System.Drawing.Size(64, 20);
            this.txtPeriodoActual.TabIndex = 1;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(78, 35);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(34, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Actual:";
            // 
            // BtnCerrarMes
            // 
            this.BtnCerrarMes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnCerrarMes.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("BtnCerrarMes.ImageOptions.Image")));
            this.BtnCerrarMes.Location = new System.Drawing.Point(117, 80);
            this.BtnCerrarMes.Name = "BtnCerrarMes";
            this.BtnCerrarMes.Size = new System.Drawing.Size(81, 37);
            this.BtnCerrarMes.TabIndex = 3;
            this.BtnCerrarMes.Text = "Cerrar";
            this.BtnCerrarMes.ToolTip = "Cerrar mes en curso";
            this.BtnCerrarMes.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Warning;
            this.BtnCerrarMes.Click += new System.EventHandler(this.BtnCerrarMes_Click);
            // 
            // txtNuevoPeriodo
            // 
            this.txtNuevoPeriodo.Location = new System.Drawing.Point(117, 54);
            this.txtNuevoPeriodo.Name = "txtNuevoPeriodo";
            this.txtNuevoPeriodo.Properties.ReadOnly = true;
            this.txtNuevoPeriodo.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtNuevoPeriodo_Properties_BeforeShowMenu);
            this.txtNuevoPeriodo.Size = new System.Drawing.Size(64, 20);
            this.txtNuevoPeriodo.TabIndex = 2;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(78, 58);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(35, 13);
            this.labelControl2.TabIndex = 0;
            this.labelControl2.Text = "Nuevo:";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic);
            this.labelControl3.Appearance.ForeColor = System.Drawing.Color.Black;
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Appearance.Options.UseForeColor = true;
            this.labelControl3.Location = new System.Drawing.Point(61, 139);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(258, 13);
            this.labelControl3.TabIndex = 2;
            this.labelControl3.Text = "* Tenga en consideracion que una vez cerrado el mes";
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // txtNuevo
            // 
            this.txtNuevo.Location = new System.Drawing.Point(184, 53);
            this.txtNuevo.Name = "txtNuevo";
            this.txtNuevo.Properties.AllowFocused = false;
            this.txtNuevo.Properties.ReadOnly = true;
            this.txtNuevo.Size = new System.Drawing.Size(100, 20);
            this.txtNuevo.TabIndex = 4;
            this.txtNuevo.TabStop = false;
            // 
            // txtActual
            // 
            this.txtActual.Location = new System.Drawing.Point(184, 32);
            this.txtActual.Name = "txtActual";
            this.txtActual.Properties.AllowFocused = false;
            this.txtActual.Properties.ReadOnly = true;
            this.txtActual.Size = new System.Drawing.Size(100, 20);
            this.txtActual.TabIndex = 4;
            this.txtActual.TabStop = false;
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic);
            this.labelControl4.Appearance.ForeColor = System.Drawing.Color.Black;
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.Appearance.Options.UseForeColor = true;
            this.labelControl4.Location = new System.Drawing.Point(64, 156);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(161, 13);
            this.labelControl4.TabIndex = 2;
            this.labelControl4.Text = "no podrá realizar nuevos cambios";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.txtNuevo);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Controls.Add(this.txtPeriodoActual);
            this.groupBox1.Controls.Add(this.BtnCerrarMes);
            this.groupBox1.Controls.Add(this.txtActual);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Controls.Add(this.txtNuevoPeriodo);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(356, 230);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // btnSalir
            // 
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.ImageOptions.Image")));
            this.btnSalir.Location = new System.Drawing.Point(374, 20);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(41, 38);
            this.btnSalir.TabIndex = 4;
            this.btnSalir.TabStop = false;
            this.btnSalir.ToolTip = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // frmCierreMes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 256);
            this.ControlBox = false;
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCierreMes";
            this.Text = "Cierre de Mes";
            this.Load += new System.EventHandler(this.frmCierreMes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodoActual.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNuevoPeriodo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNuevo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtActual.Properties)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.TextEdit txtPeriodoActual;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton BtnCerrarMes;
        private DevExpress.XtraEditors.TextEdit txtNuevoPeriodo;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtNuevo;
        private DevExpress.XtraEditors.TextEdit txtActual;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
    }
}