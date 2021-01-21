namespace Labour
{
    partial class frmFiniquitosMasivos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFiniquitosMasivos));
            this.btnPlantilla = new DevExpress.XtraEditors.SimpleButton();
            this.txtRutaPlantilla = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.dtFecha = new DevExpress.XtraEditors.DateEdit();
            this.btnSalida = new DevExpress.XtraEditors.SimpleButton();
            this.txtSalida = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.btnGenera = new DevExpress.XtraEditors.SimpleButton();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.cbHoras = new DevExpress.XtraEditors.CheckEdit();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.txtCondicion = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.BarraCalculo = new DevExpress.XtraEditors.ProgressBarControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtPeriodo = new DevExpress.XtraEditors.LookUpEdit();
            this.lblname = new DevExpress.XtraEditors.LabelControl();
            this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
            this.txtSeguroCes = new DevExpress.XtraEditors.TextEdit();
            this.cbSeguroCes = new DevExpress.XtraEditors.CheckEdit();
            this.cbPrestamo = new DevExpress.XtraEditors.CheckEdit();
            this.cbAviso = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaPlantilla.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFecha.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFecha.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSalida.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbHoras.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCondicion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BarraCalculo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSeguroCes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbSeguroCes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPrestamo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAviso.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPlantilla
            // 
            this.btnPlantilla.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPlantilla.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPlantilla.ImageOptions.Image")));
            this.btnPlantilla.Location = new System.Drawing.Point(369, 58);
            this.btnPlantilla.Name = "btnPlantilla";
            this.btnPlantilla.Size = new System.Drawing.Size(40, 26);
            this.btnPlantilla.TabIndex = 4;
            this.btnPlantilla.Click += new System.EventHandler(this.btnPlantilla_Click);
            // 
            // txtRutaPlantilla
            // 
            this.txtRutaPlantilla.Location = new System.Drawing.Point(122, 65);
            this.txtRutaPlantilla.Name = "txtRutaPlantilla";
            this.txtRutaPlantilla.Size = new System.Drawing.Size(241, 20);
            this.txtRutaPlantilla.TabIndex = 3;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(32, 45);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(76, 13);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "Fecha Finiquito:";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(42, 65);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(66, 13);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Ruta plantilla:";
            // 
            // dtFecha
            // 
            this.dtFecha.EditValue = null;
            this.dtFecha.Location = new System.Drawing.Point(122, 41);
            this.dtFecha.Name = "dtFecha";
            this.dtFecha.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dtFecha.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFecha.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFecha.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            this.dtFecha.Size = new System.Drawing.Size(100, 20);
            this.dtFecha.TabIndex = 2;
            // 
            // btnSalida
            // 
            this.btnSalida.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalida.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalida.ImageOptions.Image")));
            this.btnSalida.Location = new System.Drawing.Point(369, 87);
            this.btnSalida.Name = "btnSalida";
            this.btnSalida.Size = new System.Drawing.Size(40, 26);
            this.btnSalida.TabIndex = 6;
            this.btnSalida.Click += new System.EventHandler(this.btnSalida_Click);
            // 
            // txtSalida
            // 
            this.txtSalida.Location = new System.Drawing.Point(122, 89);
            this.txtSalida.Name = "txtSalida";
            this.txtSalida.Size = new System.Drawing.Size(241, 20);
            this.txtSalida.TabIndex = 5;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(49, 89);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(59, 13);
            this.labelControl3.TabIndex = 2;
            this.labelControl3.Text = "Rusa Salida:";
            // 
            // btnGenera
            // 
            this.btnGenera.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenera.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGenera.ImageOptions.Image")));
            this.btnGenera.Location = new System.Drawing.Point(122, 265);
            this.btnGenera.Name = "btnGenera";
            this.btnGenera.Size = new System.Drawing.Size(43, 34);
            this.btnGenera.TabIndex = 11;
            this.btnGenera.Click += new System.EventHandler(this.btnGenera_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.ImageOptions.Image")));
            this.btnSalir.Location = new System.Drawing.Point(452, 12);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 30);
            this.btnSalir.TabIndex = 13;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // cbHoras
            // 
            this.cbHoras.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbHoras.Location = new System.Drawing.Point(122, 111);
            this.cbHoras.Name = "cbHoras";
            this.cbHoras.Properties.Caption = "Es remuneración pagada por horas?";
            this.cbHoras.Size = new System.Drawing.Size(196, 19);
            this.cbHoras.TabIndex = 7;
            // 
            // cbTodos
            // 
            this.cbTodos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbTodos.EditValue = true;
            this.cbTodos.Location = new System.Drawing.Point(122, 131);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos los registros del periodo";
            this.cbTodos.Size = new System.Drawing.Size(248, 19);
            this.cbTodos.TabIndex = 8;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // txtCondicion
            // 
            this.txtCondicion.Enabled = false;
            this.txtCondicion.Location = new System.Drawing.Point(122, 155);
            this.txtCondicion.Name = "txtCondicion";
            this.txtCondicion.Properties.MaxLength = 12;
            this.txtCondicion.Size = new System.Drawing.Size(56, 20);
            this.txtCondicion.TabIndex = 9;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(58, 159);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(50, 13);
            this.labelControl4.TabIndex = 13;
            this.labelControl4.Text = "Condición:";
            // 
            // btnConjunto
            // 
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.Enabled = false;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(182, 152);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(25, 24);
            this.btnConjunto.TabIndex = 10;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // BarraCalculo
            // 
            this.BarraCalculo.Location = new System.Drawing.Point(8, 306);
            this.BarraCalculo.Name = "BarraCalculo";
            this.BarraCalculo.Size = new System.Drawing.Size(488, 16);
            this.BarraCalculo.TabIndex = 12;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(68, 24);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(40, 13);
            this.labelControl5.TabIndex = 14;
            this.labelControl5.Text = "Periodo:";
            // 
            // txtPeriodo
            // 
            this.txtPeriodo.Location = new System.Drawing.Point(122, 20);
            this.txtPeriodo.Name = "txtPeriodo";
            this.txtPeriodo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtPeriodo.Size = new System.Drawing.Size(141, 20);
            this.txtPeriodo.TabIndex = 1;
            // 
            // lblname
            // 
            this.lblname.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.lblname.Appearance.Options.UseFont = true;
            this.lblname.Location = new System.Drawing.Point(15, 306);
            this.lblname.Name = "lblname";
            this.lblname.Size = new System.Drawing.Size(34, 13);
            this.lblname.TabIndex = 16;
            this.lblname.Text = "Name";
            this.lblname.Visible = false;
            // 
            // labelControl10
            // 
            this.labelControl10.Location = new System.Drawing.Point(113, 206);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Size = new System.Drawing.Size(6, 13);
            this.labelControl10.TabIndex = 23;
            this.labelControl10.Text = "$";
            // 
            // txtSeguroCes
            // 
            this.txtSeguroCes.Enabled = false;
            this.txtSeguroCes.EnterMoveNextControl = true;
            this.txtSeguroCes.Location = new System.Drawing.Point(123, 203);
            this.txtSeguroCes.Name = "txtSeguroCes";
            this.txtSeguroCes.Properties.MaxLength = 7;
            this.txtSeguroCes.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtSeguroCes_Properties_BeforeShowMenu);
            this.txtSeguroCes.Size = new System.Drawing.Size(77, 20);
            this.txtSeguroCes.TabIndex = 20;
            this.txtSeguroCes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSeguroCes_KeyPress);
            // 
            // cbSeguroCes
            // 
            this.cbSeguroCes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbSeguroCes.Location = new System.Drawing.Point(122, 182);
            this.cbSeguroCes.Name = "cbSeguroCes";
            this.cbSeguroCes.Properties.Caption = "Descuenta seguro de cesantía";
            this.cbSeguroCes.Size = new System.Drawing.Size(177, 19);
            this.cbSeguroCes.TabIndex = 19;
            this.cbSeguroCes.CheckedChanged += new System.EventHandler(this.cbSeguroCes_CheckedChanged);
            // 
            // cbPrestamo
            // 
            this.cbPrestamo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbPrestamo.Location = new System.Drawing.Point(125, 243);
            this.cbPrestamo.Name = "cbPrestamo";
            this.cbPrestamo.Properties.Caption = "Descuenta prestamos";
            this.cbPrestamo.Size = new System.Drawing.Size(139, 19);
            this.cbPrestamo.TabIndex = 22;
            this.cbPrestamo.ToolTip = "Se incluyen o descuentan todos los\r\nprestamos adeudados.";
            this.cbPrestamo.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
            // 
            // cbAviso
            // 
            this.cbAviso.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbAviso.Location = new System.Drawing.Point(125, 225);
            this.cbAviso.Name = "cbAviso";
            this.cbAviso.Properties.Caption = "Avisa termino de contrato con 30 días de anticipación?";
            this.cbAviso.Size = new System.Drawing.Size(292, 19);
            this.cbAviso.TabIndex = 21;
            this.cbAviso.ToolTip = "Si selecciona esta opción, usted está indicando \r\nque avisó el termino del contra" +
    "to al trabajador\r\ncon un mínimo de 30 días antes de la\r\nfecha de despido.";
            this.cbAviso.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
            // 
            // frmFiniquitosMasivos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 328);
            this.Controls.Add(this.labelControl10);
            this.Controls.Add(this.txtSeguroCes);
            this.Controls.Add(this.cbSeguroCes);
            this.Controls.Add(this.cbPrestamo);
            this.Controls.Add(this.cbAviso);
            this.Controls.Add(this.lblname);
            this.Controls.Add(this.txtPeriodo);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.BarraCalculo);
            this.Controls.Add(this.btnConjunto);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.txtCondicion);
            this.Controls.Add(this.cbTodos);
            this.Controls.Add(this.cbHoras);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.btnGenera);
            this.Controls.Add(this.dtFecha);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.txtSalida);
            this.Controls.Add(this.btnSalida);
            this.Controls.Add(this.txtRutaPlantilla);
            this.Controls.Add(this.btnPlantilla);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFiniquitosMasivos";
            this.Text = "Emisión de finiquitos";
            this.Load += new System.EventHandler(this.frmFiniquitosMasivos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtRutaPlantilla.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFecha.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFecha.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSalida.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbHoras.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCondicion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BarraCalculo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSeguroCes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbSeguroCes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPrestamo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAviso.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnPlantilla;
        private DevExpress.XtraEditors.TextEdit txtRutaPlantilla;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.DateEdit dtFecha;
        private DevExpress.XtraEditors.SimpleButton btnSalida;
        private DevExpress.XtraEditors.TextEdit txtSalida;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnGenera;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.CheckEdit cbHoras;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.TextEdit txtCondicion;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.ProgressBarControl BarraCalculo;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LookUpEdit txtPeriodo;
        private DevExpress.XtraEditors.LabelControl lblname;
        private DevExpress.XtraEditors.LabelControl labelControl10;
        private DevExpress.XtraEditors.TextEdit txtSeguroCes;
        private DevExpress.XtraEditors.CheckEdit cbSeguroCes;
        private DevExpress.XtraEditors.CheckEdit cbPrestamo;
        private DevExpress.XtraEditors.CheckEdit cbAviso;
    }
}