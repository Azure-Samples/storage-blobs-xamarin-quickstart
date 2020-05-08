using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

// these are brand new
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;

namespace BlobQuickstartV12
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
#warning enter your connection string
        string storageConnectionString = "{yourconnectionstringgoeshere}";
        
        string fileName = $"{Guid.NewGuid()}-temp.txt";

        BlobServiceClient client;
        BlobContainerClient containerClient;
        BlobClient blobClient;

        public MainPage()
        {
            InitializeComponent();            
        }

        protected async override void OnAppearing()
        {
            string containerName = $"quickstartblobs{Guid.NewGuid()}";

            client = new BlobServiceClient(storageConnectionString);
            containerClient = await client.CreateBlobContainerAsync(containerName);

            resultsLabel.Text = "Container Created\n";

            blobClient = containerClient.GetBlobClient(fileName);

            uploadButton.IsEnabled = true;
        }

        async void Upload_Clicked(object sender, EventArgs e)
        {                                    
            using MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Hello World!"));

            await containerClient.UploadBlobAsync(fileName, memoryStream);

            resultsLabel.Text += "Blob Uploaded\n";

            uploadButton.IsEnabled = false;
            listButton.IsEnabled = true;
        }


        async void List_Clicked(object sender, EventArgs e)
        {            
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                resultsLabel.Text += blobItem.Name + "\n";                
            }

            listButton.IsEnabled = false;
            downloadButton.IsEnabled = true;
        }

        async void Download_Clicked(object sender, EventArgs e)
        {
            BlobDownloadInfo downloadInfo = await blobClient.DownloadAsync();

            using MemoryStream memoryStream = new MemoryStream();
            
            await downloadInfo.Content.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using StreamReader streamReader = new StreamReader(memoryStream);

            resultsLabel.Text += "Blob Contents: \n";
            resultsLabel.Text += await streamReader.ReadToEndAsync();
            resultsLabel.Text += "\n";

            downloadButton.IsEnabled = false;
            deleteButton.IsEnabled = true;
        }

        async void Delete_Clicked(object sender, EventArgs e)
        {            
            var deleteContainer = await Application.Current.MainPage.DisplayAlert("Delete Container",
                "You are about to delete the container proceeed?", "OK", "Cancel");

            if (deleteContainer == false)
                return;

            await containerClient.DeleteAsync();

            resultsLabel.Text += "Container deleted";

            deleteButton.IsEnabled = false;
        }
    }
}
