﻿using Nightingale.Core.Cookies.Models;
using System.Collections.ObjectModel;

namespace Nightingale.ViewModels;

public class CookiesViewModel : ViewModelBase
{
    public CookiesViewModel()
    {
    }

    public ObservableCollection<Cookie> Cookies { get; set; }

    public string NewDomain { get; set; } = "";

    public string NewCookieString { get; set; } = "";

    public bool NoCookiesVisisble => Cookies == null ? true : Cookies.Count == 0;

    public bool DeleteAllButtonVisible => Cookies == null ? false : Cookies.Count > 0;

    public void DeleteCookie(Cookie cookie)
    {
        if (Cookies == null || cookie == null)
        {
            return;
        }

        Cookies.Remove(cookie);
        UpdateCookiesUI();
    }

    public void AddCookie()
    {
        if (Cookies == null || (string.IsNullOrWhiteSpace(NewDomain) && string.IsNullOrWhiteSpace(NewCookieString)))
        {
            return;
        }

        NewDomain = NewDomain?.Trim() ?? "";
        NewCookieString = NewCookieString?.Trim() ?? "";

        Cookies.Add(new Cookie()
        {
            Domain = NewDomain,
            Raw = NewCookieString
        });

        UpdateCookiesUI();
    }

    public void DeleteAll()
    {
        if (Cookies == null || Cookies.Count == 0)
        {
            return;
        }

        Cookies.Clear();
        UpdateCookiesUI();
    }

    private void UpdateCookiesUI()
    {
        RaisePropertyChanged("NoCookiesVisisble");
        RaisePropertyChanged("DeleteAllButtonVisible");
    }
}
