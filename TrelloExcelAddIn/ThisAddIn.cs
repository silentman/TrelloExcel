﻿using System.Threading.Tasks;
using Microsoft.Office.Core;
using TrelloExcelAddIn.Properties;
using TrelloNet;
using CustomTaskPane = Microsoft.Office.Tools.CustomTaskPane;

namespace TrelloExcelAddIn
{
	public partial class ThisAddIn
	{
		public Trello Trello { get; private set; }

		public ExportCardsPresenter ExportCardsPresenter { get; private set; }
        public ImportCardsPresenter ImportCardsPresenter { get; private set; }
		public AuthorizePresenter AuthorizePresenter { get; set; }

		public CustomTaskPane ExportCardsTaskPane { get; private set; }
        public CustomTaskPane ImportCardsTaskPane { get; private set; }

		public TaskScheduler TaskScheduler { get; private set; }
		public MessageBus MessageBus { get; private set; }

		private void ThisAddIn_Startup(object sender, System.EventArgs e)
		{
			Trello = new Trello("1ed8d91b5af35305a60e169a321ac248");
			MessageBus = new MessageBus();

			var exportCardsControl = new ExportCardsControl();
            var importCardsControl = new ImportCardsControl();
			var authorizeForm = new AuthorizationDialog();

			ExportCardsTaskPane = CustomTaskPanes.Add(exportCardsControl, "Export cards to Trello");
			ExportCardsTaskPane.Width = 300;
			ExportCardsTaskPane.DockPositionRestrict = MsoCTPDockPositionRestrict.msoCTPDockPositionRestrictNoHorizontal;

            ImportCardsTaskPane = CustomTaskPanes.Add(importCardsControl, "Import cards from Trello");
            ImportCardsTaskPane.Width = 300;
            ImportCardsTaskPane.DockPositionRestrict = MsoCTPDockPositionRestrict.msoCTPDockPositionRestrictNoHorizontal;

			TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

			ExportCardsPresenter = new ExportCardsPresenter(exportCardsControl, Trello, new GridToNewCardTransformer(), TaskScheduler, MessageBus);
		    ImportCardsPresenter = new ImportCardsPresenter(importCardsControl, MessageBus, Trello, TaskScheduler);
			AuthorizePresenter = new AuthorizePresenter(authorizeForm, Trello, MessageBus);

			Globals.Ribbons.TrelloRibbon.SetMessageBus(MessageBus);

			TryToAuthorizeTrello();
		}

		private void TryToAuthorizeTrello()
		{
			if (string.IsNullOrWhiteSpace(Settings.Default.Token))
				return;

			Trello.Authorize(Settings.Default.Token);
			Trello.Async.Members.Me().ContinueWith(t =>
			{
				if (t.Exception == null)
					MessageBus.Publish(new TrelloWasAuthorizedEvent(t.Result));
				else
				{
					Trello.Deauthorize();
					Settings.Default.Token = "";
					Settings.Default.Save();
				}
			});
		}

		private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
		{
		}

		#region VSTO generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(ThisAddIn_Startup);
			this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
		}

		#endregion
	}
}
