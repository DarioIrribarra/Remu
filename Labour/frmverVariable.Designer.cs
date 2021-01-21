namespace Labour
{
    partial class frmverVariable
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
            this.gridVariable = new DevExpress.XtraGrid.GridControl();
            this.viewVariable = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridVariable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewVariable)).BeginInit();
            this.SuspendLayout();
            // 
            // gridVariable
            // 
            this.gridVariable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridVariable.Location = new System.Drawing.Point(0, 0);
            this.gridVariable.MainView = this.viewVariable;
            this.gridVariable.Name = "gridVariable";
            this.gridVariable.Size = new System.Drawing.Size(426, 482);
            this.gridVariable.TabIndex = 1;
            this.gridVariable.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewVariable});
            this.gridVariable.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.gridVariable_ProcessGridKey);
            this.gridVariable.DoubleClick += new System.EventHandler(this.gridVariable_DoubleClick);
            this.gridVariable.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridVariable_KeyDown);
            // 
            // viewVariable
            // 
            this.viewVariable.GridControl = this.gridVariable;
            this.viewVariable.Name = "viewVariable";
            this.viewVariable.OptionsView.ShowGroupPanel = false;
            // 
            // frmverVariable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 482);
            this.Controls.Add(this.gridVariable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmverVariable";
            this.Text = "Variables Actuales";
            this.Load += new System.EventHandler(this.frmverVariable_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridVariable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewVariable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraGrid.GridControl gridVariable;
        private DevExpress.XtraGrid.Views.Grid.GridView viewVariable;
    }
}