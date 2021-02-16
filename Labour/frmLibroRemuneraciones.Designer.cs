namespace Labour
{
    partial class frmLibroRemuneraciones
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLibroRemuneraciones));
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnGuardar = new DevExpress.XtraEditors.SimpleButton();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitFormRemuneraciones), true, true);
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.btnConfLibro = new DevExpress.XtraEditors.SimpleButton();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.btnTablasExcel = new DevExpress.XtraEditors.SimpleButton();
            this.txtGrupo = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.cbxSeleccionConjunto = new DevExpress.XtraEditors.LookUpEdit();
            this.lookUpEdit1 = new DevExpress.XtraEditors.LookUpEdit();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
            this.btnEditarReporte = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGrupo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbxSeleccionConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lookUpEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // cbTodos
            // 
            this.cbTodos.Location = new System.Drawing.Point(103, 85);
            this.cbTodos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos los registros del periodo";
            this.cbTodos.Size = new System.Drawing.Size(225, 20);
            this.cbTodos.TabIndex = 3;
            this.cbTodos.TabStop = false;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(49, 57);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 16);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Periodo:";
            // 
            // btnGuardar
            // 
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.ImageOptions.Image")));
            this.btnGuardar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGuardar.Location = new System.Drawing.Point(22, 215);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(111, 37);
            this.btnGuardar.TabIndex = 6;
            this.btnGuardar.Text = "Generar";
            this.btnGuardar.ToolTip = "Generar Libro";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.ImageOptions.Image")));
            this.btnSalir.Location = new System.Drawing.Point(413, 32);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(41, 37);
            this.btnSalir.TabIndex = 8;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnConfLibro
            // 
            this.btnConfLibro.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfLibro.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConfLibro.ImageOptions.Image")));
            this.btnConfLibro.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnConfLibro.Location = new System.Drawing.Point(242, 215);
            this.btnConfLibro.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConfLibro.Name = "btnConfLibro";
            this.btnConfLibro.Size = new System.Drawing.Size(50, 37);
            this.btnConfLibro.TabIndex = 7;
            this.btnConfLibro.ToolTip = "Configurar libro";
            this.btnConfLibro.Click += new System.EventHandler(this.btnConfLibro_Click);
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.btnEditarReporte);
            this.groupControl1.Controls.Add(this.btnTablasExcel);
            this.groupControl1.Controls.Add(this.txtGrupo);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Controls.Add(this.labelControl2);
            this.groupControl1.Controls.Add(this.cbxSeleccionConjunto);
            this.groupControl1.Controls.Add(this.lookUpEdit1);
            this.groupControl1.Controls.Add(this.btnConjunto);
            this.groupControl1.Controls.Add(this.separatorControl1);
            this.groupControl1.Controls.Add(this.btnGuardar);
            this.groupControl1.Controls.Add(this.btnSalir);
            this.groupControl1.Controls.Add(this.btnConfLibro);
            this.groupControl1.Controls.Add(this.cbTodos);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(471, 268);
            this.groupControl1.TabIndex = 40;
            // 
            // btnTablasExcel
            // 
            this.btnTablasExcel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTablasExcel.ImageOptions.Image = global::Labour.Properties.Resources.exporttoxls_32x32;
            this.btnTablasExcel.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnTablasExcel.Location = new System.Drawing.Point(139, 215);
            this.btnTablasExcel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnTablasExcel.Name = "btnTablasExcel";
            this.btnTablasExcel.Size = new System.Drawing.Size(91, 37);
            this.btnTablasExcel.TabIndex = 46;
            this.btnTablasExcel.Text = "Tabla \r\ndatos";
            this.btnTablasExcel.ToolTip = "Generar Tablas";
            this.btnTablasExcel.Click += new System.EventHandler(this.btnTablasExcel_Click);
            // 
            // txtGrupo
            // 
            this.txtGrupo.Location = new System.Drawing.Point(103, 145);
            this.txtGrupo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtGrupo.Name = "txtGrupo";
            this.txtGrupo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtGrupo.Size = new System.Drawing.Size(162, 22);
            this.txtGrupo.TabIndex = 45;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(22, 148);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(74, 16);
            this.labelControl3.TabIndex = 44;
            this.labelControl3.Text = "Agrupar por:";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(37, 118);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(60, 16);
            this.labelControl2.TabIndex = 43;
            this.labelControl2.Text = "Condición:";
            // 
            // cbxSeleccionConjunto
            // 
            this.cbxSeleccionConjunto.Location = new System.Drawing.Point(103, 116);
            this.cbxSeleccionConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbxSeleccionConjunto.Name = "cbxSeleccionConjunto";
            this.cbxSeleccionConjunto.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbxSeleccionConjunto.Size = new System.Drawing.Size(117, 22);
            this.cbxSeleccionConjunto.TabIndex = 42;
            // 
            // lookUpEdit1
            // 
            this.lookUpEdit1.Location = new System.Drawing.Point(103, 53);
            this.lookUpEdit1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lookUpEdit1.Name = "lookUpEdit1";
            this.lookUpEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lookUpEdit1.Size = new System.Drawing.Size(184, 22);
            this.lookUpEdit1.TabIndex = 41;
            // 
            // btnConjunto
            // 
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(226, 116);
            this.btnConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(38, 25);
            this.btnConjunto.TabIndex = 5;
            this.btnConjunto.ToolTip = "Ver formulario condiciones";
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // separatorControl1
            // 
            this.separatorControl1.Location = new System.Drawing.Point(6, 182);
            this.separatorControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.separatorControl1.Name = "separatorControl1";
            this.separatorControl1.Padding = new System.Windows.Forms.Padding(10, 11, 10, 11);
            this.separatorControl1.Size = new System.Drawing.Size(457, 26);
            this.separatorControl1.TabIndex = 40;
            // 
            // btnEditarReporte
            // 
            this.btnEditarReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditarReporte.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditarReporte.ImageOptions.Image")));
            this.btnEditarReporte.Location = new System.Drawing.Point(298, 215);
            this.btnEditarReporte.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEditarReporte.Name = "btnEditarReporte";
            this.btnEditarReporte.Size = new System.Drawing.Size(114, 37);
            this.btnEditarReporte.TabIndex = 47;
            this.btnEditarReporte.Text = "Editar\r\nReporte\r\n";
            this.btnEditarReporte.Click += new System.EventHandler(this.btnEditarReporte_Click);
            // 
            // frmLibroRemuneraciones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 268);
            this.ControlBox = false;
            this.Controls.Add(this.groupControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLibroRemuneraciones";
            this.Text = "Libro Remuneraciones";
            this.Load += new System.EventHandler(this.cbActual_Load);
            this.Shown += new System.EventHandler(this.frmLibroRemuneraciones_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGrupo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbxSeleccionConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lookUpEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnGuardar;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnConfLibro;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SeparatorControl separatorControl1;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.LookUpEdit lookUpEdit1;
        private DevExpress.XtraEditors.LookUpEdit cbxSeleccionConjunto;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LookUpEdit txtGrupo;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnTablasExcel;
        private DevExpress.XtraEditors.SimpleButton btnEditarReporte;
    }
}