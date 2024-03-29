## Recent changelog

4.4.1
- Fixed telemetry bug

4.4.0
- Fixed issues with environment variables when importing Insomnia (GitHub #123). Thanks to contributor **@jamesmcroft**.
- Added support for exporting collections to Postman files. Try it by right clicking on a collection. (GitHub #146). Thanks to contributor **@Ombrelin**.
- Added support for importing OData files. Thanks to contributor **@paule96**.

4.3.9
- Fixed typo in about page

4.3.8
- Fixed curl import error
- Added tooltip to request tab on mouse hover

4.3.7
- F2 rename shortcut
- Fixed some postman import issues
- Added curl paste support

4.3.4
- Fixed duplicate queries in postman converstion (GitHub #132)
- Improved error messages in OAuth 2.0
- Fixed null Url (GitHub #144)
- Added curl import support (GitHub #70)

4.3.1
- Fixed trailing '?' added to requests (GitHub #139)

4.3.0
- ARM64 support

4.2.0
- Added random GUID insertion in Ctrl+Space toolbox
- Added toggle word wrap button to text editor
- Added new setting to change the default word wrap mode
- Improved log tab to show raw request and raw response log data

4.1.10
- Fixed theme bug (GitHub issue #129)
- Added right click context menu to tabs

4.1.9
- Fixed postman collection import issue (GitHub issue #133)
- Added rounded corners to dialogs

4.1.6
- Improved UX upon successful import
- Improved UX upon successful export


4.1.5
- Added support for non-standard auth header
- Fixed broken OAuth 2.0 auth code flow
- Fixed broken grant type field when switching tabs

4.1.4
- Nightingale will now launch with your last used workspace
- Added option to change menu background image
- Added setting to always wrap URL bar

4.1.3
- Fixed crash caused by importing an online-only OneDrive file without internet
- Fixed collection page results not updating
- Added auto save every 5 minutes
- Fixed Set-Cookie not correctly being detected when receiving a response
- Added auto decoding of gzip-encoded responses
- Fixed bug with text size changing in URL bar
- Added link to instructions in the mock server dialog

4.1.2
- Added HTTP method to tab
- Improved toggle side bar icon
- Now displaying which layout is actively used in the View menu
- Improved file size of NCF files
- Added support button
- Added out of box experience for first-time installers
- Added status code information
- Fixed the logo size of NCF files

4.1.1
- New feature: added mock server deployment. Go to Workspace > deploy mock server to learn more.

4.0.8
- Fixed bug with form data not functioning correctly when switching between tabs

4.0.7
- Press Ctrl+Space in a query to insert a DateTime string like 2015-03-25T12:00:00Z
- Fixed bug with tabs sometimes not rendering properly
- Fixed bug with auth selection not changing property when switching tabs
- Fixed bug with request body sometimes not being rendered correctly when switching tabs
- Fixed bug with URL bar not expanding reliably when focused

4.0.4
- Added tabs
- Added new menu UI
- Added new logo
- Added new keyboard shortcuts for tab UX
- Added new Get Started page
- Moved workspace switcher to the title bar

3.8.2
- Added queries and headers to collections
- Fixed crash when a cancelled request is duplicated
- Added Ctrl+Space shortcut to insert variables in queries and headers

3.7.10
- Added private variables
- Fixed exception when using authorization header and having an auth setting enabled
- Enabled file picker to view all files when selecting files in multipart form body content

3.7.2
- Added auto expanding fields

3.7.1
- Added image preview
- Fixed URI format exception with URLs that are missing the URI scheme

3.6.10
- Fixed handling keys in form data
- Added `Variable Quick Add` feature for headers and form url encoded body.

3.6.8
- Fixed collections being duplicated upon app shutdown and restart
- Fixed imported workspace not persisting after app shutdown and restart
- Fixed issue where user cannot send the request when using touch to tap the send button

3.6.7
- Added ability to select which workspaces to export
- Added auto detection of binary file content type in form data and binary file body
- Added Environment Quick Edit
- Added ability to rename and clone environments
- Added ability to make an environment active directly from the environment manager
- Moved environments button down to beside the active environment selector
- Fixed access denied issues when attaching files to requests
- Fixed custom user content type not being acknowledged by Nightingale
- Improved treeview performance
- Requests can now hold children
- Improved UX when adding new requests/collections

3.5.7
- Fixed form data postman import
- Fixed variables not working for OAuth 2.0 properties
- Removed API Test Preview features temporarily. We plan to reintroduce testing in the future.

3.5.6
- Fixed null reference when importing postman files.

3.5.5
- Added Variable Quick Add feature
- Added NCF import feature
- Fixed crash caused by null URL

3.5.2
- Left sidebar will now automatically collapse when the window becomes narrow 
- URL padding and sizes were improved for more real estate in narrow view
- List of queries will now synchronize with the queries in the URL bar
- Fixed bug where XML bodies were not being sent
- Fixed bug in multipart form data where multiple empty rows are added randomly
- Fixed bug where cancelling file selection from the import dialog causes a crash

3.4.7
- Fixed bug with OAuth 2.0 not working if the authorization URL you provide already contains query parameters
- Performance improvement when sending requests

3.4.4
- HTTP engine rewritten to replace RestSharp with .NET's built-in HttpClient
- OAuth 1.0a improvements
- Added support for custom HTTP methods

3.3.1
- Fixed authentication inheritance not working when collection or request were just created.

3.3.0
- Added authentication to collections.
- Requests and collections can now recursively inherit the authentication of its parent.

3.2.9
- Added a help message to the bottom status bar while working with localhost in case troubleshooting is needed.

3.2.5
- Workspaces are automatically saved when a crash occurs
- Improved screen reader support
- Fixed bug with variables not working for username and password in basic authentication
- Fixed bug with search bar not getting focused when pressing Ctrl+F

3.2.4
- Fixed crash when adding a URL without a proper protocol.
- Fixed issue with Nightingale not setting the key name of a form data file.

3.2.3
- Added abilitiy to import OpenAPI files (preview)

3.2.1
- Added F2 rename shortcut
- 1xx and 3xx status codes are now displayed in orange
- Fixed crash when extracting a property that doesn't exist during variable chaining
- Fixed crashes caused by invalid postman collection files
- Known issue: in some scenarios, the F2 rename shortcut does not rename the actively selected item. It will rename a previously selected item instead. Unfortunately, to fix this, a SelectionChanged API is required from the Windows UI Library's TreeView control. [Click here for the API proposal](https://github.com/microsoft/microsoft-ui-xaml/issues/322).

3.1.19
- Added ability to import Postman Collection v2.1 files
- Added ability to add new item to workspace root via context menu
- Fixed issue where page navigation failed when creating new item in collection via context menu

3.1.8
- Bug fixes

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
- Added ability to save data as text-based Nightingale collection format (aka NCF file)
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
