using Nightingale.Core.Models;
using Nightingale.CustomEventArgs;
using Nightingale.Utilities;
using System;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class RequestBodyControl : ObservableUserControl
    {
        public event EventHandler<DeletedItemArgs<Parameter>> FormDataItemDeleted;
        public event EventHandler<AddedItemArgs<string>> JsonTextChanged;
        public event EventHandler<AddedItemArgs<string>> XmlTextChanged;
        public event EventHandler<AddedItemArgs<Parameter>> FormEncodedAddVariableClicked;
        public event TypedEventHandler<object, string> PlainTextChanged;
        public event EventHandler SelectBinaryFileClicked;
        public event EventHandler BodyTypeIndexChanged;
        public event TypedEventHandler<object, FormData> FormDataSelectFilesClicked;

        public RequestBodyControl()
        {
            this.InitializeComponent();
        }

        public int BodyTypeIndex
        {
            get => (int)GetValue(BodyTypeIndexProperty);
            set => SetValueDp(BodyTypeIndexProperty, value);
        }

        public bool NoBodyVisible
        {
            get => (bool)GetValue(NoBodyVisibleProperty);
            set => SetValueDp(NoBodyVisibleProperty, value);
        }

        public bool JsonVisible
        {
            get => (bool)GetValue(JsonVisibleProperty);
            set => SetValueDp(JsonVisibleProperty, value);
        }

        public bool XmlVisible
        {
            get => (bool)GetValue(XmlVisibleProperty);
            set => SetValueDp(XmlVisibleProperty, value);
        }

        public bool FormEncodedListViewVisible
        {
            get => (bool)GetValue(FormEncodedListViewVisibleProperty);
            set => SetValueDp(FormEncodedListViewVisibleProperty, value);
        }

        public bool BinaryViewVisible
        {
            get => (bool)GetValue(BinaryViewVisibleProperty);
            set => SetValueDp(BinaryViewVisibleProperty, value);
        }

        public bool FormDataVisible
        {
            get => (bool)GetValue(FormDataVisibleProperty);
            set => SetValueDp(FormDataVisibleProperty, value);
        }

        public string JsonText
        {
            get => (string)GetValue(JsonTextProperty);
            set => SetValueDp(JsonTextProperty, value);
        }

        public string XmlText
        {
            get => (string)GetValue(XmlTextProperty);
            set => SetValueDp(XmlTextProperty, value);
        }

        public ObservableCollection<Parameter> FormEncodedList
        {
            get => (ObservableCollection<Parameter>)GetValue(FormEncodedListProperty);
            set => SetValueDp(FormEncodedListProperty, value);
        }

        public ObservableCollection<FormData> FormDataList
        {
            get => (ObservableCollection<FormData>)GetValue(FormDataListProperty);
            set => SetValueDp(FormDataListProperty, value);
        }

        public string BinaryFilePath
        {
            get => (string)GetValue(BinaryFilePathProperty);
            set => SetValueDp(BinaryFilePathProperty, value);
        }

        public bool RadioButtonsVisible
        {
            get => (bool)GetValue(RadioButtonsVisibleProperty);
            set => SetValueDp(RadioButtonsVisibleProperty, value);
        }

        public bool BodyTypeComboBoxVisible
        {
            get => (bool)GetValue(BodyTypeComboBoxVisibleProperty);
            set => SetValueDp(BodyTypeComboBoxVisibleProperty, value);
        }

        public bool TextVisible
        {
            get => (bool)GetValue(TextVisibleProperty);
            set => SetValueDp(TextVisibleProperty, value);
        }

        public string PlainText
        {
            get => (string)GetValue(PlainTextProperty);
            set => SetValueDp(PlainTextProperty, value);
        }

        public static readonly DependencyProperty PlainTextProperty = DependencyProperty.Register(
            "PlainText",
            typeof(string),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty TextVisibleProperty = DependencyProperty.Register(
            "TextVisible",
            typeof(bool),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty RadioButtonsVisibleProperty = DependencyProperty.Register(
            "RadioButtonsVisible",
            typeof(bool),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty BodyTypeComboBoxVisibleProperty = DependencyProperty.Register(
            "BodyTypeComboBoxVisible",
            typeof(bool),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty FormDataListProperty = DependencyProperty.Register(
            "FormDataList",
            typeof(ObservableCollection<FormData>),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty FormDataVisibleProperty = DependencyProperty.Register(
            "FormDataVisible",
            typeof(bool),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty BinaryFilePathProperty = DependencyProperty.Register(
            "BinaryFilePath",
            typeof(string),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty FormEncodedListProperty = DependencyProperty.Register(
            "FormEncodedList",
            typeof(ObservableCollection<Parameter>),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty XmlTextProperty = DependencyProperty.Register(
            "XmlText",
            typeof(string),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty JsonTextProperty = DependencyProperty.Register(
            "JsonText",
            typeof(string),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty BinaryViewVisibleProperty = DependencyProperty.Register(
            "BinaryViewVisible",
            typeof(bool),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty FormEncodedListViewVisibleProperty = DependencyProperty.Register(
            "FormEncodedListViewVisible",
            typeof(bool),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty XmlVisibleProperty = DependencyProperty.Register(
            "XmlVisible",
            typeof(bool),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty JsonVisibleProperty = DependencyProperty.Register(
            "JsonVisible",
            typeof(bool),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty NoBodyVisibleProperty = DependencyProperty.Register(
            "NoBodyVisible",
            typeof(bool),
            typeof(RequestBodyControl),
            null);

        public static readonly DependencyProperty BodyTypeIndexProperty = DependencyProperty.Register(
            "BodyTypeIndex",
            typeof(int),
            typeof(RequestBodyControl),
            null);

        private void XmlBodyEditor_EditorTextChanged(object sender, EventArgs e)
        {
            XmlTextChanged?.Invoke(sender, new AddedItemArgs<string>(XmlBodyEditor.Text));
        }

        private void JsonBodyEditor_EditorTextChanged(object sender, EventArgs e)
        {
            JsonTextChanged?.Invoke(sender, new AddedItemArgs<string>(JsonBodyEditor.Text));
        }

        private void TextBodyEditor_EditorTextChanged(object sender, EventArgs e)
        {
            PlainTextChanged?.Invoke(sender, TextBodyEditor.Text);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                BodyTypeIndex = rb.GetIndexWithinGroup();
                BodyTypeIndexChanged?.Invoke(this, new EventArgs());
            }
        }

        private void BodyTypeSelectionChanged() => BodyTypeIndexChanged?.Invoke(this, new EventArgs());

        private void SelectBinaryFile() => SelectBinaryFileClicked?.Invoke(this, new EventArgs());

        private void DeleteFormDataItem(object s, DeletedItemArgs<Parameter> e) => FormDataItemDeleted?.Invoke(s, e);

        private void FormDataListControl_SelectFilesClicked(object sender, FormData args)
        {
            FormDataSelectFilesClicked?.Invoke(sender, args);
        }

        private void FormEncodedAddVariableClick(object sender, AddedItemArgs<Parameter> e)
        {
            FormEncodedAddVariableClicked?.Invoke(this, e);
        }
    }
}
