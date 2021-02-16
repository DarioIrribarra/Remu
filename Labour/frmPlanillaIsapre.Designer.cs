namespace Labour
{
    partial class frmPlanillaIsapre
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPlanillaIsapre));
            this.labelControl11 = new DevExpress.XtraEditors.LabelControl();
            this.txtIsapre = new DevExpress.XtraEditors.LookUpEdit();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnImpresionRapida = new DevExpress.XtraEditors.SimpleButton();
            this.btnImprimir = new DevExpress.XtraEditors.SimpleButton();
            this.btnBuscar = new DevExpress.XtraEditors.SimpleButton();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitFormRemuneraciones), true, true);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtAgrupacion = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.btnPdf = new DevExpress.XtraEditors.SimpleButton();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.txtConjunto = new DevExpress.XtraEditors.TextEdit();
            this.labelControl12 = new DevExpress.XtraEditors.LabelControl();
            this.txtComboPeriodo = new DevExpress.XtraEditors.LookUpEdit();
            this.btnEditarReporte = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtIsapre.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAgrupacion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl11
            // 
            this.labelControl11.Location = new System.Drawing.Point(23, 38);
            this.labelControl11.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl11.Name = "labelControl11";
            this.labelControl11.Size = new System.Drawing.Size(66, 16);
            this.labelControl11.TabIndex = 41;
            this.labelControl11.Text = "Seleccione:";
            this.labelControl11.Click += new System.EventHandler(this.labelControl11_Click);
            // 
            // txtIsapre
            // 
            this.txtIsapre.EnterMoveNextControl = true;
            this.txtIsapre.Location = new System.Drawing.Point(96, 34);
            this.txtIsapre.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtIsapre.Name = "txtIsapre";
            this.txtIsapre.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtIsapre.Size = new System.Drawing.Size(176, 22);
            this.txtIsapre.TabIndex = 3;
            // 
            // cbTodos
            // 
            this.cbTodos.EditValue = true;
            this.cbTodos.Location = new System.Drawing.Point(96, 90);
            this.cbTodos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos Los registros del periodo en curso";
            this.cbTodos.Size = new System.Drawing.Size(253, 20);
            this.cbTodos.TabIndex = 5;
            this.cbTodos.TabStop = false;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(40, 64);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 16);
            this.labelControl1.TabIndex = 32;
            this.labelControl1.Text = "Periodo:";
            // 
            // btnImpresionRapida
            // 
            this.btnImpresionRapida.AllowFocus = false;
            this.btnImpresionRapida.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImpresionRapida.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImpresionRapida.ImageOptions.Image")));
            this.btnImpresionRapida.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnImpresionRapida.Location = new System.Drawing.Point(253, 199);
            this.btnImpresionRapida.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnImpresionRapida.Name = "btnImpresionRapida";
            this.btnImpresionRapida.Size = new System.Drawing.Size(49, 38);
            this.btnImpresionRapida.TabIndex = 9;
            this.btnImpresionRapida.TabStop = false;
            this.btnImpresionRapida.ToolTip = "Impresion Rapida";
            this.btnImpresionRapida.Click += new System.EventHandler(this.btnImpresionRapida_Click);
            // 
            // btnImprimir
            // 
            this.btnImprimir.AllowFocus = false;
            this.btnImprimir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImprimir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImprimir.ImageOptions.Image")));
            this.btnImprimir.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnImprimir.Location = new System.Drawing.Point(201, 199);
            this.btnImprimir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(49, 38);
            this.btnImprimir.TabIndex = 8;
            this.btnImprimir.TabStop = false;
            this.btnImprimir.ToolTip = "Generar Documento ";
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // btnBuscar
            // 
            this.btnBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBuscar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnBuscar.ImageOptions.Image")));
            this.btnBuscar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnBuscar.Location = new System.Drawing.Point(94, 199);
            this.btnBuscar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(100, 37);
            this.btnBuscar.TabIndex = 8;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.ToolTip = "Guardar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.ImageOptions.Image")));
            this.btnSalir.Location = new System.Drawing.Point(404, 17);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(45, 37);
            this.btnSalir.TabIndex = 10;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnEditarReporte);
            this.groupBox1.Controls.Add(this.txtAgrupacion);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Controls.Add(this.btnPdf);
            this.groupBox1.Controls.Add(this.btnConjunto);
            this.groupBox1.Controls.Add(this.txtConjunto);
            this.groupBox1.Controls.Add(this.labelControl12);
            this.groupBox1.Controls.Add(this.txtComboPeriodo);
            this.groupBox1.Controls.Add(this.btnImpresionRapida);
            this.groupBox1.Controls.Add(this.btnImprimir);
            this.groupBox1.Controls.Add(this.txtIsapre);
            this.groupBox1.Controls.Add(this.btnBuscar);
            this.groupBox1.Controls.Add(this.labelControl11);
            this.groupBox1.Controls.Add(this.cbTodos);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Controls.Add(this.btnSalir);
            this.groupBox1.Location = new System.Drawing.Point(14, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(467, 255);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Isapre";
            // 
            // txtAgrupacion
            // 
            this.txtAgrupacion.Location = new System.Drawing.Point(96, 143);
            this.txtAgrupacion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtAgrupacion.Name = "txtAgrupacion";
            this.txtAgrupacion.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtAgrupacion.Size = new System.Drawing.Size(177, 22);
            this.txtAgrupacion.TabIndex = 56;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(14, 146);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(74, 16);
            this.labelControl3.TabIndex = 55;
            this.labelControl3.Text = "Agrupar por:";
            // 
            // btnPdf
            // 
            this.btnPdf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPdf.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPdf.ImageOptions.Image")));
            this.btnPdf.Location = new System.Drawing.Point(307, 201);
            this.btnPdf.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPdf.Name = "btnPdf";
            this.btnPdf.Size = new System.Drawing.Size(48, 37);
            this.btnPdf.TabIndex = 18;
            this.btnPdf.Click += new System.EventHandler(this.btnPdf_Click);
            // 
            // btnConjunto
            // 
            this.btnConjunto.AllowFocus = false;
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.Enabled = false;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(170, 114);
            this.btnConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(30, 26);
            this.btnConjunto.TabIndex = 54;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // txtConjunto
            // 
            this.txtConjunto.Enabled = false;
            this.txtConjunto.Location = new System.Drawing.Point(96, 116);
            this.txtConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtConjunto.Name = "txtConjunto";
            this.txtConjunto.Size = new System.Drawing.Size(71, 22);
            this.txtConjunto.TabIndex = 43;
            // 
            // labelControl12
            // 
            this.labelControl12.Location = new System.Drawing.Point(28, 121);
            this.labelControl12.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl12.Name = "labelControl12";
            this.labelControl12.Size = new System.Drawing.Size(60, 16);
            this.labelControl12.TabIndex = 19;
            this.labelControl12.Text = "Condición:";
            // 
            // txtComboPeriodo
            // 
            this.txtComboPeriodo.Location = new System.Drawing.Point(96, 62);
            this.txtComboPeriodo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtComboPeriodo.Name = "txtComboPeriodo";
            this.txtComboPeriodo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtComboPeriodo.Size = new System.Drawing.Size(176, 22);
            this.txtComboPeriodo.TabIndex = 42;
            // 
            // btnEditarReporte
            // 
            this.btnEditarReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditarReporte.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditarReporte.ImageOptions.Image")));
            this.btnEditarReporte.Location = new System.Drawing.Point(361, 199);
            this.btnEditarReporte.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEditarReporte.Name = "btnEditarReporte";
            this.btnEditarReporte.Size = new System.Drawing.Size(96, 37);
            this.btnEditarReporte.TabIndex = 57;
            this.btnEditarReporte.Text = "Editar\r\nReporte\r\n";
            this.btnEditarReporte.Click += new System.EventHandler(this.btnEditarReporte_Click);
            // 
            // frmPlanillaIsapre
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 281);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmPlanillaIsapre";
            this.Text = "Planilla Institucion de Salud Previsional (ISAPRE)";
            this.Load += new System.EventHandler(this.frmPlanillaIsapre_Load);
            this.Shown += new System.EventHandler(this.frmPlanillaIsapre_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.txtIsapre.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAgrupacion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton btnImpresionRapida;
        private DevExpress.XtraEditors.SimpleButton btnImprimir;
        private DevExpress.XtraEditors.SimpleButton btnBuscar;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl11;
        private DevExpress.XtraEditors.LookUpEdit txtIsapre;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraEditors.SimpleButton btnPdf;
        private DevExpress.XtraEditors.LookUpEdit txtComboPeriodo;
        private DevExpress.XtraEditors.LabelControl labelControl12;
        private DevExpress.XtraEditors.TextEdit txtConjunto;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.LookUpEdit txtAgrupacion;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnEditarReporte;
    }
}