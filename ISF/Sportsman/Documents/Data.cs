using System;

namespace Sportsman.Documents
{
	#region enumerations
	public enum ChampionshipDocumentType
	{
		Undefined=-1,
		PlayersReport=0,
		TeamsReport,
		RefereesReport,
		RefereePaymentReport,
		ClubReport,
		AdministrationReport,
		OtherSportsReport //אירועי ספורט
	}
	
	public enum CompetitionReportType
	{
		Undefined=-1,
		CompetitionCompetitorsReport=0,
		CompetitorVoucher,
		GroupTeamsReport,
		TeamVoucher_School,
		TeamVoucher_Student,
		RefereeReport,
		TeamFullReport,
		MultiCompetitionReport,
		ClubCompetitionsReport,
		TeamCompetitorsReport
	}
	#endregion
}
