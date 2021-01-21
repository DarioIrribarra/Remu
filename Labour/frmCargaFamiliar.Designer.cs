namespace Labour
{
    partial class frmCargaFamiliar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCargaFamiliar));
            this.cbMaternal = new DevExpress.XtraEditors.CheckEdit();
            this.cbInvalido = new DevExpress.XtraEditors.CheckEdit();
            this.dtTermino = new DevExpress.XtraEditors.DateEdit();
            this.dtIngreso = new DevExpress.XtraEditors.DateEdit();
            this.dtNacimiento = new DevExpress.XtraEditors.DateEdit();
            this.lblmsg = new DevExpress.XtraEditors.LabelControl();
            this.txtRelacionLegal = new DevExpress.XtraEditors.LookUpEdit();
            this.txtParentesco = new DevExpress.XtraEditors.LookUpEdit();
            this.txtSexo = new DevExpress.XtraEditors.LookUpEdit();
            this.cbNoAplica = new DevExpress.XtraEditors.CheckEdit();
            this.txtNombre = new DevExpress.XtraEditors.TextEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.txtRut = new DevExpress.XtraEditors.TextEdit();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.lblCargasFamiliares = new DevExpress.XtraEditors.LabelControl();
            this.gridFamiliares = new DevExpress.XtraGrid.GridControl();
            this.viewFamiliares = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnNuevo = new DevExpress.XtraEditors.SimpleButton();
            this.btnGrabar = new DevExpress.XtraEditors.SimpleButton();
            this.btnEliminar = new DevExpress.XtraEditors.SimpleButton();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.btnImprimirPdf = new DevExpress.XtraEditors.SimpleButton();
            this.btnImpresionRapida = new DevExpress.XtraEditors.SimpleButton();
            this.btnImprimeExcel = new DevExpress.XtraEditors.SimpleButton();
            this.btnPdf = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.cbMaternal.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbInvalido.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTermino.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTermino.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIngreso.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIngreso.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtNacimiento.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtNacimiento.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRelacionLegal.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtParentesco.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSexo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbNoAplica.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNombre.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRut.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridFamiliares)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewFamiliares)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbMaternal
            // 
            this.cbMaternal.Location = new System.Drawing.Point(467, 76);
            this.cbMaternal.Name = "cbMaternal";
            this.cbMaternal.Properties.Caption = "Maternal";
            this.cbMaternal.Size = new System.Drawing.Size(66, 19);
            this.cbMaternal.TabIndex = 7;
            // 
            // cbInvalido
            // 
            this.cbInvalido.Location = new System.Drawing.Point(404, 76);
            this.cbInvalido.Name = "cbInvalido";
            this.cbInvalido.Properties.Caption = "Invalido";
            this.cbInvalido.Size = new System.Drawing.Size(66, 19);
            this.cbInvalido.TabIndex = 6;
            // 
            // dtTermino
            // 
            this.dtTermino.EditValue = null;
            this.dtTermino.EnterMoveNextControl = true;
            this.dtTermino.Location = new System.Drawing.Point(355, 122);
            this.dtTermino.Name = "dtTermino";
            this.dtTermino.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dtTermino.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTermino.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTermino.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Classic;
            this.dtTermino.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            this.dtTermino.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.False;
            this.dtTermino.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.dtTermino_Properties_BeforeShowMenu);
            this.dtTermino.Size = new System.Drawing.Size(156, 20);
            this.dtTermino.TabIndex = 11;
            this.dtTermino.EditValueChanged += new System.EventHandler(this.dtTermino_EditValueChanged);
            // 
            // dtIngreso
            // 
            this.dtIngreso.EditValue = null;
            this.dtIngreso.EnterMoveNextControl = true;
            this.dtIngreso.Location = new System.Drawing.Point(111, 122);
            this.dtIngreso.Name = "dtIngreso";
            this.dtIngreso.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dtIngreso.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIngreso.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIngreso.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Classic;
            this.dtIngreso.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            this.dtIngreso.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.False;
            this.dtIngreso.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.dtIngreso_Properties_BeforeShowMenu);
            this.dtIngreso.Size = new System.Drawing.Size(156, 20);
            this.dtIngreso.TabIndex = 10;
            this.dtIngreso.EditValueChanged += new System.EventHandler(this.dtIngreso_EditValueChanged);
            // 
            // dtNacimiento
            // 
            this.dtNacimiento.EditValue = new System.DateTime(2018, 2, 27, 11, 52, 31, 0);
            this.dtNacimiento.EnterMoveNextControl = true;
            this.dtNacimiento.Location = new System.Drawing.Point(274, 77);
            this.dtNacimiento.Name = "dtNacimiento";
            this.dtNacimiento.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dtNacimiento.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtNacimiento.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtNacimiento.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Classic;
            this.dtNacimiento.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
            this.dtNacimiento.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.False;
            this.dtNacimiento.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.dtNacimiento_Properties_BeforeShowMenu);
            this.dtNacimiento.Size = new System.Drawing.Size(123, 20);
            this.dtNacimiento.TabIndex = 5;
            this.dtNacimiento.EditValueChanged += new System.EventHandler(this.dtNacimiento_EditValueChanged);
            // 
            // lblmsg
            // 
            this.lblmsg.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblmsg.Appearance.ForeColor = System.Drawing.Color.DarkRed;
            this.lblmsg.Appearance.Options.UseFont = true;
            this.lblmsg.Appearance.Options.UseForeColor = true;
            this.lblmsg.Location = new System.Drawing.Point(111, 145);
            this.lblmsg.Name = "lblmsg";
            this.lblmsg.Size = new System.Drawing.Size(51, 13);
            this.lblmsg.TabIndex = 111;
            this.lblmsg.Text = "message";
            this.lblmsg.Visible = false;
            // 
            // txtRelacionLegal
            // 
            this.txtRelacionLegal.EnterMoveNextControl = true;
            this.txtRelacionLegal.Location = new System.Drawing.Point(355, 99);
            this.txtRelacionLegal.Name = "txtRelacionLegal";
            this.txtRelacionLegal.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtRelacionLegal.Properties.PopupSizeable = false;
            this.txtRelacionLegal.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtRelacionLegal_Properties_BeforeShowMenu);
            this.txtRelacionLegal.Size = new System.Drawing.Size(156, 20);
            this.txtRelacionLegal.TabIndex = 9;
            // 
            // txtParentesco
            // 
            this.txtParentesco.EnterMoveNextControl = true;
            this.txtParentesco.Location = new System.Drawing.Point(111, 100);
            this.txtParentesco.Name = "txtParentesco";
            this.txtParentesco.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtParentesco.Properties.PopupSizeable = false;
            this.txtParentesco.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtParentesco_Properties_BeforeShowMenu);
            this.txtParentesco.Size = new System.Drawing.Size(156, 20);
            this.txtParentesco.TabIndex = 8;
            this.txtParentesco.EditValueChanged += new System.EventHandler(this.txtParentesco_EditValueChanged);
            // 
            // txtSexo
            // 
            this.txtSexo.EnterMoveNextControl = true;
            this.txtSexo.Location = new System.Drawing.Point(111, 77);
            this.txtSexo.Name = "txtSexo";
            this.txtSexo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtSexo.Properties.PopupSizeable = false;
            this.txtSexo.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtSexo_Properties_BeforeShowMenu);
            this.txtSexo.Size = new System.Drawing.Size(62, 20);
            this.txtSexo.TabIndex = 4;
            // 
            // cbNoAplica
            // 
            this.cbNoAplica.Location = new System.Drawing.Point(234, 33);
            this.cbNoAplica.Name = "cbNoAplica";
            this.cbNoAplica.Properties.Caption = "No Aplica";
            this.cbNoAplica.Size = new System.Drawing.Size(75, 19);
            this.cbNoAplica.TabIndex = 2;
            this.cbNoAplica.TabStop = false;
            this.cbNoAplica.CheckedChanged += new System.EventHandler(this.cbNoAplica_CheckedChanged);
            // 
            // txtNombre
            // 
            this.txtNombre.EnterMoveNextControl = true;
            this.txtNombre.Location = new System.Drawing.Point(111, 55);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombre.Properties.MaxLength = 100;
            this.txtNombre.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtNombre_Properties_BeforeShowMenu);
            this.txtNombre.Size = new System.Drawing.Size(359, 20);
            this.txtNombre.TabIndex = 3;
            this.txtNombre.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNombre_KeyDown);
            this.txtNombre.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNombre_KeyPress);
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(276, 103);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(72, 13);
            this.labelControl7.TabIndex = 0;
            this.labelControl7.Text = "Relacion Legal:";
            // 
            // txtRut
            // 
            this.txtRut.EnterMoveNextControl = true;
            this.txtRut.Location = new System.Drawing.Point(111, 33);
            this.txtRut.Name = "txtRut";
            this.txtRut.Properties.MaxLength = 12;
            this.txtRut.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtRut_Properties_BeforeShowMenu);
            this.txtRut.Size = new System.Drawing.Size(118, 20);
            this.txtRut.TabIndex = 1;
            this.txtRut.EditValueChanged += new System.EventHandler(this.txtRut_EditValueChanged);
            this.txtRut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtRut_KeyDown);
            this.txtRut.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRut_KeyPress);
            this.txtRut.Leave += new System.EventHandler(this.txtRut_Leave);
            // 
            // labelControl9
            // 
            this.labelControl9.Location = new System.Drawing.Point(276, 125);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(74, 13);
            this.labelControl9.TabIndex = 0;
            this.labelControl9.Text = "Fecha Termino:";
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(24, 125);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(76, 13);
            this.labelControl8.TabIndex = 0;
            this.labelControl8.Text = "Fecha Ingreso :";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(24, 103);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(76, 13);
            this.labelControl6.TabIndex = 0;
            this.labelControl6.Text = "Parentesco      :";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(179, 80);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(88, 13);
            this.labelControl5.TabIndex = 0;
            this.labelControl5.Text = "Fecha Nacimiento:";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(24, 81);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(76, 13);
            this.labelControl4.TabIndex = 0;
            this.labelControl4.Text = "Sexo                :";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(24, 58);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(77, 13);
            this.labelControl3.TabIndex = 0;
            this.labelControl3.Text = "Nombre            :";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(24, 35);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(77, 13);
            this.labelControl2.TabIndex = 0;
            this.labelControl2.Text = "Rut Carga        :";
            // 
            // lblCargasFamiliares
            // 
            this.lblCargasFamiliares.Appearance.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.lblCargasFamiliares.Appearance.Options.UseFont = true;
            this.lblCargasFamiliares.Location = new System.Drawing.Point(19, 26);
            this.lblCargasFamiliares.Name = "lblCargasFamiliares";
            this.lblCargasFamiliares.Size = new System.Drawing.Size(274, 18);
            this.lblCargasFamiliares.TabIndex = 0;
            this.lblCargasFamiliares.Text = "CARGAS FAMILIARES ASOCIADAS A:";
            // 
            // gridFamiliares
            // 
            this.gridFamiliares.Location = new System.Drawing.Point(15, 20);
            this.gridFamiliares.MainView = this.viewFamiliares;
            this.gridFamiliares.Name = "gridFamiliares";
            this.gridFamiliares.Size = new System.Drawing.Size(743, 313);
            this.gridFamiliares.TabIndex = 18;
            this.gridFamiliares.TabStop = false;
            this.gridFamiliares.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewFamiliares});
            this.gridFamiliares.Click += new System.EventHandler(this.gridFamiliares_Click);
            this.gridFamiliares.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridFamiliares_KeyUp);
            // 
            // viewFamiliares
            // 
            this.viewFamiliares.GridControl = this.gridFamiliares;
            this.viewFamiliares.Name = "viewFamiliares";
            this.viewFamiliares.OptionsView.ShowGroupPanel = false;
            // 
            // btnNuevo
            // 
            this.btnNuevo.AllowFocus = false;
            this.btnNuevo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNuevo.ImageOptions.Image")));
            this.btnNuevo.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnNuevo.Location = new System.Drawing.Point(8, 15);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(72, 29);
            this.btnNuevo.TabIndex = 12;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.ToolTip = "Nuevo Registro";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnGrabar
            // 
            this.btnGrabar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGrabar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGrabar.ImageOptions.Image")));
            this.btnGrabar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGrabar.Location = new System.Drawing.Point(82, 15);
            this.btnGrabar.Name = "btnGrabar";
            this.btnGrabar.Size = new System.Drawing.Size(66, 29);
            this.btnGrabar.TabIndex = 13;
            this.btnGrabar.Text = "Guardar";
            this.btnGrabar.ToolTip = "Guardar Registro";
            this.btnGrabar.Click += new System.EventHandler(this.btnGrabar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.AllowFocus = false;
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminar.ImageOptions.Image")));
            this.btnEliminar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnEliminar.Location = new System.Drawing.Point(152, 15);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(30, 29);
            this.btnEliminar.TabIndex = 14;
            this.btnEliminar.ToolTip = "Eliminar Trabajador";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(754, 14);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(32, 30);
            this.btnSalir.TabIndex = 116;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnImprimirPdf
            // 
            this.btnImprimirPdf.AllowFocus = false;
            this.btnImprimirPdf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImprimirPdf.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImprimirPdf.ImageOptions.Image")));
            this.btnImprimirPdf.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnImprimirPdf.Location = new System.Drawing.Point(586, 14);
            this.btnImprimirPdf.Name = "btnImprimirPdf";
            this.btnImprimirPdf.Size = new System.Drawing.Size(42, 30);
            this.btnImprimirPdf.TabIndex = 16;
            this.btnImprimirPdf.TabStop = false;
            this.btnImprimirPdf.Click += new System.EventHandler(this.btnImprimirPdf_Click);
            // 
            // btnImpresionRapida
            // 
            this.btnImpresionRapida.AllowFocus = false;
            this.btnImpresionRapida.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImpresionRapida.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImpresionRapida.ImageOptions.Image")));
            this.btnImpresionRapida.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnImpresionRapida.Location = new System.Drawing.Point(722, 14);
            this.btnImpresionRapida.Name = "btnImpresionRapida";
            this.btnImpresionRapida.Size = new System.Drawing.Size(42, 30);
            this.btnImpresionRapida.TabIndex = 17;
            this.btnImpresionRapida.TabStop = false;
            this.btnImpresionRapida.Click += new System.EventHandler(this.btnImpresionRapida_Click);
            // 
            // btnImprimeExcel
            // 
            this.btnImprimeExcel.AllowFocus = false;
            this.btnImprimeExcel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImprimeExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImprimeExcel.ImageOptions.Image")));
            this.btnImprimeExcel.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnImprimeExcel.Location = new System.Drawing.Point(633, 14);
            this.btnImprimeExcel.Name = "btnImprimeExcel";
            this.btnImprimeExcel.Size = new System.Drawing.Size(42, 30);
            this.btnImprimeExcel.TabIndex = 15;
            this.btnImprimeExcel.TabStop = false;
            this.btnImprimeExcel.Click += new System.EventHandler(this.btnImprimeExcel_Click);
            // 
            // btnPdf
            // 
            this.btnPdf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPdf.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPdf.ImageOptions.Image")));
            this.btnPdf.Location = new System.Drawing.Point(680, 14);
            this.btnPdf.Name = "btnPdf";
            this.btnPdf.Size = new System.Drawing.Size(38, 30);
            this.btnPdf.TabIndex = 117;
            this.btnPdf.Click += new System.EventHandler(this.btnPdf_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gridFamiliares);
            this.groupBox1.Location = new System.Drawing.Point(12, 302);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(774, 352);
            this.groupBox1.TabIndex = 118;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Registros Actuales";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnGrabar);
            this.groupBox2.Controls.Add(this.btnNuevo);
            this.groupBox2.Controls.Add(this.btnPdf);
            this.groupBox2.Controls.Add(this.btnEliminar);
            this.groupBox2.Controls.Add(this.btnImpresionRapida);
            this.groupBox2.Controls.Add(this.btnImprimirPdf);
            this.groupBox2.Controls.Add(this.btnImprimeExcel);
            this.groupBox2.Location = new System.Drawing.Point(13, 243);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(773, 53);
            this.groupBox2.TabIndex = 119;
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbMaternal);
            this.groupBox3.Controls.Add(this.txtRut);
            this.groupBox3.Controls.Add(this.cbInvalido);
            this.groupBox3.Controls.Add(this.labelControl2);
            this.groupBox3.Controls.Add(this.dtTermino);
            this.groupBox3.Controls.Add(this.labelControl3);
            this.groupBox3.Controls.Add(this.dtIngreso);
            this.groupBox3.Controls.Add(this.labelControl4);
            this.groupBox3.Controls.Add(this.dtNacimiento);
            this.groupBox3.Controls.Add(this.labelControl5);
            this.groupBox3.Controls.Add(this.lblmsg);
            this.groupBox3.Controls.Add(this.labelControl6);
            this.groupBox3.Controls.Add(this.txtRelacionLegal);
            this.groupBox3.Controls.Add(this.labelControl8);
            this.groupBox3.Controls.Add(this.txtParentesco);
            this.groupBox3.Controls.Add(this.labelControl9);
            this.groupBox3.Controls.Add(this.txtSexo);
            this.groupBox3.Controls.Add(this.labelControl7);
            this.groupBox3.Controls.Add(this.cbNoAplica);
            this.groupBox3.Controls.Add(this.txtNombre);
            this.groupBox3.Location = new System.Drawing.Point(13, 51);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(773, 187);
            this.groupBox3.TabIndex = 120;
            this.groupBox3.TabStop = false;
            // 
            // frmCargaFamiliar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 666);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblCargasFamiliares);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCargaFamiliar";
            this.Text = "Cargas Familiares";
            this.Load += new System.EventHandler(this.frmCargaFamiliar_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cbMaternal.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbInvalido.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTermino.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTermino.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIngreso.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIngreso.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtNacimiento.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtNacimiento.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRelacionLegal.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtParentesco.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSexo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbNoAplica.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNombre.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRut.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridFamiliares)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewFamiliares)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.DateEdit dtNacimiento;
        private DevExpress.XtraEditors.LookUpEdit txtParentesco;
        private DevExpress.XtraEditors.LookUpEdit txtSexo;
        private DevExpress.XtraEditors.CheckEdit cbNoAplica;
        private DevExpress.XtraEditors.TextEdit txtNombre;
        private DevExpress.XtraEditors.TextEdit txtRut;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl lblCargasFamiliares;
        private DevExpress.XtraEditors.LookUpEdit txtRelacionLegal;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.DateEdit dtTermino;
        private DevExpress.XtraEditors.DateEdit dtIngreso;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraGrid.GridControl gridFamiliares;
        private DevExpress.XtraGrid.Views.Grid.GridView viewFamiliares;
        private DevExpress.XtraEditors.SimpleButton btnNuevo;
        private DevExpress.XtraEditors.SimpleButton btnGrabar;
        private DevExpress.XtraEditors.SimpleButton btnEliminar;
        private DevExpress.XtraEditors.LabelControl lblmsg;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnImprimirPdf;
        private DevExpress.XtraEditors.SimpleButton btnImpresionRapida;
        private DevExpress.XtraEditors.SimpleButton btnImprimeExcel;
        private DevExpress.XtraEditors.CheckEdit cbMaternal;
        private DevExpress.XtraEditors.CheckEdit cbInvalido;
        private DevExpress.XtraEditors.SimpleButton btnPdf;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}