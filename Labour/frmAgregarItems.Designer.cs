namespace Labour
{
    partial class frmAgregarItems
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAgregarItems));
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtConjunto = new DevExpress.XtraEditors.TextEdit();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.btnAgregar = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.txtTotalCuotas = new DevExpress.XtraEditors.TextEdit();
            this.txtNumCuota = new DevExpress.XtraEditors.TextEdit();
            this.cbPesos = new DevExpress.XtraEditors.CheckEdit();
            this.cbPorcentaje = new DevExpress.XtraEditors.CheckEdit();
            this.cbUf = new DevExpress.XtraEditors.CheckEdit();
            this.cbCuota = new DevExpress.XtraEditors.CheckEdit();
            this.cbProporcional = new DevExpress.XtraEditors.CheckEdit();
            this.txtValor = new DevExpress.XtraEditors.TextEdit();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.cbTope = new DevExpress.XtraEditors.CheckEdit();
            this.cbPermanente = new DevExpress.XtraEditors.CheckEdit();
            this.txtDescripcion = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.txtTipo = new DevExpress.XtraEditors.TextEdit();
            this.txtItem = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.txtOrden = new DevExpress.XtraEditors.TextEdit();
            this.txtPeriodo = new DevExpress.XtraEditors.TextEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.lblMensaje = new DevExpress.XtraEditors.LabelControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ComboFormula = new DevExpress.XtraEditors.LookUpEdit();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalCuotas.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumCuota.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPesos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPorcentaje.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbUf.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbCuota.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbProporcional.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTope.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPermanente.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescripcion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTipo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItem.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrden.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ComboFormula.Properties)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(14, 102);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(50, 13);
            this.labelControl2.TabIndex = 0;
            this.labelControl2.Text = "Condición:";
            this.labelControl2.Click += new System.EventHandler(this.labelControl2_Click);
            // 
            // txtConjunto
            // 
            this.txtConjunto.Enabled = false;
            this.txtConjunto.EnterMoveNextControl = true;
            this.txtConjunto.Location = new System.Drawing.Point(73, 99);
            this.txtConjunto.Name = "txtConjunto";
            this.txtConjunto.Properties.MaxLength = 12;
            this.txtConjunto.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtConjunto_Properties_BeforeShowMenu);
            this.txtConjunto.Size = new System.Drawing.Size(60, 20);
            this.txtConjunto.TabIndex = 5;
            this.txtConjunto.TabStop = false;
            this.txtConjunto.EditValueChanged += new System.EventHandler(this.txtConjunto_EditValueChanged);
            this.txtConjunto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtConjunto_KeyPress);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(550, 19);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 30);
            this.btnSalir.TabIndex = 23;
            this.btnSalir.TabStop = false;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnAgregar
            // 
            this.btnAgregar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAgregar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAgregar.ImageOptions.Image")));
            this.btnAgregar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnAgregar.Location = new System.Drawing.Point(13, 311);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(79, 30);
            this.btnAgregar.TabIndex = 22;
            this.btnAgregar.Text = "Agregar";
            this.btnAgregar.ToolTip = "Guardar";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(13, 59);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(26, 13);
            this.labelControl3.TabIndex = 34;
            this.labelControl3.Text = "Item:";
            // 
            // labelControl10
            // 
            this.labelControl10.Location = new System.Drawing.Point(111, 109);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Size = new System.Drawing.Size(12, 13);
            this.labelControl10.TabIndex = 37;
            this.labelControl10.Text = "N°";
            // 
            // labelControl9
            // 
            this.labelControl9.Location = new System.Drawing.Point(174, 109);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(12, 13);
            this.labelControl9.TabIndex = 37;
            this.labelControl9.Text = "de";
            // 
            // txtTotalCuotas
            // 
            this.txtTotalCuotas.Enabled = false;
            this.txtTotalCuotas.EnterMoveNextControl = true;
            this.txtTotalCuotas.Location = new System.Drawing.Point(192, 106);
            this.txtTotalCuotas.Name = "txtTotalCuotas";
            this.txtTotalCuotas.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtTotalCuotas_Properties_BeforeShowMenu);
            this.txtTotalCuotas.Size = new System.Drawing.Size(39, 20);
            this.txtTotalCuotas.TabIndex = 14;
            this.txtTotalCuotas.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textEdit3_KeyPress);
            // 
            // txtNumCuota
            // 
            this.txtNumCuota.Enabled = false;
            this.txtNumCuota.EnterMoveNextControl = true;
            this.txtNumCuota.Location = new System.Drawing.Point(129, 106);
            this.txtNumCuota.Name = "txtNumCuota";
            this.txtNumCuota.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtNumCuota_Properties_BeforeShowMenu);
            this.txtNumCuota.Size = new System.Drawing.Size(39, 20);
            this.txtNumCuota.TabIndex = 13;
            this.txtNumCuota.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textEdit2_KeyPress);
            // 
            // cbPesos
            // 
            this.cbPesos.Location = new System.Drawing.Point(132, 132);
            this.cbPesos.Name = "cbPesos";
            this.cbPesos.Properties.AllowFocused = false;
            this.cbPesos.Properties.Caption = "Pesos";
            this.cbPesos.Size = new System.Drawing.Size(55, 19);
            this.cbPesos.TabIndex = 17;
            this.cbPesos.TabStop = false;
            this.cbPesos.CheckedChanged += new System.EventHandler(this.cbPesos_CheckedChanged);
            // 
            // cbPorcentaje
            // 
            this.cbPorcentaje.Location = new System.Drawing.Point(91, 131);
            this.cbPorcentaje.Name = "cbPorcentaje";
            this.cbPorcentaje.Properties.AllowFocused = false;
            this.cbPorcentaje.Properties.Caption = "%";
            this.cbPorcentaje.Size = new System.Drawing.Size(30, 19);
            this.cbPorcentaje.TabIndex = 16;
            this.cbPorcentaje.TabStop = false;
            this.cbPorcentaje.CheckedChanged += new System.EventHandler(this.cbPorcentaje_CheckedChanged);
            // 
            // cbUf
            // 
            this.cbUf.Location = new System.Drawing.Point(53, 131);
            this.cbUf.Name = "cbUf";
            this.cbUf.Properties.AllowFocused = false;
            this.cbUf.Properties.Caption = "Uf";
            this.cbUf.Size = new System.Drawing.Size(39, 19);
            this.cbUf.TabIndex = 15;
            this.cbUf.TabStop = false;
            this.cbUf.CheckedChanged += new System.EventHandler(this.cbUf_CheckedChanged);
            // 
            // cbCuota
            // 
            this.cbCuota.Location = new System.Drawing.Point(53, 107);
            this.cbCuota.Name = "cbCuota";
            this.cbCuota.Properties.AllowFocused = false;
            this.cbCuota.Properties.Caption = "Cuota";
            this.cbCuota.Size = new System.Drawing.Size(55, 19);
            this.cbCuota.TabIndex = 12;
            this.cbCuota.TabStop = false;
            this.cbCuota.CheckedChanged += new System.EventHandler(this.cbCuota_CheckedChanged);
            // 
            // cbProporcional
            // 
            this.cbProporcional.Location = new System.Drawing.Point(53, 89);
            this.cbProporcional.Name = "cbProporcional";
            this.cbProporcional.Properties.AllowFocused = false;
            this.cbProporcional.Properties.Caption = "Proporcional";
            this.cbProporcional.Size = new System.Drawing.Size(87, 19);
            this.cbProporcional.TabIndex = 11;
            this.cbProporcional.TabStop = false;
            // 
            // txtValor
            // 
            this.txtValor.EditValue = "0";
            this.txtValor.EnterMoveNextControl = true;
            this.txtValor.Location = new System.Drawing.Point(53, 25);
            this.txtValor.Name = "txtValor";
            this.txtValor.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtValor_Properties_BeforeShowMenu);
            this.txtValor.Size = new System.Drawing.Size(100, 20);
            this.txtValor.TabIndex = 8;
            this.txtValor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textEdit1_KeyPress);
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(12, 29);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(37, 13);
            this.labelControl8.TabIndex = 42;
            this.labelControl8.Text = "Valor $:";
            // 
            // cbTope
            // 
            this.cbTope.Location = new System.Drawing.Point(53, 69);
            this.cbTope.Name = "cbTope";
            this.cbTope.Properties.AllowFocused = false;
            this.cbTope.Properties.Caption = "Con tope";
            this.cbTope.Size = new System.Drawing.Size(75, 19);
            this.cbTope.TabIndex = 10;
            this.cbTope.TabStop = false;
            // 
            // cbPermanente
            // 
            this.cbPermanente.Location = new System.Drawing.Point(53, 49);
            this.cbPermanente.Name = "cbPermanente";
            this.cbPermanente.Properties.AllowFocused = false;
            this.cbPermanente.Properties.Caption = "Permanente";
            this.cbPermanente.Size = new System.Drawing.Size(91, 19);
            this.cbPermanente.TabIndex = 9;
            this.cbPermanente.TabStop = false;
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(233, 56);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Properties.AllowFocused = false;
            this.txtDescripcion.Properties.ReadOnly = true;
            this.txtDescripcion.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtDescripcion_Properties_BeforeShowMenu);
            this.txtDescripcion.Size = new System.Drawing.Size(247, 20);
            this.txtDescripcion.TabIndex = 3;
            this.txtDescripcion.TabStop = false;
            this.txtDescripcion.EditValueChanged += new System.EventHandler(this.txtDescripcion_EditValueChanged);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(13, 36);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(40, 13);
            this.labelControl4.TabIndex = 37;
            this.labelControl4.Text = "Periodo:";
            // 
            // cbTodos
            // 
            this.cbTodos.EditValue = true;
            this.cbTodos.Location = new System.Drawing.Point(73, 79);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.Caption = "Todos los trabajadores del periodo";
            this.cbTodos.Size = new System.Drawing.Size(199, 19);
            this.cbTodos.TabIndex = 4;
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // txtTipo
            // 
            this.txtTipo.Location = new System.Drawing.Point(67, 68);
            this.txtTipo.Name = "txtTipo";
            this.txtTipo.Properties.AllowFocused = false;
            this.txtTipo.Properties.ReadOnly = true;
            this.txtTipo.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtTipo_Properties_BeforeShowMenu);
            this.txtTipo.Size = new System.Drawing.Size(156, 20);
            this.txtTipo.TabIndex = 20;
            this.txtTipo.TabStop = false;
            // 
            // txtItem
            // 
            this.txtItem.EnterMoveNextControl = true;
            this.txtItem.Location = new System.Drawing.Point(73, 56);
            this.txtItem.Name = "txtItem";
            this.txtItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtItem.Properties.PopupSizeable = false;
            this.txtItem.Size = new System.Drawing.Size(155, 20);
            this.txtItem.TabIndex = 2;
            this.txtItem.EditValueChanged += new System.EventHandler(this.txtItem_EditValueChanged);
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(38, 70);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(24, 13);
            this.labelControl7.TabIndex = 40;
            this.labelControl7.Text = "Tipo:";
            // 
            // txtOrden
            // 
            this.txtOrden.Location = new System.Drawing.Point(67, 46);
            this.txtOrden.Name = "txtOrden";
            this.txtOrden.Properties.AllowFocused = false;
            this.txtOrden.Properties.ReadOnly = true;
            this.txtOrden.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtOrden_Properties_BeforeShowMenu);
            this.txtOrden.Size = new System.Drawing.Size(75, 20);
            this.txtOrden.TabIndex = 19;
            this.txtOrden.TabStop = false;
            // 
            // txtPeriodo
            // 
            this.txtPeriodo.EnterMoveNextControl = true;
            this.txtPeriodo.Location = new System.Drawing.Point(73, 33);
            this.txtPeriodo.Name = "txtPeriodo";
            this.txtPeriodo.Properties.AllowFocused = false;
            this.txtPeriodo.Properties.ReadOnly = true;
            this.txtPeriodo.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtPeriodo_Properties_BeforeShowMenu);
            this.txtPeriodo.Size = new System.Drawing.Size(79, 20);
            this.txtPeriodo.TabIndex = 1;
            this.txtPeriodo.TabStop = false;
            this.txtPeriodo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPeriodo_KeyPress);
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(28, 49);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(34, 13);
            this.labelControl6.TabIndex = 40;
            this.labelControl6.Text = "Orden:";
            // 
            // lblMensaje
            // 
            this.lblMensaje.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblMensaje.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.lblMensaje.Appearance.Options.UseFont = true;
            this.lblMensaje.Appearance.Options.UseForeColor = true;
            this.lblMensaje.Location = new System.Drawing.Point(14, 347);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(57, 16);
            this.lblMensaje.TabIndex = 36;
            this.lblMensaje.Text = "message";
            this.lblMensaje.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.lblMensaje);
            this.groupBox1.Controls.Add(this.btnConjunto);
            this.groupBox1.Controls.Add(this.btnAgregar);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.txtConjunto);
            this.groupBox1.Controls.Add(this.txtItem);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Controls.Add(this.btnSalir);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Controls.Add(this.cbTodos);
            this.groupBox1.Controls.Add(this.txtPeriodo);
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.txtDescripcion);
            this.groupBox1.Location = new System.Drawing.Point(16, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(616, 373);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Formulario";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ComboFormula);
            this.groupBox3.Controls.Add(this.labelControl1);
            this.groupBox3.Controls.Add(this.labelControl7);
            this.groupBox3.Controls.Add(this.labelControl6);
            this.groupBox3.Controls.Add(this.txtOrden);
            this.groupBox3.Controls.Add(this.txtTipo);
            this.groupBox3.Location = new System.Drawing.Point(304, 139);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(306, 193);
            this.groupBox3.TabIndex = 48;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Propiedades";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // ComboFormula
            // 
            this.ComboFormula.Location = new System.Drawing.Point(67, 23);
            this.ComboFormula.Name = "ComboFormula";
            this.ComboFormula.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ComboFormula.Size = new System.Drawing.Size(211, 20);
            this.ComboFormula.TabIndex = 41;
            // 
            // btnConjunto
            // 
            this.btnConjunto.AllowFocus = false;
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(137, 99);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(26, 21);
            this.btnConjunto.TabIndex = 6;
            this.btnConjunto.TabStop = false;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtValor);
            this.groupBox2.Controls.Add(this.labelControl10);
            this.groupBox2.Controls.Add(this.cbPermanente);
            this.groupBox2.Controls.Add(this.txtNumCuota);
            this.groupBox2.Controls.Add(this.cbCuota);
            this.groupBox2.Controls.Add(this.labelControl8);
            this.groupBox2.Controls.Add(this.txtTotalCuotas);
            this.groupBox2.Controls.Add(this.cbPesos);
            this.groupBox2.Controls.Add(this.cbTope);
            this.groupBox2.Controls.Add(this.cbProporcional);
            this.groupBox2.Controls.Add(this.cbUf);
            this.groupBox2.Controls.Add(this.cbPorcentaje);
            this.groupBox2.Controls.Add(this.labelControl9);
            this.groupBox2.Location = new System.Drawing.Point(13, 136);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(279, 169);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Propiedades";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(19, 26);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(42, 13);
            this.labelControl1.TabIndex = 40;
            this.labelControl1.Text = "Formula:";
            // 
            // frmAgregarItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 396);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAgregarItems";
            this.Text = "Agregar items";
            this.Load += new System.EventHandler(this.frmAgregarItems_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalCuotas.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumCuota.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPesos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPorcentaje.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbUf.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbCuota.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbProporcional.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTope.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPermanente.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescripcion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTipo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItem.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrden.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo.Properties)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ComboFormula.Properties)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.TextEdit txtConjunto;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnAgregar;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LookUpEdit txtItem;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtPeriodo;
        private DevExpress.XtraEditors.LabelControl lblMensaje;
        private DevExpress.XtraEditors.TextEdit txtDescripcion;
        private DevExpress.XtraEditors.TextEdit txtOrden;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.CheckEdit cbProporcional;
        private DevExpress.XtraEditors.TextEdit txtValor;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.CheckEdit cbTope;
        private DevExpress.XtraEditors.CheckEdit cbPermanente;
        private DevExpress.XtraEditors.TextEdit txtTipo;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.TextEdit txtTotalCuotas;
        private DevExpress.XtraEditors.TextEdit txtNumCuota;
        private DevExpress.XtraEditors.CheckEdit cbCuota;
        private DevExpress.XtraEditors.LabelControl labelControl10;
        private DevExpress.XtraEditors.CheckEdit cbPesos;
        private DevExpress.XtraEditors.CheckEdit cbPorcentaje;
        private DevExpress.XtraEditors.CheckEdit cbUf;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.LookUpEdit ComboFormula;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}