namespace Labour
{
    partial class frmCuentaContable
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCuentaContable));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtCodigo = new DevExpress.XtraEditors.TextEdit();
            this.txtDescripcion = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.btnNuevo = new DevExpress.XtraEditors.SimpleButton();
            this.btnEliminar = new DevExpress.XtraEditors.SimpleButton();
            this.btnGuardar = new DevExpress.XtraEditors.SimpleButton();
            this.gridMaestro = new DevExpress.XtraGrid.GridControl();
            this.viewMaestro = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.groupCuentas = new System.Windows.Forms.GroupBox();
            this.txtAgrupaCombo = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.lblMessage = new DevExpress.XtraEditors.LabelControl();
            this.groupEsquema = new System.Windows.Forms.GroupBox();
            this.gridEsquema = new DevExpress.XtraGrid.GridControl();
            this.viewEsquema = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtSeparador = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.txtFormato = new DevExpress.XtraEditors.LookUpEdit();
            this.btnGuardarEsquema = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.btnEliminarEsquema = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtDescEsquema = new DevExpress.XtraEditors.TextEdit();
            this.btnNuevoEsquema = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.txtCodEsquema = new DevExpress.XtraEditors.TextEdit();
            this.txtColEsquema = new DevExpress.XtraEditors.LookUpEdit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescripcion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMaestro)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewMaestro)).BeginInit();
            this.groupCuentas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAgrupaCombo.Properties)).BeginInit();
            this.groupEsquema.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridEsquema)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewEsquema)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSeparador.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFormato.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescEsquema.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodEsquema.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtColEsquema.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(28, 31);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(37, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Codigo:";
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(72, 28);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(51, 20);
            this.txtCodigo.TabIndex = 1;
            this.txtCodigo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCodigo_KeyPress);
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(72, 50);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(336, 20);
            this.txtDescripcion.TabIndex = 2;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(7, 53);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(58, 13);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Descripción:";
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSalir.Location = new System.Drawing.Point(482, 14);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 30);
            this.btnSalir.TabIndex = 115;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnNuevo
            // 
            this.btnNuevo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNuevo.ImageOptions.Image")));
            this.btnNuevo.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnNuevo.Location = new System.Drawing.Point(14, 104);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(84, 30);
            this.btnNuevo.TabIndex = 116;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.ToolTip = "Nuevo registro";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminar.ImageOptions.Image")));
            this.btnEliminar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnEliminar.Location = new System.Drawing.Point(194, 104);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(48, 30);
            this.btnEliminar.TabIndex = 118;
            this.btnEliminar.ToolTip = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.ImageOptions.Image")));
            this.btnGuardar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGuardar.Location = new System.Drawing.Point(101, 104);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(88, 30);
            this.btnGuardar.TabIndex = 117;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.ToolTip = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // gridMaestro
            // 
            this.gridMaestro.Location = new System.Drawing.Point(11, 186);
            this.gridMaestro.MainView = this.viewMaestro;
            this.gridMaestro.Name = "gridMaestro";
            this.gridMaestro.Size = new System.Drawing.Size(470, 268);
            this.gridMaestro.TabIndex = 119;
            this.gridMaestro.TabStop = false;
            this.gridMaestro.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewMaestro});
            this.gridMaestro.Click += new System.EventHandler(this.gridMaestro_Click);
            this.gridMaestro.DoubleClick += new System.EventHandler(this.gridMaestro_DoubleClick);
            this.gridMaestro.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridMaestro_KeyUp);
            // 
            // viewMaestro
            // 
            this.viewMaestro.GridControl = this.gridMaestro;
            this.viewMaestro.Name = "viewMaestro";
            this.viewMaestro.OptionsView.ShowGroupPanel = false;
            this.viewMaestro.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.viewMaestro_PopupMenuShowing);
            // 
            // groupCuentas
            // 
            this.groupCuentas.Controls.Add(this.txtAgrupaCombo);
            this.groupCuentas.Controls.Add(this.labelControl4);
            this.groupCuentas.Controls.Add(this.lblMessage);
            this.groupCuentas.Controls.Add(this.btnGuardar);
            this.groupCuentas.Controls.Add(this.btnEliminar);
            this.groupCuentas.Controls.Add(this.gridMaestro);
            this.groupCuentas.Controls.Add(this.btnNuevo);
            this.groupCuentas.Controls.Add(this.btnSalir);
            this.groupCuentas.Controls.Add(this.txtCodigo);
            this.groupCuentas.Controls.Add(this.labelControl2);
            this.groupCuentas.Controls.Add(this.labelControl1);
            this.groupCuentas.Controls.Add(this.txtDescripcion);
            this.groupCuentas.Location = new System.Drawing.Point(553, 12);
            this.groupCuentas.Name = "groupCuentas";
            this.groupCuentas.Size = new System.Drawing.Size(531, 469);
            this.groupCuentas.TabIndex = 121;
            this.groupCuentas.TabStop = false;
            this.groupCuentas.Text = "Cuentas contables";
            this.groupCuentas.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // txtAgrupaCombo
            // 
            this.txtAgrupaCombo.Location = new System.Drawing.Point(73, 73);
            this.txtAgrupaCombo.Name = "txtAgrupaCombo";
            this.txtAgrupaCombo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtAgrupaCombo.Size = new System.Drawing.Size(50, 20);
            this.txtAgrupaCombo.TabIndex = 123;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(6, 76);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(59, 13);
            this.labelControl4.TabIndex = 122;
            this.labelControl4.Text = "Agrupa Rut:";
            // 
            // lblMessage
            // 
            this.lblMessage.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblMessage.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.lblMessage.Appearance.Options.UseFont = true;
            this.lblMessage.Appearance.Options.UseForeColor = true;
            this.lblMessage.Location = new System.Drawing.Point(15, 141);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(75, 13);
            this.lblMessage.TabIndex = 121;
            this.lblMessage.Text = "labelControl4";
            this.lblMessage.Visible = false;
            // 
            // groupEsquema
            // 
            this.groupEsquema.Controls.Add(this.gridEsquema);
            this.groupEsquema.Controls.Add(this.txtSeparador);
            this.groupEsquema.Controls.Add(this.labelControl9);
            this.groupEsquema.Controls.Add(this.txtFormato);
            this.groupEsquema.Controls.Add(this.btnGuardarEsquema);
            this.groupEsquema.Controls.Add(this.labelControl8);
            this.groupEsquema.Controls.Add(this.labelControl7);
            this.groupEsquema.Controls.Add(this.btnEliminarEsquema);
            this.groupEsquema.Controls.Add(this.labelControl5);
            this.groupEsquema.Controls.Add(this.txtDescEsquema);
            this.groupEsquema.Controls.Add(this.btnNuevoEsquema);
            this.groupEsquema.Controls.Add(this.labelControl6);
            this.groupEsquema.Controls.Add(this.txtCodEsquema);
            this.groupEsquema.Controls.Add(this.txtColEsquema);
            this.groupEsquema.Location = new System.Drawing.Point(12, 12);
            this.groupEsquema.Name = "groupEsquema";
            this.groupEsquema.Size = new System.Drawing.Size(519, 469);
            this.groupEsquema.TabIndex = 122;
            this.groupEsquema.TabStop = false;
            this.groupEsquema.Text = "Definicion de Esquema";
            // 
            // gridEsquema
            // 
            this.gridEsquema.Location = new System.Drawing.Point(22, 189);
            this.gridEsquema.MainView = this.viewEsquema;
            this.gridEsquema.Name = "gridEsquema";
            this.gridEsquema.Size = new System.Drawing.Size(470, 268);
            this.gridEsquema.TabIndex = 119;
            this.gridEsquema.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewEsquema});
            this.gridEsquema.Click += new System.EventHandler(this.gridEsquema_Click);
            // 
            // viewEsquema
            // 
            this.viewEsquema.GridControl = this.gridEsquema;
            this.viewEsquema.Name = "viewEsquema";
            this.viewEsquema.OptionsView.ShowGroupPanel = false;
            // 
            // txtSeparador
            // 
            this.txtSeparador.Location = new System.Drawing.Point(84, 117);
            this.txtSeparador.Name = "txtSeparador";
            this.txtSeparador.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtSeparador.Size = new System.Drawing.Size(100, 20);
            this.txtSeparador.TabIndex = 8;
            // 
            // labelControl9
            // 
            this.labelControl9.Location = new System.Drawing.Point(21, 118);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(54, 13);
            this.labelControl9.TabIndex = 7;
            this.labelControl9.Text = "Separador:";
            // 
            // txtFormato
            // 
            this.txtFormato.Location = new System.Drawing.Point(84, 96);
            this.txtFormato.Name = "txtFormato";
            this.txtFormato.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtFormato.Size = new System.Drawing.Size(100, 20);
            this.txtFormato.TabIndex = 6;
            // 
            // btnGuardarEsquema
            // 
            this.btnGuardarEsquema.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardarEsquema.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardarEsquema.ImageOptions.Image")));
            this.btnGuardarEsquema.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGuardarEsquema.Location = new System.Drawing.Point(170, 143);
            this.btnGuardarEsquema.Name = "btnGuardarEsquema";
            this.btnGuardarEsquema.Size = new System.Drawing.Size(81, 30);
            this.btnGuardarEsquema.TabIndex = 117;
            this.btnGuardarEsquema.Text = "Guardar";
            this.btnGuardarEsquema.ToolTip = "Guardar";
            this.btnGuardarEsquema.Click += new System.EventHandler(this.btnGuardarEsquema_Click);
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(31, 99);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(44, 13);
            this.labelControl8.TabIndex = 5;
            this.labelControl8.Text = "Formato:";
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(10, 77);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(65, 13);
            this.labelControl7.TabIndex = 4;
            this.labelControl7.Text = "N° Columnas:";
            // 
            // btnEliminarEsquema
            // 
            this.btnEliminarEsquema.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminarEsquema.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminarEsquema.ImageOptions.Image")));
            this.btnEliminarEsquema.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnEliminarEsquema.Location = new System.Drawing.Point(255, 143);
            this.btnEliminarEsquema.Name = "btnEliminarEsquema";
            this.btnEliminarEsquema.Size = new System.Drawing.Size(50, 30);
            this.btnEliminarEsquema.TabIndex = 118;
            this.btnEliminarEsquema.ToolTip = "Eliminar";
            this.btnEliminarEsquema.Click += new System.EventHandler(this.btnEliminarEsquema_Click);
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(38, 34);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(37, 13);
            this.labelControl5.TabIndex = 0;
            this.labelControl5.Text = "Codigo:";
            // 
            // txtDescEsquema
            // 
            this.txtDescEsquema.Location = new System.Drawing.Point(84, 53);
            this.txtDescEsquema.Name = "txtDescEsquema";
            this.txtDescEsquema.Size = new System.Drawing.Size(332, 20);
            this.txtDescEsquema.TabIndex = 2;
            // 
            // btnNuevoEsquema
            // 
            this.btnNuevoEsquema.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevoEsquema.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNuevoEsquema.ImageOptions.Image")));
            this.btnNuevoEsquema.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnNuevoEsquema.Location = new System.Drawing.Point(84, 143);
            this.btnNuevoEsquema.Name = "btnNuevoEsquema";
            this.btnNuevoEsquema.Size = new System.Drawing.Size(79, 30);
            this.btnNuevoEsquema.TabIndex = 116;
            this.btnNuevoEsquema.Text = "Nuevo";
            this.btnNuevoEsquema.ToolTip = "Nuevo registro";
            this.btnNuevoEsquema.Click += new System.EventHandler(this.btnNuevoEsquema_Click);
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(17, 56);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(58, 13);
            this.labelControl6.TabIndex = 3;
            this.labelControl6.Text = "Descripción:";
            // 
            // txtCodEsquema
            // 
            this.txtCodEsquema.Location = new System.Drawing.Point(84, 31);
            this.txtCodEsquema.Name = "txtCodEsquema";
            this.txtCodEsquema.Size = new System.Drawing.Size(51, 20);
            this.txtCodEsquema.TabIndex = 1;
            this.txtCodEsquema.EditValueChanged += new System.EventHandler(this.textEdit2_EditValueChanged);
            this.txtCodEsquema.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCodigo_KeyPress);
            // 
            // txtColEsquema
            // 
            this.txtColEsquema.Location = new System.Drawing.Point(84, 74);
            this.txtColEsquema.Name = "txtColEsquema";
            this.txtColEsquema.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtColEsquema.Properties.NullText = "";
            this.txtColEsquema.Size = new System.Drawing.Size(51, 20);
            this.txtColEsquema.TabIndex = 1;
            this.txtColEsquema.EditValueChanged += new System.EventHandler(this.textEdit2_EditValueChanged);
            this.txtColEsquema.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCodigo_KeyPress);
            // 
            // frmCuentaContable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1096, 498);
            this.Controls.Add(this.groupEsquema);
            this.Controls.Add(this.groupCuentas);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCuentaContable";
            this.Text = "Maestro contable";
            this.Load += new System.EventHandler(this.frmCuentaContable_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescripcion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridMaestro)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewMaestro)).EndInit();
            this.groupCuentas.ResumeLayout(false);
            this.groupCuentas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAgrupaCombo.Properties)).EndInit();
            this.groupEsquema.ResumeLayout(false);
            this.groupEsquema.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridEsquema)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewEsquema)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSeparador.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFormato.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescEsquema.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodEsquema.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtColEsquema.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtCodigo;
        private DevExpress.XtraEditors.TextEdit txtDescripcion;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnNuevo;
        private DevExpress.XtraEditors.SimpleButton btnEliminar;
        private DevExpress.XtraEditors.SimpleButton btnGuardar;
        private DevExpress.XtraGrid.GridControl gridMaestro;
        private DevExpress.XtraGrid.Views.Grid.GridView viewMaestro;
        private System.Windows.Forms.GroupBox groupCuentas;
        private DevExpress.XtraEditors.LabelControl lblMessage;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LookUpEdit txtAgrupaCombo;
        private System.Windows.Forms.GroupBox groupEsquema;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.TextEdit txtDescEsquema;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.TextEdit txtCodEsquema;
        private DevExpress.XtraGrid.GridControl gridEsquema;
        private DevExpress.XtraGrid.Views.Grid.GridView viewEsquema;
        private DevExpress.XtraEditors.LookUpEdit txtSeparador;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.LookUpEdit txtFormato;
        private DevExpress.XtraEditors.SimpleButton btnGuardarEsquema;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.SimpleButton btnEliminarEsquema;
        private DevExpress.XtraEditors.SimpleButton btnNuevoEsquema;
        private DevExpress.XtraEditors.LookUpEdit txtColEsquema;
    }
}