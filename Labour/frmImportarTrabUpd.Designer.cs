namespace Labour
{
    partial class frmImportarTrabUpd
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportarTrabUpd));
            this.btnSalirArea = new DevExpress.XtraEditors.SimpleButton();
            this.lblResult = new DevExpress.XtraEditors.LabelControl();
            this.btnCargaInformacion = new DevExpress.XtraEditors.SimpleButton();
            this.btnImportar = new DevExpress.XtraEditors.SimpleButton();
            this.txtRuta = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtRuta.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSalirArea
            // 
            this.btnSalirArea.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalirArea.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalirArea.Location = new System.Drawing.Point(422, 23);
            this.btnSalirArea.Name = "btnSalirArea";
            this.btnSalirArea.Size = new System.Drawing.Size(38, 30);
            this.btnSalirArea.TabIndex = 42;
            this.btnSalirArea.ToolTip = "Cerrar Formulario";
            this.btnSalirArea.Click += new System.EventHandler(this.btnSalirArea_Click);
            // 
            // lblResult
            // 
            this.lblResult.Appearance.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblResult.Appearance.Options.UseForeColor = true;
            this.lblResult.Location = new System.Drawing.Point(163, 74);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(42, 13);
            this.lblResult.TabIndex = 4;
            this.lblResult.Text = "Message";
            this.lblResult.Visible = false;
            // 
            // btnCargaInformacion
            // 
            this.btnCargaInformacion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCargaInformacion.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCargaInformacion.ImageOptions.Image")));
            this.btnCargaInformacion.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnCargaInformacion.Location = new System.Drawing.Point(23, 74);
            this.btnCargaInformacion.Name = "btnCargaInformacion";
            this.btnCargaInformacion.Size = new System.Drawing.Size(38, 33);
            this.btnCargaInformacion.TabIndex = 3;
            this.btnCargaInformacion.Click += new System.EventHandler(this.btnCargaInformacion_Click);
            // 
            // btnImportar
            // 
            this.btnImportar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImportar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImportar.ImageOptions.Image")));
            this.btnImportar.Location = new System.Drawing.Point(338, 41);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(43, 27);
            this.btnImportar.TabIndex = 2;
            this.btnImportar.ToolTip = "Cargar Archivo";
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);
            // 
            // txtRuta
            // 
            this.txtRuta.Location = new System.Drawing.Point(22, 48);
            this.txtRuta.Name = "txtRuta";
            this.txtRuta.Properties.ReadOnly = true;
            this.txtRuta.Size = new System.Drawing.Size(310, 20);
            this.txtRuta.TabIndex = 1;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(23, 30);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(75, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Cargar archivo:";
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.lblResult);
            this.groupControl1.Controls.Add(this.btnSalirArea);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Controls.Add(this.btnCargaInformacion);
            this.groupControl1.Controls.Add(this.txtRuta);
            this.groupControl1.Controls.Add(this.btnImportar);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(476, 121);
            this.groupControl1.TabIndex = 43;
            // 
            // frmImportarTrabUpd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 121);
            this.ControlBox = false;
            this.Controls.Add(this.groupControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportarTrabUpd";
            this.Text = "Actualizar Informacion trabajador desde archivo";
            this.Load += new System.EventHandler(this.frmImportarTrabUpd_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtRuta.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnSalirArea;
        private DevExpress.XtraEditors.SimpleButton btnCargaInformacion;
        private DevExpress.XtraEditors.SimpleButton btnImportar;
        private DevExpress.XtraEditors.TextEdit txtRuta;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lblResult;
        private DevExpress.XtraEditors.GroupControl groupControl1;
    }
}