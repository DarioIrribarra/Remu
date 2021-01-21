namespace Labour
{
    partial class frmGeneracionContrato
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGeneracionContrato));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnPlantilla = new DevExpress.XtraEditors.SimpleButton();
            this.txtRutaPlantilla = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.cbPersonalizada = new DevExpress.XtraEditors.CheckEdit();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.btnRutaSalida = new DevExpress.XtraEditors.SimpleButton();
            this.txtRutaSalida = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.txtConjunto = new DevExpress.XtraEditors.TextEdit();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.txtRegistros = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txtPeriodo = new DevExpress.XtraEditors.LookUpEdit();
            this.BarraContratos = new DevExpress.XtraEditors.ProgressBarControl();
            this.lblName = new DevExpress.XtraEditors.LabelControl();
            this.btnGenerar = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaPlantilla.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPersonalizada.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaSalida.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRegistros.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BarraContratos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).BeginInit();
            this.panelControl4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnPlantilla);
            this.panelControl1.Controls.Add(this.txtRutaPlantilla);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Location = new System.Drawing.Point(10, 141);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(510, 88);
            this.panelControl1.TabIndex = 0;
            // 
            // btnPlantilla
            // 
            this.btnPlantilla.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPlantilla.Enabled = false;
            this.btnPlantilla.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPlantilla.ImageOptions.Image")));
            this.btnPlantilla.Location = new System.Drawing.Point(437, 44);
            this.btnPlantilla.Name = "btnPlantilla";
            this.btnPlantilla.Size = new System.Drawing.Size(45, 23);
            this.btnPlantilla.TabIndex = 1;
            this.btnPlantilla.Click += new System.EventHandler(this.btnPlantilla_Click);
            // 
            // txtRutaPlantilla
            // 
            this.txtRutaPlantilla.Enabled = false;
            this.txtRutaPlantilla.Location = new System.Drawing.Point(26, 47);
            this.txtRutaPlantilla.Name = "txtRutaPlantilla";
            this.txtRutaPlantilla.Properties.ReadOnly = true;
            this.txtRutaPlantilla.Size = new System.Drawing.Size(405, 20);
            this.txtRutaPlantilla.TabIndex = 1;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(26, 28);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(66, 13);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Ruta plantilla:";
            // 
            // cbPersonalizada
            // 
            this.cbPersonalizada.Location = new System.Drawing.Point(20, 131);
            this.cbPersonalizada.Name = "cbPersonalizada";
            this.cbPersonalizada.Properties.Caption = "Usar plantilla personalizada";
            this.cbPersonalizada.Size = new System.Drawing.Size(156, 19);
            this.cbPersonalizada.TabIndex = 0;
            this.cbPersonalizada.CheckedChanged += new System.EventHandler(this.cbPersonalizada_CheckedChanged);
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.labelControl3);
            this.panelControl2.Controls.Add(this.btnRutaSalida);
            this.panelControl2.Controls.Add(this.txtRutaSalida);
            this.panelControl2.Location = new System.Drawing.Point(10, 251);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(510, 71);
            this.panelControl2.TabIndex = 1;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(28, 13);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(27, 13);
            this.labelControl3.TabIndex = 3;
            this.labelControl3.Text = "Ruta:";
            // 
            // btnRutaSalida
            // 
            this.btnRutaSalida.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRutaSalida.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnRutaSalida.ImageOptions.Image")));
            this.btnRutaSalida.Location = new System.Drawing.Point(436, 29);
            this.btnRutaSalida.Name = "btnRutaSalida";
            this.btnRutaSalida.Size = new System.Drawing.Size(45, 23);
            this.btnRutaSalida.TabIndex = 1;
            this.btnRutaSalida.Click += new System.EventHandler(this.btnRutaSalida_Click);
            // 
            // txtRutaSalida
            // 
            this.txtRutaSalida.Location = new System.Drawing.Point(25, 32);
            this.txtRutaSalida.Name = "txtRutaSalida";
            this.txtRutaSalida.Properties.ReadOnly = true;
            this.txtRutaSalida.Size = new System.Drawing.Size(405, 20);
            this.txtRutaSalida.TabIndex = 1;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(14, 235);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(133, 13);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Especifique la ruta de salida";
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.btnSalir);
            this.panelControl3.Controls.Add(this.labelControl6);
            this.panelControl3.Controls.Add(this.btnConjunto);
            this.panelControl3.Controls.Add(this.txtConjunto);
            this.panelControl3.Controls.Add(this.cbTodos);
            this.panelControl3.Controls.Add(this.txtRegistros);
            this.panelControl3.Controls.Add(this.labelControl5);
            this.panelControl3.Controls.Add(this.labelControl4);
            this.panelControl3.Controls.Add(this.txtPeriodo);
            this.panelControl3.Location = new System.Drawing.Point(10, 6);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(510, 119);
            this.panelControl3.TabIndex = 4;
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(436, 6);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 28);
            this.btnSalir.TabIndex = 124;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(11, 81);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(50, 13);
            this.labelControl6.TabIndex = 5;
            this.labelControl6.Text = "Condición:";
            // 
            // btnConjunto
            // 
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(115, 77);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(26, 21);
            this.btnConjunto.TabIndex = 123;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // txtConjunto
            // 
            this.txtConjunto.Location = new System.Drawing.Point(65, 78);
            this.txtConjunto.Name = "txtConjunto";
            this.txtConjunto.Properties.MaxLength = 12;
            this.txtConjunto.Size = new System.Drawing.Size(46, 20);
            this.txtConjunto.TabIndex = 5;
            // 
            // cbTodos
            // 
            this.cbTodos.EditValue = true;
            this.cbTodos.Location = new System.Drawing.Point(65, 57);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos los registros del periodo";
            this.cbTodos.Size = new System.Drawing.Size(169, 19);
            this.cbTodos.TabIndex = 9;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // txtRegistros
            // 
            this.txtRegistros.Location = new System.Drawing.Point(65, 33);
            this.txtRegistros.Name = "txtRegistros";
            this.txtRegistros.Properties.ReadOnly = true;
            this.txtRegistros.Size = new System.Drawing.Size(42, 20);
            this.txtRegistros.TabIndex = 8;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(12, 36);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(49, 13);
            this.labelControl5.TabIndex = 7;
            this.labelControl5.Text = "Registros:";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(21, 13);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(40, 13);
            this.labelControl4.TabIndex = 6;
            this.labelControl4.Text = "Periodo:";
            // 
            // txtPeriodo
            // 
            this.txtPeriodo.Location = new System.Drawing.Point(65, 10);
            this.txtPeriodo.Name = "txtPeriodo";
            this.txtPeriodo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtPeriodo.Properties.PopupSizeable = false;
            this.txtPeriodo.Size = new System.Drawing.Size(195, 20);
            this.txtPeriodo.TabIndex = 5;
            this.txtPeriodo.EditValueChanged += new System.EventHandler(this.txtPeriodo_EditValueChanged);
            // 
            // BarraContratos
            // 
            this.BarraContratos.Location = new System.Drawing.Point(14, 70);
            this.BarraContratos.Name = "BarraContratos";
            this.BarraContratos.Size = new System.Drawing.Size(467, 15);
            this.BarraContratos.TabIndex = 5;
            this.BarraContratos.Visible = false;
            // 
            // lblName
            // 
            this.lblName.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblName.Appearance.Options.UseFont = true;
            this.lblName.Location = new System.Drawing.Point(17, 51);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(32, 13);
            this.lblName.TabIndex = 6;
            this.lblName.Text = "Name";
            this.lblName.Visible = false;
            // 
            // btnGenerar
            // 
            this.btnGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGenerar.ImageOptions.Image")));
            this.btnGenerar.Location = new System.Drawing.Point(14, 12);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(91, 32);
            this.btnGenerar.TabIndex = 3;
            this.btnGenerar.Text = "Generar";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            // 
            // panelControl4
            // 
            this.panelControl4.Controls.Add(this.btnGenerar);
            this.panelControl4.Controls.Add(this.BarraContratos);
            this.panelControl4.Controls.Add(this.lblName);
            this.panelControl4.Location = new System.Drawing.Point(10, 329);
            this.panelControl4.Name = "panelControl4";
            this.panelControl4.Size = new System.Drawing.Size(510, 94);
            this.panelControl4.TabIndex = 7;
            // 
            // frmGeneracionContrato
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 427);
            this.ControlBox = false;
            this.Controls.Add(this.panelControl4);
            this.Controls.Add(this.panelControl3);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.cbPersonalizada);
            this.Controls.Add(this.panelControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmGeneracionContrato";
            this.Text = "Emision de contratos";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGeneracionContrato_FormClosing);
            this.Load += new System.EventHandler(this.frmGeneracionContrato_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaPlantilla.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPersonalizada.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaSalida.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelControl3.ResumeLayout(false);
            this.panelControl3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRegistros.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BarraContratos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl4)).EndInit();
            this.panelControl4.ResumeLayout(false);
            this.panelControl4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnPlantilla;
        private DevExpress.XtraEditors.TextEdit txtRutaPlantilla;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckEdit cbPersonalizada;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnRutaSalida;
        private DevExpress.XtraEditors.TextEdit txtRutaSalida;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton btnGenerar;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraEditors.TextEdit txtConjunto;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.TextEdit txtRegistros;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LookUpEdit txtPeriodo;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.ProgressBarControl BarraContratos;
        private DevExpress.XtraEditors.LabelControl lblName;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.PanelControl panelControl4;
    }
}