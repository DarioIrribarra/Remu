namespace Labour
{
    partial class frmBuscarFormula
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBuscarFormula));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.lblmsg = new DevExpress.XtraEditors.LabelControl();
            this.btnLimpiar = new DevExpress.XtraEditors.SimpleButton();
            this.btnBuscar = new DevExpress.XtraEditors.SimpleButton();
            this.txtbusqueda = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.lblresultados = new DevExpress.XtraEditors.LabelControl();
            this.gridBusqueda = new DevExpress.XtraGrid.GridControl();
            this.viewBusqueda = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtbusqueda.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBusqueda)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewBusqueda)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnSalir);
            this.panelControl1.Controls.Add(this.lblmsg);
            this.panelControl1.Controls.Add(this.btnLimpiar);
            this.panelControl1.Controls.Add(this.btnBuscar);
            this.panelControl1.Controls.Add(this.txtbusqueda);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Location = new System.Drawing.Point(12, 13);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(432, 100);
            this.panelControl1.TabIndex = 0;
            // 
            // lblmsg
            // 
            this.lblmsg.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblmsg.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblmsg.Appearance.Options.UseFont = true;
            this.lblmsg.Appearance.Options.UseForeColor = true;
            this.lblmsg.Location = new System.Drawing.Point(148, 55);
            this.lblmsg.Name = "lblmsg";
            this.lblmsg.Size = new System.Drawing.Size(75, 13);
            this.lblmsg.TabIndex = 5;
            this.lblmsg.Text = "labelControl3";
            this.lblmsg.Visible = false;
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLimpiar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnLimpiar.ImageOptions.Image")));
            this.btnLimpiar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnLimpiar.Location = new System.Drawing.Point(66, 55);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(41, 35);
            this.btnLimpiar.TabIndex = 4;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // btnBuscar
            // 
            this.btnBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBuscar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnBuscar.ImageOptions.Image")));
            this.btnBuscar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnBuscar.Location = new System.Drawing.Point(19, 55);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(41, 35);
            this.btnBuscar.TabIndex = 3;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // txtbusqueda
            // 
            this.txtbusqueda.Location = new System.Drawing.Point(19, 29);
            this.txtbusqueda.Name = "txtbusqueda";
            this.txtbusqueda.Size = new System.Drawing.Size(288, 20);
            this.txtbusqueda.TabIndex = 1;
            this.txtbusqueda.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtbusqueda_KeyDown);
            this.txtbusqueda.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtbusqueda_KeyPress);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(19, 10);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(213, 13);
            this.labelControl2.TabIndex = 0;
            this.labelControl2.Text = "Escriba el codigo o descripcion de la formula:";
            // 
            // lblresultados
            // 
            this.lblresultados.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblresultados.Appearance.Options.UseFont = true;
            this.lblresultados.Location = new System.Drawing.Point(12, 132);
            this.lblresultados.Name = "lblresultados";
            this.lblresultados.Size = new System.Drawing.Size(207, 19);
            this.lblresultados.TabIndex = 1;
            this.lblresultados.Text = "REGISTROS ENCONTRADOS";
            // 
            // gridBusqueda
            // 
            this.gridBusqueda.Location = new System.Drawing.Point(12, 157);
            this.gridBusqueda.MainView = this.viewBusqueda;
            this.gridBusqueda.Name = "gridBusqueda";
            this.gridBusqueda.Size = new System.Drawing.Size(432, 380);
            this.gridBusqueda.TabIndex = 2;
            this.gridBusqueda.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewBusqueda});
            this.gridBusqueda.DoubleClick += new System.EventHandler(this.gridBusqueda_DoubleClick);
            // 
            // viewBusqueda
            // 
            this.viewBusqueda.GridControl = this.gridBusqueda;
            this.viewBusqueda.Name = "viewBusqueda";
            this.viewBusqueda.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.viewBusqueda_PopupMenuShowing);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSalir.Location = new System.Drawing.Point(395, 0);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 30);
            this.btnSalir.TabIndex = 116;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // frmBuscarFormula
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 549);
            this.ControlBox = false;
            this.Controls.Add(this.gridBusqueda);
            this.Controls.Add(this.lblresultados);
            this.Controls.Add(this.panelControl1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBuscarFormula";
            this.Text = "Buscar Formula";
            this.Load += new System.EventHandler(this.frmBuscarFormula_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtbusqueda.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridBusqueda)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewBusqueda)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnLimpiar;
        private DevExpress.XtraEditors.SimpleButton btnBuscar;
        private DevExpress.XtraEditors.TextEdit txtbusqueda;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl lblresultados;
        private DevExpress.XtraGrid.GridControl gridBusqueda;
        private DevExpress.XtraGrid.Views.Grid.GridView viewBusqueda;
        private DevExpress.XtraEditors.LabelControl lblmsg;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
    }
}