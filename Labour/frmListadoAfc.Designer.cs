namespace Labour
{
    partial class frmListadoAfc
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmListadoAfc));
            this.gridAfc = new DevExpress.XtraGrid.GridControl();
            this.viewAfc = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtBusqueda = new DevExpress.XtraEditors.TextEdit();
            this.btnBusqueda = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.gridAfc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewAfc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBusqueda.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // gridAfc
            // 
            this.gridAfc.Location = new System.Drawing.Point(15, 93);
            this.gridAfc.MainView = this.viewAfc;
            this.gridAfc.Name = "gridAfc";
            this.gridAfc.Size = new System.Drawing.Size(409, 356);
            this.gridAfc.TabIndex = 0;
            this.gridAfc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewAfc});
            // 
            // viewAfc
            // 
            this.viewAfc.GridControl = this.gridAfc;
            this.viewAfc.Name = "viewAfc";
            this.viewAfc.OptionsView.ShowGroupPanel = false;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(20, 13);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(72, 13);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Buscar por rut:";
            // 
            // txtBusqueda
            // 
            this.txtBusqueda.Location = new System.Drawing.Point(20, 33);
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.Properties.MaxLength = 70;
            this.txtBusqueda.Size = new System.Drawing.Size(187, 20);
            this.txtBusqueda.TabIndex = 2;
            // 
            // btnBusqueda
            // 
            this.btnBusqueda.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBusqueda.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.ImageOptions.Image")));
            this.btnBusqueda.Location = new System.Drawing.Point(213, 26);
            this.btnBusqueda.Name = "btnBusqueda";
            this.btnBusqueda.Size = new System.Drawing.Size(48, 33);
            this.btnBusqueda.TabIndex = 3;
            this.btnBusqueda.Click += new System.EventHandler(this.btnBusqueda_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(18, 73);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(53, 13);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "Resultados";
            // 
            // frmListadoAfc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 461);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.btnBusqueda);
            this.Controls.Add(this.txtBusqueda);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.gridAfc);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmListadoAfc";
            this.Text = "Montos Afc";
            this.Load += new System.EventHandler(this.frmListadoAfc_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridAfc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewAfc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBusqueda.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridAfc;
        private DevExpress.XtraGrid.Views.Grid.GridView viewAfc;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtBusqueda;
        private DevExpress.XtraEditors.SimpleButton btnBusqueda;
        private DevExpress.XtraEditors.LabelControl labelControl2;
    }
}