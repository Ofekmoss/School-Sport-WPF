function ApplyTabFilters(objCombo)
{
	var selPhase = parseInt(document.forms[0].elements["phase"].value);
	var selGroup = parseInt(document.forms[0].elements["group"].value);
	var selCompetition = parseInt(document.forms[0].elements["competition"].value);
	var selTeam = parseInt(document.forms[0].elements["team"].value);
	SetTabStatus(0, (selPhase >= 0 && selGroup >= 0));
	SetTabStatus(1, (selCompetition >= 0));
	SetTabStatus(2, (selTeam >= 0));
	
	if (typeof objCombo != "undefined")
	{
		var strComboName = objCombo.name;
		if (strComboName == "phase" || strComboName == "group")
		{
			RefreshCurrentTab();
		}
		else
		{
			if (strComboName == "competition" && _currentTabIndex == 1)
			{
				RefreshCurrentTab();
			}
			else
			{
				if (strComboName == "team" && _currentTabIndex == 2)
				{
					RefreshCurrentTab();
				}
			}
		}
	}
}
