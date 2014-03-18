using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using Mono.TextEditor;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Ide.Gui.Components;
using Mono.Addins;
using System.Collections.Generic;


namespace Prime31
{
	public static class FileHelper
	{
		public static void removeTrailingWhitespace( TextEditorData editor )
		{
			var caretLocation = editor.Caret.Location;
			var lines = new List<string>();

			foreach( var line in editor.Lines )
			{
				var lineText = editor.Text.Substring( line.Offset, line.Length );
				lines.Add( lineText.TrimEnd() );
			}

			editor.Text = string.Join( "\n", lines );
			editor.Caret.Location = caretLocation;
			editor.CenterToCaret();
		}
	}



	#region CommandHandlers

	public class Save : CommandHandler
	{
		protected override void Run()
		{
			FileHelper.removeTrailingWhitespace( IdeApp.Workbench.ActiveDocument.Editor );
			IdeApp.Workbench.ActiveDocument.Save();
		}


		protected override void Update( CommandInfo info )
		{
			info.Enabled = IdeApp.Workbench.ActiveDocument != null && IdeApp.Workbench.ActiveDocument.IsDirty;
		}
	}


	public class SaveAll : CommandHandler
	{
		protected override void Run()
		{
			for( int i = 0; i < IdeApp.Workbench.Documents.Count; i++ )
			{
				if( IdeApp.Workbench.Documents[i].IsDirty )
					FileHelper.removeTrailingWhitespace( IdeApp.Workbench.Documents[i].Editor );
			}

			IdeApp.Workbench.SaveAll();
		}


		protected override void Update( CommandInfo info )
		{
			bool hasdirty = false;
			for( int i = 0; i < IdeApp.Workbench.Documents.Count; i++ )
				hasdirty |= IdeApp.Workbench.Documents[i].IsDirty;

			info.Enabled = hasdirty;
		}
	}


	public class RemoveTrailingWhitespace : CommandHandler
	{
		protected override void Run()
		{
			FileHelper.removeTrailingWhitespace( IdeApp.Workbench.ActiveDocument.Editor );
		}


		protected override void Update( CommandInfo info )
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			info.Enabled = doc != null && doc.Editor != null;
		}
	}

	#endregion


	#region Command Enums

	public enum BetterFileCommands
	{
		Save,
		SaveAll
	}

	public enum BetterEditCommands
	{
		RemoveTrailingWhitespace
	}

	#endregion

}

