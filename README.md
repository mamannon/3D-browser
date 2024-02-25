## PLEASE NOTICE THAT THIS REPOSITORY DOESN'T COMPLY WITH THE USUAL GIT VERSION CONTROL: THE MASTER BRANCH IS NOT IN USE.
## JUST GO STRAIGHT TO THE BRANCHES ANDROID_NEW_TASAVALTA_2 OR WINDOWS_NEW_TASAVALTA. THERE ARE THE LATEST VERSIONS.
## IF YOU DOWNLOAD A ZIP IN A MASTER BRANCH, YOU WONT'T GET ANY CODE. YOU NEED TO CHOOSE AN APPROPRIATE BRANCH.

## THE TEXT BELOW IS FOR INITIAL VERSIONS. ANDROID_NEW_TASAVALTA_2 AND WINDOWS_NEW_TASAVALTA HAVE THEIR OWN READMES.

Here are two versions of the same application, one for Microsoft Windows platform and another for Android platform. The purpose is to compare the relative performance of smartphones and laptop computers. Both versions are basically the same program, which is developed first for Windows and then ported to Android, same time editing both versions to keep the code as close as possible the same. Please keep in mind that these programs are under development phase: Windows version is (almost) stable, but Android still has some threading issues, which can cause crash when opening a CAD file. However, you can safely run them, but don't be surprised if they won't work on your machine. BTW, if you have some smart phone or computer where you cannot get these running, I'd like to hear what's your device. After all, when you have both a working Android and a Windows application, you probably get amazed how effective an average Android phone can be compared to the laptops.

The application could be described as a hybrid of an electric book, 3D CAD viewer and HTML browser. There are separate windows for viewing 3D scene and web pages. You can print documents and CAD images and mark bookmarks to open the application lately to the same position you had when you made a bookmark. You can run simulations, click links and select objects. But you cannot add your own HTML or CAD files or modify existing ones: this is prevented by a checksum.

So, the appication has two libraries, libKirjasto1.so or RunOpengl.dll and libKirjasto2.so or Teksti.dll, which are both available in binary form only. Actually, the only source code available here is the Android project belonging to the Android version of the application. You need to compile the Android project by Android Studio to get an android version of the application, but Windows version is delivered as a ready-to-use Windows installer program. 

One of the two libraries uses Opengl 5.0 and OpenCL 1.2/CUDA 5.0 in Windows or Opengl ES 3.2 and OpenCL 1.2 in Android to show a 3D scene in a window, but if your device doesn't support those, rollback to OpenGL 1.5 or OpenGL ES 2.0 happens. Another library is using API functions of the operating system, Windows or Android, to draw text and other objects to the canvas.

If you just want to see screenshots of programs without compiling or installing them, there are some pictures for you. I hope you like what you see and thank you for your time!

PS. To find the files, please click the "Branch" button above and choose "Screenshots", "Windows" or "Android" instead of "master". 
