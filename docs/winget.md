# Install Nightingale via WinGet

You can now programmatically install Nightingale via [WinGet, Microsoft's new CLI tool for installing packages](https://github.com/microsoft/winget-cli). Note this feature is currently experimental.

![](/images/winget.png)

## Instructions

1. [Install WinGet](https://github.com/microsoft/winget-cli#installing-the-client)
1. Run `winget settings` to open its settings file.
1. Follow the instructions [in this file](https://aka.ms/winget-settings) to activate the `experimentalMSStore` feature.
1. Once the feature is enabled, run `winget install "Nightingale REST Client"`.
1. Enjoy Nightingale 🙂

---
Reference: https://github.com/jenius-apps/nightingale-rest-api-client/issues/69