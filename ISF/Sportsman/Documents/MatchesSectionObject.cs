using System;
using System.Linq;
using System.Drawing;
using Sport.Documents;
using Sportsman.Producer;
using System.Collections.Generic;

namespace Sportsman.Documents
{
	public class MatchesSectionObject : TableTreeSectionObject
	{
		private TableItem baseTable;

		private bool _groupGroups;
		private bool _groupRounds;
		private bool _groupCycles;
		private bool _groupTournaments;
		private int _selectedPhase;
		private int _selectedGroup;
		private int _selectedRound;
		private int _selectedCycle;

		private Font _groupFont;
		private Font _cycleFont;

		private MatchVisualizer.MatchField[] _fields;

		private MatchVisualizer visualizer;

		public MatchesSectionObject(
			Sport.Championships.MatchChampionship championship, 
			MatchVisualizer.MatchField[] fields, bool groupGroups, bool groupRounds, 
			bool groupCycles, bool groupTournaments, int selectedPhase, 
			int selectedGroup, int selectedRound, int selectedCycle)
		{
			_groupGroups = groupGroups;
			_groupRounds = groupRounds;
			_groupCycles = groupCycles;
			_groupTournaments = groupTournaments;
			_selectedPhase = selectedPhase;
			_selectedGroup = selectedGroup;
			_selectedRound = selectedRound;
			_selectedCycle = selectedCycle;
			
			_fields = fields;
			
			System.Collections.ArrayList gf = new System.Collections.ArrayList();
			for (int n = 0; n < fields.Length; n++)
			{
				if (fields[n] <= MatchVisualizer.MatchField.Tournament)
					gf.Add(n);
			}

			GroupFields = (int[]) gf.ToArray(typeof(int));

			_cycleFont = new System.Drawing.Font("Tahoma", 
				16, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			_groupFont = new System.Drawing.Font("Tahoma", 
				20, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);

			baseTable = new TableItem();
			baseTable.RelativeColumnWidth = true;

			visualizer = new MatchVisualizer();

			TableItem.TableColumn tc;

			TableItem.TableCell[] header = new TableItem.TableCell[fields.Length];
			for (int n = 0; n < fields.Length; n++)
			{
				tc = new TableItem.TableColumn();
				Sport.UI.IVisualizerField visualField = visualizer.GetField((int) fields[n]);
				tc.Width = visualField.DefaultWidth;
				tc.Alignment = (TextAlignment) visualField.Alignment;
				baseTable.Columns.Add(tc);

				header[n] = new TableItem.TableCell(visualField.Title);
				header[n].Border = SystemPens.WindowFrame;
			}

			baseTable.Border = SystemPens.WindowFrame;

			baseTable.Bounds = Bounds;

			TableItem.TableRow headerRow = new TableItem.TableRow(header);
			headerRow.BackColor = System.Drawing.Color.SkyBlue;
			headerRow.Font = new System.Drawing.Font("Tahoma", 12, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			headerRow.Alignment = Sport.Documents.TextAlignment.Center;
			headerRow.LineAlignment = Sport.Documents.TextAlignment.Center;

			baseTable.Rows.Add(headerRow);

			BaseTable = baseTable;

			foreach (Sport.Championships.MatchPhase phase in championship.Phases)
			{
				if ((_selectedPhase >= 0)&&(_selectedPhase != phase.Index))
					continue;
				AddPhase(phase);
			}
		}

		private void AddPhase(Sport.Championships.MatchPhase phase)
		{
			TableTreeNode node = new TableTreeNode(_groupFont, phase.Name, phase.Name + " (המשך)", true);
			Nodes.Add(node);
			
			foreach (Sport.Championships.MatchGroup group in phase.Groups)
			{
				if ((_selectedGroup >= 0)&&(_selectedGroup != group.Index))
					continue;
				AddGroup(group, node);
			}
		}

		private void AddGroup(Sport.Championships.MatchGroup group, TableTreeNode parent)
		{
			if (_groupGroups)
			{
				TableTreeNode node = new TableTreeNode(_groupFont, group.Name, group.Name + " (המשך)");
				parent.Children.Add(node);
				parent = node;
			}

			foreach (Sport.Championships.Round round in group.Rounds)
			{
				if ((_selectedRound >= 0)&&(_selectedRound != round.Index))
					continue;
				AddRound(round, parent);
			}
		}

		private void AddRound(Sport.Championships.Round round, TableTreeNode parent)
		{
			if (_groupRounds)
			{
				TableTreeNode node = new TableTreeNode(_cycleFont, round.Name, round.Name + " (המשך)");
				parent.Children.Add(node);
				parent = node;
			}

			foreach (Sport.Championships.Cycle cycle in round.Cycles)
			{
				if ((_selectedCycle >= 0)&&(_selectedCycle != cycle.Index))
					continue;
				AddCycle(cycle, parent);
			}
		}

		private void AddCycle(Sport.Championships.Cycle cycle, TableTreeNode parent)
		{
			if (_groupCycles)
			{
				TableTreeNode node = new TableTreeNode(_cycleFont, cycle.Name, cycle.Name + " (המשך)");
				parent.Children.Add(node);
				parent = node;
			}
			
			if (_groupTournaments)
			{
				bool addNoTournament = cycle.Matches.OfType<Sport.Championships.Match>().ToList().Exists(m => m.Tournament == -1);
				if (addNoTournament)
					AddTournament(cycle, -1, parent);

				foreach (Sport.Championships.Tournament tournament in cycle.Tournaments)
					AddTournament(cycle, tournament.Index, parent);
			}
			else
			{
				AddTournament(cycle, -1, parent, true);
			}
		}

		private void AddTournament(Sport.Championships.Cycle cycle, int tournament, TableTreeNode parent, bool addAllMatches)
		{
			if (_groupTournaments)
			{
				string tournamentName = tournament == -1 ? "ללא טורניר" : "טורניר " + cycle.Tournaments[tournament].Number.ToString();
				TableTreeNode node = new TableTreeNode(_cycleFont, tournamentName, tournamentName + " (המשך)");
				parent.Children.Add(node);
				parent = node;
			}
			
			List<Sport.Championships.Match> matches = cycle.Matches.OfType<Sport.Championships.Match>().ToList();
			if (!addAllMatches)
				matches.RemoveAll(m => m.Tournament != tournament);

			bool first = true;
			matches.ForEach(match =>
			{
				string[] record = new string[_fields.Length];
				for (int n = 0; n < _fields.Length; n++)
					record[n] = visualizer.GetText(match, _fields[n]);
				
				parent.Rows.Add(new TableTreeRow(record, first));
				if (first)
					first = false;
			});
		}

		private void AddTournament(Sport.Championships.Cycle cycle, int tournament, TableTreeNode parent)
		{
			AddTournament(cycle, tournament, parent, false);
		}

		protected override void Dispose(bool disposing)
		{
			baseTable.Dispose();
			base.Dispose (disposing);
		}
	}
}
