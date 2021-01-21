namespace Labour
{
    partial class frmColumnasData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmColumnasData));
            this.ListBoxData = new DevExpress.XtraEditors.ListBoxControl();
            this.ListBoxResultante = new DevExpress.XtraEditors.ListBoxControl();
            this.btnAgregar = new DevExpress.XtraEditors.SimpleButton();
            this.btnEliminar = new DevExpress.XtraEditors.SimpleButton();
            this.btnAgregaMultiple = new DevExpress.XtraEditors.SimpleButton();
            this.btnGuardar = new DevExpress.XtraEditors.SimpleButton();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnQuitar = new DevExpress.XtraEditors.SimpleButton();
            this.btnQuitarMultiple = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.lblNumPrimary = new DevExpress.XtraEditors.LabelControl();
            this.lblNumResultante = new DevExpress.XtraEditors.LabelControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.ListBoxData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ListBoxResultante)).BeginInit();
            this.SuspendLayout();
            // 
            // ListBoxData
            // 
            this.ListBoxData.Cursor = System.Windows.Forms.Cursors.Default;
            this.ListBoxData.Location = new System.Drawing.Point(12, 32);
            this.ListBoxData.Name = "ListBoxData";
            this.ListBoxData.Size = new System.Drawing.Size(173, 335);
            this.ListBoxData.TabIndex = 0;
            // 
            // ListBoxResultante
            // 
            this.ListBoxResultante.Cursor = System.Windows.Forms.Cursors.Default;
            this.ListBoxResultante.Location = new System.Drawing.Point(250, 32);
            this.ListBoxResultante.Name = "ListBoxResultante";
            this.ListBoxResultante.Size = new System.Drawing.Size(174, 335);
            this.ListBoxResultante.TabIndex = 1;
            // 
            // btnAgregar
            // 
            this.btnAgregar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAgregar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAgregar.ImageOptions.Image")));
            this.btnAgregar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnAgregar.Location = new System.Drawing.Point(195, 53);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(41, 33);
            this.btnAgregar.TabIndex = 2;
            this.btnAgregar.ToolTip = "Agregar campo";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminar.ImageOptions.Image")));
            this.btnEliminar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnEliminar.Location = new System.Drawing.Point(195, 247);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(41, 33);
            this.btnEliminar.TabIndex = 2;
            this.btnEliminar.ToolTip = "Eliminar campo";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnAgregaMultiple
            // 
            this.btnAgregaMultiple.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAgregaMultiple.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAgregaMultiple.ImageOptions.Image")));
            this.btnAgregaMultiple.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnAgregaMultiple.Location = new System.Drawing.Point(195, 92);
            this.btnAgregaMultiple.Name = "btnAgregaMultiple";
            this.btnAgregaMultiple.Size = new System.Drawing.Size(41, 33);
            this.btnAgregaMultiple.TabIndex = 2;
            this.btnAgregaMultiple.ToolTip = "Agregar todos los campos";
            this.btnAgregaMultiple.Click += new System.EventHandler(this.btnAgregaMultiple_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.ImageOptions.Image")));
            this.btnGuardar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGuardar.Location = new System.Drawing.Point(195, 286);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(41, 33);
            this.btnGuardar.TabIndex = 2;
            this.btnGuardar.ToolTip = "Confirmar seleccion";
            this.btnGuardar.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Question;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.ImageOptions.Image")));
            this.btnRefresh.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnRefresh.Location = new System.Drawing.Point(195, 131);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(41, 33);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.ToolTip = "Volver a estado original";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnQuitar
            // 
            this.btnQuitar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnQuitar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnQuitar.ImageOptions.Image")));
            this.btnQuitar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnQuitar.Location = new System.Drawing.Point(195, 170);
            this.btnQuitar.Name = "btnQuitar";
            this.btnQuitar.Size = new System.Drawing.Size(41, 33);
            this.btnQuitar.TabIndex = 2;
            this.btnQuitar.ToolTip = "Regresar campo";
            this.btnQuitar.Click += new System.EventHandler(this.btnQuitar_Click);
            // 
            // btnQuitarMultiple
            // 
            this.btnQuitarMultiple.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnQuitarMultiple.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnQuitarMultiple.ImageOptions.Image")));
            this.btnQuitarMultiple.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnQuitarMultiple.Location = new System.Drawing.Point(195, 208);
            this.btnQuitarMultiple.Name = "btnQuitarMultiple";
            this.btnQuitarMultiple.Size = new System.Drawing.Size(41, 33);
            this.btnQuitarMultiple.TabIndex = 2;
            this.btnQuitarMultiple.ToolTip = "Regresar todos los campos";
            this.btnQuitarMultiple.Click += new System.EventHandler(this.btnQuitarMultiple_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 12);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(59, 13);
            this.labelControl1.TabIndex = 3;
            this.labelControl1.Text = "Campos #:  ";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(256, 12);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(56, 13);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Campos #: ";
            // 
            // lblNumPrimary
            // 
            this.lblNumPrimary.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblNumPrimary.Appearance.Options.UseFont = true;
            this.lblNumPrimary.Location = new System.Drawing.Point(71, 12);
            this.lblNumPrimary.Name = "lblNumPrimary";
            this.lblNumPrimary.Size = new System.Drawing.Size(31, 13);
            this.lblNumPrimary.TabIndex = 4;
            this.lblNumPrimary.Text = "value";
            // 
            // lblNumResultante
            // 
            this.lblNumResultante.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblNumResultante.Appearance.Options.UseFont = true;
            this.lblNumResultante.Location = new System.Drawing.Point(317, 12);
            this.lblNumResultante.Name = "lblNumResultante";
            this.lblNumResultante.Size = new System.Drawing.Size(31, 13);
            this.lblNumResultante.TabIndex = 4;
            this.lblNumResultante.Text = "value";
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSalir.Location = new System.Drawing.Point(195, 322);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(41, 30);
            this.btnSalir.TabIndex = 115;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // frmColumnasData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 373);
            this.ControlBox = false;
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblNumResultante);
            this.Controls.Add(this.lblNumPrimary);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.btnAgregaMultiple);
            this.Controls.Add(this.btnQuitarMultiple);
            this.Controls.Add(this.btnQuitar);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.ListBoxResultante);
            this.Controls.Add(this.ListBoxData);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmColumnasData";
            this.Text = "Columnas";
            this.Load += new System.EventHandler(this.frmColumnasData_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ListBoxData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ListBoxResultante)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.ListBoxControl ListBoxData;
        private DevExpress.XtraEditors.ListBoxControl ListBoxResultante;
        private DevExpress.XtraEditors.SimpleButton btnAgregar;
        private DevExpress.XtraEditors.SimpleButton btnEliminar;
        private DevExpress.XtraEditors.SimpleButton btnAgregaMultiple;
        private DevExpress.XtraEditors.SimpleButton btnGuardar;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnQuitar;
        private DevExpress.XtraEditors.SimpleButton btnQuitarMultiple;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl lblNumPrimary;
        private DevExpress.XtraEditors.LabelControl lblNumResultante;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
    }
}