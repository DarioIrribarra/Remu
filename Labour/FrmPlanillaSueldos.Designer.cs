namespace Labour
{
    partial class FrmPlanillaSueldos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPlanillaSueldos));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.lueOrdenPor = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.lblName = new DevExpress.XtraEditors.LabelControl();
            this.barLiquidaciones = new DevExpress.XtraEditors.ProgressBarControl();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.txtConjunto = new DevExpress.XtraEditors.TextEdit();
            this.txtRegistros = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.btnGenerar = new DevExpress.XtraEditors.SimpleButton();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtPeriodo = new DevExpress.XtraEditors.LookUpEdit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lueOrdenPor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barLiquidaciones.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRegistros.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.lueOrdenPor);
            this.panelControl1.Controls.Add(this.labelControl3);
            this.panelControl1.Controls.Add(this.lblName);
            this.panelControl1.Controls.Add(this.barLiquidaciones);
            this.panelControl1.Controls.Add(this.btnConjunto);
            this.panelControl1.Controls.Add(this.btnSalir);
            this.panelControl1.Controls.Add(this.txtConjunto);
            this.panelControl1.Controls.Add(this.txtRegistros);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.btnGenerar);
            this.panelControl1.Controls.Add(this.cbTodos);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.txtPeriodo);
            this.panelControl1.Location = new System.Drawing.Point(6, 5);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(645, 262);
            this.panelControl1.TabIndex = 0;
            // 
            // lueOrdenPor
            // 
            this.lueOrdenPor.Location = new System.Drawing.Point(82, 127);
            this.lueOrdenPor.Name = "lueOrdenPor";
            this.lueOrdenPor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueOrdenPor.Size = new System.Drawing.Size(125, 22);
            this.lueOrdenPor.TabIndex = 9;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(15, 127);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(63, 16);
            this.labelControl3.TabIndex = 8;
            this.labelControl3.Text = "Orden por:";
            // 
            // lblName
            // 
            this.lblName.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblName.Appearance.Options.UseFont = true;
            this.lblName.Location = new System.Drawing.Point(6, 211);
            this.lblName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(60, 17);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "Nombre ";
            this.lblName.Visible = false;
            // 
            // barLiquidaciones
            // 
            this.barLiquidaciones.Location = new System.Drawing.Point(4, 232);
            this.barLiquidaciones.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barLiquidaciones.Name = "barLiquidaciones";
            this.barLiquidaciones.Size = new System.Drawing.Size(623, 21);
            this.barLiquidaciones.TabIndex = 1;
            this.barLiquidaciones.Visible = false;
            // 
            // btnConjunto
            // 
            this.btnConjunto.AllowFocus = false;
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(147, 71);
            this.btnConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(30, 26);
            this.btnConjunto.TabIndex = 4;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.ImageOptions.Image")));
            this.btnSalir.Location = new System.Drawing.Point(587, 7);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(41, 37);
            this.btnSalir.TabIndex = 7;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // txtConjunto
            // 
            this.txtConjunto.EnterMoveNextControl = true;
            this.txtConjunto.Location = new System.Drawing.Point(82, 73);
            this.txtConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtConjunto.Name = "txtConjunto";
            this.txtConjunto.Properties.MaxLength = 12;
            this.txtConjunto.Properties.ReadOnly = true;
            this.txtConjunto.Size = new System.Drawing.Size(64, 22);
            this.txtConjunto.TabIndex = 3;
            // 
            // txtRegistros
            // 
            this.txtRegistros.EditValue = "0";
            this.txtRegistros.Location = new System.Drawing.Point(82, 100);
            this.txtRegistros.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtRegistros.Name = "txtRegistros";
            this.txtRegistros.Properties.AllowFocused = false;
            this.txtRegistros.Properties.ReadOnly = true;
            this.txtRegistros.Size = new System.Drawing.Size(50, 22);
            this.txtRegistros.TabIndex = 5;
            this.txtRegistros.TabStop = false;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(20, 103);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(58, 16);
            this.labelControl2.TabIndex = 6;
            this.labelControl2.Text = "Registros:";
            // 
            // btnGenerar
            // 
            this.btnGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGenerar.ImageOptions.Image")));
            this.btnGenerar.Location = new System.Drawing.Point(78, 158);
            this.btnGenerar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(99, 39);
            this.btnGenerar.TabIndex = 6;
            this.btnGenerar.Text = "Generar";
            this.btnGenerar.ToolTip = "Ejecutar proceso";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            // 
            // cbTodos
            // 
            this.cbTodos.EditValue = true;
            this.cbTodos.Location = new System.Drawing.Point(82, 49);
            this.cbTodos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos los registros del periodo";
            this.cbTodos.Size = new System.Drawing.Size(204, 20);
            this.cbTodos.TabIndex = 1;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(30, 23);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 16);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Periodo:";
            // 
            // txtPeriodo
            // 
            this.txtPeriodo.Location = new System.Drawing.Point(82, 20);
            this.txtPeriodo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPeriodo.Name = "txtPeriodo";
            this.txtPeriodo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtPeriodo.Size = new System.Drawing.Size(218, 22);
            this.txtPeriodo.TabIndex = 0;
            this.txtPeriodo.EditValueChanged += new System.EventHandler(this.txtPeriodo_EditValueChanged);
            // 
            // FrmPlanillaSueldos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 275);
            this.ControlBox = false;
            this.Controls.Add(this.panelControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmPlanillaSueldos";
            this.Text = "Liquidaciones de Sueldo";
            this.Load += new System.EventHandler(this.FrmPlanillaSueldos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lueOrdenPor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barLiquidaciones.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRegistros.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnGenerar;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LookUpEdit txtPeriodo;
        private DevExpress.XtraEditors.TextEdit txtConjunto;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.ProgressBarControl barLiquidaciones;
        private DevExpress.XtraEditors.TextEdit txtRegistros;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl lblName;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.LookUpEdit lueOrdenPor;
        private DevExpress.XtraEditors.LabelControl labelControl3;
    }
}