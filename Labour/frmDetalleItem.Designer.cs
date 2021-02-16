namespace Labour
{
    partial class frmDetalleItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDetalleItem));
            this.gridDetalleItem = new DevExpress.XtraGrid.GridControl();
            this.viewDetalleCalculado = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblmensaje = new DevExpress.XtraEditors.LabelControl();
            this.cbShowOriginal = new DevExpress.XtraEditors.CheckEdit();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.btnImprimir = new DevExpress.XtraEditors.SimpleButton();
            this.btnCargaitem = new DevExpress.XtraEditors.SimpleButton();
            this.btnLimpiar = new DevExpress.XtraEditors.SimpleButton();
            this.btnGuardar = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtConjunto = new DevExpress.XtraEditors.TextEdit();
            this.txtcodigo = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.lblTotalOriginal = new DevExpress.XtraEditors.LabelControl();
            this.lblTotalCalculado = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtAgrupa = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.txtComboPeriodo = new DevExpress.XtraEditors.LookUpEdit();
            this.btnConjunto = new DevExpress.XtraEditors.SimpleButton();
            this.cbTodos = new DevExpress.XtraEditors.CheckEdit();
            this.btnExcel = new DevExpress.XtraEditors.SimpleButton();
            this.groupPeriodo = new System.Windows.Forms.GroupBox();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitFormRemuneraciones), true, true);
            this.btnEditarReporte = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridDetalleItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDetalleCalculado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbShowOriginal.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtcodigo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAgrupa.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).BeginInit();
            this.groupPeriodo.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridDetalleItem
            // 
            this.gridDetalleItem.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridDetalleItem.Location = new System.Drawing.Point(20, 70);
            this.gridDetalleItem.MainView = this.viewDetalleCalculado;
            this.gridDetalleItem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridDetalleItem.Name = "gridDetalleItem";
            this.gridDetalleItem.Size = new System.Drawing.Size(723, 313);
            this.gridDetalleItem.TabIndex = 13;
            this.gridDetalleItem.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewDetalleCalculado});
            // 
            // viewDetalleCalculado
            // 
            this.viewDetalleCalculado.DetailHeight = 431;
            this.viewDetalleCalculado.GridControl = this.gridDetalleItem;
            this.viewDetalleCalculado.Name = "viewDetalleCalculado";
            this.viewDetalleCalculado.OptionsView.ShowGroupPanel = false;
            // 
            // lblmensaje
            // 
            this.lblmensaje.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblmensaje.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblmensaje.Appearance.Options.UseFont = true;
            this.lblmensaje.Appearance.Options.UseForeColor = true;
            this.lblmensaje.Location = new System.Drawing.Point(135, 225);
            this.lblmensaje.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblmensaje.Name = "lblmensaje";
            this.lblmensaje.Size = new System.Drawing.Size(92, 17);
            this.lblmensaje.TabIndex = 13;
            this.lblmensaje.Text = "labelControl5";
            this.lblmensaje.Visible = false;
            // 
            // cbShowOriginal
            // 
            this.cbShowOriginal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbShowOriginal.Location = new System.Drawing.Point(131, 129);
            this.cbShowOriginal.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbShowOriginal.Name = "cbShowOriginal";
            this.cbShowOriginal.Properties.Caption = "Mostrar valor original item";
            this.cbShowOriginal.Size = new System.Drawing.Size(336, 20);
            this.cbShowOriginal.TabIndex = 7;
            this.cbShowOriginal.CheckedChanged += new System.EventHandler(this.cbShowOriginal_CheckedChanged);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(713, 20);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(49, 37);
            this.btnSalir.TabIndex = 11;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnImprimir
            // 
            this.btnImprimir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImprimir.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImprimir.ImageOptions.Image")));
            this.btnImprimir.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnImprimir.Location = new System.Drawing.Point(77, 25);
            this.btnImprimir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(49, 38);
            this.btnImprimir.TabIndex = 12;
            this.btnImprimir.TabStop = false;
            this.btnImprimir.ToolTip = "Generar Documento";
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // btnCargaitem
            // 
            this.btnCargaitem.AllowFocus = false;
            this.btnCargaitem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCargaitem.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCargaitem.ImageOptions.Image")));
            this.btnCargaitem.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnCargaitem.Location = new System.Drawing.Point(246, 25);
            this.btnCargaitem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCargaitem.Name = "btnCargaitem";
            this.btnCargaitem.Size = new System.Drawing.Size(34, 23);
            this.btnCargaitem.TabIndex = 2;
            this.btnCargaitem.ToolTip = "Buscar un item";
            this.btnCargaitem.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Question;
            this.btnCargaitem.Click += new System.EventHandler(this.btnCargaitem_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.AllowFocus = false;
            this.btnLimpiar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLimpiar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnLimpiar.ImageOptions.Image")));
            this.btnLimpiar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnLimpiar.Location = new System.Drawing.Point(222, 181);
            this.btnLimpiar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(36, 37);
            this.btnLimpiar.TabIndex = 10;
            this.btnLimpiar.ToolTip = "Limpiar Campos";
            this.btnLimpiar.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.ImageOptions.Image")));
            this.btnGuardar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGuardar.Location = new System.Drawing.Point(133, 181);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(82, 37);
            this.btnGuardar.TabIndex = 9;
            this.btnGuardar.Text = "Buscar";
            this.btnGuardar.ToolTip = "Iniciar Busqueda";
            this.btnGuardar.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(69, 107);
            this.labelControl4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(60, 16);
            this.labelControl4.TabIndex = 2;
            this.labelControl4.Text = "Condición:";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(80, 58);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(48, 16);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "Periodo:";
            // 
            // txtConjunto
            // 
            this.txtConjunto.Location = new System.Drawing.Point(131, 101);
            this.txtConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtConjunto.Name = "txtConjunto";
            this.txtConjunto.Properties.MaxLength = 6;
            this.txtConjunto.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.textEdit1_Properties_BeforeShowMenu);
            this.txtConjunto.Size = new System.Drawing.Size(89, 22);
            this.txtConjunto.TabIndex = 5;
            this.txtConjunto.EditValueChanged += new System.EventHandler(this.txtConjunto_EditValueChanged);
            this.txtConjunto.DoubleClick += new System.EventHandler(this.txtConjunto_DoubleClick);
            this.txtConjunto.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConjunto_KeyDown);
            // 
            // txtcodigo
            // 
            this.txtcodigo.EnterMoveNextControl = true;
            this.txtcodigo.Location = new System.Drawing.Point(131, 25);
            this.txtcodigo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtcodigo.Name = "txtcodigo";
            this.txtcodigo.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtcodigo.Properties.MaxLength = 15;
            this.txtcodigo.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtcodigo_Properties_BeforeShowMenu);
            this.txtcodigo.Size = new System.Drawing.Size(110, 22);
            this.txtcodigo.TabIndex = 1;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(55, 30);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(74, 16);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Codigo Item:";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(359, 6);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(147, 17);
            this.labelControl3.TabIndex = 8;
            this.labelControl3.Text = "TOTAL CALCULADO: ";
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.lblTotalOriginal);
            this.panelControl2.Controls.Add(this.lblTotalCalculado);
            this.panelControl2.Controls.Add(this.labelControl5);
            this.panelControl2.Controls.Add(this.labelControl3);
            this.panelControl2.Location = new System.Drawing.Point(22, 391);
            this.panelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(636, 34);
            this.panelControl2.TabIndex = 9;
            // 
            // lblTotalOriginal
            // 
            this.lblTotalOriginal.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTotalOriginal.Appearance.Options.UseFont = true;
            this.lblTotalOriginal.Location = new System.Drawing.Point(190, 6);
            this.lblTotalOriginal.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblTotalOriginal.Name = "lblTotalOriginal";
            this.lblTotalOriginal.Size = new System.Drawing.Size(47, 17);
            this.lblTotalOriginal.TabIndex = 8;
            this.lblTotalOriginal.Text = "VALUE";
            // 
            // lblTotalCalculado
            // 
            this.lblTotalCalculado.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTotalCalculado.Appearance.Options.UseFont = true;
            this.lblTotalCalculado.Location = new System.Drawing.Point(496, 6);
            this.lblTotalCalculado.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblTotalCalculado.Name = "lblTotalCalculado";
            this.lblTotalCalculado.Size = new System.Drawing.Size(47, 17);
            this.lblTotalCalculado.TabIndex = 8;
            this.lblTotalCalculado.Text = "VALUE";
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Location = new System.Drawing.Point(64, 6);
            this.labelControl5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(130, 17);
            this.labelControl5.TabIndex = 8;
            this.labelControl5.Text = "TOTAL ORIGINAL:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtAgrupa);
            this.groupBox1.Controls.Add(this.labelControl6);
            this.groupBox1.Controls.Add(this.txtComboPeriodo);
            this.groupBox1.Controls.Add(this.btnConjunto);
            this.groupBox1.Controls.Add(this.lblmensaje);
            this.groupBox1.Controls.Add(this.txtcodigo);
            this.groupBox1.Controls.Add(this.cbShowOriginal);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Controls.Add(this.btnSalir);
            this.groupBox1.Controls.Add(this.txtConjunto);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Controls.Add(this.cbTodos);
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.btnGuardar);
            this.groupBox1.Controls.Add(this.btnCargaitem);
            this.groupBox1.Controls.Add(this.btnLimpiar);
            this.groupBox1.Location = new System.Drawing.Point(7, 7);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(771, 257);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            // 
            // txtAgrupa
            // 
            this.txtAgrupa.Location = new System.Drawing.Point(131, 153);
            this.txtAgrupa.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtAgrupa.Name = "txtAgrupa";
            this.txtAgrupa.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtAgrupa.Size = new System.Drawing.Size(117, 22);
            this.txtAgrupa.TabIndex = 18;
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(54, 158);
            this.labelControl6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(69, 16);
            this.labelControl6.TabIndex = 17;
            this.labelControl6.Text = "Agrupa por:";
            // 
            // txtComboPeriodo
            // 
            this.txtComboPeriodo.Location = new System.Drawing.Point(131, 52);
            this.txtComboPeriodo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtComboPeriodo.Name = "txtComboPeriodo";
            this.txtComboPeriodo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtComboPeriodo.Size = new System.Drawing.Size(219, 22);
            this.txtComboPeriodo.TabIndex = 15;
            // 
            // btnConjunto
            // 
            this.btnConjunto.AllowFocus = false;
            this.btnConjunto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConjunto.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConjunto.ImageOptions.Image")));
            this.btnConjunto.Location = new System.Drawing.Point(229, 100);
            this.btnConjunto.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConjunto.Name = "btnConjunto";
            this.btnConjunto.Size = new System.Drawing.Size(30, 26);
            this.btnConjunto.TabIndex = 14;
            this.btnConjunto.Click += new System.EventHandler(this.btnConjunto_Click);
            // 
            // cbTodos
            // 
            this.cbTodos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbTodos.Location = new System.Drawing.Point(131, 79);
            this.cbTodos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTodos.Name = "cbTodos";
            this.cbTodos.Properties.AllowFocused = false;
            this.cbTodos.Properties.Caption = "Todos los registros del periodo";
            this.cbTodos.Size = new System.Drawing.Size(271, 20);
            this.cbTodos.TabIndex = 6;
            this.cbTodos.TabStop = false;
            this.cbTodos.ToolTip = "Usar periodo actual";
            this.cbTodos.CheckedChanged += new System.EventHandler(this.cbTodos_CheckedChanged);
            // 
            // btnExcel
            // 
            this.btnExcel.ImageOptions.Image = global::Labour.Properties.Resources.exporttoxls_32x32;
            this.btnExcel.Location = new System.Drawing.Point(23, 25);
            this.btnExcel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(49, 38);
            this.btnExcel.TabIndex = 16;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // groupPeriodo
            // 
            this.groupPeriodo.Controls.Add(this.btnEditarReporte);
            this.groupPeriodo.Controls.Add(this.gridDetalleItem);
            this.groupPeriodo.Controls.Add(this.panelControl2);
            this.groupPeriodo.Controls.Add(this.btnExcel);
            this.groupPeriodo.Controls.Add(this.btnImprimir);
            this.groupPeriodo.Location = new System.Drawing.Point(8, 272);
            this.groupPeriodo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupPeriodo.Name = "groupPeriodo";
            this.groupPeriodo.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupPeriodo.Size = new System.Drawing.Size(770, 443);
            this.groupPeriodo.TabIndex = 15;
            this.groupPeriodo.TabStop = false;
            this.groupPeriodo.Text = "Periodo Evaluado:";
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // btnEditarReporte
            // 
            this.btnEditarReporte.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditarReporte.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditarReporte.ImageOptions.Image")));
            this.btnEditarReporte.Location = new System.Drawing.Point(130, 25);
            this.btnEditarReporte.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEditarReporte.Name = "btnEditarReporte";
            this.btnEditarReporte.Size = new System.Drawing.Size(114, 37);
            this.btnEditarReporte.TabIndex = 25;
            this.btnEditarReporte.Text = "Editar\r\nReporte\r\n";
            this.btnEditarReporte.Click += new System.EventHandler(this.btnEditarReporte_Click);
            // 
            // frmDetalleItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 725);
            this.ControlBox = false;
            this.Controls.Add(this.groupPeriodo);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDetalleItem";
            this.Text = "Informacion Item";
            this.Load += new System.EventHandler(this.frmDetalleItem_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridDetalleItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDetalleCalculado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbShowOriginal.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConjunto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtcodigo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAgrupa.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtComboPeriodo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTodos.Properties)).EndInit();
            this.groupPeriodo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.TextEdit txtcodigo;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton btnCargaitem;
        private DevExpress.XtraEditors.SimpleButton btnGuardar;
        private DevExpress.XtraGrid.GridControl gridDetalleItem;
        private DevExpress.XtraGrid.Views.Grid.GridView viewDetalleCalculado;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.LabelControl lblTotalCalculado;
        private DevExpress.XtraEditors.SimpleButton btnLimpiar;
        private DevExpress.XtraEditors.SimpleButton btnImprimir;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtConjunto;
        private DevExpress.XtraEditors.CheckEdit cbShowOriginal;
        private DevExpress.XtraEditors.LabelControl lblmensaje;
        private DevExpress.XtraEditors.LabelControl lblTotalOriginal;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupPeriodo;
        private DevExpress.XtraEditors.CheckEdit cbTodos;
        private DevExpress.XtraEditors.SimpleButton btnConjunto;
        private DevExpress.XtraEditors.LookUpEdit txtComboPeriodo;
        private DevExpress.XtraEditors.SimpleButton btnExcel;
        private DevExpress.XtraEditors.LookUpEdit txtAgrupa;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.SimpleButton btnEditarReporte;
    }
}