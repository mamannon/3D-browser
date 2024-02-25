<img src="https://imgur.com/LDjU8fp.jpg" title="3D-browser" alt="3D-browser">

# 3D-Browser for Android

An internet browser like Chrome or Firefox is intended to show a text document of HTML format, which is basically a 2D content. There are technologies like WebGL that make possible to add 3D content into internet browser, but a hybrid browser, which could combine the world of text documents to the 3D simulations, has been absent until now. This is a story about an application named Tasavalta.

## Getting Started

Actually Tasavalta is not a real web browser because is is completely offline. To be honest, it is only a combined CAD and text viewer. Furthermore, Tasavalta cannot open anything but the 119 files attached to it, so Tasavalta is merely an electric book. It would be easy to make Tasavalta to open files offered by user, but because Tasavalta is missing certain properties I didin't want to allow that.

This branch is for Tasavalta for Android. There is another branch for Tasavalta for Windows also. They both share the same core: two independent libraries, one for 2D content and another for 3D. Tasavalta for Android uses libKirjasto1.so to render a 3D view and libKirjasto2.so to show a HTML document. 

Tasavalta for Android itself is coded by Java 1.8.0_241. Kirjasto1 uses OpenGL ES 3.2 and OpenCL 1.2, or if those are not available, OpenGL ES 2.0. Kirjasto2 operates through Java Native Interface (JNI) utilizing Android Java functions to draw a HTML dokument to a canvas.

Kirjasto1 includes a tiled forward rendering engine when using OpenGL ES 3.2. Kirjasto1 always uses a scenegraph to build complex view and also octree to skip everything outside the viewing frustum.

Clone everything from branch Android_New_Tasavalta_2 to your computer so you can build and develop code.

## Deployment

If you just want to try out Tasavalta into your Android device, there's a ready to install APK file in a path <Android_New_Tasavalta_2>/app/release/app-release.apk. Copy paste it to the memory of your Android device and tap to install. 

## System Requirements

Ideally you need two devices for development: a laptop computer and an Android phone, but only computer with an emulator is sufficient. The computer needs to have an Android Studio Hedgehog 2023.1.1 Patch 2 installed with target SDK version 34 (UpsideDownCake), Gradle 7.5 and Gradle plugin 7.4.2. The Android phone needs to have at least Android 5.0 (Lollipop).

## Versioning

The GitHub account mamannon includes five branches for application Tasavalta: two for Microsoft Windows and three for Google Android. This version here is currently the latest version of Tasavalta for Android.

## License

This project is licensed under the MIT License

## Acknowledgments

When I started to make Tasavalta, my initial concept was to embed a 3D view as a part into the 2D text view. Soon I realized that a better solution, or at least an easier solution to implement, is to use two independent modules inside a single application: one for 2D view and another for 3D view. As a result Tasavalta has different windows for CAD and HTML.

When I wrote code to HTML viewer (libKirjasto2.so), I used wxWidgets library.

And when I wrote code to CAD viewer (libKirjasto1.so), I used OpenGL-FreeType library.
