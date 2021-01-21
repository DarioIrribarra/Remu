namespace Labour
{
    partial class frmSeleccionItemElimina
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSeleccionItemElimina));
            this.lblTitulo = new DevExpress.XtraEditors.LabelControl();
            this.gridSeleccion = new DevExpress.XtraGrid.GridControl();
            this.viewSeleccion = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnConfirmar = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridSeleccion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSeleccion)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Appearance.Options.UseFont = true;
            this.lblTitulo.Location = new System.Drawing.Point(12, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(247, 16);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Seleccione los item que desea eliminar";
            // 
            // gridSeleccion
            // 
            this.gridSeleccion.Location = new System.Drawing.Point(12, 31);
            this.gridSeleccion.MainView = this.viewSeleccion;
            this.gridSeleccion.Name = "gridSeleccion";
            this.gridSeleccion.Size = new System.Drawing.Size(697, 351);
            this.gridSeleccion.TabIndex = 1;
            this.gridSeleccion.TabStop = false;
            this.gridSeleccion.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewSeleccion});
            // 
            // viewSeleccion
            // 
            this.viewSeleccion.GridControl = this.gridSeleccion;
            this.viewSeleccion.Name = "viewSeleccion";
            this.viewSeleccion.OptionsView.ShowGroupPanel = false;
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirmar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConfirmar.ImageOptions.Image")));
            this.btnConfirmar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnConfirmar.Location = new System.Drawing.Point(12, 388);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(101, 39);
            this.btnConfirmar.TabIndex = 2;
            this.btnConfirmar.Text = "Confirmar";
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            // 
            // frmSeleccionItemElimina
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 434);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.gridSeleccion);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSeleccionItemElimina";
            this.Text = "Seleccion Item ";
            this.Load += new System.EventHandler(this.frmSeleccionItemElimina_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridSeleccion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSeleccion)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblTitulo;
        private DevExpress.XtraGrid.GridControl gridSeleccion;
        private DevExpress.XtraGrid.Views.Grid.GridView viewSeleccion;
        private DevExpress.XtraEditors.SimpleButton btnConfirmar;
    }
}