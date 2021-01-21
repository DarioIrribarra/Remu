namespace Labour
{
    partial class frmBuscarTrabajador
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBuscarTrabajador));
            this.btnBuscar = new DevExpress.XtraEditors.SimpleButton();
            this.txtbusqueda = new DevExpress.XtraEditors.TextEdit();
            this.gridTrabajdor = new DevExpress.XtraGrid.GridControl();
            this.viewTrabajador = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSalir = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.groupResultados = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtbusqueda.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridTrabajdor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewTrabajador)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupResultados.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBuscar
            // 
            this.btnBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBuscar.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnBuscar.ImageOptions.Image")));
            this.btnBuscar.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnBuscar.Location = new System.Drawing.Point(21, 79);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(75, 23);
            this.btnBuscar.TabIndex = 1;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.ToolTip = "Iniciar Busqueda";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // txtbusqueda
            // 
            this.txtbusqueda.Location = new System.Drawing.Point(21, 53);
            this.txtbusqueda.Name = "txtbusqueda";
            this.txtbusqueda.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(this.txtbusqueda_Properties_BeforeShowMenu);
            this.txtbusqueda.Size = new System.Drawing.Size(247, 20);
            this.txtbusqueda.TabIndex = 0;
            this.txtbusqueda.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtbusqueda_KeyDown);
            this.txtbusqueda.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtbusqueda_KeyPress);
            this.txtbusqueda.Leave += new System.EventHandler(this.txtbusqueda_Leave);
            // 
            // gridTrabajdor
            // 
            this.gridTrabajdor.Location = new System.Drawing.Point(21, 23);
            this.gridTrabajdor.MainView = this.viewTrabajador;
            this.gridTrabajdor.Name = "gridTrabajdor";
            this.gridTrabajdor.Size = new System.Drawing.Size(550, 372);
            this.gridTrabajdor.TabIndex = 3;
            this.gridTrabajdor.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewTrabajador});
            this.gridTrabajdor.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.gridTrabajdor_ProcessGridKey);
            this.gridTrabajdor.DoubleClick += new System.EventHandler(this.gridTrabajdor_DoubleClick);
            // 
            // viewTrabajador
            // 
            this.viewTrabajador.GridControl = this.gridTrabajdor;
            this.viewTrabajador.Name = "viewTrabajador";
            this.viewTrabajador.OptionsBehavior.KeepGroupExpandedOnSorting = false;
            this.viewTrabajador.OptionsView.ShowGroupPanel = false;
            this.viewTrabajador.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.viewTrabajador_PopupMenuShowing);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSalir);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Controls.Add(this.btnBuscar);
            this.groupBox1.Controls.Add(this.txtbusqueda);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(588, 141);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Búsqueda";
            // 
            // btnSalir
            // 
            this.btnSalir.AllowFocus = false;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalir.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.btnSalir.Location = new System.Drawing.Point(534, 17);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(38, 30);
            this.btnSalir.TabIndex = 117;
            this.btnSalir.ToolTip = "Cerrar Formulario";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic);
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(21, 108);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(204, 13);
            this.labelControl1.TabIndex = 11;
            this.labelControl1.Text = "Puedes buscar por rut , nombre o apellido.";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(21, 34);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(108, 13);
            this.labelControl2.TabIndex = 10;
            this.labelControl2.Text = "Ingrese una búsqueda";
            // 
            // groupResultados
            // 
            this.groupResultados.Controls.Add(this.gridTrabajdor);
            this.groupResultados.Location = new System.Drawing.Point(12, 159);
            this.groupResultados.Name = "groupResultados";
            this.groupResultados.Size = new System.Drawing.Size(588, 413);
            this.groupResultados.TabIndex = 10;
            this.groupResultados.TabStop = false;
            this.groupResultados.Text = "Resultados";
            // 
            // frmBuscarTrabajador
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(613, 579);
            this.ControlBox = false;
            this.Controls.Add(this.groupResultados);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBuscarTrabajador";
            this.Text = "Busqueda";
            this.Load += new System.EventHandler(this.frmBuscarTrabajador_Load);
            this.Shown += new System.EventHandler(this.frmBuscarTrabajador_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.txtbusqueda.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridTrabajdor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewTrabajador)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupResultados.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton btnBuscar;
        private DevExpress.XtraEditors.TextEdit txtbusqueda;
        private DevExpress.XtraGrid.GridControl gridTrabajdor;
        private DevExpress.XtraGrid.Views.Grid.GridView viewTrabajador;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private System.Windows.Forms.GroupBox groupResultados;
        private DevExpress.XtraEditors.SimpleButton btnSalir;
    }
}