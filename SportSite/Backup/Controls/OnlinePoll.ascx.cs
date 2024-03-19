namespace SportSite.Controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Collections;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for OnlinePoll.
	/// </summary>
	public class OnlinePoll : System.Web.UI.UserControl
	{
	   	protected Panel pnOnlinePollPanel;
		protected Label lbPollQuestion;
		protected Table tbPollAnswers;
		protected ImageButton btnPollVote;
		private RadioButton[] answersButtons;
		private WebSiteServices.PollAnswerData[] latestPollAnswers;
		private bool _showResults=false;
		
		public bool ShowResults
		{
			get {return _showResults;}
			set {_showResults = value;}
		}
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Initialization
			pnOnlinePollPanel.Attributes["dir"] = "rtl";
			
			//showLatestPollResults();
			defaultToLatestPoll();
			
			btnPollVote.ImageUrl = Common.Data.AppPath+"/Images/btn_send.gif";
			btnPollVote.Click += new System.Web.UI.ImageClickEventHandler(btnPollVote_Click);
		}
		
		private WebSiteServices.PollData EmptyPollData
		{
			get 
			{ 
				WebSiteServices.PollData result;
				result = new WebSiteServices.PollData();
				result.ID = -1;
				result.question = null;
				result.creationDate = DateTime.MinValue;
				result.experationDate = DateTime.MinValue;
				result.possibleAnswers = null;
				result.creator = -1;
				return result;
			}
		}
		

		/// <summary>
		/// Gets the valid poll with the latest creation date.
		/// </summary>
		/// <returns></returns>
		private WebSiteServices.PollData getLatestPoll()
		{
			// Get latest poll
			object data = null;
			WebSiteServices.PollData latestPoll = EmptyPollData;
			if (CacheStore.Instance.Get("LatestPoll", out data))
			{
				latestPoll = (WebSiteServices.PollData)data;
			}
			else
			{
				WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService();
				WebSiteServices.PollData[] pollsList = service.getPollsByFilter(WebSiteServices.PollReturnFilter.Latest);
				if (pollsList.Length > 0)
					latestPoll = pollsList[0];
				CacheStore.Instance.Update("LatestPoll", latestPoll, 5);
			}
			return latestPoll;
		}
		
		/// <summary>
		/// Gets the latest poll from the database and defaults the OnlinePoll
		/// control to that poll.
		/// </summary>
		private void defaultToLatestPoll()
		{
			WebSiteServices.PollData latestPoll = getLatestPoll();
			if (latestPoll.ID < 0)
			{
				this.Visible = false;
				return;
			}
			if (_showResults)
			{
				showLatestPollResults();
				return;
			}
			latestPollAnswers = latestPoll.possibleAnswers;
			
			// Set the question
			string strQuestion="";
			if (_showResults == false)
				strQuestion += "<a href=\""+Common.Data.AppPath+"/?action="+Common.SportSiteAction.ShowPollResults+"\">";
			strQuestion += latestPoll.question;
			if (_showResults == false)
				strQuestion += "</a>";
			lbPollQuestion.Text = strQuestion;
			
			// Set the answers
			ArrayList buttonList = new ArrayList();
			RadioButton currentButton;
			TableRow currentRow;
			TableCell currentCell;
			foreach (WebSiteServices.PollAnswerData currentAnswer in latestPollAnswers)
			{
				currentButton = new RadioButton();
				currentButton.GroupName = "answers";
				currentButton.ID = currentAnswer.ID.ToString();
				currentButton.Text = currentAnswer.answer;
				currentRow = new TableRow();
				currentCell = new TableCell();
				currentCell.Controls.Add(currentButton);
				buttonList.Add(currentButton);
				currentRow.Cells.Add(currentCell);
				tbPollAnswers.Rows.Add(currentRow);
			}
			answersButtons = (RadioButton[])(buttonList.ToArray(typeof(RadioButton)));
		}
		
		/// <summary>
		/// Displays the results for the latest poll using progress bars
		/// </summary>
		private void showLatestPollResults()
		{
			btnPollVote.Visible = false;
			
			WebSiteServices.PollData latestPoll = getLatestPoll();
			if (latestPoll.ID < 0)
			{
				this.Visible = false;
				return;
			}
			latestPollAnswers = latestPoll.possibleAnswers;
			
			// Set the question
			lbPollQuestion.CssClass = "PollQuestionBig";
			lbPollQuestion.Text = "<center>"+latestPoll.question+"</center>";
			
			// Count all the results
			int totalVoters = 0;
			foreach (WebSiteServices.PollAnswerData currentAnswer in latestPollAnswers)
			{
				totalVoters = totalVoters + currentAnswer.results.Length;
			}
			
			TableRow currentRow;
			TableCell currentCell;
			
			double[] arrPercentages=new double[latestPollAnswers.Length];
			string[] arrColors = new string[] {"#CC0000", "#1479C0",  "#FFCC00",    "#FF9900",    "#009900"};
			string[] arrImages = new string[] {"red.gif", "blue.gif", "yellow.gif", "orange.gif", "green.gif"};
			double maxPercentage=0;
			for (int i=0; i<latestPollAnswers.Length; i++)
			{
				double percentage=0;
				if (totalVoters > 0)
					percentage = (((double) latestPollAnswers[i].results.Length)/((double) totalVoters));
				arrPercentages[i] = percentage;
				if (percentage > maxPercentage)
					maxPercentage = percentage;
			}
			
			int textHeight=30;
			int totalBarHeight=300;
			int cellHeight=textHeight+((int) (maxPercentage*totalBarHeight));
			currentRow = new TableRow();
			currentCell = new TableCell();
			currentCell.Style["height"] = cellHeight+"px";
			currentCell.Style["width"] = "10px";
			currentCell.Text = "&nbsp;";
			currentRow.Cells.Add(currentCell);
			for (int i=0; i<arrPercentages.Length; i++)
			{
				int curPercentage=(int) ((arrPercentages[i]+0.005)*100);
				int barHeight=(int) (totalBarHeight*arrPercentages[i]);
				currentCell = new TableCell();
				currentCell.Style["vertical-align"] = "bottom";
				currentCell.Style["height"] = cellHeight+"px";
				currentCell.CssClass = "PollAnswerCell";
				currentCell.Text = curPercentage+"%";
				currentCell.Text += "<br />";
				if (arrPercentages[i] > 0)
				{
					string strContainerID="poll_result_"+(i+1);
					string strBgColor=arrColors[i % arrColors.Length];
					string strImageName=Common.Data.AppPath+"/Images/seker_"+
						arrImages[i % arrImages.Length];
					currentCell.Text += "<div class=\"poll_result\" "+
						"id=\""+strContainerID+"\" "+
						"style=\"position: relative; width: 73px; height: "+barHeight+"px; "+
						"background-color: "+strBgColor+";\">"+
						"<div style=\"position: absolute; left: 0px; top: 0px; "+
						"width: 73px; height: 16px; background-repeat: no-repeat; "+
						"background-image: url("+strImageName+");\">&nbsp;</div></div>";
				}
				currentRow.Cells.Add(currentCell);
				currentCell = new TableCell();
				currentCell.Style["height"] = cellHeight+"px";
				currentCell.Style["width"] = "35px";
				currentCell.Text = "&nbsp;";
				currentRow.Cells.Add(currentCell);
			}
			tbPollAnswers.Rows.Add(currentRow);
			
			int colCount=currentRow.Cells.Count;
			
			currentRow = new TableRow();
			currentCell = new TableCell();
			currentCell.CssClass = "PollBottomSeperator";
			currentCell.ColumnSpan = colCount;
			currentCell.Text = "&nbsp;";
			currentRow.Cells.Add(currentCell);
			tbPollAnswers.Rows.Add(currentRow);
			
			currentRow = new TableRow();
			currentCell = new TableCell();
			currentCell.Style["width"] = "10px";
			currentCell.Text = "&nbsp;";
			currentRow.Cells.Add(currentCell);
			for (int i=0; i<latestPollAnswers.Length; i++)
			{
				currentCell = new TableCell();
				currentCell.Style["vertical-align"] = "top";
				currentCell.Style["text-align"] = "center";
				currentCell.Style["font-weight"] = "bold";
				currentCell.Text = latestPollAnswers[i].answer;
				currentRow.Cells.Add(currentCell);
				currentCell = new TableCell();
				currentCell.Style["width"] = "35px";
				currentCell.Text = "&nbsp;";
				currentRow.Cells.Add(currentCell);
			}
			tbPollAnswers.Rows.Add(currentRow);
			
			/*
			// Show progress bars
			System.Web.UI.Control loadedControl;
			ProgressBar currentProgress;
			Label currentProgressAnswer;
			Label currentNumberOfVoters;
			foreach (WebSiteServices.PollAnswerData currentAnswer in latestPollAnswers)
			{
				currentRow = new TableRow();
				
				// Add the answer
				currentProgressAnswer = new Label();
				currentProgressAnswer.Text = currentAnswer.answer;
				
				currentCell = new TableCell();
				currentCell.Controls.Add(currentProgressAnswer);
				currentCell.Width = Unit.Percentage(40);
				currentRow.Cells.Add(currentCell);
				
				// Add the progress bar
				loadedControl = Page.LoadControl("Controls/ProgressBar.ascx");
				currentProgress = (ProgressBar)loadedControl;
				//currentProgress.Width = 100;
				currentProgress.Height = 5; // Temporary
				currentProgress.Overall = totalVoters;
				currentProgress.Progress = currentAnswer.results.Length;
				
				currentCell = new TableCell();
				currentCell.Controls.Add(currentProgress);
				currentCell.Width = Unit.Percentage(50);
				currentRow.Cells.Add(currentCell);
				
				currentNumberOfVoters = new Label();
				currentNumberOfVoters.Text = currentAnswer.results.Length.ToString();
				
				currentCell = new TableCell();
				currentCell.Controls.Add(currentNumberOfVoters);
				currentCell.Width = Unit.Percentage(10);
				currentRow.Cells.Add(currentCell);
				
				tbPollAnswers.Rows.Add(currentRow);
			}
			*/
		}
		
		/// <summary>
		/// Votes for the given answer.
		/// </summary>
		/// <param name="answerData">The answer</param>
		/// <returns>True if vote successful, False otherwise.</returns>
		private bool sendVoteToService(WebSiteServices.PollAnswerData answerData)
		{
			/*
			if (didUserVote(answerData.pollId))
				return false;
			*/
			// ToDo: Should add check if user enabled cookies
			string userIP = Request.UserHostAddress;
			WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService();
			int resultID = service.vote(answerData.pollId,answerData.ID,userIP);
			
			// Update the "Polls" cookie at the poll sub-key to the current result.
			
			Response.Cookies["Polls"][answerData.pollId.ToString()] = resultID.ToString();
			Response.Cookies["Polls"].Expires = DateTime.Now.AddYears(1);
			
			return true;
		}

		/// <summary>
		/// Check to see if the current user voted for a given poll
		/// </summary>
		/// <param name="pollID">The poll to check</param>
		/// <returns>True if user already voted (will also return true if user does not support cookies)
		/// , False if not.</returns>
		private bool didUserVote(int pollID)
		{
			// ToDo: Check two things: User IP and Cookies enabled.
			if ((Request.Cookies["Polls"] == null)||(Request.Cookies["Polls"][pollID.ToString()] == null))
			{
				string userIP = Request.UserHostAddress;
				WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService();
				WebSiteServices.PollData selectedPoll = service.getPoll(pollID);
				if (selectedPoll.ID < 0)
					return true;
				
				/*
				WebSiteServices.PollAnswerData[] possibleAnsers = selectedPoll.possibleAnswers;
				WebSiteServices.PollResult[] possibleResults;
				foreach (WebSiteServices.PollAnswerData currentAnswer in possibleAnsers)
				{
					possibleResults = currentAnswer.results;
					foreach (WebSiteServices.PollResult currentResult in possibleResults)
						if (currentResult.visitorIp.Equals(userIP))
						{
							Response.Cookies["Polls"][pollID.ToString()] = currentResult.ID.ToString();
							Response.Cookies["Polls"].Expires = DateTime.Now.AddYears(1);
							return true;
						}
				}
				*/
				
				return false;
			}
			else 
				return true;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
		
		private void btnPollVote_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			for (int i = 0;i < answersButtons.Length;i++)
			{
				if (answersButtons[i].Checked)
					sendVoteToService(latestPollAnswers[i]);
			}

			//clear cache:
			CacheStore.Instance.Remove("LatestPoll");

			//Session["ActionToPerform"] = Common.SportSiteAction.ShowPollResults;
			Response.Redirect(Common.Data.AppPath+"/Main.aspx?action="+Common.SportSiteAction.ShowPollResults);
		}
	}
}
