namespace Labour
{
    partial class frmLiquidacionesTrabajador
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLiquidacionesTrabajador));
            this.lblTrabajador = new DevExpress.XtraEditors.LabelControl();
            this.gridLiquidacion = new DevExpress.XtraGrid.GridControl();
            this.viewLiquidacion = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtHasta = new DevExpress.XtraEditors.LookUpEdit();
            this.txtDesde = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.cbRango = new DevExpress.XtraEditors.CheckEdit();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.btnNuevo = new DevExpress.XtraEditors.SimpleButton();
            this.btnImpresionRapida = new DevExpress.XtraEditors.SimpleButton();
            this.btnImprimir = new DevExpress.XtraEditors.SimpleButton();
            this.btnBuscar = new DevExpress.XtraEditors.SimpleButton();
            this.lblrut = new DevExpress.XtraEditors.LabelControl();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitFormRemuneraciones), true, true);
            this.btnPdf = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnZipLiquidaciones = new DevExpress.XtraEditors.SimpleButton();
            this.btnMail = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridLiquidacion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewLiquidacion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHasta.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDesde.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbRango.Properties)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTrabajador
            // 
            this.lblTrabajador.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblTrabajador.Appearance.Options.UseFont = true;
            this.lblTrabajador.Location = new System.Drawing.Point(17, 20);
            this.lblTrabajador.Name = "lblTrabajador";
            this.lblTrabajador.Size = new System.Drawing.Size(76, 16);
            this.lblTrabajador.TabIndex = 0;
            this.lblTrabajador.Text = "Trabajador:";
            // 
            // gridLiquidacion
            // 
            this.gridLiquidacion.Location = new System.Drawing.Point(17, 20);
            this.gridLiquidacion.MainView = this.viewLiquidacion;
            this.gridLiquidacion.Name = "gridLiquidacion";
            this.gridLiquidacion.Size = new System.Drawing.Size(914, 372);
            this.gridLiquidacion.TabIndex = 2;
            this.gridLiquidacion.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewLiquidacion});
            this.gridLiquidacion.DragDrop += new System.Windows.Forms.DragEventHandler(this.gridLiquidacion_DragDrop);
            this.gridLiquidacion.DoubleClick += new System.EventHandler(this.gridLiquidacion_DoubleClick);
            this.gridLiquidacion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridLiquidacion_KeyDown);
            // 
            // viewLiquidacion
            // 
            this.viewLiquidacion.GridControl = this.gridLiquidacion;
            this.viewLiquidacion.Name = "viewLiquidacion";
            this.viewLiquidacion.OptionsView.ShowGroupPanel = false;
            this.viewLiquidacion.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.viewLiquidacion_PopupMenuShowing);
            // 
            // txtHasta
            // 
            this.txtHasta.Location = new System.Drawing.Point(214, 45);
            this.txtHasta.Name = "txtHasta";
            this.txtHasta.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtHasta.Size = new System.Drawing.Size(95, 20);
            this.txtHasta.TabIndex = 3;
            this.txtHasta.EditValueChanged += new System.EventHandler(this.txtHasta_EditValueChanged);
            // 
            // txtDesde
            // 
            this.txtDesde.Location = new System.Drawing.Point(61, 45);
            this.txtDesde.Name = "txtDesde";
            this.txtDesde.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtDesde.Size = new System.Drawing.Size(95, 20);
            this.txtDesde.TabIndex = 2;
            this.txtDesde.EditValueChanged += new System.EventHandler(this.txtDesde_EditValueChanged);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(174, 48);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(32, 13);
            this.labelControl4.TabIndex = 6;
            this.labelControl4.Text = "Hasta:";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(21, 48);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(34, 13);
            this.labelControl3.TabIndex = 6;
            this.labelControl3.Text = "Desde:";
            // 
            // cbRango
            // 
            this.cbRango.Location = new System.Drawing.Point(17, 20);
            this.cbRango.Name = "cbRango";
            this.cbRango.Properties.AllowFocused = false;
            this.cbRango.Properties.Caption = "Filtrar por rango";
            this.cbRango.Size = new System.Drawing.Size(131, 19);
            this.cbRango.TabIndex = 0;
            this.cbRango.TabStop = false;
            this.cbRango.CheckedChanged += new System.EventHandler(this.cbRango_CheckedChanged);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.ImageOptions.Image")));
            this.btnSalir.Location = new System.Drawing.Point(920, 12);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(35, 30);
            this.btnSalir.TabIndex = 8;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnNuevo
            // 
            this.btnNuevo.AllowFocus = false;
            this.btnNuevo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNuevo.ImageOptions.Image")));
            this.btnNuevo.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnNuevo.Location = new System.Drawing.Point(892, 11);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(40, 30);
            this.btnNuevo.TabIndex = 4;
            this.btnNuevo.ToolTip = "Refrescar";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnImpresionRapida
            // 
            this.btnImpresionRapida.AllowFocus = false;
            this.btnImpresionRapida.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImpresionRapida.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImpresionRapida.ImageOptions.Image")));
            this.btnImpresionRapida.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnImpresionRapida.Location = new System.Drawing.Point(799, 11);
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
            this.btnImprimir.Location = new System.Drawing.Point(752, 11);
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
            this.btnBuscar.Location = new System.Drawing.Point(9, 12);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(86, 30);
            this.btnBuscar.TabIndex = 5;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.ToolTip = "Guardar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // lblrut
            // 
            this.lblrut.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblrut.Appearance.Options.UseFont = true;
            this.lblrut.Location = new System.Drawing.Point(373, 20);
            this.lblrut.Name = "lblrut";
            this.lblrut.Size = new System.Drawing.Size(28, 16);
            this.lblrut.TabIndex = 0;
            this.lblrut.Text = "Rut:";
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // btnPdf
            // 
            this.btnPdf.AllowFocus = false;
            this.btnPdf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPdf.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPdf.ImageOptions.Image")));
            this.btnPdf.Location = new System.Drawing.Point(844, 11);
            this.btnPdf.Name = "btnPdf";
            this.btnPdf.Size = new System.Drawing.Size(41, 31);
            this.btnPdf.TabIndex = 9;
            this.btnPdf.Click += new System.EventHandler(this.btnPdf_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtHasta);
            this.groupBox1.Controls.Add(this.cbRango);
            this.groupBox1.Controls.Add(this.txtDesde);
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Location = new System.Drawing.Point(17, 45);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(944, 93);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnZipLiquidaciones);
            this.groupBox2.Controls.Add(this.btnMail);
            this.groupBox2.Controls.Add(this.btnBuscar);
            this.groupBox2.Controls.Add(this.btnImprimir);
            this.groupBox2.Controls.Add(this.btnPdf);
            this.groupBox2.Controls.Add(this.btnNuevo);
            this.groupBox2.Controls.Add(this.btnImpresionRapida);
            this.groupBox2.Location = new System.Drawing.Point(14, 144);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(947, 48);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            // 
            // btnZipLiquidaciones
            // 
            this.btnZipLiquidaciones.AllowFocus = false;
            this.btnZipLiquidaciones.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnZipLiquidaciones.ImageOptions.Image = global::Labour.Properties.Resources.zip;
            this.btnZipLiquidaciones.Location = new System.Drawing.Point(703, 11);
            this.btnZipLiquidaciones.Name = "btnZipLiquidaciones";
            this.btnZipLiquidaciones.Size = new System.Drawing.Size(43, 31);
            this.btnZipLiquidaciones.TabIndex = 11;
            this.btnZipLiquidaciones.Click += new System.EventHandler(this.btnZipLiquidaciones_Click);
            // 
            // btnMail
            // 
            this.btnMail.AllowFocus = false;
            this.btnMail.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMail.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMail.ImageOptions.Image")));
            this.btnMail.Location = new System.Drawing.Point(656, 11);
            this.btnMail.Name = "btnMail";
            this.btnMail.Size = new System.Drawing.Size(42, 30);
            this.btnMail.TabIndex = 10;
            this.btnMail.ToolTip = "Enviar liquidaciones por correo";
            this.btnMail.Click += new System.EventHandler(this.btnMail_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.gridLiquidacion);
            this.groupBox3.Location = new System.Drawing.Point(12, 215);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(949, 448);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Liquidaciones";
            // 
            // frmLiquidacionesTrabajador
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(973, 676);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblrut);
            this.Controls.Add(this.lblTrabajador);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmLiquidacionesTrabajador";
            this.Text = "Liquidaciones Historicas";
            this.Load += new System.EventHandler(this.frmLiquidacionesTrabajador_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridLiquidacion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewLiquidacion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHasta.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDesde.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbRango.Properties)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblTrabajador;
        private DevExpress.XtraGrid.GridControl gridLiquidacion;
        private DevExpress.XtraGrid.Views.Grid.GridView viewLiquidacion;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.CheckEdit cbRango;
        private DevExpress.XtraEditors.LookUpEdit txtHasta;
        private DevExpress.XtraEditors.LookUpEdit txtDesde;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnNuevo;
        private DevExpress.XtraEditors.SimpleButton btnImpresionRapida;
        private DevExpress.XtraEditors.SimpleButton btnImprimir;
        private DevExpress.XtraEditors.SimpleButton btnBuscar;
        private DevExpress.XtraEditors.LabelControl lblrut;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.SimpleButton btnPdf;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private DevExpress.XtraEditors.SimpleButton btnMail;
        private DevExpress.XtraEditors.SimpleButton btnZipLiquidaciones;
    }
}