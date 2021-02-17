namespace Labour
{
    partial class frmNominasBanco
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNominasBanco));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.cbxSeleccionConjunto = new DevExpress.XtraEditors.LookUpEdit();
            this.btnReporte = new DevExpress.XtraEditors.SimpleButton();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.btnGenerar = new DevExpress.XtraEditors.SimpleButton();
            this.dtFechaProceso = new DevExpress.XtraEditors.DateEdit();
            this.btnFiltro = new DevExpress.XtraEditors.SimpleButton();
            this.cbActual = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txtItem = new DevExpress.XtraEditors.LookUpEdit();
            this.txtBanco = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.txtPeriodo = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbxSeleccionConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFechaProceso.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFechaProceso.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbActual.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItem.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBanco.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.cbxSeleccionConjunto);
            this.panelControl1.Controls.Add(this.btnReporte);
            this.panelControl1.Controls.Add(this.btnSalir);
            this.panelControl1.Controls.Add(this.labelControl6);
            this.panelControl1.Controls.Add(this.btnGenerar);
            this.panelControl1.Controls.Add(this.dtFechaProceso);
            this.panelControl1.Controls.Add(this.btnFiltro);
            this.panelControl1.Controls.Add(this.cbActual);
            this.panelControl1.Controls.Add(this.labelControl4);
            this.panelControl1.Controls.Add(this.txtItem);
            this.panelControl1.Controls.Add(this.txtBanco);
            this.panelControl1.Controls.Add(this.labelControl3);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.cbTodos);
            this.panelControl1.Controls.Add(this.txtPeriodo);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(696, 261);
            this.panelControl1.TabIndex = 0;
            // 
            // cbxSeleccionConjunto
            // 
            this.cbxSeleccionConjunto.Location = new System.Drawing.Point(104, 95);
            this.cbxSeleccionConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbxSeleccionConjunto.Name = "cbxSeleccionConjunto";
            this.cbxSeleccionConjunto.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbxSeleccionConjunto.Size = new System.Drawing.Size(117, 22);
            this.cbxSeleccionConjunto.TabIndex = 16;
            // 
            // btnReporte
            // 
            this.btnReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReporte.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnReporte.ImageOptions.Image")));
            this.btnReporte.Location = new System.Drawing.Point(208, 212);
            this.btnReporte.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnReporte.Name = "btnReporte";
            this.btnReporte.Size = new System.Drawing.Size(54, 38);
            this.btnReporte.TabIndex = 15;
            this.btnReporte.Click += new System.EventHandler(this.btnReporte_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(645, 15);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(44, 34);
            this.btnSalir.TabIndex = 12;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(12, 185);
            this.labelControl6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(88, 16);
            this.labelControl6.TabIndex = 14;
            this.labelControl6.Text = "Fecha Proceso:";
            // 
            // btnGenerar
            // 
            this.btnGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGenerar.ImageOptions.Image")));
            this.btnGenerar.Location = new System.Drawing.Point(104, 212);
            this.btnGenerar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(97, 38);
            this.btnGenerar.TabIndex = 11;
            this.btnGenerar.Text = "Crear";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            // 
            // dtFechaProceso
            // 
            this.dtFechaProceso.EditValue = null;
            this.dtFechaProceso.EnterMoveNextControl = true;
            this.dtFechaProceso.Location = new System.Drawing.Point(104, 180);
            this.dtFechaProceso.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dtFechaProceso.Name = "dtFechaProceso";
            this.dtFechaProceso.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dtFechaProceso.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFechaProceso.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFechaProceso.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Classic;
            this.dtFechaProceso.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.False;
            this.dtFechaProceso.Size = new System.Drawing.Size(183, 22);
            this.dtFechaProceso.TabIndex = 8;
            // 
            // btnFiltro
            // 
            this.btnFiltro.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFiltro.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnFiltro.ImageOptions.Image")));
            this.btnFiltro.Location = new System.Drawing.Point(224, 91);
            this.btnFiltro.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnFiltro.Name = "btnFiltro";
            this.btnFiltro.Size = new System.Drawing.Size(37, 32);
            this.btnFiltro.TabIndex = 5;
            this.btnFiltro.Click += new System.EventHandler(this.btnFiltro_Click);
            // 
            // cbActual
            // 
            this.cbActual.Location = new System.Drawing.Point(104, 22);
            this.cbActual.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbActual.Name = "cbActual";
            this.cbActual.Properties.Caption = "Actual";
            this.cbActual.Size = new System.Drawing.Size(68, 20);
            this.cbActual.TabIndex = 1;
            this.cbActual.CheckedChanged += new System.EventHandler(this.cbActual_CheckedChanged);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(65, 158);
            this.labelControl4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(31, 16);
            this.labelControl4.TabIndex = 8;
            this.labelControl4.Text = "Item:";
            // 
            // txtItem
            // 
            this.txtItem.EnterMoveNextControl = true;
            this.txtItem.Location = new System.Drawing.Point(104, 153);
            this.txtItem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtItem.Name = "txtItem";
            this.txtItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtItem.Properties.PopupSizeable = false;
            this.txtItem.Size = new System.Drawing.Size(279, 22);
            this.txtItem.TabIndex = 7;
            this.txtItem.EditValueChanged += new System.EventHandler(this.txtItem_EditValueChanged);
            // 
            // txtBanco
            // 
            this.txtBanco.EnterMoveNextControl = true;
            this.txtBanco.Location = new System.Drawing.Point(104, 126);
            this.txtBanco.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtBanco.Name = "txtBanco";
            this.txtBanco.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtBanco.Properties.PopupSizeable = false;
            this.txtBanco.Size = new System.Drawing.Size(334, 22);
            this.txtBanco.TabIndex = 6;
            this.txtBanco.EditValueChanged += new System.EventHandler(this.txtBanco_EditValueChanged);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(57, 130);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(39, 16);
            this.labelControl3.TabIndex = 6;
            this.labelControl3.Text = "Banco:";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(40, 102);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(56, 16);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Conjunto:";
            // 
            // cbTodos
            // 
            this.cbTodos.Location = new System.Drawing.Point(104, 71);
            this.cbTodos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos los registros del periodo";
            this.cbTodos.Size = new System.Drawing.Size(209, 20);
            this.cbTodos.TabIndex = 3;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // txtPeriodo
            // 
            this.txtPeriodo.EnterMoveNextControl = true;
            this.txtPeriodo.Location = new System.Drawing.Point(104, 47);
            this.txtPeriodo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPeriodo.Name = "txtPeriodo";
            this.txtPeriodo.Properties.MaxLength = 6;
            this.txtPeriodo.Size = new System.Drawing.Size(86, 22);
            this.txtPeriodo.TabIndex = 2;
            this.txtPeriodo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPeriodo_KeyPress);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(49, 50);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 16);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Periodo:";
            // 
            // frmNominasBanco
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 261);
            this.ControlBox = false;
            this.Controls.Add(this.panelControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNominasBanco";
            this.Text = "Pago de nóminas a banco";
            this.Load += new System.EventHandler(this.frmNominasBanco_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbxSeleccionConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFechaProceso.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFechaProceso.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbActual.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItem.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBanco.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.CheckEdit cbActual;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LookUpEdit txtItem;
        private DevExpress.XtraEditors.LookUpEdit txtBanco;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.TextEdit txtPeriodo;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnGenerar;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnFiltro;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.DateEdit dtFechaProceso;
        private DevExpress.XtraEditors.SimpleButton btnReporte;
        private DevExpress.XtraEditors.LookUpEdit cbxSeleccionConjunto;
    }
}