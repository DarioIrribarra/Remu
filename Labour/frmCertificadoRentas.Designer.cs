namespace Labour
{
    partial class frmCertificadoRentas
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCertificadoRentas));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtPeriodo = new DevExpress.XtraEditors.LookUpEdit();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtCondicion = new DevExpress.XtraEditors.TextEdit();
            this.btnCondicion = new DevExpress.XtraEditors.SimpleButton();
            this.BarraCalculo = new DevExpress.XtraEditors.ProgressBarControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.lblNombre = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txtSalida = new DevExpress.XtraEditors.TextEdit();
            this.btnSalida = new DevExpress.XtraEditors.SimpleButton();
            this.guardarPdf = new DevExpress.XtraEditors.SimpleButton();
            this.btnImprimir = new DevExpress.XtraEditors.SimpleButton();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitFormRemuneraciones), true, true);
            this.btnEditarReporte = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCondicion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BarraCalculo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSalida.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(20, 18);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 16);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Periodo:";
            // 
            // txtPeriodo
            // 
            this.txtPeriodo.Location = new System.Drawing.Point(75, 15);
            this.txtPeriodo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPeriodo.Name = "txtPeriodo";
            this.txtPeriodo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtPeriodo.Size = new System.Drawing.Size(142, 22);
            this.txtPeriodo.TabIndex = 1;
            // 
            // cbTodos
            // 
            this.cbTodos.EditValue = true;
            this.cbTodos.Location = new System.Drawing.Point(75, 41);
            this.cbTodos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos los registros de periodo";
            this.cbTodos.Size = new System.Drawing.Size(195, 20);
            this.cbTodos.TabIndex = 2;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(8, 70);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(60, 16);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Condición:";
            // 
            // txtCondicion
            // 
            this.txtCondicion.Enabled = false;
            this.txtCondicion.Location = new System.Drawing.Point(75, 68);
            this.txtCondicion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCondicion.Name = "txtCondicion";
            this.txtCondicion.Size = new System.Drawing.Size(92, 22);
            this.txtCondicion.TabIndex = 3;
            // 
            // btnCondicion
            // 
            this.btnCondicion.Enabled = false;
            this.btnCondicion.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCondicion.ImageOptions.Image")));
            this.btnCondicion.Location = new System.Drawing.Point(171, 65);
            this.btnCondicion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCondicion.Name = "btnCondicion";
            this.btnCondicion.Size = new System.Drawing.Size(30, 28);
            this.btnCondicion.TabIndex = 4;
            this.btnCondicion.Click += new System.EventHandler(this.btnCondicion_Click);
            // 
            // BarraCalculo
            // 
            this.BarraCalculo.Location = new System.Drawing.Point(8, 201);
            this.BarraCalculo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BarraCalculo.Name = "BarraCalculo";
            this.BarraCalculo.Size = new System.Drawing.Size(395, 14);
            this.BarraCalculo.TabIndex = 7;
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(355, 10);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(44, 34);
            this.btnSalir.TabIndex = 13;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // lblNombre
            // 
            this.lblNombre.Location = new System.Drawing.Point(10, 221);
            this.lblNombre.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(32, 16);
            this.lblNombre.TabIndex = 14;
            this.lblNombre.Text = "name";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(8, 98);
            this.labelControl4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(114, 16);
            this.labelControl4.TabIndex = 15;
            this.labelControl4.Text = "Guardar archivo en:";
            // 
            // txtSalida
            // 
            this.txtSalida.Location = new System.Drawing.Point(75, 123);
            this.txtSalida.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSalida.Name = "txtSalida";
            this.txtSalida.Size = new System.Drawing.Size(273, 22);
            this.txtSalida.TabIndex = 16;
            // 
            // btnSalida
            // 
            this.btnSalida.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalida.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalida.ImageOptions.Image")));
            this.btnSalida.Location = new System.Drawing.Point(355, 121);
            this.btnSalida.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalida.Name = "btnSalida";
            this.btnSalida.Size = new System.Drawing.Size(49, 28);
            this.btnSalida.TabIndex = 17;
            this.btnSalida.Click += new System.EventHandler(this.btnSalida_Click);
            // 
            // guardarPdf
            // 
            this.guardarPdf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.guardarPdf.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("guardarPdf.ImageOptions.Image")));
            this.guardarPdf.Location = new System.Drawing.Point(128, 155);
            this.guardarPdf.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.guardarPdf.Name = "guardarPdf";
            this.guardarPdf.Size = new System.Drawing.Size(47, 38);
            this.guardarPdf.TabIndex = 19;
            this.guardarPdf.Click += new System.EventHandler(this.guardarPdf_Click);
            // 
            // btnImprimir
            // 
            this.btnImprimir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImprimir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImprimir.ImageOptions.Image")));
            this.btnImprimir.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnImprimir.Location = new System.Drawing.Point(75, 153);
            this.btnImprimir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(47, 38);
            this.btnImprimir.TabIndex = 20;
            this.btnImprimir.ToolTip = "Generar Documento";
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // btnEditarReporte
            // 
            this.btnEditarReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditarReporte.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditarReporte.ImageOptions.Image")));
            this.btnEditarReporte.Location = new System.Drawing.Point(181, 153);
            this.btnEditarReporte.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEditarReporte.Name = "btnEditarReporte";
            this.btnEditarReporte.Size = new System.Drawing.Size(114, 37);
            this.btnEditarReporte.TabIndex = 25;
            this.btnEditarReporte.Text = "Editar\r\nReporte\r\n";
            this.btnEditarReporte.Click += new System.EventHandler(this.btnEditarReporte_Click);
            // 
            // frmCertificadoRentas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 249);
            this.Controls.Add(this.btnEditarReporte);
            this.Controls.Add(this.btnImprimir);
            this.Controls.Add(this.guardarPdf);
            this.Controls.Add(this.btnSalida);
            this.Controls.Add(this.txtSalida);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.txtPeriodo);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.cbTodos);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.BarraCalculo);
            this.Controls.Add(this.txtCondicion);
            this.Controls.Add(this.btnCondicion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCertificadoRentas";
            this.Text = "Certificado de rentas";
            this.Load += new System.EventHandler(this.frmCertificadoRentas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCondicion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BarraCalculo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSalida.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LookUpEdit txtPeriodo;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtCondicion;
        private DevExpress.XtraEditors.SimpleButton btnCondicion;
        private DevExpress.XtraEditors.ProgressBarControl BarraCalculo;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.LabelControl lblNombre;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtSalida;
        private DevExpress.XtraEditors.SimpleButton btnSalida;
        private DevExpress.XtraEditors.SimpleButton guardarPdf;
        private DevExpress.XtraEditors.SimpleButton btnImprimir;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.SimpleButton btnEditarReporte;
    }
}