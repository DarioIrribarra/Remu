namespace Labour
{
    partial class frmAdmUsuario
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
            this.gridUsuario = new DevExpress.XtraGrid.GridControl();
            this.viewUsuario = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridUsuario)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewUsuario)).BeginInit();
            this.SuspendLayout();
            // 
            // gridUsuario
            // 
            this.gridUsuario.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridUsuario.Location = new System.Drawing.Point(0, 0);
            this.gridUsuario.MainView = this.viewUsuario;
            this.gridUsuario.Name = "gridUsuario";
            this.gridUsuario.Size = new System.Drawing.Size(675, 288);
            this.gridUsuario.TabIndex = 0;
            this.gridUsuario.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewUsuario});
            // 
            // viewUsuario
            // 
            this.viewUsuario.GridControl = this.gridUsuario;
            this.viewUsuario.Name = "viewUsuario";
            // 
            // frmAdmUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 288);
            this.Controls.Add(this.gridUsuario);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmAdmUsuario";
            this.Text = "Administracion de Usuarios";
            this.Load += new System.EventHandler(this.frmAdmUsuario_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridUsuario)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewUsuario)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridUsuario;
        private DevExpress.XtraGrid.Views.Grid.GridView viewUsuario;
    }
}