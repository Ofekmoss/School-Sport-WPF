using System;
using Sportsman.Views;
using System.Windows.Forms;
using Sport.UI;
using Sport.UI.Controls;
using Sport.UI.Dialogs;

namespace Sportsman.Producer
{
	public class OpenChampionshipCommand : Sport.UI.Command
	{
		private static GenericEditDialog _dialog;
		private static Sport.Championships.Championship[] _championships=null;
		
		public static Sport.Entities.ChampionshipCategory SelectChampionshipCategory(
			Sport.Entities.Region region, Sport.Entities.Sport sport, 
			Sport.Entities.Championship championship, 
			Sport.Entities.ChampionshipCategory category)
		{
			if (region == null)
			{
				_dialog.Items[0].Value = Sport.Entities.Region.Type.Lookup(Sport.Core.Session.Region);
			}
			else
			{
				_dialog.Items[0].Value = region.Entity;
			}

			_dialog.Items[1].Value = sport == null ? null : sport.Entity;
			SportChanged(null, EventArgs.Empty);

			if (sport != null)
			{
				_dialog.Items[2].Value = championship == null ? null : championship.Entity;
				ChampionshipChanged(null, EventArgs.Empty);

				if (championship != null)
				{
					_dialog.Items[3].Value = category == null ? null : category.Entity;
					CategoryChanged(null, EventArgs.Empty);
				}
			}

			if (_dialog.ShowDialog() == DialogResult.OK)
			{
				if (_dialog.Items[3].Value != null)
					return new Sport.Entities.ChampionshipCategory((Sport.Data.Entity) _dialog.Items[3].Value);
			}

			return null;
		}
	
		public static int OfflineChampionshipsCount
		{
			get { return (_championships == null)?0:_championships.Length; }
		}
		
		public static int[] GetOfflineChamionships(bool blnResultsOnly, 
			ref int recentIndex, ref string[] filesPath)
		{
			//initialize result:
			System.Collections.ArrayList result=
				new System.Collections.ArrayList();
			
			//get all cache files:
			string[] arrFileNames=null;
			try
			{
				arrFileNames = System.IO.Directory.GetFiles(
					Sport.Core.Session.GetSeasonCache(false));
			}
			catch
			{}
			
			//build array of the files path:
			System.Collections.ArrayList arrOfflineFiles=
				new System.Collections.ArrayList();
			
			//got anything?
			if (arrFileNames != null)
			{
				//what are we looking for?
				string strLookFor="championship"+((blnResultsOnly)?"_":"");
				DateTime maxDate=DateTime.MinValue;
				
				//iterare through offline files, look for desired name.
				for (int i=0; i<arrFileNames.Length; i++)
				{
					//get file path:
					string strFilePath=arrFileNames[i];
					
					//get name and extension:
					string strName      = System.IO.Path.GetFileNameWithoutExtension(strFilePath).ToLower();
					string strExtension = System.IO.Path.GetExtension(strFilePath).ToLower();
					
					//valid name?
					if (strName.Length <= strLookFor.Length)
						continue;
					
					//starting with what we're looking for?
					if (!strName.StartsWith(strLookFor))
						continue;
					
					//correct extension?
					if (strExtension.IndexOf("xml") < 0)
						continue;
					
					//make sure the next character is digit:
					if (!Sport.Common.Tools.IsDigit(strName[strLookFor.Length]))
						continue;
					
					//grab ID:
					int categoryID=Sport.Common.Tools.CIntDef(strName.Substring(
						strLookFor.Length, strName.Length-strLookFor.Length), -1);
					
					//got anything?
					if (categoryID < 0)
						continue;
					
					//add to result:
					result.Add(categoryID);
					arrOfflineFiles.Add(strFilePath);
					
					//maximum date?
					DateTime curDate=System.IO.File.GetLastWriteTime(strFilePath);
					if (curDate > maxDate)
					{
						maxDate = curDate;
						recentIndex = result.Count-1;
					}
				} //end loop over files
			} //end if got any files
			
			//done.
			filesPath = (string[]) arrOfflineFiles.ToArray(typeof(string));
			return (int[]) result.ToArray(typeof(int));
		} //end function GetOfflineChamionships
		
		public static int[] GetOfflineChamionships(bool blnResultsOnly, 
			ref int recentIndex)
		{
			string[] dummy=null;
			return GetOfflineChamionships(blnResultsOnly, ref recentIndex, ref dummy);
		}
		
		private static void LoadOfflineChampionships(ref int recentChampIndex)
		{
			//initialize array of championships:
			System.Collections.ArrayList arrChampionships=new System.Collections.ArrayList();
			
			//get ID of the offline championships:
			int[] arrCategoriesID=GetOfflineChamionships(false, ref recentChampIndex);
			
			//iterate over the categories, build championship for each:
			foreach (int categoryID in arrCategoriesID)
			{
				//initialize current championship and try to load:
				Sport.Championships.Championship championship=null;
				try
				{
					championship = Sport.Championships.Championship.GetChampionship(categoryID);
				}
				catch (Exception ex)
				{
					Sport.UI.MessageBox.Error("שגיאה בעת טעינת אליפות "+
						categoryID+"\n"+ex.Message+"\n"+ex.StackTrace, 
						"טעינת אליפויות");
				}
				
				//add to result if we got anything:
				if (championship != null)
					arrChampionships.Add(championship);
			} //end loop over offline categories
			
			//got anything?
			if (arrChampionships.Count == 0)
			{
				Sport.UI.MessageBox.Error("לא נמצאו אליפויות על הדיסק", "שגיאת מערכת");
				return;
			}
			
			//apply global array:
			_championships = (Sport.Championships.Championship[])
				arrChampionships.ToArray(typeof(Sport.Championships.Championship));
		} //end function LoadOfflineChampionships
		
		static OpenChampionshipCommand()
		{
			//initialize local variables:
			_championships = null;
			object[] regions=null;
			object[] sports=null;
			object selRegion=null;
			_dialog = new GenericEditDialog("בחר אליפות");
			
			//connected?
			int recentChampIndex=-1;
			if (!Sport.Core.Session.Connected)
			{
				Sport.UI.Dialogs.WaitForm.ShowWait("אנא המתן טוען רשימת אליפויות");
				LoadOfflineChampionships(ref recentChampIndex);
				Sport.UI.Dialogs.WaitForm.HideWait();
				
				//got anything?
				if ((_championships == null)||(_championships.Length == 0))
					return;
				
				Sport.Common.ListItem[] champItems=new Sport.Common.ListItem[_championships.Length];
				for (int i=0; i<_championships.Length; i++)
					champItems[i] = new Sport.Common.ListItem(_championships[i].Name, _championships[i].CategoryID);
				
				if ((recentChampIndex < 0)||(recentChampIndex >= champItems.Length))
					recentChampIndex = 0;
				
				_dialog.Items.Add(new GenericItem("אליפות", GenericItemType.Selection, 
					champItems[recentChampIndex], champItems));
				_dialog.Confirmable = true;
				
				_dialog.Items[0].ValueChanged += new EventHandler(ChampionshipChanged);
				_dialog.Items[0].Nullable = false;
				
				return;
			}
			
			//get all regions:
			regions = Sport.Entities.Region.Type.GetEntities(null);
			selRegion = new Sport.Entities.Region(Sport.Core.Session.Region).Entity;
			
			//get all sports:
			sports = Sport.Entities.Sport.Type.GetEntities(null);
			
			_dialog.Items.Add(new GenericItem("מחוז", GenericItemType.Selection, selRegion, regions));
			_dialog.Items.Add(new GenericItem("ענף ספורט", GenericItemType.Selection, null, sports));
			_dialog.Items.Add(new GenericItem("אליפות", GenericItemType.Selection));
			_dialog.Items.Add(new GenericItem("קטגוריה", GenericItemType.Selection));
			_dialog.Confirmable = false;
			
			_dialog.Items[0].ValueChanged += new EventHandler(RegionChanged);
			_dialog.Items[1].ValueChanged += new EventHandler(SportChanged);
			_dialog.Items[2].ValueChanged += new EventHandler(ChampionshipChanged);
			_dialog.Items[3].ValueChanged += new EventHandler(CategoryChanged);
			
			_dialog.Items[0].Nullable = false;
			_dialog.Items[1].Nullable = false;
			_dialog.Items[2].Nullable = false;
			_dialog.Items[3].Nullable = false;
		}

		private void GetRecentChampionship(out Sport.Entities.Region region,
            out Sport.Entities.Sport sport,
			out Sport.Entities.Championship championship,
			out Sport.Entities.ChampionshipCategory category)
		{
			region = null;
			sport = null;
			category = null;
			championship = null;

			//check if we have something stored:
			string str = Sport.Core.Configuration.ReadString("RecentChampionship", "Region");
			if (str != null)
			{
				region = new Sport.Entities.Region(Int32.Parse(str));
				if (!region.IsValid())
					region = null;
			}

			str = Sport.Core.Configuration.ReadString("RecentChampionship", "Sport");
			if (str != null)
			{
				sport = new Sport.Entities.Sport(Int32.Parse(str));
				if (!sport.IsValid())
				{
					sport = null;
				}
				else
				{
					str = Sport.Core.Configuration.ReadString("RecentChampionship", "Championship");

					if (str != null)
					{
						championship = new Sport.Entities.Championship(Int32.Parse(str));
						if (!championship.IsValid())
						{
							championship = null;
						}
						else
						{
							str = Sport.Core.Configuration.ReadString("RecentChampionship", "Category");

							if (str != null)
							{
								category = new Sport.Entities.ChampionshipCategory(Int32.Parse(str));
								if (!category.IsValid())
									category = null;
							}
						}
					}
				}
			}
		}

		public override void Execute(string param)
		{
			//initialize championship:
			Sport.Championships.Championship championship=null;
			int categoryID=-1;
			
			//connected?
			if (!Sport.Core.Session.Connected)
			{
				if (_dialog.ShowDialog() == DialogResult.OK)
				{
					if (_dialog.Items[0].Value != null)
					{
						Sport.Common.ListItem item=(Sport.Common.ListItem) _dialog.Items[0].Value;
						categoryID = (int) item.Value;
						Sport.UI.Dialogs.WaitForm.ShowWait("טוען אליפות '" + item.Text + "'...", true);
						try
						{
							championship = Sport.Championships.Championship.GetChampionship(categoryID);
						}
						catch (Exception e)
						{
							Sport.UI.Dialogs.WaitForm.HideWait();
							Sport.UI.MessageBox.Error("טעינת אליפות נכשלה\n"+e.Message+"\n"+e.StackTrace, "טעינת אליפות");
							return;
						}
						Sport.UI.Dialogs.WaitForm.HideWait();
					}
				}
			} //end if not connected
			else
			{
				Sport.Entities.Region region = null;
				Sport.Entities.Sport sport = null;
				Sport.Entities.ChampionshipCategory category = null;
				Sport.Entities.Championship champ = null;
				GetRecentChampionship(out region, out sport, out champ, out category);
				
				category = SelectChampionshipCategory(region, sport, champ, category);
				
				if (category != null)
				{
					//create proper entity:
					Sport.UI.Dialogs.WaitForm.ShowWait("טוען אליפות '" + category.Championship.Name + " - " + category.Name + "'...", true);
					
					//load the championship from database or memory:
					try
					{
						championship = Sport.Championships.Championship.GetChampionship(category.Id);
					}
					catch (Exception e)
					{
						//delete dat files and try again:
						System.Diagnostics.Debug.WriteLine("failed to load championship: "+e.Message);
						System.Diagnostics.Debug.WriteLine(e.StackTrace);
						System.Diagnostics.Debug.WriteLine("deleting dat files and trying again...");
						Sport.Entities.Sport.Type.DeleteDatFile();
						Sport.Entities.ChampionshipCategory.Type.DeleteDatFile();
						Sport.Entities.Championship.Type.DeleteDatFile();
						Sport.Entities.Ruleset.Type.DeleteDatFile();
						Sport.Entities.StandardChampionship.Type.DeleteDatFile();
						Sport.Entities.StandardChampionshipCategory.Type.DeleteDatFile();
						try
						{
							championship = Sport.Championships.Championship.GetChampionship(category.Id);
						}
						catch (Exception e2)
						{
							System.Diagnostics.Debug.WriteLine("failed to load championship: "+e2.Message);
							Sport.UI.Dialogs.WaitForm.HideWait();
							Sport.UI.MessageBox.Show("טעינת אליפות נכשלה\nאנא דווח על השגיאה במסך הערות תוך פירוט פעולות אחרונות\nשגיאה: "+
								e2.Message+"\n"+e2.StackTrace, "טעינת אליפות", MessageBoxIcon.Error);
							return;
						}
					}
					Sport.UI.Dialogs.WaitForm.HideWait();
					categoryID = championship.ChampionshipCategory.Id;
				}
			} //end if connected
			
			//got anything?
			if (championship == null)
				return;
			
			//getting here means we have valid championship. check type:
			if (championship is Sport.Championships.MatchChampionship)
			{
				ViewManager.OpenView(typeof(MatchChampionshipEditorView), 
					"championshipcategory=" + categoryID.ToString());
			}
			else
			{
				ViewManager.OpenView(typeof(CompetitionChampionshipEditorView), 
					"championshipcategory=" + categoryID.ToString());
			}
			
			//store last choice:
			try
			{
				if (Sport.Core.Session.Connected)
				{
					Sport.Core.Configuration.WriteString("RecentChampionship", "Region", (_dialog.Items[0].Value as Sport.Data.Entity).Id.ToString());
					Sport.Core.Configuration.WriteString("RecentChampionship", "Sport", (_dialog.Items[1].Value as Sport.Data.Entity).Id.ToString());
					Sport.Core.Configuration.WriteString("RecentChampionship", "Championship", (_dialog.Items[2].Value as Sport.Data.Entity).Id.ToString());
					Sport.Core.Configuration.WriteString("RecentChampionship", "Category", (_dialog.Items[3].Value as Sport.Data.Entity).Id.ToString());
				}
				else
				{
					Sport.Core.Configuration.WriteString("RecentChampionship", "Category", categoryID.ToString());
				}
			}
			catch
			{
				System.Diagnostics.Debug.WriteLine("failed to write recent championship choice to ini file.");
			}
		}


		private static void ResetChampionships()
		{
			_dialog.Confirmable = false;

			_dialog.Items[2].Value = null;
			_dialog.Items[3].Value = null;
			_dialog.Items[3].Values = null;

		
			Sport.Data.Entity region = _dialog.Items[0].Value as Sport.Data.Entity;
			Sport.Data.Entity sport = _dialog.Items[1].Value as Sport.Data.Entity;

			if (sport == null || region == null)
			{
				_dialog.Items[2].Values = null;
			}
			else
			{
				Sport.Data.EntityFilter filter=new Sport.Data.EntityFilter(
					(int) Sport.Entities.Championship.Fields.Sport, sport.Id);
				filter.Add(new Sport.Data.EntityFilterField((int) Sport.Entities.Championship.Fields.Region, region.Id));
				filter.Add(Sport.Entities.Championship.CurrentSeasonFilter());
				Sport.Data.Entity[] championships = 
					Sport.Entities.Championship.Type.GetEntities(filter);
				_dialog.Items[2].Values = championships;
			}
		}

		private static void RegionChanged(object sender, EventArgs e)
		{
			ResetChampionships();
		}

		private static void SportChanged(object sender, EventArgs e)
		{
			ResetChampionships();
		}

		private static void ChampionshipChanged(object sender, EventArgs e)
		{
			if (!Sport.Core.Session.Connected)
			{
				_dialog.Confirmable = (_dialog.Items[0].Value != null);
				return;
			}
			
			_dialog.Confirmable = false;

			_dialog.Items[3].Value = null;

			Sport.Data.Entity championship = _dialog.Items[2].Value as Sport.Data.Entity;

			if (championship == null)
			{
				_dialog.Items[3].Values = null;
			}
			else
			{
				Sport.Data.Entity[] categories = Sport.Entities.ChampionshipCategory.Type.GetEntities(
					new Sport.Data.EntityFilter((int) Sport.Entities.ChampionshipCategory.Fields.Championship, (int) championship.Id));
				_dialog.Items[3].Values = categories;
			}
		}

		private static void CategoryChanged(object sender, EventArgs e)
		{
			_dialog.Confirmable = _dialog.Items[3].Value != null;
		}

		public override bool IsPermitted(string param)
		{
			return Sport.UI.View.IsViewPermitted(typeof(Views.ChampionshipsTableView));
		}

	}
}
