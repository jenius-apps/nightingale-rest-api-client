## Recent changelog

3.1.6
- New cookie manager in Nightingale. You can now add cookies to your requests!
- You can now copy headers and cookies from your responses.

3.0.27
- Nightingale now automatically saves your workspaces when you close the app. This can be turned on or off in settings.

3.0.25
- Added popup asking to save files when closing the app.
- Fixed cache issue with Nightingale Premium not being activated until the app was restarted.
- Fixed crash that occured when you try to save the workspace but the file you're using had been deleted.
- Fixed bug where you sometimes cannot delete items in collections

3.0.23
- Nightingale now opens a request when it's newly created, and the URL bar is focused automatically
- Fixed bug with form data only sending first row of data.

3.0.20
- Added support for multipart form data in request body
- Added support for text body in request
- Fixed bug with being unable to cancel collections that are actively running
- Added digest authentication support

3.0.7
- WebView response preview replaced with HTML text with syntax highlighting to improve performance. Want preview to return? Let us know by opening an issue and we'll investigate re-introducing it with improved performance.
- Fixed bug where the Save As button sometimes did not actually save data
- Fixed issue fetching add-on prices
- Fixed font size in request tooltip
- Fixed tab-index ordering in request page
- Fixed crash sometimes caused by toggling between add-on prices

3.0.5
- New UI
- Added ability to open multiple instances of Nightingale
- Added ability to save data as text-based Nightingale collection file (aka NCF file)
- Added ability to double click on NCF files to launch Nightingale
- Added WinUI support
- Fixed drag and drop issues. You can now drag and drop once again
- Fixed bug with localhost connection not working when there's no internet
- Fixed slow downs when launching Nightingale caused by large history
- Adding ability to connect to localhost on another computer in the same network
- Added ability to store collections inside collections

2.4.1
- Added beautify button

2.4.0
- Improved text editor
- Text is now searchable
- Support for very large text

2.3.1
- Added request history
- Added indicators to display how many queries/headers you have
- Added request/collection tooltip previews

2.2.0
- Added OneDrive backup integration

2.1.3
- Fixed issues with chaining and collections

2.1.1
- Added C#-based API tests
- Added variable chaining

2.0.8
- Improved error message when using OAuth2 without a callback URL
- Added send and download button
