namespace Labour
{
    partial class frmitemTrabajador
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmitemTrabajador));
            this.cbPorcentaje = new DevExpress.XtraEditors.CheckEdit();
            this.cbUf = new DevExpress.XtraEditors.CheckEdit();
            this.cbPesos = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.txtTotalCuotas = new DevExpress.XtraEditors.TextEdit();
            this.txtNumCuotas = new DevExpress.XtraEditors.TextEdit();
            this.cbAplicaCuotas = new DevExpress.XtraEditors.CheckEdit();
            this.cbTope = new DevExpress.XtraEditors.CheckEdit();
            this.cbpermanente = new DevExpress.XtraEditors.CheckEdit();
            this.cbProporcional = new DevExpress.XtraEditors.CheckEdit();
            this.txtDesc = new DevExpress.XtraEditors.TextEdit();
            this.lblvalue = new DevExpress.XtraEditors.LabelControl();
            this.lblnumber = new DevExpress.XtraEditors.LabelControl();
            this.lblmsg = new DevExpress.XtraEditors.LabelControl();
            this.txtValor = new DevExpress.XtraEditors.TextEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.txtFormula = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.txtnumItem = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtItem = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.lblAnomes = new DevExpress.XtraEditors.LabelControl();
            this.btnEliminar = new DevExpress.XtraEditors.SimpleButton();
            this.btnNuevo = new DevExpress.XtraEditors.SimpleButton();
            this.btnGuardar = new DevExpress.XtraEditors.SimpleButton();
            this.gridHis = new DevExpress.XtraGrid.GridControl();
            this.viewHis = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnliquidacion = new DevExpress.XtraEditors.SimpleButton();
            this.lblAdulto = new DevExpress.XtraEditors.LabelControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbSuspendido = new DevExpress.XtraEditors.CheckEdit();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.groupItems = new System.Windows.Forms.GroupBox();
            this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
            this.lblNombre = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.cbPorcentaje.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbUf.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPesos.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalCuotas.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumCuotas.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAplicaCuotas.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTope.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbpermanente.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbProporcional.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDesc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFormula.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtnumItem.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItem.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridHis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewHis)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbSuspendido.Properties)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbPorcentaje
            // 
            this.cbPorcentaje.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbPorcentaje.Location = new System.Drawing.Point(167, 99);
            this.cbPorcentaje.Name = "cbPorcentaje";
            this.cbPorcentaje.Properties.Caption = "%";
            this.cbPorcentaje.Size = new System.Drawing.Size(41, 19);
            this.cbPorcentaje.TabIndex = 7;
            this.cbPorcentaje.CheckedChanged += new System.EventHandler(this.cbPorcentaje_CheckedChanged);
            // 
            // cbUf
            // 
            this.cbUf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbUf.Location = new System.Drawing.Point(209, 99);
            this.cbUf.Name = "cbUf";
            this.cbUf.Properties.Caption = "Uf";
            this.cbUf.Size = new System.Drawing.Size(41, 19);
            this.cbUf.TabIndex = 8;
            this.cbUf.CheckedChanged += new System.EventHandler(this.cbUf_CheckedChanged);
            // 
            // cbPesos
            // 
            this.cbPesos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbPesos.Location = new System.Drawing.Point(251, 99);
            this.cbPesos.Name = "cbPesos";
            this.cbPesos.Properties.Caption = "Pesos";
            this.cbPesos.Size = new System.Drawing.Size(48, 19);
            this.cbPesos.TabIndex = 9;
            this.cbPesos.CheckedChanged += new System.EventHandler(this.cbPesos_CheckedChanged);
            // 
            // labelControl9
            // 
            this.labelControl9.Location = new System.Drawing.Point(35, 51);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(48, 13);
            this.labelControl9.TabIndex = 35;
            this.labelControl9.Text = "Cuota N°:";
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(138, 51);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(12, 13);
            this.labelControl8.TabIndex = 34;
            this.labelControl8.Text = "de";
            // 
            // txtTotalCuotas
            // 
            this.txtTotalCuotas.Enabled = false;
            this.txtTotalCuotas.EnterMoveNextControl = true;
            this.txtTotalCuotas.Location = new System.Drawing.Point(156, 47);
            this.txtTotalCuotas.Name = "txtTotalCuotas";
            this.txtTotalCuotas.Properties.MaxLength = 3;
            this.txtTotalCuotas.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtTotalCuotas_Properties_BeforeShowMenu);
            this.txtTotalCuotas.Size = new System.Drawing.Size(41, 20);
            this.txtTotalCuotas.TabIndex = 15;
            this.txtTotalCuotas.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTotalCuotas_KeyPress);
            // 
            // txtNumCuotas
            // 
            this.txtNumCuotas.Enabled = false;
            this.txtNumCuotas.EnterMoveNextControl = true;
            this.txtNumCuotas.Location = new System.Drawing.Point(89, 47);
            this.txtNumCuotas.Name = "txtNumCuotas";
            this.txtNumCuotas.Properties.MaxLength = 3;
            this.txtNumCuotas.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtNumCuotas_Properties_BeforeShowMenu);
            this.txtNumCuotas.Size = new System.Drawing.Size(41, 20);
            this.txtNumCuotas.TabIndex = 14;
            this.txtNumCuotas.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumCuotas_KeyPress);
            // 
            // cbAplicaCuotas
            // 
            this.cbAplicaCuotas.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbAplicaCuotas.Location = new System.Drawing.Point(35, 25);
            this.cbAplicaCuotas.Name = "cbAplicaCuotas";
            this.cbAplicaCuotas.Properties.AllowFocused = false;
            this.cbAplicaCuotas.Properties.Caption = "Aplica Cuotas";
            this.cbAplicaCuotas.Size = new System.Drawing.Size(90, 19);
            this.cbAplicaCuotas.TabIndex = 13;
            this.cbAplicaCuotas.TabStop = false;
            this.cbAplicaCuotas.CheckedChanged += new System.EventHandler(this.cbAplicaCuotas_CheckedChanged);
            // 
            // cbTope
            // 
            this.cbTope.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbTope.EnterMoveNextControl = true;
            this.cbTope.Location = new System.Drawing.Point(15, 52);
            this.cbTope.Name = "cbTope";
            this.cbTope.Properties.AllowFocused = false;
            this.cbTope.Properties.Caption = "Tope";
            this.cbTope.Size = new System.Drawing.Size(83, 19);
            this.cbTope.TabIndex = 12;
            this.cbTope.TabStop = false;
            this.cbTope.CheckedChanged += new System.EventHandler(this.cbTope_CheckedChanged);
            // 
            // cbpermanente
            // 
            this.cbpermanente.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbpermanente.EnterMoveNextControl = true;
            this.cbpermanente.Location = new System.Drawing.Point(15, 33);
            this.cbpermanente.Name = "cbpermanente";
            this.cbpermanente.Properties.AllowFocused = false;
            this.cbpermanente.Properties.Caption = "Permanente";
            this.cbpermanente.Size = new System.Drawing.Size(79, 19);
            this.cbpermanente.TabIndex = 11;
            this.cbpermanente.TabStop = false;
            this.cbpermanente.CheckedChanged += new System.EventHandler(this.cbpermanente_CheckedChanged);
            // 
            // cbProporcional
            // 
            this.cbProporcional.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbProporcional.EnterMoveNextControl = true;
            this.cbProporcional.Location = new System.Drawing.Point(15, 16);
            this.cbProporcional.Name = "cbProporcional";
            this.cbProporcional.Properties.AllowFocused = false;
            this.cbProporcional.Properties.Caption = "Proporcional";
            this.cbProporcional.Size = new System.Drawing.Size(82, 19);
            this.cbProporcional.TabIndex = 10;
            this.cbProporcional.TabStop = false;
            this.cbProporcional.CheckedChanged += new System.EventHandler(this.cbProporcional_CheckedChanged);
            // 
            // txtDesc
            // 
            this.txtDesc.EnterMoveNextControl = true;
            this.txtDesc.Location = new System.Drawing.Point(328, 34);
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Properties.AllowFocused = false;
            this.txtDesc.Properties.ReadOnly = true;
            this.txtDesc.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtDesc_Properties_BeforeShowMenu);
            this.txtDesc.Size = new System.Drawing.Size(343, 20);
            this.txtDesc.TabIndex = 2;
            // 
            // lblvalue
            // 
            this.lblvalue.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.lblvalue.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblvalue.Appearance.Options.UseFont = true;
            this.lblvalue.Appearance.Options.UseForeColor = true;
            this.lblvalue.Location = new System.Drawing.Point(68, 121);
            this.lblvalue.Name = "lblvalue";
            this.lblvalue.Size = new System.Drawing.Size(53, 13);
            this.lblvalue.TabIndex = 28;
            this.lblvalue.Text = "message";
            this.lblvalue.Visible = false;
            // 
            // lblnumber
            // 
            this.lblnumber.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.lblnumber.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblnumber.Appearance.Options.UseFont = true;
            this.lblnumber.Appearance.Options.UseForeColor = true;
            this.lblnumber.Location = new System.Drawing.Point(108, 78);
            this.lblnumber.Name = "lblnumber";
            this.lblnumber.Size = new System.Drawing.Size(53, 13);
            this.lblnumber.TabIndex = 27;
            this.lblnumber.Text = "message";
            this.lblnumber.Visible = false;
            // 
            // lblmsg
            // 
            this.lblmsg.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblmsg.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblmsg.Appearance.Options.UseFont = true;
            this.lblmsg.Appearance.Options.UseForeColor = true;
            this.lblmsg.Location = new System.Drawing.Point(71, 15);
            this.lblmsg.Name = "lblmsg";
            this.lblmsg.Size = new System.Drawing.Size(51, 13);
            this.lblmsg.TabIndex = 26;
            this.lblmsg.Text = "message";
            this.lblmsg.Visible = false;
            // 
            // txtValor
            // 
            this.txtValor.EnterMoveNextControl = true;
            this.txtValor.Location = new System.Drawing.Point(68, 98);
            this.txtValor.Name = "txtValor";
            this.txtValor.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtValor_Properties_BeforeShowMenu);
            this.txtValor.Size = new System.Drawing.Size(93, 20);
            this.txtValor.TabIndex = 6;
            this.txtValor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtValor_KeyDown);
            this.txtValor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtValor_KeyPress);
            this.txtValor.Leave += new System.EventHandler(this.txtValor_Leave);
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(34, 102);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(28, 13);
            this.labelControl7.TabIndex = 9;
            this.labelControl7.Text = "Valor:";
            // 
            // txtFormula
            // 
            this.txtFormula.EnterMoveNextControl = true;
            this.txtFormula.Location = new System.Drawing.Point(68, 55);
            this.txtFormula.Name = "txtFormula";
            this.txtFormula.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtFormula.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtFormula_Properties_BeforeShowMenu);
            this.txtFormula.Size = new System.Drawing.Size(255, 20);
            this.txtFormula.TabIndex = 3;
            this.txtFormula.EditValueChanged += new System.EventHandler(this.txtFormula_EditValueChanged);
            this.txtFormula.DoubleClick += new System.EventHandler(this.txtFormula_DoubleClick);
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(20, 58);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(42, 13);
            this.labelControl6.TabIndex = 5;
            this.labelControl6.Text = "Formula:";
            // 
            // txtnumItem
            // 
            this.txtnumItem.EditValue = "1";
            this.txtnumItem.EnterMoveNextControl = true;
            this.txtnumItem.Location = new System.Drawing.Point(68, 76);
            this.txtnumItem.Name = "txtnumItem";
            this.txtnumItem.Properties.AllowFocused = false;
            this.txtnumItem.Properties.ReadOnly = true;
            this.txtnumItem.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtnumItem_Properties_BeforeShowMenu);
            this.txtnumItem.Size = new System.Drawing.Size(35, 20);
            this.txtnumItem.TabIndex = 5;
            this.txtnumItem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtnumItem_KeyDown);
            this.txtnumItem.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtnumItem_KeyPress);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(21, 79);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(41, 13);
            this.labelControl3.TabIndex = 7;
            this.labelControl3.Text = "N° Item:";
            // 
            // txtItem
            // 
            this.txtItem.EnterMoveNextControl = true;
            this.txtItem.Location = new System.Drawing.Point(68, 34);
            this.txtItem.Name = "txtItem";
            this.txtItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtItem.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtItem_Properties_BeforeShowMenu);
            this.txtItem.Size = new System.Drawing.Size(255, 20);
            this.txtItem.TabIndex = 1;
            this.txtItem.EditValueChanged += new System.EventHandler(this.txtItem_EditValueChanged);
            this.txtItem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtItem_KeyDown);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(36, 37);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(26, 13);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Item:";
            // 
            // lblAnomes
            // 
            this.lblAnomes.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblAnomes.Appearance.Options.UseFont = true;
            this.lblAnomes.Location = new System.Drawing.Point(552, 14);
            this.lblAnomes.Name = "lblAnomes";
            this.lblAnomes.Size = new System.Drawing.Size(61, 16);
            this.lblAnomes.TabIndex = 0;
            this.lblAnomes.Text = "PERIODO:";
            // 
            // btnEliminar
            // 
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminar.ImageOptions.Image")));
            this.btnEliminar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnEliminar.Location = new System.Drawing.Point(165, 300);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(70, 30);
            this.btnEliminar.TabIndex = 18;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.ToolTip = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnNuevo
            // 
            this.btnNuevo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNuevo.ImageOptions.Image")));
            this.btnNuevo.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnNuevo.Location = new System.Drawing.Point(15, 300);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(70, 30);
            this.btnNuevo.TabIndex = 16;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.ToolTip = "Nuevo registro";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.ImageOptions.Image")));
            this.btnGuardar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGuardar.Location = new System.Drawing.Point(89, 300);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(70, 30);
            this.btnGuardar.TabIndex = 17;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.ToolTip = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // gridHis
            // 
            this.gridHis.Location = new System.Drawing.Point(18, 20);
            this.gridHis.MainView = this.viewHis;
            this.gridHis.Name = "gridHis";
            this.gridHis.Size = new System.Drawing.Size(702, 346);
            this.gridHis.TabIndex = 20;
            this.gridHis.TabStop = false;
            this.gridHis.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewHis});
            this.gridHis.Click += new System.EventHandler(this.gridHis_Click);
            this.gridHis.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridHis_KeyUp);
            // 
            // viewHis
            // 
            this.viewHis.GridControl = this.gridHis;
            this.viewHis.Name = "viewHis";
            this.viewHis.OptionsView.ShowGroupPanel = false;
            this.viewHis.KeyDown += new System.Windows.Forms.KeyEventHandler(this.viewHis_KeyDown);
            // 
            // btnliquidacion
            // 
            this.btnliquidacion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnliquidacion.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnliquidacion.ImageOptions.Image")));
            this.btnliquidacion.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnliquidacion.Location = new System.Drawing.Point(241, 300);
            this.btnliquidacion.Name = "btnliquidacion";
            this.btnliquidacion.Size = new System.Drawing.Size(49, 30);
            this.btnliquidacion.TabIndex = 19;
            this.btnliquidacion.ToolTip = "Ver Liquidación";
            this.btnliquidacion.Click += new System.EventHandler(this.btnliquidacion_Click);
            // 
            // lblAdulto
            // 
            this.lblAdulto.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.lblAdulto.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblAdulto.Appearance.Options.UseFont = true;
            this.lblAdulto.Appearance.Options.UseForeColor = true;
            this.lblAdulto.Location = new System.Drawing.Point(16, 333);
            this.lblAdulto.Name = "lblAdulto";
            this.lblAdulto.Size = new System.Drawing.Size(53, 13);
            this.lblAdulto.TabIndex = 19;
            this.lblAdulto.Text = "Mensaje:";
            this.lblAdulto.Visible = false;
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSalir.ImageOptions.Image")));
            this.btnSalir.Location = new System.Drawing.Point(714, 4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(34, 30);
            this.btnSalir.TabIndex = 21;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbPorcentaje);
            this.groupBox1.Controls.Add(this.txtItem);
            this.groupBox1.Controls.Add(this.cbUf);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Controls.Add(this.cbPesos);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Controls.Add(this.txtnumItem);
            this.groupBox1.Controls.Add(this.labelControl6);
            this.groupBox1.Controls.Add(this.txtFormula);
            this.groupBox1.Controls.Add(this.labelControl7);
            this.groupBox1.Controls.Add(this.txtValor);
            this.groupBox1.Controls.Add(this.lblmsg);
            this.groupBox1.Controls.Add(this.lblnumber);
            this.groupBox1.Controls.Add(this.lblvalue);
            this.groupBox1.Controls.Add(this.txtDesc);
            this.groupBox1.Location = new System.Drawing.Point(12, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(741, 164);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbProporcional);
            this.groupBox3.Controls.Add(this.cbpermanente);
            this.groupBox3.Controls.Add(this.cbSuspendido);
            this.groupBox3.Controls.Add(this.cbTope);
            this.groupBox3.Location = new System.Drawing.Point(13, 204);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(328, 92);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Opciones";
            // 
            // cbSuspendido
            // 
            this.cbSuspendido.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbSuspendido.EnterMoveNextControl = true;
            this.cbSuspendido.Location = new System.Drawing.Point(15, 71);
            this.cbSuspendido.Name = "cbSuspendido";
            this.cbSuspendido.Properties.AllowFocused = false;
            this.cbSuspendido.Properties.Caption = "Suspender";
            this.cbSuspendido.Size = new System.Drawing.Size(83, 19);
            this.cbSuspendido.TabIndex = 12;
            this.cbSuspendido.TabStop = false;
            this.cbSuspendido.ToolTip = "Si seleccionas esta opción este item no se considerará dentro de la liquidacion d" +
    "e este periodo.";
            this.cbSuspendido.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Warning;
            this.cbSuspendido.CheckedChanged += new System.EventHandler(this.cbTope_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbAplicaCuotas);
            this.groupBox4.Controls.Add(this.labelControl9);
            this.groupBox4.Controls.Add(this.txtNumCuotas);
            this.groupBox4.Controls.Add(this.txtTotalCuotas);
            this.groupBox4.Controls.Add(this.labelControl1);
            this.groupBox4.Controls.Add(this.labelControl8);
            this.groupBox4.Location = new System.Drawing.Point(349, 204);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(404, 92);
            this.groupBox4.TabIndex = 24;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Opciones";
            this.groupBox4.Enter += new System.EventHandler(this.groupBox4_Enter);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(203, 50);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(34, 13);
            this.labelControl1.TabIndex = 34;
            this.labelControl1.Text = "Cuotas";
            // 
            // groupItems
            // 
            this.groupItems.Controls.Add(this.gridHis);
            this.groupItems.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupItems.Location = new System.Drawing.Point(12, 349);
            this.groupItems.Name = "groupItems";
            this.groupItems.Size = new System.Drawing.Size(741, 384);
            this.groupItems.TabIndex = 25;
            this.groupItems.TabStop = false;
            this.groupItems.Text = "Items";
            // 
            // toolTipController1
            // 
            this.toolTipController1.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
            // 
            // lblNombre
            // 
            this.lblNombre.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lblNombre.Appearance.Options.UseFont = true;
            this.lblNombre.Location = new System.Drawing.Point(16, 14);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(54, 16);
            this.lblNombre.TabIndex = 26;
            this.lblNombre.Text = "Nombre:";
            // 
            // frmitemTrabajador
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 739);
            this.ControlBox = false;
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.groupItems);
            this.Controls.Add(this.btnNuevo);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnliquidacion);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.lblAdulto);
            this.Controls.Add(this.lblAnomes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmitemTrabajador";
            this.Text = "Item trabajador";
            this.Load += new System.EventHandler(this.frmitemHis_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cbPorcentaje.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbUf.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbPesos.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalCuotas.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumCuotas.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAplicaCuotas.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTope.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbpermanente.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbProporcional.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDesc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFormula.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtnumItem.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItem.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridHis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewHis)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbSuspendido.Properties)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupItems.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.LabelControl lblAnomes;
        private DevExpress.XtraEditors.TextEdit txtValor;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.LookUpEdit txtFormula;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.TextEdit txtnumItem;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LookUpEdit txtItem;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton btnEliminar;
        private DevExpress.XtraEditors.SimpleButton btnNuevo;
        private DevExpress.XtraEditors.SimpleButton btnGuardar;
        private DevExpress.XtraGrid.GridControl gridHis;
        private DevExpress.XtraGrid.Views.Grid.GridView viewHis;
        private DevExpress.XtraEditors.LabelControl lblmsg;
        private DevExpress.XtraEditors.LabelControl lblvalue;
        private DevExpress.XtraEditors.LabelControl lblnumber;
        private DevExpress.XtraEditors.TextEdit txtDesc;
        private DevExpress.XtraEditors.SimpleButton btnliquidacion;
        private DevExpress.XtraEditors.CheckEdit cbProporcional;
        private DevExpress.XtraEditors.LabelControl lblAdulto;
        private DevExpress.XtraEditors.CheckEdit cbpermanente;
        private DevExpress.XtraEditors.CheckEdit cbTope;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.CheckEdit cbAplicaCuotas;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.TextEdit txtTotalCuotas;
        private DevExpress.XtraEditors.TextEdit txtNumCuotas;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.CheckEdit cbUf;
        private DevExpress.XtraEditors.CheckEdit cbPesos;
        private DevExpress.XtraEditors.CheckEdit cbPorcentaje;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.GroupBox groupItems;
        private DevExpress.Utils.ToolTipController toolTipController1;
        private DevExpress.XtraEditors.LabelControl lblNombre;
        private DevExpress.XtraEditors.CheckEdit cbSuspendido;
    }
}