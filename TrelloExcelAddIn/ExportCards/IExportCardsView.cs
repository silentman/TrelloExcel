using System;
using System.Collections.Generic;
using TrelloNet;

namespace TrelloExcelAddIn
{
	public interface IExportCardsView
	{
		event EventHandler BoardWasSelected;
		event EventHandler ExportCardsWasClicked;
		event EventHandler RefreshButtonWasClicked;	
		
		bool EnableSelectionOfBoards { get; set; }
		bool EnableSelectionOfLists { get; set; }
		bool EnableExportCards { get; set; }
		IBoardId SelectedBoard { get; }
		IListId SelectedList { get; }
		void DisplayBoards(IEnumerable<BoardViewModel> boards, IBoardId selectBoard = null);
		void DisplayLists(IEnumerable<List> lists);
		void ShowErrorMessage(string message);
		void ShowStatusMessage(string message, params object[] args);		
	}
}