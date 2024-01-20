using Nightingale.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Nightingale.Core.CodeGenerators;
using Nightingale.Core.Workspaces.Models;

namespace Nightingale.ViewModels
{
    public enum LanguageIndex
    {
        csharp,
        curl,
        python
    }

    public class CodeGenPageViewModel : ViewModelBase
    {
        private readonly ICurlGenerator _curlGenerator;
        private readonly IRestSharpCodeGenerator _restSharpGenerator;
        private string _code;
        private string _name;
        private int _selectedLangIndex;

        public CodeGenPageViewModel(
            ICurlGenerator curlGenerator,
            IRestSharpCodeGenerator restSharpGenerator)
        {
            _curlGenerator = curlGenerator ?? throw new ArgumentNullException(nameof(curlGenerator));
            _restSharpGenerator = restSharpGenerator ?? throw new ArgumentNullException(nameof(restSharpGenerator));
        }

        public Item WorkspaceItem { get; set; }

        public int SelectedLangIndex
        {
            get => _selectedLangIndex;
            set
            {
                if (_selectedLangIndex == value)
                    return;

                _selectedLangIndex = value;
                RaisePropertyChanged("SelectedLangIndex");
                Analytics.TrackEvent("Code generator language changed", new Dictionary<string, string>() { { "Lang", ((LanguageIndex)value).ToString() } });
                Rerun();
            }
        }

        public string Code
        {
            get => _code;
            set
            {
                _code = value;
                RaisePropertyChanged("Code");
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public void CopyCode() => Common.Copy(Code);

        private void Rerun() => GenerateCode();

        private void ResetCodeText() => Code = "";

        public async void GenerateCode()
        {
            if (WorkspaceItem == null)
            {
                return;
            }

            if (WorkspaceItem.Type == ItemType.Request)
            {
                await ProcessRequestAsync(WorkspaceItem);
            }
            else if (WorkspaceItem.Type == ItemType.Collection)
            {
                // Not currently supported.
            }
        }

        private async Task ProcessRequestAsync(Item r)
        {
            if (r == null)
            {
                return;
            }

            Name = r.Name;
            ResetCodeText();

            switch ((LanguageIndex)SelectedLangIndex)
            {
                case LanguageIndex.csharp:
                    Code = await _restSharpGenerator.GenerateCodeAsync(r);
                    break;
                case LanguageIndex.curl:
                    Code = await _curlGenerator.GenerateCodeAsync(r);
                    break;
                default:
                    break;
            }
        }
    }
}
