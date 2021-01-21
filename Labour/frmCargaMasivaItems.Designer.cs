namespace Labour
{
    partial class frmCargaMasivaItems
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCargaMasivaItems));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtOperacion = new DevExpress.XtraEditors.LookUpEdit();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.lblResult = new DevExpress.XtraEditors.LabelControl();
            this.btnSalirArea = new DevExpress.XtraEditors.SimpleButton();
            this.btnCargaInformacion = new DevExpress.XtraEditors.SimpleButton();
            this.btnImportar = new DevExpress.XtraEditors.SimpleButton();
            this.txtRuta = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.gridResumen = new DevExpress.XtraGrid.GridControl();
            this.viewResumen = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitForm3), true, true);
            this.lblRegistros = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtOperacion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRuta.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridResumen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewResumen)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.labelControl3);
            this.panelControl1.Controls.Add(this.txtOperacion);
            this.panelControl1.Controls.Add(this.btnRefresh);
            this.panelControl1.Controls.Add(this.lblResult);
            this.panelControl1.Controls.Add(this.btnSalirArea);
            this.panelControl1.Controls.Add(this.btnCargaInformacion);
            this.panelControl1.Controls.Add(this.btnImportar);
            this.panelControl1.Controls.Add(this.txtRuta);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Location = new System.Drawing.Point(9, 10);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(445, 169);
            this.panelControl1.TabIndex = 0;
            this.panelControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.panelControl1_Paint);
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(22, 10);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(122, 13);
            this.labelControl3.TabIndex = 45;
            this.labelControl3.Text = "Selecciona operación:";
            // 
            // txtOperacion
            // 
            this.txtOperacion.Location = new System.Drawing.Point(22, 28);
            this.txtOperacion.Name = "txtOperacion";
            this.txtOperacion.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtOperacion.Properties.PopupSizeable = false;
            this.txtOperacion.Size = new System.Drawing.Size(125, 20);
            this.txtOperacion.TabIndex = 44;
            // 
            // btnRefresh
            // 
            this.btnRefresh.AllowFocus = false;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.Enabled = false;
            this.btnRefresh.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.ImageOptions.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(360, 71);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(39, 29);
            this.btnRefresh.TabIndex = 43;
            this.btnRefresh.ToolTip = "Recargar Archivo";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblResult
            // 
            this.lblResult.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblResult.Appearance.Options.UseForeColor = true;
            this.lblResult.Location = new System.Drawing.Point(24, 100);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(63, 13);
            this.lblResult.TabIndex = 42;
            this.lblResult.Text = "labelControl2";
            this.lblResult.Visible = false;
            // 
            // btnSalirArea
            // 
            this.btnSalirArea.AllowFocus = false;
            this.btnSalirArea.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalirArea.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalirArea.Location = new System.Drawing.Point(401, 5);
            this.btnSalirArea.Name = "btnSalirArea";
            this.btnSalirArea.Size = new System.Drawing.Size(38, 30);
            this.btnSalirArea.TabIndex = 42;
            this.btnSalirArea.ToolTip = "Cerrar Formulario";
            this.btnSalirArea.Click += new System.EventHandler(this.btnSalirArea_Click);
            // 
            // btnCargaInformacion
            // 
            this.btnCargaInformacion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCargaInformacion.Enabled = false;
            this.btnCargaInformacion.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCargaInformacion.ImageOptions.Image")));
            this.btnCargaInformacion.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnCargaInformacion.Location = new System.Drawing.Point(24, 119);
            this.btnCargaInformacion.Name = "btnCargaInformacion";
            this.btnCargaInformacion.Size = new System.Drawing.Size(84, 33);
            this.btnCargaInformacion.TabIndex = 3;
            this.btnCargaInformacion.Text = "Cargar";
            this.btnCargaInformacion.Click += new System.EventHandler(this.btnCargaInformacion_Click);
            // 
            // btnImportar
            // 
            this.btnImportar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImportar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImportar.ImageOptions.Image")));
            this.btnImportar.Location = new System.Drawing.Point(311, 71);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(43, 27);
            this.btnImportar.TabIndex = 2;
            this.btnImportar.ToolTip = "Cargar Archivo";
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);
            // 
            // txtRuta
            // 
            this.txtRuta.Location = new System.Drawing.Point(24, 74);
            this.txtRuta.Name = "txtRuta";
            this.txtRuta.Properties.ReadOnly = true;
            this.txtRuta.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtRuta_Properties_BeforeShowMenu);
            this.txtRuta.Size = new System.Drawing.Size(283, 20);
            this.txtRuta.TabIndex = 1;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(24, 53);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(73, 15);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Ruta archivo:";
            // 
            // gridResumen
            // 
            this.gridResumen.Location = new System.Drawing.Point(13, 207);
            this.gridResumen.MainView = this.viewResumen;
            this.gridResumen.Name = "gridResumen";
            this.gridResumen.Size = new System.Drawing.Size(442, 274);
            this.gridResumen.TabIndex = 43;
            this.gridResumen.TabStop = false;
            this.gridResumen.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewResumen});
            // 
            // viewResumen
            // 
            this.viewResumen.GridControl = this.gridResumen;
            this.viewResumen.Name = "viewResumen";
            this.viewResumen.OptionsView.ShowGroupPanel = false;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(14, 184);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(51, 15);
            this.labelControl2.TabIndex = 44;
            this.labelControl2.Text = "Resumen";
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // lblRegistros
            // 
            this.lblRegistros.Appearance.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRegistros.Appearance.Options.UseFont = true;
            this.lblRegistros.Location = new System.Drawing.Point(281, 186);
            this.lblRegistros.Name = "lblRegistros";
            this.lblRegistros.Size = new System.Drawing.Size(64, 15);
            this.lblRegistros.TabIndex = 45;
            this.lblRegistros.Text = "Registros #";
            // 
            // frmCargaMasivaItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 490);
            this.ControlBox = false;
            this.Controls.Add(this.lblRegistros);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.gridResumen);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCargaMasivaItems";
            this.Text = "Carga Masiva items";
            this.Load += new System.EventHandler(this.frmCargaMasivaItems_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtOperacion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRuta.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridResumen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewResumen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl lblResult;
        private DevExpress.XtraEditors.SimpleButton btnCargaInformacion;
        private DevExpress.XtraEditors.SimpleButton btnImportar;
        private DevExpress.XtraEditors.TextEdit txtRuta;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnSalirArea;
        private DevExpress.XtraGrid.GridControl gridResumen;
        private DevExpress.XtraGrid.Views.Grid.GridView viewResumen;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.LabelControl lblRegistros;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LookUpEdit txtOperacion;
    }
}