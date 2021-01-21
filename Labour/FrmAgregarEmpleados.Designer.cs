namespace Labour
{
    partial class FrmAgregarEmpleados
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAgregarEmpleados));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnCargarArchivo = new DevExpress.XtraEditors.SimpleButton();
            this.txtFile = new DevExpress.XtraEditors.TextEdit();
            this.lblMensaje = new DevExpress.XtraEditors.LabelControl();
            this.btnCargarTrabajadores = new DevExpress.XtraEditors.SimpleButton();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitForm3), true, true);
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.btnPlantilla = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtFile.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(19, 33);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(101, 14);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Selecciona Archivo";
            // 
            // btnCargarArchivo
            // 
            this.btnCargarArchivo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCargarArchivo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCargarArchivo.ImageOptions.Image")));
            this.btnCargarArchivo.Location = new System.Drawing.Point(385, 51);
            this.btnCargarArchivo.Name = "btnCargarArchivo";
            this.btnCargarArchivo.Size = new System.Drawing.Size(42, 28);
            this.btnCargarArchivo.TabIndex = 2;
            this.btnCargarArchivo.Click += new System.EventHandler(this.btnCargarArchivo_Click);
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(19, 55);
            this.txtFile.Name = "txtFile";
            this.txtFile.Properties.ReadOnly = true;
            this.txtFile.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtFile_Properties_BeforeShowMenu);
            this.txtFile.Size = new System.Drawing.Size(360, 20);
            this.txtFile.TabIndex = 1;
            // 
            // lblMensaje
            // 
            this.lblMensaje.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblMensaje.Appearance.Options.UseForeColor = true;
            this.lblMensaje.Location = new System.Drawing.Point(122, 81);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(68, 13);
            this.lblMensaje.TabIndex = 1;
            this.lblMensaje.Text = "Procesando...";
            this.lblMensaje.Visible = false;
            // 
            // btnCargarTrabajadores
            // 
            this.btnCargarTrabajadores.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCargarTrabajadores.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCargarTrabajadores.ImageOptions.Image")));
            this.btnCargarTrabajadores.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnCargarTrabajadores.Location = new System.Drawing.Point(20, 81);
            this.btnCargarTrabajadores.Name = "btnCargarTrabajadores";
            this.btnCargarTrabajadores.Size = new System.Drawing.Size(44, 30);
            this.btnCargarTrabajadores.TabIndex = 3;
            this.btnCargarTrabajadores.Click += new System.EventHandler(this.btnCargarTrabajadores_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSalir.Location = new System.Drawing.Point(460, 23);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 30);
            this.btnSalir.TabIndex = 114;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.lblMensaje);
            this.groupControl1.Controls.Add(this.btnCargarArchivo);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Controls.Add(this.btnPlantilla);
            this.groupControl1.Controls.Add(this.txtFile);
            this.groupControl1.Controls.Add(this.btnCargarTrabajadores);
            this.groupControl1.Controls.Add(this.btnSalir);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(518, 126);
            this.groupControl1.TabIndex = 115;
            this.groupControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.groupControl1_Paint);
            // 
            // btnPlantilla
            // 
            this.btnPlantilla.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPlantilla.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPlantilla.ImageOptions.Image")));
            this.btnPlantilla.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnPlantilla.Location = new System.Drawing.Point(70, 81);
            this.btnPlantilla.Name = "btnPlantilla";
            this.btnPlantilla.Size = new System.Drawing.Size(42, 30);
            this.btnPlantilla.TabIndex = 3;
            this.btnPlantilla.ToolTip = "Generar plantilla con variables";
            this.btnPlantilla.Click += new System.EventHandler(this.btnPlantilla_Click);
            // 
            // FrmAgregarEmpleados
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 126);
            this.ControlBox = false;
            this.Controls.Add(this.groupControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAgregarEmpleados";
            this.Text = "Agregar Empleados";
            this.Load += new System.EventHandler(this.FrmAgregarEmpleados_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtFile.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCargarArchivo;
        private DevExpress.XtraEditors.TextEdit txtFile;
        private DevExpress.XtraEditors.LabelControl lblMensaje;
        private DevExpress.XtraEditors.SimpleButton btnCargarTrabajadores;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SimpleButton btnPlantilla;
    }
}