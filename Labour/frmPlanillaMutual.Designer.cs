﻿namespace Labour
{
    partial class frmPlanillaMutual
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPlanillaMutual));
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.btnImpresionRapida = new DevExpress.XtraEditors.SimpleButton();
            this.btnImprimir = new DevExpress.XtraEditors.SimpleButton();
            this.btnBuscar = new DevExpress.XtraEditors.SimpleButton();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitFormRemuneraciones), true, true);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnPdf = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.txtConjunto = new DevExpress.XtraEditors.TextEdit();
            this.txtComboPeriodo = new DevExpress.XtraEditors.LookUpEdit();
            this.txtAgrupa = new DevExpress.XtraEditors.LookUpEdit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAgrupa.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // cbTodos
            // 
            this.cbTodos.EditValue = true;
            this.cbTodos.Location = new System.Drawing.Point(70, 42);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.AllowFocused = false;
            this.cbTodos.Properties.Caption = "Todos Los registros del periodo en curso";
            this.cbTodos.Size = new System.Drawing.Size(217, 19);
            this.cbTodos.TabIndex = 1;
            this.cbTodos.TabStop = false;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(4, 92);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(58, 13);
            this.labelControl2.TabIndex = 37;
            this.labelControl2.Text = "Agrupa por:";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(24, 24);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(40, 13);
            this.labelControl1.TabIndex = 32;
            this.labelControl1.Text = "Periodo:";
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.ImageOptions.Image")));
            this.btnSalir.Location = new System.Drawing.Point(363, 16);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(35, 30);
            this.btnSalir.TabIndex = 9;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnImpresionRapida
            // 
            this.btnImpresionRapida.AllowFocus = false;
            this.btnImpresionRapida.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImpresionRapida.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImpresionRapida.ImageOptions.Image")));
            this.btnImpresionRapida.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnImpresionRapida.Location = new System.Drawing.Point(207, 115);
            this.btnImpresionRapida.Name = "btnImpresionRapida";
            this.btnImpresionRapida.Size = new System.Drawing.Size(42, 31);
            this.btnImpresionRapida.TabIndex = 7;
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
            this.btnImprimir.Location = new System.Drawing.Point(162, 115);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(42, 31);
            this.btnImprimir.TabIndex = 6;
            this.btnImprimir.TabStop = false;
            this.btnImprimir.ToolTip = "Generar Documento ";
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // btnBuscar
            // 
            this.btnBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBuscar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnBuscar.ImageOptions.Image")));
            this.btnBuscar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnBuscar.Location = new System.Drawing.Point(70, 116);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(86, 30);
            this.btnBuscar.TabIndex = 5;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.ToolTip = "Guardar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtAgrupa);
            this.groupBox1.Controls.Add(this.btnPdf);
            this.groupBox1.Controls.Add(this.labelControl6);
            this.groupBox1.Controls.Add(this.btnConjunto);
            this.groupBox1.Controls.Add(this.txtConjunto);
            this.groupBox1.Controls.Add(this.txtComboPeriodo);
            this.groupBox1.Controls.Add(this.btnImpresionRapida);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Controls.Add(this.btnImprimir);
            this.groupBox1.Controls.Add(this.btnSalir);
            this.groupBox1.Controls.Add(this.btnBuscar);
            this.groupBox1.Controls.Add(this.cbTodos);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Location = new System.Drawing.Point(7, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(413, 160);
            this.groupBox1.TabIndex = 58;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Búsqueda";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // btnPdf
            // 
            this.btnPdf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPdf.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPdf.ImageOptions.Image")));
            this.btnPdf.Location = new System.Drawing.Point(254, 116);
            this.btnPdf.Name = "btnPdf";
            this.btnPdf.Size = new System.Drawing.Size(42, 31);
            this.btnPdf.TabIndex = 8;
            this.btnPdf.Click += new System.EventHandler(this.btnPdf_Click);
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(14, 68);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(50, 13);
            this.labelControl6.TabIndex = 58;
            this.labelControl6.Text = "Condición:";
            // 
            // btnConjunto
            // 
            this.btnConjunto.AllowFocus = false;
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.Enabled = false;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(130, 63);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(26, 21);
            this.btnConjunto.TabIndex = 3;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // txtConjunto
            // 
            this.txtConjunto.Enabled = false;
            this.txtConjunto.Location = new System.Drawing.Point(70, 64);
            this.txtConjunto.Name = "txtConjunto";
            this.txtConjunto.Size = new System.Drawing.Size(56, 20);
            this.txtConjunto.TabIndex = 2;
            // 
            // txtComboPeriodo
            // 
            this.txtComboPeriodo.Location = new System.Drawing.Point(70, 20);
            this.txtComboPeriodo.Name = "txtComboPeriodo";
            this.txtComboPeriodo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtComboPeriodo.Size = new System.Drawing.Size(123, 20);
            this.txtComboPeriodo.TabIndex = 0;
            // 
            // txtAgrupa
            // 
            this.txtAgrupa.Location = new System.Drawing.Point(70, 90);
            this.txtAgrupa.Name = "txtAgrupa";
            this.txtAgrupa.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtAgrupa.Size = new System.Drawing.Size(134, 20);
            this.txtAgrupa.TabIndex = 59;
            // 
            // frmPlanillaMutual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 175);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPlanillaMutual";
            this.Text = "Planilla cotizacion mutual";
            this.Load += new System.EventHandler(this.frmPlanillaMutual_Load);            
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAgrupa.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnImpresionRapida;
        private DevExpress.XtraEditors.SimpleButton btnImprimir;
        private DevExpress.XtraEditors.SimpleButton btnBuscar;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraEditors.SimpleButton btnPdf;
        private DevExpress.XtraEditors.TextEdit txtConjunto;
        private DevExpress.XtraEditors.LookUpEdit txtComboPeriodo;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.LookUpEdit txtAgrupa;
    }
}