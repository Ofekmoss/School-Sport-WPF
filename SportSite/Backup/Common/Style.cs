using System;
using System.Drawing;

namespace SportSite.Common
{
	/// <summary>
	/// hold static information about available css class names and general style.
	/// </summary>
	public class Style
	{
		//CSS classes:
		public static readonly string HeaderLinkCss="HeaderLink";
		public static readonly string MainPanelCss="MainPanel";
		public static readonly string DatePanelCss="DatePanel";
		public static readonly string NavBarCaptionCss="NavBarCaption";
		public static readonly string NavBarLinkCss="NavBarLink";
		public static readonly string LeftNavBarLinkCss="LeftNavBarLink";
		public static readonly string ExtraDetailsCss="ExtraDetailsTable";
		public static readonly string SportLinksCss="SportLinks";
		public static readonly string DetailsLinkCss="DetailsLink";
		public static readonly string PendingTeamCss="PendingTeam";
		public static readonly string PlayersTableCss="PlayersTable";
		public static readonly string HotLinkCss="HotLink";
		
		//background colors:
		public static readonly Color HeaderBgColor=Color.FromArgb(0, 0, 153);
		public static readonly Color NavBarBgColor=Color.FromArgb(250, 218, 39); //#FADA27
		public static readonly Color LeftNavBarBgColor=Color.FromArgb(223, 48, 55);
		public static readonly Color ExtraDetailsBgColor=Color.FromArgb(220, 220, 220);
		public static readonly Color ActiveLinkColor=Color.FromArgb(198, 200, 0);
		public static readonly Color PrimaryArticleBgcolor=Color.FromArgb(229, 255, 229);
		public static readonly Color SubArticleBgcolor=Color.FromArgb(255, 229, 255);
		public static readonly Color HotArticleBgcolor=Color.FromArgb(255, 205, 205);
		public static readonly Color ZooZooArticleBgcolor = Color.FromArgb(205, 205, 255);
		public static readonly Color LinkFieldsetBgColor=Color.FromArgb(255, 255, 235);
		public static readonly Color AttachmentFieldsetBgColor=Color.FromArgb(235, 255, 235);
		public static readonly Color HotLinkBgColor=Color.FromArgb(200, 0, 0);
		public static readonly Color DynamicPageBgColor=Color.FromArgb(240, 240, 240);
		public static readonly Color PermanentChampBgColor=Color.FromArgb(255, 255, 255);

		//registration colors:
		public static readonly Color[] PendingTeamsColors={
			Color.FromArgb(165, 216, 255), Color.FromArgb(246, 194, 196)};
		public static readonly Color[] PendingPlayersColors={
			Color.FromArgb(185, 236, 255), Color.FromArgb(246, 194, 196)};
		public static readonly Color[] ChoosePlayerColors={
			Color.FromArgb(165, 216, 255), Color.FromArgb(246, 194, 196)};
		public static Color PlayerSelectHighlight=Color.FromArgb(255, 255, 0);		
		
		//elements dimensions:
		public static readonly int LinkFieldsetWidth=480;
		public static readonly int AttachmentFieldsetWidth=380;
		
		//table view:
		public static readonly string TableViewTableCss="TableViewTable";
		public static readonly string TableViewCaptionCss="TableViewCaption";
		public static readonly string TableViewHeaderCss="TableViewHeader";
		public static readonly string TableViewCellCss="TableViewCell";
		
		//general:
		public static readonly int NavBarAddLight=30;
		public static readonly int EmptyLinkGap=12; //pixels
		public static readonly string DefaultFontFamily="Arial"; //"Tahoma";
		public static readonly string HotLinkFontColor="white";
		public static readonly int HotLinkAddLight=60;
	}
}
