using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing.Design;

namespace Sport.Documents
{
	public class DocumentDesignerView : UserControl
	{
		private System.Windows.Forms.Panel documentControl;
		private System.Windows.Forms.ImageList buttonsImages;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button btnFirst;
		private System.Windows.Forms.Button btnPrev;
		private System.Windows.Forms.Button btnNext;
		private System.Windows.Forms.Button btnLast;
		private System.Windows.Forms.Label labelPage;
		private System.Windows.Forms.Splitter documentSplitter;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Label labelScale;
		private System.Windows.Forms.TextBox tbScale;
		private System.Windows.Forms.Panel panelScale;
		private System.Windows.Forms.VScrollBar vScrollBar;
		private System.Windows.Forms.Panel panelHScroll;
		private System.Windows.Forms.HScrollBar hScrollBar;
		private System.Windows.Forms.Panel panelPage;
		private DocumentDesigner _designer;

		public DocumentDesignerView(DocumentDesigner designer)
		{
			_designer = designer;
			InitializeComponent();

			hScrollBar.Minimum = -10;
			hScrollBar.Maximum = 10;
			hScrollBar.Value = -10;

			vScrollBar.Minimum = -10;
			vScrollBar.Maximum = 10;
			vScrollBar.Value = -10;

			SetScale(1);

			if (Document == null || Document.Pages.Count == 0)
				SetPageIndex(-1);
			else
				SetPageIndex(0);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DocumentDesignerView));
			this.documentControl = new System.Windows.Forms.Panel();
			this.panelScale = new System.Windows.Forms.Panel();
			this.tbScale = new System.Windows.Forms.TextBox();
			this.labelScale = new System.Windows.Forms.Label();
			this.btnRemove = new System.Windows.Forms.Button();
			this.buttonsImages = new System.Windows.Forms.ImageList(this.components);
			this.btnAdd = new System.Windows.Forms.Button();
			this.labelPage = new System.Windows.Forms.Label();
			this.btnLast = new System.Windows.Forms.Button();
			this.btnNext = new System.Windows.Forms.Button();
			this.btnPrev = new System.Windows.Forms.Button();
			this.btnFirst = new System.Windows.Forms.Button();
			this.documentSplitter = new System.Windows.Forms.Splitter();
			this.vScrollBar = new System.Windows.Forms.VScrollBar();
			this.panelHScroll = new System.Windows.Forms.Panel();
			this.hScrollBar = new System.Windows.Forms.HScrollBar();
			this.panelPage = new System.Windows.Forms.Panel();
			this.documentControl.SuspendLayout();
			this.panelScale.SuspendLayout();
			this.panelHScroll.SuspendLayout();
			this.SuspendLayout();
			// 
			// documentControl
			// 
			this.documentControl.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(251)), ((System.Byte)(233)));
			this.documentControl.Controls.Add(this.panelScale);
			this.documentControl.Controls.Add(this.btnRemove);
			this.documentControl.Controls.Add(this.btnAdd);
			this.documentControl.Controls.Add(this.labelPage);
			this.documentControl.Controls.Add(this.btnLast);
			this.documentControl.Controls.Add(this.btnNext);
			this.documentControl.Controls.Add(this.btnPrev);
			this.documentControl.Controls.Add(this.btnFirst);
			this.documentControl.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.documentControl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.documentControl.Location = new System.Drawing.Point(0, 168);
			this.documentControl.Name = "documentControl";
			this.documentControl.Size = new System.Drawing.Size(544, 40);
			this.documentControl.TabIndex = 0;
			// 
			// panelScale
			// 
			this.panelScale.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelScale.Controls.Add(this.tbScale);
			this.panelScale.Controls.Add(this.labelScale);
			this.panelScale.Location = new System.Drawing.Point(255, 8);
			this.panelScale.Name = "panelScale";
			this.panelScale.Size = new System.Drawing.Size(97, 23);
			this.panelScale.TabIndex = 7;
			// 
			// tbScale
			// 
			this.tbScale.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbScale.AutoSize = false;
			this.tbScale.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbScale.Location = new System.Drawing.Point(40, 1);
			this.tbScale.Name = "tbScale";
			this.tbScale.Size = new System.Drawing.Size(54, 19);
			this.tbScale.TabIndex = 1;
			this.tbScale.Text = "%100";
			this.tbScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.tbScale.LostFocus += new System.EventHandler(this.tbScale_LostFocus);
			this.tbScale.GotFocus += new System.EventHandler(this.tbScale_GotFocus);
			this.tbScale.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbScale_KeyPress);
			this.tbScale.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.tbScale_MouseWheel);
			// 
			// labelScale
			// 
			this.labelScale.Dock = System.Windows.Forms.DockStyle.Left;
			this.labelScale.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.labelScale.Location = new System.Drawing.Point(0, 0);
			this.labelScale.Name = "labelScale";
			this.labelScale.Size = new System.Drawing.Size(40, 21);
			this.labelScale.TabIndex = 0;
			this.labelScale.Text = "Scale:";
			this.labelScale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnRemove
			// 
			this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRemove.ImageIndex = 5;
			this.btnRemove.ImageList = this.buttonsImages;
			this.btnRemove.Location = new System.Drawing.Point(226, 8);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(24, 23);
			this.btnRemove.TabIndex = 6;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// buttonsImages
			// 
			this.buttonsImages.ImageSize = new System.Drawing.Size(12, 12);
			this.buttonsImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("buttonsImages.ImageStream")));
			this.buttonsImages.TransparentColor = System.Drawing.Color.White;
			// 
			// btnAdd
			// 
			this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAdd.ImageIndex = 4;
			this.btnAdd.ImageList = this.buttonsImages;
			this.btnAdd.Location = new System.Drawing.Point(201, 8);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(24, 23);
			this.btnAdd.TabIndex = 5;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// labelPage
			// 
			this.labelPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelPage.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.labelPage.Location = new System.Drawing.Point(58, 8);
			this.labelPage.Name = "labelPage";
			this.labelPage.Size = new System.Drawing.Size(88, 23);
			this.labelPage.TabIndex = 4;
			this.labelPage.Text = "Page 1 of 1";
			this.labelPage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnLast
			// 
			this.btnLast.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnLast.ImageIndex = 3;
			this.btnLast.ImageList = this.buttonsImages;
			this.btnLast.Location = new System.Drawing.Point(172, 8);
			this.btnLast.Name = "btnLast";
			this.btnLast.Size = new System.Drawing.Size(24, 23);
			this.btnLast.TabIndex = 3;
			this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
			// 
			// btnNext
			// 
			this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnNext.ImageIndex = 2;
			this.btnNext.ImageList = this.buttonsImages;
			this.btnNext.Location = new System.Drawing.Point(147, 8);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(24, 23);
			this.btnNext.TabIndex = 2;
			this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
			// 
			// btnPrev
			// 
			this.btnPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnPrev.ImageIndex = 1;
			this.btnPrev.ImageList = this.buttonsImages;
			this.btnPrev.Location = new System.Drawing.Point(33, 8);
			this.btnPrev.Name = "btnPrev";
			this.btnPrev.Size = new System.Drawing.Size(24, 23);
			this.btnPrev.TabIndex = 1;
			this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
			// 
			// btnFirst
			// 
			this.btnFirst.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnFirst.ImageIndex = 0;
			this.btnFirst.ImageList = this.buttonsImages;
			this.btnFirst.Location = new System.Drawing.Point(8, 8);
			this.btnFirst.Name = "btnFirst";
			this.btnFirst.Size = new System.Drawing.Size(24, 23);
			this.btnFirst.TabIndex = 0;
			this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
			// 
			// documentSplitter
			// 
			this.documentSplitter.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(236)), ((System.Byte)(233)), ((System.Byte)(216)));
			this.documentSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.documentSplitter.Location = new System.Drawing.Point(0, 165);
			this.documentSplitter.Name = "documentSplitter";
			this.documentSplitter.Size = new System.Drawing.Size(544, 3);
			this.documentSplitter.TabIndex = 1;
			this.documentSplitter.TabStop = false;
			// 
			// vScrollBar
			// 
			this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar.Location = new System.Drawing.Point(527, 0);
			this.vScrollBar.Name = "vScrollBar";
			this.vScrollBar.Size = new System.Drawing.Size(17, 148);
			this.vScrollBar.TabIndex = 2;
			this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
			// 
			// panelHScroll
			// 
			this.panelHScroll.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(236)), ((System.Byte)(233)), ((System.Byte)(216)));
			this.panelHScroll.Controls.Add(this.hScrollBar);
			this.panelHScroll.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelHScroll.Location = new System.Drawing.Point(0, 148);
			this.panelHScroll.Name = "panelHScroll";
			this.panelHScroll.Size = new System.Drawing.Size(544, 17);
			this.panelHScroll.TabIndex = 3;
			// 
			// hScrollBar
			// 
			this.hScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.hScrollBar.Location = new System.Drawing.Point(0, 0);
			this.hScrollBar.Name = "hScrollBar";
			this.hScrollBar.Size = new System.Drawing.Size(527, 17);
			this.hScrollBar.TabIndex = 0;
			this.hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar_Scroll);
			// 
			// panelPage
			// 
			this.panelPage.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.panelPage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelPage.Location = new System.Drawing.Point(0, 0);
			this.panelPage.Name = "panelPage";
			this.panelPage.Size = new System.Drawing.Size(527, 148);
			this.panelPage.TabIndex = 4;
			this.panelPage.Paint += new System.Windows.Forms.PaintEventHandler(this.panelPage_Paint);
			// 
			// DocumentDesignerView
			// 
			this.Controls.Add(this.panelPage);
			this.Controls.Add(this.vScrollBar);
			this.Controls.Add(this.panelHScroll);
			this.Controls.Add(this.documentSplitter);
			this.Controls.Add(this.documentControl);
			this.Name = "DocumentDesignerView";
			this.Size = new System.Drawing.Size(544, 208);
			this.documentControl.ResumeLayout(false);
			this.panelScale.ResumeLayout(false);
			this.panelHScroll.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		public Document Document
		{
			get { return _designer.Component as Document; }
		}

		public void RefreshDocument()
		{
			int index = pageIndex;
			if (index == -1 && Document != null && Document.Pages.Count > 0)
			{
				index = 0;
			}
			else if (index >= Document.Pages.Count)
				index = Document.Pages.Count - 1;

			SetPageIndex(index);
		}

		#region Page Control

		private int pageIndex;
		public int PageIndex
		{
			get { return pageIndex; }
			set { SetPageIndex(value); }
		}

		public Page CurrentPage
		{
			get { return pageIndex == -1 ? null : Document.Pages[pageIndex]; }
		}

		private void SetPageIndex(int index)
		{
			if (Document == null)
				throw new ArgumentNullException("document", "No document selected");

			int pageCount = Document.Pages.Count;
			if (index >= pageCount)
				throw new ArgumentOutOfRangeException("index", "Page index out of bound");

            pageIndex = index;
			if (pageIndex == -1)
				labelPage.Text = Document.Pages.Count.ToString() + " pages";
			else
				labelPage.Text = "Page " + (pageIndex + 1).ToString() + " of " + Document.Pages.Count.ToString();

			if (pageCount == 0)
			{
				btnFirst.Enabled = false;
				btnPrev.Enabled = false;
				btnNext.Enabled = false;
				btnLast.Enabled = false;
				btnRemove.Enabled = false;
			}
			else
			{
				btnFirst.Enabled = pageIndex != 0;
				btnPrev.Enabled = pageIndex != 0;
				btnNext.Enabled = pageIndex != pageCount - 1;
				btnLast.Enabled = pageIndex != pageCount - 1;
				btnRemove.Enabled = pageIndex != -1;
			}

			ResetScroll();

			Refresh();

			ISelectionService ss = (ISelectionService) _designer.Component.Site.GetService(typeof(ISelectionService));
			ss.SetSelectedComponents(
				pageIndex == -1 ? new object[] { Document } : new object[] { CurrentPage });
		}

		private void btnFirst_Click(object sender, System.EventArgs e)
		{
			SetPageIndex(0);
		}

		private void btnPrev_Click(object sender, System.EventArgs e)
		{
			if (pageIndex == -1)
			{
				if (Document == null)
					throw new ArgumentNullException("document", "No document selected");

				SetPageIndex(Document.Pages.Count - 1);
			}
			else
			{
				SetPageIndex(pageIndex - 1);
			}
		}

		private void btnNext_Click(object sender, System.EventArgs e)
		{
			SetPageIndex(pageIndex + 1);
		}

		private void btnLast_Click(object sender, System.EventArgs e)
		{
			if (Document == null)
				throw new ArgumentNullException("document", "No document selected");

			SetPageIndex(Document.Pages.Count - 1);
		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			if (Document == null)
				throw new ArgumentNullException("document", "No document selected");

			Page page = new Page();
			_designer.Component.Site.Container.Add(page);
			Document.Pages.Add(page);
			RefreshDocument();
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			if (Document == null)
				throw new ArgumentNullException("document", "No document selected");

			_designer.Component.Site.Container.Remove(CurrentPage);
			Document.Pages.RemoveAt(pageIndex);
			RefreshDocument();
		}

		#endregion

		#region Scaling

		private float pageScale;
		public float PageScale
		{
			get { return pageScale; }
			set { SetScale(value); }
		}

		private void SetScale(float scale)
		{
			if (scale < 0.01f)
				scale = 0.01f;

			pageScale = scale;

			if (pageScale == 0)
				tbScale.Text = null;
			else
				tbScale.Text = (scale * 100).ToString("F") + "%";

			ResetScroll();

			Refresh();
		}

		private void tbScale_GotFocus(object sender, EventArgs e)
		{
            tbScale.SelectAll();
		}

		private void tbScale_LostFocus(object sender, EventArgs e)
		{
			ResetScale();
		}

		private void tbScale_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char) Keys.Enter)
			{
				e.Handled = true;
				tbScale.SelectAll();
				ResetScale();
			}
		}

		private void tbScale_MouseWheel(object sender, MouseEventArgs e)
		{
			float scale = -1;
			try
			{
				string s = tbScale.Text.Replace("%", " ");
				scale = (float) Double.Parse(s) / 100;
			}
			catch
			{
				scale = -1;
			}

			if (scale == -1)
			{
				scale = 0.05f;
			}
			else
			{
				int per = e.Delta / 24;
				scale += (float) (per) / 100f;
			}

			if (scale > 0 && scale < 10)
				SetScale(scale);

		}


		private void ResetScale()
		{
			float scale = -1;
			try
			{
				string s = tbScale.Text.Replace("%", " ");
				scale = (float) Double.Parse(s) / 100;
			}
			catch
			{
				scale = -1;
			}

			if (scale > 0 && scale < 10)
				SetScale(scale);
			else
				tbScale.Text = (pageScale * 100).ToString() + "%";
		}

		#endregion

		#region Scrolling

		private void hScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			Refresh();
		}

		private void vScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			Refresh();
		}

		private void ResetScroll()
		{
			vScrollBar.Enabled = false;
			hScrollBar.Enabled = false;
		}

		#endregion

		private void panelPage_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{

			Document document = Document;

			if (document != null)
			{
				Page page = CurrentPage;
				if (page != null)
				{
					// Painting page

					System.Drawing.Rectangle bounds = 
						new System.Drawing.Rectangle(4, 4, e.ClipRectangle.Width - 8,
						e.ClipRectangle.Height - 8);

					if (page.Size.Width > 0 && page.Size.Height > 0)
					{
						e.Graphics.TranslateTransform(- hScrollBar.Value, - vScrollBar.Value);
						//e.Graphics.PageUnit = System.Drawing.GraphicsUnit.Document;
						float sx = e.Graphics.DpiX / 100 * pageScale;
						float sy = e.Graphics.DpiY / 100 * pageScale;
						e.Graphics.ScaleTransform(sx, sy);

						System.Drawing.Rectangle rect = new System.Drawing.Rectangle(System.Drawing.Point.Empty, page.Size);
						e.Graphics.FillRectangle(
							page.BackBrush == null ? System.Drawing.SystemBrushes.Window : page.BackBrush, 
							rect);
						e.Graphics.DrawRectangle(
							page.Border == null ? System.Drawing.SystemPens.WindowFrame : page.Border,
							rect);

						page.Paint(e.Graphics);
						e.Graphics.ResetTransform();
					}
				}
				else
				{
					e.Graphics.DrawString("No page", Font, System.Drawing.SystemBrushes.WindowText,
						new System.Drawing.Rectangle(10, 10, 100, 20));
				}
			}
			else
			{
				throw new DocumentException("Component is not a Document");
			}
		}
	}

	public class DocumentDesigner : ComponentDesigner, IRootDesigner, IToolboxUser
	{
		public DocumentDesigner()
		{
		}

		private DocumentDesignerView _view;

		object IRootDesigner.GetView(ViewTechnology technology) 
		{
			if (technology != ViewTechnology.Default) //ViewTechnology.WindowsForms)
			{
				throw new ArgumentException("Not a supported view technology", "technology");
			}

			if (_view == null)
			{
				_view = new DocumentDesignerView(this);
			}

			return _view;
		}

		ViewTechnology[] IRootDesigner.SupportedTechnologies 
		{
			get
			{
				return new ViewTechnology[] {ViewTechnology.Default};
			}
		}

		public override void Initialize(IComponent component)
		{
			base.Initialize (component);

			IDesignerHost dh = (IDesignerHost) GetService(typeof(IDesignerHost));
			dh.LoadComplete += new EventHandler(ComponentLoadComplete);

			IComponentChangeService ccs = (IComponentChangeService) GetService(typeof(IComponentChangeService));
			ccs.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
			ccs.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
		}

		private void ComponentLoadComplete(object sender, EventArgs e)
		{
			if (_view != null)
				_view.RefreshDocument();
		}

		private void OnComponentAdded(object sender, ComponentEventArgs e)
		{
		}

		private void OnComponentRemoved(object sender, ComponentEventArgs e)
		{
		}

		#region IToolboxUser Members

		public void ToolPicked(System.Drawing.Design.ToolboxItem tool)
		{
			_view.Refresh();
		}

		public bool GetToolSupported(ToolboxItem tool)
		{
			//_view.Refresh();
			return false;
		}

		#endregion
	}
}
