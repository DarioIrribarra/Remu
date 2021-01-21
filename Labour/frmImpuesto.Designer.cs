namespace Labour
{
    partial class frmImpuesto
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImpuesto));
            this.separador1 = new DevExpress.XtraEditors.SeparatorControl();
            this.btnEliminar = new DevExpress.XtraEditors.SimpleButton();
            this.btnNuevo = new DevExpress.XtraEditors.SimpleButton();
            this.btnGuardar = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl12 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl11 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
            this.lblError = new DevExpress.XtraEditors.LabelControl();
            this.txtdato2 = new DevExpress.XtraEditors.TextEdit();
            this.txtdato1 = new DevExpress.XtraEditors.TextEdit();
            this.txtHasta = new DevExpress.XtraEditors.TextEdit();
            this.labelControl14 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl13 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.txtInicio = new DevExpress.XtraEditors.TextEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.txtRebaja = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txtFactor = new DevExpress.XtraEditors.TextEdit();
            this.txtTope = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtTramo = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtIdImpuesto = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.GridImpuesto = new DevExpress.XtraGrid.GridControl();
            this.viewImpuesto = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.separador1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtdato2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtdato1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHasta.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInicio.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRebaja.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFactor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTope.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTramo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIdImpuesto.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridImpuesto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewImpuesto)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // separador1
            // 
            this.separador1.Location = new System.Drawing.Point(17, 48);
            this.separador1.Name = "separador1";
            this.separador1.Size = new System.Drawing.Size(1306, 23);
            this.separador1.TabIndex = 24;
            // 
            // btnEliminar
            // 
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminar.ImageOptions.Image")));
            this.btnEliminar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnEliminar.Location = new System.Drawing.Point(519, 12);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(70, 30);
            this.btnEliminar.TabIndex = 23;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.ToolTip = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnNuevo
            // 
            this.btnNuevo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNuevo.ImageOptions.Image")));
            this.btnNuevo.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnNuevo.Location = new System.Drawing.Point(369, 12);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(70, 30);
            this.btnNuevo.TabIndex = 22;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.ToolTip = "Nuevo registro";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.ImageOptions.Image")));
            this.btnGuardar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGuardar.Location = new System.Drawing.Point(443, 12);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(70, 30);
            this.btnGuardar.TabIndex = 21;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.ToolTip = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // labelControl12
            // 
            this.labelControl12.Location = new System.Drawing.Point(160, 116);
            this.labelControl12.Name = "labelControl12";
            this.labelControl12.Size = new System.Drawing.Size(29, 13);
            this.labelControl12.TabIndex = 40;
            this.labelControl12.Text = "(UTM)";
            // 
            // labelControl11
            // 
            this.labelControl11.Location = new System.Drawing.Point(160, 94);
            this.labelControl11.Name = "labelControl11";
            this.labelControl11.Size = new System.Drawing.Size(19, 13);
            this.labelControl11.TabIndex = 39;
            this.labelControl11.Text = "(%)";
            // 
            // labelControl10
            // 
            this.labelControl10.Location = new System.Drawing.Point(160, 69);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Size = new System.Drawing.Size(29, 13);
            this.labelControl10.TabIndex = 38;
            this.labelControl10.Text = "(UTM)";
            // 
            // lblError
            // 
            this.lblError.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblError.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblError.Appearance.Options.UseFont = true;
            this.lblError.Appearance.Options.UseForeColor = true;
            this.lblError.Location = new System.Drawing.Point(92, 226);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(50, 13);
            this.lblError.TabIndex = 29;
            this.lblError.Text = "Message";
            // 
            // txtdato2
            // 
            this.txtdato2.EnterMoveNextControl = true;
            this.txtdato2.Location = new System.Drawing.Point(92, 202);
            this.txtdato2.Name = "txtdato2";
            this.txtdato2.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtHasta_Properties_BeforeShowMenu);
            this.txtdato2.Size = new System.Drawing.Size(112, 20);
            this.txtdato2.TabIndex = 9;
            this.txtdato2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtdato2_KeyPress);
            // 
            // txtdato1
            // 
            this.txtdato1.EnterMoveNextControl = true;
            this.txtdato1.Location = new System.Drawing.Point(92, 180);
            this.txtdato1.Name = "txtdato1";
            this.txtdato1.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtdato1.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtHasta_Properties_BeforeShowMenu);
            this.txtdato1.Size = new System.Drawing.Size(38, 20);
            this.txtdato1.TabIndex = 8;
            this.txtdato1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtdato1_KeyPress);
            // 
            // txtHasta
            // 
            this.txtHasta.EnterMoveNextControl = true;
            this.txtHasta.Location = new System.Drawing.Point(92, 159);
            this.txtHasta.Name = "txtHasta";
            this.txtHasta.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtHasta_Properties_BeforeShowMenu);
            this.txtHasta.Size = new System.Drawing.Size(63, 20);
            this.txtHasta.TabIndex = 7;
            this.txtHasta.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtHasta_KeyPress);
            // 
            // labelControl14
            // 
            this.labelControl14.Location = new System.Drawing.Point(43, 209);
            this.labelControl14.Name = "labelControl14";
            this.labelControl14.Size = new System.Drawing.Size(38, 13);
            this.labelControl14.TabIndex = 37;
            this.labelControl14.Text = "dato02:";
            // 
            // labelControl13
            // 
            this.labelControl13.Location = new System.Drawing.Point(43, 187);
            this.labelControl13.Name = "labelControl13";
            this.labelControl13.Size = new System.Drawing.Size(38, 13);
            this.labelControl13.TabIndex = 37;
            this.labelControl13.Text = "dato01:";
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(49, 163);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(32, 13);
            this.labelControl7.TabIndex = 37;
            this.labelControl7.Text = "Hasta:";
            // 
            // txtInicio
            // 
            this.txtInicio.EnterMoveNextControl = true;
            this.txtInicio.Location = new System.Drawing.Point(92, 136);
            this.txtInicio.Name = "txtInicio";
            this.txtInicio.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtInicio_Properties_BeforeShowMenu);
            this.txtInicio.Size = new System.Drawing.Size(63, 20);
            this.txtInicio.TabIndex = 6;
            this.txtInicio.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInicio_KeyPress);
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(52, 141);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(29, 13);
            this.labelControl6.TabIndex = 35;
            this.labelControl6.Text = "Inicio:";
            // 
            // txtRebaja
            // 
            this.txtRebaja.EnterMoveNextControl = true;
            this.txtRebaja.Location = new System.Drawing.Point(92, 113);
            this.txtRebaja.Name = "txtRebaja";
            this.txtRebaja.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtRebaja_Properties_BeforeShowMenu);
            this.txtRebaja.Size = new System.Drawing.Size(63, 20);
            this.txtRebaja.TabIndex = 5;
            this.txtRebaja.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtRebaja_KeyDown);
            this.txtRebaja.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRebaja_KeyPress);
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(43, 118);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(38, 13);
            this.labelControl5.TabIndex = 33;
            this.labelControl5.Text = "Rebaja:";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(46, 95);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(35, 13);
            this.labelControl4.TabIndex = 32;
            this.labelControl4.Text = "Factor:";
            // 
            // txtFactor
            // 
            this.txtFactor.EnterMoveNextControl = true;
            this.txtFactor.Location = new System.Drawing.Point(92, 90);
            this.txtFactor.Name = "txtFactor";
            this.txtFactor.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtFactor_Properties_BeforeShowMenu);
            this.txtFactor.Size = new System.Drawing.Size(63, 20);
            this.txtFactor.TabIndex = 4;
            this.txtFactor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFactor_KeyDown);
            this.txtFactor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFactor_KeyPress);
            // 
            // txtTope
            // 
            this.txtTope.EnterMoveNextControl = true;
            this.txtTope.Location = new System.Drawing.Point(92, 66);
            this.txtTope.Name = "txtTope";
            this.txtTope.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtTope_Properties_BeforeShowMenu);
            this.txtTope.Size = new System.Drawing.Size(63, 20);
            this.txtTope.TabIndex = 3;
            this.txtTope.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTope_KeyDown);
            this.txtTope.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTope_KeyPress);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(53, 72);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(28, 13);
            this.labelControl3.TabIndex = 29;
            this.labelControl3.Text = "Tope:";
            // 
            // txtTramo
            // 
            this.txtTramo.EnterMoveNextControl = true;
            this.txtTramo.Location = new System.Drawing.Point(92, 43);
            this.txtTramo.Name = "txtTramo";
            this.txtTramo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtTramo.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtTramo_Properties_BeforeShowMenu);
            this.txtTramo.Size = new System.Drawing.Size(130, 20);
            this.txtTramo.TabIndex = 2;
            this.txtTramo.EditValueChanged += new System.EventHandler(this.txtTramo_EditValueChanged);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(47, 48);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(34, 13);
            this.labelControl2.TabIndex = 27;
            this.labelControl2.Text = "Tramo:";
            // 
            // txtIdImpuesto
            // 
            this.txtIdImpuesto.EnterMoveNextControl = true;
            this.txtIdImpuesto.Location = new System.Drawing.Point(92, 20);
            this.txtIdImpuesto.Name = "txtIdImpuesto";
            this.txtIdImpuesto.Size = new System.Drawing.Size(63, 20);
            this.txtIdImpuesto.TabIndex = 1;
            this.txtIdImpuesto.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIdImpuesto_KeyDown);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(19, 25);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(62, 13);
            this.labelControl1.TabIndex = 26;
            this.labelControl1.Text = "Id Impuesto:";
            // 
            // GridImpuesto
            // 
            this.GridImpuesto.Location = new System.Drawing.Point(19, 25);
            this.GridImpuesto.MainView = this.viewImpuesto;
            this.GridImpuesto.Name = "GridImpuesto";
            this.GridImpuesto.Size = new System.Drawing.Size(698, 327);
            this.GridImpuesto.TabIndex = 28;
            this.GridImpuesto.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewImpuesto});
            this.GridImpuesto.Click += new System.EventHandler(this.GridImpuesto_Click);
            this.GridImpuesto.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GridImpuesto_KeyUp);
            // 
            // viewImpuesto
            // 
            this.viewImpuesto.GridControl = this.GridImpuesto;
            this.viewImpuesto.Name = "viewImpuesto";
            this.viewImpuesto.OptionsView.ShowGroupPanel = false;
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(973, 12);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(36, 30);
            this.btnSalir.TabIndex = 36;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelControl12);
            this.groupBox1.Controls.Add(this.txtIdImpuesto);
            this.groupBox1.Controls.Add(this.labelControl11);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Controls.Add(this.labelControl10);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Controls.Add(this.lblError);
            this.groupBox1.Controls.Add(this.txtTramo);
            this.groupBox1.Controls.Add(this.txtdato2);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Controls.Add(this.txtdato1);
            this.groupBox1.Controls.Add(this.txtTope);
            this.groupBox1.Controls.Add(this.txtHasta);
            this.groupBox1.Controls.Add(this.txtFactor);
            this.groupBox1.Controls.Add(this.labelControl14);
            this.groupBox1.Controls.Add(this.labelControl4);
            this.groupBox1.Controls.Add(this.labelControl13);
            this.groupBox1.Controls.Add(this.labelControl5);
            this.groupBox1.Controls.Add(this.labelControl7);
            this.groupBox1.Controls.Add(this.txtRebaja);
            this.groupBox1.Controls.Add(this.txtInicio);
            this.groupBox1.Controls.Add(this.labelControl6);
            this.groupBox1.Location = new System.Drawing.Point(279, 86);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(730, 251);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Impuesto";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.GridImpuesto);
            this.groupBox2.Location = new System.Drawing.Point(279, 352);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(730, 358);
            this.groupBox2.TabIndex = 38;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Registros actuales";
            // 
            // frmImpuesto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1344, 722);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.separador1);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnNuevo);
            this.Controls.Add(this.btnGuardar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmImpuesto";
            this.Text = "Impuesto";
            this.Load += new System.EventHandler(this.frmImpuesto_Load);
            ((System.ComponentModel.ISupportInitialize)(this.separador1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtdato2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtdato1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHasta.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInicio.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRebaja.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFactor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTope.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTramo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIdImpuesto.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridImpuesto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewImpuesto)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SeparatorControl separador1;
        private DevExpress.XtraEditors.SimpleButton btnEliminar;
        private DevExpress.XtraEditors.SimpleButton btnNuevo;
        private DevExpress.XtraEditors.SimpleButton btnGuardar;
        private DevExpress.XtraEditors.TextEdit txtHasta;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.TextEdit txtInicio;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.TextEdit txtRebaja;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtFactor;
        private DevExpress.XtraEditors.TextEdit txtTope;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LookUpEdit txtTramo;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtIdImpuesto;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraGrid.GridControl GridImpuesto;
        private DevExpress.XtraGrid.Views.Grid.GridView viewImpuesto;
        private DevExpress.XtraEditors.LabelControl lblError;
        private DevExpress.XtraEditors.LabelControl labelControl12;
        private DevExpress.XtraEditors.LabelControl labelControl11;
        private DevExpress.XtraEditors.LabelControl labelControl10;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.TextEdit txtdato2;
        private DevExpress.XtraEditors.TextEdit txtdato1;
        private DevExpress.XtraEditors.LabelControl labelControl14;
        private DevExpress.XtraEditors.LabelControl labelControl13;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}