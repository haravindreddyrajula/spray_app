# WPF_Application

Coding Assignment
----------

	Design, code, and test a WPF application to perform the following tasks:
		1. Create a form that allows users to select and load an image (JPG, PNG, BMP, etc. ).
		2. The user will be able to spray paint the image using the mouse. Line drawing is not accepted.
		3. The user will be able to change the color and the density of the paint.
		4. The user can erase some or all of the spray using the mouse.
		5. The user will be able to save changes to a new image.
		6. The user can save the changes and close, open the application again, and edit and update the spray paint on the same image. Spray should NOT be saved on the image in this case; they should be saved into a separate file.


Stack
-----
```
-.NETFramework,Version=v4.7.2
Additionally used " Newtonsoft.Json" package
```

Usage
-----

	To launch an application you can use the `sprayapp1.exe` located `./bin/Debug`
	
Instructions on Using the Features
----------------------------------

- Load an Image
    ```
    Click the "Load Image" button to select and load an image from your local PC. It will load the image together with the "Paint" panel, "save as" and "Help" buttons.
	```
- spraying on the Image
	```
    Click and drag the mouse on the image to spray. 
	```
- Selecting color(s) & choosing thickness
	```
	Under the "spray paint tab" Choose the color you desire from the list. else default option would be "black."
	you can slide it for your desired thickness.
	```
- Erase the spray(s)
    ```
	Click the "erase" button under the "spray paint" tab. you can erase the paint as much as you want.
    Once you click the "erase" button, this button will be collapsed. and "paint" button will be enabled.
	```
- Save the Changes to a New Image
    ```
    Click the "Save As" button to save the changes, user can select the location to save in the dialog box.
    The new image will be saved to the specified location together with the metadata stored in json file with the same iamge name.
	```
- Additional Point 
	```
	A user can reload the image and can continue to work till he developed by clicking "Load Image" button.
	```
- Additional Features

	1. Help button
		If user clicks "help" button. The readme file is displayed on separate dailog box
	2. Scroll option
		To make user comfortable a "scroller" is introduced at the right.
		

Developer details
------------------
```
Haravind Reddy Rajula 
Graduate Research Assistant
Center for Real-Time Distributed Sensing and Autonomy Lab, UMBC
Computer Science Graduate | UMBC | Baltimore, MD
Ph: 667.900.2815 | E: haravindreddyrajula@gmail.com / hrajula1@umbc.edu
```