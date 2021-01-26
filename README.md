---
page_type: sample
languages:
- csharp
products:
- xamarin
- storage
description: "Quick start to create and delete containers. And upload, download and list blobs."
urlFragment: "storage-blobs-xamarin-quickstart"
---

# Official Microsoft Sample

Azure Blob Storage is Microsoft's object storage solution for the cloud. Blob storage is optimized for storing massive amounts of unstructured data.

Xamarin is Microsoft's mobile development framework. Xamarin allows you to create apps for iOS, Android, and UWP from one codebase with C# and .NET

Follow these steps to find out how to upload blobs to Azure Storage from a Xamarin app.

## Contents

| File/folder       | Description                                |
|-------------------|--------------------------------------------|
| `src`             | Sample source code.                        |
| `.gitignore`      | Define what to ignore at commit time.      |
| `CHANGELOG.md`    | List of changes to the sample.             |
| `CONTRIBUTING.md` | Guidelines for contributing to the sample. |
| `README.md`       | This README file.                          |
| `LICENSE`         | The license for the sample.                |

## Prerequisites

To complete this quick start:

* Azure subscription - [create one for free](https://azure.microsoft.com/free/)
* Azure storage account - [create a storage account](https://docs.microsoft.com/azure/storage/common/storage-quickstart-create-account)
* Visual Studio with [Mobile Development for .NET workload](https://docs.microsoft.com/en-us/xamarin/get-started/installation/?pivots=windows) installed or [Visual Studio for Mac](https://docs.microsoft.com/en-us/visualstudio/mac/installation?view=vsmac-2019)

## Setup

First you need to copy your credentials from the Azure portal.

### Copy your credentials from the Azure portal

When the sample application makes a request to Azure Storage, it must be authorized. To authorize a request, add your storage account credentials to the application as a connection string. View your storage account credentials by following these steps:

1. Sign in to the [Azure portal](https://portal.azure.com).
2. Locate your storage account.
3. In the **Settings** section of the storage account overview, select **Access keys**. Here, you can view your account access keys and the complete connection string for each key.
4. Find the **Connection string** value under **key1**, and select the **Copy** button to copy the connection string. You will add the connection string value to an environment variable in the next step.

    ![Screenshot showing how to copy a connection string from the Azure portal](https://docs.microsoft.com/en-us/azure/includes/media/storage-copy-connection-string-portal/portal-connection-string.png)

### Configure your storage connection string

After you have copied your connection string, set it to a class level variable in your *MainPage.xaml.cs* file. Open up *MainPaage.xaml.cs* and find the `storageConnectionString` variable. Replace `<yourconnectionstring>` with your actual connection string.

Here's the code:

```csharp
string storageConnectionString = "<yourconnectionstring>";
```

### Open the solution in Visual Studio

The open the `src\BlobQuickstartV12.sln` file from Visual Studio 2019 or Visual Studio for Mac.

## Key Concepts

These example code snippets show you how key concepts when using the Azure Blob Storage client library for .NET in a Xamarin.Forms app:

* [Create class level variables](#create-class-level-variables)
* [Create a container](#create-a-container)
* [Upload blobs to a container](#upload-blobs-to-a-container)
* [List the blobs in a container](#list-the-blobs-in-a-container)
* [Download blobs](#download-blobs)
* [Delete a container](#delete-a-container)

### Create class level variables

The code below declares several class level variables needed to communicate to Azure Blob Storage.

The class level variables below are in addition to the connection string for the storage account set in the [Configure your storage connection string](#configure-your-storage-connection-string) section.

These class level variables are found inside the *MainPage.xaml.cs* file:

```csharp
string storageConnectionString = "{set in the Configure your storage connection string section";
string fileName = $"{Guid.NewGuid()}-temp.txt";

BlobServiceClient client;
BlobContainerClient containerClient;
BlobClient blobClient;
```

### Create a container

Decide on a name for the new container. The code below appends a GUID value to the container name to ensure that it is unique.

> [!IMPORTANT]
> Container names must be lowercase. For more information about naming containers and blobs, see [Naming and Referencing Containers, Blobs, and Metadata](https://docs.microsoft.com/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata).

Create an instance of the [BlobServiceClient](https://docs.microsoft.com/dotnet/api/azure.storage.blobs.blobserviceclient) class. Then, call the [CreateBlobContainerAsync](https://docs.microsoft.com/dotnet/api/azure.storage.blobs.blobserviceclient.createblobcontainerasync) method to create the container in your storage account.

Find this code in the *MainPage.xaml.cs* file:

```csharp
protected async override void OnAppearing()
{            
    string containerName = $"quickstartblobs{Guid.NewGuid()}";
    
    client = new BlobServiceClient(storageConnectionString);
    containerClient = await client.CreateBlobContainerAsync(containerName);

    resultsLabel.Text = "Container Created\n";

    blobClient = containerClient.GetBlobClient(fileName);

    uploadButton.IsEnabled = true;
}
```

### Upload blobs to a container

The following code snippet:

1. Creates a `MemoryStream` of text.
1. Uploads the text to a Blob by calling the [UploadAsync](https://docs.microsoft.com/dotnet/api/azure.storage.blobs.blobcontainerclient.uploadblobasync?view=azure-dotnet#Azure_Storage_Blobs_BlobContainerClient_UploadBlobAsync_System_String_System_IO_Stream_System_Threading_CancellationToken_) function of the [BlobContainerClient](https://docs.microsoft.com/dotnet/api/azure.storage.blobs.blobcontainerclient) class, passing it both the filename defined the class level variable and the `MemoryStream` of text. This method creates the blob if it doesn't already exist, and overwrites it if it does.

Find this code in the *MainPage.xaml.cs* file:

```csharp
async void Upload_Clicked(object sender, EventArgs e)
{
    using MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Hello World!"));

    await containerClient.UploadBlobAsync(fileName, memoryStream);

    resultsLabel.Text += "Blob Uploaded\n";

    uploadButton.IsEnabled = false;
    listButton.IsEnabled = true;
}
```

### List the blobs in a container

List the blobs in the container by calling the [GetBlobsAsync](https://docs.microsoft.com/dotnet/api/azure.storage.blobs.blobcontainerclient.getblobsasync) method. In this case, only one blob has been added to the container, so the listing operation returns just that one blob.

Find this code in the *MainPage.xaml.cs* file:

```csharp
async void List_Clicked(object sender, EventArgs e)
{
    await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
    {
        resultsLabel.Text += blobItem.Name + "\n";
    }

    listButton.IsEnabled = false;
    downloadButton.IsEnabled = true;
}
```

### Download blobs

Download the previously created blob by calling the [​Download​Async](https://docs.microsoft.com/dotnet/api/azure.storage.blobs.specialized.blobbaseclient.downloadasync) method. The example code copies the `Stream` representation of the blob first into a `MemoryStream` and then into a `StreamReader` so the text can be displayed.

Find this code in the *MainPage.xaml.cs* file:

```csharp
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
```

### Delete a container

The following code cleans up the resources the app created by deleting the entire container by using [​DeleteAsync](https://docs.microsoft.com/dotnet/api/microsoft.azure.storage.blob.cloudblobcontainer.deleteasync).

The app first prompts to confirm before it deletes the blob and container. This is a good chance to verify that the resources were actually created correctly, before they are deleted.

Find this code in the *MainPage.xaml.cs* file:

```csharp
async void Delete_Clicked(object sender, EventArgs e)
{
    var deleteContainer = await Application.Current.MainPage.DisplayAlert("Delete Container",
        "You are about to delete the container proceeed?", "OK", "Cancel");

    if (deleteContainer == false)
        return;

    await containerClient.DeleteAsync();

    resultsLabel.Text += "Container Deleted";

    deleteButton.IsEnabled = false;
}
```

## Running the Sample

When the app starts, it will first create the container as it appears. Then you will need to click the buttons in order to upload, list, download the blobs, and delete the container.

Open the app solution file found in the `src` directory with Visual Studio. Then to run the app on Windows press F5. To run the app on Mac press Cmd+Enter.

The app writes to the screen after every operation. The output of the app is similar to the following example:

```output
Container Created
Blob Uploaded
98d9a472-8e98-4978-ba4f-081d69d2e6f8-temp.txt
Blob Contents:
Hello World!
Container Deleted
```

Before you begin the clean up process, verify the output of the blob's contents on screen match the value which was uploaded.

After you've verified the values, confirm the prompt to delete the container and finish the demo.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
