using Microsoft.AppCenter.Analytics;
using System;
using System.Linq;
using Nightingale.Utilities;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Helpers;
using Nightingale.Core.Enums;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Nightingale.Core.Models;
using System.Collections.Generic;

namespace Nightingale.ViewModels
{
    public class BodyControlViewModel : ObservableBase
    {
        private readonly IResponseFileWriter _fileWriter;
        private IWorkspaceResponse _workspaceResponse;
        private int _bodyTypeIndex;

        public BodyControlViewModel(IResponseFileWriter fileWriter)
        {
            _fileWriter = fileWriter ?? throw new ArgumentNullException(nameof(fileWriter));
        }

        public IWorkspaceResponse WorkspaceResponse
        {
            get => _workspaceResponse;
            set
            {
                if (_workspaceResponse == value)
                {
                    return;
                }

                _workspaceResponse = value;

                if (_workspaceResponse != null)
                {
                    BodyTypeIndex = (int)GetContentType(_workspaceResponse.ContentType);
                }

                RaisePropertyChanged("Body");
                RaisePropertyChanged("ContentVisible");
                RaisePropertyChanged("ErrorMessage");
                RaisePropertyChanged("ErrorMessageVisible");
                RaisePropertyChanged("NoContentMessageVisible");
                RaisePropertyChanged(nameof(HtmlPreviewVisible));
                UpdateIndexProperties();
            }
        }

        public string ResponseContentType
        {
            get => WorkspaceResponse?.ContentType ?? string.Empty;
        }

        public string RawBytesString
        {
            get => WorkspaceResponse?.RawBytes == null ? "" : string.Join(" ", WorkspaceResponse.RawBytes.Select(x => (int)x).ToArray());
        }

        public int BodyTypeIndex
        {
            get => _bodyTypeIndex;
            set
            {
                _bodyTypeIndex = value >= 0 && value < Enum.GetNames(typeof(ContentType)).Length ? value : 0;
                RaisePropertyChanged("BodyTypeIndex");
                RaisePropertyChanged("SyntaxType");
                RaisePropertyChanged("EditorVisible");
                RaisePropertyChanged("RawBytesVisible");
                RaisePropertyChanged(nameof(IsHtmlIndex));
                RaisePropertyChanged(nameof(IsImageIndex));

                if (_bodyTypeIndex == (int)ContentType.Bytes)
                {
                    RaisePropertyChanged("RawBytesString");
                }

                if (_bodyTypeIndex == (int)ContentType.Image)
                {
                    LoadImageAsync();
                }
            }
        }

        /// <summary>
        /// Displays the image in the UI.
        /// </summary>
        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                RaisePropertyChanged();
            }
        }
        private ImageSource _imageSource;

        public bool ErrorMessageVisible
        {
            get => WorkspaceResponse?.Successful == false && NoContentVisible;
        }

        public string ErrorMessage
        {
            get => WorkspaceResponse == null || WorkspaceResponse.Successful 
                ? "" 
                : $"{WorkspaceResponse.StatusCode.ToString()} {WorkspaceResponse.StatusDescription}";
        }

        public bool NoContentMessageVisible
        {
            get => WorkspaceResponse?.Successful == true && NoContentVisible;
        }

        public bool IsTextIndex
        {
            get => BodyTypeIndex == (int)ContentType.Text;
        }

        public bool IsJsonIndex
        {
            get => BodyTypeIndex == (int)ContentType.Json;
        }

        public bool IsHtmlIndex
        {
            get => BodyTypeIndex == (int)ContentType.Html;
        }

        public bool IsXmlIndex
        {
            get => BodyTypeIndex == (int)ContentType.Xml;
        }

        public bool IsBytesIndex
        {
            get => BodyTypeIndex == (int)ContentType.Bytes;
        }

        /// <summary>
        /// Flag for if "Image" is currently
        /// the selected index in the UI.
        /// </summary>
        public bool IsImageIndex
        {
            get => BodyTypeIndex == (int)ContentType.Image;
        }

        public string Body
        {
            get => TextBeautifier.Beautify(WorkspaceResponse?.Body ?? "", GetContentType(ResponseContentType));
        }

        public bool NoContentVisible
        {
            get => string.IsNullOrEmpty(Body);
        }

        public bool ContentVisible
        {
            get => !NoContentVisible;
        }

        public bool EditorVisible
        {
            get => ContentVisible &&
                (BodyTypeIndex == (int)ContentType.Text
                || BodyTypeIndex == (int)ContentType.Xml
                || BodyTypeIndex == (int)ContentType.Html
                || BodyTypeIndex == (int)ContentType.Json);
        }

        /// <summary>
        /// Returns the syntax highlighting that should
        /// be used with the editor.
        /// </summary>
        public SyntaxType SyntaxType
        {
            get
            {
                switch ((ContentType)BodyTypeIndex)
                {
                    case ContentType.Json:
                        return SyntaxType.Json;
                    case ContentType.Xml:
                        return SyntaxType.Xml;
                    case ContentType.Html:
                        return SyntaxType.Html;
                    default:
                        return SyntaxType.Plain;
                }
            }
        }

        /// <summary>
        /// Visibility flag for an HTML webview preview.
        /// </summary>
        public bool HtmlPreviewVisible
        {
            get => _htmlPreviewVisible;
            set
            {
                if (_htmlPreviewVisible != value)
                {
                    _htmlPreviewVisible = value;
                    RaisePropertyChanged();
                }
            }
        }
        private bool _htmlPreviewVisible;

        public bool RawBytesVisible
        {
            get => BodyTypeIndex == (int)ContentType.Bytes && ContentVisible;
        }

        /// <summary>
        /// Toggles <see cref="HtmlPreviewVisible"/>.
        /// </summary>
        public void ToggleHtmlPreview()
        {
            HtmlPreviewVisible = !HtmlPreviewVisible;
            Analytics.TrackEvent(Telemetry.HtmlPreviewToggled);
        }

        /// <summary>
        /// Clears response and resets properties.
        /// </summary>
        public void Dispose()
        {
            ImageSource = null;
            HtmlPreviewVisible = false;
        }

        /// <summary>
        /// Takes the raw bytes property and attempts to
        /// load an image based on it.
        /// </summary>
        /// <remarks>
        /// Ref: https://marcominerva.wordpress.com/2013/04/15/how-to-convert-a-byte-array-to-image-in-a-windows-store-app/
        /// </remarks>
        private async void LoadImageAsync()
        {
            if (WorkspaceResponse?.RawBytes == null)
            {
                return;
            }

            try
            {
                using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
                {
                    // Writes the image byte array in an InMemoryRandomAccessStream
                    // that is needed to set the source of BitmapImage.
                    using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes(WorkspaceResponse.RawBytes);
                        await writer.StoreAsync();
                    }

                    var image = new BitmapImage();
                    await image.SetSourceAsync(ms);

                    // Updates the UI
                    ImageSource = image;
                }

                Analytics.TrackEvent("Response image load success", new Dictionary<string, string>
                {
                    { "content type", ResponseContentType }
                });
            }
            catch
            {
                Analytics.TrackEvent("Response image load failed", new Dictionary<string, string>
                {
                    { "content type", ResponseContentType }
                });
                ImageSource = null;
            }
        }

        public async void CopyOutput()
        {
            if ((ContentType)BodyTypeIndex == ContentType.Bytes)
            {
                Common.Copy(RawBytesString);
                Analytics.TrackEvent("Raw bytes copied");
            }
            else if ((ContentType)BodyTypeIndex == ContentType.Image && ImageSource != null)
            {
                await Common.CopyImage(WorkspaceResponse?.RawBytes);
                Analytics.TrackEvent("Image copied");
            }
            else
            {
                Common.Copy(Body);
                Analytics.TrackEvent("Output copied");
            }
        }

        /// <summary>
        /// Extracts content type from response content type. 
        /// E.g. "application/xml" will return "xml"
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private ContentType GetContentType(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return ContentType.Text;
            }

            if (content.Contains("json"))
            {
                return ContentType.Json;
            }
            else if (content.Contains("xml"))
            {
                return ContentType.Xml;
            }
            else if (content.Contains("html"))
            {
                return ContentType.Html;
            }
            else if (content.Contains("image/png") || content.Contains("image/jpeg") || content.Contains("image/jpg"))
            {
                return ContentType.Image;
            }

            return ContentType.Text;
        }
        public async void SaveBody()
        {
            if (WorkspaceResponse?.Body == null || WorkspaceResponse?.ContentType == null)
            {
                return;
            }

            if (IsImageIndex && ImageSource != null)
            {
                await _fileWriter.WriteImageAsync(WorkspaceResponse.RawBytes);
            }
            else
            {
                await _fileWriter.WriteFileAsync(WorkspaceResponse);
            }
        }

        private void UpdateIndexProperties()
        {
            RaisePropertyChanged("IsTextIndex");
            RaisePropertyChanged("IsJsonIndex");
            RaisePropertyChanged("IsXmlIndex");
            RaisePropertyChanged("IsHtmlIndex");
            RaisePropertyChanged("IsBytesIndex");
            RaisePropertyChanged("IsImageIndex");
        }
    }
}
