namespace Labour
{
    partial class frmFeriado
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFeriado));
            this.PanelIngreso = new DevExpress.XtraEditors.PanelControl();
            this.dtFecha = new DevExpress.XtraEditors.DateEdit();
            this.txtdescripcion = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnEliminar = new DevExpress.XtraEditors.SimpleButton();
            this.btnNuevo = new DevExpress.XtraEditors.SimpleButton();
            this.btnGuardar = new DevExpress.XtraEditors.SimpleButton();
            this.gridFeriado = new DevExpress.XtraGrid.GridControl();
            this.viewFeriado = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.PanelPeriodo = new DevExpress.XtraEditors.PanelControl();
            this.txtYearCopia = new DevExpress.XtraEditors.TextEdit();
            this.txtPeriodoGraba = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.cbCopiaPeriodo = new DevExpress.XtraEditors.CheckEdit();
            this.calendarioFeriados = new DevExpress.XtraEditors.Controls.CalendarControl();
            this.toolTipController1 = new DevExpress.Utils.ToolTipController();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.PanelIngreso)).BeginInit();
            this.PanelIngreso.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtFecha.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFecha.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtdescripcion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridFeriado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewFeriado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PanelPeriodo)).BeginInit();
            this.PanelPeriodo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtYearCopia.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodoGraba.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbCopiaPeriodo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.calendarioFeriados.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelIngreso
            // 
            this.PanelIngreso.Controls.Add(this.dtFecha);
            this.PanelIngreso.Controls.Add(this.txtdescripcion);
            this.PanelIngreso.Controls.Add(this.labelControl3);
            this.PanelIngreso.Controls.Add(this.labelControl2);
            this.PanelIngreso.Controls.Add(this.btnSalir);
            this.PanelIngreso.Location = new System.Drawing.Point(370, 31);
            this.PanelIngreso.Name = "PanelIngreso";
            this.PanelIngreso.Size = new System.Drawing.Size(556, 76);
            this.PanelIngreso.TabIndex = 0;
            // 
            // dtFecha
            // 
            this.dtFecha.EditValue = null;
            this.dtFecha.EnterMoveNextControl = true;
            this.dtFecha.Location = new System.Drawing.Point(92, 14);
            this.dtFecha.Name = "dtFecha";
            this.dtFecha.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFecha.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFecha.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Classic;
            this.dtFecha.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            this.dtFecha.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.False;
            this.dtFecha.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.dtFecha_Properties_BeforeShowMenu);
            this.dtFecha.Size = new System.Drawing.Size(111, 20);
            this.dtFecha.TabIndex = 3;
            // 
            // txtdescripcion
            // 
            this.txtdescripcion.EnterMoveNextControl = true;
            this.txtdescripcion.Location = new System.Drawing.Point(92, 37);
            this.txtdescripcion.Name = "txtdescripcion";
            this.txtdescripcion.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtdescripcion.Properties.MaxLength = 100;
            this.txtdescripcion.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtdescripcion_Properties_BeforeShowMenu);
            this.txtdescripcion.Size = new System.Drawing.Size(336, 20);
            this.txtdescripcion.TabIndex = 4;
            this.txtdescripcion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtdescripcion_KeyPress);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(16, 16);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(33, 13);
            this.labelControl3.TabIndex = 0;
            this.labelControl3.Text = "Fecha:";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(16, 40);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(58, 13);
            this.labelControl2.TabIndex = 0;
            this.labelControl2.Text = "Descripcion:";
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(505, 9);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(34, 30);
            this.btnSalir.TabIndex = 39;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(28, 16);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(64, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Año a copiar:";
            // 
            // btnEliminar
            // 
            this.btnEliminar.AllowFocus = false;
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminar.ImageOptions.Image")));
            this.btnEliminar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnEliminar.Location = new System.Drawing.Point(171, 113);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(70, 30);
            this.btnEliminar.TabIndex = 30;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.ToolTip = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnNuevo
            // 
            this.btnNuevo.AllowFocus = false;
            this.btnNuevo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNuevo.ImageOptions.Image")));
            this.btnNuevo.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnNuevo.Location = new System.Drawing.Point(21, 113);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(70, 30);
            this.btnNuevo.TabIndex = 29;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.ToolTip = "Nuevo registro";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.ImageOptions.Image")));
            this.btnGuardar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGuardar.Location = new System.Drawing.Point(95, 113);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(70, 30);
            this.btnGuardar.TabIndex = 28;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.ToolTip = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // gridFeriado
            // 
            this.gridFeriado.Location = new System.Drawing.Point(21, 171);
            this.gridFeriado.MainView = this.viewFeriado;
            this.gridFeriado.Name = "gridFeriado";
            this.gridFeriado.Size = new System.Drawing.Size(572, 315);
            this.gridFeriado.TabIndex = 31;
            this.gridFeriado.TabStop = false;
            this.gridFeriado.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewFeriado});
            this.gridFeriado.Click += new System.EventHandler(this.gridFeriado_Click);
            this.gridFeriado.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridFeriado_KeyUp);
            // 
            // viewFeriado
            // 
            this.viewFeriado.GridControl = this.gridFeriado;
            this.viewFeriado.Name = "viewFeriado";
            this.viewFeriado.OptionsView.ShowGroupPanel = false;
            // 
            // PanelPeriodo
            // 
            this.PanelPeriodo.Controls.Add(this.txtYearCopia);
            this.PanelPeriodo.Controls.Add(this.txtPeriodoGraba);
            this.PanelPeriodo.Controls.Add(this.labelControl4);
            this.PanelPeriodo.Controls.Add(this.labelControl1);
            this.PanelPeriodo.Location = new System.Drawing.Point(18, 31);
            this.PanelPeriodo.Name = "PanelPeriodo";
            this.PanelPeriodo.Size = new System.Drawing.Size(337, 76);
            this.PanelPeriodo.TabIndex = 32;
            // 
            // txtYearCopia
            // 
            this.txtYearCopia.EnterMoveNextControl = true;
            this.txtYearCopia.Location = new System.Drawing.Point(99, 13);
            this.txtYearCopia.Name = "txtYearCopia";
            this.txtYearCopia.Properties.MaxLength = 4;
            this.txtYearCopia.Properties.ReadOnly = true;
            this.txtYearCopia.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtYearCopia_Properties_BeforeShowMenu);
            this.txtYearCopia.Size = new System.Drawing.Size(100, 20);
            this.txtYearCopia.TabIndex = 2;
            this.txtYearCopia.EditValueChanged += new System.EventHandler(this.txtYearCopia_EditValueChanged);
            this.txtYearCopia.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtYearCopia_KeyDown);
            this.txtYearCopia.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtYearCopia_KeyPress);
            // 
            // txtPeriodoGraba
            // 
            this.txtPeriodoGraba.Location = new System.Drawing.Point(99, 37);
            this.txtPeriodoGraba.Name = "txtPeriodoGraba";
            this.txtPeriodoGraba.Properties.MaxLength = 4;
            this.txtPeriodoGraba.Properties.ReadOnly = true;
            this.txtPeriodoGraba.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtPeriodoGraba_Properties_BeforeShowMenu);
            this.txtPeriodoGraba.Size = new System.Drawing.Size(100, 20);
            this.txtPeriodoGraba.TabIndex = 2;
            this.txtPeriodoGraba.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPeriodoGraba_KeyDown);
            this.txtPeriodoGraba.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPeriodoGraba_KeyPress);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(35, 40);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(57, 13);
            this.labelControl4.TabIndex = 0;
            this.labelControl4.Text = "Nuevo Año:";
            // 
            // cbCopiaPeriodo
            // 
            this.cbCopiaPeriodo.Location = new System.Drawing.Point(21, 8);
            this.cbCopiaPeriodo.Name = "cbCopiaPeriodo";
            this.cbCopiaPeriodo.Properties.AllowFocused = false;
            this.cbCopiaPeriodo.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.cbCopiaPeriodo.Properties.Appearance.Options.UseFont = true;
            this.cbCopiaPeriodo.Properties.Caption = "Copiar Año Anterior";
            this.cbCopiaPeriodo.Size = new System.Drawing.Size(177, 20);
            this.cbCopiaPeriodo.TabIndex = 33;
            this.cbCopiaPeriodo.CheckedChanged += new System.EventHandler(this.cbCopiaPeriodo_CheckedChanged);
            // 
            // calendarioFeriados
            // 
            this.calendarioFeriados.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.calendarioFeriados.Location = new System.Drawing.Point(13, 44);
            this.calendarioFeriados.Name = "calendarioFeriados";
            this.calendarioFeriados.Size = new System.Drawing.Size(251, 227);
            this.calendarioFeriados.TabIndex = 40;
            this.calendarioFeriados.CustomDrawDayNumberCell += new DevExpress.XtraEditors.Calendar.CustomDrawDayNumberCellEventHandler(this.calendarioFeriados_CustomDrawDayNumberCell);
            // 
            // toolTipController1
            // 
            this.toolTipController1.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.calendarioFeriados);
            this.panelControl1.Location = new System.Drawing.Point(608, 173);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(317, 315);
            this.panelControl1.TabIndex = 41;
            // 
            // labelControl6
            // 
            this.labelControl6.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.labelControl6.Appearance.Options.UseFont = true;
            this.labelControl6.Location = new System.Drawing.Point(372, 9);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(116, 16);
            this.labelControl6.TabIndex = 42;
            this.labelControl6.Text = "Ingreso Individual";
            // 
            // labelControl7
            // 
            this.labelControl7.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.labelControl7.Appearance.Options.UseFont = true;
            this.labelControl7.Location = new System.Drawing.Point(21, 149);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(139, 16);
            this.labelControl7.TabIndex = 42;
            this.labelControl7.Text = "REGISTROS ACTUALES";
            // 
            // labelControl8
            // 
            this.labelControl8.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.labelControl8.Appearance.Options.UseFont = true;
            this.labelControl8.Location = new System.Drawing.Point(708, 151);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(82, 16);
            this.labelControl8.TabIndex = 42;
            this.labelControl8.Text = "CALENDARIO";
            // 
            // frmFeriado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(938, 500);
            this.ControlBox = false;
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.cbCopiaPeriodo);
            this.Controls.Add(this.labelControl8);
            this.Controls.Add(this.labelControl7);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.PanelPeriodo);
            this.Controls.Add(this.gridFeriado);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnNuevo);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.PanelIngreso);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFeriado";
            this.Text = "Feriado";
            this.Load += new System.EventHandler(this.frmFeriado_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PanelIngreso)).EndInit();
            this.PanelIngreso.ResumeLayout(false);
            this.PanelIngreso.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtFecha.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFecha.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtdescripcion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridFeriado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewFeriado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PanelPeriodo)).EndInit();
            this.PanelPeriodo.ResumeLayout(false);
            this.PanelPeriodo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtYearCopia.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodoGraba.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbCopiaPeriodo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.calendarioFeriados.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl PanelIngreso;
        private DevExpress.XtraEditors.TextEdit txtdescripcion;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnEliminar;
        private DevExpress.XtraEditors.SimpleButton btnNuevo;
        private DevExpress.XtraEditors.SimpleButton btnGuardar;
        private DevExpress.XtraGrid.GridControl gridFeriado;
        private DevExpress.XtraGrid.Views.Grid.GridView viewFeriado;
        private DevExpress.XtraEditors.PanelControl PanelPeriodo;
        private DevExpress.XtraEditors.CheckEdit cbCopiaPeriodo;
        private DevExpress.XtraEditors.DateEdit dtFecha;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtPeriodoGraba;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtYearCopia;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.Controls.CalendarControl calendarioFeriados;
        private DevExpress.Utils.ToolTipController toolTipController1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.LabelControl labelControl8;
    }
}