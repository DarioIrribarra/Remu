namespace Labour
{
    partial class frmImportarItems
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportarItems));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.lblResult = new DevExpress.XtraEditors.LabelControl();
            this.btnCargaInformacion = new DevExpress.XtraEditors.SimpleButton();
            this.btnImportar = new DevExpress.XtraEditors.SimpleButton();
            this.txtRuta = new DevExpress.XtraEditors.TextEdit();
            this.btnSalirArea = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRuta.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(19, 20);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(75, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Cargar archivo:";
            this.labelControl1.Click += new System.EventHandler(this.labelControl1_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.lblResult);
            this.panelControl1.Controls.Add(this.btnCargaInformacion);
            this.panelControl1.Controls.Add(this.btnImportar);
            this.panelControl1.Controls.Add(this.txtRuta);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Location = new System.Drawing.Point(12, 50);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(442, 135);
            this.panelControl1.TabIndex = 0;
            this.panelControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.panelControl1_Paint);
            // 
            // lblResult
            // 
            this.lblResult.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblResult.Appearance.Options.UseForeColor = true;
            this.lblResult.Location = new System.Drawing.Point(20, 77);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(63, 13);
            this.lblResult.TabIndex = 42;
            this.lblResult.Text = "labelControl2";
            this.lblResult.Visible = false;
            this.lblResult.Click += new System.EventHandler(this.lblResult_Click);
            // 
            // btnCargaInformacion
            // 
            this.btnCargaInformacion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCargaInformacion.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCargaInformacion.ImageOptions.Image")));
            this.btnCargaInformacion.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnCargaInformacion.Location = new System.Drawing.Point(19, 95);
            this.btnCargaInformacion.Name = "btnCargaInformacion";
            this.btnCargaInformacion.Size = new System.Drawing.Size(132, 33);
            this.btnCargaInformacion.TabIndex = 41;
            this.btnCargaInformacion.Text = "Carga informacion";
            this.btnCargaInformacion.Click += new System.EventHandler(this.btnCargaInformacion_Click);
            // 
            // btnImportar
            // 
            this.btnImportar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImportar.Location = new System.Drawing.Point(335, 49);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(43, 27);
            this.btnImportar.TabIndex = 2;
            this.btnImportar.Text = "...";
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);
            // 
            // txtRuta
            // 
            this.txtRuta.Location = new System.Drawing.Point(19, 53);
            this.txtRuta.Name = "txtRuta";
            this.txtRuta.Properties.ReadOnly = true;
            this.txtRuta.Size = new System.Drawing.Size(310, 20);
            this.txtRuta.TabIndex = 1;
            this.txtRuta.EditValueChanged += new System.EventHandler(this.txtRuta_EditValueChanged);
            // 
            // btnSalirArea
            // 
            this.btnSalirArea.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalirArea.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalirArea.Location = new System.Drawing.Point(416, 12);
            this.btnSalirArea.Name = "btnSalirArea";
            this.btnSalirArea.Size = new System.Drawing.Size(38, 30);
            this.btnSalirArea.TabIndex = 40;
            this.btnSalirArea.ToolTip = "Cerrar Formulario";
            this.btnSalirArea.Click += new System.EventHandler(this.btnSalirArea_Click);
            // 
            // frmImportarItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 191);
            this.Controls.Add(this.btnSalirArea);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmImportarItems";
            this.Text = "Importar Items";
            this.Load += new System.EventHandler(this.frmImportarItems_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRuta.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnImportar;
        private DevExpress.XtraEditors.TextEdit txtRuta;
        private DevExpress.XtraEditors.SimpleButton btnSalirArea;
        private DevExpress.XtraEditors.SimpleButton btnCargaInformacion;
        private DevExpress.XtraEditors.LabelControl lblResult;
    }
}