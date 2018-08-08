# VpnSelector/WebAccount

## Description

The VpnSelector program is a graphical user interface for working with VPN connections based on standard Windows protocols PPTP and L2TP ( [https://ru.wikipedia.org/wiki/L2TP](https://translate.google.com/translate?hl=ru&prev=_t&sl=ru&tl=en&u=https://ru.wikipedia.org/wiki/L2TP) ) . PPTP and L2TP connections can be configured manually through the Windows interface ( [https://support.microsoft.com/en-us/help/323441/how-to-install-and-configure-a-virtual-private-network- server-in-windo](https://translate.google.com/translate?hl=ru&prev=_t&sl=ru&tl=en&u=https://support.microsoft.com/en-us/help/323441/how-to-install-and-configure-a-virtual-private-network-server-in-windo) ), but VpnSelector automates and simplifies this process for Windows 7,8,10, ...

The advantages of this automation are particularly  well manifested when using the program in a portable mode. You connect your flash drive to the computer and run the VpnSelector  from this drive. In this case, the program settings, including the list of VPN connections (address, login, password) that you created beforehand, are also stored together with the program on the flash drive. Then in the program interface you select the required VPN connection from the list and click "Connect". After that, the program creates an entry in the list of Windows connections and connects to the remote VPN server. The created connection is subsequently available via the standard Windows interface, but the password is not saved. Password  is transferred to this connection each time from the interface of the VpnSelector. Thus, no one, except you, can use the created VPN connection.

After the end of the work, you can also easily delete the VPN connection you created (or all VPN connections).

Additional features of the program.

*   User-friendly graphical interface.
*   Displays the user interface in the system tray ( Windows notification area ) with an indicator and context menu.
*   Graphical indication of the current VPN connection.
*   Use the country flags to visually show the current VPN connection.
*   Automatic detection of an external IP address before and after connecting over a VPN channel (using the dyndns.org service).
*   Obtaining and displaying information about this IP address (using the ip-api.com service).
*   Reconciliation of the country and city fields from this information with the data that was entered in the program interface when creating a record of the VPN connection. If the information is different, then it will be updated. This allows you to quickly identify cases when the server's declared location of the VPN services does not correspond to its actual location.
*   Automatically restore the connection to the VPN server when the connection is lost.
*   Verify the health of one or more VPN connections (in the background mode).
*   View the console and the log of background checks.
*   Remember the information about the number of successful and unsuccessful connections (or background checks) to this VPN server. This allows you to rank VPN connections in terms of reliability.
*   Import a list of VPN connections from the vpngate.net server .

### <span class="notranslate">Versions

*   VpnSelector - lightweight ( installer ~ 2.4 M) application for working with VPN
*   WebAccount - an extended version ( installer ~ 45.0 M), including the functionality of VpnSelector, plus the password (web accounts) manager with the embedded web browser Chromium

To download latest installers go to Releases page https://github.com/dionext/nettools/releases

## Working with the program

### System requirements

  Windows 7,8,10 (on versions after 10 has not yet been tested), Net Framework 4.6

Installed Chrome browser ( for the advanced version of the WebAccount application )

### Installation

Although the program can work completely in portabile mode, there is an installer. It deploys the program from the archive to the directory specified by the user, and also creates shortcuts in the "Programs" menu and the shortcut on the desktop.

All program settings and the saved list of VPN servers are stored in the local folder of the program. 

*   [Program folder]\Data\Profile\appconfig - settings for appearance, columns of lists, sizes of windows, etc. 

*   [Program folder]\Data\Profile\dataStorage - user database of VPN providers and servers, country directory, etc. 

*   [Users folder]\[User]\AppData\Local\[App name]\[Versions] - The exception is the user interface font settings. It is bound to a specific computer and stored in the system folder 

### Running the program

The program can be launched via the menu item in the "Programs", Shortcut on the desktop, or directly by the exe file in the program folder (portable mode). The main mode of the program - without displaying the main application window, but only the icon in the windows system tray.

*   To display the main application window use the context menu of the system tray menu - "Show window"
*   To hide the main application window use "Main menu - File - Hide main application window"

The icon in the windows system tray can look like this:

*   Network symbol with red backlight - no current VPN connection
*   Green network network symbol - there is a current connection via VPN , but it was not possible to determine the connection parameters
*   Flag of the country - there is a current connection on the VPN , its parameters are defined, the flag of the country of location of the VPN server is displayed

### Work with the base of VPN connections

For convenience, the base of VPN connections is divided into two dictionaries "VPN provider" and "VPN server" (in relation one-to-many). To access the dictionary "Main Menu - Dictionaries - VPN - ... " . The pre-filled country dictionary is available: "Main Menu - Dictionaries - Service - Countries"

Data entry is carried out using the toolbar or the context menu in the corresponding dictionary list. Sorting, filtering, setting of displayed columns in lists are available.

### Working with a VPN connection

All VPN connection management is carried out from the context menu of the dictionary list "VPN server" (some menu items are also duplicated in the context menu of the system tray)

Create an entry for the VPN connection - creates a Windows entry about PPTP or L2TP Connection. 

Connect to a VPN server - the VPN is connected to the previously created record ( if the entry did not exist, it will be created automatically)

Disconnect from VPN connection - disconnect from this VPN if it is active

Disconnect from active VPN connection - disconnect from any current VPN connection

Set as VPN connection by default - this VPN server is marked as the default connection. In the future, this connection will be used when double clicking on the icon in the system tray.

Run the test - in the background the connection test with the VPN is started . You can run this test: 1) as a single job for the current VPN server, 2) as a batch job for all VPN servers allocated by you in the list, 3) as a batch job for all VPN servers you marked as "Favorites", 4) as a batch job for all VPN servers from the list.

After the test is started, its status is displayed in the column of the list of VPN servers "Status of the test task". Tests in the package are performed sequentially. For each test, the following status is available in the status "Running": cancel (destroying the test flow), viewing the progress output console. For the batch job, the same features are also available for the entire job.

Note: During the test run, you access third-party network services. It is not recommended to use testing too often, so that you do not get a refusal of these resources to provide your service to your IP address.

Delete all VPN entries - deletes all entries for configured VPN connections in Windows

Import the VPN server from the site vpngate.net. Service vpngate.net is an alternative free provider of VPN services . It should be understood, as a rule, the choice, quality and reliability of free VPN servers is worse than for paid ones. Nevertheless, you can try to use this alternative. For the vpngate.net service, the PPTP and L2TP protocols are not a priority, in the first place the data service is oriented to other protocols. Also it is worth bearing in mind that the list of servers is constantly changing. The program VpnSelector implemented the procedure for importing the current list of servers for which it is stated that they support the PPTP and L2TP protocols. Imports are implemented by direct parsing of the service page with a list of available servers [https://www.vpngate.net/en/](https://translate.google.com/translate?hl=ru&prev=_t&sl=ru&tl=en&u=https://www.vpngate.net/en/) (the recommended method of importing from the service provided by the CSV service is not supported, since it does not contain information about the PPTP and L2TP protocols). During the import procedure, only new (compared to the previous import) servers are imported. They are added to the list. Previously imported servers from the vpngate.net service that are not in the current list are marked with the "Archived" attribute. In the future, if the user has not manually removed this server from the list, and with the next import this server will be found again in the list of vpngate.net servers, the "Archived" attribute will be removed.

### Exiting the program

Exit the program: The context menu of the system tray menu is "Exit"  or main menu "Close program".

Note: The "Main menu - File - Hide main program window" item only hides the main window, but does not stop the program. The program icon remains in the system tray and the program continues to work.

## Additional features of the WebAccount application

WebAccount is an extended version of the VpnSelector program that includes VpnSelector functionality, plus a password (web accounts) manager  with a embedded browser .   
 

WebAccount - the manager of bookmarks and passwords (like a popular program  KeePass ) .

Additional features of the WebAccount program that extend the functionality of VpnSelector

*   Categories (hierarchical structure), web resources, user accounts (with multiple binding to a web resource)
*   Embedded Web Browser
*   The ability to set a different User Agent for the embedded browser
*   Quickly fill in the login and password fields by clicking the buttons in the toolbar of the embeded browser.
*   Private mode. Cookie files, saved passwords, and other browser cache may maintain in isolation for each user account or a web resource

## Private mode.

Cookie files, saved passwords, and other browser cache may maintain in isolation for each user account or a web resource. There are several modes of operation of the embedded browser, which are selected when you open a web resource or user account of a web resource from the context menu.

*   by default the browser uses the shared directory for the cached files [program folder]\Data\Profile\dataCache\browserCommonCache
*   "protected mode, cache on disk" the browser uses a separate directory for each web resource [program folder]\Data\Profile\dataCache\Dionext.JWeb\[id] and for each user account of the web resource [program folder]\Data\Profile\dataCache \Dionext.FWebAccount\[id]
*   "protected mode, cache on disk (clean)" is the same, but this directory is pre-cleared before opening the browser
*   "protected mode, cache in memory" the browser uses the cache in memory

## WebAccountApp and KeePass comparation (main features)

<table border="1" width="100%">

<tbody>

<tr>

<td>Functionality</td>

<td>WebAccountApp</td>

<td>KeePass</td>

</tr>

<tr>

<td>Data Warehouse Type</td>

<td>File</td>

<td>File</td>

</tr>

<tr>

<td>Stored Entities</td>

<td>Categories (hierarchical structure), web resources, user accounts (with multiple binding to a web resource)</td>

<td>Categories (hierarchical structure), web resources (including one user account)</td>

</tr>

<tr>

<td>Multiple binding of user accounts to a web resource</td>

<td>Yes</td>

<td>No</td>

</tr>

<tr>

<td>Ability to quickly transfer a database</td>

<td>Yes</td>

<td>Yes</td>

</tr>

<tr>

<td>Portable mode (no installation required)</td>

<td>Yes</td>

<td>Yes</td>

</tr>

<tr>

<td>Secured data security by encryption</td>

<td>No (it is recommended to use protected partitions on the disk)</td>

<td>Yes</td>

</tr>

<tr>

<td>Export/Import (supported formats)</td>

<td>Json, KeePass</td>

<td>Supports for multiple formats</td>

</tr>

<tr>

<td>Embedded browser</td>

<td>Yes</td>

<td>No</td>

</tr>

<tr>

<td>The ability to set different User Agent for the embedded browser</td>

<td>Yes</td>

<td>No</td>

</tr>

<tr>

<td>Opening the URL in an external browser</td>

<td>Yes</td>

<td>Yes</td>

</tr>

<tr>

<td>Opening the URL in the embedded browser</td>

<td>Yes</td>

<td>No</td>

</tr>

<tr>

<td>Methods of auto-complete password forms</td>

<td>Quickly fill in the login and password fields by clicking the buttons in the toolbar of the embedded browser. Copy through the clipboard.</td>

<td>Use global hotkeys. Copy through the clipboard.</td>

</tr>

<tr>

<td>Multi-Language Support</td>

<td>Two languages ??are available. It is possible to add other languages ??through resource files.</td>

<td>45 language packs are available.</td>

</tr>

<tr>

<td>Plugin support</td>

<td>No</td>

<td>Yes</td>

</tr>

<tr>

<td><span class="notranslate">Free and open source software</td>

<td>Yes</td>

<td>Yes</td>

</tr>

<tr>

<td>Search and sort in lists</td>

<td>Yes</td>

<td>Yes</td>

</tr>

<tr>

<td>VPN support</td>

<td>Yes</td>

<td>No</td>

</tr>

</tbody>

</table>

## Technical information

* To manage VPN connections, VpnSelector uses the capabilities of the free <span class="notranslate">and open source DotRas library [https://archive.codeplex.com/?p=dotras](https://translate.google.com/translate?hl=ru&prev=_t&sl=ru&tl=en&u=https://archive.codeplex.com/%3Fp%3Ddotras)
* As an embedded browser, the capabilities of the free <span class="notranslate">and open source ** CefSharp** library (The CefSharp Chromium-based browser component)

## Screenshots

todo

## License

MIT License

## Contact information. 

* Email: dionextsoftware@gmail.com 
* Web: https://github.com/dionext/nettools
