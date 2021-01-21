namespace Labour
{
    partial class frmconfLibro
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmconfLibro));
            this.btnGrabar = new DevExpress.XtraEditors.SimpleButton();
            this.gridLibro = new DevExpress.XtraGrid.GridControl();
            this.viewLibro = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cbCursiva = new DevExpress.XtraEditors.CheckEdit();
            this.cbNegrita = new DevExpress.XtraEditors.CheckEdit();
            this.cbVisible = new DevExpress.XtraEditors.CheckEdit();
            this.txtItem = new DevExpress.XtraEditors.LookUpEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtNum = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnNuevo = new DevExpress.XtraEditors.SimpleButton();
            this.btnEliminar = new DevExpress.XtraEditors.SimpleButton();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.cbxTipoItem = new DevExpress.XtraEditors.LookUpEdit();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.txtAlias = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
            ((System.ComponentModel.ISupportInitialize)(this.gridLibro)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewLibro)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbCursiva.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbNegrita.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbVisible.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItem.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNum.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbxTipoItem.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAlias.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGrabar
            // 
            this.btnGrabar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGrabar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGrabar.ImageOptions.Image")));
            this.btnGrabar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnGrabar.Location = new System.Drawing.Point(103, 169);
            this.btnGrabar.Name = "btnGrabar";
            this.btnGrabar.Size = new System.Drawing.Size(66, 29);
            this.btnGrabar.TabIndex = 8;
            this.btnGrabar.Text = "Guardar";
            this.btnGrabar.ToolTip = "Guardar Registro";
            this.btnGrabar.Click += new System.EventHandler(this.btnGrabar_Click);
            // 
            // gridLibro
            // 
            this.gridLibro.Location = new System.Drawing.Point(19, 204);
            this.gridLibro.MainView = this.viewLibro;
            this.gridLibro.Name = "gridLibro";
            this.gridLibro.Size = new System.Drawing.Size(643, 318);
            this.gridLibro.TabIndex = 10;
            this.gridLibro.TabStop = false;
            this.gridLibro.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewLibro});
            this.gridLibro.Click += new System.EventHandler(this.gridLibro_Click);
            this.gridLibro.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridLibro_KeyUp);
            // 
            // viewLibro
            // 
            this.viewLibro.GridControl = this.gridLibro;
            this.viewLibro.Name = "viewLibro";
            this.viewLibro.OptionsView.ShowGroupPanel = false;
            // 
            // cbCursiva
            // 
            this.cbCursiva.Location = new System.Drawing.Point(181, 127);
            this.cbCursiva.Name = "cbCursiva";
            this.cbCursiva.Properties.Caption = "Cursiva";
            this.cbCursiva.Size = new System.Drawing.Size(75, 19);
            this.cbCursiva.TabIndex = 6;
            // 
            // cbNegrita
            // 
            this.cbNegrita.Location = new System.Drawing.Point(111, 127);
            this.cbNegrita.Name = "cbNegrita";
            this.cbNegrita.Properties.Caption = "Negrita";
            this.cbNegrita.Size = new System.Drawing.Size(64, 19);
            this.cbNegrita.TabIndex = 5;
            // 
            // cbVisible
            // 
            this.cbVisible.EditValue = true;
            this.cbVisible.Location = new System.Drawing.Point(55, 127);
            this.cbVisible.Name = "cbVisible";
            this.cbVisible.Properties.Caption = "Visible";
            this.cbVisible.Size = new System.Drawing.Size(54, 19);
            this.cbVisible.TabIndex = 4;
            // 
            // txtItem
            // 
            this.txtItem.Location = new System.Drawing.Point(55, 57);
            this.txtItem.Name = "txtItem";
            this.txtItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtItem.Size = new System.Drawing.Size(201, 20);
            this.txtItem.TabIndex = 1;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(23, 60);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(26, 13);
            this.labelControl2.TabIndex = 6;
            this.labelControl2.Text = "Item:";
            // 
            // txtNum
            // 
            this.txtNum.Location = new System.Drawing.Point(55, 79);
            this.txtNum.Name = "txtNum";
            this.txtNum.Properties.MaxLength = 4;
            this.txtNum.Properties.ReadOnly = true;
            this.txtNum.Size = new System.Drawing.Size(54, 20);
            this.txtNum.TabIndex = 2;
            this.txtNum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNum_KeyPress);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(33, 83);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(16, 13);
            this.labelControl1.TabIndex = 4;
            this.labelControl1.Text = "N°:";
            // 
            // btnNuevo
            // 
            this.btnNuevo.AllowFocus = false;
            this.btnNuevo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevo.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNuevo.ImageOptions.Image")));
            this.btnNuevo.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnNuevo.Location = new System.Drawing.Point(23, 169);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(74, 29);
            this.btnNuevo.TabIndex = 7;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.ToolTip = "Nuevo Registro";
            this.btnNuevo.Visible = false;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.AllowFocus = false;
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEliminar.ImageOptions.Image")));
            this.btnEliminar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnEliminar.Location = new System.Drawing.Point(175, 169);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(66, 29);
            this.btnEliminar.TabIndex = 9;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.ToolTip = "Guardar Registro";
            this.btnEliminar.Visible = false;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.labelControl5);
            this.groupControl1.Controls.Add(this.labelControl4);
            this.groupControl1.Controls.Add(this.cbxTipoItem);
            this.groupControl1.Controls.Add(this.btnSalir);
            this.groupControl1.Controls.Add(this.txtAlias);
            this.groupControl1.Controls.Add(this.labelControl3);
            this.groupControl1.Controls.Add(this.separatorControl1);
            this.groupControl1.Controls.Add(this.cbCursiva);
            this.groupControl1.Controls.Add(this.gridLibro);
            this.groupControl1.Controls.Add(this.btnNuevo);
            this.groupControl1.Controls.Add(this.btnEliminar);
            this.groupControl1.Controls.Add(this.labelControl2);
            this.groupControl1.Controls.Add(this.btnGrabar);
            this.groupControl1.Controls.Add(this.cbNegrita);
            this.groupControl1.Controls.Add(this.txtItem);
            this.groupControl1.Controls.Add(this.cbVisible);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Controls.Add(this.txtNum);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(674, 525);
            this.groupControl1.TabIndex = 9;
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Location = new System.Drawing.Point(263, 105);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(106, 13);
            this.labelControl5.TabIndex = 14;
            this.labelControl5.Text = "Máximo 15 caracteres";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(23, 36);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(24, 13);
            this.labelControl4.TabIndex = 13;
            this.labelControl4.Text = "Tipo:";
            // 
            // cbxTipoItem
            // 
            this.cbxTipoItem.Location = new System.Drawing.Point(55, 33);
            this.cbxTipoItem.Name = "cbxTipoItem";
            this.cbxTipoItem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbxTipoItem.Size = new System.Drawing.Size(201, 20);
            this.cbxTipoItem.TabIndex = 12;
            this.cbxTipoItem.EditValueChanged += new System.EventHandler(this.cbxTipoItem_EditValueChanged);
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.Location = new System.Drawing.Point(609, 23);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(43, 30);
            this.btnSalir.TabIndex = 11;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // txtAlias
            // 
            this.txtAlias.Location = new System.Drawing.Point(55, 101);
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.Properties.MaxLength = 15;
            this.txtAlias.Size = new System.Drawing.Size(201, 20);
            this.txtAlias.TabIndex = 3;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(23, 105);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(26, 13);
            this.labelControl3.TabIndex = 10;
            this.labelControl3.Text = "Alias:";
            // 
            // separatorControl1
            // 
            this.separatorControl1.Location = new System.Drawing.Point(23, 147);
            this.separatorControl1.Name = "separatorControl1";
            this.separatorControl1.Size = new System.Drawing.Size(643, 20);
            this.separatorControl1.TabIndex = 9;
            // 
            // frmconfLibro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 525);
            this.ControlBox = false;
            this.Controls.Add(this.groupControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmconfLibro";
            this.Text = "Configuracion libro";
            this.Load += new System.EventHandler(this.frmconfLibro_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridLibro)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewLibro)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbCursiva.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbNegrita.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbVisible.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItem.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNum.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbxTipoItem.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAlias.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnGrabar;
        private DevExpress.XtraGrid.GridControl gridLibro;
        private DevExpress.XtraGrid.Views.Grid.GridView viewLibro;
        private DevExpress.XtraEditors.CheckEdit cbCursiva;
        private DevExpress.XtraEditors.CheckEdit cbNegrita;
        private DevExpress.XtraEditors.CheckEdit cbVisible;
        private DevExpress.XtraEditors.LookUpEdit txtItem;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtNum;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnNuevo;
        private DevExpress.XtraEditors.SimpleButton btnEliminar;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SeparatorControl separatorControl1;
        private DevExpress.XtraEditors.TextEdit txtAlias;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LookUpEdit cbxTipoItem;
        private DevExpress.XtraEditors.LabelControl labelControl5;
    }
}