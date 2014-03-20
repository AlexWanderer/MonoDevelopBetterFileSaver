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
		public static void removeTrailingWhitespace( TextEditorData data, DocumentLine line )
		{
			if( line != null )
			{
				int num = 0;
				for( int i = line.Length - 1; i >= 0; i-- )
				{
					if( !char.IsWhiteSpace( data.Document.GetCharAt( line.Offset + i ) ) )
						break;
					num++;
				}

				if( num > 0 )
				{
					int offset = line.Offset + line.Length - num;
					data.Remove( offset, num );
				}
			}
		}

		public static void removeTrailingWhitespace( TextEditorData editor )
		{
			foreach( var line in editor.Lines )
				removeTrailingWhitespace( editor, line );
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

