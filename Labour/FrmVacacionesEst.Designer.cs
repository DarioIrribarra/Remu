namespace Labour
{
    partial class FrmVacacionesEst
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmVacacionesEst));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.dtFecha = new DevExpress.XtraEditors.DateEdit();
            this.btnProcesar = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.cbtodos = new DevExpress.XtraEditors.CheckEdit();
            this.txtConjunto = new DevExpress.XtraEditors.TextEdit();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtAgrupa = new DevExpress.XtraEditors.LookUpEdit();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitFormRemuneraciones), true, true);
            this.btnEditarReporte = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.dtFecha.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFecha.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbtodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAgrupa.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(36, 95);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(225, 16);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Calcular hasta la siguiente fecha límite:";
            // 
            // dtFecha
            // 
            this.dtFecha.EditValue = null;
            this.dtFecha.Location = new System.Drawing.Point(37, 118);
            this.dtFecha.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dtFecha.Name = "dtFecha";
            this.dtFecha.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFecha.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFecha.Size = new System.Drawing.Size(146, 22);
            this.dtFecha.TabIndex = 1;
            // 
            // btnProcesar
            // 
            this.btnProcesar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProcesar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnProcesar.ImageOptions.Image")));
            this.btnProcesar.Location = new System.Drawing.Point(36, 208);
            this.btnProcesar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnProcesar.Name = "btnProcesar";
            this.btnProcesar.Size = new System.Drawing.Size(110, 44);
            this.btnProcesar.TabIndex = 2;
            this.btnProcesar.Text = "Procesar";
            this.btnProcesar.Click += new System.EventHandler(this.btnProcesar_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(36, 53);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(60, 16);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Condición:";
            // 
            // cbtodos
            // 
            this.cbtodos.Location = new System.Drawing.Point(36, 16);
            this.cbtodos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbtodos.Name = "cbtodos";
            this.cbtodos.Properties.Caption = "Todos los registros del periodo";
            this.cbtodos.Size = new System.Drawing.Size(217, 20);
            this.cbtodos.TabIndex = 4;
            this.cbtodos.CheckedChanged += new System.EventHandler(this.cbtodos_CheckedChanged);
            // 
            // txtConjunto
            // 
            this.txtConjunto.Location = new System.Drawing.Point(103, 50);
            this.txtConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtConjunto.Name = "txtConjunto";
            this.txtConjunto.Properties.MaxLength = 12;
            this.txtConjunto.Size = new System.Drawing.Size(80, 22);
            this.txtConjunto.TabIndex = 5;
            // 
            // btnConjunto
            // 
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(189, 44);
            this.btnConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(36, 32);
            this.btnConjunto.TabIndex = 6;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.ImageOptions.Image")));
            this.btnSalir.Location = new System.Drawing.Point(414, 10);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(42, 37);
            this.btnSalir.TabIndex = 116;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(37, 149);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(74, 16);
            this.labelControl3.TabIndex = 117;
            this.labelControl3.Text = "Agrupar por:";
            // 
            // txtAgrupa
            // 
            this.txtAgrupa.Location = new System.Drawing.Point(37, 170);
            this.txtAgrupa.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtAgrupa.Name = "txtAgrupa";
            this.txtAgrupa.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtAgrupa.Size = new System.Drawing.Size(146, 22);
            this.txtAgrupa.TabIndex = 118;
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // btnEditarReporte
            // 
            this.btnEditarReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditarReporte.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditarReporte.ImageOptions.Image")));
            this.btnEditarReporte.Location = new System.Drawing.Point(152, 208);
            this.btnEditarReporte.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEditarReporte.Name = "btnEditarReporte";
            this.btnEditarReporte.Size = new System.Drawing.Size(114, 44);
            this.btnEditarReporte.TabIndex = 119;
            this.btnEditarReporte.Text = "Editar\r\nReporte\r\n";
            this.btnEditarReporte.Click += new System.EventHandler(this.btnEditarReporte_Click);
            // 
            // FrmVacacionesEst
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 273);
            this.Controls.Add(this.btnEditarReporte);
            this.Controls.Add(this.txtAgrupa);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.btnConjunto);
            this.Controls.Add(this.txtConjunto);
            this.Controls.Add(this.cbtodos);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.btnProcesar);
            this.Controls.Add(this.dtFecha);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmVacacionesEst";
            this.Text = "Resumen Vacaciones";
            this.Load += new System.EventHandler(this.FrmVacacionesEst_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtFecha.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFecha.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbtodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAgrupa.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.DateEdit dtFecha;
        private DevExpress.XtraEditors.SimpleButton btnProcesar;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.CheckEdit cbtodos;
        private DevExpress.XtraEditors.TextEdit txtConjunto;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LookUpEdit txtAgrupa;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.SimpleButton btnEditarReporte;
    }
}