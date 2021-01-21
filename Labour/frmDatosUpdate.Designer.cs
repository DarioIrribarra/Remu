namespace Labour
{
    partial class frmDatosUpdate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDatosUpdate));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnProbarConexion = new DevExpress.XtraEditors.SimpleButton();
            this.gridDatos = new DevExpress.XtraGrid.GridControl();
            this.viewDatos = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnEliminar = new DevExpress.XtraEditors.SimpleButton();
            this.btnNuevo = new DevExpress.XtraEditors.SimpleButton();
            this.btnSeleccionar = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::Labour.WaitForm1), true, true, true);
            this.btnSalirAfp = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDatos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDatos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSalirAfp);
            this.layoutControl1.Controls.Add(this.btnProbarConexion);
            this.layoutControl1.Controls.Add(this.gridDatos);
            this.layoutControl1.Controls.Add(this.btnEliminar);
            this.layoutControl1.Controls.Add(this.btnNuevo);
            this.layoutControl1.Controls.Add(this.btnSeleccionar);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(904, 240, 450, 400);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(649, 449);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnProbarConexion
            // 
            this.btnProbarConexion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProbarConexion.ImageOptions.ImageUri.Uri = "EditDataSource;Size32x32;Office2013";
            this.btnProbarConexion.Location = new System.Drawing.Point(352, 12);
            this.btnProbarConexion.Name = "btnProbarConexion";
            this.btnProbarConexion.Size = new System.Drawing.Size(113, 38);
            this.btnProbarConexion.StyleController = this.layoutControl1;
            this.btnProbarConexion.TabIndex = 25;
            this.btnProbarConexion.Text = "Test Conexion";
            this.btnProbarConexion.ToolTip = "Probar Conexion";
            this.btnProbarConexion.Click += new System.EventHandler(this.btnProbarConexion_Click);
            // 
            // gridDatos
            // 
            this.gridDatos.Location = new System.Drawing.Point(12, 64);
            this.gridDatos.MainView = this.viewDatos;
            this.gridDatos.Name = "gridDatos";
            this.gridDatos.Size = new System.Drawing.Size(625, 373);
            this.gridDatos.TabIndex = 13;
            this.gridDatos.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewDatos});
            this.gridDatos.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.gridDatos_ProcessGridKey);
            this.gridDatos.DoubleClick += new System.EventHandler(this.gridDatos_DoubleClick);
            this.gridDatos.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridDatos_KeyDown);
            this.gridDatos.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gridDatos_KeyPress);
            this.gridDatos.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.gridDatos_PreviewKeyDown);
            // 
            // viewDatos
            // 
            this.viewDatos.GridControl = this.gridDatos;
            this.viewDatos.Name = "viewDatos";
            this.viewDatos.OptionsClipboard.AllowCopy = DevExpress.Utils.DefaultBoolean.False;
            this.viewDatos.OptionsCustomization.AllowColumnMoving = false;
            this.viewDatos.OptionsCustomization.AllowColumnResizing = false;
            this.viewDatos.OptionsCustomization.AllowFilter = false;
            this.viewDatos.OptionsCustomization.AllowGroup = false;
            this.viewDatos.OptionsCustomization.AllowQuickHideColumns = false;
            this.viewDatos.OptionsCustomization.AllowSort = false;
            this.viewDatos.OptionsMenu.EnableColumnMenu = false;
            this.viewDatos.OptionsMenu.EnableFooterMenu = false;
            this.viewDatos.OptionsMenu.EnableGroupPanelMenu = false;
            this.viewDatos.OptionsMenu.ShowAutoFilterRowItem = false;
            this.viewDatos.OptionsMenu.ShowDateTimeGroupIntervalItems = false;
            this.viewDatos.OptionsMenu.ShowGroupSortSummaryItems = false;
            this.viewDatos.OptionsMenu.ShowSplitItem = false;
            this.viewDatos.OptionsNavigation.UseTabKey = false;
            this.viewDatos.OptionsView.ShowGroupPanel = false;
            this.viewDatos.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.viewDatos_PopupMenuShowing);
            this.viewDatos.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.viewDatos_ShowingEditor);
            this.viewDatos.HiddenEditor += new System.EventHandler(this.viewDatos_HiddenEditor);
            this.viewDatos.ShownEditor += new System.EventHandler(this.viewDatos_ShownEditor);
            this.viewDatos.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.viewDatos_CellValueChanged);
            this.viewDatos.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.viewDatos_CellValueChanging);
            this.viewDatos.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(this.viewDatos_RowUpdated);
            this.viewDatos.DoubleClick += new System.EventHandler(this.viewDatos_DoubleClick);
            this.viewDatos.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.viewDatos_ValidatingEditor);
            this.viewDatos.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.viewDatos_InvalidValueException);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.ImageOptions.ImageUri.Uri = "Cancel;Size32x32;Office2013";
            this.btnEliminar.Location = new System.Drawing.Point(165, 12);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(81, 38);
            this.btnEliminar.StyleController = this.layoutControl1;
            this.btnEliminar.TabIndex = 20;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.ToolTip = "Eliminar Juego";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnNuevo
            // 
            this.btnNuevo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevo.ImageOptions.ImageUri.Uri = "Add;Size32x32;Office2013";
            this.btnNuevo.Location = new System.Drawing.Point(78, 12);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(83, 38);
            this.btnNuevo.StyleController = this.layoutControl1;
            this.btnNuevo.TabIndex = 21;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.ToolTip = "Nueva Configuracion";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnSeleccionar
            // 
            this.btnSeleccionar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSeleccionar.ImageOptions.ImageUri.Uri = "Apply;Size32x32;Office2013";
            this.btnSeleccionar.Location = new System.Drawing.Point(250, 12);
            this.btnSeleccionar.Name = "btnSeleccionar";
            this.btnSeleccionar.Size = new System.Drawing.Size(98, 38);
            this.btnSeleccionar.StyleController = this.layoutControl1;
            this.btnSeleccionar.TabIndex = 23;
            this.btnSeleccionar.Text = "Seleccionar";
            this.btnSeleccionar.ToolTip = "Seleccionar Juego";
            this.btnSeleccionar.Click += new System.EventHandler(this.btnSeleccionar_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.emptySpaceItem3,
            this.layoutControlItem8,
            this.layoutControlItem7,
            this.layoutControlItem11,
            this.emptySpaceItem2,
            this.emptySpaceItem5,
            this.layoutControlItem2,
            this.layoutControlItem10,
            this.layoutControlItem1});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(649, 449);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(0, 42);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(585, 10);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.btnNuevo;
            this.layoutControlItem8.Location = new System.Drawing.Point(66, 0);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(87, 42);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnEliminar;
            this.layoutControlItem7.Location = new System.Drawing.Point(153, 0);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(85, 42);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.btnSeleccionar;
            this.layoutControlItem11.Location = new System.Drawing.Point(238, 0);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(102, 42);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(66, 42);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem5
            // 
            this.emptySpaceItem5.AllowHotTrack = false;
            this.emptySpaceItem5.Location = new System.Drawing.Point(457, 0);
            this.emptySpaceItem5.Name = "emptySpaceItem5";
            this.emptySpaceItem5.Size = new System.Drawing.Size(128, 42);
            this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnProbarConexion;
            this.layoutControlItem2.Location = new System.Drawing.Point(340, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(117, 42);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.gridDatos;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 52);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(629, 377);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // btnSalirAfp
            // 
            this.btnSalirAfp.AllowFocus = false;
            this.btnSalirAfp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalirAfp.ImageOptions.Image = global::Labour.Properties.Resources.cerrarpuerta;
            this.btnSalirAfp.Location = new System.Drawing.Point(597, 12);
            this.btnSalirAfp.Name = "btnSalirAfp";
            this.btnSalirAfp.Size = new System.Drawing.Size(40, 38);
            this.btnSalirAfp.StyleController = this.layoutControl1;
            this.btnSalirAfp.TabIndex = 11;
            this.btnSalirAfp.ToolTip = "Cerrar Formulario";
            this.btnSalirAfp.Click += new System.EventHandler(this.btnSalirAfp_Click);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnSalirAfp;
            this.layoutControlItem1.Location = new System.Drawing.Point(585, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(44, 52);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // frmDatosUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(649, 449);
            this.ControlBox = false;
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDatosUpdate";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuracion";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDatosUpdate_FormClosing);
            this.Load += new System.EventHandler(this.frmDatosUpdate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridDatos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDatos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridDatos;
        private DevExpress.XtraGrid.Views.Grid.GridView viewDatos;
        private DevExpress.XtraEditors.SimpleButton btnEliminar;
        private DevExpress.XtraEditors.SimpleButton btnNuevo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.SimpleButton btnSeleccionar;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem5;
        private DevExpress.XtraEditors.SimpleButton btnProbarConexion;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.SimpleButton btnSalirAfp;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}