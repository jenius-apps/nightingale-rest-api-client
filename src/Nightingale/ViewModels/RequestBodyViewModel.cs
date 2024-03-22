using JeniusApps.Common.Telemetry;
using MimeMapping;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nightingale.ViewModels;

public class RequestBodyViewModel : ViewModelBase
{
    private readonly IFilePicker _filepicker;
    private readonly ITelemetry _telemetry;
    private RequestBody _requestBody;

    public RequestBodyViewModel(
        IFilePicker filePicker,
        ITelemetry telemetry)
    {
        _filepicker = filePicker;
        _telemetry = telemetry;
    }

    public RequestBody RequestBody
    {
        get => _requestBody;
        set
        {
            _requestBody = value;
            RaisePropertyChanged(string.Empty);
        }
    }

    public bool NoBodyVisible
    {
        get => (RequestBodyType)BodyTypeIndex == RequestBodyType.None;
    }

    public bool FormEncodedListViewVisible
    {
        get => (RequestBodyType)BodyTypeIndex == RequestBodyType.FormEncoded;
    }

    public bool JsonVisible
    {
        get => (RequestBodyType)BodyTypeIndex == RequestBodyType.Json;
    }

    public bool XmlVisible
    {
        get => (RequestBodyType)BodyTypeIndex == RequestBodyType.Xml;
    }

    public bool BinaryViewVisible
    {
        get => (RequestBodyType)BodyTypeIndex == RequestBodyType.Binary;
    }

    public bool FormDataVisible
    {
        get => (RequestBodyType)BodyTypeIndex == RequestBodyType.FormData;
    }

    public bool TextVisible => (RequestBodyType)BodyTypeIndex == RequestBodyType.Text;

    public int BodyTypeIndex
    {
        get => RequestBody == null ? 0 : (int)RequestBody.BodyType;
        set
        {
            if (RequestBody == null)
            {
                return;
            }
            
            RequestBody.BodyType = value >= 0 && value < Enum.GetNames(typeof(RequestBodyType)).Length ? (RequestBodyType)value : 0;
            RaisePropertyChanged(string.Empty);
        }
    }

    public string XmlText
    {
        get => RequestBody?.XmlBody;
        set
        {
            if (RequestBody == null || RequestBody.XmlBody == value)
            {
                return;
            }

            RequestBody.XmlBody = value;
            RaisePropertyChanged("XmlText");
        }
    }

    public string PlainText
    {
        get => RequestBody?.TextBody;
        set
        {
            if (RequestBody == null || RequestBody.TextBody == value)
            {
                return;
            }

            RequestBody.TextBody = value;
            RaisePropertyChanged("PlainText");
        }
    }

    public string JsonText
    {
        get => RequestBody?.JsonBody;
        set
        {
            if (RequestBody == null || RequestBody.JsonBody == value)
            {
                return;
            }

            RequestBody.JsonBody = value;
            RaisePropertyChanged("JsonText");
        }
    }

    public ObservableCollection<Parameter> FormEncodedList
    {
        get => RequestBody?.FormEncodedData;
    }

    public ObservableCollection<FormData> FormDataList
    {
        get => RequestBody?.FormDataList;
    }

    public string BinaryFilePath
    {
        get => RequestBody?.BinaryFilePath ?? string.Empty;
        set
        {
            if (RequestBody == null)
            {
                return;
            }

            RequestBody.BinaryFilePath = value;
            RaisePropertyChanged("BinaryFilePath");
        }
    }

    public void UpdateBody()
    {
        RaisePropertyChanged(nameof(BodyTypeIndex));
        RaisePropertyChanged(string.Empty);
    }

    public async void SelectBinaryFile()
    {
        if (RequestBody == null)
        {
            return;
        }

        BinaryFilePath = await _filepicker.PickFileAsync();
        RaisePropertyChanged("BinaryFilePath");
    }

    public void RequestBodyControl_JsonTextChanged(object sender, CustomEventArgs.AddedItemArgs<string> e)
    {
        if (RequestBody != null)
        {
            RequestBody.JsonBody = e.AddedItem;
        }
    }

    public void RequestBodyControl_XmlextChanged(object sender, CustomEventArgs.AddedItemArgs<string> e)
    {
        if (RequestBody != null)
        {
            RequestBody.XmlBody = e.AddedItem;
        }
    }

    public void PlainTextChanged(object sender, string newText)
    {
        if (RequestBody != null)
        {
            RequestBody.TextBody = newText;
        }
    }

    public async void FormDataSelectFilesClicked(object sender, FormData item)
    {
        if (item == null)
        {
            return;
        }

        IList<string> paths = await _filepicker.PickFilesAsync();
        item.FilePaths = paths ?? new List<string>();

        if (string.IsNullOrWhiteSpace(item.ContentType) && item.FilePaths.Count > 0)
        {
            item.AutoContentType = MimeUtility.GetMimeMapping(item.FilePaths[0]);
        }

        _telemetry.TrackEvent("Form data select files clicked", new Dictionary<string, string>
        {
            { "Number of files", paths == null ? "0" : paths.Count.ToString() }
        });
    }
}
