namespace Labour
{
    partial class frmPrevired
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrevired));
            this.txtConjunto = new DevExpress.XtraEditors.TextEdit();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.cbCsv = new DevExpress.XtraEditors.CheckEdit();
            this.cbTxt = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnResumen = new DevExpress.XtraEditors.SimpleButton();
            this.btnGenerar = new DevExpress.XtraEditors.SimpleButton();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitFormRemuneraciones), true, true);
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.txtComboPeriodo = new DevExpress.XtraEditors.LookUpEdit();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
            this.btnEditarReporte = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbCsv.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTxt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtConjunto
            // 
            this.txtConjunto.Location = new System.Drawing.Point(77, 90);
            this.txtConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtConjunto.Name = "txtConjunto";
            this.txtConjunto.Size = new System.Drawing.Size(65, 22);
            this.txtConjunto.TabIndex = 3;
            this.txtConjunto.DoubleClick += new System.EventHandler(this.txtConjunto_DoubleClick);
            this.txtConjunto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtConjunto_KeyPress);
            // 
            // cbTodos
            // 
            this.cbTodos.Location = new System.Drawing.Point(77, 64);
            this.cbTodos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos los registros del periodo";
            this.cbTodos.Size = new System.Drawing.Size(215, 20);
            this.cbTodos.TabIndex = 2;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbConjunto_CheckedChanged);
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(77, 122);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(94, 16);
            this.labelControl3.TabIndex = 10;
            this.labelControl3.Text = "Formato salida";
            // 
            // cbCsv
            // 
            this.cbCsv.Location = new System.Drawing.Point(140, 145);
            this.cbCsv.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbCsv.Name = "cbCsv";
            this.cbCsv.Properties.Caption = ".csv";
            this.cbCsv.Size = new System.Drawing.Size(87, 20);
            this.cbCsv.TabIndex = 5;
            // 
            // cbTxt
            // 
            this.cbTxt.EditValue = true;
            this.cbTxt.Location = new System.Drawing.Point(77, 145);
            this.cbTxt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTxt.Name = "cbTxt";
            this.cbTxt.Properties.Caption = ".txt";
            this.cbTxt.Size = new System.Drawing.Size(56, 20);
            this.cbTxt.TabIndex = 4;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(23, 41);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 16);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Periodo:";
            // 
            // btnResumen
            // 
            this.btnResumen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnResumen.Enabled = false;
            this.btnResumen.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnResumen.ImageOptions.Image")));
            this.btnResumen.Location = new System.Drawing.Point(118, 208);
            this.btnResumen.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnResumen.Name = "btnResumen";
            this.btnResumen.Size = new System.Drawing.Size(104, 39);
            this.btnResumen.TabIndex = 9;
            this.btnResumen.Text = "Resumen";
            this.btnResumen.Click += new System.EventHandler(this.btnResumen_Click);
            // 
            // btnGenerar
            // 
            this.btnGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGenerar.ImageOptions.Image")));
            this.btnGenerar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGenerar.Location = new System.Drawing.Point(14, 208);
            this.btnGenerar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(99, 39);
            this.btnGenerar.TabIndex = 8;
            this.btnGenerar.Text = "Generar";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.ImageOptions.Image")));
            this.btnSalir.Location = new System.Drawing.Point(537, 32);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(44, 37);
            this.btnSalir.TabIndex = 7;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.btnEditarReporte);
            this.groupControl1.Controls.Add(this.txtComboPeriodo);
            this.groupControl1.Controls.Add(this.btnConjunto);
            this.groupControl1.Controls.Add(this.separatorControl1);
            this.groupControl1.Controls.Add(this.cbCsv);
            this.groupControl1.Controls.Add(this.btnSalir);
            this.groupControl1.Controls.Add(this.btnResumen);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Controls.Add(this.btnGenerar);
            this.groupControl1.Controls.Add(this.cbTxt);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Controls.Add(this.txtConjunto);
            this.groupControl1.Controls.Add(this.cbTodos);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(602, 258);
            this.groupControl1.TabIndex = 10;
            this.groupControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupControl1_Paint);
            // 
            // txtComboPeriodo
            // 
            this.txtComboPeriodo.Location = new System.Drawing.Point(77, 37);
            this.txtComboPeriodo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtComboPeriodo.Name = "txtComboPeriodo";
            this.txtComboPeriodo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtComboPeriodo.Properties.PopupSizeable = false;
            this.txtComboPeriodo.Size = new System.Drawing.Size(159, 22);
            this.txtComboPeriodo.TabIndex = 11;
            // 
            // btnConjunto
            // 
            this.btnConjunto.AllowFocus = false;
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(149, 89);
            this.btnConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(30, 26);
            this.btnConjunto.TabIndex = 12;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // separatorControl1
            // 
            this.separatorControl1.Location = new System.Drawing.Point(14, 176);
            this.separatorControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.separatorControl1.Name = "separatorControl1";
            this.separatorControl1.Padding = new System.Windows.Forms.Padding(10, 11, 10, 11);
            this.separatorControl1.Size = new System.Drawing.Size(567, 28);
            this.separatorControl1.TabIndex = 11;
            // 
            // btnEditarReporte
            // 
            this.btnEditarReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditarReporte.Enabled = false;
            this.btnEditarReporte.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditarReporte.ImageOptions.Image")));
            this.btnEditarReporte.Location = new System.Drawing.Point(228, 208);
            this.btnEditarReporte.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEditarReporte.Name = "btnEditarReporte";
            this.btnEditarReporte.Size = new System.Drawing.Size(114, 37);
            this.btnEditarReporte.TabIndex = 25;
            this.btnEditarReporte.Text = "Editar\r\nReporte\r\n";
            this.btnEditarReporte.Click += new System.EventHandler(this.btnEditarReporte_Click);
            // 
            // frmPrevired
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 258);
            this.ControlBox = false;
            this.Controls.Add(this.groupControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPrevired";
            this.Text = "Archivo previred";
            this.Load += new System.EventHandler(this.frmPrevired_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbCsv.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTxt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnGenerar;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.CheckEdit cbCsv;
        private DevExpress.XtraEditors.CheckEdit cbTxt;
        private DevExpress.XtraEditors.TextEdit txtConjunto;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.SimpleButton btnResumen;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SeparatorControl separatorControl1;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.LookUpEdit txtComboPeriodo;
        private DevExpress.XtraEditors.SimpleButton btnEditarReporte;
    }
}