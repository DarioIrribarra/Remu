namespace Labour
{
    partial class frmResumenProceso
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmResumenProceso));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txtComboPeriodo = new DevExpress.XtraEditors.LookUpEdit();
            this.btnCargarDatos = new DevExpress.XtraEditors.SimpleButton();
            this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtConjunto = new DevExpress.XtraEditors.TextEdit();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.lblPeriodoEval = new DevExpress.XtraEditors.LabelControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitFormRemuneraciones), true, true);
            this.gridRes = new DevExpress.XtraGrid.GridControl();
            this.viewRes = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.btnPdf = new DevExpress.XtraEditors.SimpleButton();
            this.btnImprimir = new DevExpress.XtraEditors.SimpleButton();
            this.btnImpresionRapida = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.btnEditarReporte = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewRes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelControl3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnConjunto);
            this.panelControl1.Controls.Add(this.labelControl4);
            this.panelControl1.Controls.Add(this.txtComboPeriodo);
            this.panelControl1.Controls.Add(this.btnCargarDatos);
            this.panelControl1.Controls.Add(this.separatorControl1);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.txtConjunto);
            this.panelControl1.Controls.Add(this.cbTodos);
            this.panelControl1.Location = new System.Drawing.Point(14, 39);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(353, 209);
            this.panelControl1.TabIndex = 0;
            // 
            // btnConjunto
            // 
            this.btnConjunto.AllowFocus = false;
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.Enabled = false;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(156, 110);
            this.btnConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(30, 26);
            this.btnConjunto.TabIndex = 19;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(28, 16);
            this.labelControl4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(48, 16);
            this.labelControl4.TabIndex = 18;
            this.labelControl4.Text = "Periodo:";
            // 
            // txtComboPeriodo
            // 
            this.txtComboPeriodo.Location = new System.Drawing.Point(82, 12);
            this.txtComboPeriodo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtComboPeriodo.Name = "txtComboPeriodo";
            this.txtComboPeriodo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtComboPeriodo.Properties.PopupSizeable = false;
            this.txtComboPeriodo.Size = new System.Drawing.Size(163, 22);
            this.txtComboPeriodo.TabIndex = 17;
            // 
            // btnCargarDatos
            // 
            this.btnCargarDatos.AllowFocus = false;
            this.btnCargarDatos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCargarDatos.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCargarDatos.ImageOptions.Image")));
            this.btnCargarDatos.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnCargarDatos.Location = new System.Drawing.Point(28, 149);
            this.btnCargarDatos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCargarDatos.Name = "btnCargarDatos";
            this.btnCargarDatos.Size = new System.Drawing.Size(110, 37);
            this.btnCargarDatos.TabIndex = 5;
            this.btnCargarDatos.Text = "Consultar";
            this.btnCargarDatos.ToolTip = "Iniciar Busqueda";
            this.btnCargarDatos.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
            this.btnCargarDatos.Click += new System.EventHandler(this.btnCargarDatos_Click);
            // 
            // separatorControl1
            // 
            this.separatorControl1.Location = new System.Drawing.Point(6, 44);
            this.separatorControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.separatorControl1.Name = "separatorControl1";
            this.separatorControl1.Padding = new System.Windows.Forms.Padding(10, 11, 10, 11);
            this.separatorControl1.Size = new System.Drawing.Size(328, 28);
            this.separatorControl1.TabIndex = 8;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(28, 116);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(60, 16);
            this.labelControl1.TabIndex = 7;
            this.labelControl1.Text = "Condición:";
            // 
            // txtConjunto
            // 
            this.txtConjunto.Enabled = false;
            this.txtConjunto.Location = new System.Drawing.Point(91, 111);
            this.txtConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtConjunto.Name = "txtConjunto";
            this.txtConjunto.Properties.MaxLength = 12;
            this.txtConjunto.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtConjunto_Properties_BeforeShowMenu);
            this.txtConjunto.Size = new System.Drawing.Size(58, 22);
            this.txtConjunto.TabIndex = 4;
            this.txtConjunto.DoubleClick += new System.EventHandler(this.txtConjunto_DoubleClick);
            this.txtConjunto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtConjunto_KeyPress);
            // 
            // cbTodos
            // 
            this.cbTodos.EditValue = true;
            this.cbTodos.Location = new System.Drawing.Point(28, 82);
            this.cbTodos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos los registros del periodo";
            this.cbTodos.Size = new System.Drawing.Size(217, 20);
            this.cbTodos.TabIndex = 3;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // lblPeriodoEval
            // 
            this.lblPeriodoEval.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblPeriodoEval.Appearance.Options.UseFont = true;
            this.lblPeriodoEval.Location = new System.Drawing.Point(14, 15);
            this.lblPeriodoEval.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblPeriodoEval.Name = "lblPeriodoEval";
            this.lblPeriodoEval.Size = new System.Drawing.Size(99, 18);
            this.lblPeriodoEval.TabIndex = 9;
            this.lblPeriodoEval.Text = "EVALUANDO:";
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.ImageOptions.Image")));
            this.btnSalir.Location = new System.Drawing.Point(882, 39);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(44, 37);
            this.btnSalir.TabIndex = 6;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // gridRes
            // 
            this.gridRes.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridRes.Location = new System.Drawing.Point(374, 39);
            this.gridRes.MainView = this.viewRes;
            this.gridRes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridRes.Name = "gridRes";
            this.gridRes.Size = new System.Drawing.Size(500, 550);
            this.gridRes.TabIndex = 15;
            this.gridRes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewRes});
            // 
            // viewRes
            // 
            this.viewRes.DetailHeight = 431;
            this.viewRes.GridControl = this.gridRes;
            this.viewRes.Name = "viewRes";
            this.viewRes.OptionsView.ShowGroupPanel = false;
            this.viewRes.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.viewRes_RowCellStyle);
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.btnEditarReporte);
            this.panelControl3.Controls.Add(this.btnPdf);
            this.panelControl3.Controls.Add(this.btnImprimir);
            this.panelControl3.Controls.Add(this.btnImpresionRapida);
            this.panelControl3.Controls.Add(this.labelControl2);
            this.panelControl3.Location = new System.Drawing.Point(14, 256);
            this.panelControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(353, 107);
            this.panelControl3.TabIndex = 16;
            // 
            // btnPdf
            // 
            this.btnPdf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPdf.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPdf.ImageOptions.Image")));
            this.btnPdf.Location = new System.Drawing.Point(127, 49);
            this.btnPdf.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPdf.Name = "btnPdf";
            this.btnPdf.Size = new System.Drawing.Size(47, 38);
            this.btnPdf.TabIndex = 15;
            this.btnPdf.Click += new System.EventHandler(this.btnPdf_Click);
            // 
            // btnImprimir
            // 
            this.btnImprimir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImprimir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImprimir.ImageOptions.Image")));
            this.btnImprimir.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnImprimir.Location = new System.Drawing.Point(22, 49);
            this.btnImprimir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(47, 38);
            this.btnImprimir.TabIndex = 12;
            this.btnImprimir.ToolTip = "Generar Documento";
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // btnImpresionRapida
            // 
            this.btnImpresionRapida.AllowFocus = false;
            this.btnImpresionRapida.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImpresionRapida.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImpresionRapida.ImageOptions.Image")));
            this.btnImpresionRapida.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnImpresionRapida.Location = new System.Drawing.Point(73, 49);
            this.btnImpresionRapida.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnImpresionRapida.Name = "btnImpresionRapida";
            this.btnImpresionRapida.Size = new System.Drawing.Size(47, 38);
            this.btnImpresionRapida.TabIndex = 14;
            this.btnImpresionRapida.TabStop = false;
            this.btnImpresionRapida.ToolTip = "Impresion Rapida";
            this.btnImpresionRapida.Click += new System.EventHandler(this.btnImpresionRapida_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(10, 12);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(82, 18);
            this.labelControl2.TabIndex = 9;
            this.labelControl2.Text = "OPCIONES";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(374, 15);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(127, 18);
            this.labelControl3.TabIndex = 9;
            this.labelControl3.Text = "TABLA RESUMEN";
            // 
            // btnEditarReporte
            // 
            this.btnEditarReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditarReporte.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditarReporte.ImageOptions.Image")));
            this.btnEditarReporte.Location = new System.Drawing.Point(180, 50);
            this.btnEditarReporte.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEditarReporte.Name = "btnEditarReporte";
            this.btnEditarReporte.Size = new System.Drawing.Size(114, 37);
            this.btnEditarReporte.TabIndex = 25;
            this.btnEditarReporte.Text = "Editar\r\nReporte\r\n";
            this.btnEditarReporte.Click += new System.EventHandler(this.btnEditarReporte_Click);
            // 
            // frmResumenProceso
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 593);
            this.Controls.Add(this.panelControl3);
            this.Controls.Add(this.gridRes);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.lblPeriodoEval);
            this.Controls.Add(this.panelControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "frmResumenProceso";
            this.Text = "Resumen del periodo";
            this.Load += new System.EventHandler(this.frmResumenProceso_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridRes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewRes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelControl3.ResumeLayout(false);
            this.panelControl3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCargarDatos;
        private DevExpress.XtraEditors.LabelControl lblPeriodoEval;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtConjunto;
        private DevExpress.XtraGrid.GridControl gridRes;
        private DevExpress.XtraGrid.Views.Grid.GridView viewRes;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraEditors.SimpleButton btnImprimir;
        private DevExpress.XtraEditors.SimpleButton btnImpresionRapida;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnPdf;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LookUpEdit txtComboPeriodo;
        private DevExpress.XtraEditors.SeparatorControl separatorControl1;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.SimpleButton btnEditarReporte;
    }
}