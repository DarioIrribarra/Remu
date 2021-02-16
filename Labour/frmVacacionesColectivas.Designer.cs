namespace Labour
{
    partial class frmVacacionesColectivas
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVacacionesColectivas));
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.btnGenerar = new DevExpress.XtraEditors.SimpleButton();
            this.txtComboPeriodo = new DevExpress.XtraEditors.LookUpEdit();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtConjunto = new DevExpress.XtraEditors.TextEdit();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.txtDiasVac = new DevExpress.XtraEditors.TextEdit();
            this.txtTipo = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl12 = new DevExpress.XtraEditors.LabelControl();
            this.dtRetornoTrabajo = new DevExpress.XtraEditors.DateEdit();
            this.dtSalida = new DevExpress.XtraEditors.DateEdit();
            this.dtFinaliza = new DevExpress.XtraEditors.DateEdit();
            this.labelControl13 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl14 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl15 = new DevExpress.XtraEditors.LabelControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnEditarReporte = new DevExpress.XtraEditors.SimpleButton();
            this.BarraCalculo = new DevExpress.XtraEditors.ProgressBarControl();
            this.lblError = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitFormRemuneraciones), true, true);
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDiasVac.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTipo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtRetornoTrabajo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtRetornoTrabajo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSalida.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSalida.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFinaliza.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFinaliza.Properties)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BarraCalculo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSalir.Location = new System.Drawing.Point(356, 25);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(44, 37);
            this.btnSalir.TabIndex = 11;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnGenerar
            // 
            this.btnGenerar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGenerar.ImageOptions.Image")));
            this.btnGenerar.Location = new System.Drawing.Point(96, 257);
            this.btnGenerar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(103, 43);
            this.btnGenerar.TabIndex = 10;
            this.btnGenerar.Text = "Generar";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            // 
            // txtComboPeriodo
            // 
            this.txtComboPeriodo.Location = new System.Drawing.Point(96, 37);
            this.txtComboPeriodo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtComboPeriodo.Name = "txtComboPeriodo";
            this.txtComboPeriodo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtComboPeriodo.Properties.PopupSizeable = false;
            this.txtComboPeriodo.Size = new System.Drawing.Size(159, 22);
            this.txtComboPeriodo.TabIndex = 1;
            // 
            // btnConjunto
            // 
            this.btnConjunto.AllowFocus = false;
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(168, 89);
            this.btnConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(30, 26);
            this.btnConjunto.TabIndex = 4;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(44, 41);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 16);
            this.labelControl1.TabIndex = 116;
            this.labelControl1.Text = "Periodo:";
            // 
            // txtConjunto
            // 
            this.txtConjunto.EnterMoveNextControl = true;
            this.txtConjunto.Location = new System.Drawing.Point(96, 90);
            this.txtConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtConjunto.Name = "txtConjunto";
            this.txtConjunto.Properties.MaxLength = 12;
            this.txtConjunto.Size = new System.Drawing.Size(65, 22);
            this.txtConjunto.TabIndex = 3;
            this.txtConjunto.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConjunto_KeyDown);
            // 
            // cbTodos
            // 
            this.cbTodos.Location = new System.Drawing.Point(96, 64);
            this.cbTodos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos los registros del periodo";
            this.cbTodos.Size = new System.Drawing.Size(215, 20);
            this.cbTodos.TabIndex = 2;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // txtDiasVac
            // 
            this.txtDiasVac.EditValue = "1";
            this.txtDiasVac.EnterMoveNextControl = true;
            this.txtDiasVac.Location = new System.Drawing.Point(96, 170);
            this.txtDiasVac.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtDiasVac.Name = "txtDiasVac";
            this.txtDiasVac.Properties.MaxLength = 4;
            this.txtDiasVac.Size = new System.Drawing.Size(69, 22);
            this.txtDiasVac.TabIndex = 7;
            this.txtDiasVac.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDiasVac_KeyDown);
            this.txtDiasVac.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDiasVac_KeyPress);
            // 
            // txtTipo
            // 
            this.txtTipo.EnterMoveNextControl = true;
            this.txtTipo.Location = new System.Drawing.Point(96, 117);
            this.txtTipo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtTipo.Name = "txtTipo";
            this.txtTipo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtTipo.Properties.PopupSizeable = false;
            this.txtTipo.Size = new System.Drawing.Size(174, 22);
            this.txtTipo.TabIndex = 5;
            // 
            // labelControl12
            // 
            this.labelControl12.Location = new System.Drawing.Point(54, 148);
            this.labelControl12.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl12.Name = "labelControl12";
            this.labelControl12.Size = new System.Drawing.Size(40, 16);
            this.labelControl12.TabIndex = 128;
            this.labelControl12.Text = "Salida:";
            // 
            // dtRetornoTrabajo
            // 
            this.dtRetornoTrabajo.EditValue = null;
            this.dtRetornoTrabajo.EnterMoveNextControl = true;
            this.dtRetornoTrabajo.Location = new System.Drawing.Point(96, 225);
            this.dtRetornoTrabajo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dtRetornoTrabajo.Name = "dtRetornoTrabajo";
            this.dtRetornoTrabajo.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dtRetornoTrabajo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtRetornoTrabajo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtRetornoTrabajo.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Classic;
            this.dtRetornoTrabajo.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            this.dtRetornoTrabajo.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.False;
            this.dtRetornoTrabajo.Size = new System.Drawing.Size(100, 22);
            this.dtRetornoTrabajo.TabIndex = 9;
            // 
            // dtSalida
            // 
            this.dtSalida.EditValue = null;
            this.dtSalida.EnterMoveNextControl = true;
            this.dtSalida.Location = new System.Drawing.Point(96, 144);
            this.dtSalida.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dtSalida.Name = "dtSalida";
            this.dtSalida.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dtSalida.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtSalida.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtSalida.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Classic;
            this.dtSalida.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            this.dtSalida.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.False;
            this.dtSalida.Size = new System.Drawing.Size(117, 22);
            this.dtSalida.TabIndex = 6;
            // 
            // dtFinaliza
            // 
            this.dtFinaliza.EditValue = null;
            this.dtFinaliza.EnterMoveNextControl = true;
            this.dtFinaliza.Location = new System.Drawing.Point(96, 197);
            this.dtFinaliza.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dtFinaliza.Name = "dtFinaliza";
            this.dtFinaliza.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dtFinaliza.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFinaliza.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFinaliza.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Classic;
            this.dtFinaliza.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            this.dtFinaliza.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.False;
            this.dtFinaliza.Size = new System.Drawing.Size(117, 22);
            this.dtFinaliza.TabIndex = 8;
            // 
            // labelControl13
            // 
            this.labelControl13.Location = new System.Drawing.Point(45, 201);
            this.labelControl13.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl13.Name = "labelControl13";
            this.labelControl13.Size = new System.Drawing.Size(48, 16);
            this.labelControl13.TabIndex = 129;
            this.labelControl13.Text = "Finaliza:";
            // 
            // labelControl14
            // 
            this.labelControl14.Location = new System.Drawing.Point(63, 175);
            this.labelControl14.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl14.Name = "labelControl14";
            this.labelControl14.Size = new System.Drawing.Size(29, 16);
            this.labelControl14.TabIndex = 130;
            this.labelControl14.Text = "Dias:";
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(41, 231);
            this.labelControl8.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(50, 16);
            this.labelControl8.TabIndex = 131;
            this.labelControl8.Text = "Retorno:";
            // 
            // labelControl15
            // 
            this.labelControl15.Location = new System.Drawing.Point(59, 122);
            this.labelControl15.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl15.Name = "labelControl15";
            this.labelControl15.Size = new System.Drawing.Size(30, 16);
            this.labelControl15.TabIndex = 132;
            this.labelControl15.Text = "Tipo:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnEditarReporte);
            this.groupBox1.Controls.Add(this.BarraCalculo);
            this.groupBox1.Controls.Add(this.lblError);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Controls.Add(this.btnSalir);
            this.groupBox1.Controls.Add(this.txtDiasVac);
            this.groupBox1.Controls.Add(this.txtComboPeriodo);
            this.groupBox1.Controls.Add(this.txtTipo);
            this.groupBox1.Controls.Add(this.cbTodos);
            this.groupBox1.Controls.Add(this.labelControl12);
            this.groupBox1.Controls.Add(this.txtConjunto);
            this.groupBox1.Controls.Add(this.dtRetornoTrabajo);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Controls.Add(this.dtSalida);
            this.groupBox1.Controls.Add(this.btnConjunto);
            this.groupBox1.Controls.Add(this.dtFinaliza);
            this.groupBox1.Controls.Add(this.btnGenerar);
            this.groupBox1.Controls.Add(this.labelControl13);
            this.groupBox1.Controls.Add(this.labelControl15);
            this.groupBox1.Controls.Add(this.labelControl14);
            this.groupBox1.Controls.Add(this.labelControl8);
            this.groupBox1.Location = new System.Drawing.Point(15, 16);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(422, 396);
            this.groupBox1.TabIndex = 133;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Formulario";
            // 
            // btnEditarReporte
            // 
            this.btnEditarReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditarReporte.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditarReporte.ImageOptions.Image")));
            this.btnEditarReporte.Location = new System.Drawing.Point(205, 260);
            this.btnEditarReporte.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEditarReporte.Name = "btnEditarReporte";
            this.btnEditarReporte.Size = new System.Drawing.Size(114, 37);
            this.btnEditarReporte.TabIndex = 136;
            this.btnEditarReporte.Text = "Editar\r\nReporte\r\n";
            this.btnEditarReporte.Click += new System.EventHandler(this.btnEditarReporte_Click);
            // 
            // BarraCalculo
            // 
            this.BarraCalculo.Location = new System.Drawing.Point(7, 331);
            this.BarraCalculo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BarraCalculo.Name = "BarraCalculo";
            this.BarraCalculo.Size = new System.Drawing.Size(393, 20);
            this.BarraCalculo.TabIndex = 135;
            // 
            // lblError
            // 
            this.lblError.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblError.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.lblError.Appearance.Options.UseFont = true;
            this.lblError.Appearance.Options.UseForeColor = true;
            this.lblError.Location = new System.Drawing.Point(96, 308);
            this.lblError.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(59, 17);
            this.lblError.TabIndex = 134;
            this.lblError.Text = "Message";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(29, 92);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(60, 16);
            this.labelControl2.TabIndex = 133;
            this.labelControl2.Text = "Condición:";
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // frmVacacionesColectivas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 452);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmVacacionesColectivas";
            this.Text = "Comprobantes de vacaciones";
            this.Load += new System.EventHandler(this.frmVacacionesColectivas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDiasVac.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTipo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtRetornoTrabajo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtRetornoTrabajo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSalida.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSalida.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFinaliza.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFinaliza.Properties)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BarraCalculo.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnGenerar;
        private DevExpress.XtraEditors.LookUpEdit txtComboPeriodo;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtConjunto;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.TextEdit txtDiasVac;
        private DevExpress.XtraEditors.LookUpEdit txtTipo;
        private DevExpress.XtraEditors.LabelControl labelControl12;
        private DevExpress.XtraEditors.DateEdit dtRetornoTrabajo;
        private DevExpress.XtraEditors.DateEdit dtSalida;
        private DevExpress.XtraEditors.DateEdit dtFinaliza;
        private DevExpress.XtraEditors.LabelControl labelControl13;
        private DevExpress.XtraEditors.LabelControl labelControl14;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.LabelControl labelControl15;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl lblError;
        private DevExpress.XtraEditors.ProgressBarControl BarraCalculo;
        private DevExpress.XtraEditors.SimpleButton btnEditarReporte;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
    }
}