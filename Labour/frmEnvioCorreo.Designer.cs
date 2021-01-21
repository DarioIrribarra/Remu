namespace Labour
{
    partial class frmEnvioCorreo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEnvioCorreo));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnArchivo = new DevExpress.XtraEditors.SimpleButton();
            this.BarraProgresoCorreo = new DevExpress.XtraEditors.ProgressBarControl();
            this.lblMessage = new DevExpress.XtraEditors.LabelControl();
            this.cbOtroMail = new DevExpress.XtraEditors.CheckEdit();
            this.btnConfiguracion = new DevExpress.XtraEditors.SimpleButton();
            this.btnEnviar = new DevExpress.XtraEditors.SimpleButton();
            this.txtRutaAdjunto = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txtMensaje = new DevExpress.XtraEditors.MemoEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtTitulo = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtCorreoDestino = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BarraProgresoCorreo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbOtroMail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaAdjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMensaje.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitulo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCorreoDestino.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnArchivo);
            this.groupBox1.Controls.Add(this.BarraProgresoCorreo);
            this.groupBox1.Controls.Add(this.lblMessage);
            this.groupBox1.Controls.Add(this.cbOtroMail);
            this.groupBox1.Controls.Add(this.btnConfiguracion);
            this.groupBox1.Controls.Add(this.btnEnviar);
            this.groupBox1.Controls.Add(this.txtRutaAdjunto);
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.txtMensaje);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Controls.Add(this.txtTitulo);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Controls.Add(this.txtCorreoDestino);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Location = new System.Drawing.Point(25, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(669, 378);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnArchivo
            // 
            this.btnArchivo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnArchivo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnArchivo.ImageOptions.Image")));
            this.btnArchivo.Location = new System.Drawing.Point(509, 263);
            this.btnArchivo.Name = "btnArchivo";
            this.btnArchivo.Size = new System.Drawing.Size(43, 33);
            this.btnArchivo.TabIndex = 38;
            this.btnArchivo.ToolTip = "Cargar archivo";
            this.btnArchivo.Click += new System.EventHandler(this.btnArchivo_Click);
            // 
            // BarraProgresoCorreo
            // 
            this.BarraProgresoCorreo.Location = new System.Drawing.Point(132, 352);
            this.BarraProgresoCorreo.Name = "BarraProgresoCorreo";
            this.BarraProgresoCorreo.Size = new System.Drawing.Size(516, 18);
            this.BarraProgresoCorreo.TabIndex = 37;
            this.BarraProgresoCorreo.Visible = false;
            // 
            // lblMessage
            // 
            this.lblMessage.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblMessage.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblMessage.Appearance.Options.UseFont = true;
            this.lblMessage.Appearance.Options.UseForeColor = true;
            this.lblMessage.Location = new System.Drawing.Point(132, 335);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(50, 13);
            this.lblMessage.TabIndex = 9;
            this.lblMessage.Text = "Message";
            this.lblMessage.Visible = false;
            // 
            // cbOtroMail
            // 
            this.cbOtroMail.Location = new System.Drawing.Point(131, 22);
            this.cbOtroMail.Name = "cbOtroMail";
            this.cbOtroMail.Properties.Caption = "Deseo ingresar otra dirección de correo";
            this.cbOtroMail.Size = new System.Drawing.Size(214, 19);
            this.cbOtroMail.TabIndex = 1;
            this.cbOtroMail.TabStop = false;
            this.cbOtroMail.CheckedChanged += new System.EventHandler(this.cbOtroMail_CheckedChanged);
            // 
            // btnConfiguracion
            // 
            this.btnConfiguracion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfiguracion.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConfiguracion.ImageOptions.Image")));
            this.btnConfiguracion.Location = new System.Drawing.Point(217, 296);
            this.btnConfiguracion.Name = "btnConfiguracion";
            this.btnConfiguracion.Size = new System.Drawing.Size(42, 33);
            this.btnConfiguracion.TabIndex = 7;
            this.btnConfiguracion.ToolTip = "Ver configuración";
            this.btnConfiguracion.Click += new System.EventHandler(this.btnConfiguracion_Click);
            // 
            // btnEnviar
            // 
            this.btnEnviar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEnviar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEnviar.ImageOptions.Image")));
            this.btnEnviar.Location = new System.Drawing.Point(132, 296);
            this.btnEnviar.Name = "btnEnviar";
            this.btnEnviar.Size = new System.Drawing.Size(79, 33);
            this.btnEnviar.TabIndex = 6;
            this.btnEnviar.Text = "Enviar";
            this.btnEnviar.ToolTip = "Haz click para enviar correo";
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);
            // 
            // txtRutaAdjunto
            // 
            this.txtRutaAdjunto.Location = new System.Drawing.Point(132, 270);
            this.txtRutaAdjunto.Name = "txtRutaAdjunto";
            this.txtRutaAdjunto.Properties.ReadOnly = true;
            this.txtRutaAdjunto.Size = new System.Drawing.Size(370, 20);
            this.txtRutaAdjunto.TabIndex = 5;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(45, 273);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(81, 13);
            this.labelControl4.TabIndex = 32;
            this.labelControl4.Text = "Archivo Adjunto:";
            // 
            // txtMensaje
            // 
            this.txtMensaje.EditValue = "Estimad@:";
            this.txtMensaje.Location = new System.Drawing.Point(132, 105);
            this.txtMensaje.Name = "txtMensaje";
            this.txtMensaje.Size = new System.Drawing.Size(516, 156);
            this.txtMensaje.TabIndex = 4;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(81, 106);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(44, 13);
            this.labelControl3.TabIndex = 36;
            this.labelControl3.Text = "Mensaje:";
            // 
            // txtTitulo
            // 
            this.txtTitulo.EditValue = "Liquidaciones de sueldo";
            this.txtTitulo.EnterMoveNextControl = true;
            this.txtTitulo.Location = new System.Drawing.Point(132, 74);
            this.txtTitulo.Name = "txtTitulo";
            this.txtTitulo.Properties.MaxLength = 200;
            this.txtTitulo.Size = new System.Drawing.Size(232, 20);
            this.txtTitulo.TabIndex = 3;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(52, 78);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(73, 13);
            this.labelControl2.TabIndex = 32;
            this.labelControl2.Text = "Titulo Mensaje:";
            // 
            // txtCorreoDestino
            // 
            this.txtCorreoDestino.EnterMoveNextControl = true;
            this.txtCorreoDestino.Location = new System.Drawing.Point(131, 47);
            this.txtCorreoDestino.Name = "txtCorreoDestino";
            this.txtCorreoDestino.Properties.MaxLength = 100;
            this.txtCorreoDestino.Properties.ReadOnly = true;
            this.txtCorreoDestino.Size = new System.Drawing.Size(233, 20);
            this.txtCorreoDestino.TabIndex = 2;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(27, 50);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(98, 13);
            this.labelControl1.TabIndex = 33;
            this.labelControl1.Text = "Dirección de Correo:";
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(709, 25);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 28);
            this.btnSalir.TabIndex = 8;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // frmEnvioCorreo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 407);
            this.ControlBox = false;
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEnvioCorreo";
            this.Text = "Envío correo ";
            this.Load += new System.EventHandler(this.frmEnvioCorreo_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BarraProgresoCorreo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbOtroMail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaAdjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMensaje.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitulo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCorreoDestino.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraEditors.TextEdit txtRutaAdjunto;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.MemoEdit txtMensaje;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtTitulo;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtCorreoDestino;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckEdit cbOtroMail;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnEnviar;
        private DevExpress.XtraEditors.SimpleButton btnConfiguracion;
        private DevExpress.XtraEditors.LabelControl lblMessage;
        private DevExpress.XtraEditors.ProgressBarControl BarraProgresoCorreo;
        private DevExpress.XtraEditors.SimpleButton btnArchivo;
    }
}