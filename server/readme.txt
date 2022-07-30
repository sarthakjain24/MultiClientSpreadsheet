The team name: 
    1265
A list of team members:
    - Andrew Osterhout (u1317172)
    - Jerry Zhang (u1253093)
    - Vijai Siddharth Vachaspathi Tunukoju Venkatapuram (u1287631)
    - Christopher Beatty (u1311628)
    - Nick Sarka (u1263335)
    - Sarthak Jain (u1240639)
Instructions for shutting down your server:
    ctrl+C
    Disconnect all clients before to ensure files save (as normally save after a time period (15-120 sec) if changes have occured, or durning sub-server shutdowns).
tldr:
    Client:
        - To update a cell, you need to hit Enter, can't click on another cell
        - Ability to create a new spreadsheet instance from under the File tab, that will create a new spreadsheet that is not connected. 
        - Cannot use the spreadsheet if it's not connected to the server.
        - After connecting to a server, select a spreadsheet file to open from the dropdown box. To open a new spreadsheet file that doesn't exist yet, type a name into the text box above it and click the "New" button.
        - Sad life about the inability to change Select Spreadsheet ComboBox color in Dark Mode (Can't do it in C# apparently).
        - Hot keys are available to undo and revert(Ctrl + Z and Ctrl + Y respectively).
    Server:
        - There is a primary control server that handles listening, and creating sub servers to handle individual spreadsheet sessions.
        - If an individual spreadsheet session sub-server is started up (by clients connecting and requesting the spreadsheet) 
           AND no client makes any requests to it with in 120sec/2min the sub-server will shutdown due to inactivity, to preserve system resources.
            - Does not time out if no request messages are received from any connected clients are made after the individual spreadsheet session sub-server starts up.
        - Sub servers that run the spreadsheet sessions will shutdown as soon as all clients disconnect.
        - History is not preserved between sessions, so you can only undo and revert changes made in the current session.
        - Server will wait a min of 15 sec between saving the spreadsheet.
        - Server will always save the spreadsheet if 2 min has passed since the last save, regardless of if changes have occured.
        - Server will save if 20 requests have been made (between all clients) to the server and the min of 15 sec has elapsed since the last save.
