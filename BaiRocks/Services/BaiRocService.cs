using BaiRocs.Common;
using BaiRocs.Models;
using BaiRocs.WF;
using LogApplication.Common;
using LogApplication.Common.Config;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaiRocs.Services
{
   public class BaiRocService
    {
        // subscriptionKey = "0123456789abcdef0123456789ABCDEF"
        private string subscriptionKey = Global.MainWindow.MyConfig.GetValue("AzureOCRKey"); // @"a79807f94b604d38acb42a596cbd4adb";

        // For printed text, change to TextRecognitionMode.Printed
        private const TextRecognitionMode textRecognitionMode =
            TextRecognitionMode.Printed;

        // localImagePath = @"C:\Documents\LocalImage.jpg"
        //private  string localImagePath = @"D:\_RESIBO\RawImageFolder\rgalvez\55803421_306079453402035_1923779808236404736_n.jpg";
        //private const string localImagePath2 = @"D:\_RESIBO\RawImageFolder\rgalvez\55661615_1251841378324000_1190743261493329920_n.jpg";
        //private const string localImagePath2 = @"D:\_RESIBO\ProjectResiboAzure\ResiboAzure\mock-image-uploader\OCRPushApp\test.jpg";


        private const int numberOfCharsInOperationId = 36;

       public void ReadImage( string localImagePath)
        {

            RawList = new List<BaiOcrLine>();

            ComputerVisionClient computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });

            // You must use the same region as you used to get your subscription
            // keys. For example, if you got your subscription keys from westus,
            // replace "westcentralus" with "westus".
            //
            // Free trial subscription keys are generated in the westcentralus
            // region. If you use a free trial subscription key, you shouldn't
            // need to change the region.

            // Specify the Azure region
            computerVision.Endpoint = "https://australiaeast.api.cognitive.microsoft.com/";//"https://westcentralus.api.cognitive.microsoft.com";

            Console.WriteLine("Images being analyzed ...");
            //var t1 = ExtractRemoteTextAsync(computerVision, remoteImageUrl);
            var t2 = ExtractLocalTextAsync(computerVision, localImagePath);
           

            //Task.WhenAll(t1, t2).Wait(5000);
            Task.WhenAll(t2).Wait(5000);
        }

        // Read text from a remote image
        private  async Task ExtractRemoteTextAsync(
            ComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine(
                    "\nInvalid remoteImageUrl:\n{0} \n", imageUrl);
                return;
            }

            // Start the async process to read the text
            BatchReadFileHeaders textHeaders =
                await computerVision.BatchReadFileAsync(
                    imageUrl, textRecognitionMode);

            await GetTextAsync(computerVision, textHeaders.OperationLocation);
        }

        // Recognize text from a local image
        private  async Task ExtractLocalTextAsync(
            ComputerVisionClient computerVision, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine(
                    "\nUnable to open or read localImagePath:\n{0} \n", imagePath);
                return;
            }

            using (Stream imageStream = File.OpenRead(imagePath))
            {
                // Start the async process to recognize the text
                try
                {
                    BatchReadFileInStreamHeaders textHeaders =
                  await computerVision.BatchReadFileInStreamAsync(
                      imageStream, textRecognitionMode);

                    await GetTextAsync(computerVision, textHeaders.OperationLocation);
                }
                catch (Exception err)
                {

                    Global.ProcessStatus = ProcessStatus.Error.ToString();
                    //Thread.Sleep(100);
                    OnReadDone?.Invoke(err, EventArgs.Empty);
                }
              
            }

            //DO FILTER

        }

        // Retrieve the recognized text
        private  async Task GetTextAsync(
            ComputerVisionClient computerVision, string operationLocation)
        {
            try
            {
                // Retrieve the URI where the recognized text will be
                // stored from the Operation-Location header
                string operationId = operationLocation.Substring(
                    operationLocation.Length - numberOfCharsInOperationId);

                Console.WriteLine("\nCalling GetReadOperationResultAsync()");
                ReadOperationResult result =
                    await computerVision.GetReadOperationResultAsync(operationId);

                // Wait for the operation to complete
                int i = 0;
                int maxRetries = 10;
                while ((result.Status == TextOperationStatusCodes.Running ||
                        result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
                {
                    Console.WriteLine(
                        "Server status: {0}, waiting {1} seconds...", result.Status, i);
                    await Task.Delay(1000);

                    result = await computerVision.GetReadOperationResultAsync(operationId);
                }

                // Display the results
                Console.WriteLine();
                var recResults = result.RecognitionResults;

                int lineNo = 0;
                foreach (TextRecognitionResult recResult in recResults)
                {
                    foreach (Line line in recResult.Lines)
                    {
                        lineNo += 1;
                        Console.WriteLine(line.Text);
                        BaiOcrLine ocr = new BaiOcrLine
                        {
                            LineNo = lineNo,
                            Content = line.Text
                        };
                        RawList.Add(ocr);
                    }
                }
                Console.WriteLine();
                Global.ProcessStatus = ProcessStatus.Ready.ToString();
                OnReadDone?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception err)
            {
                throw err;

            }


        }

        public event EventHandler OnReadDone; 
        #region TEXT ANALISIS
        //public List<string> RawList { get; set; }
        public List<BaiOcrLine> RawList { get; set; }



        #endregion
    }
}
