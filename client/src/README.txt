By Sarthak Jain and Dimitrius Maritsas 

3rd Oct 2020: Went over PS6Demo given by faculty. Read through the specifications for PS6. 
Designed our approach to the problem. Tried playing around with what was given.
Placed the Spreadsheet, and set its anchors, and created a File Drop Down menu with it's options.
Created the different text boxes representing the cell name, cell content and cell value.

4th Oct 2020: Implemented Functionality of the basic spreadsheet. Got the cellName, cellContents and 
cellValue to show up in the text boxes. Got a way for the user to enter the contents into the spreadsheet
through pressing the enter button.

5th Oct 2020: Implemented the functionality of the drop down menu which included how to create a new 
spreadsheet, open an existing spreadsheet, saving a spreadsheet, and closing a spreadsheet. Used sources such
as Microsoft's API to do so. (See code (Form1.cs) for actual citation link). Clarified on the delegates
such as validator, normalizer and version for the spreadsheet. Threw exceptions in Message boxes if error was
found. Also added an external feature, where if the user opens a file, then they can see the contents of the
file they opened.

6th Oct 2020: Cleaned the code a bit, removed redundant code. Added more options in the File Drop Down, added 
shortcuts for the options respectively. These short cuts can be seen by looking in the drop down file menu with the 
shortcut key bindings written next to the each button they go to. We also implemented a way to navigate through the grid using 
the arrow keys, however because of this you must click on the text box if you wish to change the location of your cursor.
To navigate through the grid using the arrow keys simply press any arrow key and the grid will move your selected cell.

7th Oct 2020: Implemented Dark Mode, by making changes to the Spreadsheet GUI and Spreadsheet Panel. Started to 
remove redundant code and comment the work we did. For the Dark mode, we implemented two radio buttons(Light mode
and Dark mode), and if one were clicked the other would not be. If the light mode button were clicked, 
the foreground colors of the text would be black, and the background would be in white. If the dark mode button
were clicked, the foreground would be orange and background would be black. The only issues we had were changing
the colors of the title and scroll bar but that seemed impossible to do so, as it would involve using a different
type of Form which we were not allowed to use.


8th Oct 2020: Finished the commenting. Found a bug with cursor in which when we moved to a new cell using the arrow keys,
cursor would be one spot to the left if we used the up and left arrow keys. To fix the bug, after doing some extensive 
googling and consulting with the course staff about that, found out the issue. The issue was due to us not returning true after
we set a change to the spreadsheet using the up, down, left, right keys.
