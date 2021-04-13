using NAPS2.ImportExport.Email;
using NAPS2.ImportExport.Images;
using NAPS2.ImportExport.Pdf;
using NAPS2.Lang.Resources;
using NAPS2.Ocr;
using NAPS2.Operation;
using NAPS2.Scan.Images;
using NAPS2.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NAPS2.WinForms
{
    public class WinFormsExportHelper
    {
        private readonly PdfSettingsContainer pdfSettingsContainer;
        private readonly ImageSettingsContainer imageSettingsContainer;
        private readonly DialogHelper dialogHelper;
        private readonly ChangeTracker changeTracker;
        private readonly IOperationFactory operationFactory;
        private readonly OcrManager ocrManager;
        private readonly IOperationProgress operationProgress;

        public WinFormsExportHelper(PdfSettingsContainer pdfSettingsContainer,
            ImageSettingsContainer imageSettingsContainer, DialogHelper dialogHelper, ChangeTracker changeTracker,
            IOperationFactory operationFactory, OcrManager ocrManager, IOperationProgress operationProgress)
        {
            this.pdfSettingsContainer = pdfSettingsContainer;
            this.imageSettingsContainer = imageSettingsContainer;
            this.dialogHelper = dialogHelper;
            this.changeTracker = changeTracker;
            this.operationFactory = operationFactory;
            this.ocrManager = ocrManager;
            this.operationProgress = operationProgress;
        }

        public async Task<bool> SavePdf(List<ScannedImage> images, ISaveNotify notify)
        {
            if (!images.Any()) return false;
            string savePath;

            var pdfSettings = pdfSettingsContainer.PdfSettings;
            if (pdfSettings.SkipSavePrompt && Path.IsPathRooted(pdfSettings.DefaultFileName))
            {
                savePath = pdfSettings.DefaultFileName;
            }
            else
            {
                if (!dialogHelper.PromptToSavePdf(pdfSettings.DefaultFileName, out savePath))
                {
                    return false;
                }
            }

            var changeToken = changeTracker.State;
            var firstFileSaved = await ExportPdf(savePath, images, false, null);
            if (firstFileSaved == null) return false;
            changeTracker.Saved(changeToken);
            notify?.PdfSaved(firstFileSaved);
            return true;
        }

        public async Task<string> ExportPdf(string filename, List<ScannedImage> images, bool email, EmailMessage emailMessage)
        {
            var op = operationFactory.Create<SavePdfOperation>();

            var pdfSettings = pdfSettingsContainer.PdfSettings;
            pdfSettings.Metadata.Creator = MiscResources.NAPS2;
            if (op.Start(filename, DateTime.Now, images, pdfSettings, ocrManager.DefaultParams, email, emailMessage))
            {
                operationProgress.ShowProgress(op);
            }
            return await op.Success ? op.FirstFileSaved : null;
        }

        public async Task<bool> SaveImages(List<ScannedImage> images, ISaveNotify notify)
        {
            if (!images.Any()) return false;
            string savePath;

            var imageSettings = imageSettingsContainer.ImageSettings;
            if (imageSettings.SkipSavePrompt && Path.IsPathRooted(imageSettings.DefaultFileName))
            {
                savePath = imageSettings.DefaultFileName;
            }
            else
            {
                if (!dialogHelper.PromptToSaveImage(imageSettings.DefaultFileName, out savePath))
                {
                    return false;
                }
            }

            var op = operationFactory.Create<SaveImagesOperation>();
            var changeToken = changeTracker.State;
            if (op.Start(savePath, DateTime.Now, images))
            {
                operationProgress.ShowProgress(op);
            }

            if (!await op.Success) return false;
            changeTracker.Saved(changeToken);
            notify?.ImagesSaved(images.Count, op.FirstFileSaved);
            return true;
        }
    }
}
