<img src="https://imgur.com/3E3iPuF.jpg" title="3D-browser" alt="3D-browser">

# 3D-Browser for Windows

An internet browser like Chrome or Firefox is intended to show a text document of HTML format, which is basically a 2D content. There are technologies like WebGL that make possible to add 3D content for internet browser, but a hybrid browser, which could combine the world of text documents to the 3D simulations, has been absent until now. This is a story about an application named Tasavalta.

## Getting Started

Actually Tasavalta is not a real web browser because is is completely offline. To be honest, it is only a combined CAD and text viewer. Furthermore, Tasavalta cannot open anything but the 119 files attached to it, so Tasavalta is merely an electric book. It would be easy to make Tasavalta to open files offered by user, but because Tasavalta is missing certain properties I didin't want to allow that.

This branch is for Tasavalta for Windows. There is another branch for Tasavalta for Android also. They both share the same core: two independent libraries, one for 2D content and another for 3D. Tasavalta for Windows uses RunOpenGL.dll to render a 3D view and Teksti.dll to show a HTML document. 

Tasavalta for Windows itself is coded by C#. RunOpenGL uses OpenGL 5.0 and OpenCL 1.2 or CUDA 5.0, or if those are not available, OpenGL 1.5. Teksti utilizes Win32 API functions to draw a HTML dokument to a canvas.

Kirjasto1 includes a tiled forward rendering engine when using OpenGL ES 3.2. Kirjasto1 always uses a scenegraph to build complex view and also octree to skip everything outside the viewing frustum.

Clone everything from branch Windows New Tasavalta to your computer so you can build and develop code.

## Deployment

If you just want to install Tasavalta into your Windows computer, there's a Binary Windows.zip file in a Folder README. Unzip that file and you're ready to use Tasavalta.

## System Requirements

You need a Windows workstation which has nVidia, AMD or Intel graphics processor, Windows 7, 8, 8.1 or 10 operating system and .NET Framework 4.6 installed. You also need Microsoft Visual Studio, preferably Visual Studio 2019.

## Versioning

The GitHub account mamannon includes four repositories for application Tasavalta: two for Microsoft Windows and two for Google Android. This version here is a new version of Tasavalta for Windows.

## License

This project is licensed under the MIT License

## Acknowledgments

When I started to make Tasavalta, my initial concept was to embed a 3D view as a part into the 2D text view. Soon I realized that a better solution, or at least an easier solution to implement, is to use two independent modules inside a single application: one for 2D view and another for 3D view. As a result Tasavalta has different windows for CAD and HTML.

When I wrote code to HTML viewer (Teksti.dll), I used wxWidgets library.

And when I wrote code to CAD viewer (RunOpenGL.dll), I used OpenGL-FreeType library.