namespace BancoItau
{
    partial class FrmBancoItau
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmBancoItau));
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::BancoItau.WaitFormRemuneraciones), true, true);
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnEditarReporte = new DevExpress.XtraEditors.SimpleButton();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.cbxFormatoArchivo = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.btnReporte = new DevExpress.XtraEditors.SimpleButton();
            this.chkCambiarCuenta = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtNumeroCuenta = new DevExpress.XtraEditors.TextEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.txtDescripcion = new System.Windows.Forms.RichTextBox();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.lblMedioRespaldo = new DevExpress.XtraEditors.LabelControl();
            this.cbxCargarMetodopago = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.btnRutaArchivo = new DevExpress.XtraEditors.SimpleButton();
            this.txtRuta = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.btnSend = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbxFormatoArchivo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCambiarCuenta.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumeroCuenta.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbxCargarMetodopago.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRuta.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnEditarReporte);
            this.panelControl1.Controls.Add(this.btnSalir);
            this.panelControl1.Controls.Add(this.cbxFormatoArchivo);
            this.panelControl1.Controls.Add(this.labelControl3);
            this.panelControl1.Controls.Add(this.btnReporte);
            this.panelControl1.Controls.Add(this.chkCambiarCuenta);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.txtNumeroCuenta);
            this.panelControl1.Controls.Add(this.labelControl7);
            this.panelControl1.Controls.Add(this.txtDescripcion);
            this.panelControl1.Controls.Add(this.labelControl6);
            this.panelControl1.Controls.Add(this.lblMedioRespaldo);
            this.panelControl1.Controls.Add(this.cbxCargarMetodopago);
            this.panelControl1.Controls.Add(this.labelControl5);
            this.panelControl1.Controls.Add(this.btnRutaArchivo);
            this.panelControl1.Controls.Add(this.txtRuta);
            this.panelControl1.Controls.Add(this.labelControl4);
            this.panelControl1.Controls.Add(this.btnSend);
            this.panelControl1.Location = new System.Drawing.Point(14, 31);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(619, 249);
            this.panelControl1.TabIndex = 8;
            // 
            // btnEditarReporte
            // 
            this.btnEditarReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditarReporte.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditarReporte.ImageOptions.Image")));
            this.btnEditarReporte.Location = new System.Drawing.Point(228, 210);
            this.btnEditarReporte.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEditarReporte.Name = "btnEditarReporte";
            this.btnEditarReporte.Size = new System.Drawing.Size(114, 37);
            this.btnEditarReporte.TabIndex = 25;
            this.btnEditarReporte.Text = "Editar\r\nReporte\r\n";
            this.btnEditarReporte.Click += new System.EventHandler(this.btnEditarReporte_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.Location = new System.Drawing.Point(569, 6);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(44, 37);
            this.btnSalir.TabIndex = 24;
            this.btnSalir.TabStop = false;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // cbxFormatoArchivo
            // 
            this.cbxFormatoArchivo.Location = new System.Drawing.Point(114, 2);
            this.cbxFormatoArchivo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbxFormatoArchivo.Name = "cbxFormatoArchivo";
            this.cbxFormatoArchivo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbxFormatoArchivo.Properties.DropDownRows = 2;
            this.cbxFormatoArchivo.Size = new System.Drawing.Size(211, 22);
            this.cbxFormatoArchivo.TabIndex = 22;
            this.cbxFormatoArchivo.EditValueChanged += new System.EventHandler(this.cbxFormatoArchivo_EditValueChanged);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(17, 6);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(98, 16);
            this.labelControl3.TabIndex = 23;
            this.labelControl3.Text = "Formato archivo:";
            // 
            // btnReporte
            // 
            this.btnReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReporte.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnReporte.ImageOptions.Image")));
            this.btnReporte.Location = new System.Drawing.Point(168, 209);
            this.btnReporte.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnReporte.Name = "btnReporte";
            this.btnReporte.Size = new System.Drawing.Size(54, 38);
            this.btnReporte.TabIndex = 16;
            this.btnReporte.Click += new System.EventHandler(this.btnReporte_Click);
            // 
            // chkCambiarCuenta
            // 
            this.chkCambiarCuenta.Location = new System.Drawing.Point(461, 69);
            this.chkCambiarCuenta.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkCambiarCuenta.Name = "chkCambiarCuenta";
            this.chkCambiarCuenta.Properties.Caption = "Cambiar Número cuenta";
            this.chkCambiarCuenta.Size = new System.Drawing.Size(160, 20);
            this.chkCambiarCuenta.TabIndex = 21;
            this.chkCambiarCuenta.CheckedChanged += new System.EventHandler(this.chkCambiarCuenta_CheckedChanged);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(332, 71);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(133, 17);
            this.labelControl1.TabIndex = 20;
            this.labelControl1.Text = "Ingrese solo números";
            // 
            // txtNumeroCuenta
            // 
            this.txtNumeroCuenta.Location = new System.Drawing.Point(114, 68);
            this.txtNumeroCuenta.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtNumeroCuenta.Name = "txtNumeroCuenta";
            this.txtNumeroCuenta.Properties.MaxLength = 10;
            this.txtNumeroCuenta.Size = new System.Drawing.Size(212, 22);
            this.txtNumeroCuenta.TabIndex = 19;
            // 
            // labelControl7
            // 
            this.labelControl7.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic);
            this.labelControl7.Appearance.Options.UseFont = true;
            this.labelControl7.Location = new System.Drawing.Point(332, 110);
            this.labelControl7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(56, 17);
            this.labelControl7.TabIndex = 16;
            this.labelControl7.Text = "Opcional";
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(114, 106);
            this.txtDescripcion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(210, 59);
            this.txtDescripcion.TabIndex = 5;
            this.txtDescripcion.Text = "";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(45, 106);
            this.labelControl6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(70, 16);
            this.labelControl6.TabIndex = 14;
            this.labelControl6.Text = "Descripción:";
            // 
            // lblMedioRespaldo
            // 
            this.lblMedioRespaldo.Location = new System.Drawing.Point(6, 71);
            this.lblMedioRespaldo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblMedioRespaldo.Name = "lblMedioRespaldo";
            this.lblMedioRespaldo.Size = new System.Drawing.Size(110, 16);
            this.lblMedioRespaldo.TabIndex = 11;
            this.lblMedioRespaldo.Text = "Número de cuenta:";
            // 
            // cbxCargarMetodopago
            // 
            this.cbxCargarMetodopago.Location = new System.Drawing.Point(114, 34);
            this.cbxCargarMetodopago.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbxCargarMetodopago.Name = "cbxCargarMetodopago";
            this.cbxCargarMetodopago.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbxCargarMetodopago.Size = new System.Drawing.Size(211, 22);
            this.cbxCargarMetodopago.TabIndex = 3;
            this.cbxCargarMetodopago.EditValueChanged += new System.EventHandler(this.cbxCargarMetodopago_EditValueChanged);
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(24, 38);
            this.labelControl5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(92, 16);
            this.labelControl5.TabIndex = 9;
            this.labelControl5.Text = "Medio respaldo:";
            // 
            // btnRutaArchivo
            // 
            this.btnRutaArchivo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRutaArchivo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnRutaArchivo.ImageOptions.Image")));
            this.btnRutaArchivo.Location = new System.Drawing.Point(472, 171);
            this.btnRutaArchivo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRutaArchivo.Name = "btnRutaArchivo";
            this.btnRutaArchivo.Size = new System.Drawing.Size(48, 28);
            this.btnRutaArchivo.TabIndex = 6;
            this.btnRutaArchivo.Click += new System.EventHandler(this.btnRutaArchivo_Click);
            // 
            // txtRuta
            // 
            this.txtRuta.Location = new System.Drawing.Point(114, 174);
            this.txtRuta.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtRuta.Name = "txtRuta";
            this.txtRuta.Size = new System.Drawing.Size(351, 22);
            this.txtRuta.TabIndex = 7;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(41, 171);
            this.labelControl4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(72, 16);
            this.labelControl4.TabIndex = 6;
            this.labelControl4.Text = "Ruta Archivo";
            // 
            // btnSend
            // 
            this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSend.ImageOptions.Image")));
            this.btnSend.Location = new System.Drawing.Point(114, 207);
            this.btnSend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(48, 41);
            this.btnSend.TabIndex = 8;
            this.btnSend.ToolTip = "Generar archivo";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(16, 7);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(122, 16);
            this.labelControl2.TabIndex = 7;
            this.labelControl2.Text = "Información adicional";
            // 
            // FrmBancoItau
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 294);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.labelControl2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FrmBancoItau";
            this.Text = "Banco Itaú Corpbanca";
            this.Load += new System.EventHandler(this.FrmBancoItau_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbxFormatoArchivo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCambiarCuenta.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumeroCuenta.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbxCargarMetodopago.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRuta.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnRutaArchivo;
        private DevExpress.XtraEditors.TextEdit txtRuta;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.SimpleButton btnSend;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LookUpEdit cbxCargarMetodopago;
        private DevExpress.XtraEditors.LabelControl lblMedioRespaldo;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private System.Windows.Forms.RichTextBox txtDescripcion;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.TextEdit txtNumeroCuenta;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckEdit chkCambiarCuenta;
        private DevExpress.XtraEditors.SimpleButton btnReporte;
        private DevExpress.XtraEditors.LookUpEdit cbxFormatoArchivo;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnEditarReporte;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
    }
}

