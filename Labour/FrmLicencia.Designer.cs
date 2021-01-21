namespace Labour
{
    partial class FrmLicencia
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLicencia));
            this.txtCodigo = new DevExpress.XtraEditors.MemoEdit();
            this.lblUsers = new DevExpress.XtraEditors.LabelControl();
            this.lblSesiones = new DevExpress.XtraEditors.LabelControl();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.lblline = new DevExpress.XtraEditors.LabelControl();
            this.lblFooter = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.lblDireccion = new DevExpress.XtraEditors.LabelControl();
            this.lblServerinfo = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.lblExpira = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.lblDiasExp = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(19, 102);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Properties.ReadOnly = true;
            this.txtCodigo.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.memoEdit1_Properties_BeforeShowMenu);
            this.txtCodigo.Size = new System.Drawing.Size(420, 65);
            this.txtCodigo.TabIndex = 0;
            this.txtCodigo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCodigo_KeyDown);
            // 
            // lblUsers
            // 
            this.lblUsers.Location = new System.Drawing.Point(20, 173);
            this.lblUsers.Name = "lblUsers";
            this.lblUsers.Size = new System.Drawing.Size(45, 13);
            this.lblUsers.TabIndex = 2;
            this.lblUsers.Text = "Usuarios:";
            // 
            // lblSesiones
            // 
            this.lblSesiones.Location = new System.Drawing.Point(21, 192);
            this.lblSesiones.Name = "lblSesiones";
            this.lblSesiones.Size = new System.Drawing.Size(46, 13);
            this.lblSesiones.TabIndex = 2;
            this.lblSesiones.Text = "Sesiones:";
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.EditValue = global::Labour.Properties.Resources.sopytec_logo;
            this.pictureEdit1.Location = new System.Drawing.Point(18, 12);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            this.pictureEdit1.Properties.PopupMenuShowing += new DevExpress.XtraEditors.Events.PopupMenuShowingEventHandler(this.pictureEdit1_Properties_PopupMenuShowing);
            this.pictureEdit1.Size = new System.Drawing.Size(156, 63);
            this.pictureEdit1.TabIndex = 126;
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(401, 12);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 28);
            this.btnSalir.TabIndex = 125;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // lblline
            // 
            this.lblline.Location = new System.Drawing.Point(5, 267);
            this.lblline.Name = "lblline";
            this.lblline.Size = new System.Drawing.Size(436, 13);
            this.lblline.TabIndex = 127;
            this.lblline.Text = "---------------------------------------------------------------------------------" +
    "----------------------------";
            // 
            // lblFooter
            // 
            this.lblFooter.Location = new System.Drawing.Point(101, 287);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Size = new System.Drawing.Size(211, 13);
            this.lblFooter.TabIndex = 127;
            this.lblFooter.Text = "Sopytec S.A Todos los derechos reservados";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(21, 84);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(41, 13);
            this.labelControl1.TabIndex = 128;
            this.labelControl1.Text = "Licencia:";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(143, 306);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(121, 13);
            this.labelControl2.TabIndex = 129;
            this.labelControl2.Text = "http://www.sopytec.com";
            // 
            // lblDireccion
            // 
            this.lblDireccion.Location = new System.Drawing.Point(21, 229);
            this.lblDireccion.Name = "lblDireccion";
            this.lblDireccion.Size = new System.Drawing.Size(14, 13);
            this.lblDireccion.TabIndex = 130;
            this.lblDireccion.Text = "Ip:";
            // 
            // lblServerinfo
            // 
            this.lblServerinfo.Location = new System.Drawing.Point(21, 248);
            this.lblServerinfo.Name = "lblServerinfo";
            this.lblServerinfo.Size = new System.Drawing.Size(63, 13);
            this.lblServerinfo.TabIndex = 131;
            this.lblServerinfo.Text = "labelControl3";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(280, 83);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(85, 13);
            this.labelControl3.TabIndex = 132;
            this.labelControl3.Text = "Fecha Expiración:";
            // 
            // lblExpira
            // 
            this.lblExpira.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblExpira.Appearance.ForeColor = System.Drawing.Color.Green;
            this.lblExpira.Appearance.Options.UseFont = true;
            this.lblExpira.Appearance.Options.UseForeColor = true;
            this.lblExpira.Location = new System.Drawing.Point(367, 83);
            this.lblExpira.Name = "lblExpira";
            this.lblExpira.Size = new System.Drawing.Size(68, 13);
            this.lblExpira.TabIndex = 133;
            this.lblExpira.Text = "20/12/2019";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(21, 211);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(101, 13);
            this.labelControl4.TabIndex = 134;
            this.labelControl4.Text = "Tu licencia expira en:";
            // 
            // lblDiasExp
            // 
            this.lblDiasExp.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDiasExp.Appearance.Options.UseFont = true;
            this.lblDiasExp.Location = new System.Drawing.Point(124, 211);
            this.lblDiasExp.Name = "lblDiasExp";
            this.lblDiasExp.Size = new System.Drawing.Size(75, 13);
            this.lblDiasExp.TabIndex = 135;
            this.lblDiasExp.Text = "labelControl5";
            // 
            // FrmLicencia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 327);
            this.ControlBox = false;
            this.Controls.Add(this.lblDiasExp);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.lblExpira);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.lblServerinfo);
            this.Controls.Add(this.lblDireccion);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.lblFooter);
            this.Controls.Add(this.lblline);
            this.Controls.Add(this.pictureEdit1);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblSesiones);
            this.Controls.Add(this.lblUsers);
            this.Controls.Add(this.txtCodigo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLicencia";
            this.Text = "Licencia";
            this.Load += new System.EventHandler(this.FrmLicencia_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.MemoEdit txtCodigo;
        private DevExpress.XtraEditors.LabelControl lblUsers;
        private DevExpress.XtraEditors.LabelControl lblSesiones;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraEditors.LabelControl lblline;
        private DevExpress.XtraEditors.LabelControl lblFooter;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl lblDireccion;
        private DevExpress.XtraEditors.LabelControl lblServerinfo;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl lblExpira;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl lblDiasExp;
    }
}