namespace Labour
{
    partial class frmParametros
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmParametros));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.btnCarta = new DevExpress.XtraEditors.SimpleButton();
            this.btnContrato = new DevExpress.XtraEditors.SimpleButton();
            this.btnFiniquito = new DevExpress.XtraEditors.SimpleButton();
            this.btnGuardar = new DevExpress.XtraEditors.SimpleButton();
            this.txtRutaCartas = new DevExpress.XtraEditors.TextEdit();
            this.txtRutaContratos = new DevExpress.XtraEditors.TextEdit();
            this.txtRutaFiniquito = new DevExpress.XtraEditors.TextEdit();
            this.txtRutaRespaldo = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPorcentaje = new DevExpress.XtraEditors.LookUpEdit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaCartas.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaContratos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaFiniquito.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaRespaldo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPorcentaje.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPorcentaje);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.btnCarta);
            this.groupBox1.Controls.Add(this.btnContrato);
            this.groupBox1.Controls.Add(this.btnFiniquito);
            this.groupBox1.Controls.Add(this.btnGuardar);
            this.groupBox1.Controls.Add(this.txtRutaCartas);
            this.groupBox1.Controls.Add(this.txtRutaContratos);
            this.groupBox1.Controls.Add(this.txtRutaFiniquito);
            this.groupBox1.Controls.Add(this.txtRutaRespaldo);
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Controls.Add(this.btnSalir);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(392, 383);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Información";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureEdit1);
            this.groupBox2.Controls.Add(this.labelControl5);
            this.groupBox2.Location = new System.Drawing.Point(21, 294);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(346, 73);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Importante";
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.EditValue = ((object)(resources.GetObject("pictureEdit1.EditValue")));
            this.pictureEdit1.Location = new System.Drawing.Point(17, 25);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Size = new System.Drawing.Size(38, 33);
            this.pictureEdit1.TabIndex = 19;
            this.pictureEdit1.PopupMenuShowing += new DevExpress.XtraEditors.Events.PopupMenuShowingEventHandler(this.pictureEdit1_PopupMenuShowing);
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Location = new System.Drawing.Point(70, 30);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(259, 26);
            this.labelControl5.TabIndex = 18;
            this.labelControl5.Text = " Por favor verique que los archivos tengan los \r\npermisos necesarios de lectura y" +
    " escritura.";
            // 
            // btnCarta
            // 
            this.btnCarta.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCarta.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCarta.ImageOptions.Image")));
            this.btnCarta.Location = new System.Drawing.Point(326, 176);
            this.btnCarta.Name = "btnCarta";
            this.btnCarta.Size = new System.Drawing.Size(41, 27);
            this.btnCarta.TabIndex = 17;
            this.btnCarta.Click += new System.EventHandler(this.btnCarta_Click);
            // 
            // btnContrato
            // 
            this.btnContrato.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnContrato.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnContrato.ImageOptions.Image")));
            this.btnContrato.Location = new System.Drawing.Point(326, 131);
            this.btnContrato.Name = "btnContrato";
            this.btnContrato.Size = new System.Drawing.Size(41, 27);
            this.btnContrato.TabIndex = 17;
            this.btnContrato.Click += new System.EventHandler(this.btnContrato_Click);
            // 
            // btnFiniquito
            // 
            this.btnFiniquito.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFiniquito.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnFiniquito.ImageOptions.Image")));
            this.btnFiniquito.Location = new System.Drawing.Point(326, 85);
            this.btnFiniquito.Name = "btnFiniquito";
            this.btnFiniquito.Size = new System.Drawing.Size(41, 27);
            this.btnFiniquito.TabIndex = 17;
            this.btnFiniquito.Click += new System.EventHandler(this.btnFiniquito_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.ImageOptions.Image")));
            this.btnGuardar.Location = new System.Drawing.Point(21, 253);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(45, 31);
            this.btnGuardar.TabIndex = 16;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // txtRutaCartas
            // 
            this.txtRutaCartas.Location = new System.Drawing.Point(21, 183);
            this.txtRutaCartas.Name = "txtRutaCartas";
            this.txtRutaCartas.Size = new System.Drawing.Size(299, 20);
            this.txtRutaCartas.TabIndex = 15;
            // 
            // txtRutaContratos
            // 
            this.txtRutaContratos.Location = new System.Drawing.Point(21, 138);
            this.txtRutaContratos.Name = "txtRutaContratos";
            this.txtRutaContratos.Size = new System.Drawing.Size(299, 20);
            this.txtRutaContratos.TabIndex = 15;
            // 
            // txtRutaFiniquito
            // 
            this.txtRutaFiniquito.Location = new System.Drawing.Point(21, 93);
            this.txtRutaFiniquito.Name = "txtRutaFiniquito";
            this.txtRutaFiniquito.Size = new System.Drawing.Size(299, 20);
            this.txtRutaFiniquito.TabIndex = 15;
            // 
            // txtRutaRespaldo
            // 
            this.txtRutaRespaldo.Location = new System.Drawing.Point(21, 48);
            this.txtRutaRespaldo.Name = "txtRutaRespaldo";
            this.txtRutaRespaldo.Size = new System.Drawing.Size(299, 20);
            this.txtRutaRespaldo.TabIndex = 15;
            this.txtRutaRespaldo.ToolTip = "Corresponde a la ruta o directorio ubicado en el servidor.";
            this.txtRutaRespaldo.ToolTipTitle = "Base de Datos";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(21, 164);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(221, 13);
            this.labelControl4.TabIndex = 14;
            this.labelControl4.Text = "Usar plantilla para cartas de aviso ubicada en:\r\n";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(21, 119);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(194, 13);
            this.labelControl3.TabIndex = 14;
            this.labelControl3.Text = "Usar plantilla para contratos ubicada en:\r\n";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(21, 74);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(186, 13);
            this.labelControl2.TabIndex = 14;
            this.labelControl2.Text = "Usar plantilla para finiquito ubicada en:";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(21, 28);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(138, 13);
            this.labelControl1.TabIndex = 14;
            this.labelControl1.Text = "Respaldar base de datos en:";
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(329, 14);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 28);
            this.btnSalir.TabIndex = 13;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 210);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Porcentaje cálculo suspensión laboral:";
            // 
            // txtPorcentaje
            // 
            this.txtPorcentaje.Location = new System.Drawing.Point(24, 227);
            this.txtPorcentaje.Name = "txtPorcentaje";
            this.txtPorcentaje.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtPorcentaje.Size = new System.Drawing.Size(119, 20);
            this.txtPorcentaje.TabIndex = 21;
            // 
            // frmParametros
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 407);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmParametros";
            this.Text = "Parametros";
            this.Load += new System.EventHandler(this.frmParametros_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaCartas.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaContratos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaFiniquito.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaRespaldo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPorcentaje.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.TextEdit txtRutaRespaldo;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnGuardar;
        private DevExpress.XtraEditors.TextEdit txtRutaCartas;
        private DevExpress.XtraEditors.TextEdit txtRutaContratos;
        private DevExpress.XtraEditors.TextEdit txtRutaFiniquito;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton btnCarta;
        private DevExpress.XtraEditors.SimpleButton btnContrato;
        private DevExpress.XtraEditors.SimpleButton btnFiniquito;
        private System.Windows.Forms.GroupBox groupBox2;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LookUpEdit txtPorcentaje;
        private System.Windows.Forms.Label label1;
    }
}