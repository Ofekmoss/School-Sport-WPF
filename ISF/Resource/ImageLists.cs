using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Sport.Resources
{
	public enum Images
	{
		Plus = 0,
		Minus,
		Save,
		Undo,
		Right,
		Left,
		DRight,
		DLeft,
		Load,
		X,
		Check,
		Clear,
		Divide,
		New, 
		Down, 
		Up
	}
	public enum ColorImages
	{
		New = 0,
		Delete,
		Cut,
		Find,
        Custom,
		Print,
		CalendarType,
		DoubleLeft,
		Receipt,
		MassConfirm,
		ImportPlayers,
		HashavshevetExport
	}

	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class ImageLists : System.Windows.Forms.Form
	{
		public static ImageListStreamer GetBlackImages()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ImageLists));
			return ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("blackImages.ImageStream")));
		}

		public static ImageListStreamer GetWhiteImages()
		{
			try
			{
				ImageLists form = new ImageLists();
				System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ImageLists));
				return ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("whiteImages.ImageStream")));
			}
			catch
			{
				return null;
			}
		}

		public static ImageListStreamer GetColorImages()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ImageLists));
			return ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("colorImages.ImageStream")));
		}

		public static Image GetLogo()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ImageLists));
			return ((System.Drawing.Image)(resources.GetObject("logo.Image")));
		}

		public static ImageList CreateBlackImageList(IContainer container)
		{
			ImageList imageList = new ImageList();
			imageList.ImageSize = new System.Drawing.Size(12, 12);
			imageList.ImageStream = GetBlackImages();
			imageList.TransparentColor = System.Drawing.Color.White;
			if (container != null)
				container.Add(imageList);
			return imageList;
		}

		public static ImageList CreateWhiteImageList(IContainer container)
		{
			ImageList imageList = new ImageList();
			imageList.ImageSize = new System.Drawing.Size(12, 12);
			for (int i = 0; i < 10; i++)
			{
				imageList.ImageStream = GetWhiteImages();
				if (imageList.ImageStream != null)
					break;
			}

			if (imageList.ImageStream == null)
				return null;

			imageList.TransparentColor = System.Drawing.Color.White;
			if (container != null)
				container.Add(imageList);
			return imageList;
		}

		public static ImageList CreateColorImageList(IContainer container)
		{
			ImageList imageList = new ImageList();
			imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			imageList.ImageSize = new System.Drawing.Size(16, 16);
			imageList.ImageStream = GetColorImages();
			imageList.TransparentColor = System.Drawing.Color.White;
			if (container != null)
				container.Add(imageList);
			return imageList;
		}

		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButton4;
		private System.Windows.Forms.ToolBarButton toolBarButton5;
		private System.Windows.Forms.ToolBarButton toolBarButton6;
		private System.Windows.Forms.ToolBarButton toolBarButton7;
		private System.Windows.Forms.ToolBarButton toolBarButton8;
		private System.Windows.Forms.ToolBarButton toolBarButton9;
		private System.Windows.Forms.ToolBarButton toolBarButton10;
		private System.Windows.Forms.ToolBarButton toolBarButton11;
		private System.Windows.Forms.ToolBarButton toolBarButton12;
		private System.Windows.Forms.ToolBarButton toolBarButton13;
		private System.Windows.Forms.ToolBarButton toolBarButton14;
		private System.Windows.Forms.ToolBarButton toolBarButton15;
		private System.Windows.Forms.ToolBarButton toolBarButton16;
		private System.Windows.Forms.ImageList blackImages;
		private System.Windows.Forms.ImageList whiteImages;
		private System.Windows.Forms.ToolBar toolBar2;
		private System.Windows.Forms.ToolBarButton toolBarButton17;
		private System.Windows.Forms.ToolBarButton toolBarButton18;
		private System.Windows.Forms.ToolBarButton toolBarButton19;
		private System.Windows.Forms.ToolBarButton toolBarButton20;
		private System.Windows.Forms.ToolBarButton toolBarButton21;
		private System.Windows.Forms.ToolBarButton toolBarButton22;
		private System.Windows.Forms.ToolBarButton toolBarButton23;
		private System.Windows.Forms.ToolBarButton toolBarButton24;
		private System.Windows.Forms.ToolBarButton toolBarButton25;
		private System.Windows.Forms.ToolBarButton toolBarButton26;
		private System.Windows.Forms.ToolBarButton toolBarButton27;
		private System.Windows.Forms.ToolBarButton toolBarButton28;
		private System.Windows.Forms.ToolBarButton toolBarButton29;
		private System.Windows.Forms.ToolBarButton toolBarButton30;
		private System.Windows.Forms.ToolBar toolBar3;
		private System.Windows.Forms.ImageList colorImages;
		private System.Windows.Forms.ToolBarButton toolBarButton32;
		private System.Windows.Forms.ToolBarButton toolBarButton31;
		private System.Windows.Forms.ToolBarButton toolBarButton33;
		private System.Windows.Forms.ToolBarButton toolBarButton34;
		private System.Windows.Forms.PictureBox logo;
		private System.Windows.Forms.ToolBarButton toolBarButton35;
		private System.Windows.Forms.ToolBarButton toolBarButton36;
		private System.Windows.Forms.ToolBarButton toolBarButton37;
		private System.Windows.Forms.ToolBarButton toolBarButton38;
		private System.Windows.Forms.ToolBarButton toolBarButton39;
		private System.Windows.Forms.ToolBarButton toolBarButton40;
		private System.Windows.Forms.ToolBarButton toolBarButton41;
		private System.Windows.Forms.ToolBarButton toolBarButton42;
		private System.Windows.Forms.ToolBarButton toolBarButton43;
		private System.ComponentModel.IContainer components;

		public ImageLists()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageLists));
			this.blackImages = new System.Windows.Forms.ImageList(this.components);
			this.whiteImages = new System.Windows.Forms.ImageList(this.components);
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton16 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton17 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton18 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton19 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton20 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton21 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton22 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton23 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton31 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton35 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton38 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton39 = new System.Windows.Forms.ToolBarButton();
			this.toolBar2 = new System.Windows.Forms.ToolBar();
			this.toolBarButton5 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton6 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton7 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton8 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton15 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton24 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton25 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton26 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton27 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton28 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton29 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton30 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton32 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton36 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton40 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton41 = new System.Windows.Forms.ToolBarButton();
			this.colorImages = new System.Windows.Forms.ImageList(this.components);
			this.toolBar3 = new System.Windows.Forms.ToolBar();
			this.toolBarButton9 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton10 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton11 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton12 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton13 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton14 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton33 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton34 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton37 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton42 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton43 = new System.Windows.Forms.ToolBarButton();
			this.logo = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
			this.SuspendLayout();
			// 
			// blackImages
			// 
			this.blackImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("blackImages.ImageStream")));
			this.blackImages.TransparentColor = System.Drawing.Color.White;
			this.blackImages.Images.SetKeyName(0, "");
			this.blackImages.Images.SetKeyName(1, "");
			this.blackImages.Images.SetKeyName(2, "");
			this.blackImages.Images.SetKeyName(3, "");
			this.blackImages.Images.SetKeyName(4, "");
			this.blackImages.Images.SetKeyName(5, "");
			this.blackImages.Images.SetKeyName(6, "");
			this.blackImages.Images.SetKeyName(7, "");
			this.blackImages.Images.SetKeyName(8, "");
			this.blackImages.Images.SetKeyName(9, "");
			this.blackImages.Images.SetKeyName(10, "");
			this.blackImages.Images.SetKeyName(11, "");
			this.blackImages.Images.SetKeyName(12, "");
			this.blackImages.Images.SetKeyName(13, "");
			this.blackImages.Images.SetKeyName(14, "");
			this.blackImages.Images.SetKeyName(15, "");
			// 
			// whiteImages
			// 
			this.whiteImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("whiteImages.ImageStream")));
			this.whiteImages.TransparentColor = System.Drawing.Color.Black;
			this.whiteImages.Images.SetKeyName(0, "");
			this.whiteImages.Images.SetKeyName(1, "");
			this.whiteImages.Images.SetKeyName(2, "");
			this.whiteImages.Images.SetKeyName(3, "");
			this.whiteImages.Images.SetKeyName(4, "");
			this.whiteImages.Images.SetKeyName(5, "");
			this.whiteImages.Images.SetKeyName(6, "");
			this.whiteImages.Images.SetKeyName(7, "");
			this.whiteImages.Images.SetKeyName(8, "");
			this.whiteImages.Images.SetKeyName(9, "");
			this.whiteImages.Images.SetKeyName(10, "");
			this.whiteImages.Images.SetKeyName(11, "");
			this.whiteImages.Images.SetKeyName(12, "");
			this.whiteImages.Images.SetKeyName(13, "");
			this.whiteImages.Images.SetKeyName(14, "");
			this.whiteImages.Images.SetKeyName(15, "");
			// 
			// toolBar1
			// 
			this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton1,
            this.toolBarButton2,
            this.toolBarButton3,
            this.toolBarButton4,
            this.toolBarButton16,
            this.toolBarButton17,
            this.toolBarButton18,
            this.toolBarButton19,
            this.toolBarButton20,
            this.toolBarButton21,
            this.toolBarButton22,
            this.toolBarButton23,
            this.toolBarButton31,
            this.toolBarButton35,
            this.toolBarButton38,
            this.toolBarButton39});
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.blackImages;
			this.toolBar1.Location = new System.Drawing.Point(0, 0);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(400, 24);
			this.toolBar1.TabIndex = 0;
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.ImageIndex = 0;
			this.toolBarButton1.Name = "toolBarButton1";
			// 
			// toolBarButton2
			// 
			this.toolBarButton2.ImageIndex = 1;
			this.toolBarButton2.Name = "toolBarButton2";
			// 
			// toolBarButton3
			// 
			this.toolBarButton3.ImageIndex = 2;
			this.toolBarButton3.Name = "toolBarButton3";
			// 
			// toolBarButton4
			// 
			this.toolBarButton4.ImageIndex = 3;
			this.toolBarButton4.Name = "toolBarButton4";
			// 
			// toolBarButton16
			// 
			this.toolBarButton16.ImageIndex = 4;
			this.toolBarButton16.Name = "toolBarButton16";
			// 
			// toolBarButton17
			// 
			this.toolBarButton17.ImageIndex = 5;
			this.toolBarButton17.Name = "toolBarButton17";
			// 
			// toolBarButton18
			// 
			this.toolBarButton18.ImageIndex = 6;
			this.toolBarButton18.Name = "toolBarButton18";
			// 
			// toolBarButton19
			// 
			this.toolBarButton19.ImageIndex = 7;
			this.toolBarButton19.Name = "toolBarButton19";
			// 
			// toolBarButton20
			// 
			this.toolBarButton20.ImageIndex = 8;
			this.toolBarButton20.Name = "toolBarButton20";
			// 
			// toolBarButton21
			// 
			this.toolBarButton21.ImageIndex = 9;
			this.toolBarButton21.Name = "toolBarButton21";
			// 
			// toolBarButton22
			// 
			this.toolBarButton22.ImageIndex = 10;
			this.toolBarButton22.Name = "toolBarButton22";
			// 
			// toolBarButton23
			// 
			this.toolBarButton23.ImageIndex = 11;
			this.toolBarButton23.Name = "toolBarButton23";
			// 
			// toolBarButton31
			// 
			this.toolBarButton31.ImageIndex = 12;
			this.toolBarButton31.Name = "toolBarButton31";
			// 
			// toolBarButton35
			// 
			this.toolBarButton35.ImageIndex = 13;
			this.toolBarButton35.Name = "toolBarButton35";
			// 
			// toolBarButton38
			// 
			this.toolBarButton38.ImageIndex = 14;
			this.toolBarButton38.Name = "toolBarButton38";
			// 
			// toolBarButton39
			// 
			this.toolBarButton39.ImageIndex = 15;
			this.toolBarButton39.Name = "toolBarButton39";
			// 
			// toolBar2
			// 
			this.toolBar2.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton5,
            this.toolBarButton6,
            this.toolBarButton7,
            this.toolBarButton8,
            this.toolBarButton15,
            this.toolBarButton24,
            this.toolBarButton25,
            this.toolBarButton26,
            this.toolBarButton27,
            this.toolBarButton28,
            this.toolBarButton29,
            this.toolBarButton30,
            this.toolBarButton32,
            this.toolBarButton36,
            this.toolBarButton40,
            this.toolBarButton41});
			this.toolBar2.DropDownArrows = true;
			this.toolBar2.ImageList = this.whiteImages;
			this.toolBar2.Location = new System.Drawing.Point(0, 24);
			this.toolBar2.Name = "toolBar2";
			this.toolBar2.ShowToolTips = true;
			this.toolBar2.Size = new System.Drawing.Size(400, 24);
			this.toolBar2.TabIndex = 1;
			// 
			// toolBarButton5
			// 
			this.toolBarButton5.ImageIndex = 0;
			this.toolBarButton5.Name = "toolBarButton5";
			// 
			// toolBarButton6
			// 
			this.toolBarButton6.ImageIndex = 1;
			this.toolBarButton6.Name = "toolBarButton6";
			// 
			// toolBarButton7
			// 
			this.toolBarButton7.ImageIndex = 2;
			this.toolBarButton7.Name = "toolBarButton7";
			// 
			// toolBarButton8
			// 
			this.toolBarButton8.ImageIndex = 3;
			this.toolBarButton8.Name = "toolBarButton8";
			// 
			// toolBarButton15
			// 
			this.toolBarButton15.ImageIndex = 4;
			this.toolBarButton15.Name = "toolBarButton15";
			// 
			// toolBarButton24
			// 
			this.toolBarButton24.ImageIndex = 5;
			this.toolBarButton24.Name = "toolBarButton24";
			// 
			// toolBarButton25
			// 
			this.toolBarButton25.ImageIndex = 6;
			this.toolBarButton25.Name = "toolBarButton25";
			// 
			// toolBarButton26
			// 
			this.toolBarButton26.ImageIndex = 7;
			this.toolBarButton26.Name = "toolBarButton26";
			// 
			// toolBarButton27
			// 
			this.toolBarButton27.ImageIndex = 8;
			this.toolBarButton27.Name = "toolBarButton27";
			// 
			// toolBarButton28
			// 
			this.toolBarButton28.ImageIndex = 9;
			this.toolBarButton28.Name = "toolBarButton28";
			// 
			// toolBarButton29
			// 
			this.toolBarButton29.ImageIndex = 10;
			this.toolBarButton29.Name = "toolBarButton29";
			// 
			// toolBarButton30
			// 
			this.toolBarButton30.ImageIndex = 11;
			this.toolBarButton30.Name = "toolBarButton30";
			// 
			// toolBarButton32
			// 
			this.toolBarButton32.ImageIndex = 12;
			this.toolBarButton32.Name = "toolBarButton32";
			// 
			// toolBarButton36
			// 
			this.toolBarButton36.ImageIndex = 13;
			this.toolBarButton36.Name = "toolBarButton36";
			// 
			// toolBarButton40
			// 
			this.toolBarButton40.ImageIndex = 14;
			this.toolBarButton40.Name = "toolBarButton40";
			// 
			// toolBarButton41
			// 
			this.toolBarButton41.ImageIndex = 15;
			this.toolBarButton41.Name = "toolBarButton41";
			// 
			// colorImages
			// 
			this.colorImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("colorImages.ImageStream")));
			this.colorImages.TransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
			this.colorImages.Images.SetKeyName(0, "");
			this.colorImages.Images.SetKeyName(1, "");
			this.colorImages.Images.SetKeyName(2, "");
			this.colorImages.Images.SetKeyName(3, "");
			this.colorImages.Images.SetKeyName(4, "");
			this.colorImages.Images.SetKeyName(5, "");
			this.colorImages.Images.SetKeyName(6, "");
			this.colorImages.Images.SetKeyName(7, "");
			this.colorImages.Images.SetKeyName(8, "");
			this.colorImages.Images.SetKeyName(9, "");
			this.colorImages.Images.SetKeyName(10, "");
			this.colorImages.Images.SetKeyName(11, "hashavshevet.bmp");
			// 
			// toolBar3
			// 
			this.toolBar3.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton9,
            this.toolBarButton10,
            this.toolBarButton11,
            this.toolBarButton12,
            this.toolBarButton13,
            this.toolBarButton14,
            this.toolBarButton33,
            this.toolBarButton34,
            this.toolBarButton37,
            this.toolBarButton42,
            this.toolBarButton43});
			this.toolBar3.DropDownArrows = true;
			this.toolBar3.ImageList = this.colorImages;
			this.toolBar3.Location = new System.Drawing.Point(0, 48);
			this.toolBar3.Name = "toolBar3";
			this.toolBar3.ShowToolTips = true;
			this.toolBar3.Size = new System.Drawing.Size(400, 28);
			this.toolBar3.TabIndex = 2;
			// 
			// toolBarButton9
			// 
			this.toolBarButton9.ImageIndex = 0;
			this.toolBarButton9.Name = "toolBarButton9";
			// 
			// toolBarButton10
			// 
			this.toolBarButton10.ImageIndex = 1;
			this.toolBarButton10.Name = "toolBarButton10";
			// 
			// toolBarButton11
			// 
			this.toolBarButton11.ImageIndex = 2;
			this.toolBarButton11.Name = "toolBarButton11";
			// 
			// toolBarButton12
			// 
			this.toolBarButton12.ImageIndex = 3;
			this.toolBarButton12.Name = "toolBarButton12";
			// 
			// toolBarButton13
			// 
			this.toolBarButton13.ImageIndex = 4;
			this.toolBarButton13.Name = "toolBarButton13";
			// 
			// toolBarButton14
			// 
			this.toolBarButton14.ImageIndex = 5;
			this.toolBarButton14.Name = "toolBarButton14";
			// 
			// toolBarButton33
			// 
			this.toolBarButton33.ImageIndex = 6;
			this.toolBarButton33.Name = "toolBarButton33";
			// 
			// toolBarButton34
			// 
			this.toolBarButton34.ImageIndex = 7;
			this.toolBarButton34.Name = "toolBarButton34";
			// 
			// toolBarButton37
			// 
			this.toolBarButton37.ImageIndex = 8;
			this.toolBarButton37.Name = "toolBarButton37";
			// 
			// toolBarButton42
			// 
			this.toolBarButton42.ImageIndex = 9;
			this.toolBarButton42.Name = "toolBarButton42";
			// 
			// toolBarButton43
			// 
			this.toolBarButton43.ImageIndex = 10;
			this.toolBarButton43.Name = "toolBarButton43";
			// 
			// logo
			// 
			this.logo.Image = ((System.Drawing.Image)(resources.GetObject("logo.Image")));
			this.logo.Location = new System.Drawing.Point(48, 104);
			this.logo.Name = "logo";
			this.logo.Size = new System.Drawing.Size(272, 56);
			this.logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.logo.TabIndex = 3;
			this.logo.TabStop = false;
			// 
			// ImageLists
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(400, 266);
			this.Controls.Add(this.logo);
			this.Controls.Add(this.toolBar3);
			this.Controls.Add(this.toolBar2);
			this.Controls.Add(this.toolBar1);
			this.Name = "ImageLists";
			this.Text = "ImageLists";
			((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/*[STAThread]
		static void Main() 
		{
			Application.Run(new ImageLists());
		}*/
	}
}
