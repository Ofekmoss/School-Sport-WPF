using System;
using System.Drawing;
using System.Windows.Forms;

using System.Collections;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for DayEventsView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships)]
	public class DayEventsView : Sport.UI.View
	{
		#region private members
		private System.Globalization.HebrewCalendar hebrewCalendar;
		private System.Windows.Forms.Label labelDate;
		private System.Globalization.GregorianCalendar gregorianCalendar;
		private System.Windows.Forms.Panel linksPanel;
		private System.Collections.ArrayList _links=null;
		private System.Collections.Hashtable _events=null;
		private readonly string MAIN_BLANKS="   ";
		#endregion
		
		#region constuctors
		public DayEventsView()
		{

			_links = new ArrayList();
			_events = new Hashtable();
			
			InitializeComponent();
			
			hebrewCalendar = new System.Globalization.HebrewCalendar();
			gregorianCalendar = new System.Globalization.GregorianCalendar();

			Title = "לוח שנה";
		}
		#endregion
		
		#region Public Methods
		#region Select Date
		/// <summary>
		/// Display events for the given date.
		/// </summary>
		public void SelectDate(DateTime date)
		{
			//first, clear previous controls from the screen:
			foreach (Control control in _links)
			{
				if (linksPanel.Controls.IndexOf(control)>=0)
					linksPanel.Controls.Remove(control);
			}
			
			//clear list of controls:
			_links.Clear();
			
			//clear all the stored events:
			foreach (ArrayList al in _events.Values)
			{
				al.Clear();
			}
			_events.Clear();
			
			//maybe invalid time?
			if (date.Year < 1900)
			{
				Title = "לוח שנה";
				labelDate.Text = null;
				return;
			}
			
			//show something while user wait patiently:
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען אירועים אנא המתן...");
			
			//get hebrew calendar:
			System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("he-IL");
			ci.DateTimeFormat.Calendar = hebrewCalendar;
			
			//get hebrew date:
			string ds = date.ToString("dddd dd בMMMM, yyyy", ci.DateTimeFormat);
			
			//append non hebrew date as well:
			ci.DateTimeFormat.Calendar = gregorianCalendar;
			ds += " - " + date.ToString("dd בMMMM, yyyy", ci.DateTimeFormat);
			
			//apply view title and page caption:
			Title = ds;
			labelDate.Text = ds;
			
			//get all events for this date...
			Sport.Championships.Event[] events=Sport.Championships.Events.GetDayEvents(date);
			
			//add each championship category as link:
			ArrayList champEvents=new ArrayList();
			int curIndex=0;
			Sport.Championships.Event lastEvent=null;
			for (int i=0; i<events.Length; i++)
			{
				//get current event:
				Sport.Championships.Event curEvent=events[i];
				
				//check if we moved to new championship category:
				if ((lastEvent != null)&&(lastEvent.ChampCategory.Id != curEvent.ChampCategory.Id))
				{
					//add all events to the same position under the last championship category:
					_links.Add(AddEventLabels(lastEvent, curIndex, champEvents));
					curIndex++;
					
					//reset list:
					champEvents = new ArrayList();
				}
				
				//add current event:
				champEvents.Add(curEvent);
				
				//maybe last?
				if (i == (events.Length-1))
					_links.Add(AddEventLabels(curEvent, curIndex, champEvents));
				
				//remember current event:
				lastEvent = curEvent;
			}
			
			//add the controls in reverse order:
			for (int i=_links.Count-1; i>=0; i--)
				linksPanel.Controls.Add(_links[i] as Control);
			
			//find maximum width of group boxes...
			int maxWidth=0;
			GroupBox[] arrBoxes=GetAllGroupBoxes(linksPanel);
			foreach (GroupBox groupBox in arrBoxes)
			{
				if (groupBox.Width > maxWidth)
					maxWidth = groupBox.Width;
			}
			
			//apply maximum width to all group boxes:
			foreach (GroupBox groupBox in arrBoxes)
				groupBox.Width = maxWidth;
			
			string strBlanks=MAIN_BLANKS;
			Label tmpLabel=new Label();
			tmpLabel.AutoSize = true;
			tmpLabel.Text = strBlanks;
			this.Controls.Add(tmpLabel);
			int panelWidth=2*tmpLabel.Width;
			this.Controls.Remove(tmpLabel);
			panelWidth += maxWidth+5;
			foreach (Control c1 in linksPanel.Controls)
			{
				if (c1 is Panel)
				{
					c1.Width = panelWidth;
					c1.Height += 3;
					foreach (Control c2 in c1.Controls)
					{
						if (c2 is Panel)
							c2.Width = panelWidth;
					}
				}
			}

			//done.
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion
		
		#region Open - inherited from View
		public override void Open()
		{
			if (State["date"] != null)
			{
				long ticks=Sport.Common.Tools.CLngDef(state["date"], 0);
				if (ticks > 0)
					SelectDate(new DateTime(ticks));
			}
			base.Open ();
		}
		#endregion
		#endregion
		
		#region Private Methods
		private GroupBox[] GetAllGroupBoxes(Control parent)
		{
			ArrayList result=new ArrayList();
			if (parent is GroupBox)
				result.Add(parent);
			foreach (Control control in parent.Controls)
			{
				if (control is GroupBox)
					result.Add(control);
				result.AddRange(GetAllGroupBoxes(control));
			}
			return (GroupBox[]) result.ToArray(typeof(GroupBox));
		}
		
		private Panel AddEventLabels(Sport.Championships.Event e, int index, ArrayList champEvents)
		{
			//sort events by place:
			SortByPlace(champEvents);
			
			LinkLabel linkLabel=new LinkLabel();
			linkLabel.Name = "linkLabel_"+(index+1);
			linkLabel.Dock = DockStyle.Top;
			linkLabel.RightToLeft = RightToLeft.Yes;
			linkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			string champName=e.ChampCategory.Championship.Name;
			string catName=e.ChampCategory.Name;
			string strGames="("+champEvents.Count+" "+((e.SportField == null)?"משחקים":"תחרויות")+")";
			linkLabel.Text = champName+" "+catName+" "+strGames;
			Sport.Common.Tools.AttachTooltip(linkLabel, "לחץ כדי לעבור למסך עריכת אליפות");
			linkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(ChampClicked);
			_links.Add(linkLabel);
			_events[index] = champEvents;
			
			//add each event...
			Panel champPanel = new Panel();
			champPanel.Dock = DockStyle.Right;
			champPanel.BackColor = Color.White;
			champPanel.BorderStyle = BorderStyle.FixedSingle;
			string strLastPhase=null; //phase and group
			string strLastPlace=null; //facility and court
			string strBlanks=MAIN_BLANKS;
			string strDelimeter="   ";
			GroupBox groupBox=null;
			Panel panel=null;
			ArrayList boxLabels=new ArrayList(); //holds the labels to be added to each group box
			foreach (Sport.Championships.Event champEvent in champEvents)
			{
				string strCurrentPhase=champEvent.GetPhaseAndGroup();
				string strCurrentPlace=champEvent.GetPlace();
				string strScore=champEvent.GetScore(true);
				if ((strLastPhase == null)||(strCurrentPhase != strLastPhase))
				{
					if (groupBox != null)
						AddGroupBoxLabels(boxLabels, groupBox, panel);
					Label label=new Label();
					label.Dock = DockStyle.Right;
					label.AutoSize = true;
					label.Text = strBlanks;
					panel = new Panel();
					panel.Dock = DockStyle.Top;
					groupBox = new GroupBox();
					groupBox.Dock = DockStyle.Right;
					//groupBox.BackColor = Color.White;
					groupBox.Text = strCurrentPhase;
					groupBox.RightToLeft = RightToLeft.Yes;
					panel.Controls.Add(groupBox);
					panel.Controls.Add(label);
					//_links.Add(panel);
					champPanel.Controls.Add(panel);
					boxLabels.Clear();
				}
				if ((strCurrentPhase != strLastPhase)||((strLastPlace == null)||(strCurrentPlace != strLastPlace)))
				{
					if (strCurrentPlace.Length > 0)
					{
						Label label=new Label();
						label.RightToLeft = RightToLeft.Yes;
						label.Font = new Font(this.Font, FontStyle.Underline|FontStyle.Bold);
						label.Text = strCurrentPlace;
						boxLabels.Add(label);
					}
				}
				boxLabels.Add(BuildEventLinks(champEvent, strDelimeter, strScore));
				strLastPlace = strCurrentPlace;
				strLastPhase = strCurrentPhase;
			}
			if (groupBox != null)
				AddGroupBoxLabels(boxLabels, groupBox, panel);
			
			//set height of result panel:
			int height=0;
			foreach (Panel childPanel in champPanel.Controls)
				height += childPanel.Height;
			champPanel.Height = height;
			
			Panel result=new Panel();
			result.Dock = DockStyle.Top;
			result.Height = champPanel.Height;
			result.Controls.Add(champPanel);
			return result;
		}
		
		private ArrayList BuildEventLinks(Sport.Championships.Event e, string delimeter,
			string score)
		{
			ArrayList result=new ArrayList();
			result.Add(Core.Tools.QuickLabel(MAIN_BLANKS+e.Date.ToString("HH:mm")+delimeter));
			if ((e.SportField == null)&&(e.TeamA != null)&&(e.TeamB != null))
			{
				Sport.Entities.Team teamA=e.TeamA;
				Sport.Entities.Team teamB=e.TeamB;
				result.Add(BuildTeamLink(teamA));
				result.Add(Core.Tools.QuickLabel("-"));
				result.Add(BuildTeamLink(teamB));
			}
			else
			{
				result.Add(Core.Tools.QuickLabel(e.GetTeamsOrCompetition()));
			}
			if (score.Length > 0)
				result.Add(Core.Tools.QuickLabel(delimeter+" ("+score+")"));
			return result;
		}

		private LinkLabel BuildTeamLink(Sport.Entities.Team team)
		{
			LinkLabel result=new LinkLabel();
			result.Name = "teamLink_"+team.Id.ToString();
			result.Text = " "+team.Name+" ";
			result.LinkColor = Color.Black;
			result.VisitedLinkColor = Color.Black;
			result.LinkClicked += new LinkLabelLinkClickedEventHandler(TeamClicked);
			return result;
		}

		private void AddGroupBoxLabels(ArrayList labels, GroupBox groupBox, Panel panel)
		{
			int height=0;
			int maxWidth=0;
			int labelHeight=0;
			
			//measure the height of the group box text and set as initial height:
			height = Core.Tools.MeasureText(this, groupBox.Text).Height;
			
			//sum the heights of the labels and find the maximum width:
			for (int i=0; i<labels.Count; i++)
			{
				if (labels[i] is ArrayList)
				{
					int maxHeight=0;
					int totWidth=0;
					foreach (Control control in (ArrayList) labels[i])
					{
						Size size=Core.Tools.MeasureText(this, control.Text);
						if (size.Height > maxHeight)
							maxHeight = size.Height;
						totWidth += size.Width;
					}
					height += maxHeight;
					if (totWidth > maxWidth)
						maxWidth = totWidth;
					if (labelHeight == 0)
						labelHeight = maxHeight;
				}
				else
				{
					Label label=(Label) labels[i];
					Size size=Core.Tools.MeasureText(this, label.Text);
					if (size.Width > maxWidth)
						maxWidth = size.Width;
					if (labelHeight == 0)
						labelHeight = size.Height;
					height += size.Height;
				}
			}
			
			//apply maximum label width as the group box width:
			groupBox.Width = maxWidth+10;
			
			//add the labels to the groupbox:
			labels.Reverse();
			for (int i=0; i<labels.Count; i++)
			{
				ArrayList arrLabels=new ArrayList();
				if (labels[i] is ArrayList)
				{
					arrLabels = (ArrayList) labels[i];
				}
				else
				{
					arrLabels.Add(labels[i]);
				}
				arrLabels.Reverse();
				groupBox.Controls.Add(BuildLinksPanel(arrLabels, maxWidth, labelHeight));
			}
			panel.Height = height+5; //+(labels.Count*6);
		}
		
		private Panel BuildLinksPanel(ArrayList labels, int width, int height)
		{
			Panel result=new Panel();
			result.Width = width;
			result.Height = height;
			result.Dock = DockStyle.Top;
			result.RightToLeft = RightToLeft.Yes;
			foreach (Control control in labels)
			{
				control.RightToLeft = RightToLeft.Yes;
				if (control is Label) 
				{
					(control as Label).AutoSize = true;
					(control as Label).TextAlign = ContentAlignment.MiddleLeft;
				}
				else if (control is LinkLabel)
				{
					(control as LinkLabel).AutoSize = true;
					(control as LinkLabel).TextAlign = ContentAlignment.MiddleLeft;
				}
				control.Dock = DockStyle.Right;
				result.Controls.Add(control);
			}
			return result;
		}

		private void SortByPlace(ArrayList eventsList)
		{
			eventsList.Sort(new EventPlaceComparer());
		}
		#endregion
		
		#region initialize component
		private void InitializeComponent()
		{
			this.labelDate = new System.Windows.Forms.Label();
			this.linksPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// labelDate
			// 
			this.labelDate.BackColor = System.Drawing.SystemColors.Highlight;
			this.labelDate.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelDate.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.labelDate.Location = new System.Drawing.Point(0, 0);
			this.labelDate.Name = "labelDate";
			this.labelDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelDate.Size = new System.Drawing.Size(1032, 17);
			this.labelDate.TabIndex = 0;
			// 
			// linksPanel
			// 
			this.linksPanel.AutoScroll = true;
			this.linksPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.linksPanel.Location = new System.Drawing.Point(0, 17);
			this.linksPanel.Name = "linksPanel";
			this.linksPanel.Size = new System.Drawing.Size(1032, 632);
			this.linksPanel.TabIndex = 1;
			// 
			// DayEventsView
			// 
			this.Controls.Add(this.linksPanel);
			this.Controls.Add(this.labelDate);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.Name = "DayEventsView";
			this.Size = new System.Drawing.Size(1032, 649);
			this.ResumeLayout(false);

		}
		#endregion
		
		#region link click
		private void ChampClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if ((sender == null)||(_events == null)||(!(sender is LinkLabel)))
				return;
			string strName=(sender as LinkLabel).Name;
			string[] arrTemp=strName.Split(new char[] {'_'});
			int index=Sport.Common.Tools.CIntDef(arrTemp[1], -1)-1;
			if (_events[index] == null)
				return;
			ArrayList arrEvents=(ArrayList) _events[index];
			Sport.Championships.Event ev=(Sport.Championships.Event) arrEvents[0];
			string strCategoryID=ev.ChampCategory.Id.ToString();
			if (ev.SportField == null)
			{
				Sport.UI.ViewManager.OpenView(typeof(Producer.MatchChampionshipEditorView), 
					Sport.Entities.ChampionshipCategory.TypeName + "=" + strCategoryID);
			}
			else
			{
				Sport.UI.ViewManager.OpenView(typeof(Producer.CompetitionChampionshipEditorView), 
					Sport.Entities.ChampionshipCategory.TypeName + "=" + strCategoryID);
			}
		}
		#endregion
		
		#region events place comparer
		private class EventPlaceComparer : IComparer
		{
			/// <summary>
			/// compare by round, group and place.
			/// </summary>
			public int Compare(object x, object y)
			{
				Sport.Championships.Event e1=(Sport.Championships.Event) x;
				Sport.Championships.Event e2=(Sport.Championships.Event) y;
				int phase1=e1.PhaseIndex;
				int phase2=e2.PhaseIndex;
				if (phase1 == phase2)
				{
					int group1=e1.GroupIndex;
					int group2=e2.GroupIndex;
					if (group1 == group2)
					{
						string place1=e1.GetPlace();
						string place2=e2.GetPlace();
						if (place1 == place2)
						{
							return e1.Date.CompareTo(e2.Date);
						}
						return place1.CompareTo(place2);
					}
					return group1.CompareTo(group2);
				}
				return phase1.CompareTo(phase2);
			}
		}
		#endregion

		private void TeamClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if ((sender == null)||(_events == null)||(!(sender is LinkLabel)))
				return;
			string strName=(sender as LinkLabel).Name;
			string[] arrTemp=strName.Split(new char[] {'_'});
			int teamID=Sport.Common.Tools.CIntDef(arrTemp[1], -1);
			if (teamID >= 0)
			{
				new Sport.UI.OpenDialogCommand().Execute("Sportsman.Details.TeamDetailsView,Sportsman?id=" + teamID.ToString());
			}
		}
	}
}
