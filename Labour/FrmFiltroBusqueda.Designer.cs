namespace Labour
{
    partial class FrmFiltroBusqueda
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFiltroBusqueda));
            this.lblmsg = new DevExpress.XtraEditors.LabelControl();
            this.txtdesc = new DevExpress.XtraEditors.TextEdit();
            this.txtcodigo = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtCondicion = new DevExpress.XtraEditors.MemoEdit();
            this.btnEliminar = new DevExpress.XtraEditors.SimpleButton();
            this.btnNuevo = new DevExpress.XtraEditors.SimpleButton();
            this.btnGuardar = new DevExpress.XtraEditors.SimpleButton();
            this.gridResultado = new DevExpress.XtraGrid.GridControl();
            this.viewResultado = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridExpresiones = new DevExpress.XtraGrid.GridControl();
            this.viewExpresiones = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtQuery = new DevExpress.XtraEditors.MemoEdit();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.btnSeleccionar = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnPrevisualizar = new DevExpress.XtraEditors.SimpleButton();
            this.lblCount = new DevExpress.XtraEditors.LabelControl();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtdesc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtcodigo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCondicion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridResultado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewResultado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridExpresiones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewExpresiones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQuery.Properties)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblmsg
            // 
            this.lblmsg.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.lblmsg.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.lblmsg.Appearance.Options.UseFont = true;
            this.lblmsg.Appearance.Options.UseForeColor = true;
            this.lblmsg.Location = new System.Drawing.Point(84, 72);
            this.lblmsg.Name = "lblmsg";
            this.lblmsg.Size = new System.Drawing.Size(75, 13);
            this.lblmsg.TabIndex = 3;
            this.lblmsg.Text = "labelControl7";
            this.lblmsg.Visible = false;
            // 
            // txtdesc
            // 
            this.txtdesc.Location = new System.Drawing.Point(84, 44);
            this.txtdesc.Name = "txtdesc";
            this.txtdesc.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtdesc.Properties.MaxLength = 100;
            this.txtdesc.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtdesc_Properties_BeforeShowMenu);
            this.txtdesc.Size = new System.Drawing.Size(323, 20);
            this.txtdesc.TabIndex = 2;
            // 
            // txtcodigo
            // 
            this.txtcodigo.EnterMoveNextControl = true;
            this.txtcodigo.Location = new System.Drawing.Point(84, 20);
            this.txtcodigo.Name = "txtcodigo";
            this.txtcodigo.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtcodigo.Properties.MaxLength = 12;
            this.txtcodigo.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtcodigo_Properties_BeforeShowMenu);
            this.txtcodigo.Size = new System.Drawing.Size(81, 20);
            this.txtcodigo.TabIndex = 1;
            this.txtcodigo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtcodigo_KeyPress);
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(21, 47);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(58, 13);
            this.labelControl3.TabIndex = 0;
            this.labelControl3.Text = "Descripcion:";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(21, 24);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(37, 13);
            this.labelControl2.TabIndex = 0;
            this.labelControl2.Text = "Codigo:";
            // 
            // txtCondicion
            // 
            this.txtCondicion.Location = new System.Drawing.Point(23, 62);
            this.txtCondicion.Name = "txtCondicion";
            this.txtCondicion.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtCondicion.Properties.Appearance.Options.UseFont = true;
            this.txtCondicion.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCondicion.Properties.MaxLength = 255;
            this.txtCondicion.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtCondicion_Properties_BeforeShowMenu);
            this.txtCondicion.Size = new System.Drawing.Size(711, 81);
            this.txtCondicion.TabIndex = 9;
            this.txtCondicion.Click += new System.EventHandler(this.txtCondicion_Click);
            this.txtCondicion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCondicion_KeyDown);
            this.txtCondicion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCondicion_KeyPress);
            this.txtCondicion.Leave += new System.EventHandler(this.txtCondicion_Leave);
            // 
            // btnEliminar
            // 
            this.btnEliminar.AllowFocus = false;
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminar.ImageOptions.Image")));
            this.btnEliminar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnEliminar.Location = new System.Drawing.Point(171, 144);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(70, 30);
            this.btnEliminar.TabIndex = 6;
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
            this.btnNuevo.Location = new System.Drawing.Point(21, 144);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(70, 30);
            this.btnNuevo.TabIndex = 4;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.ToolTip = "Nuevo registro";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.ImageOptions.Image")));
            this.btnGuardar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGuardar.Location = new System.Drawing.Point(95, 144);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(70, 30);
            this.btnGuardar.TabIndex = 5;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.ToolTip = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // gridResultado
            // 
            this.gridResultado.Location = new System.Drawing.Point(22, 29);
            this.gridResultado.MainView = this.viewResultado;
            this.gridResultado.Name = "gridResultado";
            this.gridResultado.Size = new System.Drawing.Size(709, 332);
            this.gridResultado.TabIndex = 11;
            this.gridResultado.TabStop = false;
            this.gridResultado.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewResultado});
            // 
            // viewResultado
            // 
            this.viewResultado.GridControl = this.gridResultado;
            this.viewResultado.Name = "viewResultado";
            this.viewResultado.OptionsView.ShowGroupPanel = false;
            // 
            // gridExpresiones
            // 
            this.gridExpresiones.Location = new System.Drawing.Point(9, 26);
            this.gridExpresiones.MainView = this.viewExpresiones;
            this.gridExpresiones.Name = "gridExpresiones";
            this.gridExpresiones.Size = new System.Drawing.Size(401, 335);
            this.gridExpresiones.TabIndex = 9;
            this.gridExpresiones.TabStop = false;
            this.gridExpresiones.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewExpresiones});
            this.gridExpresiones.Click += new System.EventHandler(this.gridExpresiones_Click);
            this.gridExpresiones.DoubleClick += new System.EventHandler(this.gridExpresiones_DoubleClick);
            this.gridExpresiones.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridExpresiones_KeyUp);
            // 
            // viewExpresiones
            // 
            this.viewExpresiones.GridControl = this.gridExpresiones;
            this.viewExpresiones.Name = "viewExpresiones";
            this.viewExpresiones.OptionsView.ShowGroupPanel = false;
            this.viewExpresiones.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.viewExpresiones_PopupMenuShowing);
            // 
            // txtQuery
            // 
            this.txtQuery.Location = new System.Drawing.Point(23, 151);
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Properties.AllowFocused = false;
            this.txtQuery.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.txtQuery.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.txtQuery.Properties.Appearance.Options.UseFont = true;
            this.txtQuery.Properties.Appearance.Options.UseForeColor = true;
            this.txtQuery.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtQuery.Properties.MaxLength = 255;
            this.txtQuery.Properties.ReadOnly = true;
            this.txtQuery.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.memoEdit1_Properties_BeforeShowMenu);
            this.txtQuery.Size = new System.Drawing.Size(711, 53);
            this.txtQuery.TabIndex = 10;
            this.txtQuery.TabStop = false;
            this.txtQuery.Click += new System.EventHandler(this.txtCondicion_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(1167, 6);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(45, 30);
            this.btnSalir.TabIndex = 12;
            this.btnSalir.ToolTip = "Cerrar Ventana";
            this.btnSalir.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnSeleccionar
            // 
            this.btnSeleccionar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSeleccionar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSeleccionar.ImageOptions.Image")));
            this.btnSeleccionar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSeleccionar.Location = new System.Drawing.Point(1063, 6);
            this.btnSeleccionar.Name = "btnSeleccionar";
            this.btnSeleccionar.Size = new System.Drawing.Size(103, 30);
            this.btnSeleccionar.TabIndex = 5;
            this.btnSeleccionar.Text = "Seleccionar";
            this.btnSeleccionar.ToolTip = "Seleccionar";
            this.btnSeleccionar.Click += new System.EventHandler(this.btnSeleccionar_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblmsg);
            this.groupBox1.Controls.Add(this.txtcodigo);
            this.groupBox1.Controls.Add(this.txtdesc);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Controls.Add(this.btnEliminar);
            this.groupBox1.Controls.Add(this.btnNuevo);
            this.groupBox1.Controls.Add(this.btnGuardar);
            this.groupBox1.Location = new System.Drawing.Point(13, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(424, 236);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Formulario";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnPrevisualizar);
            this.groupBox2.Controls.Add(this.lblCount);
            this.groupBox2.Controls.Add(this.txtCondicion);
            this.groupBox2.Controls.Add(this.txtQuery);
            this.groupBox2.Location = new System.Drawing.Point(443, 40);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(770, 236);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Previsualización";
            // 
            // btnPrevisualizar
            // 
            this.btnPrevisualizar.AllowFocus = false;
            this.btnPrevisualizar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPrevisualizar.ImageOptions.Image")));
            this.btnPrevisualizar.Location = new System.Drawing.Point(620, 30);
            this.btnPrevisualizar.Name = "btnPrevisualizar";
            this.btnPrevisualizar.Size = new System.Drawing.Size(112, 27);
            this.btnPrevisualizar.TabIndex = 12;
            this.btnPrevisualizar.TabStop = false;
            this.btnPrevisualizar.Text = "Previsualizar";
            this.btnPrevisualizar.ToolTip = "Previsualizar";
            this.btnPrevisualizar.Click += new System.EventHandler(this.btnPrevisualizar_Click);
            // 
            // lblCount
            // 
            this.lblCount.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCount.Appearance.Options.UseFont = true;
            this.lblCount.Location = new System.Drawing.Point(25, 210);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(57, 13);
            this.lblCount.TabIndex = 8;
            this.lblCount.Text = "Registros:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.gridResultado);
            this.groupBox3.Location = new System.Drawing.Point(445, 290);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(768, 376);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Resultado";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.gridExpresiones);
            this.groupBox4.Location = new System.Drawing.Point(13, 290);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(424, 376);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Conjuntos";
            // 
            // FrmFiltroBusqueda
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1227, 673);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.btnSeleccionar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmFiltroBusqueda";
            this.Text = "Condiciones";
            this.Load += new System.EventHandler(this.FrmFiltroBusqueda_Load);
            this.Shown += new System.EventHandler(this.FrmFiltroBusqueda_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.txtdesc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtcodigo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCondicion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridResultado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewResultado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridExpresiones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewExpresiones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQuery.Properties)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.TextEdit txtdesc;
        private DevExpress.XtraEditors.TextEdit txtcodigo;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.MemoEdit txtCondicion;
        private DevExpress.XtraEditors.SimpleButton btnEliminar;
        private DevExpress.XtraEditors.SimpleButton btnNuevo;
        private DevExpress.XtraEditors.SimpleButton btnGuardar;
        private DevExpress.XtraGrid.GridControl gridResultado;
        private DevExpress.XtraGrid.Views.Grid.GridView viewResultado;
        private DevExpress.XtraGrid.GridControl gridExpresiones;
        private DevExpress.XtraGrid.Views.Grid.GridView viewExpresiones;
        private DevExpress.XtraEditors.LabelControl lblmsg;
        private DevExpress.XtraEditors.MemoEdit txtQuery;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.SimpleButton btnSeleccionar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private DevExpress.XtraEditors.LabelControl lblCount;
        private DevExpress.XtraEditors.SimpleButton btnPrevisualizar;
    }
}