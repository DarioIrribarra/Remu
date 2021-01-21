namespace Labour
{
    partial class frmSeleccionMultiple
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSeleccionMultiple));
            this.gridSeleccion = new DevExpress.XtraGrid.GridControl();
            this.viewSeleccion = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblTitulo = new DevExpress.XtraEditors.LabelControl();
            this.btnConfirmar = new DevExpress.XtraEditors.SimpleButton();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridSeleccion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSeleccion)).BeginInit();
            this.SuspendLayout();
            // 
            // gridSeleccion
            // 
            this.gridSeleccion.Location = new System.Drawing.Point(10, 48);
            this.gridSeleccion.MainView = this.viewSeleccion;
            this.gridSeleccion.Name = "gridSeleccion";
            this.gridSeleccion.Size = new System.Drawing.Size(697, 293);
            this.gridSeleccion.TabIndex = 4;
            this.gridSeleccion.TabStop = false;
            this.gridSeleccion.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewSeleccion});
            this.gridSeleccion.Load += new System.EventHandler(this.gridSeleccion_Load_1);
            // 
            // viewSeleccion
            // 
            this.viewSeleccion.GridControl = this.gridSeleccion;
            this.viewSeleccion.Name = "viewSeleccion";
            this.viewSeleccion.OptionsView.ShowGroupPanel = false;
            this.viewSeleccion.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.viewSeleccion_CustomRowCellEdit);
            this.viewSeleccion.Layout += new System.EventHandler(this.viewSeleccion_Layout);
            // 
            // lblTitulo
            // 
            this.lblTitulo.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Appearance.Options.UseFont = true;
            this.lblTitulo.Location = new System.Drawing.Point(10, 26);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(268, 16);
            this.lblTitulo.TabIndex = 3;
            this.lblTitulo.Text = "Seleccione los items que desea modificar:";
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirmar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConfirmar.ImageOptions.Image")));
            this.btnConfirmar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnConfirmar.Location = new System.Drawing.Point(10, 345);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(92, 39);
            this.btnConfirmar.TabIndex = 5;
            this.btnConfirmar.Text = "Confirmar";
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(668, 12);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(34, 30);
            this.btnSalir.TabIndex = 42;
            this.btnSalir.TabStop = false;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // frmSeleccionMultiple
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 389);
            this.ControlBox = false;
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.gridSeleccion);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSeleccionMultiple";
            this.Text = "Selección";
            this.Load += new System.EventHandler(this.frmSeleccionMultiple_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridSeleccion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSeleccion)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnConfirmar;
        private DevExpress.XtraGrid.GridControl gridSeleccion;
        private DevExpress.XtraGrid.Views.Grid.GridView viewSeleccion;
        private DevExpress.XtraEditors.LabelControl lblTitulo;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
    }
}