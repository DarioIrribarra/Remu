namespace Labour
{
    partial class frmMontoAnual
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMontoAnual));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtYear = new DevExpress.XtraEditors.LookUpEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnSalirArea = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtRuta = new DevExpress.XtraEditors.TextEdit();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.btnTablasSIISopytec = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtYear.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRuta.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(26, 38);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(92, 16);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Seleccione Año:";
            // 
            // txtYear
            // 
            this.txtYear.Location = new System.Drawing.Point(127, 34);
            this.txtYear.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtYear.Name = "txtYear";
            this.txtYear.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtYear.Size = new System.Drawing.Size(154, 22);
            this.txtYear.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.ImageOptions.Image")));
            this.btnSave.Location = new System.Drawing.Point(127, 95);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(52, 47);
            this.btnSave.TabIndex = 2;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSalirArea
            // 
            this.btnSalirArea.AllowFocus = false;
            this.btnSalirArea.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalirArea.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalirArea.Location = new System.Drawing.Point(372, 6);
            this.btnSalirArea.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalirArea.Name = "btnSalirArea";
            this.btnSalirArea.Size = new System.Drawing.Size(44, 37);
            this.btnSalirArea.TabIndex = 44;
            this.btnSalirArea.ToolTip = "Cerrar Formulario";
            this.btnSalirArea.Click += new System.EventHandler(this.btnSalirArea_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(47, 66);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(69, 16);
            this.labelControl2.TabIndex = 45;
            this.labelControl2.Text = "Guardar en:";
            // 
            // txtRuta
            // 
            this.txtRuta.Location = new System.Drawing.Point(127, 63);
            this.txtRuta.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtRuta.Name = "txtRuta";
            this.txtRuta.Size = new System.Drawing.Size(192, 22);
            this.txtRuta.TabIndex = 46;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.simpleButton1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.ImageOptions.Image")));
            this.simpleButton1.Location = new System.Drawing.Point(325, 59);
            this.simpleButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(52, 28);
            this.simpleButton1.TabIndex = 47;
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // btnTablasSIISopytec
            // 
            this.btnTablasSIISopytec.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTablasSIISopytec.ImageOptions.Image = global::Labour.Properties.Resources.exporttoxls_32x32;
            this.btnTablasSIISopytec.Location = new System.Drawing.Point(185, 95);
            this.btnTablasSIISopytec.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnTablasSIISopytec.Name = "btnTablasSIISopytec";
            this.btnTablasSIISopytec.Size = new System.Drawing.Size(52, 47);
            this.btnTablasSIISopytec.TabIndex = 48;
            this.btnTablasSIISopytec.Click += new System.EventHandler(this.btnTablasSIISopytec_Click);
            // 
            // frmMontoAnual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 155);
            this.Controls.Add(this.btnTablasSIISopytec);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.txtRuta);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.btnSalirArea);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtYear);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMontoAnual";
            this.Text = "Montos Anuales";
            this.Load += new System.EventHandler(this.frmMontoAnual_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtYear.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRuta.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LookUpEdit txtYear;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnSalirArea;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtRuta;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton btnTablasSIISopytec;
    }
}