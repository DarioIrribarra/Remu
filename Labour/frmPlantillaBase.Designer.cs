namespace Labour
{
    partial class frmPlantillaBase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPlantillaBase));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnBase = new DevExpress.XtraEditors.SimpleButton();
            this.txtBase = new DevExpress.XtraEditors.LookUpEdit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBase.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(27, 21);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(161, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Seleccione estructura a usar";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnBase);
            this.panelControl1.Controls.Add(this.txtBase);
            this.panelControl1.Location = new System.Drawing.Point(27, 41);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(318, 85);
            this.panelControl1.TabIndex = 1;
            // 
            // btnBase
            // 
            this.btnBase.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBase.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnBase.ImageOptions.Image")));
            this.btnBase.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnBase.Location = new System.Drawing.Point(96, 46);
            this.btnBase.Name = "btnBase";
            this.btnBase.Size = new System.Drawing.Size(113, 23);
            this.btnBase.TabIndex = 1;
            this.btnBase.Text = "Cargar Base";
            this.btnBase.Click += new System.EventHandler(this.btnBase_Click);
            // 
            // txtBase
            // 
            this.txtBase.Location = new System.Drawing.Point(67, 17);
            this.txtBase.Name = "txtBase";
            this.txtBase.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtBase.Size = new System.Drawing.Size(178, 20);
            this.txtBase.TabIndex = 0;
            // 
            // frmPlantillaBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 149);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPlantillaBase";
            this.Text = "Plantilla";
            this.Load += new System.EventHandler(this.frmPlantillaBase_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtBase.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnBase;
        private DevExpress.XtraEditors.LookUpEdit txtBase;
    }
}