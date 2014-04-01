Audio-Walk
==========

This a Unity3D game that continuously retrieve data sent from UDP. It moves the audio listener according to the positions received on the port 12345 and rotate it
based on the rotations received on the port 11999.
It requires these two other applications: [IR-Led-Tracker application](https://github.com/simonchauvin/IR-Led-Video-Tracking) and [Android sensor application](https://github.com/simonchauvin/Android-Sensor-Data-Through-UDP)


#Installation instructions

Download and install [Visual C++ 2012 x86 Redistribuable](http://www.microsoft.com/en-us/download/details.aspx?id=30679)
Download and instal [Firefly MV FMVU-03MTM-CS camera drivers (x86 32 bits)](http://www.ptgrey.com/support/downloads/downloads_admin/Download.aspx)
Download the [IR-Led-Tracker application](https://github.com/simonchauvin/IR-Led-Video-Tracking)
Copy the FlyCapture2.dll (from the PointGrey Research folder in ProgramFiles/bin) into the bin folder of the IR-Led-Tracker application
Download the [Android sensor application](https://github.com/simonchauvin/Android-Sensor-Data-Through-UDP) and put it in an Android Phone

#Launch instructions

Connect the Android phone and the computer on the same Wifi network.
Launch the PhoneTracking application from the Android phone.
Launch the IR-Led-Tracker application.
Launch the Unity application.
You can now use an IR Led that will be tracked.
