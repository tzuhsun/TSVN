﻿using SamirBoulema.TSVN.Properties;

namespace SamirBoulema.TSVN
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using EnvDTE;
    using Helpers;

    [Guid("81a57ae8-6550-4dd0-940c-503d379550cc")]
    public class TSVNToolWindow : ToolWindowPane
    {
        private FileHelper _fileHelper;
        private DocumentEvents _documentEvents;
        private SolutionEvents _solutionEvents;
        private readonly TSVNToolWindowControl _tsvnToolWindowControl;
        private CommandHelper _commandHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="TSVNToolWindow"/> class.
        /// </summary>
        /// 
        public TSVNToolWindow() : base(null)
        {
            Caption = "TSVN Pending Changes";
            Content = new TSVNToolWindowControl();

            _tsvnToolWindowControl = Content as TSVNToolWindowControl;
        }

        public override void OnToolWindowCreated()
        {
            var tsvnPackage = Package as TSVNPackage;
            var dte = (DTE)tsvnPackage.GetServiceHelper(typeof(DTE));
            _tsvnToolWindowControl.SetDTE(dte);
            _fileHelper = new FileHelper(dte);
            _commandHelper = new CommandHelper(dte);

            _documentEvents = dte.Events.DocumentEvents;
            _documentEvents.DocumentSaved += DocumentEvents_DocumentSaved;

            _solutionEvents = dte.Events.SolutionEvents;
            _solutionEvents.Opened += SolutionEvents_Opened;

            _tsvnToolWindowControl.HideUnversionedButton.IsChecked = Settings.Default.HideUnversioned;

            _tsvnToolWindowControl.Update(_commandHelper.GetPendingChanges(), _fileHelper.GetSolutionDir());
        }

        private void SolutionEvents_Opened()
        {
            _tsvnToolWindowControl.Update(_commandHelper.GetPendingChanges(), _fileHelper.GetSolutionDir());
        }

        private void DocumentEvents_DocumentSaved(Document document)
        {
            _tsvnToolWindowControl.Update(_commandHelper.GetPendingChanges(), _fileHelper.GetSolutionDir());
        }
    }
}