namespace Labour
{
    partial class frmContrato
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmContrato));
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCrear = new DevExpress.XtraEditors.SimpleButton();
            this.btnSalida = new DevExpress.XtraEditors.SimpleButton();
            this.btnPlantilla = new DevExpress.XtraEditors.SimpleButton();
            this.txtSalida = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtPlantilla = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtContrato = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.lblNombre = new DevExpress.XtraEditors.LabelControl();
            this.BarraProceso = new DevExpress.XtraEditors.ProgressBarControl();
            this.btnVariables = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSalida.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlantilla.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContrato.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BarraProceso.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.ImageOptions.Image")));
            this.btnSalir.Location = new System.Drawing.Point(324, 5);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 30);
            this.btnSalir.TabIndex = 6;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnVariables);
            this.panelControl1.Controls.Add(this.btnCrear);
            this.panelControl1.Controls.Add(this.btnSalida);
            this.panelControl1.Controls.Add(this.btnPlantilla);
            this.panelControl1.Controls.Add(this.txtSalida);
            this.panelControl1.Controls.Add(this.labelControl3);
            this.panelControl1.Controls.Add(this.txtPlantilla);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.txtContrato);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.lblNombre);
            this.panelControl1.Controls.Add(this.btnSalir);
            this.panelControl1.Location = new System.Drawing.Point(12, 17);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(367, 145);
            this.panelControl1.TabIndex = 11;
            // 
            // btnCrear
            // 
            this.btnCrear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCrear.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCrear.ImageOptions.Image")));
            this.btnCrear.Location = new System.Drawing.Point(87, 99);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(40, 28);
            this.btnCrear.TabIndex = 5;
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            // 
            // btnSalida
            // 
            this.btnSalida.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalida.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalida.ImageOptions.Image")));
            this.btnSalida.Location = new System.Drawing.Point(306, 71);
            this.btnSalida.Name = "btnSalida";
            this.btnSalida.Size = new System.Drawing.Size(41, 23);
            this.btnSalida.TabIndex = 4;
            this.btnSalida.Click += new System.EventHandler(this.btnSalida_Click);
            // 
            // btnPlantilla
            // 
            this.btnPlantilla.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPlantilla.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPlantilla.ImageOptions.Image")));
            this.btnPlantilla.Location = new System.Drawing.Point(306, 47);
            this.btnPlantilla.Name = "btnPlantilla";
            this.btnPlantilla.Size = new System.Drawing.Size(41, 23);
            this.btnPlantilla.TabIndex = 16;
            this.btnPlantilla.Click += new System.EventHandler(this.btnPlantilla_Click);
            // 
            // txtSalida
            // 
            this.txtSalida.Location = new System.Drawing.Point(87, 73);
            this.txtSalida.Name = "txtSalida";
            this.txtSalida.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.textEdit2_Properties_BeforeShowMenu);
            this.txtSalida.Size = new System.Drawing.Size(216, 20);
            this.txtSalida.TabIndex = 3;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(18, 76);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(57, 13);
            this.labelControl3.TabIndex = 14;
            this.labelControl3.Text = "Ruta salida:";
            // 
            // txtPlantilla
            // 
            this.txtPlantilla.Location = new System.Drawing.Point(87, 50);
            this.txtPlantilla.Name = "txtPlantilla";
            this.txtPlantilla.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.textEdit1_Properties_BeforeShowMenu);
            this.txtPlantilla.Size = new System.Drawing.Size(216, 20);
            this.txtPlantilla.TabIndex = 2;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(18, 53);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(40, 13);
            this.labelControl2.TabIndex = 14;
            this.labelControl2.Text = "Plantilla:";
            // 
            // txtContrato
            // 
            this.txtContrato.Location = new System.Drawing.Point(87, 28);
            this.txtContrato.Name = "txtContrato";
            this.txtContrato.Properties.ReadOnly = true;
            this.txtContrato.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtContrato_Properties_BeforeShowMenu);
            this.txtContrato.Size = new System.Drawing.Size(108, 20);
            this.txtContrato.TabIndex = 0;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(21, 32);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(60, 13);
            this.labelControl1.TabIndex = 12;
            this.labelControl1.Text = "N° contrato:";
            // 
            // lblNombre
            // 
            this.lblNombre.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblNombre.Appearance.Options.UseFont = true;
            this.lblNombre.Location = new System.Drawing.Point(21, 8);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(44, 13);
            this.lblNombre.TabIndex = 11;
            this.lblNombre.Text = "Nombre";
            // 
            // BarraProceso
            // 
            this.BarraProceso.Location = new System.Drawing.Point(12, 2);
            this.BarraProceso.Name = "BarraProceso";
            this.BarraProceso.Size = new System.Drawing.Size(367, 11);
            this.BarraProceso.TabIndex = 12;
            this.BarraProceso.Visible = false;
            // 
            // btnVariables
            // 
            this.btnVariables.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVariables.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.ImageOptions.Image")));
            this.btnVariables.Location = new System.Drawing.Point(133, 101);
            this.btnVariables.Name = "btnVariables";
            this.btnVariables.Size = new System.Drawing.Size(44, 26);
            this.btnVariables.TabIndex = 13;
            this.btnVariables.ToolTip = "Obtener variables Archivo";
            this.btnVariables.Click += new System.EventHandler(this.btnVariables_Click);
            // 
            // frmContrato
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 168);
            this.ControlBox = false;
            this.Controls.Add(this.BarraProceso);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmContrato";
            this.Text = "Contrato";
            this.Load += new System.EventHandler(this.frmContrato_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSalida.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlantilla.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContrato.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BarraProceso.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCrear;
        private DevExpress.XtraEditors.SimpleButton btnSalida;
        private DevExpress.XtraEditors.SimpleButton btnPlantilla;
        private DevExpress.XtraEditors.TextEdit txtSalida;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtPlantilla;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtContrato;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lblNombre;
        private DevExpress.XtraEditors.ProgressBarControl BarraProceso;
        private DevExpress.XtraEditors.SimpleButton btnVariables;
    }
}