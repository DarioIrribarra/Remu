namespace Labour
{
    partial class frmConfCorreo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfCorreo));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblMessage = new DevExpress.XtraEditors.LabelControl();
            this.btnTestMail = new DevExpress.XtraEditors.SimpleButton();
            this.btnGuardar = new DevExpress.XtraEditors.SimpleButton();
            this.cbSsl = new DevExpress.XtraEditors.CheckEdit();
            this.txtPasswordConfirm = new DevExpress.XtraEditors.TextEdit();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtMail = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtPuerto = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtSmtp = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.ProgressBar = new DevExpress.XtraEditors.ProgressBarControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbSsl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPasswordConfirm.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPuerto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSmtp.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProgressBar.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSalir);
            this.groupBox1.Controls.Add(this.lblMessage);
            this.groupBox1.Controls.Add(this.btnTestMail);
            this.groupBox1.Controls.Add(this.btnGuardar);
            this.groupBox1.Controls.Add(this.cbSsl);
            this.groupBox1.Controls.Add(this.txtPasswordConfirm);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.labelControl5);
            this.groupBox1.Controls.Add(this.txtMail);
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Controls.Add(this.txtPuerto);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Controls.Add(this.txtSmtp);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Location = new System.Drawing.Point(28, 30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(423, 230);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parametros";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // lblMessage
            // 
            this.lblMessage.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblMessage.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblMessage.Appearance.Options.UseFont = true;
            this.lblMessage.Appearance.Options.UseForeColor = true;
            this.lblMessage.Location = new System.Drawing.Point(118, 210);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(50, 13);
            this.lblMessage.TabIndex = 4;
            this.lblMessage.Text = "Message";
            this.lblMessage.Visible = false;
            // 
            // btnTestMail
            // 
            this.btnTestMail.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTestMail.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnTestMail.ImageOptions.Image")));
            this.btnTestMail.Location = new System.Drawing.Point(192, 170);
            this.btnTestMail.Name = "btnTestMail";
            this.btnTestMail.Size = new System.Drawing.Size(58, 35);
            this.btnTestMail.TabIndex = 8;
            this.btnTestMail.Text = "Test";
            this.btnTestMail.ToolTip = "Enviar correo de prueba";
            this.btnTestMail.Click += new System.EventHandler(this.btnTestMail_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.ImageOptions.Image")));
            this.btnGuardar.Location = new System.Drawing.Point(118, 170);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(68, 35);
            this.btnGuardar.TabIndex = 7;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.ToolTip = "Guardar configuración";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // cbSsl
            // 
            this.cbSsl.Location = new System.Drawing.Point(118, 149);
            this.cbSsl.Name = "cbSsl";
            this.cbSsl.Properties.AllowFocused = false;
            this.cbSsl.Properties.Caption = "ssl";
            this.cbSsl.Size = new System.Drawing.Size(51, 19);
            this.cbSsl.TabIndex = 6;
            this.cbSsl.TabStop = false;
            // 
            // txtPasswordConfirm
            // 
            this.txtPasswordConfirm.EnterMoveNextControl = true;
            this.txtPasswordConfirm.Location = new System.Drawing.Point(118, 102);
            this.txtPasswordConfirm.Name = "txtPasswordConfirm";
            this.txtPasswordConfirm.Properties.MaxLength = 50;
            this.txtPasswordConfirm.Properties.PasswordChar = '*';
            this.txtPasswordConfirm.Size = new System.Drawing.Size(184, 20);
            this.txtPasswordConfirm.TabIndex = 4;
            this.txtPasswordConfirm.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPasswordConfirm_KeyDown);
            // 
            // txtPassword
            // 
            this.txtPassword.EnterMoveNextControl = true;
            this.txtPassword.Location = new System.Drawing.Point(118, 79);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.MaxLength = 50;
            this.txtPassword.Properties.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(184, 20);
            this.txtPassword.TabIndex = 3;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(13, 106);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(96, 13);
            this.labelControl5.TabIndex = 0;
            this.labelControl5.Text = "Confirme password:";
            // 
            // txtMail
            // 
            this.txtMail.EnterMoveNextControl = true;
            this.txtMail.Location = new System.Drawing.Point(118, 55);
            this.txtMail.Name = "txtMail";
            this.txtMail.Properties.MaxLength = 50;
            this.txtMail.Size = new System.Drawing.Size(184, 20);
            this.txtMail.TabIndex = 2;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(59, 82);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(50, 13);
            this.labelControl4.TabIndex = 0;
            this.labelControl4.Text = "Password:";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(81, 58);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(28, 13);
            this.labelControl2.TabIndex = 0;
            this.labelControl2.Text = "Email:";
            // 
            // txtPuerto
            // 
            this.txtPuerto.EditValue = "25";
            this.txtPuerto.EnterMoveNextControl = true;
            this.txtPuerto.Location = new System.Drawing.Point(118, 126);
            this.txtPuerto.Name = "txtPuerto";
            this.txtPuerto.Properties.MaxLength = 5;
            this.txtPuerto.Size = new System.Drawing.Size(51, 20);
            this.txtPuerto.TabIndex = 5;
            this.txtPuerto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPuerto_KeyPress);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(73, 129);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(36, 13);
            this.labelControl3.TabIndex = 0;
            this.labelControl3.Text = "Puerto:";
            // 
            // txtSmtp
            // 
            this.txtSmtp.EnterMoveNextControl = true;
            this.txtSmtp.Location = new System.Drawing.Point(118, 29);
            this.txtSmtp.Name = "txtSmtp";
            this.txtSmtp.Properties.MaxLength = 50;
            this.txtSmtp.Size = new System.Drawing.Size(184, 20);
            this.txtSmtp.TabIndex = 1;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(39, 32);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(70, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Servidor smtp:";
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(28, 8);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(423, 18);
            this.ProgressBar.TabIndex = 1;
            this.ProgressBar.Visible = false;
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(383, 10);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 30);
            this.btnSalir.TabIndex = 6;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // frmConfCorreo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 274);
            this.ControlBox = false;
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfCorreo";
            this.Text = "Configuración correo";
            this.Load += new System.EventHandler(this.frmConfCorreo_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbSsl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPasswordConfirm.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPuerto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSmtp.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProgressBar.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraEditors.TextEdit txtSmtp;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckEdit cbSsl;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraEditors.TextEdit txtMail;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtPuerto;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnTestMail;
        private DevExpress.XtraEditors.SimpleButton btnGuardar;
        private DevExpress.XtraEditors.TextEdit txtPasswordConfirm;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl lblMessage;
        private DevExpress.XtraEditors.ProgressBarControl ProgressBar;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
    }
}