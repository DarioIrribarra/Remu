namespace Labour
{
    partial class frmCartaAviso
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCartaAviso));
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.BarraCartas = new DevExpress.XtraEditors.ProgressBarControl();
            this.lblProgress = new DevExpress.XtraEditors.LabelControl();
            this.btnGenerar = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.btnSalida = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtSalida = new DevExpress.XtraEditors.TextEdit();
            this.btnPlantilla = new DevExpress.XtraEditors.SimpleButton();
            this.txtPlantilla = new DevExpress.XtraEditors.TextEdit();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.txtRegistros = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.txtConjunto = new DevExpress.XtraEditors.TextEdit();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.txtPeriodo = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.BarraCartas.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSalida.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlantilla.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRegistros.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(413, 9);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 28);
            this.btnSalir.TabIndex = 125;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // BarraCartas
            // 
            this.BarraCartas.Location = new System.Drawing.Point(21, 242);
            this.BarraCartas.Name = "BarraCartas";
            this.BarraCartas.Size = new System.Drawing.Size(430, 15);
            this.BarraCartas.TabIndex = 10;
            this.BarraCartas.Visible = false;
            // 
            // lblProgress
            // 
            this.lblProgress.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.lblProgress.Appearance.Options.UseFont = true;
            this.lblProgress.Location = new System.Drawing.Point(21, 227);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(78, 13);
            this.lblProgress.TabIndex = 136;
            this.lblProgress.Text = "labelControl7";
            this.lblProgress.Visible = false;
            // 
            // btnGenerar
            // 
            this.btnGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGenerar.ImageOptions.Image")));
            this.btnGenerar.Location = new System.Drawing.Point(21, 192);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(91, 33);
            this.btnGenerar.TabIndex = 9;
            this.btnGenerar.Text = "Generar";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(22, 146);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(91, 13);
            this.labelControl4.TabIndex = 129;
            this.labelControl4.Text = "Directorio de salida";
            // 
            // btnSalida
            // 
            this.btnSalida.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalida.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalida.ImageOptions.Image")));
            this.btnSalida.Location = new System.Drawing.Point(412, 159);
            this.btnSalida.Name = "btnSalida";
            this.btnSalida.Size = new System.Drawing.Size(39, 23);
            this.btnSalida.TabIndex = 8;
            this.btnSalida.Click += new System.EventHandler(this.btnSalida_Click);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(22, 106);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(62, 13);
            this.labelControl3.TabIndex = 129;
            this.labelControl3.Text = "Ruta plantilla";
            // 
            // txtSalida
            // 
            this.txtSalida.Location = new System.Drawing.Point(22, 162);
            this.txtSalida.Name = "txtSalida";
            this.txtSalida.Properties.ReadOnly = true;
            this.txtSalida.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtSalida_Properties_BeforeShowMenu);
            this.txtSalida.Size = new System.Drawing.Size(380, 20);
            this.txtSalida.TabIndex = 7;
            // 
            // btnPlantilla
            // 
            this.btnPlantilla.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPlantilla.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPlantilla.ImageOptions.Image")));
            this.btnPlantilla.Location = new System.Drawing.Point(412, 119);
            this.btnPlantilla.Name = "btnPlantilla";
            this.btnPlantilla.Size = new System.Drawing.Size(39, 23);
            this.btnPlantilla.TabIndex = 6;
            this.btnPlantilla.Click += new System.EventHandler(this.btnPlantilla_Click);
            // 
            // txtPlantilla
            // 
            this.txtPlantilla.Location = new System.Drawing.Point(22, 122);
            this.txtPlantilla.Name = "txtPlantilla";
            this.txtPlantilla.Properties.ReadOnly = true;
            this.txtPlantilla.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtPlantilla_Properties_BeforeShowMenu);
            this.txtPlantilla.Size = new System.Drawing.Size(380, 20);
            this.txtPlantilla.TabIndex = 5;
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.txtRegistros);
            this.panelControl2.Controls.Add(this.labelControl1);
            this.panelControl2.Controls.Add(this.BarraCartas);
            this.panelControl2.Controls.Add(this.labelControl6);
            this.panelControl2.Controls.Add(this.lblProgress);
            this.panelControl2.Controls.Add(this.btnConjunto);
            this.panelControl2.Controls.Add(this.btnGenerar);
            this.panelControl2.Controls.Add(this.txtConjunto);
            this.panelControl2.Controls.Add(this.labelControl4);
            this.panelControl2.Controls.Add(this.cbTodos);
            this.panelControl2.Controls.Add(this.btnSalida);
            this.panelControl2.Controls.Add(this.txtPeriodo);
            this.panelControl2.Controls.Add(this.labelControl3);
            this.panelControl2.Controls.Add(this.labelControl2);
            this.panelControl2.Controls.Add(this.txtSalida);
            this.panelControl2.Controls.Add(this.btnSalir);
            this.panelControl2.Controls.Add(this.btnPlantilla);
            this.panelControl2.Controls.Add(this.txtPlantilla);
            this.panelControl2.Location = new System.Drawing.Point(5, 7);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(476, 264);
            this.panelControl2.TabIndex = 128;
            // 
            // txtRegistros
            // 
            this.txtRegistros.Location = new System.Drawing.Point(69, 74);
            this.txtRegistros.Name = "txtRegistros";
            this.txtRegistros.Size = new System.Drawing.Size(47, 20);
            this.txtRegistros.TabIndex = 4;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(15, 77);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(49, 13);
            this.labelControl1.TabIndex = 137;
            this.labelControl1.Text = "Registros:";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(13, 55);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(50, 13);
            this.labelControl6.TabIndex = 133;
            this.labelControl6.Text = "Condición:";
            // 
            // btnConjunto
            // 
            this.btnConjunto.AllowFocus = false;
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(119, 51);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(26, 21);
            this.btnConjunto.TabIndex = 3;
            this.btnConjunto.TabStop = false;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // txtConjunto
            // 
            this.txtConjunto.Location = new System.Drawing.Point(69, 52);
            this.txtConjunto.Name = "txtConjunto";
            this.txtConjunto.Properties.MaxLength = 12;
            this.txtConjunto.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtConjunto_Properties_BeforeShowMenu);
            this.txtConjunto.Size = new System.Drawing.Size(47, 20);
            this.txtConjunto.TabIndex = 2;
            // 
            // cbTodos
            // 
            this.cbTodos.EditValue = true;
            this.cbTodos.Location = new System.Drawing.Point(67, 31);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos los registros del periodo";
            this.cbTodos.Size = new System.Drawing.Size(169, 19);
            this.cbTodos.TabIndex = 1;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // txtPeriodo
            // 
            this.txtPeriodo.Location = new System.Drawing.Point(67, 9);
            this.txtPeriodo.Name = "txtPeriodo";
            this.txtPeriodo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtPeriodo.Properties.PopupSizeable = false;
            this.txtPeriodo.Size = new System.Drawing.Size(156, 20);
            this.txtPeriodo.TabIndex = 0;
            this.txtPeriodo.EditValueChanged += new System.EventHandler(this.txtPeriodo_EditValueChanged);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(24, 12);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(40, 13);
            this.labelControl2.TabIndex = 129;
            this.labelControl2.Text = "Periodo:";
            // 
            // frmCartaAviso
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 277);
            this.ControlBox = false;
            this.Controls.Add(this.panelControl2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCartaAviso";
            this.Text = "Carta de aviso";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCartaAviso_FormClosing);
            this.Load += new System.EventHandler(this.frmCartaAviso_Load);
            ((System.ComponentModel.ISupportInitialize)(this.BarraCartas.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSalida.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlantilla.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRegistros.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnPlantilla;
        private DevExpress.XtraEditors.TextEdit txtPlantilla;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.LookUpEdit txtPeriodo;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.SimpleButton btnSalida;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtSalida;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.TextEdit txtConjunto;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.SimpleButton btnGenerar;
        private DevExpress.XtraEditors.ProgressBarControl BarraCartas;
        private DevExpress.XtraEditors.LabelControl lblProgress;
        private DevExpress.XtraEditors.TextEdit txtRegistros;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}